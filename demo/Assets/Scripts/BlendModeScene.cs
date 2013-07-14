using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using starling.display;
using pumpkin.events;
using pumpkin.display;
using pumpkin.text;
using pumpkin.swf;

public class BlendModeScene : Scene {
	
	private Button mButton;
    private Image mImage;
	private BitmapTextField mInfoText;
	private List<int> mBlendModes;
	
	public BlendModeScene()
	{
		mBlendModes = new List<int>();
		mBlendModes.Add(BlendMode.ADD);
		mBlendModes.Add(BlendMode.NORMAL);
		
		mButton = new Button(new Image(Game.assets.getTexture("button_normal")), "Switch Mode", null, Game.textFormat);
        mButton.x = 320f - mButton.width / 2f;
        mButton.y = 30f;
        mButton.addEventListener(Button.BUTTON_CLICKED, onButtonTriggered);
        addChild(mButton);
        
        mImage = new Image(Game.assets.getTexture("starling_rocket"));
        mImage.x = 320f - mImage.width / 2f;
        mImage.y = 340;
        addChild(mImage);
        
        mInfoText = new BitmapTextField();
		mInfoText.width = 600;
		mInfoText.height = 32;
		mInfoText.textFormat = Game.textFormat;
        mInfoText.x = 20;
        mInfoText.y = 660;
        addChild(mInfoText);
	}
	
	private void onButtonTriggered( CEvent e )
    {
        var blendMode = mBlendModes[0];
		mBlendModes.RemoveAt(0);
        mBlendModes.Add(blendMode);
        
        mInfoText.text = blendMode == BlendMode.NORMAL ? "BlendMode Normal" : "BlendMode Add";
        mImage.blendMode = blendMode;
    }
}
