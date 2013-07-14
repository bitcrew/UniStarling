using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using starling.animation;
using starling.display;
using starling.events;
using pumpkin.events;
using pumpkin.display;
using pumpkin.text;
using pumpkin.swf;

public class AnimationScene : Scene {
	
	private Button mStartButton;
    private Button mDelayButton;
    private Image mEgg;
    private BitmapTextField mTransitionLabel;
    private List<string> mTransitions;
	private delegate void callbackDelegate();
	private delegate void callbackDelegateBool(bool b);
	
	public AnimationScene() : base()
	{
		mTransitions = new List<String>();
		mTransitions.Add(Transitions.LINEAR);
		mTransitions.Add(Transitions.EASE_IN_OUT);
		mTransitions.Add(Transitions.EASE_OUT_BACK);
		mTransitions.Add(Transitions.EASE_OUT_BOUNCE);
		mTransitions.Add(Transitions.EASE_OUT_ELASTIC);
		
		var buttonTexture = Game.assets.getTexture("button_normal");
		
		// create a button that starts the tween
        mStartButton = new Button(new Image(buttonTexture), "Start animation", null, Game.textFormat);
        mStartButton.addEventListener(Button.BUTTON_CLICKED, onStartButtonTriggered);
        mStartButton.x = 320f - mStartButton.width / 2f;
        mStartButton.y = 40f;
        addChild(mStartButton);
		
		// this button will show you how to call a method with a delay
        mDelayButton = new Button(new Image(buttonTexture), "Delayed call", null, Game.textFormat);
        mDelayButton.addEventListener(Button.BUTTON_CLICKED, onDelayButtonTriggered);
        mDelayButton.x = mStartButton.x;
        mDelayButton.y = mStartButton.y + 80f;
        addChild(mDelayButton);
		
		// the Starling will be tweened
        mEgg = new Image(Game.assets.getTexture("starling_front"));
        addChild(mEgg);
		resetEgg();
		
		mTransitionLabel = new BitmapTextField();
		mTransitionLabel.width = 640f;
		mTransitionLabel.height = 30f;
		mTransitionLabel.textFormat = Game.textFormat;
        mTransitionLabel.y = mDelayButton.y + 80f;
        mTransitionLabel.alpha = 0.0f; // invisible, will be shown later
        addChild(mTransitionLabel);
	}
	
	private void resetEgg()
    {
        mEgg.x = 40f;
        mEgg.y = 200f;
        mEgg.scaleX = mEgg.scaleY = 1.0f;
        mEgg.rotation = 0.0f;
    }
	
	private void onStartButtonTriggered( CEvent e )
	{
		mStartButton.enabled = false;
        resetEgg();
		
		// get next transition style from array and enqueue it at the end
        var transition = mTransitions[0];
		mTransitions.RemoveAt(0);
        mTransitions.Add(transition);
		
		// to animate any numeric property of an arbitrary object (not just display objects!), 
        // you can create a 'Tween'. One tween object animates one target for a certain time, 
        // a with certain transition function.
        var tween = new Tween(mEgg, 2.0f, transition);
		
		// you can animate any property as long as it's numeric (int, uint, Number). 
        // it is animated from it's current value to a target value.  
        tween.animate("rotation", 90f); // conventional 'animate' call
        tween.moveTo(600f, 720f);       // convenience method for animating 'x' and 'y'
        tween.scaleTo(0.5f);             // convenience method for 'scaleX' and 'scaleY'
        tween.onComplete = new callbackDelegate(resetStartBtn);
		
		// the tween alone is useless -- for an animation to be carried out, it has to be 
        // advance once in every frame.            
        // This is done by the 'Juggler'. It receives the tween and will carry it out.
        // We use the default juggler here, but you can create your own jugglers, as well.            
        // That way, you can group animations into logical parts.  
        Starling.juggler.add(tween);
		
		// show which tweening function is used
        mTransitionLabel.text = transition;
        mTransitionLabel.alpha = 1.0f;
        
        var hideTween = new Tween(mTransitionLabel, 2.0f, Transitions.EASE_IN);
        hideTween.animate("alpha", 0.0f);
        Starling.juggler.add(hideTween);
		
		/*
		Starling.juggler.tween(mEgg, 2.0f, new TweenParams(){
			{"delay",2.0f},{"x",600f},{"y",720f},{"scaleX",0.5f},{"scaleY",0.5f}
		});
		*/
	}
	
	private void resetStartBtn()
	{
		mStartButton.enabled = true;
	}
	
	private void onDelayButtonTriggered( CEvent e )
	{
		mDelayButton.enabled = false;
            
        // Using the juggler, you can delay a method call. This is especially useful when
        // you use your own juggler in a component of your game, because it gives you perfect 
        // control over the flow of time and animations. 
        
        Starling.juggler.delayCall(new callbackDelegateBool(colorizeEgg), 1.0f, new object[]{true});
        Starling.juggler.delayCall(new callbackDelegateBool(colorizeEgg), 2.0f, new object[]{false});
        Starling.juggler.delayCall(resetDelayBtn, 2.0f);
	}
	
	private void resetDelayBtn()
	{
		mDelayButton.enabled = true;
	}
	
	private void colorizeEgg(bool colorize)
    {
        mEgg.color = colorize ? Color.red : Color.white;
    }
}
