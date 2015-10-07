using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using GooglePlayGames;


public class HUDController : MonoBehaviour {

	//
	//
	//
	public GameObject PauseMenu;

	/**
	 * 
	 */
	UILabel mCoinsLbl;
	UILabel mMetersLbl;
	UIButton mMuteButton;
	UIButton mSoundOnButton;
	UIButton mMenuButton;


	///My Change
	//public UITweener XPTween;
	public UISlider XPSlide;
	public UILabel XPLabel;
	float XPValue;
	UIWidget widget; 

	public PlayerController playerController;

	#if UNITY_ANDROID
	public const string New_Seat = "CgkIpP-liokGEAIQAQ";
	public const string Rising_Star = "CgkIpP-liokGEAIQA";
	public const string Meteor = "CgkIpP-liokGEAIQAw";
	public const string Lobbist = "CgkIpP-liokGEAIQBA";
	public const string New_Member = "CgkIpP-liokGEAIQBQ";
	public const string Party_Member = "CgkIpP-liokGEAIQBg";
	public const string Prime_Minister = "CgkIpP-liokGEAIQBw";

	#endif



	#if UNITY_IPHONE
	public const string NOVICE_UNLOCKED = "NU1234";
	public const string ROOKIE_UNLOCKED = "RU1234";
	public const string BEGINNER_UNLOCKED = "BU1234";
	public const string TALENTED_UNLOCKED = "TU1234";
	public const string SKILLED_UNLOCKED = "SU1234";
	public const string INTERMEDIATE_UNLOCKED = "IU1234";
	public const string SKILLFUL_UNLOCKED = "SfullU1234";
	public const string SEASONED_UNLOCKED = "SeasonedU1234";
	public const string PROFICIENT_UNLOCKED = "PU1234";
	public const string EXPERIENCED_UNLOCKED = "EU1234";
	public const string MASTER_UNLOCKED = "MU1234";
	public const string RUNNING_GOD_UNLOCKED = "RGU1234";
	public const string MASTER_OF_UNIVERSE_UNLOCKED = "MOUU1234";
	#endif



	/**
	 * 
	 */

	void Start()
	{
		//playerController.MetersRun = 2390;
		XPSlide.value = 0;
		widget = XPSlide.foregroundWidget;
	}

	public void SetEnable(bool b)
	{
		mMuteButton.enabled = mMenuButton.enabled = b;
	}

	/**
	 * 
	 */
	public void SetCoins(int coins)
	{
		mCoinsLbl.text = coins.ToString();
	}

	/**
	 * 
	 */
	public void SetMeters(int meters)
	{
		mMetersLbl.text = meters.ToString() ;//+ " m";
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
	public void OnClickMenu() {
		PauseMenuController menu = PauseMenu.GetComponent<PauseMenuController>();
		menu.Closed += OnPauseMenuClosed;
		menu.OpenPauseMenu();
	}


	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			OnClickMenu();
		}
		
		//My Changes
		XPLabelController ();
		XPController ();
		XPControllerBetweenTitels();
		//#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_WEBPLAYER) && !UNITY_EDITOR 

		//#endif
		
	}

	public static float NextBestScore;
	public static float BestScore;
	public static string PlayersXPStatus;

	public static void XPRefresh()
	{

		PlayersXPStatus = PlayerPrefs.GetString ("PlayersXPStatus");
		BestScore = PlayerPrefs.GetFloat ("BestScore");
		NextBestScore = PlayerPrefs.GetFloat ("NextBestScore");

	}
	void Awake()
	{
		Transform transf = transform;
		mCoinsLbl = transf.FindChild("Coins").GetComponent<UILabel>();
		mMetersLbl = transf.FindChild("Meters").GetComponent<UILabel>();
		mMuteButton = transf.FindChild("MuteButton").GetComponent<UIButton>();
		mSoundOnButton = transf.FindChild("SoundOnButton").GetComponent<UIButton>();
		mMenuButton = transf.FindChild("MenuButton").GetComponent<UIButton>();

		//print (mCoinsLbl.text);
	}


	public void WhenDie()
	{
		PlayerPrefs.SetString ("PlayersXPStatus", XPLabel.text);
		PlayerPrefs.SetFloat("NextBestScore" , NextBestScore);
		PlayerPrefs.SetFloat("BestScore", BestScore);
		XPSlide.value = 0;
	}



	void XPController()
	{


		if (playerController.MetersRun < 50 && (BestScore < 50)) 
		{
			XPValue = (playerController.MetersRun/100) * 2;
			XPSlide.value = XPValue;
			widget.color = Color.green;

			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore =  playerController.MetersRun;
			}
		}

		if (playerController.MetersRun < 100 && playerController.MetersRun > 50 && (BestScore < 100)) 
		{
			XPValue = (playerController.MetersRun -50) /50;
			XPSlide.value = XPValue;
			widget.color = Color.yellow;
			if (BestScore < playerController.MetersRun)
			{ 
				BestScore =  playerController.MetersRun;
			}
		}


		if (playerController.MetersRun < 200 && playerController.MetersRun > 100 && (BestScore < 200)) 
		{
			XPValue = (playerController.MetersRun - 100) /100;
			XPSlide.value = XPValue;
			widget.color = Color.blue;
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore =  playerController.MetersRun;
			}
		}

		if (playerController.MetersRun < 350 && playerController.MetersRun > 200 && (BestScore < 350)) 
		{
			XPValue = (playerController.MetersRun - 200) /150;
			XPSlide.value = XPValue;
			widget.color = Color.red;
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore =  playerController.MetersRun;
			}
		}
		if (playerController.MetersRun < 500 && playerController.MetersRun > 350 && (BestScore < 500)) 
		{
			XPValue = (playerController.MetersRun - 350) /150;
			XPSlide.value = XPValue;
			widget.color = Color.cyan;
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore =  playerController.MetersRun;
			}
		}
		if (playerController.MetersRun < 700 && playerController.MetersRun > 500 && (BestScore < 700)) 
		{
			XPValue = (playerController.MetersRun - 500) /200;
			XPSlide.value = XPValue;
			widget.color = Color.magenta;
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore = playerController.MetersRun;
			}
		}
		if (playerController.MetersRun < 900 && playerController.MetersRun > 700 && (BestScore < 900)) 
		{
			XPValue = (playerController.MetersRun - 700) /200;
			XPSlide.value = XPValue;
			widget.color = new Color(0.23921568627450980392156862745098f,0.85882352941176470588235294117647f,0.93333333333333333333333333333333f,1f);
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore =  playerController.MetersRun;
			}
		}
		if (playerController.MetersRun < 1150 && playerController.MetersRun > 900 && (BestScore < 1150)) 
		{
			XPValue = (playerController.MetersRun - 900) /250;
			XPSlide.value = XPValue;
			widget.color = new Color(0.43529411764705882352941176470588f,0.23921568627450980392156862745098f,0.93333333333333333333333333333333f,1f);
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore =  playerController.MetersRun;
			}
		}
		if (playerController.MetersRun < 1400 && playerController.MetersRun > 1150 && (BestScore < 1400)) 
		{
			XPValue = (playerController.MetersRun - 1150) /250;
			XPSlide.value = XPValue;
			widget.color = new Color (0.18431372549019607843137254901961f,0.88627450980392156862745098039216f,0.49803921568627450980392156862745f,1f);
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore = playerController.MetersRun;
			}
		}
		if (playerController.MetersRun < 1700 && playerController.MetersRun > 1400 && (BestScore < 1700)) 
		{
			XPValue = (playerController.MetersRun - 1400) /300;
			XPSlide.value = XPValue;
			widget.color = new Color (0.94901960784313725490196078431373f,0.67843137254901960784313725490196f,0.10196078431372549019607843137255f,1f);
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore =  playerController.MetersRun;
			}
		}
		if (playerController.MetersRun < 2000 && playerController.MetersRun > 1700 && (BestScore < 2000)) 
		{
			XPValue = (playerController.MetersRun - 1700) /300;
			XPSlide.value = XPValue;
			widget.color = new Color(0.69411764705882352941176470588235f,0.93333333333333333333333333333333f,0.23921568627450980392156862745098f,1f);
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore =  playerController.MetersRun;
			}
		}
		if (playerController.MetersRun < 2400 && playerController.MetersRun > 2000 && (BestScore < 2400)) 
		{
			XPValue = (playerController.MetersRun - 2000) /400;
			XPSlide.value = XPValue;
			widget.color = new Color(0.94509803921568627450980392156863f,0.25098039215686274509803921568627f,0.93725490196078431372549019607843f,1f);
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore =  playerController.MetersRun;
			}
		}
		if (playerController.MetersRun < 2900 && playerController.MetersRun > 2400 && (BestScore < 2900)) 
		{
			XPValue = (playerController.MetersRun - 2400) /500;
			XPSlide.value = XPValue;
			widget.color = new Color(1f,0.47450980392156862745098039215686f,0.25882352941176470588235294117647f,1f);
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore = playerController.MetersRun;
			}
		}
		if (playerController.MetersRun < 3500 && playerController.MetersRun > 2900 && (BestScore < 3500)) 
		{
			XPValue = (playerController.MetersRun - 2900) /600;
			XPSlide.value = XPValue;
			widget.color = new Color(0.83529411764705882352941176470588f,0.62352941176470588235294117647059f,0.05882352941176470588235294117647f,1f);
			if (BestScore <  playerController.MetersRun)
			{ 
				BestScore =  playerController.MetersRun;
			}
		}


	}
	

	public void XPControllerBetweenTitels()
	{

			
		if (playerController.MetersRun < 100 && playerController.MetersRun > 0 && (PlayersXPStatus == "Yeni koltuk" )) 
		{
			XPValue = (playerController.MetersRun ) /100;
			XPSlide.value = XPValue;
			widget.color = Color.yellow;
		}
		if (playerController.MetersRun < 200 && playerController.MetersRun > 0 && (PlayersXPStatus == "Yükselen yıldız" )) 
		{
			XPValue = (playerController.MetersRun) /200;
			XPSlide.value = XPValue;
			widget.color = Color.blue;
		}
		if (playerController.MetersRun < 350 && playerController.MetersRun > 0 && (PlayersXPStatus == "Meteor")) 
		{
			XPValue = (playerController.MetersRun) /350;
			XPSlide.value = XPValue;
			widget.color = Color.red;
		}
		if (playerController.MetersRun < 500 && playerController.MetersRun > 0 && (PlayersXPStatus == "Lobici" )) 
		{
			XPValue = (playerController.MetersRun ) /500;
			XPSlide.value = XPValue;
			widget.color = Color.cyan;
	
		}
		if (playerController.MetersRun < 700 && playerController.MetersRun > 0 && (PlayersXPStatus == "Yeni üye")) 
		{
			XPValue = (playerController.MetersRun) /700;
			XPSlide.value = XPValue;
			widget.color = Color.magenta;
		}
		if (playerController.MetersRun < 900 && playerController.MetersRun > 0 && (PlayersXPStatus == "Parti üyesi")) 
		{
			XPValue = (playerController.MetersRun) /900;
			XPSlide.value = XPValue;
			widget.color = new Color(0.23921568627450980392156862745098f,0.85882352941176470588235294117647f,0.93333333333333333333333333333333f,1f);
		}
		if (playerController.MetersRun < 1150 && playerController.MetersRun > 0 && (PlayersXPStatus == "Başbakan")) 
		{
			XPValue = (playerController.MetersRun ) /1150;
			XPSlide.value = XPValue;
			widget.color = new Color(0.43529411764705882352941176470588f,0.23921568627450980392156862745098f,0.93333333333333333333333333333333f,1f);
			
		}
//		if (playerController.MetersRun < 1400 && playerController.MetersRun > 0 && (PlayersXPStatus == "אסיכל דומצ")) 
//		{
//			XPValue = (playerController.MetersRun) /1400;
//			XPSlide.value = XPValue;
//			widget.color = new Color (0.18431372549019607843137254901961f,0.88627450980392156862745098039216f,0.49803921568627450980392156862745f,1f);
//		}
//		if (playerController.MetersRun < 1700 && playerController.MetersRun > 0 && (PlayersXPStatus == "!הנפתמ אל")) 
//		{
//			XPValue = (playerController.MetersRun) /1700;
//			XPSlide.value = XPValue;
//			widget.color = new Color (0.94901960784313725490196078431373f,0.67843137254901960784313725490196f,0.10196078431372549019607843137255f,1f);
//		}
//		if (playerController.MetersRun < 2000 && playerController.MetersRun > 0 && (PlayersXPStatus == "סרפ ןועמש")) 
//		{
//			XPValue = (playerController.MetersRun ) /2000;
//			XPSlide.value = XPValue;
//			widget.color =  new Color(0.69411764705882352941176470588235f,0.93333333333333333333333333333333f,0.23921568627450980392156862745098f,1f);
//			
//		}
//		if (playerController.MetersRun < 2400 && playerController.MetersRun > 0 && (PlayersXPStatus == "Master")) 
//		{
//			XPValue = (playerController.MetersRun) /2400;
//			XPSlide.value = XPValue;
//			widget.color = new Color(0.94509803921568627450980392156863f,0.25098039215686274509803921568627f,0.93725490196078431372549019607843f,1f);
//		}
//		if (playerController.MetersRun < 2900 && playerController.MetersRun > 0 && (PlayersXPStatus == "Running God")) 
//		{
//			XPValue = (playerController.MetersRun) /2900;
//			XPSlide.value = XPValue;
//			widget.color = new Color(1f,0.47450980392156862745098039215686f,0.25882352941176470588235294117647f,1f);
//		}
//		if (playerController.MetersRun < 3500 && playerController.MetersRun > 0 && (PlayersXPStatus == "Master of Universe")) 
//		{
//			XPValue = (playerController.MetersRun) /3500;
//			XPSlide.value = XPValue;
//			widget.color = new Color(0.83529411764705882352941176470588f,0.62352941176470588235294117647059f,0.05882352941176470588235294117647f,1f);
//		}
//

	}

	
	void XPLabelController()
	{
		float mBestScore = BestScore;
		
		if (mBestScore < 50 && mBestScore > 0) {
			XPLabel.text = "Yeni koltuk";
			NextBestScore = 100f;
			//PlayerPrefs.SetFloat("NextBestScore" , 100f);
		}
		if (mBestScore < 100 && mBestScore > 50) {
			XPLabel.text = "Yükselen yıldız";
			NextBestScore = 200f;
			//PlayerPrefs.SetFloat("NextBestScore" , 200f);
		}
		if (mBestScore < 200 && mBestScore > 100) {
			XPLabel.text = "Meteor";
			NextBestScore = 350f;
			//PlayerPrefs.SetFloat("NextBestScore" , 350f);
		}
		if (mBestScore < 350 && mBestScore > 200) {
			XPLabel.text = "Lobici";
			NextBestScore = 500f;
			//PlayerPrefs.SetFloat("NextBestScore" , 500f);
		}
		if (mBestScore < 500 && mBestScore > 350) {
			XPLabel.text = "Yeni üye";
			NextBestScore = 700f;
			//PlayerPrefs.SetFloat("NextBestScore" , 700f);
		}
		if (mBestScore < 700 && mBestScore > 500) {
			XPLabel.text = "Parti üyesi";
			NextBestScore = 900f;
			//PlayerPrefs.SetFloat("NextBestScore" , 900f);
		}
		if (mBestScore < 900 && mBestScore > 700) {
			XPLabel.text = "Başbakan";
			NextBestScore = 1150f;
			//PlayerPrefs.SetFloat("NextBestScore" , 1150f);
		}
		if (mBestScore < 1150 && mBestScore > 900) {
			//XPLabel.text = "אסיכל דומצ";
			NextBestScore = 1400f;
			//PlayerPrefs.SetFloat("NextBestScore" , 1400f);
		}
		if (mBestScore < 1400 && mBestScore > 1150) {
			//XPLabel.text = "!הנפתמ אל";
			NextBestScore = 1700f;
			//PlayerPrefs.SetFloat("NextBestScore" , 1700f);
		}
		if (mBestScore < 1700 && mBestScore > 1400) {
			//XPLabel.text = "סרפ ןועמש";
			NextBestScore = 2000f;
			//PlayerPrefs.SetFloat("NextBestScore" , 2000f);
		}
		if (mBestScore < 2000 && mBestScore > 1700) {
			//XPLabel.text = "Experienced";
			NextBestScore = 2400f;
			//PlayerPrefs.SetFloat("NextBestScore" , 2400f);
		}
		if (mBestScore < 2400 && mBestScore > 2000) {
			//XPLabel.text = "Master";
			NextBestScore = 2900f;
			//PlayerPrefs.SetFloat("NextBestScore" , 2900f);
		}
		if (mBestScore < 2900 && mBestScore > 2400) {
			//XPLabel.text = "Running God";
			NextBestScore = 3500f;
			//PlayerPrefs.SetFloat("NextBestScore" , 3500f);
		}
		if (mBestScore < 3500 && mBestScore > 2900) {
			//XPLabel.text = "Master of Universe";
			NextBestScore = 3500;
			//PlayerPrefs.SetFloat("NextBestScore" , 200f);
		}
		
	}
	

	
	public void TellGooglePlayXPAchivments()
	{
		float mBestScore = PlayerPrefs.GetFloat ("BestScore");
		
		if (mBestScore < 100 && mBestScore > 50) {
			
			#if (UNITY_ANDROID || UNITY_IPHONE)
			Social.ReportProgress(New_Seat , 100, null);
			#endif
			
		}
		if (mBestScore < 200 && mBestScore > 100) {
			
			#if (UNITY_ANDROID || UNITY_IPHONE)
			Social.ReportProgress(Rising_Star , 100, null);
			#endif
			;
		}
		if (mBestScore < 350 && mBestScore > 200) {
			
			#if (UNITY_ANDROID || UNITY_IPHONE)
			Social.ReportProgress(Meteor , 100, null);
			#endif
			
		}
		if (mBestScore < 500 && mBestScore > 350) {
			
			#if (UNITY_ANDROID || UNITY_IPHONE)
			Social.ReportProgress(Lobbist  , 100, null);
			#endif
			
		}
		if (mBestScore < 700 && mBestScore > 500) {
			
			#if (UNITY_ANDROID || UNITY_IPHONE)
			Social.ReportProgress(New_Member  , 100, null);
			#endif
			
		}
		if (mBestScore < 900 && mBestScore > 700) {
			
			#if (UNITY_ANDROID || UNITY_IPHONE)
			Social.ReportProgress(Party_Member  , 100, null);
			#endif
			
		}
		if (mBestScore < 1150 && mBestScore > 900) {
			
			#if (UNITY_ANDROID || UNITY_IPHONE)
			Social.ReportProgress(Prime_Minister  , 100, null);
			#endif
			
		}
//		if (mBestScore < 1400 && mBestScore > 1150) {
//			
//			#if (UNITY_ANDROID || UNITY_IPHONE)
//			Social.ReportProgress(SEASONED_UNLOCKED  , 100, null);
//			#endif
//			
//		}
//		if (mBestScore < 1700 && mBestScore > 1400) {
//			
//			#if (UNITY_ANDROID || UNITY_IPHONE)
//			Social.ReportProgress(PROFICIENT_UNLOCKED  , 100, null);
//			#endif
//			
//		}
//		if (mBestScore < 2000 && mBestScore > 1700) {
//			
//			#if (UNITY_ANDROID || UNITY_IPHONE)
//			Social.ReportProgress(EXPERIENCED_UNLOCKED  , 100, null);
//			#endif
//			
//		}
//		if (mBestScore < 2400 && mBestScore > 2000) {
//			
//			#if (UNITY_ANDROID || UNITY_IPHONE)
//			Social.ReportProgress(MASTER_UNLOCKED  , 100, null);
//			#endif
//			
//		}
//		if (mBestScore < 2900 && mBestScore > 2400) {
//			
//			#if (UNITY_ANDROID || UNITY_IPHONE)
//			Social.ReportProgress(RUNNING_GOD_UNLOCKED  , 100, null);
//			#endif
//			
//		}
//		if (mBestScore < 3500 && mBestScore > 2900) {
//			
//			#if (UNITY_ANDROID || UNITY_IPHONE)
//			Social.ReportProgress(MASTER_OF_UNIVERSE_UNLOCKED  , 100, null);
//			#endif
//			
//		}
//		
	}



	
	IEnumerator WaitBefor ()
	{
		yield return new WaitForSeconds (1);
	}


	//
	//
	//
	void OnPauseMenuClosed() {
		PauseMenuController menu = PauseMenu.GetComponent<PauseMenuController>();
		menu.Closed -= OnPauseMenuClosed;
		UpdateButton(!AppManager.enableSound);
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
	void OnEnable()
	{
		if(Analytics.gua != null) Analytics.gua.sendAppScreenHit("Main Menu");
		UpdateButton(!AppManager.enableSound);
	}
}
