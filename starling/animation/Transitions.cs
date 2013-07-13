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

using pumpkin.events;

namespace starling.animation {
	
	/** The Transitions class contains static methods that define easing functions. 
     *  Those functions are used by the Tween class to execute animations.
     * 
     *  <p>Here is a visual representation of the available transitions:</p> 
     *  <img src="http://gamua.com/img/blog/2010/sparrow-transitions.png"/>
     *  
     *  <p>You can define your own transitions through the "registerTransition" function. A 
     *  transition function must have the following signature, where <code>ratio</code> is 
     *  in the range 0-1:</p>
     *  
     *  <pre>function myTransition(ratio:Number):Number</pre>
     */
	public class Transitions {
		
		public const string LINEAR = "linear";
		public const string EASE_IN = "easeIn";
		public const string EASE_OUT = "easeOut";
		public const string EASE_IN_OUT = "easeInOut";
		public const string EASE_OUT_IN = "easeOutIn";        
		public const string EASE_IN_BACK = "easeInBack";
		public const string EASE_OUT_BACK = "easeOutBack";
		public const string EASE_IN_OUT_BACK = "easeInOutBack";
		public const string EASE_OUT_IN_BACK = "easeOutInBack";
		public const string EASE_IN_ELASTIC = "easeInElastic";
		public const string EASE_OUT_ELASTIC = "easeOutElastic";
		public const string EASE_IN_OUT_ELASTIC = "easeInOutElastic";
		public const string EASE_OUT_IN_ELASTIC = "easeOutInElastic";  
		public const string EASE_IN_BOUNCE = "easeInBounce";
		public const string EASE_OUT_BOUNCE = "easeOutBounce";
		public const string EASE_IN_OUT_BOUNCE = "easeInOutBounce";
		public const string EASE_OUT_IN_BOUNCE = "easeOutInBounce";
		public const string EASE_IN_SINE = "easeInSine";
		public const string EASE_OUT_SINE = "easeOutSine";
		public const string EASE_IN_OUT_SINE = "easeInOutSine";
		
		//private static Dictionary sTransitions;
		private static Dictionary<string, Delegate> sTransitions;
		private delegate float transitionDelegate(float ratio);
		private const float _HALF_PI = Mathf.PI / 2;
		
		/** @private */
		public Transitions() { /*throw new AbstractClassError();*/ }
		
		/** Returns the transition function that was registered under a certain name. */ 
		public static Delegate getTransition(string name)
		{
		    if (sTransitions == null) registerDefaults();
		    return sTransitions[name];
		}
		
		/** Registers a new transition function under a certain name. */
		public static void register(string name, Delegate func)
		{
		    if (sTransitions == null) registerDefaults();
		    sTransitions[name] = func;
		}
		
		private static void registerDefaults()
		{
		    sTransitions = new Dictionary<string, Delegate>();
		    
		    register(LINEAR, new transitionDelegate(linear));
		    register(EASE_IN, new transitionDelegate(easeIn));
		    register(EASE_OUT, new transitionDelegate(easeOut));
		    register(EASE_IN_OUT, new transitionDelegate(easeInOut));
			register(EASE_OUT_IN, new transitionDelegate(easeOutIn));
			register(EASE_IN_BACK, new transitionDelegate(easeInBack));
			register(EASE_OUT_BACK, new transitionDelegate(easeOutBack));
			register(EASE_IN_OUT_BACK, new transitionDelegate(easeInOutBack));
			register(EASE_OUT_IN_BACK, new transitionDelegate(easeOutInBack));
			register(EASE_IN_ELASTIC, new transitionDelegate(easeInElastic));
			register(EASE_OUT_ELASTIC, new transitionDelegate(easeOutElastic));
			register(EASE_IN_OUT_ELASTIC, new transitionDelegate(easeInOutElastic));
			register(EASE_OUT_IN_ELASTIC, new transitionDelegate(easeOutInElastic));
			register(EASE_IN_BOUNCE, new transitionDelegate(easeInBounce));
			register(EASE_OUT_BOUNCE, new transitionDelegate(easeOutBounce));
			register(EASE_IN_OUT_BOUNCE, new transitionDelegate(easeInOutBounce));
			register(EASE_OUT_IN_BOUNCE, new transitionDelegate(easeOutInBounce));
			register(EASE_IN_SINE, new transitionDelegate(easeInSine));
			register(EASE_OUT_SINE, new transitionDelegate(easeOutSine));
			register(EASE_IN_OUT_SINE, new transitionDelegate(easeInOutSine));
		}
		
		// transition functions
		
		protected static float linear(float ratio)
		{
		    return ratio;
		}
		
		protected static float easeIn(float ratio)
		{
		    return ratio * ratio * ratio;
		}
		
		protected static float easeOut(float ratio)
		{
		    var invRatio = ratio - 1.0f;
		    return invRatio * invRatio * invRatio + 1;
		}
		
		protected static float easeInOut(float ratio)
		{
		    return easeCombined(new transitionDelegate(easeIn), new transitionDelegate(easeOut), ratio);
		}   
		
		protected static float easeOutIn(float ratio)
		{
		    return easeCombined(new transitionDelegate(easeOut), new transitionDelegate(easeIn), ratio);
		}
		
		protected static float easeInBack(float ratio)
		{
		    var s = 1.70158f;
		    return Mathf.Pow(ratio, 2f) * ((s + 1.0f)*ratio - s);
		}
		
		protected static float easeOutBack(float ratio)
		{
		    var invRatio = ratio - 1.0f;            
		    var s = 1.70158f;
		    return Mathf.Pow(invRatio, 2f) * ((s + 1.0f)*invRatio + s) + 1.0f;
		}
		
		protected static float easeInOutBack(float ratio)
		{
		    return easeCombined(new transitionDelegate(easeInBack), new transitionDelegate(easeOutBack), ratio);
		}   
		
		protected static float easeOutInBack(float ratio)
		{
		    return easeCombined(new transitionDelegate(easeOutBack), new transitionDelegate(easeInBack), ratio);
		}        
		
		protected static float easeInElastic(float ratio)
		{
		    if (ratio == 0f || ratio == 1f) return ratio;
		    else
		    {
		        var p = 0.3f;
		        var s = p/4.0f;
		        var invRatio = ratio - 1f;
		        return -1.0f * Mathf.Pow(2.0f, 10.0f*invRatio) * Mathf.Sin((invRatio-s)*(2.0f*Mathf.PI)/p);                
		    }            
		}
		
		protected static float easeOutElastic(float ratio)
		{
		    if (ratio == 0 || ratio == 1) return ratio;
		    else
		    {
		        var p = 0.3f;
		        var s = p/4.0f;                
		        return Mathf.Pow(2.0f, -10.0f*ratio) * Mathf.Sin((ratio-s)*(2.0f*Mathf.PI)/p) + 1f;                
		    }            
		}
		
		protected static float easeInOutElastic(float ratio)
		{
		    return easeCombined(new transitionDelegate(easeInElastic), new transitionDelegate(easeOutElastic), ratio);
		}   
		
		protected static float easeOutInElastic(float ratio)
		{
		    return easeCombined(new transitionDelegate(easeOutElastic), new transitionDelegate(easeInElastic), ratio);
		}
		
		protected static float easeInBounce(float ratio)
		{
		    return 1.0f - easeOutBounce(1.0f - ratio);
		}
		
		protected static float easeOutBounce(float ratio)
		{
		    float s = 7.5625f;
		    float p = 2.75f;
		    float l = 0.0f;
		    if (ratio < (1.0f/p))
		    {
		        l = s * Mathf.Pow(ratio, 2);
		    }
		    else
		    {
		        if (ratio < (2.0f/p))
		        {
		            ratio -= 1.5f/p;
		            l = s * Mathf.Pow(ratio, 2) + 0.75f;
		        }
		        else
		        {
		            if (ratio < 2.5f/p)
		            {
		                ratio -= 2.25f/p;
		                l = s * Mathf.Pow(ratio, 2) + 0.9375f;
		            }
		            else
		            {
		                ratio -= 2.625f/p;
		                l =  s * Mathf.Pow(ratio, 2) + 0.984375f;
		            }
		        }
		    }
		    return l;
		}
		
		protected static float easeInOutBounce(float ratio)
		{
		    return easeCombined(new transitionDelegate(easeInBounce), new transitionDelegate(easeOutBounce), ratio);
		}   
		
		protected static float easeOutInBounce(float ratio)
		{
		    return easeCombined(new transitionDelegate(easeOutBounce), new transitionDelegate(easeInBounce), ratio);
		}
		
		protected static float easeInSine(float ratio)
		{
		    return -Mathf.Cos(ratio * _HALF_PI) + 1f;
		}
		
		protected static float easeOutSine(float ratio)
		{
		    return Mathf.Sin(ratio * _HALF_PI);
		}
		
		protected static float easeInOutSine(float ratio)
		{
		    return -0.5f * (Mathf.Cos(Mathf.PI * ratio) - 1f);
		}
		
		protected static float easeCombined(Delegate startFunc, Delegate endFunc, float ratio)
		{
		    float r1 = (float) startFunc.DynamicInvoke( ratio * 2.0f );
			float r2 = (float) endFunc.DynamicInvoke( (ratio-0.5f) * 2.0f );
			
			if (ratio < 0.5f){
				return 0.5f * r1;
			}else{
				return 0.5f * r2 + 0.5f;
			}
		}
	}
}