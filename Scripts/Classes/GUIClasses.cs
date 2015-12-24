using UnityEngine;
using System.Collections;

[System.Serializable]
public class CGUIManager {
	public Vector2 resolution;
	public CGUIElements[] list;
}

public class CGUIBundle {
	public CGUIElements[] list;
}

[System.Serializable]
public class CGUIElements { 
	public Rect loc = new Rect(0,0,100,100);	
	public int depth = 0;
	public int state = 0;
	public string text = "";
	public int fontSize = 11;
	public Texture image;
	public bool useStyle = false;
	public GUIStyle style = new GUIStyle();
	public GUIStyle style2 = new GUIStyle();
	public CGUICustom custom = new CGUICustom();
	public CBoxElement box = new CBoxElement();
	public CButtonElement button = new CButtonElement();
	public CLabelElement label = new CLabelElement();
	public CTextureElement texture = new CTextureElement();
	public CSliderElement slider = new CSliderElement();
	public CToggleElement toggle = new CToggleElement();
	public CTextAreaElement textArea = new CTextAreaElement();
	public CTextFieldElement textField = new CTextFieldElement();
	public CScrollViewElement scrollView = new CScrollViewElement();
	public CGUIElements linkedElement = null;
	public bool linkedTo = false;
	public Vector2 linkedOriginalLoc = Vector2.zero;
	public bool selected = false;
	public bool active = false;
	public bool displayed = false;
	public CGUIAnimationManager[] animation = new CGUIAnimationManager[0];
	public CFunctionManager functions = new CFunctionManager();
	public bool insideRect = false;
	/* Normal, Hover, Active, Focused, On Normal, On Hover, On Active, On Focused */
	public int guiState = 0;
	public bool mouseDown = false;
	public CGUIElements curElement = null;
	bool playingAnimation = false;
	
	public CGUIElements (){
	}
	
	public CGUIElements (CGUIElements myElement){
		loc = myElement.loc;
		depth = myElement.depth;
		state = myElement.state;
		text = myElement.text;
		fontSize = myElement.fontSize;
		image = myElement.image;
		useStyle = myElement.useStyle;
		style = new GUIStyle(myElement.style);
		style2 = new GUIStyle(myElement.style2);
		custom = new CGUICustom(myElement.custom);
		button = new CButtonElement(myElement.button);
		texture = new CTextureElement(myElement.texture);
		slider = new CSliderElement(myElement.slider);
		toggle = new CToggleElement(myElement.toggle);
		textArea = new CTextAreaElement(myElement.textArea);
		textField = new CTextFieldElement(myElement.textField);
		scrollView = new CScrollViewElement(myElement.scrollView);
		linkedElement = myElement.linkedElement;
		linkedTo = myElement.linkedTo;
		linkedOriginalLoc = myElement.linkedOriginalLoc;
		active = myElement.active;
		displayed = myElement.displayed;
		animation = myElement.animation;
		functions = new CFunctionManager(myElement.functions);
		insideRect = myElement.insideRect;
		guiState = myElement.guiState;
		mouseDown = myElement.mouseDown;
	}
	
	void ChangeDepth (int amount) {
		depth = depth + amount;
	}
	
	public void DrawElement (Rect disp, Event e){
		if(!custom.indv){
			GUI.backgroundColor = Color.white;
			GUI.contentColor = Color.white;
			GUI.color = custom.color;
			
		}
		else{
			GUI.color = Color.white;
			GUI.backgroundColor = custom.backgroundColor;
			GUI.contentColor = custom.contentColor;
		}
		int mouseState = 2;
		if(mouseDown){
			mouseState = 1;
		}
		if(e.button == 0 && e.type == EventType.MouseDown){
			if(!mouseDown){
				mouseDown = true;
				mouseState = 0;
			}
			else{
				mouseState = 1;
			}
		}
		if(e.button == 0 && e.type == EventType.MouseUp){
			mouseDown = false;
			mouseState = 2;
		}
		Rect newLoc = loc;
		if(animation.Length > 0){
			newLoc = new Rect((int) (newLoc.x), (int) (newLoc.y), newLoc.width, newLoc.height);
		}
		if(linkedElement == null){
			newLoc = new Rect(newLoc.x + disp.x, newLoc.y + disp.y, newLoc.width + disp.width, newLoc.height + disp.height);
		}
		else{
			if(linkedElement.state != 8){
				loc = new Rect(loc.x + (linkedElement.loc.x-linkedOriginalLoc.x), loc.y + (linkedElement.loc.y-linkedOriginalLoc.y), loc.width, loc.height);
				linkedOriginalLoc = new Vector2(linkedElement.loc.x, linkedElement.loc.y);
				newLoc = new Rect(newLoc.x + disp.x, newLoc.y + disp.y, newLoc.width, newLoc.height);
			}
			else{
				newLoc = new Rect(newLoc.x, newLoc.y, newLoc.width, newLoc.height);
			}
			linkedElement.linkedTo = true;
		}
		if(e.mousePosition.x >= newLoc.x && e.mousePosition.x <= newLoc.x+newLoc.width && e.mousePosition.y >= newLoc.y && e.mousePosition.y <= newLoc.y + newLoc.height){
			if(insideRect){
				if(mouseState == 1){
					guiState = 6;
				}
				else if(mouseState == 0){
					guiState = 2;
				}
				else if(mouseState == 2){
					guiState = 1;
				}
			}
			else{
				insideRect = true;
				guiState = 5;
			}
		}
		else{
			if(insideRect){
				guiState = 4;
			}
			else{
				guiState = 0;
			}
			insideRect = false;
			
		}
		functions.Functions(guiState);
		GUIContent content;
		if(image){
			content = new GUIContent(text, image);
		}
		else{
			content = new GUIContent(text);
		}
		if(state == 0){
			label.DrawElement(newLoc, content, style, useStyle, fontSize);
		}
		else if(state == 1){
			button.DrawElement(newLoc, content, style, useStyle, fontSize);
		}
		else if(state == 2){
			box.DrawElement(newLoc, content, style, useStyle, fontSize);
		}
		else if(state == 3){
			texture.DrawElement(newLoc, image);
		}
		else if(state == 4){
			slider.DrawElement(newLoc, style, style2, useStyle, fontSize);
		}
		else if(state == 5){
			toggle.DrawElement(newLoc, content, style, useStyle, fontSize);
		}
		else if(state == 6){
			GUI.skin.textArea.fontSize = fontSize;
			GUI.skin.textArea.richText = true;
			if(useStyle){
				text = GUI.TextArea(newLoc, text, textArea.maxLength, style);
			}
			else{
				text = GUI.TextArea(newLoc, text, textArea.maxLength);
			}
			GUI.skin.textArea.fontSize = 0;
			GUI.skin.textArea.richText = false;
		}
		else if(state == 7){
			GUI.skin.textField.fontSize = fontSize;
			GUI.skin.textField.richText = true;
			if(useStyle){
				text = GUI.TextField(newLoc, text, textField.maxLength, style);
			}
			else{
				text = GUI.TextField(newLoc, text, textField.maxLength);
			}
			GUI.skin.textField.fontSize = 0;
			GUI.skin.textField.richText = false;
		}
		else if(state == 8){
			scrollView.DrawElement(newLoc, content, style, useStyle);
			if(!linkedTo){
				EndElement();
			}
		}
		else if(state == 9){
			//Window
		}
	}
	
	public int PlayAnimation (int anim, float time, CGUIElements myElement) {
		if(!playingAnimation){
			animation[anim].StartAnimation();
			playingAnimation = true;
		}
		int curFrame = 0;
		if(animation.Length > 0){
			return animation[anim].PlayAnimation(time);
		}
		else{
			return 0;
		}
	}
	
	public void StopAnimation (int anim) {
		playingAnimation = false;
		if(animation.Length > 0){
			animation[anim].StopAnimation();
		}
	}
	
	public void EndElement () {
		if(state == 8){
			GUI.EndScrollView();
		}
	}
}

[System.Serializable]
public class CLabelElement {
	public void DrawElement (Rect loc, GUIContent content, GUIStyle style, bool useStyle, int fontSize) {
		GUI.skin.label.fontSize = fontSize;
		if(useStyle){	
			GUI.Label(loc, content, style);
		}
		else{
			GUI.Label(loc, content);
		}
		GUI.skin.label.fontSize = 0;
	}
}

[System.Serializable]
public class CBoxElement {
	public void DrawElement (Rect loc, GUIContent content, GUIStyle style, bool useStyle, int fontSize) {
		GUI.skin.box.fontSize = fontSize;
		GUI.skin.box.richText = true;
		if(useStyle){	
			GUI.Box(loc, content, style);
		}
		else{
			GUI.Box(loc, content);
		}
		GUI.skin.box.fontSize = 0;
	}
}

[System.Serializable]
public class CButtonElement {
	public bool repeat = false;
	
	public CButtonElement (){
		repeat = false;
	}
	public CButtonElement (CButtonElement element){
		repeat = element.repeat;
	}
	public void DrawElement (Rect loc, GUIContent content, GUIStyle style, bool useStyle, int fontSize) {
		GUI.skin.button.fontSize = fontSize;
		GUI.skin.button.richText = true;
		if(useStyle){
			if(repeat){
				GUI.RepeatButton(loc, content, style);
			}
			else{
				GUI.Button(loc, content, style);
			}
		}
		else{
			if(repeat){
				GUI.RepeatButton(loc, content);
			}
			else{
				GUI.Button(loc, content);
			}
		}
		GUI.skin.button.fontSize = 0;
	}
}

[System.Serializable]
public class CTextureElement {
	public int scale;
	public Rect ratio = new Rect(0,0,10,10);
	
	public CTextureElement (){
		scale = 1;
		ratio = new Rect(0,0,10,10);
	}
	public CTextureElement (CTextureElement element){
		scale = element.scale;
		ratio = element.ratio;
	}
	public void DrawElement (Rect loc, Texture image) {
		ScaleMode scaling = ScaleMode.StretchToFill;
		if(scale == 0){
			scaling = ScaleMode.StretchToFill;
		}
		else if(scale == 1){
			scaling = ScaleMode.ScaleAndCrop;
		}
		else if(scale == 2){
			scaling = ScaleMode.ScaleToFit;
		}
		else if(scale == 3){
			scaling = ScaleMode.ScaleAndCrop;
		}
		if(image){
			if(scale != 3){
				GUI.DrawTexture(loc, image, scaling);
			}
			else{
				GUI.DrawTextureWithTexCoords(loc, image, ratio);
			}
		}
	}
}

[System.Serializable]
public class CSliderElement {
	public bool vertical = false;
	public float value;
	public float size;
	public float leftValue;
	public float rightValue;
	
	public CSliderElement (){
		vertical = false;
		value = 0;
		size = 100;
		leftValue = 0;
		rightValue = 0;
	}
	public CSliderElement (CSliderElement element){
		vertical = element.vertical;
		value = element.value;
		size = element.size;
		leftValue = element.leftValue;
		rightValue = element.rightValue;
	}
	
	public void DrawElement (Rect loc, GUIStyle style, GUIStyle thumbStyle, bool useStyle, int fontSize) {
		GUI.skin.horizontalSlider.fontSize = fontSize;
		GUI.skin.verticalSlider.fontSize = fontSize;
		GUI.skin.verticalSlider.richText = true;
		GUI.skin.horizontalSlider.richText = true;
		if(vertical){
			if(useStyle){	
				value = GUI.VerticalSlider(loc, value, leftValue, rightValue, style, thumbStyle);
			}
			else{
				value = GUI.VerticalSlider(loc, value, leftValue, rightValue);
			}
		}
		else{
			if(useStyle){	
				value = GUI.HorizontalSlider(loc, value, leftValue, rightValue, style, thumbStyle);
			}
			else{
				value = GUI.HorizontalSlider(loc, value, leftValue, rightValue);
			}

		}
		GUI.skin.horizontalSlider.fontSize = 0;
		GUI.skin.verticalSlider.fontSize = 0;
	}
}

[System.Serializable]
public class CToggleElement {
	public bool value = false;
	
	public CToggleElement (){
		value = false;
	}
	public CToggleElement (CToggleElement element){
		value = element.value;
	}
	public void DrawElement (Rect loc, GUIContent content, GUIStyle style, bool useStyle, int fontSize) {
		GUI.skin.toggle.fontSize = fontSize;
		GUI.skin.toggle.richText = true;
		if(useStyle){
			value = GUI.Toggle(loc, value, content, style);
		}
		else{
			value = GUI.Toggle(loc, value, content);
		}
		GUI.skin.toggle.fontSize = 0;
	}
}

[System.Serializable]
public class CTextAreaElement {
	public int maxLength = 1000;
	public CTextAreaElement () {
		maxLength = 1000;
	}
	public CTextAreaElement (CTextAreaElement element){
		maxLength = element.maxLength;
	}
}

[System.Serializable]
public class CTextFieldElement {
	public int maxLength = 1000;
	
	public CTextFieldElement () {
		maxLength = 1000;
	}
	public CTextFieldElement (CTextFieldElement element){
		maxLength = element.maxLength;
	}
}

[System.Serializable]
public class CGUICustom {
	public Color color = new Color(1f,1f,1f,1f);
	public bool indv = false;
	public Color backgroundColor = Color.white;
	public Color contentColor = Color.white;
	
	public CGUICustom (){
	
	}
	public CGUICustom (CGUICustom custom){
		color = custom.color;
		indv = custom.indv;
		backgroundColor = custom.backgroundColor;
		contentColor = custom.contentColor;
	}
}

[System.Serializable]
public class CScrollViewElement {
	public Vector2 scrollPos;
	public Rect viewRect;
	
	public CScrollViewElement () {
		scrollPos = Vector2.zero;
		viewRect = new Rect(0,0,100, 0);
	}
	
	public CScrollViewElement (CScrollViewElement element) {
		scrollPos = element.scrollPos;
		viewRect = element.viewRect;
	}
	
	public void DrawElement (Rect loc, GUIContent content, GUIStyle style, bool useStyle){
		 scrollPos = GUI.BeginScrollView(loc, scrollPos, viewRect);
	}
}

[System.Serializable]
public class CFunctionManager { 
	public CFunctionElement[] normalFunction = new CFunctionElement[0];
	public CFunctionElement[] hoverFunction = new CFunctionElement[0];
	public CFunctionElement[] activeFunction = new CFunctionElement[0];
	public CFunctionElement[] focusedFunction = new CFunctionElement[0];
	public CFunctionElement[] onNormalFunction = new CFunctionElement[0];
	public CFunctionElement[] onHoverFunction = new CFunctionElement[0];
	public CFunctionElement[] onActiveFunction = new CFunctionElement[0];
	public CFunctionElement[] onFocusedFunction = new CFunctionElement[0];
	
	public CFunctionManager () {
	
	}
	public CFunctionManager (CFunctionManager manager){
		normalFunction = new CFunctionElement[manager.normalFunction.Length];
		for(int x = 0; x < normalFunction.Length; x++){
			normalFunction[x] = new CFunctionElement(manager.normalFunction[x]);
		}
		hoverFunction = new CFunctionElement[manager.hoverFunction.Length];
		for(int x = 0; x < hoverFunction.Length; x++){
			hoverFunction[x] = new CFunctionElement(manager.hoverFunction[x]);
		}
		activeFunction = new CFunctionElement[manager.activeFunction.Length];
		for(int x = 0; x < activeFunction.Length; x++){
			activeFunction[x] = new CFunctionElement(manager.activeFunction[x]);
		}
		focusedFunction = new CFunctionElement[manager.focusedFunction.Length];
		for(int x = 0; x < focusedFunction.Length; x++){
			focusedFunction[x] = new CFunctionElement(manager.focusedFunction[x]);
		}
		onNormalFunction = new CFunctionElement[manager.onNormalFunction.Length];
		for(int x = 0; x < onNormalFunction.Length; x++){
			onNormalFunction[x] = new CFunctionElement(manager.onNormalFunction[x]);
		}
		onHoverFunction = new CFunctionElement[manager.onHoverFunction.Length];
		for(int x = 0; x < onHoverFunction.Length; x++){
			onHoverFunction[x] = new CFunctionElement(manager.onHoverFunction[x]);
		}
		onActiveFunction = new CFunctionElement[manager.onActiveFunction.Length];
		for(int x = 0; x < onActiveFunction.Length; x++){
			onActiveFunction[x] = new CFunctionElement(manager.onActiveFunction[x]);
		}
		onFocusedFunction = new CFunctionElement[manager.onFocusedFunction.Length];
		for(int x = 0; x < onFocusedFunction.Length; x++){
			onFocusedFunction[x] = new CFunctionElement(manager.onFocusedFunction[x]);
		}
	}
	
	public void Functions (int state){
		if(state == 0){
			for(int x = 0; x < normalFunction.Length; x++){
				normalFunction[x].CallFunction();
			}
		}
		else if(state == 1){
			for(int x = 0; x < hoverFunction.Length; x++){
				hoverFunction[x].CallFunction();
			}
		}
		else if(state == 2){
			for(int x = 0; x < activeFunction.Length; x++){
				activeFunction[x].CallFunction();
			}
		}
		else if(state == 3){
			for(int x = 0; x < focusedFunction.Length; x++){
				focusedFunction[x].CallFunction();
			}
		}
		else if(state == 4){
			for(int x = 0; x < onNormalFunction.Length; x++){
				onNormalFunction[x].CallFunction();
			}
		}
		else if(state == 5){
			for(int x = 0; x < onHoverFunction.Length; x++){
				onHoverFunction[x].CallFunction();
			}
		}
		else if(state == 6){
			for(int x = 0; x < onActiveFunction.Length; x++){
				onActiveFunction[x].CallFunction();
			}
		}
		else if(state == 7){
			for(int x = 0; x < onFocusedFunction.Length; x++){
				onFocusedFunction[x].CallFunction();
			}
		}
	}
}

[System.Serializable]
public class CFunctionElement {
	public GameObject obj;
	public string compName = "";
	public string functionName;
	public float value;
	
	public CFunctionElement (){
	
	}
	
	public CFunctionElement (CFunctionElement func){
		obj = func.obj;
		compName = func.compName;
		functionName = func.functionName;
		value = func.value;
	}
	
	public void CallFunction () {
		if(obj){
			if(compName != ""){
				Component comp = obj.GetComponent(compName);
				if(comp){
					comp.SendMessage(functionName, value, SendMessageOptions.DontRequireReceiver);
				}
			}
			else{
				obj.SendMessage(functionName, value, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}