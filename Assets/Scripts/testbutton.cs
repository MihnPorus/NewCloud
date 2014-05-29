using UnityEngine;
using System.Collections;

public class testbutton : MonoBehaviour {

	// Use this for initialization
	public class Guis : MonoBehaviour {
		
		void OnGUI() {
			
			GUI.Button(new Rect(Screen.width - Screen.width/2,Screen.height - Screen.height/2,100, 50), "MyButton Text");
			
		}
		
	}

}
