// =================================================================================================
//
//  Starling Framework
//  Copyright 2011 Gamua OG. All Rights Reserved.
//
//  This program is free software. You can redistribute and/or modify it
//  in accordance with the terms of the accompanying license agreement.
//
// =================================================================================================

using UnityEngine;
using System;
using System.Collections;

using pumpkin.events;

namespace starling.animation {

	public class DelayedCall : EventDispatcher, IAnimatable {
	
		public delegate void CallbackDelegate();
		public delegate void CallbackDelegateWithArgs(object[] args);
		
		private float mCurrentTime;
        private float mTotalTime;
		private Delegate mCall;
        private object[] mArgs;
        private int mRepeatCount;
        
		
		public DelayedCall(Delegate call, float delay)
        {
			reset(call, delay, null);
        }
		
		public DelayedCall(Delegate call, float delay, object[] args)
        {
			reset(call, delay, args);
        }
		
		/** Resets the delayed call to its default values, which is useful for pooling. */
        public DelayedCall reset(Delegate call, float delay, object[] args)
        {
            mCurrentTime = 0;
            mTotalTime = Convert.ToSingle( Mathf.Max(delay, 0.0001f) );
            mCall = call;
            mArgs = args;
            mRepeatCount = 1;
            
            return this;
        }
        
        public void advanceTime(float time)
        {
        	var previousTime = mCurrentTime;
            mCurrentTime = Mathf.Min(mTotalTime, mCurrentTime + time);
            
            if (previousTime < mTotalTime && mCurrentTime >= mTotalTime)
            {
				if(mArgs == null)
				{
					mCall.DynamicInvoke(null);	
				}
				else
				{
					// needs object cast for CallbackDelegateWithArgs
					mCall.DynamicInvoke((object[])mArgs);	
				}
					
                if (mRepeatCount == 0 || mRepeatCount > 1)
                {
                    if (mRepeatCount > 0) mRepeatCount -= 1;
                    mCurrentTime = 0;
                    advanceTime((previousTime + time) - mTotalTime);
                }
                else
                {
                    dispatchEvent( new CEvent(starling.events.Event.REMOVE_FROM_JUGGLER) );
                }
            }
        }
		
		/** Indicates if enough time has passed, and the call has already been executed. */
        public Boolean isComplete
        { 
            get { return mRepeatCount == 1 && mCurrentTime >= mTotalTime; } 
        }
		
		/** The time for which calls will be delayed (in seconds). */
        public float totalTime
		{ 
			get { return mTotalTime; }
		}
		
		/** The time that has already passed (in seconds). */
        public float currentTime
		{
			get { return mCurrentTime; }
		}
		
		/** The number of times the call will be repeated. 
         *  Set to '0' to repeat indefinitely. @default 1 */
        public int repeatCount
		{ 
			get { return mRepeatCount; }
        	set { mRepeatCount = value; }
		}
		
		// callback delegate helpers
		
		public static Delegate Call( CallbackDelegate fn )
		{
			return new CallbackDelegate(fn);
		}
		
		public static Delegate CallArgs( CallbackDelegateWithArgs fn )
		{
			return new CallbackDelegateWithArgs(fn);
		}
	}
}