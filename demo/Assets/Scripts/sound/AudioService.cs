using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using starling.animation;


public class AudioService {

	private static AudioService _instance;
	private static Dictionary<String, Sound> _sounds = new Dictionary<String, Sound>();
	private static Dictionary<String, SoundTrack> _tracks = new Dictionary<String, SoundTrack>();
	
	private float _soundVolume = 1f;
	private float _trackVolume = 1f;
	
	//private float _soundVolumePreMute = 1f;
	//private float _trackVolumePreMute = 1f;
	
	public GameObject gameObject;
	
	
	public AudioService()
	{
		
	}
	
	public static AudioService getInstance()
	{	
		return _instance != null ? _instance : _instance = new AudioService();
	}
	
	/////////////////////////////////////////////////////
	// sound methods
	/////////////////////////////////////////////////////
	
	public void addSound(String name)
	{
		if(_sounds.ContainsKey(name) == false){
			_sounds[name] = new Sound( name );
		}
	}
	
	public void playSound(String name, float startPos = 0f, int loops = 0)
	{
		var sound = _sounds[name];
		if(sound != null){
			sound.play(startPos, loops, _soundVolume);
		}
	}
	
	public Sound getSound(String name)
	{
		return _sounds[name];
	}
	
	public void removeSound(String name)
	{
		var sound = _sounds[name];
		if(sound != null){
			try{
				sound.close();
			}
			catch(Exception e){
				Debug.Log( e.Message );
			}
			_sounds.Remove(name);
			sound = null;
		}
	}
	
	public void adjustSfxVolume(int volume)
	{
		var floatVolume = (float)volume / (float)100;
		
		// clamps
		if(floatVolume < 0.05 ) floatVolume = 0;
		if(floatVolume > 1.00 ) floatVolume = 1;
				
		_soundVolume = floatVolume;
		//trace("SFX vol: " + _soundVolume);
	}
	
	public void adjustMusicVolume(int volume)
	{
		var floatVolume = (float)volume / (float)100;
		
		// clamps
		if(floatVolume < 0.05 ) floatVolume = 0;
		if(floatVolume > 1.00 ) floatVolume = 1;
		
		_trackVolume = floatVolume;
		//trace("Music vol: " + _trackVolume);
	}
	
	public void updateVolumeOnCurrentTracks()
	{
		SoundTrack tempTrack = null;
		if(_tracks != null)
		{				
			// set to level
			foreach(var kname in _tracks) 
			{
				if(_tracks.ContainsKey(kname.Key))
				{
					tempTrack = _tracks[kname.Key];
					if(tempTrack != null)
					{
						//tempTrack.volume = 0.0;
						//tempTrack.volume = 1.0;
						tempTrack.volume = _trackVolume * tempTrack.volumeRatio;
					}
				}
			}
		}
	}
	
	/////////////////////////////////////////////////////
	// track methods
	/////////////////////////////////////////////////////
	
	public void addTrack(String name, Delegate onCompleteHandler=null)
	{
		if(_tracks.ContainsKey(name) == false){
			_tracks[name] = new SoundTrack( new Sound( name ), gameObject, onCompleteHandler );
			_tracks[name].volume = _trackVolume;
		}
	}
	
	public void playTrack(String trackName, bool resume=false)
	{
		var track = _tracks[trackName];
		if(!track.isPlaying){
			track.play(resume);
		}
	}
	
	public void stopTrack(String trackName)
	{
		if(_tracks[trackName] != null)
		{
			var track = _tracks[trackName];
			if(track.isPlaying && track != null){
				track.stop();
			}
		}
	}
	
	public void loopTrack(String trackName, float loopPosition=0, bool resume=false, float volumeRatio=1.0f)
	{	
		var track = _tracks[trackName];
		if(!track.isPlaying){
			track.loop(loopPosition, resume);
			track.volume = _trackVolume * volumeRatio;
			track.volumeRatio = volumeRatio;
		}
	}
	
	public void fadeInTrack(String fadeTrack, float fadeTime)
	{
		var track = _tracks[fadeTrack];
		if(track.isPlaying){
			var tmpVolume = track.volume;
			track.volume = 0;
			//TweenLite.from(track, fadeTime, {volume:0 });
			Starling.juggler.tween( track, fadeTime, new TweenParams(){
				{"volume",tmpVolume}
			});
		}
	}
}
