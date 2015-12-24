using UnityEngine;

using UnityEditor;

using System.Collections;

using System.Reflection;

 

public class CopyEditorSkin : EditorWindow {

    public GUIStyle skin;
   
 

    [MenuItem("Window/CopyEditorSkin")]

    public static void Init() {

        CopyEditorSkin window = (CopyEditorSkin)EditorWindow.GetWindow(typeof(CopyEditorSkin));

    }

 

    public void OnGUI() {
        if(GUILayout.Button("Copy Editor Skin")) {
		CGUIEditorTextures editor = GameObject.Find("CGUIETextures").GetComponent<CGUIEditorTextures>();
		GUISkin builtinSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
		editor.textStyle.normal.background = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textArea.normal.background;
		editor.textStyle.hover.background = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textArea.hover.background;
		editor.textStyle.active.background = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textArea.active.background;
		editor.textStyle.focused.background = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textArea.focused.background;
		editor.textStyle.onNormal.background = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textArea.onNormal.background;
		editor.textStyle.onHover.background = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textArea.onHover.background;
		editor.textStyle.onActive.background = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textArea.onActive.background;
		editor.textStyle.onFocused.background = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textArea.onFocused.background;
        }

    }

}