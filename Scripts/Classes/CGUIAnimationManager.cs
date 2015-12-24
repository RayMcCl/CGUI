using UnityEngine;
using System.Collections;

[System.Serializable]
public class CGUIAnimationManager {
	public string name;
	public int id;
	public Frame[] frame = new Frame[0];
	public int curFrame = 0;
	public int rate;
	public float lastFrame = 0;
	
	public CGUIAnimationManager () {
		rate = 1;
	}
	
	public void StartAnimation () {
		lastFrame = 0;
	}
	
	public int PlayAnimation (float time) {
		if(lastFrame + (1.0/rate) < time){
			curFrame++;
			if(frame.Length <= curFrame){
				curFrame = 0;
			}
			lastFrame = time;
		}
		return curFrame;
	}
	
	
	public void StopAnimation () {
		
	}
}

[System.Serializable]
public class Frame {
	public CGUIElements element;
	public int curFrame = 0;
	public int state = 0;
	
	public void Animation () {
		
	}
}
