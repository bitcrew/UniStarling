using System;

namespace starling.animation{
	
	public interface IAnimatable
	{
		/** Advance the time by a number of seconds. @param time in seconds. */
		void advanceTime(float time);
	}
}