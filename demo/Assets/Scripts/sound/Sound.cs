using UnityEngine;
using System;
using System.Collections;

public class Sound {
	
	private AudioClip mClip;
	
	
	public Sound( String name )
	{
		mClip = Resources.Load("audio/" + name) as AudioClip;
	}
	
	public Sound( AudioClip clip )
	{
		mClip = clip;
	}
	
	public void play(float startPos = 0, int loops=0, float volume=1)
	{
		AudioSource.PlayClipAtPoint(mClip, Vector3.zero, volume);
	}
	
	public void close()
	{
		Resources.UnloadAsset(mClip);
	}
	
	public AudioClip clip
	{
		get { return mClip; }
		set { mClip = value; }
	}
}
