using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using starling.display;
using pumpkin.events;
using pumpkin.display;
using pumpkin.text;
using pumpkin.swf;

public class TouchScene : Scene {
	
	private Image sheet;
	
	public TouchScene()
	{
		var description = "[use Ctrl/Cmd & Shift to simulate multi-touch]";
            
        var infoText = new BitmapTextField();
		infoText.width = 600f;	
		infoText.height = 50f;
		infoText.textFormat = Game.textFormat;
		infoText.text = description;
        infoText.x = infoText.y = 20;
        addChild(infoText);
		
		// to find out how to react to touch events have a look at the TouchSheet class! 
        // It's part of the demo.
		
		sheet = new Image( Game.assets.getTexture("starling_sheet") );
		sheet.x = 360f - sheet.width * 0.5f;
        sheet.y = 440f - sheet.height * 0.5f;
        sheet.rotation = 10f;
        addChild(sheet);
		
		addEventListener(CEvent.ADDED_TO_STAGE, onAddedToStage);
		addEventListener(CEvent.REMOVED_FROM_STAGE, onRemovedFromStage);
	}
	
	private void onAddedToStage( CEvent e )
	{
		removeEventListener(CEvent.ADDED_TO_STAGE, onAddedToStage);
		stage.addEventListener(MouseEvent.MOUSE_UP, onMouseUp);
		stage.addEventListener(MouseEvent.MOUSE_DOWN, onMouseDown);
	}
	
	private void onRemovedFromStage( CEvent e )
	{
		removeEventListener(CEvent.ADDED_TO_STAGE, onAddedToStage);
		removeEventListener(CEvent.ADDED_TO_STAGE, onRemovedFromStage);
		stage.removeEventListener(MouseEvent.MOUSE_UP, onMouseUp);
		stage.removeEventListener(MouseEvent.MOUSE_DOWN, onMouseDown);
	}
	
	private void onMouseUp( CEvent e )
	{
		stage.removeEventListener(MouseEvent.MOUSE_MOVE, onMouseMove);
	}
	
	private void onMouseDown( CEvent e )
	{
		stage.addEventListener(MouseEvent.MOUSE_MOVE, onMouseMove);
	}
	
		
	private void onMouseMove( CEvent e )
	{
		var pos = (e as MouseEvent);
        sheet.x = pos.stageX - (sheet.width  * 0.5f);
        sheet.y = pos.stageY - (sheet.height * 0.5f);
	}
}
