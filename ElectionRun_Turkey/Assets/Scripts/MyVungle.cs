using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
//using GooglePlayGames;
//using GoogleMobileAds.Android;
#endif

public class MyVungle : MonoBehaviour {

	public AdColonyCommends adColonyCommends;
	public GameObject UpgradePanel;
	


	//string VungelIDAndroid = "54cf851539ae6d516000019b";
	//string VungelIDIOS = "54cf8b3999ec68887f0000a3";


	void Start () {

		#if UNITY_ANDROID
		//VungleAndroid.init(VungelIDAndroid);
		#endif

		#if UNITY_IPHONE
		//VungleBinding.startWithAppId(VungelIDIOS);
		#endif
	}

	public void IsVundelAvaileble()
	{
		#if UNITY_ANDROID
//			if (VungleAndroid.isVideoAvailable())
//			{
//				adColonyCommends.DeActivateGUIBTNS();
//				VungleAndroid.playAd();
//				VungleAndroidManager.onAdEndEvent += adColonyCommends.OnAdFinish ;
//
//			}
//			if(!VungleAndroid.isVideoAvailable())
//			{
				adColonyCommends.TryAdColony();
//			}
		#endif

		#if UNITY_IPHONE

		//!VungleAndroid.isVideoAvailable()

//			if (VungleBinding.isAdAvailable())
//			{
//				adColonyCommends.DeActivateGUIBTNS();
//				VungleBinding.playAd();
//				//VungleManager.vungleSDKwillCloseAdEvent += adColonyCommends.OnVungleVideoFinished(true, 2 , false , false);
//				//Debug.Log 
//			}
//			if(!VungleBinding.isAdAvailable())
//			{
				adColonyCommends.TryAdColony();			
			//}
		#endif

	}

}
