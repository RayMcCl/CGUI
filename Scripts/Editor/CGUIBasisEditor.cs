using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CGUIBasis))]
public class CGUIBasisEditor : Editor {
	
	GUISkin skin;
	CGUIEditorTextures text;
	int[] windowState = new int[5];
	Color myColor = Color.black;
	int myFont = 12;
	bool mouseDown = false;
	bool htmlEditor = false;
	bool normalEditor = true;
	TextEditor t;
	Vector2 scroll;
	Vector2 scroll2;
	int bloc = 0;
	int functionState = 0;
	bool[] functionBool = new bool[0];
	CFunctionElement[] myElement;
	
	string ColorToHex(Color32 color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}
	
	public override void OnInspectorGUI () {
		CGUIBasis myTarget = target as CGUIBasis;
		if(!skin){
			skin = GUI.skin;
		}
		if(!text){
			text = GameObject.Find("CGUIETextures").GetComponent<CGUIEditorTextures>();
		}
		if(text.selected){
			CGUIElements nTarget = null;
			if(text.editorState == "Editor"){
				nTarget = myTarget.gui.list[text.curObj];
			}
			else if(text.editorState == "Animator"){
				nTarget = myTarget.gui.list[text.curObj].animation[text.curAnimation].frame[text.curFrame].element;
			}
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Text")){
				bloc = 0;
			}
			if(GUILayout.Button("Details")){
				bloc = 1;
			}
			if(GUILayout.Button("Functions")){
				bloc = 2;
			}
			GUILayout.EndHorizontal();
			if(bloc == 0){
				GUILayout.BeginHorizontal();
				normalEditor = EditorGUILayout.Toggle("Normal Editor", normalEditor);
				htmlEditor = EditorGUILayout.Toggle("HTML Editor", htmlEditor);
				GUILayout.EndHorizontal();
				if(normalEditor){
					
					EditorGUILayout.Space();
					text.textStyle.richText = EditorGUILayout.Toggle("Rich Text : ", text.textStyle.richText);
					scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height (150));
					nTarget.text = EditorGUILayout.TextArea(nTarget.text, text.textStyle);
					EditorGUILayout.EndScrollView();
					
				}
				GUILayout.BeginHorizontal();
				if(htmlEditor){	
					if(GUILayout.Button("Bold")){
						Debug.Log(t.pos);
						nTarget.text = nTarget.text.Insert(t.pos, "<b>");
						nTarget.text = nTarget.text.Insert(t.pos+t.SelectedText.Length+3, "</b>");
						t.pos = t.pos + 3;
						t.selectPos = t.selectPos + 3;
					}
					myFont = EditorGUILayout.IntField("Font Size : ", myFont);
					if(GUILayout.Button("Change Size")){
						int lastLength = nTarget.text.Length;
						nTarget.text = nTarget.text.Insert(t.pos, "<size=" + myFont + ">");
						int newLength = nTarget.text.Length;
						nTarget.text = nTarget.text.Insert(t.pos+t.SelectedText.Length+newLength-lastLength, "</size>");
						t.pos = t.pos + newLength-lastLength;
						t.selectPos = t.selectPos + newLength-lastLength;
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					if(GUILayout.Button("Italics")){
						nTarget.text = nTarget.text.Insert(t.pos, "<i>");
						nTarget.text = nTarget.text.Insert(t.pos+t.SelectedText.Length+3, "</i>");
						t.pos = t.pos + 3;
						t.selectPos = t.selectPos + 3;
					}
					myColor = EditorGUILayout.ColorField("Text Color : ", myColor);
					if(GUILayout.Button("Change Color")){
						string hex = ColorToHex(myColor);
						int lastLength = nTarget.text.Length;
						nTarget.text = nTarget.text.Insert(t.pos, "<color=#" + hex + ">");
						int newLength = nTarget.text.Length;
						nTarget.text = nTarget.text.Insert(t.pos+t.SelectedText.Length+newLength-lastLength, "</color>");
						t.pos = t.pos + newLength-lastLength;
						t.selectPos = t.selectPos + newLength-lastLength;
					}
				}
				GUILayout.EndHorizontal();
				t = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl); 
				if(htmlEditor){
					EditorGUILayout.Space();
					scroll2 = EditorGUILayout.BeginScrollView(scroll2, GUILayout.Height (150));
					nTarget.text = GUILayout.TextArea(nTarget.text);
					EditorGUILayout.EndScrollView();
				}
			}
			else if(bloc == 1){
				if(!nTarget.useStyle){
					nTarget.fontSize = EditorGUILayout.IntField("Font Size : ", nTarget.fontSize);
				}
				else{
					nTarget.style.fontSize = EditorGUILayout.IntField("Font Size : ", nTarget.style.fontSize);
				}
				EditorGUILayout.Space();
				string[] types = {"Label", "Button", "Box", "Texture", "Slider", "Toggle", "Text Area", "Text Field", "Scroll View"};
				GUILayout.Label("Depth : " + nTarget.depth);
				nTarget.state = EditorGUILayout.Popup("Type : ", nTarget.state, types);
				nTarget.image = EditorGUILayout.ObjectField("Image", nTarget.image, typeof(Texture), true) as Texture;
				nTarget.loc = EditorGUILayout.RectField("GUI Position", nTarget.loc);
				nTarget.custom.indv = EditorGUILayout.Toggle("Seperate Color : ", nTarget.custom.indv);
				if(!nTarget.custom.indv){
					nTarget.custom.color = EditorGUILayout.ColorField("Color : ", nTarget.custom.color);
				}
				else{
					nTarget.custom.backgroundColor = EditorGUILayout.ColorField("Background Color : ", nTarget.custom.backgroundColor);
					nTarget.custom.contentColor = EditorGUILayout.ColorField("Content Color : ", nTarget.custom.contentColor);
				}
				if(nTarget.state == 1){
					nTarget.button.repeat = EditorGUILayout.Toggle("Repeat Button", nTarget.button.repeat);
				}
				else if(nTarget.state == 3){
					string[] scaleTypes = {"Stretch To Fill", "Scale And Crop", "Scale To Fit", "Tex Coords"};
					nTarget.texture.scale = EditorGUILayout.Popup("Scale Mode : ", nTarget.texture.scale, scaleTypes);
					if(nTarget.texture.scale == 3){
						nTarget.texture.ratio = EditorGUILayout.RectField("Ratio : ", nTarget.texture.ratio);
					}
				}
				else if(nTarget.state == 4){
					CSliderElement myVar = nTarget.slider;
					myVar.value = EditorGUILayout.FloatField("Value : ", myVar.value);
					myVar.leftValue = EditorGUILayout.FloatField("Left Value : ", myVar.leftValue);
					myVar.rightValue = EditorGUILayout.FloatField("Right Value : ", myVar.rightValue);
					myVar.vertical = EditorGUILayout.Toggle("Vertical : ", myVar.vertical);
					EditorGUILayout.BeginHorizontal("");
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
					}
					if(GUILayout.Button("Content")){
						windowState = new int[5];
						windowState[0] = 2;
					}
					EditorGUILayout.EndHorizontal();
					if(windowState[0] == 0){
						EditorGUILayout.BeginHorizontal("");
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
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal("");
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
						EditorGUILayout.EndHorizontal();
						if(windowState[1] == 0){
							EditorGUILayout.BeginHorizontal("");
							nTarget.useStyle = EditorGUILayout.Toggle("Use Style : ", nTarget.useStyle);
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal("");
							nTarget.style2.normal.background = EditorGUILayout.ObjectField("Texture : ", nTarget.style2.normal.background, typeof(Texture2D)) as Texture2D;
							nTarget.style2.normal.textColor = EditorGUILayout.ColorField("Color : ", nTarget.style2.normal.textColor);
							EditorGUILayout.EndHorizontal();
						}
						else if(windowState[1] == 1){
							EditorGUILayout.BeginHorizontal("");
							nTarget.useStyle = EditorGUILayout.Toggle("Use Style : ", nTarget.useStyle);
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal("");
							nTarget.style2.hover.background = EditorGUILayout.ObjectField("Texture : ", nTarget.style2.hover.background, typeof(Texture2D)) as Texture2D;
							nTarget.style2.hover.textColor = EditorGUILayout.ColorField("Color : ", nTarget.style2.hover.textColor);
							EditorGUILayout.EndHorizontal();
						}
						else if(windowState[1] == 2){
							EditorGUILayout.BeginHorizontal("");
							nTarget.useStyle = EditorGUILayout.Toggle("Use Style : ", nTarget.useStyle);
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal("");
							nTarget.style2.active.background = EditorGUILayout.ObjectField("Texture : ", nTarget.style2.active.background, typeof(Texture2D)) as Texture2D;
							nTarget.style2.active.textColor = EditorGUILayout.ColorField("Color : ", nTarget.style2.active.textColor);
							EditorGUILayout.EndHorizontal();
						}
						else if(windowState[1] == 3){
							EditorGUILayout.BeginHorizontal("");
							nTarget.useStyle = EditorGUILayout.Toggle("Use Style : ", nTarget.useStyle);
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal("");
							nTarget.style2.focused.background = EditorGUILayout.ObjectField("Texture : ", nTarget.style2.focused.background, typeof(Texture2D)) as Texture2D;
							nTarget.style2.focused.textColor = EditorGUILayout.ColorField("Color : ", nTarget.style2.focused.textColor);
							EditorGUILayout.EndHorizontal();
						}
						else if(windowState[1] == 4){
							EditorGUILayout.BeginHorizontal("");
							nTarget.useStyle = EditorGUILayout.Toggle("Use Style : ", nTarget.useStyle);
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal("");
							nTarget.style2.onNormal.background = EditorGUILayout.ObjectField("Texture : ", nTarget.style2.onNormal.background, typeof(Texture2D)) as Texture2D;
							nTarget.style2.onNormal.textColor = EditorGUILayout.ColorField("Color : ", nTarget.style2.onNormal.textColor);
							EditorGUILayout.EndHorizontal();
						}
						else if(windowState[1] == 5){
							EditorGUILayout.BeginHorizontal("");
							nTarget.useStyle = EditorGUILayout.Toggle("Use Style : ", nTarget.useStyle);
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal("");
							nTarget.style2.onHover.background = EditorGUILayout.ObjectField("Texture : ", nTarget.style2.onHover.background, typeof(Texture2D)) as Texture2D;
							nTarget.style2.onHover.textColor = EditorGUILayout.ColorField("Color : ", nTarget.style2.onHover.textColor);
							EditorGUILayout.EndHorizontal();
						}
						else if(windowState[1] == 6){
							EditorGUILayout.BeginHorizontal("");
							nTarget.useStyle = EditorGUILayout.Toggle("Use Style : ", nTarget.useStyle);
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal("");
							nTarget.style2.onActive.background = EditorGUILayout.ObjectField("Texture : ", nTarget.style2.onActive.background, typeof(Texture2D)) as Texture2D;
							nTarget.style2.onActive.textColor = EditorGUILayout.ColorField("Color : ", nTarget.style2.onActive.textColor);
							EditorGUILayout.EndHorizontal();
						}
						else if(windowState[1] == 7){
							EditorGUILayout.BeginHorizontal("");
							nTarget.useStyle = EditorGUILayout.Toggle("Use Style : ", nTarget.useStyle);
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal("");
							nTarget.style2.onFocused.background = EditorGUILayout.ObjectField("Texture : ", nTarget.style2.onFocused.background, typeof(Texture2D)) as Texture2D;
							nTarget.style2.onFocused.textColor = EditorGUILayout.ColorField("Color : ", nTarget.style2.onFocused.textColor);
							EditorGUILayout.EndHorizontal();
						}
					}
					else if(windowState[0] == 1){
						EditorGUILayout.BeginHorizontal("");
						int curLoc = 0;
						int loc = 0;
						string[] prop = {"Upper Left", "Upper Center", "Upper Right", "Middle Left", "Middle Center", "Middle Right", "Lower Left", "Lower Center", "Lower Right"};
						if(nTarget.style2.alignment == TextAnchor.UpperLeft)
							curLoc = 0;
						else if(nTarget.style2.alignment == TextAnchor.UpperCenter)
							curLoc = 1;
						else if(nTarget.style2.alignment == TextAnchor.UpperRight)
							curLoc = 2;
						else if(nTarget.style2.alignment == TextAnchor.MiddleLeft)
							curLoc = 3;
						else if(nTarget.style2.alignment == TextAnchor.MiddleCenter)
							curLoc = 4;
						else if(nTarget.style2.alignment == TextAnchor.MiddleRight)
							curLoc = 5;
						else if(nTarget.style2.alignment == TextAnchor.LowerLeft)
							curLoc = 6;
						else if(nTarget.style2.alignment == TextAnchor.LowerCenter)
							curLoc = 7;
						else if(nTarget.style2.alignment == TextAnchor.LowerRight)
							curLoc = 8;
						loc = curLoc;
						curLoc = EditorGUILayout.Popup("Alignment : ", curLoc, prop);
						if(curLoc == 0)
							nTarget.style2.alignment = TextAnchor.UpperLeft;
						else if(curLoc == 1)
							nTarget.style2.alignment = TextAnchor.UpperCenter;
						else if(curLoc == 2)
							nTarget.style2.alignment = TextAnchor.UpperRight;
						else if(curLoc == 3)
							nTarget.style2.alignment = TextAnchor.MiddleLeft;
						else if(curLoc == 4)
							nTarget.style2.alignment = TextAnchor.MiddleCenter;
						else if(curLoc == 5)
							nTarget.style2.alignment = TextAnchor.MiddleRight;
						else if(curLoc == 6)
							nTarget.style2.alignment = TextAnchor.LowerLeft;
						else if(curLoc == 7)
							nTarget.style2.alignment = TextAnchor.LowerCenter;
						else if(curLoc == 8)
							nTarget.style2.alignment = TextAnchor.LowerRight;
						EditorGUILayout.EndHorizontal();
						//Border
						GUILayout.Label("Border : ");
						EditorGUILayout.BeginHorizontal("");
						int rectPoint = nTarget.style2.border.left;
						rectPoint = EditorGUILayout.IntField("Left", rectPoint);
						nTarget.style2.border = new RectOffset(rectPoint, nTarget.style2.border.right, nTarget.style2.border.top, nTarget.style2.border.bottom);
						rectPoint = nTarget.style2.border.right;
						rectPoint = EditorGUILayout.IntField("Right", rectPoint);
						nTarget.style2.border = new RectOffset(nTarget.style2.border.left, rectPoint, nTarget.style2.border.top, nTarget.style2.border.bottom);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal("");
						rectPoint = nTarget.style2.border.top;
						rectPoint = EditorGUILayout.IntField("Top", rectPoint);
						nTarget.style2.border = new RectOffset(nTarget.style2.border.left, nTarget.style2.border.right, rectPoint, nTarget.style2.border.bottom);
						rectPoint = nTarget.style2.border.bottom;
						rectPoint = EditorGUILayout.IntField("Bottom", rectPoint);
						nTarget.style2.border = new RectOffset(nTarget.style2.border.left, nTarget.style2.border.right, nTarget.style2.border.top, rectPoint);
						EditorGUILayout.EndHorizontal();
						//Margin
						GUILayout.Label("Margin : ");
						EditorGUILayout.BeginHorizontal("");
						rectPoint = nTarget.style2.margin.left;
						rectPoint = EditorGUILayout.IntField("Left", rectPoint);
						nTarget.style2.margin = new RectOffset(rectPoint, nTarget.style2.margin.right, nTarget.style2.margin.top, nTarget.style2.margin.bottom);
						rectPoint = nTarget.style2.margin.right;
						rectPoint = EditorGUILayout.IntField("Right", rectPoint);
						nTarget.style2.margin = new RectOffset(nTarget.style2.margin.left, rectPoint, nTarget.style2.margin.top, nTarget.style2.margin.bottom);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal("");
						rectPoint = nTarget.style2.margin.top;
						rectPoint = EditorGUILayout.IntField("Top", rectPoint);
						nTarget.style2.margin = new RectOffset(nTarget.style2.margin.left, nTarget.style2.margin.right, rectPoint, nTarget.style2.margin.bottom);
						rectPoint = nTarget.style2.margin.bottom;
						rectPoint = EditorGUILayout.IntField("Bottom", rectPoint);
						nTarget.style2.margin = new RectOffset(nTarget.style2.margin.left, nTarget.style2.margin.right, nTarget.style2.margin.top, rectPoint);
						EditorGUILayout.EndHorizontal();
						//Padding
						GUILayout.Label("Padding : ");
						EditorGUILayout.BeginHorizontal("");
						rectPoint = nTarget.style2.padding.left;
						rectPoint = EditorGUILayout.IntField("Left", rectPoint);
						nTarget.style2.padding = new RectOffset(rectPoint, nTarget.style2.padding.right, nTarget.style2.padding.top, nTarget.style2.padding.bottom);
						rectPoint = nTarget.style2.padding.right;
						rectPoint = EditorGUILayout.IntField("Right", rectPoint);
						nTarget.style2.padding = new RectOffset(nTarget.style2.padding.left, rectPoint, nTarget.style2.padding.top, nTarget.style2.padding.bottom);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal("");
						rectPoint = nTarget.style2.padding.top;
						rectPoint = EditorGUILayout.IntField("Top", rectPoint);
						nTarget.style2.padding = new RectOffset(nTarget.style2.padding.left, nTarget.style2.padding.right, rectPoint, nTarget.style2.padding.bottom);
						rectPoint = nTarget.style2.padding.bottom;
						rectPoint = EditorGUILayout.IntField("Bottom", rectPoint);
						nTarget.style2.padding = new RectOffset(nTarget.style2.padding.left, nTarget.style2.padding.right, nTarget.style2.padding.top, rectPoint);
						EditorGUILayout.EndHorizontal();
						//Overflow
						GUILayout.Label("Overflow : ");
						EditorGUILayout.BeginHorizontal("");
						rectPoint = nTarget.style2.overflow.left;
						rectPoint = EditorGUILayout.IntField("Left", rectPoint);
						nTarget.style2.overflow = new RectOffset(rectPoint, nTarget.style2.overflow.right, nTarget.style2.overflow.top, nTarget.style2.overflow.bottom);
						rectPoint = nTarget.style2.overflow.right;
						rectPoint = EditorGUILayout.IntField("Right", rectPoint);
						nTarget.style2.overflow = new RectOffset(nTarget.style2.overflow.left, rectPoint, nTarget.style2.overflow.top, nTarget.style2.overflow.bottom);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal("");
						rectPoint = nTarget.style2.overflow.top;
						rectPoint = EditorGUILayout.IntField("Top", rectPoint);
						nTarget.style2.overflow = new RectOffset(nTarget.style2.overflow.left, nTarget.style2.overflow.right, rectPoint, nTarget.style2.overflow.bottom);
						rectPoint = nTarget.style2.overflow.bottom;
						rectPoint = EditorGUILayout.IntField("Bottom", rectPoint);
						nTarget.style2.overflow = new RectOffset(nTarget.style2.overflow.left, nTarget.style2.overflow.right, nTarget.style2.overflow.top, rectPoint);
						EditorGUILayout.EndHorizontal();
					}
					else if(windowState[0] == 2){
						nTarget.style2.font = EditorGUILayout.ObjectField("Font : ", nTarget.style2.font, typeof(Font)) as Font;
						nTarget.style2.fontSize = EditorGUILayout.IntField("Font Size : ", nTarget.style2.fontSize);
						int curLoc = 0;
						int loc = 0;
						string[] prop = {"Normal", "Bold", "Italic", "Bold and Italic"};
						if(nTarget.style2.fontStyle == FontStyle.Normal)
							curLoc = 0;
						else if(nTarget.style2.fontStyle == FontStyle.Bold)
							curLoc = 1;
						else if(nTarget.style2.fontStyle == FontStyle.Italic)
							curLoc = 2;
						else if(nTarget.style2.fontStyle == FontStyle.BoldAndItalic)
							curLoc = 3;
						curLoc = EditorGUILayout.Popup("Font Style : ", curLoc, prop);
						if(curLoc == 0)
							nTarget.style2.fontStyle = FontStyle.Normal;
						else if(curLoc == 1)
							nTarget.style2.fontStyle = FontStyle.Bold;
						else if(curLoc == 2)
							nTarget.style2.fontStyle = FontStyle.Italic;
						else if(curLoc == 3)
							nTarget.style2.fontStyle = FontStyle.BoldAndItalic;
						nTarget.style2.wordWrap = EditorGUILayout.Toggle("Word Wrap : ", nTarget.style2.wordWrap);
						nTarget.style2.richText = EditorGUILayout.Toggle("Rich Text : ", nTarget.style2.richText);
						if(!nTarget.style2.wordWrap){
							bool textClipping;
							if(nTarget.style2.clipping == TextClipping.Overflow){
								textClipping = true;
							}
							else{
								textClipping = false;
							}
							textClipping = EditorGUILayout.Toggle("Overflow : ", textClipping);
							if(textClipping){
								nTarget.style2.clipping = TextClipping.Overflow;
							}
							else{
								nTarget.style2.clipping = TextClipping.Clip;
							}
						}
						curLoc = 0;
						loc = 0;
						string[] ns = {"Image Left", "Image Above", "Image Only", "Text Only"};
						if(nTarget.style2.imagePosition == ImagePosition.ImageLeft)
							curLoc = 0;
						else if(nTarget.style2.imagePosition == ImagePosition.ImageAbove)
							curLoc = 1;
						else if(nTarget.style2.imagePosition == ImagePosition.ImageOnly)
							curLoc = 2;
						else if(nTarget.style2.imagePosition == ImagePosition.TextOnly)
							curLoc = 3;
						curLoc = EditorGUILayout.Popup("Image Position : ", curLoc, ns);
						if(curLoc == 0)
							nTarget.style2.imagePosition = ImagePosition.ImageLeft;
						else if(curLoc == 1)
							nTarget.style2.imagePosition = ImagePosition.ImageAbove;
						else if(curLoc == 2)
							nTarget.style2.imagePosition = ImagePosition.ImageOnly;
						else if(curLoc == 3)
							nTarget.style2.imagePosition = ImagePosition.TextOnly;
						nTarget.style2.fixedWidth = EditorGUILayout.FloatField("Fixed Width : ", nTarget.style2.fixedWidth);
						nTarget.style2.fixedHeight = EditorGUILayout.FloatField("Fixed Height : ", nTarget.style2.fixedHeight);
						nTarget.style2.stretchWidth = EditorGUILayout.Toggle("Stretch Width : ", nTarget.style2.stretchWidth);
						nTarget.style2.stretchHeight = EditorGUILayout.Toggle("Stretch Height : ", nTarget.style2.stretchHeight);
						nTarget.style2.contentOffset = EditorGUILayout.Vector2Field("Content Offset : ", nTarget.style2.contentOffset);
					}
				}
				else if(nTarget.state == 6 || nTarget.state == 7){
					if(nTarget.state == 6){
						nTarget.textArea.maxLength = EditorGUILayout.IntField("Max Length : ", nTarget.textArea.maxLength);
					}
					else{
						nTarget.textField.maxLength = EditorGUILayout.IntField("Max Length : ", nTarget.textField.maxLength);
					}
				}
			}
			else if(bloc == 2){ 
				string[] functionName = {"Normal", "Hover", "Active", "Focused", "On Normal", "On Hover", "On Active", "On Focused"};
				int[] options = {0,1,2,3,4,5,6,7};
				functionState = EditorGUILayout.IntPopup("State : ", functionState, functionName, options);
				if(functionState == 0){
					if(functionBool.Length != nTarget.functions.normalFunction.Length){
						functionBool = new bool[nTarget.functions.normalFunction.Length];
					}
					for(int x = 0; x < nTarget.functions.normalFunction.Length; x++){
						functionBool[x] = EditorGUILayout.Foldout(functionBool[x], "Function : " + (x+1));
						if(functionBool[x]){
							nTarget.functions.normalFunction[x].obj = EditorGUILayout.ObjectField("GameObject : ", nTarget.functions.normalFunction[x].obj, typeof(GameObject), true) as GameObject;
							nTarget.functions.normalFunction[x].functionName = EditorGUILayout.TextField("Function Name : ", nTarget.functions.normalFunction[x].functionName);
							nTarget.functions.normalFunction[x].compName = EditorGUILayout.TextField("Component Name : ", nTarget.functions.normalFunction[x].compName);
							nTarget.functions.normalFunction[x].value = EditorGUILayout.FloatField("Value : ", nTarget.functions.normalFunction[x].value);
							if(GUILayout.Button("Delete")){
								CopyFunction(myTarget, nTarget.functions.normalFunction.Length-1, nTarget.functions.normalFunction.Length, 0);
							}
						}
					}
					if(GUILayout.Button("Add")){
						CopyFunction(myTarget, nTarget.functions.normalFunction.Length+1, nTarget.functions.normalFunction.Length, 0);
					}
				}
				else if(functionState == 1){
					if(functionBool.Length != nTarget.functions.hoverFunction.Length){
						functionBool = new bool[nTarget.functions.hoverFunction.Length];
					}
					for(int x = 0; x < nTarget.functions.hoverFunction.Length; x++){
						functionBool[x] = EditorGUILayout.Foldout(functionBool[x], "Function : " + (x+1));
						if(functionBool[x]){
							nTarget.functions.hoverFunction[x].obj = EditorGUILayout.ObjectField("GameObject : ", nTarget.functions.hoverFunction[x].obj, typeof(GameObject), true) as GameObject;
							nTarget.functions.hoverFunction[x].functionName = EditorGUILayout.TextField("Function Name : ", nTarget.functions.hoverFunction[x].functionName);
							nTarget.functions.hoverFunction[x].compName = EditorGUILayout.TextField("Component Name : ", nTarget.functions.hoverFunction[x].compName);
							nTarget.functions.hoverFunction[x].value = EditorGUILayout.FloatField("Value : ", nTarget.functions.hoverFunction[x].value);
							if(GUILayout.Button("Delete")){
								CopyFunction(myTarget, nTarget.functions.hoverFunction.Length-1, nTarget.functions.hoverFunction.Length, 0);
							}
						}
						
					}
					if(GUILayout.Button("Add")){
						CopyFunction(myTarget, nTarget.functions.hoverFunction.Length+1, nTarget.functions.hoverFunction.Length, 0);
					}
				}
				else if(functionState == 2){
					if(functionBool.Length != nTarget.functions.activeFunction.Length){
						functionBool = new bool[nTarget.functions.activeFunction.Length];
					}
					for(int x = 0; x < nTarget.functions.activeFunction.Length; x++){
						functionBool[x] = EditorGUILayout.Foldout(functionBool[x], "Function : " + (x+1));
						if(functionBool[x]){
							nTarget.functions.activeFunction[x].obj = EditorGUILayout.ObjectField("GameObject : ", nTarget.functions.activeFunction[x].obj, typeof(GameObject), true) as GameObject;
							nTarget.functions.activeFunction[x].functionName = EditorGUILayout.TextField("Function Name : ", nTarget.functions.activeFunction[x].functionName);
							nTarget.functions.activeFunction[x].compName = EditorGUILayout.TextField("Component Name : ", nTarget.functions.activeFunction[x].compName);
							nTarget.functions.activeFunction[x].value = EditorGUILayout.FloatField("Value : ", nTarget.functions.activeFunction[x].value);
							if(GUILayout.Button("Delete")){
								CopyFunction(myTarget, nTarget.functions.activeFunction.Length-1, nTarget.functions.activeFunction.Length, 0);
							}
						}
					}
					if(GUILayout.Button("Add")){
						CopyFunction(myTarget, nTarget.functions.activeFunction.Length+1, nTarget.functions.activeFunction.Length, 0);
					}
				}
				else if(functionState == 3){
					if(functionBool.Length != nTarget.functions.focusedFunction.Length){
						functionBool = new bool[nTarget.functions.focusedFunction.Length];
					}
					for(int x = 0; x < nTarget.functions.focusedFunction.Length; x++){
						functionBool[x] = EditorGUILayout.Foldout(functionBool[x], "Function : " + (x+1));
						if(functionBool[x]){
							nTarget.functions.focusedFunction[x].obj = EditorGUILayout.ObjectField("GameObject : ", nTarget.functions.focusedFunction[x].obj, typeof(GameObject), true) as GameObject;
							nTarget.functions.focusedFunction[x].functionName = EditorGUILayout.TextField("Function Name : ", nTarget.functions.focusedFunction[x].functionName);
							nTarget.functions.focusedFunction[x].compName = EditorGUILayout.TextField("Component Name : ", nTarget.functions.focusedFunction[x].compName);
							nTarget.functions.focusedFunction[x].value = EditorGUILayout.FloatField("Value : ", nTarget.functions.focusedFunction[x].value);
							if(GUILayout.Button("Delete")){
								CopyFunction(myTarget, nTarget.functions.focusedFunction.Length-1, nTarget.functions.focusedFunction.Length, 0);
							}
						}
					}
					if(GUILayout.Button("Add")){
						CopyFunction(myTarget, nTarget.functions.focusedFunction.Length+1, nTarget.functions.focusedFunction.Length, 0);
					}
				}
				else if(functionState == 4){
					if(functionBool.Length != nTarget.functions.onNormalFunction.Length){
						functionBool = new bool[nTarget.functions.onNormalFunction.Length];
					}
					for(int x = 0; x < nTarget.functions.onNormalFunction.Length; x++){
						functionBool[x] = EditorGUILayout.Foldout(functionBool[x], "Function : " + (x+1));
						if(functionBool[x]){
							nTarget.functions.onNormalFunction[x].obj = EditorGUILayout.ObjectField("GameObject : ", nTarget.functions.onNormalFunction[x].obj, typeof(GameObject), true) as GameObject;
							nTarget.functions.onNormalFunction[x].functionName = EditorGUILayout.TextField("Function Name : ", nTarget.functions.onNormalFunction[x].functionName);
							nTarget.functions.onNormalFunction[x].compName = EditorGUILayout.TextField("Component Name : ", nTarget.functions.onNormalFunction[x].compName);
							nTarget.functions.onNormalFunction[x].value = EditorGUILayout.FloatField("Value : ", nTarget.functions.onNormalFunction[x].value);
							if(GUILayout.Button("Delete")){
								CopyFunction(myTarget, nTarget.functions.onNormalFunction.Length-1, nTarget.functions.onNormalFunction.Length, 0);
							}
						}
					}
					if(GUILayout.Button("Add")){
						CopyFunction(myTarget, nTarget.functions.onNormalFunction.Length+1, nTarget.functions.onNormalFunction.Length, 0);
					}
				}
				else if(functionState == 5){
					if(functionBool.Length != nTarget.functions.onHoverFunction.Length){
						functionBool = new bool[nTarget.functions.onHoverFunction.Length];
					}
					for(int x = 0; x < nTarget.functions.onHoverFunction.Length; x++){
						functionBool[x] = EditorGUILayout.Foldout(functionBool[x], "Function : " + (x+1));
						if(functionBool[x]){
							nTarget.functions.onHoverFunction[x].obj = EditorGUILayout.ObjectField("GameObject : ", nTarget.functions.onHoverFunction[x].obj, typeof(GameObject), true) as GameObject;
							nTarget.functions.onHoverFunction[x].functionName = EditorGUILayout.TextField("Function Name : ", nTarget.functions.onHoverFunction[x].functionName);
							nTarget.functions.onHoverFunction[x].compName = EditorGUILayout.TextField("Component Name : ", nTarget.functions.onHoverFunction[x].compName);
							nTarget.functions.onHoverFunction[x].value = EditorGUILayout.FloatField("Value : ", nTarget.functions.onHoverFunction[x].value);
							if(GUILayout.Button("Delete")){
								CopyFunction(myTarget, nTarget.functions.onHoverFunction.Length-1, nTarget.functions.onHoverFunction.Length, 0);
							}
						}
					}
					if(GUILayout.Button("Add")){
						CopyFunction(myTarget, nTarget.functions.onHoverFunction.Length+1, nTarget.functions.onHoverFunction.Length, 0);
					}
				}
				else if(functionState == 6){
					if(functionBool.Length != nTarget.functions.onActiveFunction.Length){
						functionBool = new bool[nTarget.functions.onActiveFunction.Length];
					}
					for(int x = 0; x < nTarget.functions.onActiveFunction.Length; x++){
						functionBool[x] = EditorGUILayout.Foldout(functionBool[x], "Function : " + (x+1));
						if(functionBool[x]){
							nTarget.functions.onActiveFunction[x].obj = EditorGUILayout.ObjectField("GameObject : ", nTarget.functions.onActiveFunction[x].obj, typeof(GameObject), true) as GameObject;
							nTarget.functions.onActiveFunction[x].functionName = EditorGUILayout.TextField("Function Name : ", nTarget.functions.onActiveFunction[x].functionName);
							nTarget.functions.onActiveFunction[x].compName = EditorGUILayout.TextField("Component Name : ", nTarget.functions.onActiveFunction[x].compName);
							nTarget.functions.onActiveFunction[x].value = EditorGUILayout.FloatField("Value : ", nTarget.functions.onActiveFunction[x].value);
							if(GUILayout.Button("Delete")){
								CopyFunction(myTarget, nTarget.functions.onActiveFunction.Length-1, nTarget.functions.onActiveFunction.Length, 0);
							}
						}
					}
					if(GUILayout.Button("Add")){
						CopyFunction(myTarget, nTarget.functions.onActiveFunction.Length+1, nTarget.functions.onActiveFunction.Length, 0);
					}
				}
				else if(functionState == 7){
					if(functionBool.Length != nTarget.functions.onFocusedFunction.Length){
						functionBool = new bool[nTarget.functions.onFocusedFunction.Length];
					}
					for(int x = 0; x < nTarget.functions.onFocusedFunction.Length; x++){
						functionBool[x] = EditorGUILayout.Foldout(functionBool[x], "Function : " + (x+1));
						if(functionBool[x]){
							nTarget.functions.onFocusedFunction[x].obj = EditorGUILayout.ObjectField("GameObject : ", nTarget.functions.onFocusedFunction[x].obj, typeof(GameObject), true) as GameObject;
							nTarget.functions.onFocusedFunction[x].functionName = EditorGUILayout.TextField("Function Name : ", nTarget.functions.onFocusedFunction[x].functionName);
							nTarget.functions.onFocusedFunction[x].compName = EditorGUILayout.TextField("Component Name : ", nTarget.functions.onFocusedFunction[x].compName);
							nTarget.functions.onFocusedFunction[x].value = EditorGUILayout.FloatField("Value : ", nTarget.functions.onFocusedFunction[x].value);
							if(GUILayout.Button("Delete")){
								CopyFunction(myTarget, nTarget.functions.onFocusedFunction.Length-1, nTarget.functions.onFocusedFunction.Length, 0);
							}
						}
					}
					if(GUILayout.Button("Add")){
						CopyFunction(myTarget, nTarget.functions.onFocusedFunction.Length+1, nTarget.functions.onFocusedFunction.Length, 0);
						CopyFunction(myTarget, nTarget.functions.onFocusedFunction.Length+1, nTarget.functions.onFocusedFunction.Length, 0);
					}
				}
			}
		}
		else if(text){
			myTarget.gui.resolution = EditorGUILayout.Vector2Field("Resolution : ", myTarget.gui.resolution);
		}
	}
	void CopyFunction (CGUIBasis myTarget, int newLength, int oldLength, int removeInt){
		CGUIElements nTarget = null;
		if(text.editorState == "Editor"){
			nTarget = myTarget.gui.list[text.curObj];
		}
		else if(text.editorState == "Animator"){
			nTarget = myTarget.gui.list[text.curObj].animation[text.curAnimation].frame[text.curFrame].element;
		}
		CFunctionElement[] myList = new CFunctionElement[oldLength];
		if(functionState == 0){
			if(newLength < oldLength){
				for(int x = 0; x < nTarget.functions.normalFunction.Length; x++){
					myList[x] = nTarget.functions.normalFunction[x];
				}
				nTarget.functions.normalFunction = new CFunctionElement[newLength];
				int y = 0;
				for(int x = 0; x < myList.Length; x++){
					if(x != removeInt){
						nTarget.functions.normalFunction[y] = myList[x];
						y++;
					}
				}
			}
			else{
				for(int x = 0; x < nTarget.functions.normalFunction.Length; x++){
					myList[x] = nTarget.functions.normalFunction[x];
				}
				nTarget.functions.normalFunction = new CFunctionElement[newLength];
				for(int x = 0; x < nTarget.functions.normalFunction.Length; x++){
					if(x < myList.Length){
						nTarget.functions.normalFunction[x] = myList[x];
					}
					else{
						nTarget.functions.normalFunction[x] = new CFunctionElement();
					}
				}
			}
		}
		else if(functionState == 1){
			if(newLength < oldLength){
				for(int x = 0; x < nTarget.functions.hoverFunction.Length; x++){
					myList[x] = nTarget.functions.hoverFunction[x];
				}
				nTarget.functions.hoverFunction = new CFunctionElement[newLength];
				int y = 0;
				for(int x = 0; x < myList.Length; x++){
					if(x != removeInt){
						nTarget.functions.hoverFunction[y] = myList[x];
						y++;
					}
				}
			}
			else{
				for(int x = 0; x < nTarget.functions.hoverFunction.Length; x++){
					myList[x] = nTarget.functions.hoverFunction[x];
				}
				nTarget.functions.hoverFunction = new CFunctionElement[newLength];
				for(int x = 0; x < nTarget.functions.hoverFunction.Length; x++){
					if(x < myList.Length){
						nTarget.functions.hoverFunction[x] = myList[x];
					}
					else{
						nTarget.functions.hoverFunction[x] = new CFunctionElement();
					}
				}
			}
		}
		else if(functionState == 2){
			if(newLength < oldLength){
				for(int x = 0; x < nTarget.functions.activeFunction.Length; x++){
					myList[x] = nTarget.functions.activeFunction[x];
				}
				nTarget.functions.activeFunction = new CFunctionElement[newLength];
				int y = 0;
				for(int x = 0; x < myList.Length; x++){
					if(x != removeInt){
						nTarget.functions.activeFunction[y] = myList[x];
						y++;
					}
				}
			}
			else{
				for(int x = 0; x < nTarget.functions.activeFunction.Length; x++){
					myList[x] = nTarget.functions.activeFunction[x];
				}
				nTarget.functions.activeFunction = new CFunctionElement[newLength];
				for(int x = 0; x < nTarget.functions.activeFunction.Length; x++){
					if(x < myList.Length){
						nTarget.functions.activeFunction[x] = myList[x];
					}
					else{
						nTarget.functions.activeFunction[x] = new CFunctionElement();
					}
				}
			}
		}
		else if(functionState == 3){
			if(newLength < oldLength){
				for(int x = 0; x < nTarget.functions.focusedFunction.Length; x++){
					myList[x] = nTarget.functions.focusedFunction[x];
				}
				nTarget.functions.focusedFunction = new CFunctionElement[newLength];
				int y = 0;
				for(int x = 0; x < myList.Length; x++){
					if(x != removeInt){
						nTarget.functions.focusedFunction[y] = myList[x];
						y++;
					}
				}
			}
			else{
				for(int x = 0; x < nTarget.functions.focusedFunction.Length; x++){
					myList[x] = nTarget.functions.focusedFunction[x];
				}
				nTarget.functions.focusedFunction = new CFunctionElement[newLength];
				for(int x = 0; x < nTarget.functions.focusedFunction.Length; x++){
					if(x < myList.Length){
						nTarget.functions.focusedFunction[x] = myList[x];
					}
					else{
						nTarget.functions.focusedFunction[x] = new CFunctionElement();
					}
				}
			}
		}
		else if(functionState == 4){
			if(newLength < oldLength){
				for(int x = 0; x < nTarget.functions.onNormalFunction.Length; x++){
					myList[x] = nTarget.functions.onNormalFunction[x];
				}
				nTarget.functions.onNormalFunction = new CFunctionElement[newLength];
				int y = 0;
				for(int x = 0; x < myList.Length; x++){
					if(x != removeInt){
						nTarget.functions.onNormalFunction[y] = myList[x];
						y++;
					}
				}
			}
			else{
				for(int x = 0; x < nTarget.functions.onNormalFunction.Length; x++){
					myList[x] = nTarget.functions.onNormalFunction[x];
				}
				nTarget.functions.onNormalFunction = new CFunctionElement[newLength];
				for(int x = 0; x < nTarget.functions.onNormalFunction.Length; x++){
					if(x < myList.Length){
						nTarget.functions.onNormalFunction[x] = myList[x];
					}
					else{
						nTarget.functions.onNormalFunction[x] = new CFunctionElement();
					}
				}
			}
		}
		else if(functionState == 5){
			if(newLength < oldLength){
				for(int x = 0; x < nTarget.functions.onHoverFunction.Length; x++){
					myList[x] = nTarget.functions.onHoverFunction[x];
				}
				nTarget.functions.onHoverFunction = new CFunctionElement[newLength];
				int y = 0;
				for(int x = 0; x < myList.Length; x++){
					if(x != removeInt){
						nTarget.functions.onHoverFunction[y] = myList[x];
						y++;
					}
				}
			}
			else{
				for(int x = 0; x < nTarget.functions.onHoverFunction.Length; x++){
					myList[x] = nTarget.functions.onHoverFunction[x];
				}
				nTarget.functions.onHoverFunction = new CFunctionElement[newLength];
				for(int x = 0; x < nTarget.functions.onHoverFunction.Length; x++){
					if(x < myList.Length){
						nTarget.functions.onHoverFunction[x] = myList[x];
					}
					else{
						nTarget.functions.onHoverFunction[x] = new CFunctionElement();
					}
				}
			}
		}
		else if(functionState == 6){
			if(newLength < oldLength){
				for(int x = 0; x < nTarget.functions.onActiveFunction.Length; x++){
					myList[x] = nTarget.functions.onActiveFunction[x];
				}
				nTarget.functions.onActiveFunction = new CFunctionElement[newLength];
				int y = 0;
				for(int x = 0; x < myList.Length; x++){
					if(x != removeInt){
						nTarget.functions.onActiveFunction[y] = myList[x];
						y++;
					}
				}
			}
			else{
				for(int x = 0; x < nTarget.functions.onActiveFunction.Length; x++){
					myList[x] = nTarget.functions.onActiveFunction[x];
				}
				nTarget.functions.onActiveFunction = new CFunctionElement[newLength];
				for(int x = 0; x < nTarget.functions.onActiveFunction.Length; x++){
					if(x < myList.Length){
						nTarget.functions.onActiveFunction[x] = myList[x];
					}
					else{
						nTarget.functions.onActiveFunction[x] = new CFunctionElement();
					}
				}
			}
		}
		else if(functionState == 7){
			if(newLength < oldLength){
				for(int x = 0; x < nTarget.functions.onFocusedFunction.Length; x++){
					myList[x] = nTarget.functions.onFocusedFunction[x];
				}
				nTarget.functions.onFocusedFunction = new CFunctionElement[newLength];
				int y = 0;
				for(int x = 0; x < myList.Length; x++){
					if(x != removeInt){
						nTarget.functions.onFocusedFunction[y] = myList[x];
						y++;
					}
				}
			}
			else{
				for(int x = 0; x < nTarget.functions.onFocusedFunction.Length; x++){
					myList[x] = nTarget.functions.onFocusedFunction[x];
				}
				nTarget.functions.onFocusedFunction = new CFunctionElement[newLength];
				for(int x = 0; x < nTarget.functions.onFocusedFunction.Length; x++){
					if(x < myList.Length){
						nTarget.functions.onFocusedFunction[x] = myList[x];
					}
					else{
						nTarget.functions.onFocusedFunction[x] = new CFunctionElement();
					}
				}
			}
		}
	}
}
