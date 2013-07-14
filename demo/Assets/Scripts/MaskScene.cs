using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using starling.display;
using pumpkin.events;
using pumpkin.display;
using pumpkin.text;
using pumpkin.swf;
using pumpkin.geom;

public class MaskScene : Scene {

	private Sprite mContents;
    private Quad mClipQuad;
	
	public MaskScene()
	{
		mContents = new Sprite();
        addChild(mContents);
		
		var stageWidth  = 640f;
        var stageHeight = 960f;
        
        var touchQuad = new Quad(stageWidth, stageHeight, Color.black);
        touchQuad.alpha = 0.01f; // used to get touch events
        addChildAt(touchQuad, 0);
		
		var image = new Image(Game.assets.getTexture("flight_00"));
        image.x = (stageWidth * 0.5f) - (image.width * 0.75f);
        image.y = 80f;
        mContents.addChild(image);
		
		var scissorText = new BitmapTextField();
		scissorText.width = 512f;
		scissorText.height = 256f;
		scissorText.multiline = true;
		scissorText.textFormat = Game.textFormat;
        scissorText.text = "Move the mouse (or a finger) over the screen to move the clipping rectangle.";
        scissorText.x = (stageWidth - scissorText.width) * 0.5f;
        scissorText.y = 480f;
        mContents.addChild(scissorText);
		
		var maskText = new BitmapTextField();
		maskText.width = 512f;
		maskText.height	= 256f;
		maskText.multiline = true;
		maskText.textFormat = Game.textFormat;
        maskText.text = "Currently, Starling supports only stage-aligned clipping; more complex masks " +
            "will be supported in future versions.";
        maskText.x = scissorText.x;
        maskText.y = 580;
        mContents.addChild(maskText);
		
		var scissorRect = new Rect(0f, 0f, 300f, 300f); 
        scissorRect.x = (stageWidth  - scissorRect.width)  / 2f;
        scissorRect.y = (stageHeight - scissorRect.height) / 2f + 5f;
        mContents.clipRect = scissorRect;
		
		mClipQuad = new Quad(scissorRect.width, scissorRect.height, 0xff0000.hexToColor());
        mClipQuad.x = scissorRect.x;
        mClipQuad.y = scissorRect.y;
        mClipQuad.alpha = 0.1f;
        addChild(mClipQuad);
		
		addEventListener(MouseEvent.MOUSE_MOVE, onTouch);
	}
	
	private void onTouch(CEvent e)
	{
		var localPos = (e as MouseEvent);
        var clipRect = mContents.clipRect;
        clipRect.x = localPos.stageX; //- (clipRect.width  * 0.5f);
        clipRect.y = localPos.stageY; //- (clipRect.height * 0.5f);
		
		if(clipRect.y > 480) clipRect.y = 480;
		mContents.clipRect = clipRect;
        
        mClipQuad.x = clipRect.x;
        mClipQuad.y = clipRect.y;
    }
}
