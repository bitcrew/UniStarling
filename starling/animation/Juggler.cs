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

using starling.errors;

using pumpkin.events;

namespace starling.animation {
	
	public class Juggler : IAnimatable {
		
		List<IAnimatable> mObjects;
	    float mElapsedTime;
		
		/** Create an empty juggler. */
		public Juggler()
		{
			mElapsedTime = 0;
	        mObjects = new List<IAnimatable>();
		}
		
		/** Adds an object to the juggler. */
	    public void add(IAnimatable obj)
	    {
	        if (obj != null && mObjects.IndexOf(obj) == -1) 
	        {
	            mObjects.Add(obj);
	        
	            var dispatcher = obj as EventDispatcher;
	            if (dispatcher != null) dispatcher.addEventListener(starling.events.Event.REMOVE_FROM_JUGGLER, onRemove);
	        }
	    }
		
		/** Determines if an object has been added to the juggler. */
	    public bool contains(IAnimatable obj)
	    {
	        return mObjects.IndexOf(obj) != -1;
	    }
		
		/** Removes an object from the juggler. */
	    public void remove(IAnimatable obj)
	    {
	        if (obj == null) return;
	        
	        var dispatcher = obj as EventDispatcher;
	        if (dispatcher != null) dispatcher.removeEventListener(starling.events.Event.REMOVE_FROM_JUGGLER, onRemove);
	
	        var index = mObjects.IndexOf(obj);
	        if (index != -1) mObjects[index] = null;
	    }
		
		/** Removes all tweens with a certain target. */
	    public void removeTweens(object target)
	    {
	        if (target == null) return;
	        
	        for (var i=mObjects.Count-1; i>=0; --i)
	        {
	            var tween = mObjects[i] as Tween;
	            if (tween != null && tween.target == target)
	            {
	                tween.removeEventListener(starling.events.Event.REMOVE_FROM_JUGGLER, onRemove);
	                mObjects[i] = null;
	            }
	        }
	    }
		
		/** Figures out if the juggler contains one or more tweens with a certain target. */
	    public bool containsTweens(object target)
	    {
	        if (target == null) return false;
	        
	        for (var i=mObjects.Count-1; i>=0; --i)
	        {
	            var tween = mObjects[i] as Tween;
	            if (tween != null && tween.target == target) return true;
	        }
	        
	        return false;
	    }
		
		/** Removes all objects at once. */
	    public void purge()
	    {
	        // the object vector is not purged right away, because if this method is called 
	        // from an 'advanceTime' call, this would make the loop crash. Instead, the
	        // vector is filled with 'null' values. They will be cleaned up on the next call
	        // to 'advanceTime'.
	        
	        for (var i=mObjects.Count-1; i>=0; --i)
	        {
	            var dispatcher = mObjects[i] as EventDispatcher;
	            if (dispatcher != null) dispatcher.removeEventListener(starling.events.Event.REMOVE_FROM_JUGGLER, onRemove);
	            mObjects[i] = null;
	        }
	    }
		
		/** Delays the execution of a function until a certain time has passed. Creates an
	     *  object of type 'DelayedCall' internally and returns it. Remove that object
	     *  from the juggler to cancel the function call. */
	    public DelayedCall delayCall(DelayedCall.CallbackDelegate call, float delay)
	    {
	        if (call == null) return null;
	        
	        var delayedCall = new DelayedCall(new DelayedCall.CallbackDelegate(call), delay);
	        add(delayedCall);
	        return delayedCall;
	    }
		
		public DelayedCall delayCall(DelayedCall.CallbackDelegateWithArgs call, float delay, object[] args)
	    {
	        if (call == null) return null;
			
	        var delayedCall = new DelayedCall(new DelayedCall.CallbackDelegateWithArgs(call), delay, args);
	        add(delayedCall);
	        return delayedCall;
	    }
		
		public DelayedCall delayCall(Delegate call, float delay, object[] args=null)
	    {
	        if (call == null) return null;
			
	        var delayedCall = new DelayedCall(call, delay, args);
	        add(delayedCall);
	        return delayedCall;
	    }
		
		/** Utilizes a tween to animate the target object over a certain time. Internally, this
         *  method uses a tween instance (taken from an object pool) that is added to the
         *  juggler right away. This method provides a convenient alternative for creating 
         *  and adding a tween manually.
         *  
         *  <p>Fill 'properties' with key-value pairs that describe both the 
         *  tween and the animation target. Here is an example:</p>
         *  
         *  <pre>
         *  juggler.tween(object, 2.0, {
         *      transition: Transitions.EASE_IN_OUT,
         *      delay: 20, // -> tween.delay = 20
         *      x: 50      // -> tween.animate("x", 50)
         *  });
         *  </pre> 
         */
        public void tween(object target, float time, TweenParams properties)
        {
            Tween tween = Tween.fromPool(target, time);
			
			foreach (var prop in properties)
			{		
                if ( hasProperty(tween, prop.Key) )
					SetValue(tween, prop.Key, prop.Value);
				else if ( hasProperty(target, prop.Key) )
                   	tween.animate(prop.Key, (float)prop.Value);
				else
                    throw new ArgumentError("Invalid property: " + prop.Key);
			}
            
            tween.addEventListener(starling.events.Event.REMOVE_FROM_JUGGLER, onPooledTweenComplete);
            add(tween);
        }
		
		private bool hasProperty(object objectToCheck, string propName)
		{
			var type = objectToCheck.GetType();
    		return type.GetProperty(propName) != null;
		}
		
		private static void SetValue(object inputObject, string propertyName, object propertyVal)
		{
		    //find out the input object type
		    Type type = inputObject.GetType();
		 
		    //get the property information based on the type
		    System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);
		 
		    //Set the value of the property
		    propertyInfo.SetValue(inputObject, propertyVal, null);
		}

		
		private void onPooledTweenComplete(CEvent e)
        {
            Tween.toPool(e.target as Tween);
        }
		
		/** Advances all objects by a certain time (in seconds). */
        public void advanceTime(float time)
        {   
            int numObjects = mObjects.Count;
            //int currentIndex = 0;
            int i;
            
            mElapsedTime += time;
            if (numObjects == 0) return;
            
            // there is a high probability that the "advanceTime" function modifies the list 
            // of animatables. we must not process new objects right now (they will be processed
            // in the next frame), and we need to clean up any empty slots in the list.
            
            for (i=0; i<numObjects; ++i)
            {
                IAnimatable obj = mObjects[i];
                if (obj != null)
                {
                    obj.advanceTime(time);
                    //++currentIndex;
                }
            }
        }
		
		private void onRemove(CEvent e)
        {
            remove(e.target as IAnimatable);
            
            var tween = e.target as Tween;
            if (tween != null && tween.isComplete)
                add(tween.nextTween);
        }
        
        /** The total life time of the juggler. */
        public float elapsedTime
		{ 
			get { return mElapsedTime; }
		}
	}
}
