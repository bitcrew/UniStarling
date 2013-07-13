using UnityEngine;
using System.Collections;

public class Point {

	//https://gist.github.com/southpawfishel/5888907
	
	public float x, y;
	
	
	// CREATION / ASSIGNMENT
	
    public Point(float _x = 0, float _y = 0)
    {
        x = _x;
        y = _y;
    }

    public void setTo(float _x, float _y)
    {
        x = _x;
        y = _y;
    }
	
	public void copyFrom(Point p)
    {
        x = p.x;
        y = p.y;
    }

    public Point clone()
    {
        return new Point(x, y);
    }
	
	
	// QUERIES
	
    public string toString()
    {
        return "[Point " + x + ", " + y + "]";
    }
	
	public float length
    {
        get { return Mathf.Sqrt(lengthSquared); }
    }
	
	public float lengthSquared
    {
        get { return x*x + y*y; }
    }
	
	/*public override bool Equals(Point p)
    {
        if (x != p.x)
            return false;
        if (y != p.y)
            return false;
        return true;
    }*/
	
	public bool equals(Point p)
    {
        if (x != p.x)
            return false;
        if (y != p.y)
            return false;
        return true;
    }
	
	
	// OPERATIONS
	
	public void normalize(float thickness = 1)
    {
        var oldLength = this.length;
        if (oldLength == 0) return;
        
        var thickNessOverLength = thickness / oldLength;
        x *= thickNessOverLength;
        y *= thickNessOverLength;
    }
	
	public void offset(float dx, float dy)
    {
        x += dx;
        y += dy;
    }
	
	public Point add(Point p)
    {
        return new Point(x + p.x, y + p.y);
    }
	
	public static Point operator +(Point p1, Point p2)
    {
        return p1.add(p2);
    }
	
	public Point subtract(Point p)
    {
        return new Point(x - p.x, y - p.y);
    }
	
	public static Point operator -(Point p1, Point p2)
    {
        return p1.subtract(p2);
    }
	
	
	// STATIC HELPERS
	
	public static float distance(Point p1, Point p2)
    {
        return Mathf.Sqrt(Point.distanceSquared(p1, p2));
    }
	
	public static float distanceSquared(Point p1, Point p2)
    {
        var dx = p2.x - p1.x;
        var dy = p2.y = p1.y;
        return dx*dx + dy*dy;
    }
	
	public static Point interpolate(Point p1, Point p2, float t)
    {
        var newX = p2.x + ((1-t) * (p1.x - p2.x));
        var newY = p2.y + ((1-t) * (p1.y - p2.y));
        return new Point(newX, newY);
    }
	
	public static Point polar(float len, float angle)
	{		
		return new Point(len * Mathf.Cos(angle), len * Mathf.Sin(angle));
	}						
}
