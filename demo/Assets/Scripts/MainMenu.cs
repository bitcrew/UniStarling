using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using starling.display;
using pumpkin.display;
using pumpkin.events;
using pumpkin.text;
using pumpkin.swf;

public class MainMenu : Sprite {
	
	private Image logo;
	//private TextField txt;
	
	public MainMenu()
	{	
		logo = new Image( Game.assets.getTexture("logo") );
		addChild(logo);
		
		var scenesToCreate = new List<String>{
			"Textures",
			"Multitouch",
			"TextFields",
			"Animations",
			//"Custom hit-test",
			"Movie Clip",
			//"Filters",
			"Blend Modes",
			//"Render Texture",
			"Clipping",
			"Benchmark"
		};
		
		var buttonTexture = Game.assets.getTexture("button_medium");
        var count = 0;
		
		foreach (var sceneToCreate in scenesToCreate)
        {
            var sceneTitle = sceneToCreate;
            
            var button = new Button( new Image(buttonTexture), sceneTitle, null, Game.textFormat);
            button.x = count % 2 == 0 ? 56 : 334;
            button.y = 310 + (count / 2) * 92;
            button.name = sceneTitle;
			button.addEventListener(Button.BUTTON_CLICKED, onButtonTriggered);
            addChild(button);
            
            if (scenesToCreate.Count % 2 != 0 && count % 2 == 1)
                button.y += 48;
            
            ++count;
        }
		
		BitmapTextField bmpTxt = new BitmapTextField( );
		bmpTxt.width = 500;
		bmpTxt.height = 200;
		bmpTxt.textFormat = Game.textFormat;		
		bmpTxt.text = "uniStarling Demo App";
		bmpTxt.y = 900;
		bmpTxt.x = 56;
		addChild( bmpTxt );
	}
	
	private void onButtonTriggered( CEvent e )
    {
		var button = (Button)e.target;
        showScene(button.name);
    }
	
	private void showScene(string name)
	{
		DisplayObject newScene;
		
		switch(name)
		{
			case "Textures":
				newScene = new TextureScene();
				break;
			case "Multitouch":
				newScene = new TouchScene();
				break;
			case "TextFields":
				newScene = new TextScene();
				break;
			case "Animations":
				newScene = new AnimationScene();
				break;
			case "Movie Clip":
				newScene = new MovieScene();
				break;
			case "Blend Modes":
				newScene = new BlendModeScene();
				break;
			case "Clipping":
				newScene = new MaskScene();
				break;
			case "Benchmark":
				newScene = new BenchmarkScene();
				break;
			default :
				newScene = new Scene();
				break;
		}
		
		Game.viewManager.setView( newScene );
	}
}
