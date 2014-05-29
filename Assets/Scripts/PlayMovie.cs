using UnityEngine;
using System.Collections;

public class PlayMovie : MonoBehaviour {
	
	void Start() {
		
		StartCoroutine(CoroutinePlayMovie());
		
	}   
	
	protected IEnumerator CoroutinePlayMovie() { 
		
		Handheld.PlayFullScreenMovie ("http://www.idigisolutions.com/AR/video1.ogg", Color.black, FullScreenMovieControlMode.CancelOnInput);
		
		yield return new WaitForSeconds(2.0f); //Allow time for Unity to pause execution while the movie plays. 
		
		Application.LoadLevel("LoadingScene"); 
		
	}
	
}