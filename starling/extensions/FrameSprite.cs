using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using starling.animation;
using starling.display;
using pumpkin.display;

namespace starling.extensions {
	
	public class FrameSprite : Sprite {
		
		private List<SMovieClip> mFrames = new List<SMovieClip>();
		private int mCurrentFrame = 1;
		private String mCurrentLabel;
		private int mFramesCount = 1;
		private bool mPaused = false;
		private Juggler mJuggler;
		
		
		public FrameSprite(Juggler juggler) : base()
		{
		    mJuggler = juggler;
		}
		
		public void addFrame(SMovieClip mFrame, String label)
		{
		    mFrame.name = label;
		    mFrames.Add(mFrame);
		    addChild(mFrame);
			mJuggler.add(mFrame);
		    if (mFramesCount++ == 1) {
		        mCurrentLabel = label;
		    } else {
		        mFrame.visible = false;
		    }
		}
		
		public void removeFrame(String _val)
		{
		    SMovieClip mTempFrame = null;
		    SMovieClip mFrame = null;
		
		    for (var i = 0; i < mFramesCount; i++) {
		        mTempFrame = mFrames[i];
		        if (mTempFrame.name == _val) {
		            mFrame = mTempFrame;
		            mCurrentFrame = i+1;
		            break;
		        }
		    }
			
			var frameIndex = Convert.ToInt32(_val);
		    if (mFrame == null) mFrame = mFrames[frameIndex-1];
		
		    if ((frameIndex) == mCurrentFrame) prevFrame();
		
		    if (mFrame != null) {
		        removeChild(mFrame);
				mJuggler.remove(mFrame);
		        mFrames.RemoveRange(frameIndex-1,1);
		        mFramesCount--;
		    }
		}
		
		public void nextFrame()
		{
		    play( (mCurrentFrame+1).ToString() );
		}
		
		public void prevFrame()
		{
		    play( (mCurrentFrame-1).ToString() );
		}
		
		public SMovieClip getCurrentFrameMovieClip()
		{
		    return (mCurrentFrame >= 1 && mCurrentFrame <= mFramesCount) ? mFrames[mCurrentFrame-1] : null;
		}
		
		public void play(String _val)
		{
			//if (mCurrentLabel == _val || mCurrentFrame == Convert.ToInt32(_val)) return;
			if (mCurrentLabel == _val) return;
		
		    SMovieClip mFrame = null;
		    SMovieClip mTempFrame = null;
		    var prevFrame = mCurrentFrame;
		
		    for (var i = 0; i < mFramesCount; i++) {
		        mTempFrame = mFrames[i];
		        if (mTempFrame.name == _val) {
		            mFrame = mTempFrame;
		            mCurrentFrame = i+1;
		            break;
		        }
		    }
		
		    /*if (mFrame == null) {
		        mCurrentFrame = _val;
		        mFrame = mFrames[_val-1];
		    }*/
		
		    if (mFrame != null) {
		        mCurrentLabel = mFrame.name;
		
		        mTempFrame = mFrames[prevFrame-1];
		        mTempFrame.stop();
		        mTempFrame.visible = false;
		
		        mFrame.currentFrame = 0;
		        mFrame.visible = true;
		        mFrame.play();
		    }
		}
		
		public void stop()
		{
		    var mFrame = getCurrentFrameMovieClip();
		    if (mFrame != null) mFrame.stop();
		}
		
		public void pauseOrResume()
		{
		    mPaused = !mPaused;
		    var mFrame = getCurrentFrameMovieClip();
		    if (mFrame != null) {
		        if (mPaused) mFrame.pause(); else mFrame.play();
		    }
		}
		
		public float getCurrentFrame()
		{
		    return mCurrentFrame;
		}
		
		public String getCurrentFrameLabel()
		{
		    return mCurrentLabel;
		}
		
		public void setCurrentFrame(int _val)
		{
		    if (_val < 1) _val = 1;
		    if (_val > mFramesCount) _val = mFramesCount;
		    mCurrentFrame = _val;
		}
		
		public float getTotalFrames()
		{
		    return mFramesCount;
		}
		
		public void destroy()
		{
		    SMovieClip mTempFrame;
		
		    for (var i = 0; i < mFramesCount-1; i++) {
		        mTempFrame = mFrames[i];
		        if (mTempFrame != null) {
		            removeChild(mTempFrame);
					mJuggler.remove(mTempFrame);
		            //mTempFrame.dispose();
		            mTempFrame = null;
		        }
		    }
		
			mFrames.Clear();
		}
	}
}
