using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CGUIAnimation : EditorWindow {
	public CGUIBasis target;
	public CGUIEditorTextures text;
	int[] curSelected = new int[0];
	bool mouseDown = false;
	GUISkin skin;
	Vector2[] scrollPosition = new Vector2[0];
	int lastAnimationLength = 0;
	string state = "Stop";
	
	[MenuItem("Window/CGUI/CAnim")]
	static void Init () {
		CGUIAnimation window = (CGUIAnimation)EditorWindow.GetWindow (typeof (CGUIAnimation));
	}
	
	public void Update()
	{
		if(!text){
			text = GameObject.Find("CGUIETextures").GetComponent<CGUIEditorTextures>();
		}
		if(!target){
			target = GameObject.Find("CGUIBasis").GetComponent<CGUIBasis>();
		}
	    // This is necessary to make the framerate normal for the editor window.
		if(text.editorStateA == "Play"){
			curSelected[text.curAnimation] = target.gui.list[text.curObj].PlayAnimation(text.curAnimation, (float) EditorApplication.timeSinceStartup, target.gui.list[text.curObj]);
		}
		else if(text.editorStateA == "Stop"){
			
		}
		Repaint();
	}
	
	void OnGUI () {
		Event e = Event.current;
		if(!skin){
			skin = GUI.skin;
		}
		GUI.skin = skin;
		if(e.type == EventType.MouseDown && e.button == 0){
			mouseDown = true;
		}
		if(e.type == EventType.MouseUp && e.button == 0){
			mouseDown = false;
		}
		if(!text){
			text = GameObject.Find("CGUIETextures").GetComponent<CGUIEditorTextures>();
		}
		if(!target){
			target = GameObject.Find("CGUIBasis").GetComponent<CGUIBasis>();
		}
		if(text.selected){
			UpdateScrollPos();
			for(int y = 0; y < target.gui.list[text.curObj].animation.Length; y++){
				Vector2 offset = new Vector2(150, 60*(y+1));
				int amount = target.gui.list[text.curObj].animation[y].frame.Length;
				if(GUI.Button(new Rect(0, offset.y, 50, 50), "-")){
					CopyAnimation(target.gui.list[text.curObj].animation.Length-1, target.gui.list[text.curObj].animation.Length, y);
					break;
				}
				target.gui.list[text.curObj].animation[y].name = EditorGUI.TextArea(new Rect(50, offset.y, 100, 25), target.gui.list[text.curObj].animation[y].name);
				bool frameUsed = false;
				if(curSelected[y] > target.gui.list[text.curObj].animation[y].frame.Length-1){
					curSelected[y] = 0;
				}
				if(target.gui.list[text.curObj].animation[y].frame.Length > 0){
					if(target.gui.list[text.curObj].animation[y].frame[curSelected[y]].state == 2){
						frameUsed = true;
					}
					if(frameUsed){
						if(GUI.Button(new Rect(50, offset.y+25, 50, 25), "-")){
							target.gui.list[text.curObj].animation[y].frame[curSelected[y]].state = 0;
							if(curSelected[y] > 0){
								target.gui.list[text.curObj].animation[y].frame[curSelected[y]].element = new CGUIElements(target.gui.list[text.curObj].animation[y].frame[curSelected[y]-1].element);
								target.gui.list[text.curObj].animation[y].frame[curSelected[y]].element.loc = new Rect(0,0,0,0);
							}
							else{
								target.gui.list[text.curObj].animation[y].frame[curSelected[y]].element = new CGUIElements(target.gui.list[text.curObj]);
								target.gui.list[text.curObj].animation[y].frame[curSelected[y]].element.loc = new Rect(0,0,0,0);
							}
						}
					}
					else{
						if(GUI.Button(new Rect(50, offset.y+25, 50, 25), "+")){
							target.gui.list[text.curObj].animation[y].frame[curSelected[y]].state = 2;
							if(curSelected[y] > 0){
								target.gui.list[text.curObj].animation[y].frame[curSelected[y]].element = new CGUIElements(target.gui.list[text.curObj].animation[y].frame[curSelected[y]-1].element);
								target.gui.list[text.curObj].animation[y].frame[curSelected[y]].element.loc = new Rect(0,0,0,0);
							}
							else{
								target.gui.list[text.curObj].animation[y].frame[curSelected[y]].element = new CGUIElements(target.gui.list[text.curObj]);
								target.gui.list[text.curObj].animation[y].frame[curSelected[y]].element.loc = new Rect(0,0,0,0);
							}
						}
					}
					target.gui.list[text.curObj].animation[y].rate = EditorGUI.IntField(new Rect(100, offset.y+25, 50, 25), target.gui.list[text.curObj].animation[y].rate);
					int nextState = 0;
					bool cur = false;
					if(text.curAnimation == y){
						cur = true;
					}
					bool lastCur = cur;
					cur = EditorGUI.Toggle(new Rect(offset.x+(position.width-offset.x-25), offset.y, 25, 20), "", cur);
					if(cur != lastCur){
						text.curAnimation = y;
					}
					text.curFrame = curSelected[text.curAnimation];
					scrollPosition[y] = GUI.BeginScrollView(new Rect(offset.x, offset.y, (position.width-offset.x - 50), (45)), scrollPosition[y], new Rect(0,0, (3000), 45), true, false);
					for(int x = 0; x < amount; x++){
						Texture myTexture = text.keyFrameTexture;
						nextState = target.gui.list[text.curObj].animation[y].frame[x].state;
						if(nextState == 0){
							myTexture = text.keyFrameTexture;
						}
						else if(nextState == 1){
							myTexture = text.lineKeyFrameTexture;
						}
						else if(nextState == 2){
							myTexture = text.usedKeyFrameTexture;
						}
						if(x == curSelected[y]){
							myTexture = text.selectedKeyFrameTexture;
						}
						GUI.DrawTexture(new Rect(x*15, 0, 15, 30), myTexture);
						if(mouseDown){	
							if(e.mousePosition.x >= x*15 && e.mousePosition.x <= x*15+15 && e.mousePosition.y >= 0 && e.mousePosition.y <= 30){
								curSelected[y] = x;
							}
						}
					}
					GUI.EndScrollView();
				}
				int curLength = target.gui.list[text.curObj].animation[y].frame.Length;
				int lastLength = curLength;
				curLength = EditorGUI.IntField(new Rect(offset.x+(position.width-offset.x-25), offset.y+25, 25, 20), "", curLength);
				if(curLength != lastLength){
					CopyFrames(curLength, target.gui.list[text.curObj].animation[y].frame.Length, 0,y);
				}
			}
			Vector2 offset2 = new Vector2(150, 60*(target.gui.list[text.curObj].animation.Length+1));
			if(GUI.Button(new Rect(0, offset2.y, 150, 50), "+")){
				CopyAnimation(target.gui.list[text.curObj].animation.Length+1, target.gui.list[text.curObj].animation.Length, 0);
			}
			int amount2 = (int) (3000)/15;
			scrollPosition[lastAnimationLength] = GUI.BeginScrollView(new Rect(offset2.x, offset2.y, (position.width-offset2.x), (45)), scrollPosition[lastAnimationLength], new Rect(0,0, (3000), 45), true, false);
			for(int x = 0; x < amount2; x++){
				Texture myTexture = text.keyFrameTexture;
				if(curSelected[lastAnimationLength] == x){
					myTexture = text.selectedKeyFrameTexture;
				}
				GUI.DrawTexture(new Rect(x*15, 0, 15, 30), myTexture);
				if(mouseDown){	
					if(e.mousePosition.x >= x*15 && e.mousePosition.x <= x*15+15 && e.mousePosition.y >= 0 && e.mousePosition.y <= 30){
						curSelected[lastAnimationLength] = x;
					}
				}
			}
			GUI.EndScrollView();
			UpdateFrames();
		}
		GUI.Box(new Rect(0,0, position.width, 50), "");
		Rect tempLoc = new Rect(position.width/2-45, 5, 40, 40);
		if(GUI.Button(tempLoc, text.playTexture)){
			if(target.gui.list[text.curObj].animation[text.curAnimation].frame.Length > 0){
				text.editorState = "Animator";
				text.editorStateA = "Play";
			}
			state = "Play";
		}
		tempLoc = new Rect(tempLoc.x+45, tempLoc.y, 40, 40);
		if(GUI.Button(tempLoc, text.pauseTexture)){
			if(target.gui.list[text.curObj].animation[text.curAnimation].frame.Length > 0){
				text.editorState = "Animator";
				text.editorStateA = "Pause";
			}
			state = "Pause";
		}
		tempLoc = new Rect(tempLoc.x+45, tempLoc.y, 40, 40);
		if(GUI.Button(tempLoc, text.stopTexture)){
			if(target.gui.list[text.curObj].animation[text.curAnimation].frame.Length > 0){
				text.editorState = "Animator";
				text.editorStateA = "Stop";
				target.gui.list[text.curObj].StopAnimation(text.curAnimation);
			}
			
			text.curAnimObj = text.curObj;
			state = "Stop";
		}
		GUI.skin = text.emptySkin;
		GUI.Button(new Rect(0, 0, position.width, position.height), "");
	}
	
	void UpdateFrames () {
		if(target.gui.list[text.curObj].animation.Length <= text.curAnimation){
			text.curAnimation = 0;
		}
		if(target.gui.list[text.curObj].animation.Length > text.curAnimation){
			CGUIElements myElement = new CGUIElements(target.gui.list[text.curObj]);
			for(int x = 0; x < target.gui.list[text.curObj].animation[text.curAnimation].frame.Length; x++){
				if(target.gui.list[text.curObj].animation[text.curAnimation].frame[x].state == 2){
					myElement = new CGUIElements(target.gui.list[text.curObj].animation[text.curAnimation].frame[x].element);
				}
				else if(x>0){
					target.gui.list[text.curObj].animation[text.curAnimation].frame[x].element = new CGUIElements(myElement);
				}
				else{
					target.gui.list[text.curObj].animation[text.curAnimation].frame[x].element = new CGUIElements(target.gui.list[text.curObj]);
				}
			}
		}
	}
	
	void UpdateScrollPos () {
		if(lastAnimationLength != target.gui.list[text.curObj].animation.Length){
			lastAnimationLength = target.gui.list[text.curObj].animation.Length;
		}
		if(scrollPosition.Length != lastAnimationLength+1){
			scrollPosition = new Vector2[lastAnimationLength+1];
			curSelected = new int[lastAnimationLength+1];
		}
	}
	
	void CopyAnimation (int newLength, int oldLength, int removeInt){
		CGUIAnimationManager[] myList = new CGUIAnimationManager[oldLength];
		if(newLength < oldLength){
			for(int x = 0; x < target.gui.list[text.curObj].animation.Length; x++){
				myList[x] = target.gui.list[text.curObj].animation[x];
			}
			target.gui.list[text.curObj].animation = new CGUIAnimationManager[newLength];
			int y = 0;
			for(int x = 0; x < myList.Length; x++){
				if(x != removeInt){
					target.gui.list[text.curObj].animation[y] = myList[x];
					y++;
				}
			}
		}
		else{
			for(int x = 0; x < target.gui.list[text.curObj].animation.Length; x++){
				myList[x] = target.gui.list[text.curObj].animation[x];
			}
			target.gui.list[text.curObj].animation = new CGUIAnimationManager[newLength];
			for(int x = 0; x < target.gui.list[text.curObj].animation.Length; x++){
				if(x < myList.Length){
					target.gui.list[text.curObj].animation[x] = myList[x];
				}
				else{
					target.gui.list[text.curObj].animation[x] = new CGUIAnimationManager();
					target.gui.list[text.curObj].animation[x].name = "New Animation";
				}
			}
		}
	}
	
	void CopyFrames (int newLength, int oldLength, int curFrame, int anim){
		Frame[] myList = new Frame[oldLength];
		if(newLength < oldLength){
			for(int x = 0; x < target.gui.list[text.curObj].animation[anim].frame.Length; x++){
				myList[x] = target.gui.list[text.curObj].animation[anim].frame[x];
			}
			target.gui.list[text.curObj].animation[anim].frame = new Frame[newLength];
			for(int x = 0; x < target.gui.list[text.curObj].animation[anim].frame.Length; x++){
				target.gui.list[text.curObj].animation[anim].frame[x] = myList[x];
			}
		}
		else{
			for(int x = 0; x < target.gui.list[text.curObj].animation[anim].frame.Length; x++){
				myList[x] = target.gui.list[text.curObj].animation[anim].frame[x];
			}
			target.gui.list[text.curObj].animation[anim].frame = new Frame[newLength];
			int y = 0;
			for(int x = 0; x < target.gui.list[text.curObj].animation[anim].frame.Length; x++){			
				target.gui.list[text.curObj].animation[anim].frame[x] = new Frame();
				target.gui.list[text.curObj].animation[anim].frame[x].curFrame = x;
				target.gui.list[text.curObj].animation[anim].frame[x].element = new CGUIElements(target.gui.list[text.curObj]);
			}
			for(int x = 0; x < myList.Length; x++){
				int z = 0;
				z = myList[x].curFrame;
				target.gui.list[text.curObj].animation[anim].frame[myList[x].curFrame].element = new CGUIElements(myList[x].element);
				target.gui.list[text.curObj].animation[anim].frame[myList[x].curFrame].curFrame = z;
				target.gui.list[text.curObj].animation[anim].frame[myList[x].curFrame].state = 2;
				target.gui.list[text.curObj].animation[anim].frame[myList[x].curFrame].element.selected = true;
			}
			CGUIElements copyElement = null;
			for(int x = 0; x < target.gui.list[text.curObj].animation[anim].frame.Length; x++){
				if(target.gui.list[text.curObj].animation[anim].frame[x].element != target.gui.list[text.curObj]){
					copyElement = new CGUIElements(target.gui.list[text.curObj].animation[anim].frame[x].element);
				}
				if(target.gui.list[text.curObj].animation[anim].frame[x].state == 0){
					target.gui.list[text.curObj].animation[anim].frame[x].element = new CGUIElements(copyElement);
					target.gui.list[text.curObj].animation[anim].frame[x].element.selected = true;
				}
			}
		}
	}
}