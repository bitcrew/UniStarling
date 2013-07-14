using UnityEngine;
using System.Collections;

using pumpkin.events;
using pumpkin.display;
using pumpkin.tweener;


public class TapScroller : DisplayObjectContainer {
	
	DisplayObject _content;
	Sprite _scrollBarV;
	Sprite _scrollBarH;
	// Scroll factor is length of mouse movement to detect dragging
	double scrollFactor = 10;
	// use vertical scrolling
	public bool useVertical = true;
	// use horizontal scrolling
	public bool useHorizontal = true;
	// dragging flag
	bool isDragging = false;
	// last mouse position
	Vector2 lastPos = new Vector2();
	// first mouse position
	Vector2 firstPos = new Vector2();
	// first mouse position in panel 
	Vector2 firstPanelPos = new Vector2();
	// difference of mouse movement
	Vector2 diff = new Vector2();
	// scroll inhertia power
	Vector2 inertia = new Vector2();
	// minimum movable length
	Vector2 min = new Vector2();
	// maximum movable length
	Vector2 max = new Vector2();
	
	float panelWidth;
	float panelHeight;
	
	
	public TapScroller(DisplayObject content, float width, float height) : base() {
	
		panelWidth = width;
		panelHeight = height;
		
		this.clipRect = new Rect(0, 0, width, height);
		
		_content = content;
		addChild(_content);
		
		// initialize scrollbar
		_scrollBarV = new Sprite();
		_scrollBarV.x = width - 10;
		addChild(_scrollBarV);
		
		_scrollBarH = new Sprite();
		_scrollBarH.y = height - 10;
		addChild(_scrollBarH);
		
		addEventListener( CEvent.ADDED_TO_STAGE, handleAddedToStage);
	}
	
	
	void handleAddedToStage(CEvent e) {
		
		removeEventListener(CEvent.ADDED_TO_STAGE, handleAddedToStage);
		addEventListener(MouseEvent.MOUSE_DOWN, handleMouseDown);
		addEventListener(CEvent.ENTER_FRAME, handleEnterFrame);
	}
	
	
	void handleMouseMove(CEvent e) {
	
		var totalX = mouseX - firstPos.x;
		var totalY = mouseY - firstPos.y;
		
		// movement detection with scrollFactor
		if (useVertical && Mathf.Abs(totalY) > scrollFactor) {
			isDragging = true;
		}
		if (useHorizontal && Mathf.Abs(totalX) > scrollFactor) {
			isDragging = true;
		}
		
		if (isDragging) {
			
			if (useVertical) {
				if (totalY < min.y) {
					totalY = min.y - Mathf.Sqrt(min.y-totalY);
				}
				if (totalY > max.y) {
					totalY = max.y + Mathf.Sqrt(totalY - max.y);
				}
				_content.y = firstPanelPos.y + totalY;
			}
			
			if (useHorizontal) {
				if (totalX < min.x) {
					totalX = min.x - Mathf.Sqrt(min.x-totalX);
				}
				if (totalX > max.x) {
					totalX = max.x + Mathf.Sqrt(totalX - max.x);
				}
				_content.x = firstPanelPos.x + totalX;
			}
			
		}
	}
	
	
	void handleMouseUp(CEvent e) {
		
		if (stage.hasEventListener(MouseEvent.MOUSE_MOVE)){
			stage.removeEventListener(MouseEvent.MOUSE_MOVE, handleMouseMove);
		}
		
		isDragging = false;
		// setting inertia power
		if (useVertical) {
			inertia.y = diff.y;
		}
		if (useHorizontal) {
			inertia.x = diff.x;
		}
		
		stage.removeEventListener(MouseEvent.MOUSE_UP, handleMouseUp);
	}
	
	
	void handleMouseDown(CEvent e) {
		
		if ( !stage.hasEventListener(MouseEvent.MOUSE_MOVE) ) {
			stage.addEventListener(MouseEvent.MOUSE_MOVE, handleMouseMove);
			stage.addEventListener(MouseEvent.MOUSE_UP, handleMouseUp);
		}
		
		inertia.y = 0;
		inertia.x = 0;
		
		firstPos.x = mouseX;
		firstPos.y = mouseY;
		
		firstPanelPos.x = _content.x;
		firstPanelPos.y = _content.y;
		
		min.x = Mathf.Min(-_content.x, -_content.width + panelWidth - _content.x);
		min.y = Mathf.Min(-_content.y, -_content.height + panelHeight - _content.y);
		
		max.x = -_content.x;
		max.y = -_content.y;
		
		_scrollBarV.graphics.clear();
		if (useVertical) {
			_scrollBarV.graphics.drawSolidRectangle(Color.gray, 0f, 0f, 6f, panelHeight * Mathf.Max(0f, panelHeight / _content.height));
		}
		
		_scrollBarH.graphics.clear();
		if (useHorizontal) {
			_scrollBarH.graphics.drawSolidRectangle(Color.gray, 0f, 0f, panelWidth * Mathf.Max(0f, panelWidth / _content.width), 6f);
		}
	}
	
	
	void handleEnterFrame( CEvent e ) {
	
		diff.y = mouseY - lastPos.y;
		diff.x = mouseX - lastPos.x;
		
		lastPos.y = mouseY;
		lastPos.x = mouseX;
		
		if (!isDragging) {
			// movements while non dragging
			
			if (useVertical) {
				if (_content.y > 0) {
					inertia.y = 0;
					_content.y *= 0.8f;
					if (_content.y < 1) {
						_content.y = 0;
					}
				}
				
				if (_content.height >= panelHeight && _content.y < panelHeight - _content.height) {
					inertia.y = 0;
					
					var goal = panelHeight - _content.height;
					diff.y = goal - _content.y;
					
					if (diff.y > 1f) {
						diff.y *= 0.2f;
					}
					_content.y += diff.y;
				}
				
				if (_content.height < panelHeight && _content.y < 0) {
					inertia.y = 0;
					_content.y *= 0.8f;
					if (_content.y > -1) {
						_content.y = 0;
					}
				}
				
				if (Mathf.Abs(inertia.y) > 1) {
					if(inertia.y > 100) inertia.y = 100;
					if(inertia.y < -100) inertia.y = -100;
					
					_content.y = Mathf.Round( _content.y += inertia.y );
					inertia.y *= 0.95f;
				} else {
					inertia.y = 0;
				}
				
				if (inertia.y != 0) {
					if (_scrollBarV.alpha < 1) {
						_scrollBarV.alpha = Mathf.Min(1, _scrollBarV.alpha+0.1f);
					}
					_scrollBarV.y = panelHeight * Mathf.Min(1, (-_content.y / _content.height));
				} else {
					if (_scrollBarV.alpha > 0) {
						_scrollBarV.alpha = Mathf.Max(0, _scrollBarV.alpha-0.1f);
					}
				}
			}
			
			if (useHorizontal) {
				if (_content.x > 0) {
					inertia.x = 0;
					_content.x *= 0.8f;
					if (_content.x < 1) {
						_content.x = 0;
					}
				}
				
				if (_content.width >= panelWidth && _content.x < panelWidth - _content.width) {
					inertia.x = 0;
					
					var goal = panelWidth - _content.width;
					diff.x = goal - _content.x;
					
					if (diff.x > 1f) {
						diff.x *= 0.2f;
					}
					_content.x += diff.x;
				}
				
				if (_content.width < panelWidth && _content.x < 0) {
					inertia.x = 0;
					_content.x *= 0.8f;
					if (_content.x > -1) {
						_content.x = 0;
					}
				}
				
				if (Mathf.Abs(inertia.x) > 1) {
					if(inertia.x > 100) inertia.x = 100;
					if(inertia.x < -100) inertia.x = -100;
					
					_content.x = Mathf.Round(_content.x += inertia.x);
					inertia.x *= 0.95f;
				} else {
					inertia.x = 0;
				}
				
				if (inertia.x != 0) {
					if (_scrollBarH.alpha < 1) {
						_scrollBarH.alpha = Mathf.Min(1, _scrollBarH.alpha+0.1f);
					}
					_scrollBarH.x = panelWidth * Mathf.Min(1, (-_content.x / _content.width));
				} else {
					if (_scrollBarH.alpha > 0) {
						_scrollBarH.alpha = Mathf.Max(0, _scrollBarH.alpha-0.1f);
					}
				}
			}
			
		} else {			
			if (useVertical) {
				if (_scrollBarV.alpha < 1) {
					_scrollBarV.alpha = Mathf.Min(1, _scrollBarV.alpha+0.1f);
				}
				_scrollBarV.y = panelHeight * Mathf.Min(1, (-_content.y / _content.height));
			}
			
			if (useHorizontal) {
				if (_scrollBarH.alpha < 1) {
					_scrollBarH.alpha = Mathf.Min(1, _scrollBarH.alpha+0.1f);
				}
				_scrollBarH.x = panelWidth * Mathf.Min(1, (-_content.x / _content.width));
			}
		}
	}
}
