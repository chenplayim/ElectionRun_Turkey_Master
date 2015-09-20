using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using GooglePlayGames;
#endif
using System;
using UnityEngine.Advertisements;


public class AdColonyCommends : MonoBehaviour {

	public string appVersion = "1.0.0";
	string AndroidappId = "app250a8c1ec1c7483fb4";
	string AndroidzoneString = "vz562be534137a41859e";


	string IOSappId = "app7f8bda159046428e82";
	string IOSzoneString = "vz7beb142e19f6434a8c";
	
	public GameObject WatchVideoBTN;

	public MainMenuController mainMenuController;
	public PlayerController playerController;

	public GameObject UpgradePanel;

	public int TimesToShowVideoinASession;
	public bool IsOnline;
	public string WasSoundWasMuted;

	#if UNITY_ANDROID
	public MyUnityAds myUnityAds;
#endif
//	public UIButton UpgradeUIButton1;
//	public UIButton UpgradeUIButton2;
//	public UIButton UpgradeUIButton3;
//	public UIButton UpgradeUIButton4;
//	public UIButton UpgradeUIButton5;
//	public UIButton UpgradeUIButton6;


	public void ActivateGUIBTNS()
	{

//		UpgradeUIButton1.enabled = true;
//		UpgradeUIButton2.enabled = true;
//		UpgradeUIButton3.enabled = true;
//		UpgradeUIButton4.enabled = true;
//		UpgradeUIButton5.enabled = true;

	}

	public void DeActivateGUIBTNS()
	{

//		UpgradeUIButton1.enabled = false;
//		UpgradeUIButton2.enabled = false;
//		UpgradeUIButton3.enabled = false;
//		UpgradeUIButton4.enabled = false;
//		UpgradeUIButton5.enabled = false;

	}



	void Start(){

		ConfigureZoneString();

//		StartCoroutine(checkInternetConnection((isConnected)=>{
//			// handle connection status here
//		}));
//		
		playerController.TimesToShowVideoinADay = PlayerPrefs.GetInt("TimesToShowVideoinADay");
		TimesToShowVideoinASession = 0;
	}

	
//	public IEnumerator checkInternetConnection(Action<bool> action){
//		
//		WWW www = new WWW("http://google.com");
//		yield return www;
//		if (www.error != null) {
//			action (false);
//			IsOnline = false;
//		} else {
//			action (true);
//			IsOnline = true;
//		}
//	} 

//	public void ConfugurUnityAdsAndAdColony()
//	{
//		ConfigureZoneString();
//		myUnityAds.ConfigureUnityAds ();
//
//		print ("AdColony.IsVideoAvailable  = " + AdColony.IsVideoAvailable());
//		print ("IsUnityAdsReady  = " + Advertisement.isReady (MyUnityAds.UnityAdsAndroidID));
//
//	}

	public void ConfigureZoneString() {

		#if UNITY_ANDROID
		AdColony.Configure(appVersion, AndroidappId, AndroidzoneString);
		#endif
		#if UNITY_IPHONE
		AdColony.Configure(appVersion, IOSappId, IOSzoneString);
		#endif

	}

	public  void TryAdColony() {

		#if UNITY_ANDROID
		if(AdColony.IsVideoAvailable(AndroidzoneString))
			{
				//DeActivateGUIBTNS();
				AdColony.ShowVideoAd(AndroidzoneString);
				AdColony.OnVideoFinished = OnVideoFinished;

			if (AudioListener.pause = false)
				{
				AudioListener.pause = true;
				WasSoundWasMuted = "no";
				}

			}
		else
			{

			myUnityAds.ShowUnityAd();

			}


		#endif

		#if UNITY_IPHONE
		if(AdColony.IsVideoAvailable(IOSzoneString))
		{


			//DeActivateGUIBTNS();
			AdColony.ShowVideoAd(IOSzoneString);
			AdColony.OnVideoFinished = OnVideoFinished;
		}
		else
		{
			//myUnityAds.ShowUnityAd();
		}
		#endif


	}


	public UpgradeScreenController UpgradeScreenController;

	public void ChackIfVideoActive()
	{
//		print ("AdColony.IsVideoAvailable  = " + AdColony.IsVideoAvailable());
//		print ("IsUnityAdsReady  = " + Advertisement.isReady (MyUnityAds.UnityAdsAndroidID));

		if ((PlayerPrefs.GetInt ("Today") == System.DateTime.Now.DayOfYear) && (playerController.TimesToShowVideoinADay > 10) || (TimesToShowVideoinASession > 5) )//||( (!AdColony.IsVideoAvailable(AndroidzoneString)) && (!VungleAndroid.isVideoAvailable()) ) ) 
		{	
			WatchVideoBTN.SetActive (false);
		} 
		else 
		{
			#if UNITY_IPHONE
			if (AdColony.IsVideoAvailable(IOSzoneString) )//|| (Advertisement.isReady(MyUnityAds.UnityAdsIOSID) == true)) 
			{
				WatchVideoBTN.SetActive (true);
			}
			else
			{
				WatchVideoBTN.SetActive (false);
				//ConfugurUnityAdsAndAdColony();

			}
			#endif

			#if UNITY_ANDROID
			if (AdColony.IsVideoAvailable(AndroidzoneString) || (Advertisement.isReady(MyUnityAds.UnityAdsAndroidID) == true)) 
			{
				WatchVideoBTN.SetActive (true);
			}
			else
			{
				WatchVideoBTN.SetActive (false);
				//ConfugurUnityAdsAndAdColony();

			}
			#endif
		}

		
	}


	private void OnVideoFinished(bool ad_was_shown)
		{
		OnAdFinish ();
		}



	public void OnAdFinish()
	{

		if ((AudioListener.pause == true) && (WasSoundWasMuted == "no"))
		{
			AudioListener.pause = false;
			WasSoundWasMuted = null;
		}

		AdColony.OnVideoFinished -= OnVideoFinished;

		ActivateGUIBTNS ();
	    TimesToShowVideoinASession ++;
		playerController.TimesToShowVideoinADay = 11;
		mainMenuController.FreeCoinsPopUp ();


		#if UNITY_ANDROID
		//VungleAndroidManager.onAdEndEvent -= OnAdFinish ;
		#endif
//		#if UNITY_IPHONE
//		VungleManager.vungleSDKwillCloseAdEvent -= OnAdFinish;
//		#endif

		if (PlayerPrefs.GetInt ("Today") != System.DateTime.Now.DayOfYear) 
		{
			PlayerPrefs.SetInt ("Today", System.DateTime.Now.DayOfYear);
		}

		ChackIfVideoActive ();

	}
	
	
}
