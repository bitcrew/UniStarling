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
using System.Collections.Generic;

using pumpkin.geom;

namespace starling.texture {
	
	public abstract class STexture {
		
		private Rectangle mFrame;
		private bool mRepeat;
		
		public STexture()
		{
		    mRepeat = false;
		}
		
		public virtual void dispose()
		{ 
		    // override in subclasses
		}
		
		/** Creates a texture that contains a region (in pixels) of another texture. The new
		 *  texture will reference the base texture; no data is duplicated. */
		public static STexture fromTexture(Texture texture, Rectangle region=null, Rectangle frame=null)
		{
		    STexture subTexture = new SubTexture(texture, region, false);   
		    subTexture.mFrame = frame;
		    return subTexture;
		}
		
		// properties
		
		/** The texture frame (see class description). */
		public Rectangle frame
		{ 
		    get { return mFrame != null ? mFrame.clone() : new Rectangle(0f, 0f, width, height); }
		    
		    // the frame property is readonly - set the frame in the 'fromTexture' method.
		    // why is it readonly? To be able to efficiently cache the texture coordinates on
		    // rendering, textures need to be immutable (except 'repeat', which is not cached,
		    // anyway).
		}
		
		/** Indicates if the texture should repeat like a wallpaper or stretch the outermost pixels.
		 *  Note: this only works in textures with sidelengths that are powers of two and 
		 *  that are not loaded from a texture atlas (i.e. no subtextures). @default false */
		public bool repeat{
			get { return mRepeat; }
			set { mRepeat = value; }
		}
		
		/** The width of the texture in points. */
		public abstract float width{
			get;
		}
		
		/** The height of the texture in points. */
		public abstract float height{
			get;
		}
		
		/** The width of the texture in pixels (without scale adjustment). */
		/*public abstract float nativeWidth{
			get;
		}*/
		
		/** The height of the texture in pixels (without scale adjustment). */
		/*public abstract float nativeHeight{
			get;
		}*/
		
		/** The scale factor, which influences width and height properties. */
		public abstract float scale{
			get;
		}
		
		/** The Stage3D texture object the texture is based on. */
		//public TextureBase base{ get { return null; } }
		
		/** The concrete (power-of-two) texture the texture is based on. */
		//public ConcreteTexture root{ get { return null; } }
		
		/** The <code>Context3DTextureFormat</code> of the underlying texture data. */
		//public String format{ { return Context3DTextureFormat.BGRA; } }
		public abstract String format{
			get;
		}
		
		/** Indicates if the texture contains mip maps. */ 
		public abstract bool mipMapping{
			get;
		}
		
		/** Indicates if the alpha values are premultiplied into the RGB values. */
		public abstract bool premultipliedAlpha{
			get;
		}
	}
}