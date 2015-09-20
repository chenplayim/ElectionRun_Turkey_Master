using UnityEngine;
using System.Collections;
using System;


public class UpgradeScreenController : MonoBehaviour
{
	/**
	 * 
	 */
	public event EventHandler<EventArgs> Closed = delegate {};

	/**
	 * 
	 */
	static UpgradeScreenController mInstance;

	/**
	 * 
	 */
	public GameObject StoreGUI;

	public GameObject StartAnim;
	/**
	 * 
	 */
	Config mGameData;
	UILabel mCoinLbl;
	UILabel mBubbleLevelLbl;
	UILabel mJumpLevelLbl;
	UILabel mDoubleJumpLevelLbl;
	UILabel mBubbleCost;
	UILabel mJumpCost;
	UILabel mDoubleJumpCost;
	UIButton mDecBubbleBtn;
	UIButton mDecJumpBtn;
	UIButton mDecDoubleJumpBtn;
	UIButton mIncBubbleBtn;
	UIButton mIncJumpBtn;
	UIButton mIncDoubleJumpBtn;
	UILabel mGetMoreCoins;
	UIButton mStoreBtn;

	UISprite mMaxLevelBubble;
	UISprite mMaxLevelJump;
	UISprite mMaxLevelDouble;

	UISprite mCoinBubble;
	UISprite mCoinJump;
	UISprite mCoinDouble;

	UILabel mLevelName;
	public UILabel XPLabel;

	public AddedFuncionallity addedFuncionallity;
	/**
	 * 
	 */
	public static UpgradeScreenController instance
	{
		get { return mInstance; }
	}

	/**
	 * 
	 */
	public UpgradeScreenController()
	{
		mInstance = this;
	}

	/**
	 * 
	 */
	public void Display()
	{
		gameObject.SetActive(true);
	}

	//
	//
	//
	public void OnOpenCoinStore()
	{
		StoreController store = StoreGUI.GetComponent<StoreController>();
		store.Closed += OnStoreClosed;
		store.OnOpenCoinStore();
		StoreGUI.SetActive(true);
		gameObject.SetActive(false);

		addedFuncionallity.UpdateHatsIcon();
		addedFuncionallity.UpdateUpgradesIcon();

	}

	//
	//
	//
	public void OnOpenHatStore()
	{
//		StoreController store = StoreGUI.GetComponent<StoreController>();
//		store.Closed += OnStoreClosed;
//		store.OnOpenHatStore();
//		StoreGUI.SetActive(true);
//		gameObject.SetActive(false);
//
//		addedFuncionallity.UpdateHatsIcon();
//		addedFuncionallity.UpdateUpgradesIcon();
	}


	//
	//
	//
	public void OnIncrementBubble()
	{

		addedFuncionallity.UpdateHatsIcon();
		addedFuncionallity.UpdateUpgradesIcon();

		if (mGameData.CurrentBubbleLevel < mGameData.BubbleUpgrades.Length - 1)
		{
			int price = mGameData.BubbleUpgradeCosts[mGameData.CurrentBubbleLevel];
			
			if (mGameData.Coins >= price)
			{
				mGameData.Coins -= price;
				++mGameData.CurrentBubbleLevel;
				UpdateGUI();
				
				// Bloat text
				BloatGameObject(mBubbleLevelLbl.gameObject);
				if(mGameData.CurrentBubbleLevel == mGameData.BubbleUpgrades.Length - 1)BloatGameObject(mMaxLevelBubble.gameObject);
			}
			else
			{
				// Insufficient coins
				//my change
				//if (!mGetMoreCoins.gameObject.activeSelf) {
					NotifyGetMoreCoins();
				//}
			}
		}
	}

	/**
	 * 
	 */
	public void OnIncrementJumpHeight()
	{

		addedFuncionallity.UpdateHatsIcon();
		addedFuncionallity.UpdateUpgradesIcon();

		if (mGameData.CurrentJumpHeightLevel < mGameData.JumpHeightUpgrades.Length - 1)
		{
			int price = mGameData.JumpHeightUpgradeCosts[mGameData.CurrentJumpHeightLevel];

			if (mGameData.Coins >= price)
			{
				mGameData.Coins -= price;
				++mGameData.CurrentJumpHeightLevel;
				UpdateGUI();

				// Bloat text
				BloatGameObject(mJumpLevelLbl.gameObject);
				if(mGameData.CurrentJumpHeightLevel == mGameData.JumpHeightUpgrades.Length - 1)BloatGameObject(mMaxLevelJump.gameObject);
			}
			else
			{
				// Insufficient coins
				//my change
				//if (!mGetMoreCoins.gameObject.activeSelf) {
					NotifyGetMoreCoins();
				//}
			}
		}
	}

	public void AddBubbleShieldFromBonusScreen(){
		++mGameData.CurrentBubbleLevel;
		UpdateGUI();
		// Bloat text
		DoubleBloatWin(mBubbleLevelLbl.gameObject);
		//if(mGameData.CurrentBubbleLevel == mGameData.BubbleUpgrades.Length - 1)BloatGameObject(mMaxLevelBubble.gameObject);
	}
	public void AddJumpHightFromBonusScreen(){
		++mGameData.CurrentJumpHeightLevel;
		UpdateGUI();
		// Bloat text
		DoubleBloatWin(mJumpLevelLbl.gameObject);
		//if(mGameData.CurrentJumpHeightLevel == mGameData.JumpHeightUpgrades.Length - 1)BloatGameObject(mMaxLevelJump.gameObject);
	}
	public void AddDoubleJumpHightFromBonusScreen(){
		++mGameData.CurrentDoubleJumpHeightLevel;
		UpdateGUI();
		// Bloat text
		DoubleBloatWin(mBubbleLevelLbl.gameObject);
		//if(mGameData.CurrentBubbleLevel == mGameData.BubbleUpgrades.Length - 1)BloatGameObject(mMaxLevelBubble.gameObject);
	}

	public void BloatCoinsFromBonusScreen(){

		UpdateGUI();
		DoubleBloatWin(mCoinLbl.gameObject);
	}

	void DoubleBloatWin(GameObject go)
	{
		// Bloat text
		TweenScale tween = TweenScale.Begin(go, 0.4f, new Vector3(1.5f, 2f, 1));
		tween.method = UITweener.Method.BounceIn;
		EventDelegate.Add(tween.onFinished, OnScaleupTweenFinished);

		if (go.gameObject.name == "BubbleLevel" || go.gameObject.name == "JumpHeightLevel" || go.gameObject.name == "DoubleJumpHeightLevel" || go.gameObject.name == "Coin" )
		{
			Color NewColor = Color.white;
			TweenColor tweenC = TweenColor.Begin(go, 0.4f, NewColor);
			tweenC.method = UITweener.Method.BounceIn;
			EventDelegate.Add(tweenC.onFinished, OnColorTweenFinishedBonus);
		}
		
	}

	void OnColorTweenFinishedBonus()
	{
		Color OrgColor = new Color (1f,1.008888888888889f,0.28627450980392155f,1f);
		TweenColor tweenC = TweenColor.Begin(UITweener.current.gameObject, 0.4f, OrgColor);
		tweenC.method = UITweener.Method.EaseOut;
	}


	//
	//
	//
	public void OnDecrementBubble()
	{

		addedFuncionallity.UpdateHatsIcon();
		addedFuncionallity.UpdateUpgradesIcon();

		if (mGameData.CurrentBubbleLevel > 0)
		{
			mGameData.Coins += mGameData.BubbleUpgradeCosts[mGameData.CurrentBubbleLevel - 1];
			--mGameData.CurrentBubbleLevel;
			UpdateGUI();
			
			// Bloat text
			BloatGameObject(mBubbleLevelLbl.gameObject);
		}
	}

	/**
	 * 
	 */
	public void OnDecrementJumpHeight()
	{

		addedFuncionallity.UpdateHatsIcon();
		addedFuncionallity.UpdateUpgradesIcon();

		if (mGameData.CurrentJumpHeightLevel > 0)
		{
			mGameData.Coins += mGameData.JumpHeightUpgradeCosts[mGameData.CurrentJumpHeightLevel - 1];
			--mGameData.CurrentJumpHeightLevel;
			UpdateGUI();

			// Bloat text
			BloatGameObject(mJumpLevelLbl.gameObject);
		}
	}

	/**
	 * 
	 */
	public void OnIncrementDoubleJumpHeight()
	{

		addedFuncionallity.UpdateHatsIcon();
		addedFuncionallity.UpdateUpgradesIcon();

		if (mGameData.CurrentDoubleJumpHeightLevel < mGameData.DoubleJumpHeightUpgrades.Length - 1)
		{
			int price = mGameData.DoubleJumpHeightUpgradeCosts[mGameData.CurrentDoubleJumpHeightLevel];

			if (mGameData.Coins >= price)
			{
				mGameData.Coins -= price;
				++mGameData.CurrentDoubleJumpHeightLevel;
				UpdateGUI();

				// Bloat text
				BloatGameObject(mDoubleJumpLevelLbl.gameObject);
				if(mGameData.CurrentDoubleJumpHeightLevel == mGameData.DoubleJumpHeightUpgrades.Length - 1)BloatGameObject(mMaxLevelDouble.gameObject);
			}
			else
			{
				// Insufficient coins
				//my change
				//if (!mGetMoreCoins.gameObject.activeSelf) {
					NotifyGetMoreCoins();
				//}
			}
		}
	}
	
	/**
	 * 
	 */
	public void OnDecrementDoubleJumpHeight()
	{

		addedFuncionallity.UpdateHatsIcon();
		addedFuncionallity.UpdateUpgradesIcon();

		if (mGameData.CurrentDoubleJumpHeightLevel > 0)
		{
			mGameData.Coins += mGameData.DoubleJumpHeightUpgradeCosts[mGameData.CurrentDoubleJumpHeightLevel - 1];
			--mGameData.CurrentDoubleJumpHeightLevel;
			UpdateGUI();

			// Bloat text
			BloatGameObject(mDoubleJumpLevelLbl.gameObject);
		}
	}

	/**
	 * 
	 */

	public GameObject GameEndScreen;

	public void OnClickExit()
	{
		gameObject.SetActive(false);
		mGameData.Save();
		Closed(this, EventArgs.Empty);
		AppManager.ShowBannerAd ();
	}

	public void OnClickExitToReplay()
	{
		gameObject.SetActive(false);
		mGameData.Save();
		//GameEndScreen.GetComponent<GameOverGUIController>().CanShowPopUps = false;
	    Closed(this, EventArgs.Empty);
		//GameEndScreen.SetActive (false);
		//AppManager.ShowBannerAd ();
	}



	/**
	 * 
	 */
	public void OnOpenStore()
	{
		StoreGUI.GetComponent<StoreController>().Closed += OnStoreClosed;
		StoreGUI.SetActive(true);
		gameObject.SetActive(false);
	}

	//
	//
	//
	void NotifyGetMoreCoins()
	{
		#if !UNITY_WEBPLAYER
		mGetMoreCoins.gameObject.SetActive(true);
		BloatGameObject(mGetMoreCoins.gameObject);
		BloatGameObject(mStoreBtn.gameObject);
		StartCoroutine( HideGameObject(mGetMoreCoins.gameObject, 2) );
		#endif
	}

	//
	//
	//
	IEnumerator HideGameObject(GameObject go, float secondsDelay)
	{
		yield return new WaitForSeconds(secondsDelay);
		go.SetActive(false);
	}

	//
	//
	//
	public void BloatGameObject(GameObject go)
	{
		// Bloat text
		TweenScale tween = TweenScale.Begin(go, 0.3f, new Vector3(1.6f, 1.6f, 1));
		tween.method = UITweener.Method.BounceIn;
		EventDelegate.Add(tween.onFinished, OnScaleupTweenFinished);


		// Bloat color
		if (go.gameObject.name == "OpenShopButton2")
		{
		Color NewColor = new Color (1f, 0.654901960784313725490196078431371f , 0.50196078431372549019607843137255f , 1f);
		TweenColor tweenC = TweenColor.Begin(go, 0.3f, NewColor);
		tweenC.method = UITweener.Method.BounceIn;
		EventDelegate.Add(tweenC.onFinished, OnColorTweenFinished);
		}

	}

	//public GameObject go;
	public GameObject starsAnimHat;
	//public bool ShowVideoBTN;
	int timeToLoop;

//	public void BloatHAT()
//	{
//
//
//		starsAnimHat.SetActive (true);
//		TweenScale tween = TweenScale.Begin(go, 0.3f, new Vector3(1.8f, 1.8f, 1));
//
//		tween.method = UITweener.Method.BounceOut;
//		EventDelegate.Add(tween.onFinished, OnHatScaleupTweenFinished);
//
//		Color NewColor = new Color (1f, 0.654901960784313725490196078431371f, 0.50196078431372549019607843137255f, 1f);
//		TweenColor tweenC = TweenColor.Begin(go, 0.3f, NewColor);
//		tweenC.method = UITweener.Method.BounceIn;
//
//
//	}
//	
//	void OnHatScaleupTweenFinished()
//	{
//
//		TweenScale tween = TweenScale.Begin(UITweener.current.gameObject, 0.3f, Vector3.one);
//		tween.method = UITweener.Method.EaseInOut;
//		TweenColor tweenC = TweenColor.Begin(UITweener.current.gameObject, 0.3f, Color.white);
//		tweenC.method = UITweener.Method.EaseInOut;
//		Destroy (starsAnimHat);
//
//	}


	//

	void OnColorTweenFinished()
	{
		TweenColor tweenC = TweenColor.Begin(UITweener.current.gameObject, 0.3f, Color.white);
		tweenC.method = UITweener.Method.EaseOut;
	}

	void OnScaleupTweenFinished()
	{
		TweenScale tween = TweenScale.Begin(UITweener.current.gameObject, 0.3f, Vector3.one);
		tween.method = UITweener.Method.EaseOut;
	}

	//
	//
	//
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			OnClickExit();
		}
	}

	/**
	 * 
	 */
	void OnStoreClosed(object sender, System.EventArgs args)
	{
		StoreGUI.GetComponent<StoreController>().Closed -= OnStoreClosed;
		gameObject.SetActive(true);
	}

	/**
	 * 
	 */
	void Awake()
	{
		AssignObjects ();
	}

	public void AssignObjects()
	{
		AppManager.instance.challengeActivated += OnChallengeActivated;
		mGameData = GameObject.Find("GameData").GetComponent<Config>();
		mBubbleLevelLbl = transform.FindChild("BubbleLevel").GetComponent<UILabel>();
		mJumpLevelLbl = transform.FindChild("JumpHeightLevel").GetComponent<UILabel>();
		mDoubleJumpLevelLbl = transform.FindChild("DoubleJumpHeightLevel").GetComponent<UILabel>();
		mCoinLbl = transform.FindChild("Coin").GetComponent<UILabel>();
		mBubbleCost = transform.FindChild("BubblePrice").GetComponent<UILabel>();
		mJumpCost = transform.FindChild("JumpPrice").GetComponent<UILabel>();
		mDoubleJumpCost = transform.FindChild("DoubleJumpPrice").GetComponent<UILabel>();
		mDecBubbleBtn = transform.FindChild("DecBubbleButton").GetComponent<UIButton>();
		mDecJumpBtn = transform.FindChild("DecJumpHeightButton").GetComponent<UIButton>();
		mDecDoubleJumpBtn = transform.FindChild("DecDoubleJumpHeightButton").GetComponent<UIButton>();
		mIncBubbleBtn = transform.FindChild("IncBubbleButton").GetComponent<UIButton>();
		mIncJumpBtn = transform.FindChild("IncJumpHeightButton").GetComponent<UIButton>();
		mIncDoubleJumpBtn = transform.FindChild("IncDoubleJumpHeightButton").GetComponent<UIButton>();
		mGetMoreCoins = transform.FindChild("GetMoreCoins").GetComponent<UILabel>();
		mStoreBtn = transform.FindChild("OpenShopButton2").GetComponent<UIButton>();
		mMaxLevelBubble = transform.FindChild ("MaxLevelBubble").GetComponent<UISprite> ();
		mMaxLevelJump = transform.FindChild ("MaxLevelJump").GetComponent<UISprite> ();
		mMaxLevelDouble = transform.FindChild ("MaxLevelDouble").GetComponent<UISprite> ();
		mCoinBubble = transform.FindChild ("CoinImageBubble").GetComponent<UISprite> ();
		mCoinJump = transform.FindChild ("CoinImageJump").GetComponent<UISprite> ();
		mCoinDouble = transform.FindChild ("CoinImageDouble").GetComponent<UISprite> ();
		mLevelName = transform.FindChild("LevelName").GetComponent<UILabel>();

	//	print ("AWWWWWWWWWWWAKWW");
	}

	/**
	 * 
	 */
	public AdColonyCommends AdColonyCommends ;
	public GameObject FacebookPageBTN;
	public GameObject WatchVideo;

	public void ElectionRunFacebook()
	{
		Application.OpenURL ("https://www.facebook.com/IsraeliElections2015");
		FacebookPageBTN.SetActive (false);
		PlayerPrefs.SetInt ("FacebookPageShow", 1);
	}

	void OnEnable()
	{

		if (PlayerPrefs.GetInt ("FacebookPageShow") == 0 && WatchVideo.activeSelf == false) {
			
			FacebookPageBTN.SetActive (true);
		
			//print ("showfacebook");
		} 
		else 
		{
			
			FacebookPageBTN.SetActive(false);
			//print ("DONTshowfacebook");
		}


		#if UNITY_WEBPLAYER
		mStoreBtn.gameObject.SetActive(false);
		transform.FindChild("StoreButton").gameObject.SetActive(false);
		transform.FindChild("OpenShopButton").gameObject.SetActive(false);
		transform.FindChild("GetMoreCoins").gameObject.SetActive(false);
		#endif


			StartAnim.GetComponent<UISpriteAnimation> ().Reset ();
			AdColonyCommends.ChackIfVideoActive ();


		UpdateGUI();
		UIPlaySound sound = GetComponent<UIPlaySound>();
		if(sound != null) sound.Play();

		// Analytics
		if(Analytics.gua != null) Analytics.gua.sendAppScreenHit("Upgrade Menu");

		mLevelName.text = XPLabel.text;
	}

	//
	//
	//
	void UpdateGUI()
	{
		mCoinLbl.text = mGameData.Coins.ToString();
		mBubbleLevelLbl.text = mGameData.CurrentBubbleLevel.ToString();
		mJumpLevelLbl.text = mGameData.CurrentJumpHeightLevel.ToString();
		mDoubleJumpLevelLbl.text = mGameData.CurrentDoubleJumpHeightLevel.ToString();

		mBubbleCost.text = Config.instance.BubbleUpgradeCosts[mGameData.CurrentBubbleLevel].ToString();
		mJumpCost.text = Config.instance.JumpHeightUpgradeCosts[mGameData.CurrentJumpHeightLevel].ToString();
		mDoubleJumpCost.text = Config.instance.DoubleJumpHeightUpgradeCosts[mGameData.CurrentDoubleJumpHeightLevel].ToString();

		mDecBubbleBtn.isEnabled = mGameData.CurrentBubbleLevel > 0;
		mDecJumpBtn.isEnabled = mGameData.CurrentJumpHeightLevel > 0;
		mDecDoubleJumpBtn.isEnabled = mGameData.CurrentDoubleJumpHeightLevel > 0;

		// Show Max Level Label
		if (mGameData.CurrentJumpHeightLevel == mGameData.JumpHeightUpgrades.Length - 1) {
			mMaxLevelJump.gameObject.SetActive (true);
			mIncJumpBtn.gameObject.SetActive(false);
			mJumpCost.gameObject.SetActive(false);
			mCoinJump.gameObject.SetActive(false);

		} else {
			mMaxLevelJump.gameObject.SetActive(false);
			mIncJumpBtn.gameObject.SetActive(true);
			mJumpCost.gameObject.SetActive(true);
			mCoinJump.gameObject.SetActive(true);
		}

		if (mGameData.CurrentBubbleLevel == mGameData.BubbleUpgrades.Length - 1) {
			mMaxLevelBubble.gameObject.SetActive (true);
			mIncBubbleBtn.gameObject.SetActive(false);
			mBubbleCost.gameObject.SetActive(false);
			mCoinBubble.gameObject.SetActive(false);
		} else {
			mMaxLevelBubble.gameObject.SetActive(false);
			mIncBubbleBtn.gameObject.SetActive(true);
			mBubbleCost.gameObject.SetActive(true);
			mCoinBubble.gameObject.SetActive(true);
		}

		if (mGameData.CurrentDoubleJumpHeightLevel == mGameData.DoubleJumpHeightUpgrades.Length - 1) {
			mMaxLevelDouble.gameObject.SetActive (true);
			mIncDoubleJumpBtn.gameObject.SetActive(false);
			mDoubleJumpCost.gameObject.SetActive(false);
			mCoinDouble.gameObject.SetActive(false);
		} else {
			mMaxLevelDouble.gameObject.SetActive(false);
			mIncDoubleJumpBtn.gameObject.SetActive(true);
			mDoubleJumpCost.gameObject.SetActive(true);
			mCoinDouble.gameObject.SetActive(true);
		}
	}

	void OnChallengeActivated(ChallengeDetail challenge)
	{
		if( !gameObject.activeSelf ) return;
		gameObject.SetActive(false);
		
		if (!LevelGenerator.instance.gameObject.activeSelf) {
			transform.parent.FindChild("MainMenu").GetComponent<MainMenuController>().StartGame(challenge.Name, challenge.Distance, challenge.Hat, challenge.FacebookID, challenge.Character);
		}
	}
	
}
