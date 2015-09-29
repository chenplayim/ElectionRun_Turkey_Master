using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AddedFuncionallity : MonoBehaviour {

	// Use this for initialization

	int SuggestMoreCoins;
	//public static bool ThisIsMoreCoinsDialog;
	public GameObject UpgradeScreen;
	public GameObject Store;
	public GameObject mHatGrid;
	public GameObject mCoinGrid;


	public Config mGameData;

	public UpgradeScreenController upgradeScreenController;

	public UISprite UpgradesIconMainScreen;
	public UISprite UpgradesIconEndScreen;
//	public UISprite UpgradesIconHats1;
//	public UISprite UpgradesIconHats2;


	public StoreController storeController;
	int MinumumHatPrice;
	public GameOverGUIController gameOverGUIController;
	
	//public UIScrollView HatsSCrollView;

	


	void Start () {

		if ((PlayerPrefs.GetInt("DialogPopUp") < 1 ))
		{
			PlayerPrefs.SetInt("DialogPopUp" , 1);
		}

		Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
		//PlayerPrefs.DeleteAll();
		//mGameData.Coins = 450;

		//Game Thrive (Posh Notifications)
//		GameThrive.Init("aaa9fe3c-8b54-11e4-b712-ef2de50f1ba8", "1056438419313", HandleNotification);

		SuggestMoreCoins = 0;

		UpdateHatsIcon();
		UpdateUpgradesIcon();
		//my change Web
		#if (UNITY_WEBPLAYER)

		mHatGrid.transform.localScale = new Vector3(0.7f,0.7f,0.7f);
		UIPanel HatsGridPanel = mHatGrid.GetComponent<UIPanel>();
		HatsGridPanel.clipping = UIDrawCall.Clipping.None;
		mHatGrid.transform.localPosition = new Vector3(0f,-31.54f,0f);
		HatsSCrollView.enabled = false;
		#endif




	}


//	void Update()
//	{
//
//	}


	//Game Thrive (Posh Notifications)
	// Gets called when the player opens the notification.
	private static void HandleNotification(string message, Dictionary<string, object> additionalData, bool isActive) {
	}

	public void BuyMoreCoinsPopUp ( )
	{
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR

		int BubblePrice = mGameData.BubbleUpgradeCosts[mGameData.CurrentBubbleLevel];
		int JumpHeightPrice = mGameData.JumpHeightUpgradeCosts[mGameData.CurrentJumpHeightLevel];
		int DoubleJumpPrice = mGameData.DoubleJumpHeightUpgradeCosts[mGameData.CurrentDoubleJumpHeightLevel];

		int MinumumPrice  =  Mathf.Min(BubblePrice , JumpHeightPrice ,DoubleJumpPrice);

		if ((SuggestMoreCoins == 0)  && (mGameData.Coins < MinumumPrice ) && (PlayerPrefs.GetInt("SuggestUpgrade", 1) == 1) && (PlayerPrefs.GetInt("MinumumHatPrice") != 0) && (mGameData.Coins >= PlayerPrefs.GetInt("MinumumHatPrice"))) {

						
						LevelGenerator.uiRoot.transform.FindChild ("ConfirmationDialog").gameObject.SetActive (true);
						DialogController.instance.SetMessage (" Not Enough Coins, Go to The Shop?");
						DialogController.instance.Closed += OnConfirmOpenCoinShop;
						SuggestMoreCoins = 1;

					//	ThisIsMoreCoinsDialog = true;

				}
		#endif
	}

	void OnStoreClosed(object sender, System.EventArgs args)
	{
		Store.GetComponent<StoreController>().Closed -= OnStoreClosed;
		gameObject.SetActive(true);
	}


	void OnConfirmOpenCoinShop(bool ok)
	{

		DialogController.instance.Closed -= OnConfirmOpenCoinShop;
		
		if (ok) {
			upgradeScreenController.OnOpenStore();

		}

	}


	/////////////Update if can buy upgrade or hat
	public void UpdateUpgradesIcon()
	{

		int BubblePrice = mGameData.BubbleUpgradeCosts[mGameData.CurrentBubbleLevel];
		int JumpHeightPrice = mGameData.JumpHeightUpgradeCosts[mGameData.CurrentJumpHeightLevel];
		int DoubleJumpPrice = mGameData.DoubleJumpHeightUpgradeCosts[mGameData.CurrentDoubleJumpHeightLevel];
		
		int MinumumPrice  =  Mathf.Min(BubblePrice , JumpHeightPrice ,DoubleJumpPrice);


		if (mGameData.Coins >= MinumumPrice || ((PlayerPrefs.GetInt("MinumumHatPrice") != 0) && (mGameData.Coins >= PlayerPrefs.GetInt("MinumumHatPrice"))))
		{
			UpgradesIconMainScreen.enabled = true;
			UpgradesIconEndScreen.enabled = true;
	
		}
		else
		{
			UpgradesIconMainScreen.enabled = false;
			UpgradesIconEndScreen.enabled = false;
		}




	}

	public void UpdateHatsIcon()
	{
	
//		if (PlayerPrefs.GetInt("MinumumHatPrice") != 0)
//		{
//
//			if (mGameData.Coins >= PlayerPrefs.GetInt("MinumumHatPrice"))
//			{
//				UpgradesIconHats1.enabled = true;
//				UpgradesIconHats2.enabled = true;
//			}
//			else
//			{
//				UpgradesIconHats1.enabled = false;
//				UpgradesIconHats2.enabled = false;
//			}
//		}
	
	}


//	public Transform HatsParentTransform;
//	public GameObject HatsParent;
//	public List<int> LeftHatsToBuyPrices;
//	public GameObject[] Hats;



	public void fillHatsArray()
	{
		//Hats = GameObject.FindGameObjectsWithTag("RegularHat");
	}

	public void ClearHatList()
	{
		//LeftHatsToBuyPrices.Clear();
	}

	/// Hast List Called From buttons - Start Button (in main menu)+ exit store (in store)
	public void HatsList ()
	{
//
//			for(int i = 0; i < Hats.Length ; i++)
//			{
//
//				Transform StatusObg = Hats[i].transform.FindChild("Status");
//				string HatsName = Hats[i].transform.FindChild("Price").GetComponent<UILabel>().text;
//				int HatsPrice = int.Parse(HatsName);
//
//					if (StatusObg.gameObject.activeSelf)
//
//						{
//						//print ("Perchased  - " + HatsPrice);
//						}
//
//					else{
//						//print ("NOTPerchased  - " +HatsPrice);
//						LeftHatsToBuyPrices.Add(HatsPrice);
//						}
//
//			}
//
//			if (LeftHatsToBuyPrices.Count > 0)
//			{
//				MinumumHatPrice = Mathf.Min(LeftHatsToBuyPrices.ToArray());
//				PlayerPrefs.SetInt("MinumumHatPrice" , MinumumHatPrice);
//
//			}

		ClearHatList ();
		//}
	}



	/// <summary>
	///  Pouse On PhoneCall
	/// </summary>
	public GameObject HudPanel;
	public bool paused;
	public PauseMenuController pauseMenuController;
//	
//	void OnApplicationPause(bool pauseStatus) {
//		
//		if (HudPanel.activeSelf == true) 
//		{
//				pauseMenuController.OpenPauseMenu ();
//		}
//	}



	
	public GameObject MyConfirmationDialog1;
	public GameObject MyConfirmationDialog2;
	public GameObject MyConfirmationDialog3;
	public int DialogPopUpINT;



	public void PopUpsDialogCon()
	{
		
		DialogPopUpINT = PlayerPrefs.GetInt("DialogPopUp");

//		if (DialogPopUpINT == 1 && (mGameData.Coins > 20))
//		{
//			MyConfirmationDialog2.gameObject.SetActive(true);
//			PlayerPrefs.SetInt("DialogPopUp" , 2);
//
//		}

//		if (DialogPopUpINT == 2) 
//		{	
//			MyConfirmationDialog1.SetActive(true);
//			PlayerPrefs.SetInt("DialogPopUp" , 3);
//		}


//		if (DialogPopUpINT == 3 && (mGameData.Coins > 60))
//		{
//			MyConfirmationDialog3.gameObject.SetActive(true);
//			PlayerPrefs.SetInt("DialogPopUp" , 4);
//
//		}

		
		
	}






}
