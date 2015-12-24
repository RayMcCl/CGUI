using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class CallFunctions : MonoBehaviour {
	public AudioSource source;
	public AudioClip clickedClip;
	public AudioClip hoverClip;
	public int lastNum;
	public GameObject activeObj;
	
	public void Normal (int num) {
		
	}
	public void Hover (int num) {
	}
	public void Active (int num) {
		source.clip = clickedClip;
		source.loop = false;
		source.Play();
	}
	public void Focused (int num) {
		
	}
	public void OnNormal (int num) {
		
	}
	public void OnHover (int num) {
		source.clip = hoverClip;
		source.loop = false;
		source.Play();
	}
	public void OnActive (int num) {
		
	}
	public void OnFocused (int num) {
		
	}
	public void ResetHover (int num){
		if(lastNum == num){
			lastNum = -1;
		}
	}
	public void SetActive (int num){
		activeObj.SetActive(!activeObj.active);
	}
}
