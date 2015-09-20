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
		//Debug.Log (FacebookManager.instance.name);
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
			// Show character selection on first play
//		if (PlayerPrefs.GetInt("Saved", 0) != 1)  {
//			CharacterSelectionGUI.SetActive(true);
//			CharacterSelectionGUI.GetComponent<CharacterSelectionController>().Closed += OnFirstCharacterSelectFinished;
//		}
//		else {
			StartGame();
//		}

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
		//Debug.Log(opponentName + opponentDistance + opponentHat + challengerFBID + challengerCharacter);
		//LevelGenerator.instance.RestartGame("Myself", 30, "Opera Mask", challengerFBID, challengerCharacter);

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
//
//		// Show tutorial dialog
//		GameObject go = transform.parent.FindChild("Tutorial").gameObject;
//		go.SetActive(true);
//		go.GetComponent<TouchToClose>().Closed += OnTutorialClosed;

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
	public AdColonyCommends adColonyCommends;
	#endif
	void Awake()
	{

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
		//SetRandomHat(transform.FindChild("GameTitle/HeadBoy/Hat").gameObject);
		//SetRandomHat(transform.FindChild("GameTitle/HeadGirl/Hat").gameObject);

		
	}

	//
	//
	//
//	void SetRandomHat(GameObject hatNode)
//	{
//		//HatInfo hat = StoreController.GetHatInfoAt( (int)(UnityEngine.Random.value * StoreController.GetNumHats()) );
//		UI2DSprite sprite = hatNode.GetComponent<UI2DSprite>();
//		GameObject hatPrefab = Resources.Load<GameObject>("Hats/" + hat.PrefabName);
//		sprite.sprite2D = Multires.GetSprite(hat.SpriteName);
//
//		float width = Screen.width > Screen.height ? Screen.width : Screen.height;
//		int myWidth = (int)sprite.sprite2D.textureRect.width;
//		int myHeight = (int)sprite.sprite2D.textureRect.height;
//		
//		if (width >= Multires.instance.MaxWidth) {
//			myWidth = myWidth/2;
//			myHeight = myHeight/2;
//		}
//		sprite.width = myWidth; 
//		sprite.height = myHeight;
//
//		hatNode.transform.localPosition = Vector3.Scale(hatPrefab.transform.localPosition, new Vector3(100, 100, 1));
//	}

	/**
	 * 
	 */


	public GameObject DialogControllerOBG;
	public GameObject ChallengeListControllerOBG;

	void Update()
	{

		#if !UNITY_WEBPLAYER
		if (Input.GetKeyDown(KeyCode.Escape)  && DialogController.IsConfermationOpen == false && ChallengeListController.challengeListOpen == false)
		{
			// Confirm game quit
			GameObject go = transform.parent.FindChild("ConfirmationDialog").gameObject;
			DialogController dlgControl = go.GetComponent<DialogController>();
			dlgControl.Closed += OnConfirmExit;
			dlgControl.SetMessage("?בוזעל ךנוצרב םאה");
			go.SetActive(true);
			//StartCoroutine(WaitForSecond());
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



//		public void PhysycalBackController ()
//		{
//			ChallengeListController.challengeListOpen = false;
//			DialogController.IsConfermationOpen = false;
//			print ("sdfgsdfg");
//		}



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

		SetMessage.text =  "תולוק " + CoinsWon + "ב תיכז";
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
		//PlayerPrefs.Flush();
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
