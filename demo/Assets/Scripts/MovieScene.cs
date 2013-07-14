using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using starling.display;
using pumpkin.events;
using pumpkin.display;
using pumpkin.text;
using pumpkin.swf;

public class MovieScene : Scene {

	private SMovieClip mMovie;
	
	public MovieScene()
	{
		var frames = Game.assets.getTextures("flight");
		mMovie = new SMovieClip(frames, 15);
		
		// add sounds
        var stepSound = new Sound("wing_flap");
        mMovie.setFrameSound(2, stepSound);
		
		// move the clip to the center and add it to the stage
		mMovie.pivotX = mMovie.width * 0.5f;
		mMovie.pivotY = mMovie.height * 0.5f;
		
        mMovie.x = 260f;
        mMovie.y = 400f;
		
        addChild(mMovie);
		
		addEventListener(CEvent.ADDED_TO_STAGE, onAddedToStage);
        addEventListener(CEvent.REMOVED_FROM_STAGE, onRemovedFromStage);
	}
	
	private void onAddedToStage( CEvent e )
    {
        Starling.juggler.add(mMovie);
    }
    
    private void onRemovedFromStage( CEvent e )
    {
        Starling.juggler.remove(mMovie);
    }
}
