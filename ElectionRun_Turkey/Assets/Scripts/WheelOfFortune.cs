using UnityEngine;
using System.Collections;


public class WheelOfFortune : MonoBehaviour {

	public GameObject Wheel;
	public int WheelSpeed ;
	string BonusName;
	string BonusTag;
	string RuffledBonusName;
	public UITweener StickTween;
	public GameObject StarParticle;
	public GameObject BonusScreen;
	public GameObject ClainPrizeBTN;
	public GameObject StoreGUI;
	public UILabel Massege;
	public UILabel YouWonLabel;
	public Config mGameData;
	public StoreController storeController;
	string DidWonHat;
	public UISprite HatSprite;
	public GameObject BonusA;
	public GameObject WheelOfFortuneOBG;
	public UISprite ComeBackTommorow;
	public GameObject PopUpPanel;
	public GameObject StopBTN;
	public UILabel CoinsLabelinUpgrade;
	public MyGameCenterScript myGameCenterScript;


	void Start()
	{
		upgradeScreenController.AssignObjects ();

		if (PlayerPrefs.GetString ("DidGetHat1") == null)
		{
		PlayerPrefs.SetString ("DidGetHat1", "No");
		}

		if (PlayerPrefs.GetString ("DidGetHat1") == "No") 
		{
			HatSprite.spriteName = "big-hat";
		}
		if (PlayerPrefs.GetString ("DidGetHat1") == "Yes") 
		{
			HatSprite.spriteName = "big-1000-coins";
		}

	}
	




	void OnTriggerEnter(Collider other)   {

	//	print ("Enter  " + other.gameObject.name);
		StickTween.PlayForward ();
	}

	
	void OnTriggerExit(Collider other)   {

		BonusName = other.gameObject.name;
		BonusTag = other.gameObject.tag;
	//	print ("Exit   "  + other.gameObject.name);
		StickTween.PlayReverse ();

		
	}
//	
	void Raffle ()
	{

		int DeviceDay = System.DateTime.Now.DayOfYear;
		int RuffledDay = Random.Range(1,31);
		int RandomBounusA = Random.Range(1,1000);

		if (RandomBounusA != 1)

		{

			if (RuffledDay == 1 || RuffledDay == 3 || RuffledDay == 5 || RuffledDay == 11 || RuffledDay == 15 || RuffledDay == 17 || RuffledDay == 20 || RuffledDay == 21 || RuffledDay == 23 || RuffledDay == 28 || RuffledDay == 30)
			{
				RuffledBonusName = "BonusB";
			}
			else
			{
				RuffledBonusName = "BonusC";
			}
		}
		if (RandomBounusA == 1) 
		{
			RuffledBonusName = "BonusA";
		}

	}

	public void SlowWheel()
	{
		StopBTN.SetActive (false);
		StartCoroutine(SlowWheelTimer());
		Raffle ();
	}

	IEnumerator SlowWheelTimer ()
	{

		for(int i = 0; i < 3 ; i++)
		{
			WheelSpeed -= 5;
			yield return new WaitForSeconds(1);
		}

	}


	void Update () {
		
		Wheel.transform.Rotate(0, 0, 10 * Time.deltaTime * WheelSpeed);
		if (WheelSpeed == 5)
		{
			StopWheel();
		}
	}


	IEnumerator WaitForSecond ()
	{

			yield return new WaitForSeconds(1);
			ComeBackTommorow.enabled = true;
			//ComeBackTommorowGlow.SetActive(true);
		
	}

	public AddedFuncionallity addedFuncionallity;

	public void StopWheel()
	{
		WheelOfFortuneOBG.rigidbody.isKinematic = true;

		if (RuffledBonusName == BonusName) 
		{
			WheelSpeed = 0;
			PopUpPanel.SetActive(true);
			StarParticle.SetActive(true);
			ClainPrizeBTN.SetActive(true);
			Massege.text = BonusTag;
			Massege.enabled = true;
			YouWonLabel.enabled = true;
			StartCoroutine(WaitForSecond());
		}


	}
	public UpgradeScreenController upgradeScreenController;
	public MainMenuController mainMenuController;
	//public UpgradeScreenController upgradeScreenController;
	//public LevelGenerator levelGenerator;

	public void CloseBonusScreen()
	{

		mainMenuController.OnClickUpgrade ();
		upgradeScreenController.Display();

		if (BonusTag == "250 Coins")
		{
			mGameData.Coins += 250;
			upgradeScreenController.BloatCoinsFromBonusScreen();
		}
		if (BonusTag == "Jump Upgrade")
		{
			upgradeScreenController.AddJumpHightFromBonusScreen();
		}
		if (BonusTag == "Bubble Shield")
		{
			upgradeScreenController.AddBubbleShieldFromBonusScreen();
		}
		if (BonusTag == "100 Coins")
		{
			mGameData.Coins += 100;
			upgradeScreenController.BloatCoinsFromBonusScreen();
		}
		if (BonusTag == "Bubble Shield + 100 Coins")
		{
			mGameData.Coins += 100;
			upgradeScreenController.BloatCoinsFromBonusScreen();
			upgradeScreenController.AddBubbleShieldFromBonusScreen();
		}
		if (BonusTag == "500 Coins")
		{
			mGameData.Coins += 500;
			upgradeScreenController.BloatCoinsFromBonusScreen();
		}
		if (BonusTag == "Double Jump")
		{
			upgradeScreenController.AddDoubleJumpHightFromBonusScreen();
		}
		if (BonusTag == "Royal Crown")
		{
			#if (UNITY_ANDROID || UNITY_IPHONE)

			//storeController.WonHatFromBonusScreen();
			PlayerPrefs.SetString("DidGetHat1", "Yes");
			BonusA.tag = "1000 Coins";
			upgradeScreenController.BloatCoinsFromBonusScreen();

			#endif
		}
		if (BonusTag == "1000 Coins")
		{
			mGameData.Coins += 1000;
			upgradeScreenController.BloatCoinsFromBonusScreen();
			addedFuncionallity.UpdateUpgradesIcon();
		}

		addedFuncionallity.UpdateHatsIcon();
		addedFuncionallity.UpdateUpgradesIcon();


		CoinsLabelinUpgrade.text = mGameData.Coins.ToString();



		RestartWheel();
		BonusScreen.SetActive(false);

		PlayerPrefs.SetInt("SavedBonusTime2",System.DateTime.Now.DayOfYear);

		#if UNITY_ANDROID
		
		AppManager.instance.ConnectToGooglePlay();
		# endif
		
		#if UNITY_IPHONE
		myGameCenterScript.ConnectToGameCenter();
		#endif



	}


	void RestartWheel()
	{
		WheelSpeed = 20;
		StarParticle.SetActive(false);
		ClainPrizeBTN.SetActive(false);
		Massege.text = BonusTag;
		Massege.enabled = false;
		YouWonLabel.enabled = false;
		ComeBackTommorow.enabled = false;
		//ComeBackTommorowGlow.SetActive(false);
		StopBTN.SetActive (true);
	}

}
