using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CGUISystem : EditorWindow {
	
	public bool displayMenu;
	public string menuType = "";
	public Rect menuLoc;
	public GUISkin skin;
	public GUISkin customSkin;
	public CGUIBasis target;
	public List<int> curObj = new List<int> (0);
	int curObjLength = 0;
	int obj = 0;
	int scalingObj = 0;
	List<int> sortedList;
	public bool sorted = false;
	public bool moveObj;
	public bool mouseDown = false;
	public bool cameraView = false;
	public bool displayGrid = false;
	public string playAnimation = "Stop";
	RenderTexture renderTexture;
	public bool scaleMode = false;
	public Vector2 scaling = Vector2.zero;
	public Rect originalSize;
	public Vector2 displacement;
	public CGUIEditorTextures text;
	public Vector2 tempLocGlob;
	bool openCanvas = false;
	bool editStyle = false;
	int[] windowState = new int[1];
	bool[] windowBool;
	bool dragSelect = false;
	Vector2 selectLoc;
	CGUIElements[] myList;
	static CGUIElements myElement;
	static CGUIElements curElement;
	static CGUIElements objElement;
	static CGUIElements[] extraElements;
	
	[MenuItem("Window/CGUI/CSystem")]
	static void Init () {
		CGUISystem window = (CGUISystem)EditorWindow.GetWindow (typeof (CGUISystem));
	}
	void OnGUI () {
		if(!skin){
			skin = GUI.skin;
		}
		if(!text){
			text = GameObject.Find("CGUIETextures").GetComponent<CGUIEditorTextures>();
		}
		if(!target){
			target = GameObject.Find("CGUIBasis").GetComponent<CGUIBasis>();
		}
		if(target.gui.list.Length == 0){
			curObjLength = 0;
			obj = 0;
		}
		DrawSetup();
		if(!mouseDown){
			openCanvas = true;
		}
		if(target){
			target = EditorGUILayout.ObjectField("Object : ", target, typeof(CGUIBasis), true) as CGUIBasis;
		}
		GUI.skin = skin;
		if(playAnimation == "Stop"){
			DrawGUIE();
			EventHandling();
		}
		else{
			DrawGUI();
		}
		if(displayGrid)
			Grid();
		if(displayMenu)
			DrawMenu();
		GUI.skin = text.emptySkin;
		GUI.Button(new Rect(0, 0, position.width, position.height), "");
	}
	
	public void Update()
	{
		if(!text){
			text = GameObject.Find("CGUIETextures").GetComponent<CGUIEditorTextures>();
		}
		if(!target){
			target = GameObject.Find("CGUIBasis").GetComponent<CGUIBasis>();
		}
		if(text){
			if(text.curObj >= target.gui.list.Length){
				text.curObj = 0;
			}
			if(target.gui.list[text.curObj].animation.Length <= text.curAnimation){
				text.curAnimation = 0;
				if(target.gui.list[text.curObj].animation.Length == 0){
					text.editorState = "Editor";
				}
			}
			else if(target.gui.list[text.curObj].animation[text.curAnimation].frame.Length == 0){
				if(text.editorStateA != "Stop"){
					text.editorStateA = "Stop";
				}
			}
			
		}
	    // This is necessary to make the framerate normal for the editor window.
	    Repaint();
	    UpdateAnimation();
	    if(Application.isPlaying){
		curObj.Clear();
		curObjLength = 0;
	    }
	}
	
	void UpdateAnimation(){
		CGUIBasis target = GameObject.Find("CGUIBasis").GetComponent<CGUIBasis>();
		if(target){
			if(playAnimation == "Play"){
				for(int x = 0; x < target.gui.list.Length; x++){
					
				}
			}
			else if(playAnimation == "Pause"){
			
			}
			else if(playAnimation == "Stop"){
				for(int x = 0; x < target.gui.list.Length; x++){
				}
			}
		}
	}
	
	void EventHandling () {
		Event e = Event.current;
		bool overMenu = false;
		bool checkFirstOpen = false;
		if(curObjLength > 0){
			if(text.editorState == "Editor"){
				objElement = target.gui.list[obj];
			}
			else{
				objElement = target.gui.list[obj].animation[text.curAnimation].frame[text.curFrame].element;
			}
		}
		if(e.type == EventType.MouseDown && e.button == 0){
			mouseDown = true;
			checkFirstOpen = true;
			dragSelect = false;
			selectLoc = e.mousePosition;
		}
		if(mouseDown){
			if(displayMenu){
				if(e.mousePosition.x >= menuLoc.x && e.mousePosition.x <= menuLoc.x+menuLoc.width && e.mousePosition.y >= menuLoc.y && e.mousePosition.y <= menuLoc.y + menuLoc.height || e.mousePosition.y == -19){
					openCanvas = false;
					overMenu = true;	
				}
			}
		}
		if(e.type == EventType.MouseUp && e.button == 0){
			mouseDown = false;
			dragSelect = false;
		}
		if(dragSelect){
			DrawSelect();
		}
		if(curObjLength > 0){
			text.curObj = curObj[0];
			text.selected = true;
			if(e.isKey){
				for(int x = 0; x < curObjLength; x++){
					if(text.editorState == "Editor"){
						curElement = target.gui.list[curObj[x]];
					}
					else if(text.editorState == "Animator"){
						curElement = target.gui.list[curObj[x]].animation[text.curAnimation].frame[text.curFrame].element;
					}
					Rect loc = curElement.loc;
					if(text.editorState == "Animator"){
						loc = target.gui.list[curObj[x]].loc;
					}
					if(e.keyCode == KeyCode.RightArrow){
						curElement.loc = new Rect(loc.x + 1, loc.y, loc.width, loc.height);
					}
					if(e.keyCode == KeyCode.LeftArrow){
						curElement.loc = new Rect(loc.x - 1, loc.y, loc.width, loc.height);
					}
					if(e.keyCode == KeyCode.UpArrow){
						curElement.loc = new Rect(loc.x, loc.y - 1, loc.width, loc.height);
					}
					if(e.keyCode == KeyCode.DownArrow){
						curElement.loc = new Rect(loc.x, loc.y + 1, loc.width, loc.height);
					}
				}
			}
		}
		else{
			text.selected = false;
		}
		if(moveObj && !overMenu){
			bool snapToGrid = false;
			if(e.shift){
				snapToGrid = true;
			}
			Vector2[] objDisp = new Vector2[curObjLength];
			for(int x = 0; x < curObjLength; x++){
				if(text.editorState == "Editor"){
					curElement = target.gui.list[curObj[x]];
				}
				else if(text.editorState == "Animator"){
					curElement = target.gui.list[curObj[x]].animation[text.curAnimation].frame[text.curFrame].element;
				}
				objDisp[x] = new Vector2(curElement.loc.x - objElement.loc.x, curElement.loc.y - objElement.loc.y);
			}
			if(objElement != null){
				objElement.selected = true;
			}
			if(curObjLength > 0){
				if(!snapToGrid){
					objElement.loc = new Rect(e.mousePosition.x - tempLocGlob.x - displacement.x, e.mousePosition.y - tempLocGlob.y - displacement.y, objElement.loc.width, objElement.loc.height);
					if(objElement.linkedElement != null){
						if(objElement.linkedElement.state == 8){
							objElement.loc = new Rect(objElement.loc.x + objElement.linkedElement.scrollView.scrollPos.x - objElement.linkedElement.loc.x, objElement.loc.y + objElement.linkedElement.scrollView.scrollPos.y  - objElement.linkedElement.loc.y, objElement.loc.width, objElement.loc.height);
						}
					}
				}
				else{
					Vector2 gridLoc = new Vector2((int)((e.mousePosition.x - tempLocGlob.x - displacement.x)/text.nodeSize), (int)((e.mousePosition.y - tempLocGlob.y - displacement.y)/text.nodeSize)); 
					if(objElement.linkedElement != null){
						if(objElement.linkedElement.state == 8){
							gridLoc = new Vector2((int)((e.mousePosition.x - tempLocGlob.x - objElement.linkedElement.loc.x - displacement.x + objElement.linkedElement.scrollView.scrollPos.x)/text.nodeSize), (int)((e.mousePosition.y - tempLocGlob.y - objElement.linkedElement.loc.y - displacement.y + objElement.linkedElement.scrollView.scrollPos.y)/text.nodeSize)); 
						}
					}
					objElement.loc = new Rect(gridLoc.x*text.nodeSize, gridLoc.y*text.nodeSize, objElement.loc.width, objElement.loc.height);
				}
				for(int x = 0; x < curObjLength; x++){
					if(text.editorState == "Editor"){
						curElement = target.gui.list[curObj[x]];
					}
					else if(text.editorState == "Animator"){
						curElement = target.gui.list[curObj[x]].animation[text.curAnimation].frame[text.curFrame].element;
					}
					if(curObj[x] != obj){
						if(curElement.linkedElement != null && curElement.linkedElement.selected){
							
						}
						else{
							curElement.selected = true;
							if(!snapToGrid){
								curElement.loc = new Rect(e.mousePosition.x - tempLocGlob.x - displacement.x + (objDisp[x].x), e.mousePosition.y - tempLocGlob.y - displacement.y + (objDisp[x].y), curElement.loc.width, curElement.loc.height);
								if(curElement.linkedElement != null){
									if(curElement.linkedElement.state == 8){
										curElement.loc = new Rect(e.mousePosition.x + (objDisp[x].x) - tempLocGlob.x - displacement.x + curElement.linkedElement.scrollView.scrollPos.x - curElement.linkedElement.loc.x, e.mousePosition.y + (objDisp[x].y) - tempLocGlob.y - displacement.y + curElement.linkedElement.scrollView.scrollPos.y - curElement.linkedElement.loc.y, curElement.loc.width, curElement.loc.height);
									}
								}
							}
							else{
								curElement.loc = new Rect(objDisp[x].x + objElement.loc.x, objDisp[x].y + objElement.loc.y, curElement.loc.width, curElement.loc.height);
								
							}
						}
					}
				}
				openCanvas = false;
			}
		}
		for(int x = 0; x < curObjLength; x++){
			DrawCorners(e, overMenu, x);
		}
		if(e.type == EventType.MouseUp && e.button == 0){
			for(int x = 0; x < curObjLength; x++){
				if(text.editorState == "Editor"){
					curElement = target.gui.list[curObj[x]];
				}
				else if(text.editorState == "Animator"){
					curElement = target.gui.list[curObj[x]].animation[text.curAnimation].frame[text.curFrame].element;
				}
				curElement.active = false;
			}
			mouseDown = false;
			moveObj = false;
			scaleMode = false;
		}
		if(mouseDown && openCanvas && !dragSelect){
			for(int x = 0; x < curObjLength; x++){
				if(text.editorState == "Editor"){
					curElement = target.gui.list[curObj[x]];
				}
				else if(text.editorState == "Animator"){
					curElement = target.gui.list[curObj[x]].animation[text.curAnimation].frame[text.curFrame].element;
				}
				curElement.selected = false;
			}
			curObjLength = 0;
			curObj.Clear();
			displayMenu = false;
			if(checkFirstOpen){
				dragSelect = true;
			}
		}
		if(mouseDown && !overMenu){
			displayMenu = false;
		}
		if(!scaleMode && !mouseDown){
			for(int x = 0; x < curObjLength; x++){
				if(text.editorState == "Editor"){
					curElement = target.gui.list[curObj[x]];
				}
				else if(text.editorState == "Animator"){
					curElement = target.gui.list[curObj[x]].animation[text.curAnimation].frame[text.curFrame].element;
				}
				if(curElement.loc.width < 0){
					if(text.editorState == "Animator"){
					}
					else{
						curElement.loc = new Rect(curElement.loc.x - curElement.loc.width*-1, curElement.loc.y, -curElement.loc.width, curElement.loc.height);
					}
				}
				if(curElement.loc.height < 0){
					if(text.editorState == "Animator"){
					}
					else{
						curElement.loc = new Rect(target.gui.list[curObj[x]].loc.x+curElement.loc.x, target.gui.list[curObj[x]].loc.y+curElement.loc.y - (target.gui.list[curObj[x]].loc.height+curElement.loc.height)*-1, target.gui.list[curObj[x]].loc.width+curElement.loc.width, -(curElement.loc.height+target.gui.list[curObj[x]].loc.height));
					}
				}
			}
		}
		if(e.type == EventType.MouseDown && e.button == 1){
			if(scaleMode){
				objElement.loc = originalSize;
			}
			else{
				if(openCanvas){
					menuType = "OpenCanvas";
				}
				displayMenu = true;
				menuLoc = new Rect(e.mousePosition.x + 1, e.mousePosition.y, 150, 170);
			}
		}
	}
	
	void DrawGUI () {
		GUI.enabled = true;
		if(!sorted){
			sortedList = new List<int>(new int[target.gui.list.Length]);
			for(int x = 0; x < target.gui.list.Length; x++){
				sortedList[(target.gui.list.Length-1) - target.gui.list[x].depth] = x;
			}
		}
		Event e = Event.current;
		bool stillSelected = false;
		target.DrawElements(tempLocGlob);
		GUI.color = Color.white;
		GUI.backgroundColor = Color.white;
		GUI.contentColor = Color.white;
	}
	
	void DrawSelect () {
		Event e = Event.current;
		Rect myRect;
		if(e.mousePosition.x > selectLoc.x){
			myRect = new Rect(selectLoc.x, 0, e.mousePosition.x - selectLoc.x, 0);
		}
		else{
			myRect = new Rect(e.mousePosition.x, 0, selectLoc.x - e.mousePosition.x, 0);
		}
		if(e.mousePosition.y > selectLoc.y){
			myRect = new Rect(myRect.x, selectLoc.y, myRect.width, e.mousePosition.y - selectLoc.y);
		}
		else{
			myRect = new Rect(myRect.x, e.mousePosition.y, myRect.width, selectLoc.y - e.mousePosition.y);
		}
		GUI.DrawTexture(myRect, text.selectTexture);
		GUI.DrawTexture(new Rect(myRect.x - 1, myRect.y - 1, myRect.width+2, 1), text.blackOutlineTexture);
		GUI.DrawTexture(new Rect(myRect.x - 1, myRect.y - 1, 1, myRect.height+2), text.blackOutlineTexture);
		GUI.DrawTexture(new Rect(myRect.x + myRect.width, myRect.y - 1, 1, myRect.height+2), text.blackOutlineTexture);
		GUI.DrawTexture(new Rect(myRect.x - 1, myRect.y + myRect.height, myRect.width+2, 1), text.blackOutlineTexture);
		for(int x = 0; x < target.gui.list.Length; x++){
			Rect loc = new Rect(target.gui.list[x].loc.x + tempLocGlob.x, target.gui.list[x].loc.y + tempLocGlob.y, target.gui.list[x].loc.width, target.gui.list[x].loc.height);
			if(loc.x > myRect.x && myRect.x+myRect.width > loc.x+loc.width && loc.y > myRect.y && myRect.y+myRect.height > loc.y+loc.height){
				if(!target.gui.list[x].selected){
					target.gui.list[x].selected = true;
					curObj.Add(x);
					curObjLength++;
				}
			}
			else{
				if(target.gui.list[x].selected){
					for(int y = 0; y < curObjLength; y++){
						if(curObj[y] == x){
							curObj.RemoveAt(y);
							y--;
							curObjLength--;
						}
					}
					target.gui.list[x].selected = false;
				}
			}
		}
	}
	
	void DrawGUIE () {
		if(!sorted){
			sortedList = new List<int>(new int[target.gui.list.Length]);
			for(int x = 0; x < target.gui.list.Length; x++){
				sortedList[(target.gui.list.Length-1) - target.gui.list[x].depth] = x;
			}
		}
		Event e = Event.current;
		bool mousePressed = false;
		for(int x = 0; x < target.gui.list.Length; x++){
			if(e.type == EventType.MouseDown){
				bool display = false;
				if(text.editorState == "Editor"){
					display = true;
					curElement = target.gui.list[sortedList[x]];
				}
				else if(text.editorState == "Animator"){
					if(sortedList[x] == text.curAnimObj){
						display = true;
						curElement = target.gui.list[sortedList[x]].animation[text.curAnimation].frame[text.curFrame].element;
					}
				}
				if(display){
					Rect myLoc = curElement.loc;
					if(curElement.linkedElement != null){
						if(curElement.linkedElement.state == 8){
							myLoc = new Rect(myLoc.x-curElement.linkedElement.scrollView.scrollPos.x+curElement.linkedElement.loc.x, myLoc.y-curElement.linkedElement.scrollView.scrollPos.y+curElement.linkedElement.loc.y, myLoc.width, myLoc.height);
						}
					}
					myLoc = new Rect(myLoc.x + tempLocGlob.x, myLoc.y + tempLocGlob.y, myLoc.width, myLoc.height);
					if(text.editorState == "Animator"){
						myLoc = new Rect(myLoc.x + target.gui.list[sortedList[x]].loc.x, myLoc.y + target.gui.list[sortedList[x]].loc.y, myLoc.width+target.gui.list[sortedList[x]].loc.width, myLoc.height+target.gui.list[sortedList[x]].loc.height);
					}
					mousePressed = true;
					if(e.button == 0){
						if(e.mousePosition.x > myLoc.x && e.mousePosition.x < myLoc.x + myLoc.width && e.mousePosition.y > myLoc.y && e.mousePosition.y < myLoc.y + myLoc.height){
							openCanvas = false;
							if(displayMenu && e.mousePosition.x >= menuLoc.x && e.mousePosition.x <= menuLoc.x+menuLoc.width && e.mousePosition.y >= menuLoc.y && e.mousePosition.y <= menuLoc.y + menuLoc.height){
							
							}
							else{
								if(e.shift){
									if(curElement.selected){
										for(int y = 0; y < curObjLength; y++){
											if(sortedList[x] == curObj[y]){
												curObj.RemoveAt(y);
												curObjLength--;
												y--;
											}
										}
										curElement.selected = false;
									}
									else{
										curObj.Add(sortedList[x]);
										curObjLength++;
										curElement.selected = true;
										moveObj = true;
										obj = sortedList[x];
										displacement = new Vector2(e.mousePosition.x - myLoc.x, e.mousePosition.y - myLoc.y);
									}
								}
								else if(e.alt){
									if(curElement.selected){
										obj = sortedList[x];
										displacement = new Vector2(e.mousePosition.x - myLoc.x, e.mousePosition.y - myLoc.y);
									}
								}
								else{
									if(curElement.selected){
										moveObj = true;
										displacement = new Vector2(e.mousePosition.x - myLoc.x, e.mousePosition.y - myLoc.y);
										obj = sortedList[x];
									}
									else{
										moveObj = true;
										displacement = new Vector2(e.mousePosition.x - myLoc.x, e.mousePosition.y - myLoc.y);
										obj = sortedList[x];
										for(int y = 0; y < curObjLength; y++){
											target.gui.list[curObj[y]].selected = false;
										}
										curObj.Clear();
										curObjLength = 1;
										curObj.Add(sortedList[x]);
									}
								}
							}
						}
					}
					else if(e.button == 1){
						if(e.mousePosition.x > myLoc.x && e.mousePosition.x < myLoc.x + myLoc.width && e.mousePosition.y > myLoc.y && e.mousePosition.y < myLoc.y + myLoc.height){
							if(curObjLength < 2 || !curElement.selected){
								menuType = "OverObject";
								curObj.Clear();
								curObjLength = 1;
								curObj.Add(sortedList[x]);
								obj = sortedList[x];
								displayMenu = true;
								openCanvas = false;
							}
							else{
								menuType = "OverGroup";
								displayMenu = true;
								openCanvas = false;
							}
						}
					}
				}
			}
		}
		if(text.editorState == "Editor"){
			GUI.enabled = false;
			target.DrawElements(tempLocGlob);
		}
		else if(text.editorState == "Animator"){
			if(text.editorStateA == "Stop"){
				GUI.enabled = false;
				target.gui.list[text.curAnimObj].animation[text.curAnimation].frame[text.curFrame].element.DrawElement(new Rect(tempLocGlob.x+target.gui.list[text.curAnimObj].loc.x,tempLocGlob.y+target.gui.list[text.curAnimObj].loc.y,target.gui.list[text.curAnimObj].loc.width, target.gui.list[text.curAnimObj].loc.height), e);
			}
			else if(text.editorStateA == "Play"){
				GUI.enabled = true;
				target.gui.list[text.curAnimObj].animation[text.curAnimation].frame[text.curFrame].element.DrawElement(new Rect(tempLocGlob.x+target.gui.list[text.curAnimObj].loc.x,tempLocGlob.y+target.gui.list[text.curAnimObj].loc.y,target.gui.list[text.curAnimObj].loc.width, target.gui.list[text.curAnimObj].loc.height), e);
			}
		}
		GUI.enabled = true;
		if(mousePressed){
			e.type = EventType.MouseDown;
		}
		GUI.color = Color.white;
		GUI.backgroundColor = Color.white;
		GUI.contentColor = Color.white;
	}
	
	void DrawMenu () {
		if(menuType == "OpenCanvas"){
			menuLoc = new Rect(menuLoc.x, menuLoc.y, 150, 145); 
			if(menuLoc.y + menuLoc.height > position.height){
				menuLoc = new Rect(menuLoc.x, position.height - menuLoc.height, menuLoc.width, menuLoc.height);
			}
			if(menuLoc.x + menuLoc.width > position.width){
				menuLoc = new Rect(position.width - menuLoc.width, menuLoc.y, menuLoc.width, menuLoc.height);
			}
			GUI.Box(menuLoc, "");
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y + 5, 140, 25), "Display Grid")){
				displayGrid = !displayGrid;
				openCanvas = false;
			}
			GUI.Label(new Rect(menuLoc.x + 5, menuLoc.y + 35, 95, 20), "Node Size");
			text.nodeSize = EditorGUI.IntField(new Rect(menuLoc.x + 100, menuLoc.y + 30, 45, 25), text.nodeSize);
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y + 55, 140, 25), "Add Element")){
				openCanvas = false;
				CopyGUI(target.gui.list.Length+1,target.gui.list.Length);
				displayMenu = false;
			}
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y + 80, 140, 25), "Paste")){
				openCanvas = false;
				if(myElement != null){
					CopyGUIPaste(target.gui.list.Length+1, target.gui.list.Length);
				}
			}
			GUI.DrawTexture(new Rect(menuLoc.x + 5, menuLoc.y + 105, 140, 10), text.spaceTexture);
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y + 115, 140, 25), "")){
				openCanvas = false;
			}
		}
		else if(menuType == "OverObject"){
			if(objElement.linkedElement != null){
				menuLoc = new Rect(menuLoc.x, menuLoc.y, 150, 265);
			}
			else{
				menuLoc = new Rect(menuLoc.x, menuLoc.y, 150, 265);
			}
			if(menuLoc.y + menuLoc.height > position.height){
				menuLoc = new Rect(menuLoc.x, position.height - menuLoc.height, menuLoc.width, menuLoc.height);
			}
			if(menuLoc.x + menuLoc.width > position.width){
				menuLoc = new Rect(position.width - menuLoc.width, menuLoc.y, menuLoc.width, menuLoc.height);
			}
			GUI.Box(menuLoc, "");
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y + 5, 140, 25), "Edit Style")){
				menuType = "EditStyle";
				openCanvas = false;
			}
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y + 30, 140, 25), "Send To Back")){
				openCanvas = false;
				SendToBack();
			}
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y + 55, 140, 25), "Bring To Front")){
				openCanvas = false;
				BringToFront();
			}
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y + 80, 140, 25), "Send Backward")){
				openCanvas = false;
				SendBackward();
			}
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y + 105, 140, 25), "Bring Forward")){
				openCanvas = false;
				BringForward();
			}
			GUI.DrawTexture(new Rect(menuLoc.x + 5, menuLoc.y + 130, 140, 10), text.spaceTexture);
			if(objElement.linkedElement != null){
				if(GUI.Button(new Rect(menuLoc.x, menuLoc.y+140, 140, 25), "Remove From Group")){
					objElement.linkedElement = null;
				}
			}
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y+170, 140, 25), "Copy")){
				openCanvas = true;
				myElement = new CGUIElements();
				myElement = target.gui.list[obj];
				int linkedObject = 0;
				List<int> links = new List<int>(0);
				if(myElement.linkedTo){
					for(int x = 0; x < target.gui.list.Length; x++){
						if(target.gui.list[x].linkedElement == target.gui.list[obj]){
							linkedObject++;
							links.Add(x);
						}
					}
					extraElements = new CGUIElements[linkedObject];
					for(int x = 0; x < linkedObject; x++){
						extraElements[x] = target.gui.list[links[x]];
					}
				}
			}
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y+200, 140, 25), "Paste")){
				openCanvas = true;
				if(myElement != null){
					CopyGUIPaste(target.gui.list.Length+1, target.gui.list.Length);
				}
			}
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y+230, 140, 25), "Delete")){
				openCanvas = true;
				CopyGUI(target.gui.list.Length-1, target.gui.list.Length);
				displayMenu = false;
				curObjLength = 0;
				curObj.Clear();
			}
		}
		else if(menuType == "EditStyle"){
			if(menuLoc.width != 500){
				menuLoc = new Rect(menuLoc.x, menuLoc.y, 500, 300);
			}
			if(menuLoc.y + menuLoc.height > position.height){
				menuLoc = new Rect(menuLoc.x, position.height - menuLoc.height, menuLoc.width, menuLoc.height);
			}
			if(menuLoc.x + 500 > position.width){
				menuLoc = new Rect(position.width - 500, menuLoc.y, menuLoc.width, menuLoc.height);
			}
			if(menuLoc.x < 0){
				menuLoc = new Rect(0, menuLoc.y, menuLoc.width, menuLoc.height);
			}
			if(menuLoc.y < 0){
				menuLoc = new Rect(menuLoc.x, 0, menuLoc.width, menuLoc.height);
			}
			BeginWindows ();
			menuLoc = GUILayout.Window(1, menuLoc, EditStyleWind, "Edit Style");
			EndWindows();
		}
		else if(menuType == "OverGroup"){
			menuLoc = new Rect(menuLoc.x, menuLoc.y, 150, 95); 
			if(menuLoc.y + menuLoc.height > position.height){
				menuLoc = new Rect(menuLoc.x, position.height - menuLoc.height, menuLoc.width, menuLoc.height);
			}
			if(menuLoc.x + menuLoc.width > position.width){
				menuLoc = new Rect(position.width - menuLoc.width, menuLoc.y, menuLoc.width, menuLoc.height);
			}
			GUI.Box(menuLoc, "");
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y + 5, 140, 25), "Group")){
				for(int x = 0; x < curObjLength; x++){
					if(text.editorState == "Editor"){
						curElement = target.gui.list[curObj[x]];
					}
					else if(text.editorState == "Animator"){
						curElement = target.gui.list[curObj[x]].animation[text.curAnimation].frame[text.curFrame].element;
					}
					if(curObj[x] != obj){
						curElement.linkedElement = target.gui.list[obj];
						curElement.linkedTo = false;
						curElement.linkedOriginalLoc = new Vector2(objElement.loc.x, objElement.loc.y);
					}
					else{
					}
				}
			}
			GUI.Label(new Rect(menuLoc.x + 5, menuLoc.y + 35, 95, 20), "Node Size");
			text.nodeSize = EditorGUI.IntField(new Rect(menuLoc.x + 100, menuLoc.y + 30, 45, 25), text.nodeSize);
			if(GUI.Button(new Rect(menuLoc.x + 5, menuLoc.y + 65, 140, 25), "Delete")){
				openCanvas = true;
				for(int x = 0; x <= curObjLength; x++){
					x = 0;
					obj = curObj[x];
					CopyGUI(target.gui.list.Length-1, target.gui.list.Length);
					curObj.RemoveAt(x);
					curObjLength--;
					for(int y = 0; y < curObjLength; y++){
						if(curObj[y] > obj){
							curObj[y]--;
						}
					}
				}
				curObjLength = 0;
				curObj.Clear();
				obj = 0;
				displayMenu = false;
			}
		}
	}
	
	void SendToBack () {
		int y = 0;
		int startDepth = objElement.depth;
		for(int x = 0; x < target.gui.list.Length; x++){
			if(x == obj){
				target.gui.list[x].depth = target.gui.list.Length-1;
			}
			else if(target.gui.list[x].depth < startDepth){
				target.gui.list[x].depth = target.gui.list[x].depth;
			}
			else if(target.gui.list[x].depth > startDepth){
				target.gui.list[x].depth = target.gui.list[x].depth-1;
			}
		}
	}	
	
	void SendBackward () {
		for(int y = 0; y < curObjLength; y++){
			if(target.gui.list[curObj[y]].depth < target.gui.list.Length-1){
				target.gui.list[curObj[y]].depth = target.gui.list[curObj[y]].depth + 1;
				for(int x = 0; x < target.gui.list.Length; x++){
					if(target.gui.list[x].depth == target.gui.list[curObj[y]].depth && x != curObj[y]){
						target.gui.list[x].depth = target.gui.list[x].depth -1;
					}
				}
			}
		}
	}
	
	void BringToFront () {
		int startDepth = objElement.depth;
		for(int x = 0; x < target.gui.list.Length; x++){
			if(x == obj){
				target.gui.list[x].depth = 0;
			}
			else if(target.gui.list[x].depth < startDepth){
				target.gui.list[x].depth = target.gui.list[x].depth + 1;
			}
			else if(target.gui.list[x].depth > startDepth){
				target.gui.list[x].depth = target.gui.list[x].depth;
			}
		}
	}
	
	void BringForward () {
		for(int y = 0; y < curObjLength; y++){
			if(target.gui.list[curObj[y]].depth > 0){
				target.gui.list[curObj[y]].depth = target.gui.list[curObj[y]].depth - 1;
				for(int x = 0; x < target.gui.list.Length; x++){
					if(target.gui.list[x].depth == target.gui.list[curObj[y]].depth && x != curObj[y]){
						target.gui.list[x].depth = target.gui.list[x].depth + 1;
					}
				}
			}
		}
	}
	
	void EditStyleWind (int id){
		
		if(GUI.Button(new Rect(menuLoc.width-15, 0, 15, 15), "")){
			displayMenu = false;
		}
		GUI.DrawTexture(new Rect(menuLoc.width-15, 0, 15, 15), text.closeTexture);
		GUI.skin = text.emptySkin;
		if(GUI.Button(new Rect(menuLoc.width-10, 0, 15,15), "")){
			displayMenu = false;
		}
		GUI.skin = skin;
		GUILayout.BeginHorizontal();
		if(windowState.Length < 5){
			windowState = new int[5];
		}
		if(GUILayout.Button("States")){
			windowState = new int[5];
			windowState[0] = 0;
		}
		if(GUILayout.Button("Alignment")){
			windowState = new int[5];
			windowState[0] = 1;
			windowBool = new bool[5];
		}
		if(GUILayout.Button("Content")){
			windowState = new int[5];
			windowState[0] = 2;
			windowBool = new bool[5];
		}
		GUILayout.EndHorizontal();
		if(windowState[0] == 0){
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Normal")){
				windowState[1] = 0;
			}
			if(GUILayout.Button("Hover")){
				windowState[1] = 1;
			}
			if(GUILayout.Button("Active")){
				windowState[1] = 2;
			}
			if(GUILayout.Button("Focused")){
				windowState[1] = 3;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("On Normal")){
				windowState[1] = 4;
			}
			if(GUILayout.Button("On Hover")){
				windowState[1] = 5;
			}
			if(GUILayout.Button("On Active")){
				windowState[1] = 6;
			}
			if(GUILayout.Button("On Focused")){
				windowState[1] = 7;
			}
			GUILayout.EndHorizontal();
			if(windowState[1] == 0){
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].useStyle = EditorGUILayout.Toggle("Use Style : ", target.gui.list[curObj[0]].useStyle);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].style.normal.background = EditorGUILayout.ObjectField("Texture : ", target.gui.list[curObj[0]].style.normal.background, typeof(Texture2D)) as Texture2D;
				target.gui.list[curObj[0]].style.normal.textColor = EditorGUILayout.ColorField("Color : ", target.gui.list[curObj[0]].style.normal.textColor);
				GUILayout.EndHorizontal();
			}
			else if(windowState[1] == 1){
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].useStyle = EditorGUILayout.Toggle("Use Style : ", target.gui.list[curObj[0]].useStyle);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].style.hover.background = EditorGUILayout.ObjectField("Texture : ", target.gui.list[curObj[0]].style.hover.background, typeof(Texture2D)) as Texture2D;
				target.gui.list[curObj[0]].style.hover.textColor = EditorGUILayout.ColorField("Color : ", target.gui.list[curObj[0]].style.hover.textColor);
				GUILayout.EndHorizontal();
			}
			else if(windowState[1] == 2){
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].useStyle = EditorGUILayout.Toggle("Use Style : ", target.gui.list[curObj[0]].useStyle);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].style.active.background = EditorGUILayout.ObjectField("Texture : ", target.gui.list[curObj[0]].style.active.background, typeof(Texture2D)) as Texture2D;
				target.gui.list[curObj[0]].style.active.textColor = EditorGUILayout.ColorField("Color : ", target.gui.list[curObj[0]].style.active.textColor);
				GUILayout.EndHorizontal();
			}
			else if(windowState[1] == 3){
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].useStyle = EditorGUILayout.Toggle("Use Style : ", target.gui.list[curObj[0]].useStyle);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].style.focused.background = EditorGUILayout.ObjectField("Texture : ", target.gui.list[curObj[0]].style.focused.background, typeof(Texture2D)) as Texture2D;
				target.gui.list[curObj[0]].style.focused.textColor = EditorGUILayout.ColorField("Color : ", target.gui.list[curObj[0]].style.focused.textColor);
				GUILayout.EndHorizontal();
			}
			else if(windowState[1] == 4){
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].useStyle = EditorGUILayout.Toggle("Use Style : ", target.gui.list[curObj[0]].useStyle);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].style.onNormal.background = EditorGUILayout.ObjectField("Texture : ", target.gui.list[curObj[0]].style.onNormal.background, typeof(Texture2D)) as Texture2D;
				target.gui.list[curObj[0]].style.onNormal.textColor = EditorGUILayout.ColorField("Color : ", target.gui.list[curObj[0]].style.onNormal.textColor);
				GUILayout.EndHorizontal();
			}
			else if(windowState[1] == 5){
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].useStyle = EditorGUILayout.Toggle("Use Style : ", target.gui.list[curObj[0]].useStyle);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].style.onHover.background = EditorGUILayout.ObjectField("Texture : ", target.gui.list[curObj[0]].style.onHover.background, typeof(Texture2D)) as Texture2D;
				target.gui.list[curObj[0]].style.onHover.textColor = EditorGUILayout.ColorField("Color : ", target.gui.list[curObj[0]].style.onHover.textColor);
				GUILayout.EndHorizontal();
			}
			else if(windowState[1] == 6){
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].useStyle = EditorGUILayout.Toggle("Use Style : ", target.gui.list[curObj[0]].useStyle);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].style.onActive.background = EditorGUILayout.ObjectField("Texture : ", target.gui.list[curObj[0]].style.onActive.background, typeof(Texture2D)) as Texture2D;
				target.gui.list[curObj[0]].style.onActive.textColor = EditorGUILayout.ColorField("Color : ", target.gui.list[curObj[0]].style.onActive.textColor);
				GUILayout.EndHorizontal();
			}
			else if(windowState[1] == 7){
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].useStyle = EditorGUILayout.Toggle("Use Style : ", target.gui.list[curObj[0]].useStyle);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				target.gui.list[curObj[0]].style.onFocused.background = EditorGUILayout.ObjectField("Texture : ", target.gui.list[curObj[0]].style.onFocused.background, typeof(Texture2D)) as Texture2D;
				target.gui.list[curObj[0]].style.onFocused.textColor = EditorGUILayout.ColorField("Color : ", target.gui.list[curObj[0]].style.onFocused.textColor);
				GUILayout.EndHorizontal();
			}
		}
		else if(windowState[0] == 1){
			GUILayout.BeginHorizontal();
			int curLoc = 0;
			int loc = 0;
			string[] prop = {"Upper Left", "Upper Center", "Upper Right", "Middle Left", "Middle Center", "Middle Right", "Lower Left", "Lower Center", "Lower Right"};
			if(target.gui.list[curObj[0]].style.alignment == TextAnchor.UpperLeft)
				curLoc = 0;
			else if(target.gui.list[curObj[0]].style.alignment == TextAnchor.UpperCenter)
				curLoc = 1;
			else if(target.gui.list[curObj[0]].style.alignment == TextAnchor.UpperRight)
				curLoc = 2;
			else if(target.gui.list[curObj[0]].style.alignment == TextAnchor.MiddleLeft)
				curLoc = 3;
			else if(target.gui.list[curObj[0]].style.alignment == TextAnchor.MiddleCenter)
				curLoc = 4;
			else if(target.gui.list[curObj[0]].style.alignment == TextAnchor.MiddleRight)
				curLoc = 5;
			else if(target.gui.list[curObj[0]].style.alignment == TextAnchor.LowerLeft)
				curLoc = 6;
			else if(target.gui.list[curObj[0]].style.alignment == TextAnchor.LowerCenter)
				curLoc = 7;
			else if(target.gui.list[curObj[0]].style.alignment == TextAnchor.LowerRight)
				curLoc = 8;
			loc = curLoc;
			curLoc = EditorGUILayout.Popup("Alignment : ", curLoc, prop);
			if(curLoc != loc){
				mouseDown = false;
			}
			if(curLoc == 0)
				target.gui.list[curObj[0]].style.alignment = TextAnchor.UpperLeft;
			else if(curLoc == 1)
				target.gui.list[curObj[0]].style.alignment = TextAnchor.UpperCenter;
			else if(curLoc == 2)
				target.gui.list[curObj[0]].style.alignment = TextAnchor.UpperRight;
			else if(curLoc == 3)
				target.gui.list[curObj[0]].style.alignment = TextAnchor.MiddleLeft;
			else if(curLoc == 4)
				target.gui.list[curObj[0]].style.alignment = TextAnchor.MiddleCenter;
			else if(curLoc == 5)
				target.gui.list[curObj[0]].style.alignment = TextAnchor.MiddleRight;
			else if(curLoc == 6)
				target.gui.list[curObj[0]].style.alignment = TextAnchor.LowerLeft;
			else if(curLoc == 7)
				target.gui.list[curObj[0]].style.alignment = TextAnchor.LowerCenter;
			else if(curLoc == 8)
				target.gui.list[curObj[0]].style.alignment = TextAnchor.LowerRight;
			GUILayout.EndHorizontal();
			//Border
			GUILayout.Label("Border : ");
			GUILayout.BeginHorizontal();
			int rectPoint = target.gui.list[curObj[0]].style.border.left;
			rectPoint = EditorGUILayout.IntField("Left", rectPoint);
			target.gui.list[curObj[0]].style.border = new RectOffset(rectPoint, target.gui.list[curObj[0]].style.border.right, target.gui.list[curObj[0]].style.border.top, target.gui.list[curObj[0]].style.border.bottom);
			rectPoint = target.gui.list[curObj[0]].style.border.right;
			rectPoint = EditorGUILayout.IntField("Right", rectPoint);
			target.gui.list[curObj[0]].style.border = new RectOffset(target.gui.list[curObj[0]].style.border.left, rectPoint, target.gui.list[curObj[0]].style.border.top, target.gui.list[curObj[0]].style.border.bottom);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			rectPoint = target.gui.list[curObj[0]].style.border.top;
			rectPoint = EditorGUILayout.IntField("Top", rectPoint);
			target.gui.list[curObj[0]].style.border = new RectOffset(target.gui.list[curObj[0]].style.border.left, target.gui.list[curObj[0]].style.border.right, rectPoint, target.gui.list[curObj[0]].style.border.bottom);
			rectPoint = target.gui.list[curObj[0]].style.border.bottom;
			rectPoint = EditorGUILayout.IntField("Bottom", rectPoint);
			target.gui.list[curObj[0]].style.border = new RectOffset(target.gui.list[curObj[0]].style.border.left, target.gui.list[curObj[0]].style.border.right, target.gui.list[curObj[0]].style.border.top, rectPoint);
			GUILayout.EndHorizontal();
			//Margin
			GUILayout.Label("Margin : ");
			GUILayout.BeginHorizontal();
			rectPoint = target.gui.list[curObj[0]].style.margin.left;
			rectPoint = EditorGUILayout.IntField("Left", rectPoint);
			target.gui.list[curObj[0]].style.margin = new RectOffset(rectPoint, target.gui.list[curObj[0]].style.margin.right, target.gui.list[curObj[0]].style.margin.top, target.gui.list[curObj[0]].style.margin.bottom);
			rectPoint = target.gui.list[curObj[0]].style.margin.right;
			rectPoint = EditorGUILayout.IntField("Right", rectPoint);
			target.gui.list[curObj[0]].style.margin = new RectOffset(target.gui.list[curObj[0]].style.margin.left, rectPoint, target.gui.list[curObj[0]].style.margin.top, target.gui.list[curObj[0]].style.margin.bottom);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			rectPoint = target.gui.list[curObj[0]].style.margin.top;
			rectPoint = EditorGUILayout.IntField("Top", rectPoint);
			target.gui.list[curObj[0]].style.margin = new RectOffset(target.gui.list[curObj[0]].style.margin.left, target.gui.list[curObj[0]].style.margin.right, rectPoint, target.gui.list[curObj[0]].style.margin.bottom);
			rectPoint = target.gui.list[curObj[0]].style.margin.bottom;
			rectPoint = EditorGUILayout.IntField("Bottom", rectPoint);
			target.gui.list[curObj[0]].style.margin = new RectOffset(target.gui.list[curObj[0]].style.margin.left, target.gui.list[curObj[0]].style.margin.right, target.gui.list[curObj[0]].style.margin.top, rectPoint);
			GUILayout.EndHorizontal();
			//Padding
			GUILayout.Label("Padding : ");
			GUILayout.BeginHorizontal();
			rectPoint = target.gui.list[curObj[0]].style.padding.left;
			rectPoint = EditorGUILayout.IntField("Left", rectPoint);
			target.gui.list[curObj[0]].style.padding = new RectOffset(rectPoint, target.gui.list[curObj[0]].style.padding.right, target.gui.list[curObj[0]].style.padding.top, target.gui.list[curObj[0]].style.padding.bottom);
			rectPoint = target.gui.list[curObj[0]].style.padding.right;
			rectPoint = EditorGUILayout.IntField("Right", rectPoint);
			target.gui.list[curObj[0]].style.padding = new RectOffset(target.gui.list[curObj[0]].style.padding.left, rectPoint, target.gui.list[curObj[0]].style.padding.top, target.gui.list[curObj[0]].style.padding.bottom);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			rectPoint = target.gui.list[curObj[0]].style.padding.top;
			rectPoint = EditorGUILayout.IntField("Top", rectPoint);
			target.gui.list[curObj[0]].style.padding = new RectOffset(target.gui.list[curObj[0]].style.padding.left, target.gui.list[curObj[0]].style.padding.right, rectPoint, target.gui.list[curObj[0]].style.padding.bottom);
			rectPoint = target.gui.list[curObj[0]].style.padding.bottom;
			rectPoint = EditorGUILayout.IntField("Bottom", rectPoint);
			target.gui.list[curObj[0]].style.padding = new RectOffset(target.gui.list[curObj[0]].style.padding.left, target.gui.list[curObj[0]].style.padding.right, target.gui.list[curObj[0]].style.padding.top, rectPoint);
			GUILayout.EndHorizontal();
			//Overflow
			GUILayout.Label("Overflow : ");
			GUILayout.BeginHorizontal();
			rectPoint = target.gui.list[curObj[0]].style.overflow.left;
			rectPoint = EditorGUILayout.IntField("Left", rectPoint);
			target.gui.list[curObj[0]].style.overflow = new RectOffset(rectPoint, target.gui.list[curObj[0]].style.overflow.right, target.gui.list[curObj[0]].style.overflow.top, target.gui.list[curObj[0]].style.overflow.bottom);
			rectPoint = target.gui.list[curObj[0]].style.overflow.right;
			rectPoint = EditorGUILayout.IntField("Right", rectPoint);
			target.gui.list[curObj[0]].style.overflow = new RectOffset(target.gui.list[curObj[0]].style.overflow.left, rectPoint, target.gui.list[curObj[0]].style.overflow.top, target.gui.list[curObj[0]].style.overflow.bottom);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			rectPoint = target.gui.list[curObj[0]].style.overflow.top;
			rectPoint = EditorGUILayout.IntField("Top", rectPoint);
			target.gui.list[curObj[0]].style.overflow = new RectOffset(target.gui.list[curObj[0]].style.overflow.left, target.gui.list[curObj[0]].style.overflow.right, rectPoint, target.gui.list[curObj[0]].style.overflow.bottom);
			rectPoint = target.gui.list[curObj[0]].style.overflow.bottom;
			rectPoint = EditorGUILayout.IntField("Bottom", rectPoint);
			target.gui.list[curObj[0]].style.overflow = new RectOffset(target.gui.list[curObj[0]].style.overflow.left, target.gui.list[curObj[0]].style.overflow.right, target.gui.list[curObj[0]].style.overflow.top, rectPoint);
			GUILayout.EndHorizontal();
		}
		else if(windowState[0] == 2){
			GUI.DrawTexture(new Rect(240, 45, 10, 450), text.verticalSpaceTexture);
			GUI.DrawTexture(new Rect(0, 40, 500, 10), text.spaceTexture);
			Rect guiRect = new Rect(10, 50, 230, 20);
			target.gui.list[curObj[0]].style.font = EditorGUI.ObjectField(guiRect, "Font : ", target.gui.list[curObj[0]].style.font, typeof(Font)) as Font;
			guiRect = new Rect(250, 50, 230, 20);
			target.gui.list[curObj[0]].style.fontSize = EditorGUI.IntField(guiRect, "Font Size : ", target.gui.list[curObj[0]].style.fontSize);
			int curLoc = 0;
			int loc = 0;
			string[] prop = {"Normal", "Bold", "Italic", "Bold and Italic"};
			if(target.gui.list[curObj[0]].style.fontStyle == FontStyle.Normal)
				curLoc = 0;
			else if(target.gui.list[curObj[0]].style.fontStyle == FontStyle.Bold)
				curLoc = 1;
			else if(target.gui.list[curObj[0]].style.fontStyle == FontStyle.Italic)
				curLoc = 2;
			else if(target.gui.list[curObj[0]].style.fontStyle == FontStyle.BoldAndItalic)
				curLoc = 3;
			guiRect = new Rect(10, 75, 230, 20);
			curLoc = EditorGUI.Popup(guiRect, "Font Style : ", curLoc, prop);
			if(curLoc != loc){
				mouseDown = false;
			}
			if(curLoc == 0)
				target.gui.list[curObj[0]].style.fontStyle = FontStyle.Normal;
			else if(curLoc == 1)
				target.gui.list[curObj[0]].style.fontStyle = FontStyle.Bold;
			else if(curLoc == 2)
				target.gui.list[curObj[0]].style.fontStyle = FontStyle.Italic;
			else if(curLoc == 3)
				target.gui.list[curObj[0]].style.fontStyle = FontStyle.BoldAndItalic;
			guiRect = new Rect(250, 75, 230, 20);
			target.gui.list[curObj[0]].style.wordWrap = EditorGUI.Toggle(guiRect, "Word Wrap : ", target.gui.list[curObj[0]].style.wordWrap);
			guiRect = new Rect(10, 100, 230, 20);
			target.gui.list[curObj[0]].style.richText = EditorGUI.Toggle(guiRect, "Rich Text : ", target.gui.list[curObj[0]].style.richText);
			guiRect = new Rect(250, 100, 230, 20);
			if(!target.gui.list[curObj[0]].style.wordWrap){
				bool textClipping;
				if(target.gui.list[curObj[0]].style.clipping == TextClipping.Overflow){
					textClipping = true;
				}
				else{
					textClipping = false;
				}
				textClipping = EditorGUI.Toggle(guiRect, "Overflow : ", textClipping);
				if(textClipping){
					target.gui.list[curObj[0]].style.clipping = TextClipping.Overflow;
				}
				else{
					target.gui.list[curObj[0]].style.clipping = TextClipping.Clip;
				}
			}
			guiRect = new Rect(10, 125, 230, 20);
			curLoc = 0;
			loc = 0;
			string[] ns = {"Image Left", "Image Above", "Image Only", "Text Only"};
			if(target.gui.list[curObj[0]].style.imagePosition == ImagePosition.ImageLeft)
				curLoc = 0;
			else if(target.gui.list[curObj[0]].style.imagePosition == ImagePosition.ImageAbove)
				curLoc = 1;
			else if(target.gui.list[curObj[0]].style.imagePosition == ImagePosition.ImageOnly)
				curLoc = 2;
			else if(target.gui.list[curObj[0]].style.imagePosition == ImagePosition.TextOnly)
				curLoc = 3;
			curLoc = EditorGUI.Popup(guiRect, "Image Position : ", curLoc, ns);
			if(curLoc != loc){
				mouseDown = false;
			}
			if(curLoc == 0)
				target.gui.list[curObj[0]].style.imagePosition = ImagePosition.ImageLeft;
			else if(curLoc == 1)
				target.gui.list[curObj[0]].style.imagePosition = ImagePosition.ImageAbove;
			else if(curLoc == 2)
				target.gui.list[curObj[0]].style.imagePosition = ImagePosition.ImageOnly;
			else if(curLoc == 3)
				target.gui.list[curObj[0]].style.imagePosition = ImagePosition.TextOnly;
			guiRect = new Rect(10, 150, 230, 20);
			target.gui.list[curObj[0]].style.fixedWidth = EditorGUI.FloatField(guiRect, "Fixed Width : ", target.gui.list[curObj[0]].style.fixedWidth);
			
			guiRect = new Rect(250, 150, 230, 20);
			target.gui.list[curObj[0]].style.fixedHeight = EditorGUI.FloatField(guiRect, "Fixed Height : ", target.gui.list[curObj[0]].style.fixedHeight);
			
			guiRect = new Rect(10, 175, 230, 20);
			target.gui.list[curObj[0]].style.stretchWidth = EditorGUI.Toggle(guiRect, "Stretch Width : ", target.gui.list[curObj[0]].style.stretchWidth);
			guiRect = new Rect(250, 175, 230, 20);
			target.gui.list[curObj[0]].style.stretchHeight = EditorGUI.Toggle(guiRect, "Stretch Height : ", target.gui.list[curObj[0]].style.stretchHeight);
			guiRect = new Rect(10, 200, 230, 20);
			target.gui.list[curObj[0]].style.contentOffset = EditorGUI.Vector2Field(guiRect, "Content Offset : ", target.gui.list[curObj[0]].style.contentOffset);
			
		}
		GUI.DragWindow ();
	}
	
	void DrawSetup () {
		if(position.height < target.gui.resolution.y + 115){
			position = new Rect(position.x, position.y, position.width, target.gui.resolution.y+115);
		}
		if(position.width < target.gui.resolution.x){
			position = new Rect(position.x, position.y, target.gui.resolution.x, position.height);
		}
		Rect tempLoc = new Rect((position.width - target.gui.resolution.x)/2-5, (position.height - target.gui.resolution.y)/2+50, target.gui.resolution.x+10, target.gui.resolution.y+10);
		GUI.DrawTextureWithTexCoords (new Rect(0, 0, position.width, position.height), text.backgroundTexture, new Rect (0, 0, (int) position.width/100, (int) position.height/100));
		GUI.DrawTexture(tempLoc, text.tempBackgroundTexture, ScaleMode.StretchToFill);
		tempLoc = new Rect((position.width - target.gui.resolution.x)/2, (position.height - target.gui.resolution.y)/2+55, target.gui.resolution.x, target.gui.resolution.y);
		GUI.DrawTexture(tempLoc, text.tempTexture, ScaleMode.StretchToFill);
		tempLocGlob = new Vector2(tempLoc.x, tempLoc.y);
		tempLoc = new Rect(tempLoc.x+tempLoc.width/2-45, tempLoc.y-60, 140, 50);
		GUI.Box(tempLoc, "");
		tempLoc = new Rect(tempLoc.x+5, tempLoc.y+5, 40, 40);
		if(GUI.Button(tempLoc, text.playTexture)){
			playAnimation = "Play";
			text.editorState = "Editor";
		}
		tempLoc = new Rect(tempLoc.x+45, tempLoc.y, 40, 40);
		if(GUI.Button(tempLoc, text.pauseTexture)){
			playAnimation = "Pause";
			text.editorState = "Editor";
		}
		tempLoc = new Rect(tempLoc.x+45, tempLoc.y, 40, 40);
		if(GUI.Button(tempLoc, text.stopTexture)){
			playAnimation = "Stop";
			text.editorState = "Editor";
		}
		GUI.Box(new Rect(0,0, Screen.width, 50), "");
		GUI.Box(new Rect(0,25,Screen.width, 25), text.editorState);
	}
	
	void DrawCorners (Event e, bool menu, int x) {
		if(text.editorState == "Editor"){
			curElement = target.gui.list[curObj[x]];
		}
		else if(text.editorState == "Animator"){
			curElement = target.gui.list[curObj[x]].animation[text.curAnimation].frame[text.curFrame].element;
		}
		if(curElement.selected){
			Rect loc = curElement.loc;
			if(curElement.linkedElement != null){
				if(curElement.linkedElement.state == 8){
					loc = new Rect(loc.x-curElement.linkedElement.scrollView.scrollPos.x+curElement.linkedElement.loc.x, loc.y-curElement.linkedElement.scrollView.scrollPos.y+curElement.linkedElement.loc.y, loc.width, loc.height);
				}
			}
			if(text.editorState == "Animator"){
				loc = new Rect(loc.x + tempLocGlob.x + target.gui.list[curObj[x]].loc.x, loc.y + tempLocGlob.y + target.gui.list[curObj[x]].loc.y, loc.width+target.gui.list[curObj[x]].loc.width, loc.height+target.gui.list[curObj[x]].loc.height);
			}
			else{
				loc = new Rect(loc.x + tempLocGlob.x, loc.y + tempLocGlob.y, loc.width, loc.height);
			}
			if(obj != curObj[x]){
				GUI.DrawTexture(new Rect(loc.x-8, loc.y-8, 16, 16), text.scaleTexture[0]);
				GUI.DrawTexture(new Rect(loc.x+loc.width-8, loc.y-8, 16, 16), text.scaleTexture[2]);
				GUI.DrawTexture(new Rect(loc.x-8, loc.y+loc.height-8, 16, 16), text.scaleTexture[6]);
				GUI.DrawTexture(new Rect(loc.x+loc.width-8, loc.y+loc.height-8, 16, 16), text.scaleTexture[4]);
				if(loc.width > 20){
					GUI.DrawTexture(new Rect(loc.x+loc.width/2-8, loc.y-8, 16, 16), text.scaleTexture[1]);
					GUI.DrawTexture(new Rect(loc.x+loc.width/2-8, loc.y+loc.height-8, 16, 16), text.scaleTexture[5]);
				}
				if(loc.height > 20){
					GUI.DrawTexture(new Rect(loc.x-8, loc.y+loc.height/2-8, 16, 16), text.scaleTexture[7]);
					GUI.DrawTexture(new Rect(loc.x+loc.width-8, loc.y+loc.height/2-8, 16, 16), text.scaleTexture[3]);
				}
			}
			else{
				GUI.DrawTexture(new Rect(loc.x-8, loc.y-8, 16, 16), text.scaleTextureGreen[0]);
				GUI.DrawTexture(new Rect(loc.x+loc.width-8, loc.y-8, 16, 16), text.scaleTextureGreen[2]);
				GUI.DrawTexture(new Rect(loc.x-8, loc.y+loc.height-8, 16, 16), text.scaleTextureGreen[6]);
				GUI.DrawTexture(new Rect(loc.x+loc.width-8, loc.y+loc.height-8, 16, 16), text.scaleTextureGreen[4]);
				if(loc.width > 20){
					GUI.DrawTexture(new Rect(loc.x+loc.width/2-8, loc.y-8, 16, 16), text.scaleTextureGreen[1]);
					GUI.DrawTexture(new Rect(loc.x+loc.width/2-8, loc.y+loc.height-8, 16, 16), text.scaleTextureGreen[5]);
				}
				if(loc.height > 20){
					GUI.DrawTexture(new Rect(loc.x-8, loc.y+loc.height/2-8, 16, 16), text.scaleTextureGreen[7]);
					GUI.DrawTexture(new Rect(loc.x+loc.width-8, loc.y+loc.height/2-8, 16, 16), text.scaleTextureGreen[3]);
				}
			}
			if(mouseDown){
				if(e.mousePosition.x >= loc.x-8 && e.mousePosition.x <= loc.x+8 && e.mousePosition.y >= loc.y-8 && e.mousePosition.y <= loc.y+8){
					scaleMode = true;
					scaling = new Vector2(-1, -1);
				}
				else if(e.mousePosition.x >= loc.x-8 && e.mousePosition.x <= loc.x+8 && e.mousePosition.y >= loc.y+loc.height-8 && e.mousePosition.y <= loc.y+loc.height+8){
					scaleMode = true;
					scaling = new Vector2(-1, 1);
				}
				else if(e.mousePosition.x >= loc.x+loc.width-8 && e.mousePosition.x <= loc.x+loc.width+8 && e.mousePosition.y >= loc.y-8 && e.mousePosition.y <= loc.y+8){
					scaleMode = true;
					scaling = new Vector2(1, -1);
				}
				else if(e.mousePosition.x >= loc.x+loc.width-8 && e.mousePosition.x <= loc.x+loc.width+8 && e.mousePosition.y >= loc.y+loc.height-8 && e.mousePosition.y <= loc.y+loc.height+8){
					scaleMode = true;
					scaling = new Vector2(1, 1);
				}
				if(curElement.loc.width > 20){
					if(e.mousePosition.x >= loc.x+loc.width/2-8 && e.mousePosition.x <= loc.x+loc.width/2+8 && e.mousePosition.y >= loc.y-8 && e.mousePosition.y <= loc.y+8){
						scaleMode = true;
						scaling = new Vector2(0, -1);
					}
					else if(e.mousePosition.x >= loc.x+loc.width/2-8 && e.mousePosition.x <= loc.x+loc.width/2+8 && e.mousePosition.y >= loc.y+loc.height-8 && e.mousePosition.y <= loc.y+loc.height+8){
						scaleMode = true;
						scaling = new Vector2(0, 1);
					}
				}				
				if(curElement.loc.height > 20){
					if(e.mousePosition.x >= loc.x-8 && e.mousePosition.x <= loc.x+8 && e.mousePosition.y >= loc.y+loc.height/2-8 && e.mousePosition.y <= loc.y+loc.height/2+8){
						scaleMode = true;
						scaling = new Vector2(-1, 0);
					}
					else if(e.mousePosition.x >= loc.x+loc.width-8 && e.mousePosition.x <= loc.x+loc.width+8 && e.mousePosition.y >= loc.y+loc.height/2-8 && e.mousePosition.y <= loc.y+loc.height/2+8){
						scaleMode = true;
						scaling = new Vector2(1,0);
					}
				}
				if(e.type == EventType.MouseDown && e.button == 0){
					if(scaleMode && !dragSelect && !menu){
						originalSize = loc;
						openCanvas = false;
						moveObj = false;
					}
				}
			}
			if(scaleMode && !menu && !dragSelect && !moveObj){
				bool snapToGrid = false;
				if(e.shift)
					snapToGrid = true;
				Vector2 newLoc;
				Vector2 newLocM;
				loc = curElement.loc;
				if(text.editorState == "Animator"){
					loc = new Rect(loc.x, loc.y, loc.width+target.gui.list[curObj[x]].loc.width, loc.height+target.gui.list[curObj[x]].loc.height);
				}
				loc = new Rect(loc.x, loc.y, loc.width, loc.height);
				if(!snapToGrid){
					newLoc = new Vector2(e.mousePosition.x - tempLocGlob.x, e.mousePosition.y - tempLocGlob.y);
					newLocM = new Vector2(e.mousePosition.x - loc.x - tempLocGlob.x, e.mousePosition.y - loc.y - tempLocGlob.y);
					if(curElement.linkedElement != null){
						if(curElement.linkedElement.state == 8){
							newLoc = new Vector2(e.mousePosition.x + curElement.linkedElement.scrollView.scrollPos.x - curElement.linkedElement.loc.x - tempLocGlob.x, e.mousePosition.y + curElement.linkedElement.scrollView.scrollPos.y  - curElement.linkedElement.loc.y - tempLocGlob.y);
							newLocM = new Vector2(e.mousePosition.x - curElement.loc.x + curElement.linkedElement.scrollView.scrollPos.x - curElement.linkedElement.loc.x - tempLocGlob.x, e.mousePosition.y - curElement.loc.y + curElement.linkedElement.scrollView.scrollPos.y  - curElement.linkedElement.loc.y - tempLocGlob.y);
						}
					}
				}
				else{
					newLoc = new Vector2((int)(((e.mousePosition.x - tempLocGlob.x))/text.nodeSize), (int)((e.mousePosition.y - tempLocGlob.y)/text.nodeSize)); 
					newLocM = new Vector2((int)((e.mousePosition.x - tempLocGlob.x)/text.nodeSize), (int)((e.mousePosition.y - tempLocGlob.y)/text.nodeSize)); 
					if(curElement.linkedElement != null){
						if(curElement.linkedElement.state == 8){
							newLoc = new Vector2((int)(((e.mousePosition.x - tempLocGlob.x + curElement.linkedElement.scrollView.scrollPos.x - curElement.linkedElement.loc.x))/text.nodeSize), (int)((e.mousePosition.y - tempLocGlob.y + curElement.linkedElement.scrollView.scrollPos.y - curElement.linkedElement.loc.y)/text.nodeSize)); 
							newLocM = new Vector2((int)((e.mousePosition.x - tempLocGlob.x + curElement.linkedElement.scrollView.scrollPos.x - curElement.linkedElement.loc.x)/text.nodeSize), (int)((e.mousePosition.y - tempLocGlob.y + curElement.linkedElement.scrollView.scrollPos.y - curElement.linkedElement.loc.y)/text.nodeSize)); 

						}
					}
					newLoc = newLoc*text.nodeSize;
					newLocM = newLocM*text.nodeSize;
					newLocM = new Vector2(newLocM.x - loc.x, newLocM.y - loc.y);
				}
				if(scaling.x > 0){
					loc = new Rect(loc.x, loc.y, newLocM.x, loc.height);
				}
				else if(scaling.x < 0){
					loc = new Rect(newLoc.x, loc.y, loc.width+(loc.x-newLoc.x), loc.height);
				}
				if(scaling.y > 0){
					loc = new Rect(loc.x, loc.y, loc.width, newLocM.y);
				}
				else if(scaling.y < 0){
					loc = new Rect(loc.x, newLoc.y, loc.width, loc.height+(loc.y-newLoc.y));
				}
				loc = new Rect(loc.x, loc.y, loc.width, loc.height);
				curElement.loc = loc;
				if(text.editorState == "Animator"){
					curElement.loc = new Rect(curElement.loc.x, curElement.loc.y, curElement.loc.width-target.gui.list[curObj[x]].loc.width, curElement.loc.height-target.gui.list[curObj[x]].loc.height);
				}
			}
			if(menu || dragSelect || moveObj){
				scaleMode = false;
			}
		}
	}
	
	void Grid () {
		Color normColor = GUI.color;
		GUI.color = text.gridColor;
		if(text.nodeSize > 0){
			int size = (int) target.gui.resolution.y/text.nodeSize;
			for(int y = 0; y < size+1; y++){
				GUI.DrawTexture(new Rect(tempLocGlob.x, y*(text.nodeSize)+tempLocGlob.y, target.gui.resolution.x, 1), text.gridTexture, ScaleMode.StretchToFill);
			}
			size = (int) target.gui.resolution.x/text.nodeSize;
			for(int x = 0; x < size+1; x++){
				GUI.DrawTexture(new Rect(x*(text.nodeSize)+tempLocGlob.x, tempLocGlob.y, 1, target.gui.resolution.y), text.gridTexture, ScaleMode.StretchToFill);
			}
		}
		GUI.color = text.importantColors;
		GUI.DrawTexture(new Rect(tempLocGlob.x+target.gui.resolution.x/2-1, tempLocGlob.y, 2, target.gui.resolution.y), text.gridTexture, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(tempLocGlob.x, tempLocGlob.y+target.gui.resolution.y/2-1, target.gui.resolution.x, 2), text.gridTexture, ScaleMode.StretchToFill);
		GUI.color = normColor;
	}

	void CopyGUI (int newLength, int oldLength) {
		myList = new CGUIElements[oldLength];
		Event e = Event.current;
		if(newLength < oldLength){
			SendToBack();
			for(int x = 0; x < target.gui.list.Length; x++){
				myList[x] = target.gui.list[x];
			}
			target.gui.list = new CGUIElements[newLength];
			int y = 0;
			for(int x = 0; x < myList.Length; x++){
				if(x != obj){
					target.gui.list[y] = myList[x];
					if(target.gui.list[y].linkedElement == myList[obj]){
						target.gui.list[y].linkedElement = null;
					}
					y++;
				}
			}
		}
		else{
			for(int x = 0; x < target.gui.list.Length; x++){
				myList[x] = target.gui.list[x];
			}
			target.gui.list = new CGUIElements[newLength];
			for(int x = 0; x < target.gui.list.Length; x++){
				Debug.Log("Add");
				if(x < myList.Length){
					target.gui.list[x] = myList[x];
				}
				else{
					target.gui.list[x] = new CGUIElements();
					target.gui.list[x].loc = new Rect(e.mousePosition.x-tempLocGlob.x, e.mousePosition.y-tempLocGlob.y, 100, 100);
					target.gui.list[x].state = 2;
					target.gui.list[x].depth = target.gui.list.Length-1;
					curObjLength = 1;
					curObj.Clear();
					curObj.Add(x);
					obj = x;
					BringToFront();
				}
			}
		} 
	}
	
	void CopyGUIPaste (int newLength, int oldLength) {
		myList = new CGUIElements[oldLength];
		Event e = Event.current;
		for(int x = 0; x < target.gui.list.Length; x++){
			myList[x] = target.gui.list[x];
		}
		newLength = newLength + extraElements.Length;
		int y = 0;
		target.gui.list = new CGUIElements[newLength];
		for(int x = 0; x < target.gui.list.Length; x++){
			if(x < myList.Length){
				target.gui.list[x] = myList[x];
			}
			else{
				if(x == myList.Length){
					target.gui.list[x] = new CGUIElements(myElement);
					target.gui.list[x].depth = x-1;
					curObjLength = 1;
					curObj.Clear();
					curObj.Add(x);
					obj = x;
				}
				else{
					target.gui.list[x] = new CGUIElements(extraElements[y]);
					target.gui.list[x].linkedElement = target.gui.list[myList.Length];
					target.gui.list[x].depth = x-1;
					curObjLength = 1;
					curObj.Clear();
					curObj.Add(x);
					obj = x;
					y++;
				}
			}
		} 
		obj = myList.Length;
		curObjLength = 1;
		curObj.Clear();
		curObj.Add(myList.Length);
		BringToFront();
	}
}
