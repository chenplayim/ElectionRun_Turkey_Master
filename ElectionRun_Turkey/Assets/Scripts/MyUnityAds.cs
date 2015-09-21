using System;
using UnityEngine;
using System.Collections;

#if UNITY_ANDROID

using UnityEngine.Advertisements;


public class MyUnityAds : MonoBehaviour {

	public static string UnityAdsAndroidID = "131626147";
	public static string UnityAdsIOSID = "131626148";

	public static string UnityAdResult;
//	public AdColonyCommends adColonyCommends;

	void Awake() {

		ConfigureUnityAds ();


	}

	

	public void ConfigureUnityAds()
	{

		#if UNITY_ANDROID
		if (Advertisement.isSupported) 
		{
			Advertisement.allowPrecache = true;
			Advertisement.Initialize (UnityAdsAndroidID);
			Debug.Log("Platform IS supported");
		} 
		else 
		{
			Debug.Log("Platform not supported");
			
		}
		
		#endif
		#if UNITY_IPHONE
		if (Advertisement.isSupported) 
		{
			Advertisement.allowPrecache = true;
			Advertisement.Initialize (UnityAdsIOSID);
		} 
		else 
		{
			Debug.Log("Platform not supported");
		}
		#endif


	}

	public void ShowUnityAd()
	{

		Advertisement.Show(null, new ShowOptions {
			pause = true,
			resultCallback = result => {
				UnityAdResult = result.ToString();
				Debug.Log(result.ToString());
				if(result.ToString() == "Failed")
				{
					print(result.ToString());
					UnityAdResult = "Failed";

				}
				else
				{
					AppManager.enableSound = !AppManager.enableSound;
					UnityAdResult = result.ToString();

				}

			}
		});
	 }


	void Update()
	{

		if (UnityAdResult == "Finished") {
			//adColonyCommends.OnAdFinish();
			UnityAdResult = "null";

		}
		
	}


}

#endif