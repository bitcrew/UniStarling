using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public static class ClassExtensions
{
	public static XmlDocument toXmlDocument( this string xml )
	{
		XmlDocument doc = new XmlDocument();
		doc.LoadXml( xml );
		return doc;
	}
	
	public static Color hexToColor( this uint color )
	{
		var r = (color >> 16) & 0xff;
		var g = (color >>  8) & 0xff;
		var b = color & 0xff;
		
		return new Color(r, g, b);
	}
	
	public static Color hexToColorA( this uint color )
	{
		var a = (color >> 24) & 0xff;
		var r = (color >> 16) & 0xff;
		var g = (color >>  8) & 0xff;
		var b = color & 0xff;
		
		return new Color(r, g, b, a);
	}
}
