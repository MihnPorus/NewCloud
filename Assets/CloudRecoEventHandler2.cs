/*==============================================================================
Copyright (c) 2012-2013 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
==============================================================================*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results as well as error messages
/// The current state is visualized and new results are enabled using the TargetFinder API.
/// </summary>
public class CloudRecoEventHandler2 : MonoBehaviour, ICloudRecoEventHandler
{
	#region PRIVATE_MEMBER_VARIABLES
	
	// ImageTracker reference to avoid lookups
	private ImageTracker mImageTracker;
	private ContentManager mContentManager;

	public string BundleURL;
	public string AssetName;
	public string titleb;
	public int version;

	private bool mIsJSONRequested = false;

	public string JsonServerUrl;
	private WWW mJsonBookInfo;

	private BookData mBookData;
	private BookInformationParser mBookInformationParser;
	//public importmovie vplayer;
	
	// the parent gameobject of the referenced ImageTargetTemplate - reused for all target search results
	private GameObject mParentOfImageTargetTemplate;

	#endregion // PRIVATE_MEMBER_VARIABLES
	
	
	
	#region EXPOSED_PUBLIC_VARIABLES
	
	/// <summary>
	/// can be set in the Unity inspector to reference a ImageTargetBehaviour that is used for augmentations of new cloud reco results.
	/// </summary>
	public ImageTargetBehaviour ImageTargetTemplate;
	
	#endregion
	
	#region ICloudRecoEventHandler_IMPLEMENTATION
	
	/// <summary>
	/// called when TargetFinder has been initialized successfully
	/// </summary>
	public void OnInitialized()
	{
		// get a reference to the Image Tracker, remember it
		mImageTracker = TrackerManager.Instance.GetTracker<ImageTracker>();
		mContentManager = (ContentManager)FindObjectOfType(typeof(ContentManager));
	}
	
	/// <summary>
	/// visualize initialization errors
	/// </summary>
	public void OnInitError(TargetFinder.InitState initError)
	{
		switch (initError)
		{
		case TargetFinder.InitState.INIT_ERROR_NO_NETWORK_CONNECTION:
			ErrorMsg.New("Network Unavailable", "Please check your internet connection and try again.", RestartApplication);
			break;
		case TargetFinder.InitState.INIT_ERROR_SERVICE_NOT_AVAILABLE:
			ErrorMsg.New("Service Unavailable", "Failed to initialize app because the service is not available.");
			break;
		}
	}
	
	/// <summary>
	/// visualize update errors
	/// </summary>
	public void OnUpdateError(TargetFinder.UpdateState updateError)
	{
		switch (updateError)
		{
		case TargetFinder.UpdateState.UPDATE_ERROR_AUTHORIZATION_FAILED:
			ErrorMsg.New("Authorization Error","The cloud recognition service access keys are incorrect or have expired.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_NO_NETWORK_CONNECTION:
			ErrorMsg.New("Network Unavailable","Please check your internet connection and try again.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_PROJECT_SUSPENDED:
			ErrorMsg.New("Authorization Error","The cloud recognition service has been suspended.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_REQUEST_TIMEOUT:
			ErrorMsg.New("Request Timeout","The network request has timed out, please check your internet connection and try again.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_SERVICE_NOT_AVAILABLE:
			ErrorMsg.New("Service Unavailable","The service is unavailable, please try again later.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_TIMESTAMP_OUT_OF_RANGE:
			ErrorMsg.New("Clock Sync Error","Please update the date and time and try again.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_UPDATE_SDK:
			ErrorMsg.New("Unsupported Version","The application is using an unsupported version of Vuforia.");
			break;
		}
	}
	
	/// <summary>
	/// when we start scanning, unregister Trackable from the ImageTargetTemplate, then delete all trackables
	/// </summary>
	public void OnStateChanged(bool scanning)
	{
		if (scanning)
		{
			// clear all known trackables
			mImageTracker.TargetFinder.ClearTrackables(false);
			
			// hide the ImageTargetTemplate
			mContentManager.ShowObject(false);
		}
	}
	
	/// <summary>
	/// Handles new search results
	/// </summary>
	/// <param name="targetSearchResult"></param>
	public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
	{
		GameObject vcube = GameObject.Find("Cube");

		// This code demonstrates how to reuse an ImageTargetBehaviour for new search results and modifying it according to the metadata
		// Depending on your application, it can make more sense to duplicate the ImageTargetBehaviour using Instantiate(), 
		// or to create a new ImageTargetBehaviour for each new result
		
		// Vuforia will return a new object with the right script automatically if you use
		// TargetFinder.EnableTracking(TargetSearchResult result, string gameObjectName)
		string model_name = targetSearchResult.MetaData; //check it with
		
		//Check if the metadata isn't null
		if(targetSearchResult.MetaData == null)
		{
			return;
		}

		Debug.Log("Metadata value is " + model_name ); //check it with


		LoadJSONBookData(model_name);
		if(mBookData != null )
		{
			titleb = mBookData.BookTitle;
			BundleURL = mBookData.BookDetailUrl;
			Debug.Log("Title" + titleb);
			
			//mIsBookThumbRequested = true;
		}
		switch(titleb)

		{
		case "assetbundle":
			Debug.Log("call assetbundle loading fuction");
			//assetbundleloadfunction()
			//Handheld.PlayFullScreenMovie ("www.idigisolutions.com/ar/video1.ogg", Color.black, FullScreenMovieControlMode.CancelOnInput);
			//vplayer = GetComponent<importmovie>();
			//var xRenderer = transform.Find("Cube");

			//vcube.SetActive(false);
			//cube.SetActive(false);
			StartCoroutine(DownloadAndCache());
				break;
		case "video":
			Debug.Log("call video streaming function");
			//IVideoBackgroundEventHandler()
			//Handheld.PlayFullScreenMovie ("www.idigisolutions.com/ar/video1.ogg", Color.black, FullScreenMovieControlMode.CancelOnInput);
			//GameObject vcube = GameObject.Find("Cube");
			//vcube.SetActive(true);

			break;
		}
		//string booktitle = 
		//Debug.Log(booktitle);


		// we have to check whether to go to assetbundle or to video
		/*if( int = video)
		{
			function(PlayMode video);
		}
		else{
			StartCoroutine(DownloadAndCache());
		}*/
		//StartCoroutine(DownloadAndCache());
		// First clear all trackables
		mImageTracker.TargetFinder.ClearTrackables(false);
		
		// enable the new result with the same ImageTargetBehaviour:
		ImageTargetBehaviour imageTargetBehaviour = mImageTracker.TargetFinder.EnableTracking(targetSearchResult, mParentOfImageTargetTemplate) as ImageTargetBehaviour;
		
		//if extended tracking was enabled from the menu, we need to start the extendedtracking on the newly found trackble.
		if(CloudRecognitionUIEventHandler.ExtendedTrackingIsEnabled)
		{
			imageTargetBehaviour.ImageTarget.StartExtendedTracking();
		}
	}
	
	#endregion // ICloudRecoEventHandler_IMPLEMENTATION
	
	
	
	#region UNTIY_MONOBEHAVIOUR_METHODS
	
	/// <summary>
	/// register for events at the CloudRecoBehaviour
	/// </summary>
	void Start()
	{
		mBookInformationParser = new BookInformationParser();
		//mBookInformationParser.SetBookObject(AugmentationObject);

		// look up the gameobject containing the ImageTargetTemplate:
		mParentOfImageTargetTemplate = ImageTargetTemplate.gameObject;
		
		// intialize the ErrorMsg class
		ErrorMsg.Init();
		
		// register this event handler at the cloud reco behaviour
		CloudRecoBehaviour cloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
		if (cloudRecoBehaviour)
		{
			cloudRecoBehaviour.RegisterEventHandler(this);
		}
	}

	IEnumerator DownloadAndCache (){
		
		while (!Caching.ready)
			yield return null;
		using(WWW www = WWW.LoadFromCacheOrDownload (BundleURL, version)){
			yield return www;
			if (www.error != null)
				throw new Exception("WWW download had an error:" + www.error);
			AssetBundle bundle = www.assetBundle;
			if (AssetName == "") {
				GameObject instantiated = Instantiate(bundle.mainAsset) as GameObject;
				GameObject cloudRecoObject = GameObject.Find("GameObject");
				instantiated.transform.parent = cloudRecoObject.transform;
			}
			bundle.Unload(false);        
		}
	}

	//functionforn video streaming

	private void LoadJSONBookData(string jsonBookUrl)
	{
		Debug.Log("inside json parsing ");
		// Gets the full book json url
		string fullBookURL = JsonServerUrl + jsonBookUrl;
		
		if(!mIsJSONRequested){
			
			// Gets the json book info from the url
			mJsonBookInfo = new WWW(fullBookURL);
			mIsJSONRequested = true;
			Debug.Log(fullBookURL);


		}
		
		//if(mJsonBookInfo.progress >= 1)
		//{
			if(mJsonBookInfo.error == null )
			{
				Debug.Log("json parsing started");
				// Parses the json Object
				JSONParser parser = new JSONParser();
				
				BookData bookData = parser.ParseString(mJsonBookInfo.text);
				//var str = parser.ParseString(mJsonBookInfo.text);
				//Debug.Log(str.v);
				mBookData = bookData;
				//Debug.Log(mBookData.BookTitle);

				// Updates state variables
				//mIsLoadingBookData = false;
				
				// Updates the BookData info in the augmented object
				mBookInformationParser.UpdateBookData(bookData);

				
				//mIsLoadingBookThumb = true;
				
			}else
			{
				Debug.LogError("Error downloading json");
				//mIsLoadingBookData = false;
			}
		//}
	}
	
	/// <summary>
	/// draw the sample GUI and error messages
	/// </summary>
	void OnGUI()
	{
		// draw error messages in case there were any
		ErrorMsg.Draw();
	}
	
	#endregion UNTIY_MONOBEHAVIOUR_METHODS
	
	#region PRIVATE_METHODS
	
	// callback for network-not-available error message
	private void RestartApplication()
	{
		Application.LoadLevel("Vuforia-1-About");
	}
	#endregion PRIVATE_METHODS
}
