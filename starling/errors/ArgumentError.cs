using UnityEngine;
using System;

namespace starling.errors {
	
	public class ArgumentError : Exception {
	
		public ArgumentError(string msg)
		{
			Debug.LogError(this.Message);
		}
	}
}