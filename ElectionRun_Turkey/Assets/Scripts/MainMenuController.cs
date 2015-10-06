using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class MainMenuController : MonoBehaviour
{
	/**
	 * 
	 */
	public GameObject CharacterSelectionGUI;
	public GameObject StoreGUI;
	public GameObject GooglePlayGUI;
	public GameObject GameCenterGUI;

	/**
	 * 
	 */
	private GameObject mHero;
	private GameObject mPlatform;
	private GameObject mLevelController;
	private GameObject mHUD;
	GameObject mUpgradeNotification;
	UIButton mMuteButton;
	UIButton mSoundOnButton;

	public static MainMenuController instence;

	//
	//
	//
	public void OnFacebookChallenges()
	{
		AppManager.instance.ShowChallengeList();
	}

	//
	//
	//
	public void OnOpenGooglePlay()
	{
		gameObject.SetActive(false);
		GooglePlayGUI.GetComponent<GooglePlayGUIController>().Closed += OnGooglePlayMenuClosed;
		GooglePlayGUI.SetActive(true);
	}
	

	public void OnGameCenterPlay()
	{
		GameCenterGUI.SetActive(true);
		gameObject.SetActive(false);
	}


	//
	//
	//
	public void OnLoginFacebook()
	{
		FacebookManager.instance.loginResult += OnFBLoginResult;
		FacebookManager.instance.Login();
		Debug.Log (FacebookManager.instance.name + FacebookManager.instance.appRequestResult);
		Config.instance.PlayerName = FacebookManager.instance.name;
	}

	/**
	 * 
	 */

	public GameObject GameEndScreen ;
	public UIPanel GameEndScreenPanel ;


	public void OnClickStart()
	{
			GameEndScreen.SetActive (false);
			GameEndScreenPanel.depth = 1;

			StartGame();

	}

	/**
	 * 
	 */
	public void OnClickCharacterSelection()
	{
		CharacterSelectionGUI.SetActive(true);
		CharacterSelectionGUI.GetComponent<CharacterSelectionController>().Closed += OnCharacterSelectionClosed;
		gameObject.SetActive(false);

		// Analytics
		Analytics.gua.sendAppScreenHit("Select Character");
	}

	/**
	 * 
	 */
	public void OnCharacterSelectionClosed(CharacterSelectionController sender)
	{
		CharacterSelectionGUI.SetActive(false);
		gameObject.SetActive(true);
	}

	/**
	 * 
	 */
	public void OnClickExitGame()
	{
		gameObject.SetActive(true);
		mHUD.SetActive(false);
		mLevelController.GetComponent<LevelGenerator>().CloseGame();
		//AppManager.ShowBannerAd();
	}

	/**
	 * 
	 */
	public void OnClickUpgrade()
	{
		gameObject.SetActive(false);
		UpgradeScreenController.instance.Closed += UpgradeClosed;
		//AppManager.HideBannerAd ();
	}

	//
	//
	//
	public void OnOpenStore()
	{
		StoreGUI.GetComponent<StoreController>().Closed += OnStoreClosed;
		StoreGUI.SetActive(true);
		gameObject.SetActive(false);
		//AppManager.HideBannerAd ();
	}

	/**
	 * 
	 */
	public void UpgradeClosed(object sender, EventArgs args)
	{
		this.gameObject.SetActive(true);
		UpgradeScreenController.instance.Closed -= UpgradeClosed;
	}

	//
	//
	//
	public void OnClickMute() {
		AppManager.enableSound = !AppManager.enableSound;
		UpdateButton(!AppManager.enableSound);
	}

	//
	//
	//
	public void StartGame(string opponentName = null, float opponentDistance = float.NaN, string opponentHat = null, string challengerFBID = null, string challengerCharacter = null) {
		mHUD.SetActive(true);
		mHero.SetActive(true);
		mPlatform.SetActive(true);
		mLevelController.SetActive(true);
		gameObject.SetActive(false);

		LevelGenerator.instance.RestartGame(opponentName, opponentDistance, opponentHat, challengerFBID, challengerCharacter);

		// Analytics
		Analytics.gua.sendEventHit("Action", "Start Game");
		Analytics.gua.sendAppScreenHit("Game Screen");
	}

	//
	//
	//
	void OnTutorialClosed(object sender, System.EventArgs args) {
		(sender as TouchToClose).Closed -= OnTutorialClosed;
		StartGame();
	}

	//
	//
	//
	void OnFirstCharacterSelectFinished(CharacterSelectionController sender) {
		sender.Closed -= OnFirstCharacterSelectFinished;


		// Update camera
		LevelGenerator.playerController.UpdateCamera();
	}

	//
	//
	//
	void OnStoreClosed(object sender, System.EventArgs args)
	{
		StoreGUI.GetComponent<StoreController>().Closed -= OnStoreClosed;
		gameObject.SetActive(true);
	}

	/**
	 * 
	 */
	#if UNITY_ANDROID
	//public AdColonyCommends adColonyCommends;
	#endif
	void Awake()
	{

		instence = this;

		AppManager.instance.challengeActivated += OnChallengeActivated;
		Transform transf = transform;
		mHero = GameObject.Find("Hero");
		mPlatform = GameObject.FindGameObjectWithTag("platform");
		mLevelController = GameObject.Find("LevelController");
		mHUD = GameObject.Find("HUD");
		mUpgradeNotification = transf.FindChild("UpgradeNotification").gameObject;
		mMuteButton = transf.FindChild("MuteButton").GetComponent<UIButton>();
		mSoundOnButton = transf.FindChild("SoundOnButton").GetComponent<UIButton>();

		mHero.SetActive(false);
		mPlatform.SetActive(false);
		mLevelController.SetActive(false);
		mHUD.SetActive(false);
		//GameObject.Find("Store").SetActive(false);

		//my change Web
		#if UNITY_WEBPLAYER
		transf.FindChild("Grid/ChallengeButton").gameObject.SetActive(false);
		transf.FindChild("RightPanel/Grid/GooglePlayButton").gameObject.SetActive(false);
		transf.FindChild("RightPanel/Grid/FacebookButton").gameObject.SetActive(false);
		transf.FindChild("RightGrid").GetComponent<UIGrid>().Reposition();
		#endif
	}

	/**
	 * 
	 */
	void Start()
	{
		Analytics.gua.sendAppScreenHit("Main Menu");
	}

	/**
	 * 
	 */


	void OnEnable()
	{
		if(Analytics.gua != null) Analytics.gua.sendAppScreenHit("Main Menu");
		UpdateButton(!AppManager.enableSound);

		// Upgrade button notification
		mUpgradeNotification.SetActive(Config.instance.HasBuyableUpgrades());

		Time.timeScale = 1;

	}


	public GameObject DialogControllerOBG;
	public GameObject ChallengeListControllerOBG;
	
	int NextTimeToShowInterstisialOnQuit;
	bool CanshowInterstisial = false;
	int TimesToShowInterstisial = 0;

	void Update()
	{

		#if !UNITY_WEBPLAYER
		if (Input.GetKeyDown(KeyCode.Escape)  && DialogController.IsConfermationOpen == false && ChallengeListController.challengeListOpen == false)
		{

			// Confirm game quit
			GameObject go = transform.parent.FindChild("ConfirmationDialog").gameObject;
			DialogController dlgControl = go.GetComponent<DialogController>();
			dlgControl.Closed += OnConfirmExit;
			dlgControl.SetMessage("Çıkmak istediğinizden eminmisiniz?");
			go.SetActive(true);

			//MyHeyzap
			if (System.DateTime.Now.Hour > PlayerPrefs.GetInt("NextTimeToShowInterstisialOnQuit"))
			{
				CanshowInterstisial = true;
				NextTimeToShowInterstisialOnQuit = System.DateTime.Now.AddHours(12).Hour;
				PlayerPrefs.SetInt("NextTimeToShowInterstisialOnQuit" , NextTimeToShowInterstisialOnQuit);
			}
			
			if (CanshowInterstisial = true && TimesToShowInterstisial == 0)
			{
				MyHeyzap.instence.ShowInterstitialAd();
				CanshowInterstisial = false;
				TimesToShowInterstisial = 1;
			}


		}
		#endif

		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Delete))
		{
			// Reset save game
			Config.instance.ResetSavedGame();
		}
		#endif


		if (DialogControllerOBG.activeSelf == false)
		{
			DialogController.IsConfermationOpen = false;
		}
		if (ChallengeListControllerOBG.activeSelf == false)
		{
			ChallengeListController.challengeListOpen = false;
		}
	}
	

	public GameObject StarsEffect;
	public UILabel CoinsLabelinUpgrade;
	public UILabel SetMessage;
	public GameObject StartAnim;

	public void FreeCoinsPopUp()
	{

		int CoinsWonRandom = UnityEngine.Random.Range (50,100);
		float CoinsWonf = CoinsWonRandom / 10;
		double CoinsWond = Math.Round(CoinsWonf) * 10;
		int CoinsWon = (int)Math.Round(CoinsWond);

		GameObject go = transform.parent.FindChild("DialogYouWon").gameObject;

		SetMessage.text = CoinsWon + " oy aldın!";
		go.SetActive(true);
		StarsEffect.SetActive (true);
		Config.instance.Coins += CoinsWon;
		CoinsLabelinUpgrade.text = Config.instance.Coins.ToString();

		StartAnim.GetComponent<UISpriteAnimation> ().Reset ();

	}

	//
	//
	//
	void OnConfirmExit(bool ok)
	{
		// Exit game
		if (ok) Application.Quit();
	}

	//
	//
	//
	void UpdateButton(bool bMute) {
		mMuteButton.gameObject.SetActive(!bMute);
		mSoundOnButton.gameObject.SetActive(bMute);
	}

	//
	//
	//
	void OnChallengeActivated(ChallengeDetail challenge)
	{
		if (!gameObject.activeSelf) return;

		Debug.Log("MainMenuController Challenge: " + challenge.Name + "," + challenge.Hat + "," + challenge.Character);
		StartGame(challenge.Name, challenge.Distance, challenge.Hat, challenge.FacebookID, challenge.Character);

		// Analytics
		Analytics.gua.sendEventHit("Action", "Start Challenge");
	}

	//
	//
	//
	void OnFBLoginResult(bool loggedIn, string error)
	{
		FacebookManager.instance.loginResult -= OnFBLoginResult;
	}

	//
	//
	//
	void OnGooglePlayMenuClosed()
	{
		GooglePlayGUI.GetComponent<GooglePlayGUIController>().Closed -= OnGooglePlayMenuClosed;
		gameObject.SetActive(true);
	}
}
