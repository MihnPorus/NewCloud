using UnityEngine;
using System.Collections;

public class Picsample : MonoBehaviour {

	public string url = "http://www.idigisolutions.com/AR/pictest.jpg";
	IEnumerator Start() {
		WWW www = new WWW(url);
		yield return www;
		renderer.material.mainTexture = www.texture;
	}
}
