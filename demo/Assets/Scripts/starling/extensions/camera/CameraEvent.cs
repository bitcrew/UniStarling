using UnityEngine;
using System.Collections;

using pumpkin.events;
using pumpkin.geom;

namespace starling.extensions.camera {
	
	public class CameraEvent : CEvent {
	
		public const string CAMERA_UPDATE = "CameraUpdate";
		public const string CAMERA_MOVE_COMPETE = "MoveComplete";
		
		Rectangle _data;
		
		public Rectangle data
		{
			get { return _data; } 
		}
		
		public CameraEvent(string type, bool bubbles=false, Rectangle data=null, bool cancelable=true ) :
		base( type, bubbles, cancelable )
		{
			//super(type, bubbles, cancelable);
			
			_data = data;
		}
	}
}
