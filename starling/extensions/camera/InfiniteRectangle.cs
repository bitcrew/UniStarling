using UnityEngine;
using System.Collections;
using System;
using pumpkin.geom;

namespace starling.extensions.camera {
	
	public class InfiniteRectangle {
		
		static InfiniteRectangle FULL_RECT() { return new InfiniteRectangle(float.NegativeInfinity, float.NegativeInfinity, float.PositiveInfinity, float.PositiveInfinity); }
		
		Rectangle rect;
		
		public InfiniteRectangle(float x=0, float y=0, float width=0, float height=0)
		{
			rect = new Rectangle(x, y, width, height);
		}
		
		public Rectangle intersection(Rectangle toIntersect)
		{
			float l = CameraUtils.clamp(toIntersect.left, rect.left, rect.right);
			float r = CameraUtils.clamp(toIntersect.right, rect.left, rect.right);
			float t = CameraUtils.clamp(toIntersect.top, rect.top, rect.bottom);
			float b = CameraUtils.clamp(toIntersect.bottom, rect.top, rect.bottom);
			
			if (t >= b || l >= r) // don't bother rendering if the texture is off screen
			{
				//return new InfiniteRectangle();
				return new Rectangle(0, 0, 0, 0);
			}
			else
			{
				//return new InfiniteRectangle(l, t, r - l, b - t);
				return asRectangle( new InfiniteRectangle(l, t, r - l, b - t) );
			}
		}
		
		public Rectangle asRectangle(InfiniteRectangle irect)
		{
			return irect.toRectangle();
		}
		
		public Rectangle toRectangle()
		{
			return new Rectangle(rect.rect);
		}
		
		public Rect toRect()
		{
			return rect.rect;
		}
		
		public InfiniteRectangle clone()
		{
			return new InfiniteRectangle(rect.x, rect.y, rect.width, rect.height);
		}
		
		public float right()
		{
			return rect.left == float.NegativeInfinity ? rect.width : rect.right;
		}
		
		public float bottom()
		{
			return rect.top == float.NegativeInfinity ? rect.height : rect.bottom;
		}
		
		public Point bottomRight()
		{
			return new Point(rect.bottom, rect.right);
		}
		
		public Point size()
		{
			return new Point(rect.width, rect.height);
		}
			
		public float x()
		{
			return rect.x;
		}
			
		public float y()
		{
			return rect.y;
		}
		
		public float width()
		{
			return rect.width;
		}
			
		public float height()
		{
			return rect.height;
		}
	}
}