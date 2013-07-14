using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using starling.display;
using pumpkin.events;
using pumpkin.display;
using pumpkin.text;
using pumpkin.swf;

public class TextureScene : Scene {
	
	public TextureScene() : base()
	{
		var image1 = new Image(Game.assets.getTexture("flight_00"));
        image1.x = -40;
        image1.y = 0;
        addChild(image1);
        
        var image2 = new Image(Game.assets.getTexture("flight_04"));
        image2.x = 180;
        image2.y = 170;
        addChild(image2);
        
        var image3 = new Image(Game.assets.getTexture("flight_08"));
        image3.x = 200;
        image3.y = -120;
        addChild(image3);
	}
}
