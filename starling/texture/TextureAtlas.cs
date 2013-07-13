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
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using starling.errors;

using pumpkin.geom;

namespace starling.texture {
	
	public class TextureAtlas {
		
		private Texture mAtlasTexture;
		private Dictionary<string, Rectangle> mTextureRegions;
		private Dictionary<string, Rectangle> mTextureFrames;
		
		/** helper objects */
		private List<String> sNames = new List<String>();
		
		/** Create a texture atlas from a texture by parsing the regions from an XML file. */
		public TextureAtlas(Texture texture, XmlDocument atlasXml=null)
		{
		    mTextureRegions = new Dictionary<string, Rectangle>();
		    mTextureFrames  = new Dictionary<string, Rectangle>();
		    mAtlasTexture   = texture;
		    
		    if (atlasXml != null)
		        parseAtlasXml(atlasXml);
		}
		
		/** Disposes the atlas texture. */
		public void dispose()
		{
		    //mAtlasTexture.dispose();
			Resources.UnloadAsset(mAtlasTexture);
		}
		
		/** This function is called by the constructor and will parse an XML in Starling's 
		 *  default atlas file format. Override this method to create custom parsing logic
		 *  (e.g. to support a different file format). */
		protected void parseAtlasXml(XmlDocument xml)
		{
			if(xml == null)
			{
				throw new ArgumentError("The atlas was not a proper XML file. Make sure to select \"Starling\" in TexturePacker.");
			}
			
			var scale = 1f; //mAtlasTexture.scale
		    
			XmlNodeList subTextures = xml.SelectNodes("/TextureAtlas/SubTexture");
		    foreach (XmlNode subTexture in subTextures)
		    {
		        float x, y, width, height, frameX, frameY, frameWidth, frameHeight;
				
				var name    = subTexture.Attributes["name"].Value;
		        x           = float.Parse(subTexture.Attributes["x"].Value) / scale;
		        y           = float.Parse(subTexture.Attributes["y"].Value) / scale;
		        width       = float.Parse(subTexture.Attributes["width"].Value) / scale;
		        height      = float.Parse(subTexture.Attributes["height"].Value) / scale;
		        frameX      = subTexture.Attributes["frameX"] == null ? 0f : float.Parse(subTexture.Attributes["frameX"].Value) / scale;
		        frameY      = subTexture.Attributes["frameY"] == null ? 0f : float.Parse(subTexture.Attributes["frameY"].Value) / scale;
		        frameWidth  = subTexture.Attributes["frameWidth"] == null ? 0f : float.Parse(subTexture.Attributes["width"].Value) / scale;
		        frameHeight = subTexture.Attributes["frameHeight"] == null ? 0f : float.Parse(subTexture.Attributes["height"].Value) / scale;
				
		        Rectangle region = new Rectangle(x, y, width, height);
		        Rectangle frame  = frameWidth > 0f && frameHeight > 0f ?
		                new Rectangle(frameX, frameY, frameWidth, frameHeight) : null;
		        
		        addRegion(name, region, frame);
		    }
		}
		
		/** Retrieves a subtexture by name. Returns <code>null</code> if it is not found. */
		public STexture getTexture(String name)
		{
			var region = mTextureRegions.ContainsKey(name) ? mTextureRegions[name] : null;
		    
		    if (region == null) return null;
		    else return STexture.fromTexture(mAtlasTexture, region, mTextureFrames[name]);
		}
		
		/** Returns all textures that start with a certain string, sorted alphabetically
		 *  (especially useful for "MovieClip"). */
		public List<STexture> getTextures(String prefix="", List<STexture> result=null)
		{
		    if (result == null) result = new List<STexture>();
		    
		    foreach (String name in getNames(prefix, sNames))
			{
		        result.Add(getTexture(name));
			}
		    sNames.Clear();
		    return result;
		}
		
		/** Returns all texture names that start with a certain string, sorted alphabetically. */
		public List<String> getNames(String prefix="", List<String> result=null)
		{
		    if (result == null) result = new List<String>();
		    
		    foreach(var pair in mTextureRegions)
			{
		        if (pair.Key.IndexOf(prefix) == 0)
		            result.Add(pair.Key);
			}
		    result.Sort();
		    return result;
		}
		
		/** Returns the region rectangle associated with a specific name. */
		public Rectangle getRegion(String name)
		{
		    return mTextureRegions[name];
		}
		
		/** Returns the frame rectangle of a specific region, or <code>null</code> if that region 
		 *  has no frame. */
		public Rectangle getFrame(String name)
		{
		    return mTextureFrames[name];
		}
		
		/** Adds a named region for a subtexture (described by rectangle with coordinates in 
		 *  pixels) with an optional frame. */
		public void addRegion(String name, Rectangle region, Rectangle frame=null)
		{
		    mTextureRegions[name] = region;
		    mTextureFrames[name]  = frame;
		}
		
		/** Removes a region with a certain name. */
		public void removeRegion(String name)
		{
		    mTextureRegions.Remove(name);
		    mTextureFrames.Remove(name);
		}
		
		public String name{
			get { return mAtlasTexture.name; }
		}
	}
}
