using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;


public class MyGameCenterScript : MonoBehaviour {


	public GameObject MainMenu;
	public AppManager appManager;


	public void ConnectToGameCenter () {
		// Authenticate and register a ProcessAuthentication callback
		// This call needs to be made before we can proceed to other calls in the Social API
		#if UNITY_IPHONE
		Social.localUser.Authenticate(ProcessAuthentication);
		GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);

		Social.localUser.Authenticate(appManager.OnAuthenticateGameCenterResult);
		# endif
	}

	// This function gets called when Authenticate completes
	// Note that if the operation is successful, Social.localUser will contain data from the server. 
	void ProcessAuthentication (bool success) {
		if (success) {
			Debug.Log ("Authenticated, checking achievements");
			
			// Request loaded achievements, and register a callback for processing them
			Social.LoadAchievements (ProcessLoadedAchievements);
		}
		else
			Debug.Log ("Failed to authenticate");
	}



	// This function gets called when the LoadAchievement call completes
	void ProcessLoadedAchievements (IAchievement[] achievements) {
		if (achievements.Length == 0)
			Debug.Log ("Error: no achievements found");
		else
			Debug.Log ("Got " + achievements.Length + " achievements");
		
		// You can also call into the functions like this
//		Social.ReportProgress ("Achievement01", 100.0, result => {
//			if (result)
//				Debug.Log ("Successfully reported achievement progress");
//			else
//				Debug.Log ("Failed to report achievement");
//		});
	}


	public void ShowGameCenterLeadBoard()
	{

		Social.ShowLeaderboardUI();
	}


	public void ShowGameCenterAchivments()
	{

		Social.ShowAchievementsUI();
	}










}
