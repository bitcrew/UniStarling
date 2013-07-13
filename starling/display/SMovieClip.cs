using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using starling.errors;
using starling.animation;
using starling.texture;

using pumpkin.display;
using pumpkin.events;
using pumpkin.swf;

	
namespace starling.display {
	
	public class SMovieClip : Image, IAnimatable {
		
		private List<STexture> mTextures;
		private List<Sound> mSounds;
        private List<float> mDurations;
        private List<float> mStartTimes;
		
		private float mDefaultFrameDuration;
        private float mCurrentTime;
        private int mCurrentFrame;
        private bool mLoop;
        private bool mPlaying;
		
		
		public SMovieClip(List<STexture> textures, int fps) : base(textures[0])
		{	
			if (textures.Count > 0)
            {
				//this.texture = textures[0];
                init(textures, fps);
            }
            else
            {
                throw new ArgumentError("Empty texture array");
            }
		}
		
		private void init(List<STexture> textures, int fps)
        {
			if (fps <= 0) throw new ArgumentError("Invalid fps: " + fps);
            var numFrames = textures.Count;
            
            mDefaultFrameDuration = 1.0f / fps;
            mLoop = true;
            mPlaying = true;
            mCurrentTime = 0.0f;
            mCurrentFrame = 0;
            mTextures = textures;
            mSounds = new List<Sound>();
            mDurations = new List<float>();
            mStartTimes = new List<float>();
            
            for (var i=0; i<numFrames; ++i)
            {
				mSounds.Add(null);
				mDurations.Add( mDefaultFrameDuration );
                mStartTimes.Add( i * mDefaultFrameDuration );
            }
		}
		
		// frame manipulation
        
        /** Adds an additional frame, optionally with a sound and a custom duration. If the 
         *  duration is omitted, the default framerate is used (as specified in the constructor). */   
        public void addFrame(STexture texture, Sound sound=null, float duration=-1)
        {
            addFrameAt(numFrames, texture, sound, duration);
        }
		
		/** Adds a frame at a certain index, optionally with a sound and a custom duration. */
        public void addFrameAt(int frameID, STexture texture, Sound sound=null, 
                                   float duration=-1)
        {
            if (frameID < 0 || frameID > numFrames) throw new ArgumentError("Invalid frame id");
            if (duration < 0) duration = mDefaultFrameDuration;
            
            mTextures.Insert(frameID, texture);
            mSounds.Insert(frameID, sound);
            mDurations.Insert(frameID, duration);
            
            if (frameID > 0 && frameID == numFrames) 
                mStartTimes[frameID] = mStartTimes[frameID-1] + mDurations[frameID-1];
            else
                updateStartTimes();
        }
		
		/** Removes the frame at a certain ID. The successors will move down. */
        public void removeFrameAt(int frameID)
        {
            if (frameID < 0 || frameID >= numFrames) throw new ArgumentError("Invalid frame id");
            if (numFrames == 1) throw new ArgumentError("Movie clip must not be empty");
            
            mTextures.RemoveRange(frameID, 1);
            mSounds.RemoveRange(frameID, 1);
            mDurations.RemoveRange(frameID, 1);
            
            updateStartTimes();
        }
		
		/** Returns the texture of a certain frame. */
        public STexture getFrameTexture(int frameID)
        {
            if (frameID < 0 || frameID >= numFrames) throw new ArgumentError("Invalid frame id");
            return mTextures[frameID];
        }
        
        /** Sets the texture of a certain frame. */
        public void setFrameTexture(int frameID, STexture texture)
        {
            if (frameID < 0 || frameID >= numFrames) throw new ArgumentError("Invalid frame id");
            mTextures[frameID] = texture;
        }
        
        /** Returns the sound of a certain frame. */
        public Sound getFrameSound(int frameID)
        {
            if (frameID < 0 || frameID >= numFrames) throw new ArgumentError("Invalid frame id");
            return mSounds[frameID];
        }
        
        /** Sets the sound of a certain frame. The sound will be played whenever the frame 
         *  is displayed. */
        public void setFrameSound(int frameID, Sound sound)
        {
            if (frameID < 0 || frameID >= numFrames) throw new ArgumentError("Invalid frame id");
            mSounds[frameID] = sound;
        }
		
		/** Returns the duration of a certain frame (in seconds). */
        public float getFrameDuration(int frameID)
        {
            if (frameID < 0 || frameID >= numFrames) throw new ArgumentError("Invalid frame id");
            return mDurations[frameID];
        }
        
        /** Sets the duration of a certain frame (in seconds). */
        public void setFrameDuration(int frameID, float duration)
        {
            if (frameID < 0 || frameID >= numFrames) throw new ArgumentError("Invalid frame id");
            mDurations[frameID] = duration;
            updateStartTimes();
        }
		
		// playback methods
        
        /** Starts playback. Beware that the clip has to be added to a juggler, too! */
        public void play()
        {
            mPlaying = true;
        }
        
        /** Pauses playback. */
        public void pause()
        {
            mPlaying = false;
        }
        
        /** Stops playback, resetting "currentFrame" to zero. */
        public void stop()
        {
            mPlaying = false;
            currentFrame = 0;
        }
        
        // helpers
        
        private void updateStartTimes()
        {
            var numFrames = this.numFrames;
            
			mStartTimes.Clear();
			mStartTimes.Add(0);
            
            for (var i=1; i<numFrames; ++i)
                mStartTimes.Add (mStartTimes[i-1] + mDurations[i-1]);
        }
        
        // IAnimatable
		
		public void advanceTime(float passedTime)
        {
			if (!mPlaying || passedTime <= 0.0) return;
            
            int finalFrame;
            int previousFrame = mCurrentFrame;
            float restTime = 0.0f;
            bool breakAfterFrame = false;
            bool hasCompleteListener = this.hasEventListener(CEvent.COMPLETE); 
            bool dispatchCompleteEvent = false;
            float totalTime = this.totalTime;
            
            if (mLoop && mCurrentTime >= totalTime)
            { 
                mCurrentTime = 0.0f; 
                mCurrentFrame = 0; 
            }
            
            if (mCurrentTime < totalTime)
            {
                mCurrentTime += passedTime;
                finalFrame = mTextures.Count - 1;
                
                while (mCurrentTime > mStartTimes[mCurrentFrame] + mDurations[mCurrentFrame])
                {
                    if (mCurrentFrame == finalFrame)
                    {
                        if (mLoop && !hasCompleteListener)
                        {
                            mCurrentTime -= totalTime;
                            mCurrentFrame = 0;
                        }
                        else
                        {
                            breakAfterFrame = true;
                            restTime = mCurrentTime - totalTime;
                            dispatchCompleteEvent = hasCompleteListener;
                            mCurrentFrame = finalFrame;
                            mCurrentTime = totalTime;
                        }
                    }
                    else
                    {
                        mCurrentFrame++;
                    }
                    
					var sound = mSounds[mCurrentFrame];
                    if (sound != null) sound.play();
                    if (breakAfterFrame) break;
                }
                
                // special case when we reach *exactly* the total time.
                if (mCurrentFrame == finalFrame && mCurrentTime == totalTime){
                    dispatchCompleteEvent = hasCompleteListener;
				}
            }
            
            if (mCurrentFrame != previousFrame)
                this.texture = mTextures[mCurrentFrame];
            
            if (dispatchCompleteEvent)
                dispatchEvent(new CEvent(CEvent.COMPLETE));
            
            if (mLoop && restTime > 0.0)
                advanceTime(restTime);
		}
		
		/** Indicates if a (non-looping) movie has come to its end. */
        public bool isComplete
        {
            get { return !mLoop && mCurrentTime >= totalTime; }
        }
		
		// properties
        
        /** The total duration of the clip in seconds. */
        public float totalTime 
        {
            get { var numFrames = mTextures.Count;
				  return mStartTimes[numFrames-1] + mDurations[numFrames-1];
			}
        }
		
		/** The time that has passed since the clip was started (each loop starts at zero). */
        public float currentTime
		{
			get { return mCurrentTime; }
		}
        
        /** The total number of frames. */
        public int numFrames
		{ 
			get { return mTextures.Count; }
		}
        
        /** Indicates if the clip should loop. */
        public bool loop
		{ 
			get { return mLoop; }
			set { mLoop = value; }
		}
        
        /** The index of the frame that is currently displayed. */
        public int currentFrame
		{ 
			get { return mCurrentFrame; }
        	set {
		            mCurrentFrame = value;
		            mCurrentTime = 0.0f;
		            
		            for (var i=0; i<value; ++i)
		                mCurrentTime += getFrameDuration(i);
		            
		            this.texture = mTextures[mCurrentFrame];
		    		if (mSounds[mCurrentFrame] != null) mSounds[mCurrentFrame].play();
			}
        }
        
        /** The default number of frames per second. Individual frames can have different 
         *  durations. If you change the fps, the durations of all frames will be scaled 
         *  relatively to the previous value. */
        public float fps
		{ 
			get { return 1.0f / mDefaultFrameDuration; }
			set {
		            if (value <= 0) throw new ArgumentError("Invalid fps: " + value);
		            
		            var newFrameDuration = 1.0f / value;
		            var acceleration = newFrameDuration / mDefaultFrameDuration;
		            mCurrentTime *= acceleration;
		            mDefaultFrameDuration = newFrameDuration;
		            
		            for (var i=0; i<numFrames; ++i) 
		            {
		                var duration = mDurations[i] * acceleration;
		                mDurations[i] = duration;
		            }
		            
		            updateStartTimes();
			}
        }
        
        /** Indicates if the clip is still playing. Returns <code>false</code> when the end 
         *  is reached. */
        public bool isPlaying 
        {
            get {
				if (mPlaying)
                	return mLoop || mCurrentTime < totalTime;
            	else
                	return false;
			}
        }
	}
}
