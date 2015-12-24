using UnityEngine;
using System.Collections;

public class CGUIEditorTextures : MonoBehaviour {
	public Texture[] scaleTexture = new Texture[8];
	//Top Left, Top, Top Right, Right, Bottom Right, Bottom, Bottom Left, Left
	public Texture[] scaleTextureGreen = new Texture[8];
	//Top Left, Top, Top Right, Right, Bottom Right, Bottom, Bottom Left, Left
	public Texture spaceTexture;
	public Texture verticalSpaceTexture;
	public Texture gridTexture;
	public Color gridColor;
	public Color importantColors;
	public int nodeSize;
	public Texture backgroundTexture;
	public Texture tempTexture;
	public Texture tempBackgroundTexture;
	public Texture playTexture;
	public Texture pauseTexture;
	public Texture stopTexture;
	public GUISkin emptySkin;
	public Texture keyFrameTexture;
	public Texture usedKeyFrameTexture;
	public Texture selectedKeyFrameTexture;
	public Texture arrowKeyFrameTexture;
	public Texture lineKeyFrameTexture;
	public Texture closeTexture;
	public int curObj = 0;
	public GUISkin customSkin;
	public GUIStyle textStyle;
	public bool selected = false;
	public Texture selectTexture;
	public Texture blackOutlineTexture;
	public string editorState = "Editor";
	public string editorStateA = "Stop";
	public int curAnimation = 0;
	public int curFrame = 0;
	public int curAnimObj = 0;
	
	
	// Use this for initialization
	void OnDrawGizmos () {
		if(gameObject.name != "CGUIETextures"){
			gameObject.name = "CGUIETextures";
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
