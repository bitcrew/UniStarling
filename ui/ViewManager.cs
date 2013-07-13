using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using pumpkin.display;

public class ViewManager {
	
	static ViewManager instance;
	
	private Stage srcClip;
	private DisplayObject _view;
	private List<DisplayObject> stack = new List<DisplayObject>();
	
	public ViewManager(Stage stage)
	{
		srcClip = stage;
		instance = this;
	}
	
	static ViewManager getInstance() {
		return instance;
	}
	
	public DisplayObject addView(DisplayObject view)
	{
		if (stack.IndexOf(view) == -1)
		{
			stack.Add(view);

			srcClip.addChild(view);
		}
		return view;
	}
	
	public void removeView(DisplayObject view, bool dispose = false)
	{
		var idx = stack.IndexOf(view);
		if (idx != -1)
		{
			srcClip.removeChild(view);
			stack.RemoveAt(idx);
			if(dispose)
				view = null;
		}
	}
	
	public void popView(bool dispose = false)
	{
		removeView(stack[stack.Count - 1] as DisplayObject, dispose);
	}
	
	public void removeAll(bool dispose = false)
	{
		foreach(DisplayObject s in stack)
		{
			removeView(s, dispose);
		}
	}
	
	public DisplayObject setView(DisplayObject viewClass, bool disposeOfLast = false)
	{
		removeExistingView(disposeOfLast);

		_view = viewClass;

		removeAll();

		srcClip.addChild(_view);
		return _view;
	}
	
	private void removeExistingView(bool dispose)
	{
		if (_view != null)
		{
			srcClip.removeChild(_view);
			if(dispose)
				_view = null;
		}
	}
}