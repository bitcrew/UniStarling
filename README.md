UniStarling
===========

A port of the Starling Framework to Unity3d and uniSWF.

uniSWF is a commercial product and is not included with the source for this project. A free trial version of uniSWF is available from the folks at Flaming Pumpkin.

http://www.uniswf.com

Demos
-------------------------

A quick port of the Starling scaffold demo 

http://dev.thebitcrew.com/mobile/uniStarling/uniStarlingDemo.html

A test of porting a game we build with Starling Framework to Unity with uniStarling

http://dev.thebitcrew.com/mobile/uniStarling/uniStarlingDrWoo.html

Features
-------------------------

uniSWF adds a GPU accelerated flash DisplayList to Unity3d and has a workflow based on importing assets that are created in Flash Pro which are parsed and baked to metadata and texture atlases.

uniStarling adds a code centric workflow based on the features of the Starling Framework on top of uniSWF and allows you to freely mix both workflows together.

uniStarling uses the Starling TextureAtlas format and AssetManager which means you can use the same texture assets and asset loading that you use in Starling inside Unity3d.

uniStarling adds support for Starling's Quad, Image and MovieClip DisplayObjects on top of the DisplayObjects natively supported by uniSWF.

The animation system of Starling was also ported to uniStarling. This includes Starling's Juggler, DelayedCall, Tween, and Transitions. You can animate both 2D & 3D objects in Unity by implementing the IAnimatable interface and adding them to a Juggler.

uniStarling is still a work in progress but is fairly stable.