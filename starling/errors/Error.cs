using UnityEngine;
using System;

namespace starling.errors {
	
	public class Error : Exception {
	
		public Error(string msg)
		{
			Debug.LogError(this.Message);
		}
	}
}