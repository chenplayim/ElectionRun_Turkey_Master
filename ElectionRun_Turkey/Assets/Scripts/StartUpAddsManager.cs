using UnityEngine;
using System.Collections;
//using StartApp;

public class StartUpAddsManager : MonoBehaviour {


//	void Start () {
//
//		if (PlayerPrefs.GetInt ("sessionNumber") == 0) {
//			PlayerPrefs.SetInt ("sessionNumber", 1);
//		}
//		else
//		{
//			PlayerPrefs.SetInt ("sessionNumber", 2);
//		}
//	//	print (PlayerPrefs.GetInt ("sessionNumber"));
//		#if UNITY_ANDROID
//		StartAppWrapper.loadAd();
//		#endif
//	}

	public void ShowStartUpInterstisial()
	{
		#if UNITY_ANDROID
//		if (PlayerPrefs.GetInt ("sessionNumber") == 2) 
//		{
//		StartAppWrapper.showAd ();
//		StartAppWrapper.loadAd ();
//		}
		#endif

	}
}
