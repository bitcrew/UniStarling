using UnityEngine;
using System;
using System.Collections;

public class SoundTrack {
	
	private Sound _soundObj;			// sound object
	private AudioSource _channel;		// sound channel
	private float _pos = 0;				// sound position (seek)
	private float _volume = 1;			// sound volume
	private float _volumeRatio = 1;		// sound volume ratio
	private float _pan = 0;				// sound pan
	private Delegate _complete;			// on complete function, called when the sound Track has reached the end of the audio file.
										// Returns a reference to SoundTrack

	private float _loopPos = 0;			// loop position, allows for an intro to the audio loop, will loop back to 0 by default
	public bool isPlaying;				// if sound is currently playing
	
	
	/**
	 * INIT soundTrack, hook up controls
	 *  @param soundObject: Sound
	 *  @param gameObject: GameObject
	 * 	@param onCompleteHandler: Delegate
	 **/
	public SoundTrack(Sound soundObject, GameObject gameObject, Delegate onCompleteHandler=null){
		_soundObj = soundObject;
		_channel = gameObject.AddComponent("AudioSource") as AudioSource;
		_channel.clip = _soundObj.clip;
		_complete = onCompleteHandler;
	}
	
	/**
	 * play the sound
	 * @param resume: start from the beginning of sound (default) or resume
	 **/
	public void play(bool resume=false)
	{
		isPlaying = true;
		if(resume){
			_channel.time = _pos;
			_channel.volume = _volume;
			_channel.pan = _pan;
			_channel.loop = false;
			_channel.Play();
		}
		else{
			_channel.time = 0;
			_channel.volume = _volume;
			_channel.pan = _pan;
			_channel.loop = false;
			_channel.Play();
		}
		
		//if(!_channel.hasEventListener(Event.SOUND_COMPLETE)){
		//	_channel.addEventListener(Event.SOUND_COMPLETE,soundHandler);
		//}
	}
	
	/**
	 * loop the sound
	 * @param startPosition: the position to restart the loop
	 **/
	public void loop(float loopPosition=0, bool resume=false)
	{
		isPlaying = true;
		if(loopPosition > 0){
			_loopPos = loopPosition;
			play(resume);
		}
		else{
			if(resume){
				_channel.time = _pos;
			_channel.volume = _volume;
			_channel.pan = _pan;
			_channel.loop = true;
			_channel.Play();
			}
			else{
				_channel.time = 0;
				_channel.volume = _volume;
				_channel.pan = _pan;
				_channel.loop = true;
				_channel.Play();
			}
		}
	}
	
	/**
	 * pause the sound
	 **/
	public void pause()
	{
		isPlaying = false;
		_pos = _channel.time;
		_channel.Stop();
	}
	
	/**
	 * stop the sound
	 **/
	public void stop()
	{
		isPlaying = false;
		_pos = 0;
		_loopPos = 0;
		_channel.Stop();
	}
	
	/**
	 * close sound and remove listeners
	 **/
	public void cleanup()
	{
		stop();
		try{
			_soundObj.close();
		}
		catch(Exception e){
			Debug.Log( e.Message );
		}
		if(_channel != null){
			//if(_channel.hasEventListener(Event.SOUND_COMPLETE)){
			//	_channel.removeEventListener(Event.SOUND_COMPLETE,soundHandler);
			//}
		}
	}
	
	/**
	 * get position of sound channel
	 * @returns a number representing the sound position
	 **/
	public float position
	{ 
		get { return _channel.time; }
	}

	/**
	 * direct access to sound
	 * @returns a Sound Object
	 **/
	public Sound sound
	{ 
		get { return _soundObj; }
	}
	
	/**
	 * set the volume as a property
	 * @param num: number representing the volume 0 - 1
	 * get the volume as a property
	 * @param val number representing the volume 0 - 1
	 **/
	public float volume
	{
		get { return _volume; }
		set { _volume = value;
			  _channel.volume = _volume;}
	}

	/**
	 * get the current pan
	 * @returns a number representing the current pan -1 - 1
	 * set the pan
	 * @param num: the pan to left or right speaker -1 - 1
	 **/
	public float pan
	{ 
		get { return _pan; }
		set { _pan = value;
			  _channel.pan = _pan; }
	}
	
	/**
	 * on sound complete event handler
	 * calls complete function if one has been set.
	 * @returns an refernce to SoundTrack
	 **/
	private void soundHandler(Event ev){
		isPlaying = false;
		_pos = 0;
		if(_loopPos > 0){
			_pos = _loopPos;
			play(true);
		}
		if(_complete != null){
			_complete.DynamicInvoke(this);
		}
	}

	/**
	 * set onComplete function
	 * @param onCompleteHandler: Delegate
	 **/
	public void onComplete(Delegate onCompleteHandler)
	{
		_complete = onCompleteHandler;
	}
	
	public float volumeRatio
	{ 
		get { return _volumeRatio; }
		set { _volumeRatio = value; }
	}
}
