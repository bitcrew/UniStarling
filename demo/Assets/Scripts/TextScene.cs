using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using starling.display;
using pumpkin.events;
using pumpkin.display;
using pumpkin.text;
using pumpkin.swf;

public class TextScene : Scene {
	
	public TextScene()
	{
		init();
	}
	
	private void init()
    {
		int offset = 20;
		//int ttFontSize = 38; 
		
		var tf = Game.textFormat;
		tf.fontClassName = null;
		
		var colorTF = new BitmapTextField();
		colorTF.width = 600f;
		colorTF.height = 160f;
		colorTF.textFormat = Game.textFormat;
        colorTF.text = "TextFields can have a border and a color. They can be aligned in different ways, ...";
		//colorTF.textFormat.size = ttFontSize;
		//colorTF.textFormat.color = 0x333399.hexToColor();
		colorTF.multiline = true;
        colorTF.x = colorTF.y = offset;
        addChild(colorTF);
		
		var leftTF = new BitmapTextField();
		leftTF.width = 290f;
		leftTF.height = 160f;
		leftTF.textFormat = Game.textFormat;
        leftTF.text = "... e.g.\ntop-left ...";
        leftTF.x = offset;
		//leftTF.textFormat.size = ttFontSize;
        //leftTF.textFormat.color = 0x993333.hexToColor();
		leftTF.multiline = true;
		leftTF.y = colorTF.y + colorTF.height + offset;
        addChild(leftTF);
		
		var rightTF = new BitmapTextField();
		rightTF.width = 290f;
		rightTF.height = 160f;
		rightTF.textFormat = Game.textFormat;
        rightTF.text = "... or\nbottom right ...";
        rightTF.x = 2*offset + leftTF.width;
        rightTF.y = leftTF.y;
        //rightTF.color = 0x228822;
		rightTF.multiline = true;
        addChild(rightTF);
		
		var fontTF = new BitmapTextField();
		fontTF.width = 600f;
		fontTF.height = 160f;
		fontTF.textFormat = Game.textFormat;
        fontTF.text = "... or centered. Embedded fonts are detected automatically.";
        fontTF.x = offset;
        fontTF.y = leftTF.y + leftTF.height + offset;
        fontTF.multiline = true;
        addChild(fontTF);
	}
}
