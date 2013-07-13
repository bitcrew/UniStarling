using UnityEngine;
using System.Collections;
using starling.animation;

	
	public class Starling : MonoBehaviour {
	
		private static Juggler mJuggler;
		
		void Awake()
		{
			mJuggler = new Juggler();
		}
		
		// Use this for initialization
		void Start()
		{
		
		}
		
		// Update is called once per frame
		void Update()
		{
			//animate objects that have been added to the juggler
			mJuggler.advanceTime(Time.deltaTime);
		}
		
		/** The default juggler of this instance. Will be advanced once per frame. */
		public static Juggler juggler
		{ 
			get { return mJuggler; }
		}
	}
