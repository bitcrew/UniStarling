using UnityEngine;
using System.Collections;

using pumpkin.geom;

namespace starling.extensions.camera {
	
	public static class CameraUtils {
	
		public static float clamp(float value, float min, float max)
		{
			return Mathf.Max(min, Mathf.Min(max, value));
		}
		
		public static float angleModulus(float value, float rotation)
		{
			while (value < -Mathf.PI)
				value+=Mathf.PI * 2;
			while (value > Mathf.PI)
				value-=Mathf.PI * 2;
			
			return (value * rotation) < 0 && Mathf.Abs(value - rotation) > Mathf.PI ? value + Mathf.PI * 2 * ((rotation > 0) ? 1 : -1) : value;
		}
		
		public static float ease(float current, float target, float easeAmount)
		{
			return current + (target - current) * easeAmount;
		}
		
		public static bool nearEquals(float a, float b, float tolerance=0.1f)
		{
			return Mathf.Abs(a - b) < tolerance;
		}
		
		public static Rectangle inflate(Rectangle rect, float x, float y)
		{
		    rect.x -= x;
		    rect.y -= y;
		    rect.width += x * 2;
		    rect.height += y * 2;
			
			return rect;
		}
		
		public static Rectangle inflatePoint(Rectangle rect, Point pt)
		{
		    rect.x -= pt.x;
		    rect.y -= pt.y;
		    rect.width += pt.x * 2;
		    rect.height += pt.y * 2;
			
			return rect;
		}
	}
}