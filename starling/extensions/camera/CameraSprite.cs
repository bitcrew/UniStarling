using UnityEngine;
using System.Collections;

using pumpkin.events;
using pumpkin.display;
using pumpkin.geom;
using pumpkin.tweener;

using starling.animation;

namespace starling.extensions.camera {
	
	public class CameraSprite : DisplayObjectContainer {
	
		public static InfiniteRectangle NO_BOUNDS = new InfiniteRectangle(float.NegativeInfinity, float.NegativeInfinity, float.PositiveInfinity, float.PositiveInfinity);
			
		/**
		 * A value to use if the camera should track the targets without easing.
		 * @default 1
		 */
		public const float NO_EASING=1;
		/**
		 * A value to use if the camera will not track the target settings at all.
		 * @default 0
		 */
		public const float NO_MOTION=0f;
		public const float CAMERA_SHAKE_INTERVAL = 0.016666666f;
		
		private Point _startPosition;
		private Rectangle _boundingRect;
		
		private float _easingPan;
		private float _easingZoom;
		private float _easingRotate;
		
		private float _targetX;
		private float _targetY;
		private float _targetZoom;
		private float _targetRot;
		private DelayedCall _shaker;
		private Juggler _juggler;
		
		private bool _shaking=false;
		private bool _moving=false;
		
		private Rectangle _viewport;
		
		private Harness _harness;
		private World _world;
		private System.Random rand = new System.Random();
		private delegate void shakeDelegate(float i);
		private Tween _cameraMoveTween;
		private Tween _cameraZoomTween;
		
		
		/**
		 * The Point for with the location that the camera would return to if reset.
		 * @return Point for with the location that the camera would return to if reset. 
		 */
		public Point startPosition
		{
			get { return _startPosition; }
			set { if(value == null) { _startPosition = value; }
				  else{ _startPosition = new Point(); }
			}
		}
		//public function set startPosition(value:Point):void  { value ? _startPosition=value : new Point(); }
		
		/**
		 * A Rectangle that constrains the movement of the camera
		 * @return  Rectangle of the bounding box
		 */
		public Rectangle boundingRect
		{
			get { return _boundingRect; }
			set { _boundingRect=value; }
		}
		
		/**
		 * The value used to determine how much the camera should move each step towards 
		 * its target in X and Y.
		 * @return Number of the pan easing value
		 */
		public float easingPan
		{
			get { return _easingPan; }
			set { _easingPan=CameraUtils.clamp(value, 0, 1); }
		}
		
		/**
		 * The value used to determine how much the camera should move each step towards 
		 * its target for scale.
		 * @return Number of the scale easing value
		 */
		public float easingZoom
		{
			get { return _easingZoom; }
			set { _easingZoom=CameraUtils.clamp(value, 0, 1); }
		}
		
		/**
		 * The value used to determine how much the camera should move each step towards 
		 * its target for rotation.
		 * @return Number of the rotation easing value
		 */
		public float easingRotate
		{
			get { return _easingRotate; }
			set { _easingRotate=CameraUtils.clamp(value, 0, 1); }
		}
		
		/**
		 * The current viewable area of the camera, taking into account position, rotation and scale
		 * @return Rectangle with the camera viewable area.
		 */
		public Rectangle viewport
		{
			get { return getWorldViewableArea(); }
		}
		
		
		/**
		 * A display object rig to access pan, rotation and zoom actions.
		 * @param viewport the starting viewable rectangle including position
		 * @param startPosition Point reprsenting the position that the camera will return to on reset
		 * @param easingPan The value used to determine how much the camera should move each step towards its target in X and Y.
		 * @param easingZoom The value used to determine how much the camera should move each step towards its target for scale.
		 * @param easingRotate The value used to determine how much the camera should move each step towards its target for rotation.
		 * @param boundingRect A Rectangle that constrains the movement of the camera
		 */
		public CameraSprite(Rectangle viewport, Point startPosition=null, float easingPan=1f, float easingZoom=1f, float easingRotate=1, Rectangle boundingRect=null, Juggler juggler=null)
		{
			_juggler = juggler;
			
			_harness=new Harness();  // The harness is a container to manage rotation, scale and shaking without modifying the pivot and other properties of the main camera.
			_world=new World(this);
			
			_viewport=viewport;
			
			this.startPosition=startPosition;
			
			_easingPan=easingPan;
			_easingZoom=easingZoom;
			_easingRotate=easingRotate;
			
			if (boundingRect != null)
			{
				this.boundingRect=boundingRect;
			}
			else
			{
				this.boundingRect=NO_BOUNDS.toRectangle();
			}
			
			_harness.center=new Point(_viewport.x + _viewport.width / 2, _viewport.y + _viewport.height / 2);
			base.addChildAt(_harness, 0);
			_harness.addChild(_world);
		}
		
		/**
		 * Reset the camera position, scale and rotation to their initial values
		 */
		public void reset()
		{
			moveTo(_startPosition.x, _startPosition.y, 1, 0, true);
			stopShake();
		}
		
		/**
		 * Set the position, scale and rotation targets for the camera and begin animation.
		 * @param xPos X Pan target value
		 * @param yPos Y Pan target value
		 * @param zoom Scale target value
		 * @param rot Rotation rarget value
		 * @param force Boolean to force the values to be set without easing or animation
		 */
		public void moveTo(float xPos, float yPos, float zoom=1f, float rot=0f, bool force=false)
		{
			if (force || _easingPan != NO_MOTION)
			{
				_targetX=-CameraUtils.clamp(xPos, _boundingRect.left, _boundingRect.right);
				_targetY=-CameraUtils.clamp(yPos, _boundingRect.top, _boundingRect.bottom);
			}
			if (force || _easingZoom != NO_MOTION)
			{
				_targetZoom=CameraUtils.clamp(zoom, 0, float.PositiveInfinity);
			}
			if (force || _easingRotate != NO_MOTION)
			{
				_targetRot=rot;
			}
			
			if (force)
			{
				_world.x=_targetX;
				_world.y=_targetY;
				_harness.scaleX=_harness.scaleY=_targetZoom;
				_harness.setRotation(_targetRot);
			}
			else
			{
				startMoving();
			}
		}
		
		/**
		 * A convenience function so other methods can start the animation when needed without having to replicate the condition
		 */
		private void startMoving()
		{
			if (!_moving)
			{
				_moving=true;
				addEventListener(CEvent.ENTER_FRAME, stepAnimation);
			}
		}
		
		/**
		 * Stop animation for moving to position, scale and rotation targets
		 */
		private void stopMoving()
		{
			_moving=false;
			removeEventListener(CEvent.ENTER_FRAME, stepAnimation);
			dispatchEvent( new CameraEvent( CameraEvent.CAMERA_MOVE_COMPETE ));
		}
		
		/**
		 * To be used as a handler for onEnterFrame.  Moves the camera towards its targets 
		 * @param e EnterFrameEvent
		 */
		private void stepAnimation(CEvent e)
		{
			//clamp, ease, angleModulus and nearEquals are from com.byxb.utils.
			_world.x=CameraUtils.clamp(CameraUtils.ease(_world.x, _targetX, _easingPan), _boundingRect.left, _boundingRect.right);
			_world.y=CameraUtils.clamp(CameraUtils.ease(_world.y, _targetY, _easingPan), _boundingRect.top, _boundingRect.bottom);
			_harness.scaleX=_harness.scaleY=CameraUtils.ease(_harness.scaleX, _targetZoom, _easingZoom);
			_targetRot=CameraUtils.angleModulus(_targetRot, _harness.getRotation());
			_harness.setRotation(CameraUtils.ease(_harness.getRotation(), _targetRot, _easingRotate));
			//_juggler.advanceTime(e.passedTime);
			_harness.updateRotation();
			
			//if pretty close to all the targets, treat as having reached the targets
			if (!_shaking && CameraUtils.nearEquals(_world.x, _targetX) && CameraUtils.nearEquals(_world.y, _targetY) && CameraUtils.nearEquals(_harness.scaleX, _targetZoom) && CameraUtils.nearEquals(_harness.getRotation(), _targetRot))
			{
				stopMoving();
			}
			//dispatchEvent(CameraEvent.CAMERA_UPDATE, false, this.viewport);
			dispatchEvent( new CameraEvent( CameraEvent.CAMERA_UPDATE, false, this.viewport, false ) );
		}
		
		/**
		 * Returns a bounding box representing the area that needs to be displayed based on scale, rotation and position.
		 * Rotation is not currently working.
		 * @return Rectangle of workd bounding box
		 */
		private Rectangle getWorldViewableArea()
		{
			//FIXME: Rectangle roation
			Rectangle worldView = new Rectangle(-_world.x + _harness.center.x - _harness.x, -_world.y + _harness.center.y - _harness.y, 0, 0);
			Point inflate = new Point((1 / _harness.scaleX * _viewport.width) / 2, (1 / _harness.scaleY * _viewport.height) / 2);
			return CameraUtils.inflatePoint(worldView, inflate);
		}
		
		/**
		 * Shakes the camera harness 
		 * @param intensity How much to shake the camera (max radius for randomization off center)
		 * @param pulses How make shake pulses to do before the shake effect stops.
		 */
		public void shake(float intensity=5f, int pulses=5)
		{
			if (pulses < 1)
			{
				return;
			}
			
			object[] shakeArgs = new object[]{ intensity };
			_shaker = _juggler.delayCall(new shakeDelegate(doShake), CAMERA_SHAKE_INTERVAL, shakeArgs);
			_shaker.repeatCount=pulses + 1;
			_juggler.add(_shaker);
			_shaking=true;
			startMoving();
		}
		
		/**
		 * halts the shake action
		 */
		public void stopShake()
		{
			_harness.x=_harness.center.x;
			_harness.y=_harness.center.y;
			_harness.rotation=_harness.getRotation();
			_juggler.remove(_shaker);
			_shaking=false;
		}
		
		/**
		 * runs each pulse
		 * @param intensity
		 */
		private void doShake(float intensity)
		{	
			Point pt = Point.polar( (float)rand.NextDouble() * intensity, (float)rand.NextDouble() * 2f * Mathf.PI );
			_harness.x=Mathf.Floor(_harness.center.x + pt.x);
			_harness.y=Mathf.Floor(_harness.center.y + pt.y);
			_harness.rotation=_harness.getRotation() + ( (float)rand.NextDouble() * 2f - 1f) * intensity * .005f;
			if (_shaker.repeatCount == 1)
			{
				stopShake();
			}
		}
		
		public void moveTween2(float tweenTime, float xPos, float yPos, float zoom=1, bool round=false)
		{				
			_cameraMoveTween = new Tween(_world, tweenTime);
			_cameraMoveTween.animate("x", -xPos);
			_cameraMoveTween.animate("y", -yPos);
			_cameraMoveTween.transition = Transitions.EASE_OUT;
			_cameraMoveTween.onComplete = Tween.Call(tweenComplete);
			_cameraMoveTween.roundToInt = round;
			
			_cameraZoomTween = new Tween(_harness, tweenTime);
			_cameraZoomTween.animate("scaleX", zoom);
			_cameraZoomTween.animate("scaleY", zoom);
			_cameraZoomTween.transition = Transitions.EASE_OUT;
			_cameraZoomTween.roundToInt = false;
			
			_juggler.add(_cameraMoveTween);
			_juggler.add(_cameraZoomTween);
		}
		
		private void tweenComplete()
		{
			_juggler.remove(_cameraMoveTween);
			_juggler.remove(_cameraZoomTween);
			
			//dispatchEventWith(CameraEvent.CAMERA_MOVE_COMPETE);
			dispatchEvent( new CameraEvent( CameraEvent.CAMERA_MOVE_COMPETE ));
		}
		
		// The overrides are here to block access to the harness from the outside, even from the children.
		public DisplayObject addChild2(DisplayObject child)
		{
			_world.addChild(child);
			return child;
		}
		
		public DisplayObject addChildAt2(DisplayObject child, int index)
		{
			_world.addChildAt(child, index);
			return child;
		}
		
		public DisplayObject removeChild2(DisplayObject child, bool dispose=false)
		{
			_world.removeChild(child);
			return child;
		}
		
		public DisplayObject removeChildAt2(int index, bool dispose=false)
		{
			DisplayObject child;
			child = _world.getChildAt(index);
			_world.removeChildAt(index);
			return child;
		}
		
		public void removeChildren(int beginIndex=0, int endIndex=-1, bool dispose=false)
		{
			//_world.removeChildren(beginIndex, endIndex);
		}
		
		public void removeAllChildren2(int beginIndex=0, int endIndex=-1, bool dispose=false)
		{
			_world.removeAllChildren();
		}
		
		public int numChildren2()
		{
			return _world.numChildren;
		}
	}
	
	
	
	internal class Harness : Sprite
	{
		Point _center;
		float _rotation = 0;
		
		/**
		 * the actual center.  This is needed for resetting after shaking.
		 * @return center Point
		 */
		public Point center
		{
			get{ return _center; }
			set{ _center = value; x = value.x; y = value.y; }
		}
		
		public Harness()
		{
			
		}
		
		/**
		 * Set the cannonical rotation.  Actual rotation will be affected by shaking.
		 * @param rotation
		 */
		public void setRotation(float rotation)
		{
			_rotation=rotation;
		}
		
		/**
		 * get the cannonical rotaiton. Actual rotation will be affected by shaking.
		 * @return 
		 */
		public float getRotation()
		{
			return _rotation;
		}
		
		/**
		 * set the actual rotation to the cannonical rotation
		 */
		public void updateRotation()
		{
			this.rotation=_rotation;
		}
		
	}
	
	
	
	internal class World : Sprite
	{
		public World(CameraSprite camera)
		{
			
		}
	}
	
}
