using UnityEngine;
using System.Collections;

/**
 * 
 */
public class GameOverGUIController : MonoBehaviour {


	//
	//
	//
	const float CounterDuration = 2.0f;

	//
	//
	//
	public float MetersRun;
	public int CoinsCollected;
	public UIPlaySound PopSound;
	public AudioClip CounterSound;
	public AudioClip CoinSound;

	//
	//
	//

	//
	//
	//
	float MeterCounter;
	float CoinCounter;
	int mState = -1;
	int mEnable = 0;
	float mCounterStartTime;
	float mCounterDuration;
	float mCounterStartingPitch;
	public UILabel mMetersLabel;
	public UILabel mCoinsLabel;
	//public UILabel mCoinsPrompt;
	//public UILabel mMetersPrompt;
	public AudioSource mAudioSource;

	#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
	int mWaitForAd = 0;
	#endif

	public UIButton mRetryButton;
	public UIButton mQuitButton;
	public UIButton mUpgradeButton;
	public UIButton mChallengeButton;
	public UIButton mChallangeListButton;
	public GameObject mUpgradeNotification;

	public GameObject Starsplode_Distance;
	public GameObject Starsplode_Coin;
	//
	// Effects
	//
	float minDistance = 10.0f;
	float minCoinCollected = 10.0f;
	
	//
	//
	//
	TweenScale mMetersTweenScale;
	TweenScale mCoinsTweenScale;

	//
	//
	//

//	void Awake()
//	{
//
//		Starsplode_Distance.SetActive (false);
//		Starsplode_Coin.SetActive (false);
//	}


	public void OnClick()
	{
		FinishCounter();
	}

	//
	//
	//
	public void OnChallengeFriend()
	{
		EnableGUI(false);
		FacebookManager.instance.loginResult += OnFBLoginResult;
		FacebookManager.instance.Login();
	}

	/**
	 * 
	 */
	public void EnableGUI(bool bEnable)
	{
		if (bEnable)
		{
			++mEnable;
			if (mEnable == 0) SetEnableGUI(true);
		}
		else
		{
			--mEnable;
			if (mEnable == -1) SetEnableGUI(false);
		}

		//Debug.Log("Enable Game Over GUI lock: " + mEnable + ", " + bEnable);
	}

	/**
	 * 
	 */
//	public void ShowAd()
//	{
//		#if (UNITY_ANDROID || UNITY_IPHONE) && !HIDE_AD && !UNITY_EDITOR
//		mWaitForAd = 0;
//		EnableGUI(false);
//		#endif
//	}

	/**
	 * 
	 */
	public void StartCounter()
	{
		//Debug.Log("Start Counter - Enable GUI");
		EnableGUI(false);
		MeterCounter = 0;
		CoinCounter = 0;
		mState = 0;
		mMetersLabel.enabled = true;
		mCoinsLabel.enabled = false;
		mCoinsLabel.text = "";
	//	mCoinsPrompt.enabled = false;
		StartCounter(MetersRun, CounterSound);
		
		// "You run"
	//	TweenScale tween = TweenScale.Begin(mMetersPrompt.gameObject, 0.2f, new Vector3(1.3f, 1.3f, 1));
	//	tween.method = UITweener.Method.BounceIn;
	//	EventDelegate.Add(tween.onFinished, OnScaleupTweenFinished);
		
		// Meters scale animation
		if (mMetersTweenScale == null) {
			mMetersTweenScale = TweenScale.Begin(mMetersLabel.gameObject, 0.15f, new Vector3(1.2f, 1.2f, 1));
			mMetersTweenScale.method = UITweener.Method.EaseOut;
			mMetersTweenScale.style = UITweener.Style.PingPong;
		}
		else {
			mMetersTweenScale.PlayForward ();
		}
	}

	/**
	 * 
	 */
	void SetEnableGUI(bool bEnable)
	{
		mRetryButton.isEnabled = bEnable;
		mQuitButton.isEnabled = bEnable;
		mUpgradeButton.isEnabled = bEnable;
		mChallengeButton.isEnabled = bEnable;
		mChallangeListButton.isEnabled = bEnable;
	}

	/**
	 * 
	 */
	//my change
	public AddedFuncionallity addedFuncionallity;
	public GameObject GameOverGUI;
	public bool CanShowPopUps;
	public bool DidChallengeAFriend;
	public bool DidChackedUpgrades;

	public UILabel XPStatusLabel;

	void OnEnable()
	{

		ChallangeAFriendAnimator.enabled = false;
		transform.Find ("Starsplode_Distance").gameObject.SetActive (false);
		transform.Find("Starsplode_Coin").gameObject.SetActive(false);

		Starsplode_Distance.SetActive (false);
		Starsplode_Coin.SetActive (false);

		AppManager.instance.UpdateChallengeExclamationMark ();
		// Lower music volume with tween
		AudioSource audioSource = AppManager.instance.GetComponent<AudioSource>();
		TweenVolume tween = AppManager.instance.GetComponent<TweenVolume>();
		tween.duration = 2;
		tween.from = audioSource.volume;
		tween.to = AppManager.defaultMusicVolume * 0.5f;
		tween.PlayForward();


		string XPStatus = PlayerPrefs.GetString ("PlayersXPStatus");
		XPStatusLabel.text = XPStatus;


		//my change Web
		#if UNITY_WEBPLAYER
		transform.FindChild("Grid/ChallengeButton").gameObject.SetActive(false);
		#endif

//		GameObject[] Ghosts = GameObject.FindGameObjectsWithTag ("ghost");
//
//		for (int i = 0; i < Ghosts.Length; i++) {
//
//			Destroy(Ghosts[i]);
//
//		}
	}

	

//	public StoreController storeController;
//
//	void OnConfirmOpenChallenge(bool ok)
//	{
//		DialogController.instance.Closed -= OnConfirmOpenChallenge;
//		
//		if (ok)
//			EventDelegate.Execute(mChallengeButton.onClick);
//			PlayerPrefs.SetInt("DialogPopUp" , 2);
//	}
//
//	void OnConfirmUpgrade(bool ok)
//	{
//		DialogController.instance.Closed -= OnConfirmUpgrade;
//
//		if (ok)
//			EventDelegate.Execute(mUpgradeButton.onClick);
//			PlayerPrefs.SetInt("DialogPopUp" , 3);
//	}
//
//	void OnConfirmOHat(bool ok)
//	{
//		DialogController.instance.Closed -= OnConfirmOHat;
//		
//		if (ok)
//			storeController.OnOpenHatStore ();
//			PlayerPrefs.SetInt("DialogPopUp" , 4);
//	}


	//
	//
	//
	void OnDisable()
	{
		if (AppManager.instance != null) {
			// Increase music volume with tween
			AudioSource audioSource = AppManager.instance.GetComponent<AudioSource>();
			TweenVolume tween = AppManager.instance.GetComponent<TweenVolume>();
			tween.duration = 2;
			tween.from = audioSource.volume;
			tween.to = AppManager.defaultMusicVolume;
			tween.PlayForward();
		}

		mEnable = 0;
		SetEnableGUI(true);
	}

	//
	//
	//
	void StartCounter(float counterTotal, AudioClip clip, float maxPitch = 1.5f)
	{
		mCounterStartTime = Time.time;
		mCounterDuration = counterTotal / CounterDuration < 4 ? 4 / counterTotal : CounterDuration;

		// Counter sound
		mAudioSource.clip = clip;
		mAudioSource.loop = true;
		mCounterStartingPitch = Mathf.Max(1f, CounterDuration / mCounterDuration);
		if (mCounterStartingPitch > maxPitch) mCounterStartingPitch = maxPitch;
		mAudioSource.pitch = mCounterStartingPitch;
		mAudioSource.Play();
	}

	public Animator ChallangeAFriendAnimator;

	void FinishCounter()
	{
		if (mState != 2) {
			mState = 2;
			mAudioSource.Stop();

			mMetersLabel.text = ((int)MetersRun).ToString();
			mMetersLabel.transform.localScale = Vector3.one;
			mMetersTweenScale.enabled = false;

			mCoinsLabel.text = CoinsCollected.ToString();
			mCoinsLabel.enabled = true;
		//	mCoinsPrompt.enabled = true;
			mCoinsLabel.transform.localScale = Vector3.one;
			if(mCoinsTweenScale != null) mCoinsTweenScale.enabled = false;

			EnableGUI(true);

			if (AppManager.instance.InterstitialAdCounter != 4)
			{
				AppManager.instance.InterstitialAdCounter ++;
			}


			addedFuncionallity.PopUpsDialogCon();

			if(CoinsCollected > minCoinCollected) transform.Find("Starsplode_Coin").gameObject.SetActive(true);

			EndGame();


			minDistance = PlayerPrefs.GetFloat("BestScore");
			//print(MetersRun + "  " + minDistance);
			if(MetersRun >= minDistance)
			{
				//startUpAddsManager.ShowStartUpInterstisial();
				transform.Find("Starsplode_Distance").gameObject.SetActive(true);
				ChallangeAFriendAnimator.enabled = true;
			}


		}

	}

	//public StartUpAddsManager startUpAddsManager;
	//
	//
	//
	void Update()
	{

		if (Input.GetKeyDown(KeyCode.Escape)) {

			if (mState != 2) {
				FinishCounter();
			}
			else {
				transform.parent.Find("MainMenu").GetComponent<MainMenuController>().OnClickExitGame();
				GameObject.Find("LevelController").GetComponent<LevelGenerator>().OnQuitClick();
			}
		}
	}

	/**
	 * 
	 */

	/// <summary>
	///	MyChange

	public void EndGame()
	{


		#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR && !HIDE_AD
		
		if (AppManager.instance.InterstitialAdCounter == 4 && (PlayerPrefs.GetInt("DialogPopUp") > 3 ))
		{
			EnableGUI(false);
			AppManager.instance.DoShowInsterstitialAd();
		}
		
		#endif

		#if UNITY_IPHONE
		if (Social.localUser.authenticated) {
			
			// First death
			Social.ReportProgress(AppManager.ACHIEVEMENT_BOOM_YOURE_DOWN_IOS, 100, null);
			
			// 50 Coins
			if (Config.instance.LifetimeCoins >= 50)
			{
				Social.ReportProgress(AppManager.ACHIEVEMENT_50_COIN_IOS, 100, null);
			}
		}
		#endif


	}



	public LevelGenerator levelGenerator;

	void FixedUpdate()
	{
		if (mState == 0)
		{
			// Meter counter
			float t = ((Time.time - mCounterStartTime) / mCounterDuration);
			float tTween = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - t));
			MeterCounter = t * MetersRun;
			mMetersLabel.text = ((int)MeterCounter).ToString() ;

			// Speed up counter sound
			mAudioSource.pitch = mCounterStartingPitch + tTween;

			if (MeterCounter >= MetersRun)
			{
				mState = 1;
				mCounterStartTime = Time.time;
				mCoinsLabel.enabled = true;
			//	mCoinsPrompt.enabled = true;
				mAudioSource.Stop();
				StartCounter(CoinsCollected, CoinSound);

				// Reset scale
				mMetersTweenScale.enabled = false;
				mMetersTweenScale.transform.localScale = Vector3.one;

				// "And Collected"
				//TweenScale tween = TweenScale.Begin(mCoinsPrompt.gameObject, 0.2f, new Vector3(1.3f, 1.3f, 1));
				//tween.method = UITweener.Method.EaseOut;
				//EventDelegate.Add(tween.onFinished, OnScaleupTweenFinished);

				// Coins pulsing
				mCoinsTweenScale = TweenScale.Begin(mCoinsLabel.gameObject, 0.15f, new Vector3(1.2f, 1.2f, 1));
				mCoinsTweenScale.style = UITweener.Style.PingPong;

//				if(MetersRun > minDistance)
//				{
//					ChallangeAFriendAnimator.enabled = true;
//					transform.Find("Starsplode_Distance").gameObject.SetActive(true);
//				}
			
			}
		}
		else if (mState == 1)
		{
			// Coin counter
			float t = ((Time.time - mCounterStartTime) / mCounterDuration);
			float tTween = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - t));
			CoinCounter = t * CoinsCollected;
			mCoinsLabel.text = ((int)CoinCounter).ToString() ;

			// Speed up counter sound
			mAudioSource.pitch = mCounterStartingPitch + 0.2f * tTween;

			if (CoinCounter >= CoinsCollected)
			{
				// All counter finished
				mState = 2;
				mCoinsLabel.transform.localScale = Vector3.one;
				mCoinsTweenScale.enabled = false;
				mAudioSource.Stop();
//				Debug.Log("Fixed Update - Enable GUI");
				EnableGUI(true);

				
				minDistance = PlayerPrefs.GetFloat("BestScore");
				print(MetersRun + "  " + minDistance);
				if(MetersRun >= minDistance)
				{
					//startUpAddsManager.ShowStartUpInterstisial();
					transform.Find("Starsplode_Distance").gameObject.SetActive(true);
					ChallangeAFriendAnimator.enabled = true;
				}


				//if(CoinsCollected > minCoinCollected) transform.Find("Starsplode_Coin").gameObject.SetActive(true);

				if (AppManager.instance.InterstitialAdCounter != 4)
				{
					AppManager.instance.InterstitialAdCounter ++;
				}
				EndGame();

			//	addedFuncionallity.PopUpsDialogCon();
			}


		}
	}



	//
	//
	//

	public void OnScaleupTweenFinished()
	{
		TweenScale tween = TweenScale.Begin(UITweener.current.gameObject, 0.2f, Vector3.one);
		tween.method = UITweener.Method.EaseOut;;

	}

	//
	//
	//
	void OnFBLoginResult(bool loggedIn, string error)
	{
		FacebookManager.instance.loginResult -= OnFBLoginResult;

		if (loggedIn)
		{
			FacebookManager.instance.appRequestResult += FBAppRequestResult;
			FacebookManager.instance.SendAppRequest(
				"I ran " + ((int)MetersRun).ToString() + " meters. Can you beat me?",
				"Can you beat me?",
				MetersRun.ToString() + "," + Config.instance.CurrentCharacter + "," + Config.instance.CurrentCharacter
			);
		}
		else
		{
			EnableGUI(true);
		}
	}

	//
	//
	//
	void FBAppRequestResult(string result)
	{
		EnableGUI(true);
		Debug.Log("App Request result: " + result);
	}
}
