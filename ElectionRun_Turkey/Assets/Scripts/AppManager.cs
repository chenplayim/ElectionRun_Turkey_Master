using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using GooglePlayGames;

//
//
//
public class ChallengeDetail
{
	public string ID;
	public string Name;
	public float Distance;
	public string Hat;
	public string Character;
	public string FacebookID;
}

/**
 * 
 */
public class AppManager : MonoBehaviour
{
	//
	//
	//
	public const float defaultMusicVolume = 0.06f;

	//
	//
	//
	#if UNITY_IPHONE
	public const string LEADERBOARD_NAME_IOS = "Top Runners";
	public const string LEADERBOARD_ID_IOS = "TP1234";

	//
	public const string ACHIEVEMENT_FIRST_COIN_IOS = "FC1234";
	public const string ACHIEVEMENT_50_COIN_IOS = "50C";
	public const string ACHIEVEMENT_BOOM_YOURE_DOWN_IOS = "BYD1234";
	public const string ACHIEVEMENT_RUNNING_LIKE_THE_WIND_IOS = "RLTW1234";
	public const string ACHIEVEMENT_WELCOME_BACK_IOS = "WB1234";	

	public const int RUNNING_LIKE_THE_WIND_DISTANCE_IOS = 500;

	#endif


	#if UNITY_ANDROID
	public const string LEADERBOARD_NAME = "Top Daily Runners";
	public const string LEADERBOARD_ID = "CgkI8f6Lxd8eEAIQAA";

	//
	//
	//
	public const string ACHIEVEMENT_FIRST_COIN = "CgkI8f6Lxd8eEAIQAg";
	public const string ACHIEVEMENT_50_COIN = "CgkIhvOU7qIWEAIQDg";
	public const string ACHIEVEMENT_BOOM_YOURE_DOWN = "CgkIhvOU7qIWEAIQDw";
	//public const string ACHIEVEMENT_RUNNING_LIKE_THE_WIND = "CgkI8f6Lxd8eEAIQBQ";
	//public const string ACHIEVEMENT_WELCOME_BACK = "CgkI8f6Lxd8eEAIQBg";	

	public const int RUNNING_LIKE_THE_WIND_DISTANCE = 500;
	#endif
	//
	//
	//
	public Action<ChallengeDetail> challengeActivated;

	// Atlases
	public UIAtlas[] ReferenceAtlases;
	public UIAtlas[] DefaultAtlases;
	public UIAtlas[] Atlases2x;

	// Fonts
	public UIFont[] ReferenceFonts;
	public UIFont[] DefaultFonts;
	public UIFont[] Fonts2x;

	//
	//
	//
	public GameObject ChallengerEntryPrefab;

	/**
	 * 
	 */
	private const string BANNER_AD_ID = "ca-app-pub-7014416070715049/7220382392";
	private const string INTERSTITIAL_AD_ID = "ca-app-pub-7014416070715049/8697115593";

	/**
	 * 
	 */
	static AppManager mInstance;
	static bool mEnableSound = true;
	bool mDeepLinking = false;
	bool mCheckingChallenges = false;
	string mChallengerName = null;
	float mChallengerDistance = float.NaN;
	//string mChallengerHat = null;
	string mChallengerFBID = null;
	bool mPause = false;

	//
	//
	//
	List<ChallengeDetail> mChallenges;
	int mCurrentChallengeDetail = -1;
	UIGrid mChallengerGrid;
	List<GameObject> mChallengerEntries;



	public static AppManager instance {
		get {
			return mInstance;
		}
	}

	public static bool enableSound
	{
		set {
			mEnableSound = value;
			AudioListener.pause = !value;
			if(AudioListener.pause)Analytics.gua.sendEventHit("Action", "Mute");
		}

		get {
			return mEnableSound;
		}
	}

	//
	//
	//
	void OnFBInitialized()
	{
		FacebookManager.initialized -= OnFBInitialized;

		#if !UNITY_WEBPLAYER && !UNITY_EDITOR

		if (FacebookManager.instance.isLoggedIn) RetrieveFBRequests();
		else FacebookManager.instance.loginResult += OnFBLoginResult;

		#endif
	}

	//
	//
	//
	void OnFBLoginResult(bool loggedIn, string error)
	{
		#if !UNITY_WEBPLAYER && !UNITY_EDITOR

		if (loggedIn) {
			//CheckFBRequest();
			RetrieveFBRequests();
		}

		#endif
	}

	//
	//
	//
//	void OnApplicationFocus(bool focused)
//	{
//		Debug.Log("focus");
//
//		if (!mEnableSound)
//		{
//			AudioListener.pause = !mEnableSound;
//			StartCoroutine( MuteAudio() );
//		}
//		else
//		{
//			AudioListener.pause = false;
//		}
//
//		if (!focused)
//		{
//			Time.timeScale = 0;
//		}
//		else
//		{
//			Time.timeScale = 1;
//		}
//	}

	//
	//
	//
		
	void OnApplicationPause(bool paused)
	{
		//Debug.Log("OnApplicationPause");

		if (!mEnableSound)
		{
			AudioListener.pause = !mEnableSound;
			StartCoroutine( MuteAudio() );
		}

		if (!paused)
		{
			//if (!mCheckingChallenges) StartCoroutine(CheckFBRequest());
			//if (!mCheckingChallenges) CheckFBRequest();
			if (FacebookManager.instance.isInitialized && FacebookManager.instance.isLoggedIn) RetrieveFBRequests();
		}
	}

	void CheckFBRequest()
	{
		mCheckingChallenges = true;
		//yield return new WaitForFixedUpdate();

		Debug.Log("Check deep link.");
		
		if (!mDeepLinking && FacebookManager.instance.isInitialized && FacebookManager.instance.isLoggedIn)
		{
			mDeepLinking = true;
			FacebookManager.instance.GetDeepLink(OnDeepLinkResult);
		}
		else
		{
			Debug.Log("Facebook initialized: " + FacebookManager.instance.isInitialized);
			Debug.Log("Facebook logged in: " + FacebookManager.instance.isLoggedIn);
		}
		
		Debug.Log("Check deep link - End.");

		mCheckingChallenges = false;
	}

	void RetrieveFBRequests()
	{
		FB.API("me/apprequests", Facebook.HttpMethod.GET, OnRetrieveFBRequestsResult);
	}
	

	void OnRetrieveFBRequestsResult(FBResult result)
	{
		if (result.Error == null && result.Text != null && result.Text.Length > 0)
		{
			Debug.Log("Retrieve FB requests result: " + result.Text);

			Dictionary<string, object> responseObject = Facebook.MiniJSON.Json.Deserialize(result.Text) as Dictionary<string, object>;
			List<object> requests = responseObject["data"] as List<object>;

			if (requests.Count > 0) {

				Dictionary<string, object> request;
				Dictionary<string, object> from;

				// Challenge grid
				Transform uiRoot = GameObject.Find("UI Root").transform;
				if (mChallengerGrid == null) {
					mChallengerGrid = uiRoot.Find("ChallengeList/PlayerScrollView/ChallengerGrid").GetComponent<UIGrid>();
				}

				// Challenge list
				if(mChallenges != null) mChallenges.Clear();
				else mChallenges = new List<ChallengeDetail>();

				Debug.Log ("Retrieve FB requests count: " + requests.Count);
				mCurrentChallengeDetail = 0;

				for(int i=0; i<requests.Count; ++i)
				{
					request = requests[i] as Dictionary<string, object>;

					Debug.Log("Retrieve FB requests " + i.ToString() + ": " + requests[i]);

					from = request["from"] as Dictionary<string, object>;
					
					// Name
					ChallengeDetail challenge = new ChallengeDetail();
					challenge.ID = request["id"].ToString();
					challenge.Name = from["name"].ToString();
					challenge.FacebookID = from["id"].ToString();

					
					// Distance and hat
					string[] parameters = request["data"].ToString().Split(","[0]);
					challenge.Distance = float.Parse(parameters[0]);
					//challenge.Hat = parameters[1];
					challenge.Character = parameters[1];




					mChallenges.Add(challenge);

					Debug.Log(challenge.Name + "," + challenge.Distance + "," + challenge.Hat + "," + challenge.FacebookID);
					
					AddChallengeEntry(challenge);
					++mCurrentChallengeDetail;
				}

				UpdateChallengeExclamationMark();
			}
		}
		else
		{
			if (result.Error != null) Debug.LogError("Retrieve FB Requests Error: " + result.Error);
		}
	}

	//
	//
	//
	IEnumerator MuteAudio()
	{
		yield return new WaitForFixedUpdate();
		AudioListener.pause = !mEnableSound;
		Debug.Log("mute");
	}

	//
	//
	//
	void OnDeepLinkResult(FBResult result)
	{
		if (result.Error == null && result.Text != null && result.Text.Length > 0)
		{
			Debug.Log("Deep Link Query: " + System.Uri.UnescapeDataString((new System.Uri(result.Text)).Query));
			
			string text = System.Uri.UnescapeDataString((new System.Uri(result.Text)).Query);
			string[] strArr = text.Split("&"[0]);
			int i;
			for(i=0; i<strArr.Length; ++i)
			{
				text = strArr[i];
				
				if (text.IndexOf("request_ids") != -1)
				{
					/*text = text.Split("="[0])[1];
					text = text.Split(","[0])[0];
					Debug.Log("Request id: " + text);
					FB.API("/" + text, Facebook.HttpMethod.GET, OnAppRequestDetails);*/

					text = text.Split("="[0])[1];
					strArr = text.Split(","[0]);
					CreateChallengeList(strArr);
					break;
				}
			}
		}
		else
		{
			if (result.Error != null) Debug.Log("deep link error: " + result.Error);
			else Debug.Log("no deep link.");
		}

		mDeepLinking = false;
	}

	//
	//
	//
	void OnAppRequestDetails(FBResult result)
	{
		if (result.Error == null || result.Error.Length == 0)
		{
			Dictionary<string, object> responseObject = Facebook.MiniJSON.Json.Deserialize(result.Text) as Dictionary<string, object>;
			Debug.Log("app request data: " + responseObject["data"]);

			Dictionary<string, object> from = responseObject["from"] as Dictionary<string, object>;
			Debug.Log("from: " + from["name"]);

			// Confirm challenge start
			GameObject go = transform.FindChild("UI Root/ConfirmationDialog").gameObject;
			DialogController dlgControl = go.GetComponent<DialogController>();
			dlgControl.Closed += OnConfirmChallengeStart;
			mChallengerName = from["name"].ToString();
			mChallengerFBID = from["id"].ToString();

			// Distance and hat
			string[] parameters = responseObject["data"].ToString().Split(","[0]);
			mChallengerDistance = float.Parse(parameters[0]);
		//	mChallengerHat = parameters[1];


			dlgControl.SetMessage("Try to beat " + mChallengerName + "'s distance: " + mChallengerDistance.ToString("0.00") + " meters?");
			go.SetActive(true);

			if (Time.timeScale > 0) {
				Time.timeScale = 0;
				mPause = true;
			}
			
			// Delete request
			FB.API("/" + responseObject["id"], Facebook.HttpMethod.DELETE);
		}
		else
		{
			Debug.Log("app request detail error: " + result.Text);
		}
	}

	//
	//
	//
	void CreateChallengeList(string[] requestIDs)
	{
		if(mChallenges != null) mChallenges.Clear();
		else mChallenges = new List<ChallengeDetail>();

		ChallengeDetail challenge;
		for(int i=0; i<requestIDs.Length; ++i)
		{
			challenge = new ChallengeDetail();
			challenge.ID = requestIDs[i];
			mChallenges.Add (challenge);
		}

		if(requestIDs.Length > 0)
		{
			mCurrentChallengeDetail = 0;
			FB.API("/" + requestIDs[0], Facebook.HttpMethod.GET, OnFBRequestDetailsResult);
		}

		// Remove unused challenger entries
		if (mChallengerEntries != null) {
			while (mChallengerEntries.Count > mChallenges.Count) {
				GameObject.Destroy(mChallengerEntries[mChallengerEntries.Count - 1]);
				mChallengerEntries.RemoveAt(mChallengerEntries.Count - 1);
			}
		}
	}

	//
	//
	//
	void OnFBRequestDetailsResult(FBResult result)
	{
		if (result.Error == null)
		{
			if (mChallenges != null && mChallenges.Count > 0 &&
			    mCurrentChallengeDetail >= 0 && mCurrentChallengeDetail < mChallenges.Count)
			{
				Dictionary<string, object> responseObject = Facebook.MiniJSON.Json.Deserialize(result.Text) as Dictionary<string, object>;
				Dictionary<string, object> from = responseObject["from"] as Dictionary<string, object>;

				// Name
				ChallengeDetail challenge = mChallenges[mCurrentChallengeDetail];
				challenge.Name = from["name"].ToString();
				challenge.FacebookID = from["id"].ToString();
				
				// Distance and hat
				string[] parameters = responseObject["data"].ToString().Split(","[0]);
				challenge.Distance = float.Parse(parameters[0]);
				//challenge.Hat = parameters[1];
				challenge.Character = parameters[1];


				// Delete request
				//FB.API("/" + responseObject["id"], Facebook.HttpMethod.DELETE);

				if (mChallengerGrid == null)
				{
					Transform uiRoot = GameObject.Find("UI Root").transform;
					mChallengerGrid = uiRoot.Find("ChallengeList/PlayerScrollView/ChallengerGrid").GetComponent<UIGrid>();
				}

				AddChallengeEntry(challenge);
				++mCurrentChallengeDetail;
			}
		}
		else
		{
			Debug.Log("FB request detail error: " + result.Text);

			// Error reading challenge, remove it from list
			mChallenges.RemoveAt(mCurrentChallengeDetail);

			if (mChallengerEntries != null) {
				if (mChallengerEntries.Count > mChallenges.Count) {
					GameObject.Destroy(mChallengerEntries[mChallengerEntries.Count - 1]);
					mChallengerEntries.RemoveAt(mChallengerEntries.Count);
				}
			}
		}

		if (mCurrentChallengeDetail < mChallenges.Count)
		{
			Debug.Log("Get next request detail: " + mChallenges[mCurrentChallengeDetail].ID);

			// Get next challenge detail
			FB.API("/" + mChallenges[mCurrentChallengeDetail].ID, Facebook.HttpMethod.GET, OnFBRequestDetailsResult);
		}
		else
		{
			Debug.Log ("Got all request details: " + mChallenges.Count);

			// Got all the challenge details
			//if(mChallenges.Count > 0) ShowChallengeList();
			mDeepLinking = false;

			UpdateChallengeExclamationMark();
		}
	}

	//
	//
	//
	public void UpdateChallengeExclamationMark()
	{
		if (FacebookManager.instance.isInitialized) OnFBInitialized();
		else FacebookManager.initialized += OnFBInitialized;

		// Show/hide exclamation mark on challenge button
		GameObject uiRoot = GameObject.Find("UI Root");
		Transform exclamation = uiRoot.transform.FindChild("MainMenu/RightPanel/Grid/FacebookButton/Exclamation");
		exclamation.gameObject.SetActive(mChallengerEntries != null && mChallengerEntries.Count > 0);

		Transform exclamation2 = uiRoot.transform.FindChild("game_end_screen/Grid/FacebookChallenges/Exclamation");
		exclamation2.gameObject.SetActive(mChallengerEntries != null && mChallengerEntries.Count > 0);
	}

	//
	//
	//
	public void ShowChallengeList()
	{
		Time.timeScale = 0;
		Transform challengeList = LevelGenerator.uiRoot.transform.FindChild("ChallengeList");
		challengeList.gameObject.SetActive(true);
		challengeList.FindChild("ExitButton").GetComponent<UIButton>().onClick.Add(new EventDelegate(OnCloseChallengeList));

		// 'No challenges' label
		Transform noChallengesLbl = challengeList.FindChild("PlayerScrollView/NoChallenges");
		noChallengesLbl.gameObject.SetActive(mChallengerEntries == null || mChallengerEntries.Count == 0);

		Analytics.gua.sendAppScreenHit ("Challenge Inbox");
	}

	//
	//
	//
	void OnCloseChallengeList()
	{
		Transform challengeList = LevelGenerator.uiRoot.transform.FindChild("ChallengeList");
		challengeList.gameObject.SetActive(false);
		Time.timeScale = 1;
	}

	//
	//
	//
	IEnumerator OnSimulateFBRequestDetailsResult(string name, float distance, string character)
	{
		if(name != null)
		{
			if (mChallenges != null && mChallenges.Count > 0 &&
			    mCurrentChallengeDetail >= 0 && mCurrentChallengeDetail < mChallenges.Count)
			{
				// Name
				ChallengeDetail challenge = mChallenges[mCurrentChallengeDetail];
				challenge.Name = name.ToString();
				challenge.FacebookID = null;
				challenge.Distance = distance;
				//challenge.Hat = hat;
				challenge.Character = character;

				if (mChallengerGrid == null)
				{
					Transform uiRoot = GameObject.Find("UI Root").transform;
					mChallengerGrid = uiRoot.Find("ChallengeList/PlayerScrollView/ChallengerGrid").GetComponent<UIGrid>();
				}

				AddChallengeEntry(challenge);
				
				++mCurrentChallengeDetail;
				if (mCurrentChallengeDetail < mChallenges.Count)
				{
					yield return new WaitForSeconds(1);
					StartCoroutine( OnSimulateFBRequestDetailsResult(((int)(UnityEngine.Random.value * 99999)).ToString(),
					                                                 20 + UnityEngine.Random.value * 30,  "ZipiBuzi") );
				}
				else
				{
					// Got all the challenge details
					ShowChallengeList();
				}
			}
		}
		else
		{
			Debug.Log("FB request detail error: simulate error.");
		}
	}

	//
	//
	//
	void AddChallengeEntry(ChallengeDetail detail)
	{
		// Get challenger entry
		GameObject challengeEntry;
		if (mChallengerEntries == null) mChallengerEntries = new List<GameObject>();

		if (mChallengerEntries.Count <= mCurrentChallengeDetail) {
			// Create new challenge entry
			challengeEntry = Instantiate(ChallengerEntryPrefab) as GameObject;
			challengeEntry.transform.parent = mChallengerGrid.transform;
			challengeEntry.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnSelectChallenge));
			mChallengerEntries.Add(challengeEntry);
		}
		else {
			// Reuse challenge entry
			challengeEntry = mChallengerEntries[mCurrentChallengeDetail];
		}
		
		// Set challenge details
		challengeEntry.name = detail.ID;
		Transform transf = challengeEntry.transform;
		transf.FindChild("Name").GetComponent<UILabel>().text = detail.Name;
		transf.FindChild("Distance").GetComponent<UILabel>().text = detail.Distance.ToString("0.00") + " m";


//		//My Change
//		transf.FindChild ("CharecterLabel").GetComponent<UILabel> ().text = detail.Name


		transf.localScale = Vector3.one;
		transf.localPosition = Vector3.zero;
		mChallengerGrid.GetComponent<UIGrid>().Reposition();

		if (mChallengerEntries.Count == 1)
		{
			// Hide "no challenges" label
			Transform uiRoot = GameObject.Find("UI Root").transform;
			Transform noChallengesLbl = uiRoot.transform.FindChild("ChallengeList/PlayerScrollView/NoChallenges");
			noChallengesLbl.gameObject.SetActive(false);
		}
	}

	//
	//
	//
	void SimulateChallenges(string[] requestIDs)
	{
		if(mChallenges != null) mChallenges.Clear();
		else mChallenges = new List<ChallengeDetail>();
		
		ChallengeDetail challenge;
		for(int i=0; i<requestIDs.Length; ++i)
		{
			challenge = new ChallengeDetail();
			challenge.ID = requestIDs[i];
			mChallenges.Add (challenge);
		}
		
		if(requestIDs.Length > 0)
		{
			mCurrentChallengeDetail = 0;
			StartCoroutine( OnSimulateFBRequestDetailsResult("fuckwit", 33, "ZipiBuzi") );
		}
		
		// Remove unused challenger entries
		if (mChallengerEntries != null)
		{
			while (mChallengerEntries.Count > mChallenges.Count)
			{
				GameObject.Destroy(mChallengerEntries[mChallengerEntries.Count - 1]);
				mChallengerEntries.RemoveAt(mChallengerEntries.Count - 1);
			}
		}
	}

	//
	//
	//
	void OnSelectChallenge()
	{
		Debug.Log("OnSelectChallenge Begin");

		ChallengeDetail challenge = FindChallengeByID(UIButton.current.name);
		Debug.Log("AppManager Challenge: " + challenge.Name + "," + challenge.Hat + "," + challenge.Character);

		if(challengeActivated != null) challengeActivated(challenge);

		Transform uiRoot = GameObject.Find("UI Root").transform;
		uiRoot.transform.FindChild("ChallengeList").gameObject.SetActive(false);
		Time.timeScale = 1;

		// Delete challenge
		FB.API("/" + challenge.ID, Facebook.HttpMethod.DELETE);
		mChallenges.Remove(challenge);
		DeleteFromChallengeList(challenge.ID);

		// Show/hide exclamation mark
		Transform exclamation = uiRoot.transform.FindChild("MainMenu/Grid/ChallengeButton/Exclamation");
		exclamation.gameObject.SetActive(mChallengerEntries.Count > 0);

		// Hide "no challenges" label
		Transform noChallengesLbl = uiRoot.transform.FindChild("ChallengeList/PlayerScrollView/NoChallenges");
		noChallengesLbl.gameObject.SetActive(false);

		Debug.Log("OnSelectChallenge End");
		Analytics.gua.sendEventHit("Action", "Challenge accepted");
	}

	//
	//
	//
	ChallengeDetail FindChallengeByID(string id)
	{
		foreach (ChallengeDetail c in mChallenges)
		{
			if (c.ID == id) return c;
		}

		return null;
	}

	//
	//
	//
	void DeleteFromChallengeList(string id)
	{
		for(int i=0; i<mChallengerEntries.Count; ++i)
		{
			if (mChallengerEntries[i].name == id)
			{
				mChallengerGrid.repositionNow = true;
				GameObject.Destroy(mChallengerEntries[i]);
				mChallengerEntries.RemoveAt(i);
				break;
			}
		}
	}

	//
	//
	//
	void OnConfirmChallengeStart(bool ok)
	{
		DialogController.instance.Closed -= OnConfirmChallengeStart;

		if (ok) {
			Debug.Log("Challenge start: " + mChallengerName + ", " + mChallengerDistance);
			//challengeActivated(mChallengerName, mChallengerDistance, mChallengerHat, mChallengerFBID);
			mChallengerName = null;
			mChallengerFBID = null;
			mChallengerDistance = float.NaN;
			//mChallengerHat = null;
		}

		if (mPause) {
			Time.timeScale = 1;
			mPause = false;
		}
	}

	/**
	 * 
	 */
	void Awake()
	{
		mInstance = this;

		// Swap atlases
		float width = Screen.width > Screen.height ? Screen.width : Screen.height;

		// Initialize facebook
		if (FacebookManager.instance.isInitialized) OnFBInitialized();
		else FacebookManager.initialized += OnFBInitialized;

		//StoreController.LoadHats();
	
		Time.timeScale = 1;

	}

	public void ConnectToGooglePlay()
	{

		#if (UNITY_ANDROID)
		
		// Connect google play
		// recommended for debugging:
		#if DEBUG
		PlayGamesPlatform.DebugLogEnabled = true;
		#endif
		
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate();
		
		// authenticate user:
		Social.localUser.Authenticate(OnAuthenticateGooglePlayResult);
		
		#endif

	}



	public GameOverGUIController gameOverGUIController;
	public AddedFuncionallity addedFuncionallity;

	#if (UNITY_ANDROID)
	void OnAuthenticateGooglePlayResult(bool success)
	{
		if (Social.localUser.authenticated)
		{
			//if(PlayerPrefs.GetInt("Saved", 0) == 1) Social.ReportProgress(AppManager.ACHIEVEMENT_WELCOME_BACK, 100, null);
			//Social.ShowAchievementsUI();
		}

		// handle success or failure
		Debug.Log("Authenticate result: " + success);
	}
	#endif

	#if UNITY_IPHONE
	public void OnAuthenticateGameCenterResult(bool success)
	{
		if (Social.localUser.authenticated)
		{
			//if(PlayerPrefs.GetInt("Saved", 0) == 1) Social.ReportProgress(AppManager.ACHIEVEMENT_WELCOME_BACK_IOS, 100, null);
		}

	}
	#endif


}
