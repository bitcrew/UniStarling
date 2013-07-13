using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using pumpkin.geom;

namespace starling.texture {
	
	public class SubTexture : STexture {
		
		private Texture mParent;
        private Rectangle mClipping;
        //private Rectangle mRootClipping;
        private bool mOwnsParent;
		
        public SubTexture(Texture parentTexture, Rectangle region, bool ownsParent=false) : base()
        {
            mParent = parentTexture;
            mOwnsParent = ownsParent;
			
			if (region == null) setClipping(new Rectangle(0f, 0f, 1f, 1f));
            else setClipping(new Rectangle(region.x / parentTexture.width,
                                           region.y / parentTexture.height,
                                           region.width / parentTexture.width,
                                           region.height / parentTexture.height));
        }
		
		/** Disposes the parent texture if this texture owns it. */
        public override void dispose()
        {
            //if (mOwnsParent) mParent.dispose();
            base.dispose();
        }
        
        private void setClipping(Rectangle value)
        {
            mClipping = value;
            //mRootClipping = value.clone();
            
            /*var parentTexture = mParent as SubTexture;
            while (parentTexture)
            {
                var parentClipping = parentTexture.mClipping;
                mRootClipping.x = parentClipping.x + mRootClipping.x * parentClipping.width;
                mRootClipping.y = parentClipping.y + mRootClipping.y * parentClipping.height;
                mRootClipping.width  *= parentClipping.width;
                mRootClipping.height *= parentClipping.height;
                parentTexture = parentTexture.mParent as SubTexture;
            }*/
        }
		
		/** The texture which the subtexture is based on. */ 
        public Texture parent{
			get { return mParent; }
		}
        
        /** Indicates if the parent texture is disposed when this object is disposed. */
        public bool ownsParent{
			get { return mOwnsParent; }
		}
        
        /** The clipping rectangle, which is the region provided on initialization 
         *  scaled into [0.0, 1.0]. */
        public Rectangle clipping{
			get { return mClipping.clone(); }
		}
        
        /** @inheritDoc */
        //public override TextureBase base{ get { return mParent.base; } }
        
        /** @inheritDoc */
        //public override ConcreteTexture root{ get { return mParent.root; } }
        
        /** @inheritDoc */
        public override String format{
			get { return "Unity ARBG Texture"; }
			//mParent.format()
		}
        
        /** @inheritDoc */
        public override float width{
			get { return mParent.width * mClipping.width; }
		}
        
        /** @inheritDoc */
        public override float height{
			get { return mParent.height * mClipping.height; }
		}
        
        /** @inheritDoc */
        //public override float nativeWidth{ get { return mParent.nativeWidth * mClipping.width; } }
        
        /** @inheritDoc */
        //public override float nativeHeight{ get { return mParent.nativeHeight * mClipping.height; } }
        
        /** @inheritDoc */
        public override bool mipMapping{
			get { return false; }
			//return mParent.mipMapping;
		}
        
        /** @inheritDoc */
        public override bool premultipliedAlpha{
			get { return false; }
			//mParent.premultipliedAlpha;
		}
        
        /** @inheritDoc */
        public override float scale{
			get { return 1.0f; }
			//mParent.scale
		}
	}
}
