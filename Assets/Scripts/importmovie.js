var url = "http://www.idigisolutions.com/AR/video1.ogg";
function OnMouseDown () {
 // Start download
 var www = new WWW(url);
// Make sure the movie is ready to start before we start playing
 var movieTexture = www.movie;
 while (!movieTexture.isReadyToPlay)
 yield;
// Initialize texture to be 1:1 resolution
 renderer.material.mainTexture = movieTexture;
// Assign clip to audio source
 // Sync playback with audio
 audio.clip = movieTexture.audioClip;
// Play both movie & sound
 movieTexture.Play();
 audio.Play();
}
// Make sure we have audio source
@script RequireComponent (AudioSource)
function Update () {
}