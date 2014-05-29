using UnityEngine;
using System.Collections;

public class streaming : MonoBehaviour {
	
	private MovieTexture  m;
	private string playButtonString="play",url= "http://www.idigisolutions.com/ar/video1.ogg";
	private string [] mediaSource = {"1.ogv","2.ogv","3.ogv","4.ogv","5.ogv"};
	public AudioSource AS;
	bool isMenu = true;
	int tmp;
	private WWW www;
	
	// Use this for initialization
	
	void Start () {
		
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape))
			
		{
			//~ Screen.showCursor = true;
			Screen.fullScreen = false;
			isMenu = true;
		}
		if(Screen.fullScreen == true) {
			if(Input.mousePosition.x >=20 && Input.mousePosition.x<=(Screen.width -40) && Input.mousePosition.y>=0 && Input.mousePosition.y <= 80)
				isMenu = true;
			else
				isMenu = false;
		}
		else
			isMenu = true;
	}
	void OnGUI() {
		if(m!=null)
			GUI.DrawTexture (new Rect (0,0,Screen.width,Screen.height),m,ScaleMode.StretchToFill);
		else
			GUI.Label(new Rect(50,100,200,50), "Video is yet to Start");
		if(isMenu) {
			GUI.Box(new Rect(20, Screen.height - 60, Screen.width-40,60),"");
			for ( int i=0;i < mediaSource.Length;i++) {
				if(m!=null)
					if (i == tmp && m.isPlaying) 
						playButtonString="pause"; //it's playing so the button should pause.
				else
					playButtonString="play"; //it's not playing and the button should play the movie.
				if (GUI.Button (new Rect (30 + (200 * i),Screen.height-40,100,30),playButtonString + i) == true)
				{
					if(m!=null)
					if (m.isPlaying == true) {
						m.Pause();
						AS.Pause();
						if (i == tmp)
							return;
					}
					else if(m.isReadyToPlay && i == tmp)
					{
						m.Play();
						AS.Play();
						return;
					}
					www = new WWW(url+mediaSource[i]);
					m = www.movie;
					AS.clip = m.audioClip;
					while(!m.isReadyToPlay) {
						SomeCoroutine();
						if (!m.isPlaying) {
							m.Play();
							if(!AS.isPlaying)
								AS.Play();
						}
					}
					Screen.fullScreen = true;
					isMenu = false;
					tmp = i;
				}
				if (GUI.Button (new Rect(130 + (200 *i),Screen.height-40,100,30),"Stop"+ i)==true && i==tmp) {
					m.Stop();
					AS.Stop();
				}
			}
		}
		else {
			GUI.Label( new Rect(Screen.width - 200, 10,200,20), "Press Esc for Menu or");
			if(GUI.Button( new Rect(Screen.width - 200, 30,100,20), "Click Here")) {
				isMenu = true;
				Screen.fullScreen = false;
			}
		}
	}
	IEnumerator SomeCoroutine () {
		// Wait for one frame
		yield return www;
	}
}

