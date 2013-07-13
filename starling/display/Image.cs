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

using starling.texture;
using starling.errors;

using pumpkin.display;
using pumpkin.swf;

namespace starling.display {
	
	public class Image : Sprite {
		
		private Material material;
		private String textureName;
		private bool materialExists;
		private TextureManager textureManager;
		private STexture mTexture;
		private float mPivotX;
		private float mPivotY;
			
		public Image(STexture sTexture = null) : base()
		{
			if (sTexture != null)
		    {
				mPivotX = mPivotY = 0.0f;
				textureManager = TextureManager.instance;
				texture = sTexture;
			}
		    else
		    {
		        throw new ArgumentError("Texture cannot be null");
		    }
		}
			
		private void overrideMatrix()
		{
			var matrix = this.getMatrix().clone();
			
			matrix.a = m_scaleX;
			matrix.b = 0.0f;
			matrix.c = 0.0f;
			matrix.d = m_scaleY;
			matrix.tx = 0.0f;
			matrix.ty = 0.0f;
			
		    float mRotation = m_Rotation * Mathf.PI / 180;
		    if (mRotation != 0.0f)
		    {
		        matrix.rotate(mRotation);
		    }
			
			matrix.tx = m_x - mPivotX * matrix.a - mPivotY * matrix.c;
			matrix.ty = m_y - mPivotX * matrix.b - mPivotY * matrix.d;
			
			this.setMatrixOverride( matrix );
		}
		
		/** The texture that is displayed on the quad. */
		public STexture texture
		{
			get { return mTexture; }
			set {
				if (value == null)
		        {
		            throw new ArgumentError("Texture cannot be null");
		        }
		        else if (mTexture != value)
		        {
		            mTexture = value;
					SubTexture st = mTexture as SubTexture;
					
					textureName = st.parent.name;
					materialExists = textureManager.materials.ContainsKey(textureName);
					
					//save draw calls by reusing existing uniSWF material
					if(materialExists){
						material = textureManager.materials[textureName];
					}else{
						material = new Material(TextureManager.baseBitmapShader);
			        	material.mainTexture = st.parent;
						textureManager.materials.Add(textureName, material);
					}
					
					Rect srcRect = new Rect( st.clipping.x, st.clipping.y, st.clipping.width, st.clipping.height );
					Rect drawRect = new Rect( -st.frame.x, -st.frame.y, st.frame.width, st.frame.height );
					
					this.graphics.clear();
					this.graphics.drawRectUV(material, srcRect, drawRect);
		        }
			}
		}
		
		public Color color
		{
			get { return this.colorTransform; }
			set { this.colorTransform = value; }
		}
		
		/** The x coordinate of the object's origin in its own coordinate space (default: 0). */
		public float pivotX
		{
			get { return mPivotX; }
			set { 
		        if (mPivotX != value)
		        {
		            mPivotX = value;
					overrideMatrix();
		        }
			}
		}
		
		/** The y coordinate of the object's origin in its own coordinate space (default: 0). */
		public float pivotY
		{
			get { return mPivotY; }
			set {
		        if (mPivotY != value)
		        {
		            mPivotY = value;
					overrideMatrix();
		        }
			}
		}
		
		public new float x
		{
			get { return m_x; }
			set { m_x = value; overrideMatrix(); }
		}
		
		public new float y
		{
			get { return m_y; }
			set { m_y = value; overrideMatrix(); }
		}
		
		public new float rotation
		{
			get { return m_Rotation; }
			set { m_Rotation = value; overrideMatrix(); }
		}
	}
}
