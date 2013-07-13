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
using System.Collections.Generic;
using System.Reflection;
using pumpkin.events;
using starling.errors;

namespace starling.animation {
	
	public class Tween : EventDispatcher, IAnimatable {
	
		public delegate void CallbackDelegate();
		public delegate void CallbackDelegateWithArgs(object[] args);
		
		private object mTarget;
        private Delegate mTransitionFunc;
        private String mTransitionName;
        
        private List<string> mProperties;
        private List<object> mStartValues;
        private List<object> mEndValues;

        private Delegate mOnStart;
        private Delegate mOnUpdate;
        private Delegate mOnRepeat;
        private Delegate mOnComplete;  
        
        private object[] mOnStartArgs;
        private object[] mOnUpdateArgs;
        private object[] mOnRepeatArgs;
        private object[] mOnCompleteArgs;
        
        private float mTotalTime;
        private float mCurrentTime;
        private float mProgress;
        private float mDelay;
        private bool mRoundToInt;
        private Tween mNextTween;
        private int mRepeatCount;
        private float mRepeatDelay;
        private bool mReverse;
        private int mCurrentCycle;
		
		
		/** Creates a tween with a target, duration (in seconds) and a transition function.
         *  @param target the object that you want to animate
         *  @param time the duration of the Tween
         *  @param transition can be either a String (e.g. one of the constants defined in the
         *         Transitions class) or a function. Look up the 'Transitions' class for a   
         *         documentation about the required function signature. */ 
        public Tween(object target, float time, object transition=null)        
        {	
			if(transition == null){ transition="linear"; }
			
			reset(target, time, transition);
        }
		
		/** Resets the tween to its default values. Useful for pooling tweens. */
        public Tween reset(object target, float time, object transition=null)
        {
            if(transition == null){ transition="linear"; }
			
			mTarget = target;
            mCurrentTime = 0.0f;
            mTotalTime = Mathf.Max(0.0001f, time);
            mProgress = 0.0f;
            mDelay = mRepeatDelay = 0.0f;
            mOnStart = mOnUpdate = mOnComplete = null;
            mOnStartArgs = mOnUpdateArgs = mOnCompleteArgs = null;
            mRoundToInt = mReverse = false;
            mRepeatCount = 1;
            mCurrentCycle = -1;
			
            if (transition is String)
                this.transition = transition as String;
            else if (transition is Delegate)
                this.transitionFunc = transition as Delegate;
            else 
                throw new ArgumentError("Transition must be either a string or a function");
			
			if(mProperties != null) mProperties.Clear(); else mProperties  = new List<string>();
            if(mStartValues != null) mStartValues.Clear(); else mStartValues = new List<object>();
            if(mEndValues != null) mEndValues.Clear(); else mEndValues   = new List<object>();
            
            return this;
        }
		
		/** Animates the property of the target to a certain value. You can call this method multiple
         *  times on one tween. */
        public void animate(string property, float endValue)
        {
            if (mTarget == null) return; // tweening null just does nothing.
                   
            mProperties.Add(property);
            mStartValues.Add(float.NaN);
            mEndValues.Add(endValue);
        }
		
		/** Animates the 'scaleX' and 'scaleY' properties of an object simultaneously. */
        public void scaleTo(float factor)
        {
            animate("scaleX", factor);
            animate("scaleY", factor);
        }
        
        /** Animates the 'x' and 'y' properties of an object simultaneously. */
        public void moveTo(float x, float y)
        {
            animate("x", x);
            animate("y", y);
        }
        
        /** Animates the 'alpha' property of an object to a certain target value. */ 
        public void fadeTo(float alpha)
        {
            animate("alpha", alpha);
        }
		
		/** @inheritDoc */
        public void advanceTime(float time)
        {
            if (time == 0 || (mRepeatCount == 1 && mCurrentTime == mTotalTime)) return;
            
            int i;
            var previousTime = mCurrentTime;
            var restTime = mTotalTime - mCurrentTime;
            var carryOverTime = time > restTime ? time - restTime : 0.0f;
            
            mCurrentTime = Mathf.Min(mTotalTime, mCurrentTime + time);
            
            if (mCurrentTime <= 0) return; // the delay is not over yet

            if (mCurrentCycle < 0 && previousTime <= 0 && mCurrentTime > 0)
            {
                mCurrentCycle++;
                if (mOnStart != null) mOnStart.DynamicInvoke(mOnStartArgs);
            }

            var ratio = mCurrentTime / mTotalTime;
            var reversed = mReverse && (mCurrentCycle % 2 == 1);
            var numProperties = mStartValues.Count;
            mProgress = reversed ? (float)mTransitionFunc.DynamicInvoke(1.0f - ratio) : (float)mTransitionFunc.DynamicInvoke(ratio);
			
            for (i=0; i<numProperties; ++i)
            {   
				PropertyInfo _prop = mTarget.GetType().GetProperty(mProperties[i]);
				
                if (float.IsNaN((float)mStartValues[i])){
					mStartValues[i] = float.Parse( _prop.GetValue(mTarget, null).ToString() );
				}
                
                var startValue = (float) mStartValues[i];
                var endValue = (float) mEndValues[i];
                var delta = endValue - startValue;
                var currentValue = startValue + mProgress * delta;
                
                if (mRoundToInt) currentValue = Mathf.Round(currentValue);
				_prop.SetValue(mTarget, currentValue, null);
            }

            if (mOnUpdate != null) 
				mOnUpdate.DynamicInvoke(mOnUpdateArgs);
            
            if (previousTime < mTotalTime && mCurrentTime >= mTotalTime)
            {
                if (mRepeatCount == 0 || mRepeatCount > 1)
                {
                    mCurrentTime = -mRepeatDelay;
                    mCurrentCycle++;
                    if (mRepeatCount > 1) mRepeatCount--;
                    if (mOnRepeat != null) mOnRepeat.DynamicInvoke(mOnRepeatArgs);
                }
                else
                {
                    // save callback & args: they might be changed through an event listener
                    Delegate onComplete = mOnComplete;
                    object[] onCompleteArgs = mOnCompleteArgs;
                    
                    // in the 'onComplete' callback, people might want to call "tween.reset" and
                    // add it to another juggler; so this event has to be dispatched *before*
                    // executing 'onComplete'.
                    dispatchEvent( new CEvent(starling.events.Event.REMOVE_FROM_JUGGLER));
                    if (onComplete != null) onComplete.DynamicInvoke(onCompleteArgs);
                }
            }
            
            if (carryOverTime > 0.0f){
                advanceTime(carryOverTime);
			}
        }
		
		/** The end value a certain property is animated to. Throws an ArgumentError if the 
         *  property is not being animated. */
        public float getEndValue(string property)
        {
            var index = mProperties.IndexOf(property);
            if (index == -1) throw new ArgumentError("The property '" + property + "' is not animated");
            else return (float)mEndValues[index];
        }
		
		/** Indicates if the tween is finished. */
        public bool isComplete
        { 
            get { return mCurrentTime >= mTotalTime && mRepeatCount == 1; }
        }        
        
        /** The target object that is animated. */
        public object target
		{ 
			get { return mTarget; }
		}
		
		/** The transition method used for the animation. @see Transitions */
        public String transition
		{ 
			get { return mTransitionName; }
        	set { 
				mTransitionName = value;
				mTransitionFunc = Transitions.getTransition(value);
				
				if (mTransitionFunc == null)
					throw new ArgumentError("Invalid transiton: " + value);
			}
        }
		
		/** The actual transition function used for the animation. */
        public Delegate transitionFunc
		{ 
			get { return mTransitionFunc; }
        	set { mTransitionName = "custom"; mTransitionFunc = value; }
        }
		
		/** The total time the tween will take per repetition (in seconds). */
        public float totalTime
		{ 
			get { return mTotalTime; }
		}
        
        /** The time that has passed since the tween was created. */
        public float currentTime
		{ 
			get { return mCurrentTime; }
		}
        
        /** The current progress between 0 and 1, as calculated by the transition function. */
        public float progress
		{
			get { return mProgress; }
		}
        
        /** The delay before the tween is started. @default 0 */
        public float delay
		{ 
			get { return mDelay; }
        	set { mCurrentTime = mCurrentTime + mDelay - value; mDelay = value; }
        }
		
		/** The number of times the tween will be executed. 
         *  Set to '0' to tween indefinitely. @default 1 */
        public int repeatCount
		{ 
			get { return mRepeatCount; }
        	set { mRepeatCount = value; }
		}
        
        /** The amount of time to wait between repeat cycles, in seconds. @default 0 */
        public float repeatDelay
		{ 
			get { return mRepeatDelay; }
        	set { mRepeatDelay = value; }
		}
		
		/** Indicates if the tween should be reversed when it is repeating. If enabled, 
         *  every second repetition will be reversed. @default false */
        public bool reverse
		{ 
			get { return mReverse; }
        	set { mReverse = value; }
		}
        
        /** Indicates if the numeric values should be cast to Integers. @default false */
        public bool roundToInt
		{ 
			get { return mRoundToInt; }
        	set { mRoundToInt = value; }
		}
		
		/** A function that will be called when the tween starts (after a possible delay). */
        public Delegate onStart
		{ 
			get { return mOnStart; }
        	set { mOnStart = value; }
		}
        
        /** A function that will be called each time the tween is advanced. */
        public Delegate onUpdate
		{ 
			get { return mOnUpdate; }
        	set { mOnUpdate = value; }
		}
        
        /** A function that will be called each time the tween finishes one repetition
         *  (except the last, which will trigger 'onComplete'). */
        public Delegate onRepeat
		{ 
			get { return mOnRepeat; }
        	set { mOnRepeat = value; }
		}
		
		/** A function that will be called when the tween is complete. */
        public Delegate onComplete
		{ 
			get { return mOnComplete; }
        	set { mOnComplete = value; }
		}
        
        /** The arguments that will be passed to the 'onStart' function. */
        public object[] onStartArgs
		{ 
			get { return mOnStartArgs; }
        	set { mOnStartArgs = value; }
		}
        
        /** The arguments that will be passed to the 'onUpdate' function. */
        public object[] onUpdateArgs
		{ 
			get { return mOnUpdateArgs; }
        	set { mOnUpdateArgs = value; }
		}
        
        /** The arguments that will be passed to the 'onRepeat' function. */
        public object[] onRepeatArgs
		{ 
			get { return mOnRepeatArgs; }
        	set { mOnRepeatArgs = value; }
		}
        
        /** The arguments that will be passed to the 'onComplete' function. */
        public object[] onCompleteArgs
		{ 
			get { return mOnCompleteArgs; }
        	set { mOnCompleteArgs = value; }
		}
        
        /** Another tween that will be started (i.e. added to the same juggler) as soon as 
         *  this tween is completed. */
        public Tween nextTween
		{ 
			get { return mNextTween; }
        	set { mNextTween = value; }
		}
		
		// tween pooling
        
        private static Stack<Tween> sTweenPool = new Stack<Tween>();
        
        /** @private */
        public static Tween fromPool(object target, float time, object transition=null)
        {	
			if(transition == null){ transition="linear"; }
				
			if (sTweenPool.Count > 0) return sTweenPool.Pop().reset(target, time, transition);
            else return new Tween(target, time, transition);
        }
        
        /** @private */
        public static void toPool(Tween tween)
        {
            // reset any object-references, to make sure we don't prevent any garbage collection
            tween.mOnStart = tween.mOnUpdate = tween.mOnRepeat = tween.mOnComplete = null;
            tween.mOnStartArgs = tween.mOnUpdateArgs = tween.mOnRepeatArgs = tween.mOnCompleteArgs = null;
            tween.mTarget = null;
            tween.mTransitionFunc = null;
            tween.removeAllEventListeners(starling.events.Event.REMOVE_FROM_JUGGLER);
			sTweenPool.Push(tween);
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
		
		public static TweenParams Params()
		{
			return new TweenParams();
		}
	}
}
