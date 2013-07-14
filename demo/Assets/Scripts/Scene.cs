using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using starling.display;
using pumpkin.events;
using pumpkin.display;
using pumpkin.text;
using pumpkin.swf;

public class Scene : Sprite {

	private Button mBackButton;
	
	public Scene()
	{
		// the main menu listens for TRIGGERED events, so we just need to add the button.
        // (the event will bubble up when it's dispatched.)
		
		var mBackButton = new Button( new Image(Game.assets.getTexture("button_back")), "Back", null, Game.textFormat);
        mBackButton.x = 320 - mBackButton.width / 2;
        mBackButton.y = 960 - mBackButton.height + 1;
        mBackButton.name = "backButton";
		mBackButton.addEventListener(Button.BUTTON_CLICKED, onButtonTriggered);
        addChild(mBackButton);
	}
	
	private void onButtonTriggered( CEvent e )
    {
		Game.viewManager.setView( new MainMenu() );
    }
}
