// =================================================================================================
//
//  Starling Framework
//  Copyright 2011 Gamua OG. All Rights Reserved.
//
//  This program is free software. You can redistribute and/or modify it
//  in accordance with the terms of the accompanying license agreement.
//
// =================================================================================================

using UnityEngine;
using System;
using System.Collections;

using pumpkin.events;
using pumpkin.geom;
using pumpkin.display;
using pumpkin.swf;
using pumpkin.text;
using pumpkin.ui;

namespace starling.display {
	
	public class Button : DisplayObjectContainer {
	
		public const string BUTTON_CLICKED = "button_clicked";
		const int MAX_DRAG_DIST = 50;
		
		private DisplayObject mParent;
		private DisplayObject mUpState;
		private DisplayObject mDownState;
		
		private Sprite mContents;
		private DisplayObject mBackground;
		private BitmapTextField mTextField;
		private Rectangle mTextBounds;
		private TextFormat mTextFormat;
		
		private float mScaleWhenDown;
		private float mAlphaWhenDisabled;
		private bool mEnabled;
		private bool mIsDown;
		private bool mUseHandCursor;
		
		
		public Button(DisplayObject upState, string text ="", DisplayObject downState=null, TextFormat format=null) {
		
			//if (upState == null) throw new ErrorEvent("Texture cannot be null");
			
			mParent = upState.parent; 
		    mUpState = upState;
			mDownState = downState != null ? downState : upState;
			mBackground = upState;
			mTextFormat = format;
			
			mScaleWhenDown = 0.9f;
		    mAlphaWhenDisabled = 0.5f;
		    mEnabled = true;
		    mIsDown = false;
		    mUseHandCursor = true;
		    mTextBounds = new Rectangle(0, 0, upState.width, upState.height);            
		    
		    mContents = new Sprite();
		    mContents.addChild(mBackground);
		    addChild(mContents);
			
		    //addEventListener(TouchEvent.TOUCH, onTouch);
			addEventListener(MouseEvent.MOUSE_DOWN, onMouseDown);
		    
		    if (text.Length > 0) this.text = text;
			
			this.x = upState.x;
			this.y = upState.y;
			upState.x = upState.y = 0;
			
			if(mParent != null)
				(mParent as MovieClip).addChild(this);
		}
		
		private void resetContents()
		{
			removeEventListener(MouseEvent.MOUSE_UP, onMouseUp);
			removeEventListener(MouseEvent.MOUSE_LEAVE, onMouseLeave);
			
		    mIsDown = false;
		    mBackground = mUpState;
		    mContents.x = mContents.y = 0f;
		    mContents.scaleX = mContents.scaleY = 1.0f;
		}
		
		private void createTextField()
		{
		    if (mTextField == null)
		    {
		        //mTextField = new TextField(mTextBounds.width, mTextBounds.height, "");
				mTextField = new BitmapTextField();
				mTextField.textFormat = mTextFormat;
		        /*mTextField.vAlign = VAlign.CENTER;
		        mTextField.hAlign = HAlign.CENTER;
		        mTextField.touchable = false;
		        mTextField.autoScale = true;*/
		        mContents.addChild(mTextField);
		    }
		    
		    mTextField.width  = mTextBounds.width;
			mTextField.height = mTextFormat.size;
		    mTextField.x = mTextBounds.x;
			mTextField.y = (mTextBounds.height * 0.5f) - (mTextField.height * 0.75f);
		}
		
		private void onTouch(CEvent e)
		{
		    //Mouse.cursor = (mUseHandCursor && mEnabled && event.interactsWith(this)) ? 
		    //    MouseCursor.BUTTON : MouseCursor.AUTO;
		    
			//MouseEvent mouseEvent = e as MouseEvent;
			
		    //Touch touch = e.getTouch(this);
			//Touch touch = mouseEvent.touch;
			
		    /*if (!mEnabled || touch == null) return;
		    
		    if (touch.phase == TouchPhase.Began && !mIsDown)
		    {
		        //mBackground.texture = mDownState;
		        mContents.scaleX = mContents.scaleY = mScaleWhenDown;
		        mContents.x = (1.0f - mScaleWhenDown) / 2.0f * mBackground.width;
		        mContents.y = (1.0f - mScaleWhenDown) / 2.0f * mBackground.height;
		        mIsDown = true;
		    }
		    else if (touch.phase == TouchPhase.Moved && mIsDown)
		    {
		        // reset button when user dragged too far away after pushing
		        Rectangle buttonRect = getBounds(stage);
		        if (touch.position.x < buttonRect.x - MAX_DRAG_DIST ||
		            touch.position.y < buttonRect.y - MAX_DRAG_DIST ||
		            touch.position.x > buttonRect.x + buttonRect.width + MAX_DRAG_DIST ||
		            touch.position.y > buttonRect.y + buttonRect.height + MAX_DRAG_DIST)
		        {
		            resetContents();
		        }
		    }
		    else if (touch.phase == TouchPhase.Ended && mIsDown)
		    {
		        resetContents();
		        //dispatchEventWith(Event.TRIGGERED, true);
		    }*/
		}
		
		private void onMouseDown(CEvent e)
		{   
			MouseEvent mouseEvent = e as MouseEvent;
			Touch touch = mouseEvent.touch;
			
			if (!mEnabled) return;
		    
		    if (touch.phase == TouchPhase.Began && !mIsDown)
		    {
		        mBackground = mDownState;
				addEventListener(MouseEvent.MOUSE_UP, onMouseUp);
				addEventListener(MouseEvent.MOUSE_LEAVE, onMouseLeave);
				
		        mContents.scaleX = mContents.scaleY = mScaleWhenDown;
		        mContents.x = (1.0f - mScaleWhenDown) / 2.0f * mBackground.width;
		        mContents.y = (1.0f - mScaleWhenDown) / 2.0f * mBackground.height;
		        mIsDown = true;
				//Debug.Log( "mouse down" );
		    }
		}
		
		private void onMouseUp(CEvent e)
		{   
			//MouseEvent mouseEvent = e as MouseEvent;
			//Touch touch = mouseEvent.touch;
			
			if (!mEnabled) return;
		    
		    /*Rectangle buttonRect = getBounds(stage);
		    if (touch.position.x < buttonRect.x - MAX_DRAG_DIST ||
		        touch.position.y < buttonRect.y - MAX_DRAG_DIST ||
		        touch.position.x > buttonRect.x + buttonRect.width + MAX_DRAG_DIST ||
		        touch.position.y > buttonRect.y + buttonRect.height + MAX_DRAG_DIST)
		    {
		        resetContents();
		    }*/
			
			resetContents();
			
			dispatchEvent( new CEvent(Button.BUTTON_CLICKED, true, true) );
		}
		
		private void onMouseLeave(CEvent e)
		{   		
			if (!mEnabled) return;
		    
		    resetContents();
		}
		
		
		/** The scale factor of the button on touch. Per default, a button with a down state 
		  * texture won't scale. */
		public float scaleWhenDown { 
			get { return mScaleWhenDown; }
			set { mScaleWhenDown = value; }
		}
		
		/** The alpha value of the button when it is disabled. @default 0.5 */
		public float alphaWhenDisabled { 
			get { return mAlphaWhenDisabled; }
			set { mAlphaWhenDisabled = value; }
		}
		
		/** Indicates if the button can be triggered. */
		public bool enabled {
			get { return mEnabled; }
			set {
		        if (mEnabled != value)
		        {
		            mEnabled = value;
		            mContents.alpha = value ? 1.0f : mAlphaWhenDisabled;
		            resetContents();
		        }
			}
		}
		
		/** The text that is displayed on the button. */
		public String text
		{
		    get{ return mTextField != null ? mTextField.text : ""; }
			set{ createTextField();
				 mTextField.text = value; }
		}
		
		/** The name of the font displayed on the button. May be a system font or a registered 
		  * bitmap font. */
		//public string fontName() { return mTextField ? mTextField.fontName : "Verdana"; }
		public string fontName
		{
		    get { createTextField(); return mTextField.textFormat.font; }
			set { createTextField(); mTextField.textFormat.font = value; }
		}
		
		/** The size of the font. */
		//public int fontSize() { return mTextField ? mTextField.fontSize : 12; }
		public float fontSize
		{
		    get { createTextField(); return mTextField.textFormat.size; }
			set { createTextField(); mTextField.textFormat.size = value; }
		}
		
		/** The color of the font. */
		//public uint fontColor() { return mTextField ? mTextField.color : 0x0; }
		public Color fontColor
		{
		    get { createTextField(); return mTextField.textFormat.color; }
			set { createTextField(); mTextField.textFormat.color = value; }
		}
		
		/** Indicates if the font should be bold. */
		//public bool fontBold() { return mTextField ? mTextField.bold : false; }
		public bool fontBold
		{
		    get { createTextField(); return mTextField.textFormat.bold; }
			set { createTextField(); mTextField.textFormat.bold = value; }
		}
		
		/** The texture that is displayed when the button is not being touched. */
		public DisplayObject upState() { return mUpState; }
		public void upState(DisplayObject value)
		{
		    if (mUpState != value)
		    {
		        mUpState = value;
		        if (!mIsDown) mBackground = value;
		    }
		}
		
		/** The texture that is displayed while the button is touched. */
		public DisplayObject downState() { return mDownState; }
		public void downState(DisplayObject value)
		{
		    if (mDownState != value)
		    {
		        mDownState = value;
		        if (mIsDown) mBackground = value;
		    }
		}
		
		/** The vertical alignment of the text on the button. */
		//public string textVAlign() { return mTextField.vAlign; }
		public void textVAlign(string value)
		{
		    //createTextField();
		    //mTextField.vAlign = value;
		}
		
		/** The horizontal alignment of the text on the button. */
		//public string textHAlign() { return mTextField.hAlign; }
		public void textHAlign(string value)
		{
		    //createTextField();
		    //mTextField.hAlign = value;
		}
		
		/** The bounds of the textfield on the button. Allows moving the text to a custom position. */
		public Rectangle textBounds() { return mTextBounds.clone(); }
		public void textBounds(Rectangle value)
		{
		    createTextField();
			mTextBounds = value.clone();
		}
		
		/** Indicates if the mouse cursor should transform into a hand while it's over the button. 
		 *  @default true */
		public bool useHandCursor() { return mUseHandCursor; }
		public void useHandCursor(bool value) { mUseHandCursor = value; }
		
	}
}
