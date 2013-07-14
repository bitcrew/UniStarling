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

using pumpkin.display;
using pumpkin.swf;

namespace starling.display {
	
	public class Quad : Sprite {
		
		private float mPivotX;
		private float mPivotY;
		
		public Quad( float width, float height, Color color) : base() {
			
			mPivotX = mPivotY = 0.0f;
			
			this.graphics.drawSolidRectangle( color, 0, 0, width, height );
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
		
		public Color color
		{
			get { return this.graphics.drawOPs[0].material.color; }
			set { this.graphics.drawOPs[0].material.color = value; }
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
