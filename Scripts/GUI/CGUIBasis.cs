using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CGUIBasis : MonoBehaviour {
	
	public CGUIManager gui;
	
	void OnDrawGizmos () {
		if(gameObject.name != "CGUIBasis"){
			gameObject.name = "CGUIBasis";
		}
		DrawElements(Vector2.zero);
	}
	
	void OnGUI () {
		DrawElements(Vector2.zero);
	}
	
	public void DrawElements (Vector2 disp) {
		Event e = Event.current;
		List<int> z = new List<int>();
		List<int> a = new List<int>();
		for(int x = 0; x < gui.list.Length; x++){
			if(gui.list[x].linkedElement != null){
				for(int y = 0; y < gui.list.Length; y++){
					if(gui.list[x].linkedElement == gui.list[y]){
						z.Add(y);
						a.Add(x);
						break;
					}
				}
			}
			else if(gui.list[x].linkedTo){
				z.Add(-2);
				a.Add(x);
			}
			else{
				z.Add(-1);
				a.Add(x);
			}
			gui.list[x].displayed = false;
		}
		List<int> b = new List<int>();
		List<int> v = new List<int>();
		for(int size = gui.list.Length; size > 0; size--){
			int highest = -20;
			int lowestInt = 0;
			int loc = 0;
			int lowestDepth = gui.list.Length;
			for(int y = 0; y < size; y++){
				if(highest <= z[y]){
					if(highest == z[y]){
						if(lowestDepth < gui.list[a[y]].depth){
							highest = z[y];
							lowestInt = a[y];
							loc = y;
							lowestDepth = gui.list[a[y]].depth;
						}
					}
					else{
						highest = z[y];
						lowestInt = a[y];
						loc = y;
						lowestDepth = gui.list[a[y]].depth;
					}
				}
			}
			b.Add(lowestInt);
			v.Add(highest);
			z.RemoveAt(loc);
			a.RemoveAt(loc);
		}
		for(int x = 0; x < gui.list.Length; x++){
			if(v[x] == -1){
				gui.list[b[x]].DrawElement(new Rect(disp.x, disp.y, 0, 0), e);
			}
			else if(v[x] != -2){
				if(!gui.list[v[x]].displayed){
					gui.list[v[x]].DrawElement(new Rect(disp.x, disp.y, 0, 0), e);
					gui.list[v[x]].displayed = true;
				}
				gui.list[b[x]].DrawElement(new Rect(disp.x, disp.y, 0, 0), e);
				if(x + 1 < gui.list.Length){
					if(v[x+1] != v[x]){
						gui.list[v[x]].EndElement();
					}
				}
				else{
					gui.list[v[x]].EndElement();
				}
			}
		}
	}
}
