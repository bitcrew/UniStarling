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
using System.Xml;

using starling.animation;
using starling.texture;
using starling.errors;

namespace starling.utils {
	
	public class AssetManager {
        
		public delegate void progressDelgate(float val);
		private delegate void resumeDelegate();
		private delegate void processDelegate();
		private Delegate progressUpdate;
		
		private float mScaleFactor;
		private bool mVerbose;
		
		private Stack<AssetItem> mRawAssets;
		private Dictionary<String, STexture> mTextures;
		private Dictionary<String, TextureAtlas> mAtlases;
		private Dictionary<String, XmlDocument> mXMLs;
		
		/** helper objects */
		private List<String> sNames = new List<String>();
		private List<XmlDocument> xmls;
		private int numElements;
		private float currentRatio;
		
		
		public AssetManager(float scaleFactor=1f, bool useMipmaps=false){
			
		    mScaleFactor = scaleFactor > 0f ? scaleFactor : 1f;
			mVerbose = false;
			
		    mRawAssets = new Stack<AssetItem>();
		    mTextures = new Dictionary<String, STexture>();
		    mAtlases = new Dictionary<String, TextureAtlas>();
			mXMLs = new Dictionary<String, XmlDocument>();
		}
		
		/** Disposes all contained textures. */
		public void dispose()
		{
		    foreach (var texture in mTextures)
		        texture.Value.dispose();
		    
		    foreach (var atlas in mAtlases)
		        atlas.Value.dispose();
		}
		
		// retrieving
		
		/** Returns a texture with a certain name. The method first looks through the directly
		 *  added textures; if no texture with that name is found, it scans through all 
		 *  texture atlases. */
		public STexture getTexture(String name)
		{
		    if (mTextures.ContainsKey(name)) return mTextures[name];
		    else
		    {
		        foreach (var atlas in mAtlases)
		        {
		            var texture = atlas.Value.getTexture(name);
		            if (texture != null) return texture;
		        }
		        return null;
		    }
		}
		
		/** Returns all textures that start with a certain string, sorted alphabetically
		 *  (especially useful for "MovieClip"). */
		public List<STexture> getTextures(String prefix="", List<STexture> result=null)
		{
		    if (result == null) result = new List<STexture>();
		    
		    foreach (var name in getTextureNames(prefix, sNames))
		        result.Add(getTexture(name));
		    
		    sNames.Clear();
		    return result;
		}
		
		/** Returns all texture names that start with a certain string, sorted alphabetically. */
		public List<String> getTextureNames(String prefix="", List<String> result=null)
		{
		    if (result == null) result = new List<String>();
		    
		    foreach(var pair in mTextures)
		        if (pair.Key.IndexOf(prefix) == 0)
		            result.Add(pair.Key);                
		    
		    foreach (var atlas in mAtlases)
		        atlas.Value.getNames(prefix, result);
		    
		    result.Sort();
		    return result;
		}
		
		/** Returns a texture atlas with a certain name, or null if it's not found. */
		public TextureAtlas getTextureAtlas(String name)
		{
		    return mAtlases[name] as TextureAtlas;
		}
		
		/** Returns a xml with a certain name. */
		public XmlDocument getXML(String name)
		{
			return mXMLs[name];
		}
		
		// direct adding
		
		/** Register a texture under a certain name. It will be available right away. */
		public void addTexture(String name, STexture texture)
		{
		    log("Adding texture '" + name + "'");
		    
		    if (mTextures.ContainsKey(name))
		        throw new Error("Duplicate texture name: " + name);
		    else
		        mTextures[name] = texture;
		}
		
		/** Register a texture atlas under a certain name. It will be available right away. */
		public void addTextureAtlas(String name, TextureAtlas atlas)
		{
		    log("Adding texture atlas '" + name + "'");
		    
		    if (mAtlases.ContainsKey(name))
		        throw new Error("Duplicate texture atlas name: " + name);
		    else
		        mAtlases[name] = atlas;
		}
		
		/** Register a XML file a certain name. It will be availble right away. */
		public void addXML(String name, XmlDocument xml)
		{
			log("Adding xml '" + name + "'");
			
			if (mXMLs.ContainsKey(name))
				throw new Error("Duplicate xml name: " + name);
			else
				mXMLs[name] = xml;
		}
		
		// removing
		
		/** Removes a certain texture, optionally disposing it. */
		public void removeTexture(String name, bool dispose=true)
		{
		    if (dispose && mTextures.ContainsKey(name))
		        mTextures[name].dispose();
		    
		    mTextures.Remove(name);
		}
		
		/** Removes a certain texture atlas, optionally disposing it. */
		public void removeTextureAtlas(String name, bool dispose=true)
		{
		    if (dispose && mAtlases.ContainsKey(name))
		        mAtlases[name].dispose();
		    
		    mAtlases.Remove(name);
		}
		
		/** Removes a certain xml file. */
		public void removeXML(String name)
		{
			mXMLs.Remove(name);
		}
		
		/** Removes assets of all types and empties the queue. */
		public void purge()
		{
		    foreach (var texture in mTextures)
		        texture.Value.dispose();
		    
		    foreach (var atlas in mAtlases)
		        atlas.Value.dispose();
		    
		    mRawAssets = new Stack<AssetItem>();
		    mTextures = new Dictionary<String, STexture>();
		    mAtlases = new Dictionary<String, TextureAtlas>();
			mXMLs = new Dictionary<String, XmlDocument>();
		}
		
		// queued adding
		
		public void enqueue(params object[] rawAssets)
		{
			foreach(var rawAsset in rawAssets)
			{
				var assetType = rawAsset.GetType();
				
				if(assetType == typeof(UnityEngine.Texture2D) || assetType == typeof(UnityEngine.Texture)){
					addTexture( (rawAsset as Texture).name, STexture.fromTexture((rawAsset as Texture), null, null) );
				}
				else if(assetType == typeof(starling.texture.TextureAtlas)){
					addTextureAtlas( (rawAsset as TextureAtlas).name, (rawAsset as TextureAtlas) );
				}
				else if (assetType == typeof(UnityEngine.TextAsset))
		        {
		            addXML( (rawAsset as TextAsset).name, (rawAsset as TextAsset).text.toXmlDocument() );
		        }
				else if (assetType == typeof(System.String))
		        {
		            enqueueWithName(rawAsset);
		        }
				else
		        {
		            log("Ignoring unsupported asset type: " + rawAsset.GetType().ToString());
		        }
			}
		}
		
		/** Enqueues a single asset with a custom name that can be used to access it later. 
		 *  If you don't pass a name, it's attempted to generate it automatically.
		 *  @returns the name under which the asset was registered. */
		public String enqueueWithName(object asset, String name=null)
		{
		    if (name == null) name = getName(asset);
		    log("Enqueuing '" + name + "'");
			
			mRawAssets.Push( new AssetItem(){ {"name",asset},{"asset",asset} } );
		    
		    return name;
		}
		
		/** Loads all enqueued assets asynchronously. The 'onProgress' function will be called
		 *  with a 'ratio' between '0.0' and '1.0', with '1.0' meaning that it's complete.
		 *
		 *  @param onProgress: <code>function(ratio:Number):void;</code> 
		 */
		public void loadQueue(Delegate onProgress=null)
		{            
		    xmls = new List<XmlDocument>();
		    numElements = mRawAssets.Count;
		    currentRatio = 0.0f;
		    
			progressUpdate = onProgress;
			
		    resume();
		}
		
		private void resume()
		{
			currentRatio = mRawAssets.Count > 0 ? 0.9f - ((float)mRawAssets.Count / (float)numElements) : 0.9f;
		    if(currentRatio < 0) currentRatio = 0;
			log ("ratio : " + currentRatio.ToString("F2"));    
			
		    if (mRawAssets.Count > 0){
				Starling.juggler.delayCall(processNext, 1.0f);
		    }else{
		        processXmls();
			}
		    
		    if (progressUpdate != null)
		        progressUpdate.DynamicInvoke(currentRatio);
		}
		
		private void processNext()
		{
		    var assetInfo = mRawAssets.Pop();
		    loadRawAsset(assetInfo["name"].ToString(), assetInfo["asset"], xmls, new progressDelgate(progress), new resumeDelegate(resume) );
		}
		
		private void processXmls()
		{
			// xmls are processed seperately at the end, because the textures they reference
		    // have to be available for other XMLs. Texture atlases are processed first:
		    // that way, their textures can be referenced, too.
			
			log("processing xml files");
			
			foreach (var xml in xmls)
		    {
				XmlNodeList atlas = xml.SelectNodes("/TextureAtlas");
				if(atlas != null)
				{
					var name =  atlas[0].Attributes["imagePath"].Value.Split('.')[0] + "_tex";
					
					var atlasTexture = (getTexture(name) as SubTexture).parent;
		            addTextureAtlas(name, new TextureAtlas(atlasTexture, xml));
		            removeTexture(name, false);
				}
				else
				{
		        	//throw new Error("XML contents not recognized");
				}
			}
			
			if (progressUpdate != null){
				log("Loading Complete");
		        progressUpdate.DynamicInvoke(1.0f);
			}
		}
		
		private void progress(float ratio)
		{
		    //onProgress(currentRatio + (1.0f / (float)numElements) * Mathf.Min(1.0f, ratio) * 0.99f);
		}
		
		private void loadRawAsset(String name, object rawAsset, List<XmlDocument> xmls,
		                              Delegate onProgress, Delegate onComplete)
		{
			var assetType = rawAsset.GetType();
			if (assetType == typeof(System.String))
			{
		        var loadedAsset = Resources.Load(name);
				var loadedAssetType = loadedAsset.GetType();
				
				if(loadedAssetType == typeof(UnityEngine.Texture2D) || loadedAssetType == typeof(UnityEngine.Texture))
				{
					addTexture( (loadedAsset as Texture).name, STexture.fromTexture((loadedAsset as Texture), null, null) );
					onComplete.DynamicInvoke();
				}
				else if(loadedAssetType == typeof(UnityEngine.TextAsset))
				{
					var xml = (loadedAsset as TextAsset).text.toXmlDocument();
					addXML( (loadedAsset as TextAsset).name, xml );
					xmls.Add(xml);
					onComplete.DynamicInvoke();
				}
				else
				{
					log("Ignoring unsupported asset type: " + rawAsset.GetType().ToString());
					onComplete.DynamicInvoke();
				}
		    }
			else
		    {
		        log("Ignoring unsupported asset type: " + rawAsset.GetType().ToString());
				onComplete.DynamicInvoke();
		    }
		}
		
		// helpers
		
		/** This method is called by 'enqueue' to determine the name under which an asset will be
		 *  accessible; override it if you need a custom naming scheme. Typically, 'rawAsset' is 
		 *  either a String or a FileReference. Note that this method won't be called for embedded
		 *  assets. */
		protected String getName(object rawAsset)
		{
			var assetType = rawAsset.GetType();
			if(assetType == typeof(System.String))
			{
				return rawAsset.ToString();
			}
			else if(assetType == typeof(UnityEngine.Texture2D) || assetType == typeof(UnityEngine.Texture))
			{
				return (rawAsset as Texture).name;
			}
			else if(assetType == typeof(starling.texture.TextureAtlas))
			{
				return (rawAsset as TextureAtlas).name;
			}
			else
			{
		        throw new ArgumentError("Cannot extract name for object of type '" + rawAsset + "'");
			}
		}
		
		/** This method is called during loading of assets when 'verbose' is activated. Per
		 *  default, it traces 'message' to the console. */
		protected void log(String message)
		{
		    if (mVerbose) Debug.Log("[AssetManager]" + message);
		}
		
		// properties
		
		/** When activated, the class will trace information about added/enqueued assets. */
		public bool verbose{
			get { return mVerbose; }
			set { mVerbose = value; }
		}
		
		/** Textures that are created from Bitmaps or ATF files will have the scale factor 
		 *  assigned here. */
		public float scaleFactor{
			get { return mScaleFactor; }
			set { mScaleFactor = value; }
		}
		
		public static Delegate Call(progressDelgate fn)
		{
			return new progressDelgate(fn); 
		}
	}
}