using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using starling.display;
using pumpkin.events;
using pumpkin.display;
using pumpkin.text;
using pumpkin.swf;

public class BenchmarkScene : Scene {
	
	private Button mStartButton;
    private BitmapTextField mResultText;
    
    private Sprite mContainer;
    private int mFrameCount;
    private float mElapsed;
    private bool mStarted;
    private int mFailCount;
    private int mWaitFrames;
	private System.Random rand = new System.Random();
	
	public BenchmarkScene()
	{
		// the container will hold all test objects
		mContainer = new Sprite();
		mContainer.mouseEnabled = false;
		mContainer.mouseChildrenEnabled = false;
		//mContainer.touchable = false; // we do not need touch events on the test objects -- 
		                              // thus, it is more efficient to disable them.
		addChildAt(mContainer, 0);
		
		mStartButton = new Button(new Image(Game.assets.getTexture("button_normal")), "Start benchmark", null, Game.textFormat);
		mStartButton.addEventListener(Button.BUTTON_CLICKED, onStartButtonTriggered);
		mStartButton.x = 320f - mStartButton.width / 2f;
		mStartButton.y = 40f;
		addChild(mStartButton);
		
		mStarted = false;
		mElapsed = 0.0f;
		
		addEventListener(CEvent.ENTER_FRAME, onEnterFrame);
	}
	
	private void onEnterFrame(CEvent e)
    {
        if (!mStarted) return;
        
        mElapsed += Time.deltaTime;
        mFrameCount++;
        
        if (mFrameCount % mWaitFrames == 0)
        {
            var fps = mWaitFrames / mElapsed;
            var targetFps = 60f;
            
            if (Mathf.Ceil(fps) >= targetFps)
            {
                mFailCount = 0;
                addTestObjects();
            }
            else
            {
                mFailCount++;
                
                if (mFailCount > 20)
                    mWaitFrames = 5; // slow down creation process to be more exact
                if (mFailCount > 30)
                    mWaitFrames = 10;
                if (mFailCount == 40)
                    benchmarkComplete(); // target fps not reached for a while
            }
            
            mElapsed = mFrameCount = 0;
        }
        
        var numObjects = mContainer.numChildren;
        //var passedTime = Time.deltaTime;
        
        for (var i=0; i<numObjects; ++i)
            mContainer.getChildAt(i).rotation += 2f; //passedTime;
    }
	
	private void onStartButtonTriggered( CEvent e )
    {
        Debug.Log("Starting benchmark");
        
        mStartButton.visible = false;
        mStarted = true;
        mFailCount = 0;
        mWaitFrames = 2;
        mFrameCount = 0;
        
        if (mResultText != null) 
        {
            removeChild(mResultText);
            mResultText = null;
        }
        
        addTestObjects();
    }
	
	private void addTestObjects()
    {
        var padding = 30;
        var numObjects = mFailCount > 20 ? 2 : 10;
        
        for (var i = 0; i<numObjects; ++i)
        {
            var egg = new Image(Game.assets.getTexture("benchmark_object"));
            egg.x = padding + (float)rand.NextDouble() * (640f - 2f * padding);
            egg.y = padding + (float)rand.NextDouble() * (900f - 2f * padding);
            mContainer.addChild(egg);
        }
    }
	
	private void benchmarkComplete()
    {
        mStarted = false;
        mStartButton.visible = true;
        
        var fps = 60;
        
        Debug.Log("Benchmark complete!");
        Debug.Log("FPS: " + fps);
        Debug.Log("Number of objects: " + mContainer.numChildren);
        
        var resultString = String.Format("Result:\n{0} objects\nwith {1} fps",
                                               mContainer.numChildren, fps);
        mResultText = new BitmapTextField();
		mResultText.width = 480f;
		mResultText.height = 400f;
		mResultText.textFormat = Game.textFormat;
		mResultText.text = resultString;
        mResultText.x = 320f - mResultText.width / 2f;
        mResultText.y = 480f - mResultText.height / 2f;
        
        addChild(mResultText);
        
        mContainer.removeAllChildren();
        //System.pauseForGCIfCollectionImminent();
    }
}
