using UnityEngine;
using System.Collections;

using starling.animation;
using starling.display;
using starling.texture;
using starling.utils;

using pumpkin.events;
using pumpkin.display;
using pumpkin.swf;
using pumpkin.text;


public class Game : MonoBehaviour {
	
	public static Stage stage;
	public static Juggler juggler;
	public static ViewManager viewManager;
	public static AssetManager sAssets;
	public static AudioService audioService;
	public static TextFormat textFormat; 
	public Camera cam;
	
	
	void Awake()
	{			
		Application.targetFrameRate = 60;
		MovieClip.preloadSWF( "flash/fonts_shared.swf" );
	}
	
	// Use this for initialization
	void Start()
	{
		if(MovieClipOverlayCameraBehaviour.instance == null)
		{
			return;
		}
		
		juggler = new Juggler();
		Starling.juggler.add(juggler);
		
		sAssets = new AssetManager(1, false);
		sAssets.verbose = true;
		
		audioService = AudioService.getInstance();
		audioService.gameObject = this.gameObject;
		
		stage = MovieClipOverlayCameraBehaviour.instance.stage;
		stage.addEventListener( CEvent.RESIZE, onResize);
		
		
		var sWidth = Screen.width;
		var scaleWidth = sWidth / 640f;
		
		stage.width = Screen.width;
		stage.height = Screen.height;
	
		MovieClipOverlayCameraBehaviour.instance.stageScale.Set(scaleWidth, scaleWidth);
		
		var tfmc = new MovieClip("flash/fonts_shared.swf:Fonts");
		tfmc.stop();
		
		// set the default textformat
		var txt = tfmc.getChildByName<TextField>("txt");
		textFormat = txt.textFormat;
		
		viewManager = new ViewManager(stage);
		
		assets.enqueue(
			"atlas",
			"atlas_tex"
		);
		assets.loadQueue( AssetManager.Call(assetsProg) );
	}
	
	private void assetsProg(float val)
	{
		if(val == 1.0f)
		{
			var background = new Image( assets.getTexture("background") );
			stage.addChild(background);
			
			Game.viewManager.setView( new MainMenu() );
		}
	}
	
	// Update is called once per frame
	public void Update()
	{
		
	}
	
	private void onResize( CEvent e )
	{
		var sWidth = Screen.width;
		var scaleWidth = sWidth / 640f;
		
		stage.width = Screen.width;
		stage.height = Screen.height;
	
		MovieClipOverlayCameraBehaviour.instance.stageScale.Set(scaleWidth, scaleWidth);
	}
	
	public static AssetManager assets
	{ 
		get { return sAssets; }
	}
}
