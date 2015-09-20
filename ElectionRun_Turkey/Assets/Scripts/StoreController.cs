using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;
using OnePF;

//
//
//
//public class HatInfo
//{
//	public string Name;
//	public int Price;
//	public string PrefabName;
//	public string SpriteName;
//	public string SkuID;
//}

/**
 * 
 */
public class StoreController : MonoBehaviour
{
	//
	//
	//
	public const int EntryStartY = 119;
	public const int EntryOffsetY = 119;
//	public const string RegularHat = "RegularHat";
//	public static Color EquippedHatButtonColor = new Color(37/255f, 192/255f, 222/255f);
//	public const int MaxHatHeight = 90;
	public const int EquipMarkerX = 75;
	public const int EquipMarkerY = 73;

	//
	public GameObject ProductEntryPrefab;
	//public GameObject HatEntryPrefab;
	public event System.EventHandler<System.EventArgs> Closed = delegate {};

	//
	//
	//
	GameObject mCoinGrid;
//	GameObject mHatGrid;
//	GameObject mEquippedHatMarker;
	UILabel mCoinLbl;
//	static List<HatInfo> mHats;
//	bool mHatsLoaded = false;
	bool mGridInited = false;

	// Hat to buy
//	string mHatSkuToBuy;
//	string mHatNameToBuy;
//	GameObject mHatButton;

	// Coins pack to buy
	string mCoinsSkuToBuy;

	#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
	bool mInitialized = false;
	#endif

	//
	//
	//
	public void OnClose()
	{
		Closed(this, System.EventArgs.Empty);
		this.gameObject.SetActive(false);
		
	}

	//
	//
	//
//	public void OnOpenHatStore()
//	{
//		mCoinGrid.transform.parent.gameObject.SetActive(false);
//		//mHatGrid.transform.parent.gameObject.SetActive(true);
//
//		// Analytics
//		//Analytics.gua.sendAppScreenHit("Hats Store");
//		AppManager.HideBannerAd();
//	}

	//
	//
	//
	public void OnOpenCoinStore()
	{
		mCoinGrid.transform.parent.gameObject.SetActive(true);
		//mHatGrid.transform.parent.gameObject.SetActive(false);

		// Analytics
		Analytics.gua.sendAppScreenHit("Coin Store");
		//AppManager.HideBannerAd();
	}

	/**
	 * 
	 */
	public void AddCoinPack(string id, string name, string price)
	{

		print(id + name + price);


		GameObject entry = Instantiate(ProductEntryPrefab) as GameObject;
		Transform transf = entry.transform;
		transf.parent = mCoinGrid.transform;
		transf.localPosition = Vector3.zero;
		transf.localScale = Vector3.one;

		// Name
		UILabel label = entry.transform.FindChild("Name").gameObject.GetComponent<UILabel>();

		//name.Substring (8);
		label.text = name.Substring(0, 8).Trim();
	
		//label.text = name.Substring(name.Length - 8); // s2=="docs";


		//label.text = name
	

		// Price
		label = entry.transform.FindChild("Price").gameObject.GetComponent<UILabel>();
		label.text = price;

		entry.name = id;
		entry.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnSelectProduct));
	}

	//
	//
	//
//	public void AddHat(string name, string price, string spriteName, string skuID = null)
//	{
//		GameObject entry = Instantiate(HatEntryPrefab) as GameObject;
//		Transform transf = entry.transform;
//		transf.parent = mHatGrid.transform;
//		transf.localPosition = Vector3.zero;
//		transf.localScale = Vector3.one;
//
//		UILabel label = entry.transform.FindChild("Name").GetComponent<UILabel>();
//		label.text = name;
//
//		UILabel priceLbl;
//		if (skuID == null) {
//			priceLbl = entry.transform.FindChild("Price").GetComponent<UILabel>();
//		}
//		else {
//			priceLbl = entry.transform.FindChild("PremiumPrice").GetComponent<UILabel>();
//		}
//		priceLbl.text = price;
//
//		UI2DSprite sprite = entry.transform.FindChild("Hat").GetComponent<UI2DSprite>();
//		sprite.sprite2D = Multires.GetSprite(spriteName);
//
//		// Hat sprite scaling
//		float scale = 1;
//	
//		float width = Screen.width > Screen.height ? Screen.width : Screen.height;
//		int myWidth = (int)sprite.sprite2D.textureRect.width;
//		int myHeight = (int)sprite.sprite2D.textureRect.height;
//
//		if (width >= Multires.instance.MaxWidth) {
//			myWidth = myWidth/2;
//			myHeight = myHeight/2;
//		}
//
//		if (myHeight > MaxHatHeight) scale = MaxHatHeight / (float)myHeight;
//	//	sprite.width = (int)(scale * sprite.sprite2D.texture.width);
//	//	sprite.height = (int)(scale * sprite.sprite2D.texture.height);
//
//		sprite.width = (int)(scale * myWidth); 
//		sprite.height = (int)(scale * myHeight);
//
//		if (skuID == null)
//		{
//			entry.name = RegularHat;
//			//my change
//			entry.tag = "RegularHat";
//		}
//		else
//		{
//			entry.name = skuID;
//		}
//
//		UpdateHatGUI(entry);
//		entry.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnSelectHat));
//	}

	//
	//
	//
//	public static void LoadHats()
//	{
//		if (mHats != null)
//		{
//			return;
//		}
//		
//		string xmlText = (Resources.Load("hats", typeof(TextAsset)) as TextAsset).text;
//		
//		XmlDocument doc = new XmlDocument();
//		doc.LoadXml(xmlText);
//		
//		mHats = new List<HatInfo>();
//		HatInfo hat;
//		
//		foreach(XmlElement hatNode in doc.SelectNodes("hats/hat"))
//		{
//			hat = new HatInfo();
//			hat.Name = hatNode.GetAttribute("name");
//			hat.Price = int.Parse( hatNode.GetAttribute("price") );
//			hat.PrefabName = hatNode.GetAttribute("prefabName");
//			hat.SpriteName = hatNode.GetAttribute("spriteName");
//			hat.SkuID = hatNode.GetAttribute("sku");
//			mHats.Add(hat);
//		}
//
//	}
	
	//
	//
	//
//	public static HatInfo GetHatInfo(string hatName)
//	{
//		foreach(HatInfo info in mHats)
//		{
//			if (info.Name == hatName) return info;
//		}
//
//		return null;
//	}

	//
	//
	//
//	public static int GetNumHats()
//	{
//		return mHats.Count;
//	}
//
//	//
//	//
//	//
//	public static HatInfo GetHatInfoAt(int index)
//	{
//		return mHats[index];
//	}
//
//	//
//	//
//	//
//	public static HatInfo GetHatInfoBySku(string sku)
//	{
//		foreach(HatInfo info in mHats)
//		{
//			if (info.SkuID == sku) return info;
//		}
//		
//		return null;
//	}

	//
	//
	//
	void OnSelectProduct()
	{
		#if (UNITY_ANDROID || UNITY_IPHONE)

		mCoinsSkuToBuy = UIButton.current.gameObject.name;

		// Package name
		UILabel label = UIButton.current.transform.FindChild("Name").GetComponent<UILabel>();
		string itemName = label.text;

		// Package price
		label = UIButton.current.transform.FindChild("Price").GetComponent<UILabel>();
		string price = label.text;

		// Show confirmation dialog
		transform.parent.FindChild("ConfirmationDialog").gameObject.SetActive(true);
		DialogController.instance.Closed += OnConfirmBuyCoins;
		DialogController.instance.SetMessage( "?" + price + " - ב תולוק " + itemName  + " הנק ");

		//Analytics
		Analytics.gua.sendEventHit("Action", itemName);

		/// My change ecommerce
		mitemName = itemName;
		mitemPrice = double.Parse(price);
		#endif
	}

	//
	//
	//
	/// My change ecommerce 
	//public GoogleUniversalAnalytics googleUniversalAnalytics;

	//string transactionID = googleUniversalAnalytics.clientID;
	string mitemName; 
	double mitemPrice;
	int itemQuantity = 1 ;
	string currencyCode = "ILS";
	string itemCategory ;

	void OnConfirmBuyCoins(bool ok)
	{
		// Remove event delegate
		DialogController.instance.Closed -= OnConfirmBuyCoins;

		if (!ok) return;

		#if (UNITY_ANDROID || UNITY_IPHONE)
		OpenIAB.purchaseProduct(mCoinsSkuToBuy);

		/// My change ecommerce 
		Analytics.gua.sendItemHit(mCoinsSkuToBuy, mitemName, mitemPrice, itemQuantity, "null", "Coins", currencyCode);
		#endif
	}




//	void OnSelectHat()
//	{
//		mHatButton = UIButton.current.gameObject;
//
//		// Hat's sku
//		mHatSkuToBuy = UIButton.current.gameObject.name;
//
//		// Hat's name
//		UILabel label = UIButton.current.transform.FindChild("Name").GetComponent<UILabel>();
//		mHatNameToBuy = label.text;
//
//		// Hat's information
//		HatInfo hatInfo = GetHatInfo(mHatNameToBuy);
//		string message = null;
//
//		// Analytics
//		Analytics.gua.sendEventHit("Action", mHatNameToBuy);
//
//		if (mHatSkuToBuy == RegularHat) {
//
//			// Regular hats
//
//			if (Config.instance.OwnsHat(hatInfo.Name))  {
//
//				// Owns hat, equip it
//				EquipHat(hatInfo.Name);
//			}
//			else {
//
//				if (Config.instance.Coins < hatInfo.Price) {
//
//					// Insufficient coins
//
//					#if (UNITY_ANDROID || UNITY_IPHONE)
//					transform.parent.FindChild("ConfirmationDialog").gameObject.SetActive(true);
//					DialogController.instance.SetMessage("You don't have enough coins. Buy more coins?");
//					DialogController.instance.Closed += OnConfirmOpenCoinShop;
//					#endif
//					//Analytics
//					Analytics.gua.sendAppScreenHit("No Coins Panel");
//
//					return;
//				}
//				else {
//
//					message = "Buy '" + mHatNameToBuy + "' for " + hatInfo.Price.ToString() + " coins?";
//				}
//			}
//		}
//		else {
//
//			// Premium hats
//
//			if (Config.instance.OwnsHat(hatInfo.Name)) {
//
//				// Owns hat, equip it
//				EquipHat(hatInfo.Name);
//			}
//			else {
//
//				UILabel priceLbl = UIButton.current.transform.FindChild("PremiumPrice").GetComponent<UILabel>();
//				message = "Buy '" + mHatNameToBuy + "' for " + priceLbl.text + "?";
//			}
//		}
//
//		if (message != null) {
//
//			// Show purchase confirmation dialog
//			transform.parent.FindChild("ConfirmationDialog").gameObject.SetActive(true);
//			DialogController.instance.Closed += OnConfirmBuyHat;
//			DialogController.instance.SetMessage(message);
//		}
//	}

	//
	//
	//
//	void OnConfirmBuyHat(bool ok)
//	{
//		// Remove event delegate
//		DialogController.instance.Closed -= OnConfirmBuyHat;
//
//		if (!ok) return;
//
//		if (mHatSkuToBuy == RegularHat)
//		{
//			//
//			// Buy regular hat with coins
//			//
//
//			HatInfo hatInfo = GetHatInfo(mHatNameToBuy);
//			
//			if (Config.instance.Coins >= hatInfo.Price)
//			{
//				// Buy hat
//				Config.instance.AddOwnedHat(hatInfo.Name);
//				Config.instance.Coins -= hatInfo.Price;
//				UpdateCoinsLabel();
//				
//				// Equip hat
//				EquipHat(hatInfo.Name);
//			}
//			
//			UpdateHatGUI(mHatButton);
//		}
//		else
//		{
//			//
//			// Buy premium hat with real money
//			//
//			Debug.Log ("Purchase hat: " + mHatSkuToBuy);
//			
//			HatInfo hatInfo = GetHatInfoBySku(mHatSkuToBuy);
//			
//			if ( !Config.instance.OwnsHat(hatInfo.Name) ) {
//				OpenIAB.purchaseProduct(mHatSkuToBuy);
//
//				/// My change ecommerce 
//				Analytics.gua.sendItemHit(mHatSkuToBuy, hatInfo.Name, hatInfo.Price, itemQuantity, "null", "Coins", currencyCode);
//
//			}
//		}
//	}

	//
	//
	//
	void OnConfirmOpenCoinShop(bool ok)
	{
		// Remove event delegate
		DialogController dialog = transform.parent.FindChild("ConfirmationDialog").GetComponent<DialogController>();
		dialog.Closed -= OnConfirmOpenCoinShop;

		if (ok) {
			//mHatGrid.transform.parent.gameObject.SetActive(false);
			mCoinGrid.transform.parent.gameObject.SetActive(true);
			//Analytics
			Analytics.gua.sendEventHit("Action", "No Coins Panel_Yes");
		}
		if (!ok) {

			Analytics.gua.sendEventHit("Action", "No Coins Panel_No");

		}
	}

	//
	//
	//
//	void EquipHat(string hatName)
//	{
//		GameObject curEquippedHat = null;
//		if(Config.instance.CurrentHat.Length > 0) curEquippedHat = FindHatEntry(Config.instance.CurrentHat);
//
//		Config.instance.CurrentHat = hatName;
//		if(curEquippedHat != null) UpdateHatGUI(curEquippedHat);
//
//		GameObject hatBtn = FindHatEntry(hatName);
//		if (hatBtn != null) UpdateHatGUI(hatBtn);
//	}

	/**
	 * 
	 */
	void Awake()
	{

		//my change

		Transform transf = transform;

		mCoinGrid = transf.Find("CoinScrollView/CoinGrid").gameObject;
	//	mHatGrid = transf.Find("HatScrollView/HatGrid").gameObject;
		mCoinLbl = transf.Find("Coin").GetComponent<UILabel>();
	//	mEquippedHatMarker = transform.Find("HatScrollView/EquippedHatMarker").gameObject;
	//	mEquippedHatMarker.SetActive(false);
		AppManager.instance.challengeActivated += OnChallengeActivated;

		#if UNITY_WEBPLAYER
		mCoinGrid.transform.parent.gameObject.SetActive(false);
		#else
	//	mHatGrid.transform.parent.gameObject.SetActive(false);
		#endif

		#if (UNITY_ANDROID || UNITY_IPHONE)
		OpenIABEventManager.queryInventorySucceededEvent += OnQueryInventorySucceeded;
		OpenIABEventManager.queryInventoryFailedEvent += OnQueryInventoryFailed;
		OpenIABEventManager.purchaseSucceededEvent += OnPurchaseSucceeded;
		OpenIABEventManager.purchaseFailedEvent += OnPurchaseFailed;
		OpenIABEventManager.consumePurchaseSucceededEvent += OnConsumeSucceeded;
		OpenIABEventManager.consumePurchaseFailedEvent += OnConsumeFailed;
		#endif

		LevelGenerator.CloseStore ();
	}

	/**
	 * 
	 */
	void OnEnable()
	{
		//AppManager.HideBannerAd();
		stars.SetActive (false);
		stars.GetComponent<UISpriteAnimation> ().Reset ();
		mCoinLbl.text = Config.instance.Coins.ToString();

//		if (!mHatsLoaded)
//		{
//			// Add hats to gui
//			foreach(HatInfo info in mHats)
//			{
//				if(info.Price != 0) AddHat(info.Name, info.Price.ToString(), info.SpriteName);
//			}
//
//			mHatsLoaded = true;
//		}
		
		#if UNITY_EDITOR
			
		EnableGUI(true);

		#elif (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR

		if (!mInitialized)
		{
			EnableGUI(true);
			PopulateStore();
		}

		#endif

		#if UNITY_WEBPLAYER
		transform.FindChild("Panel/GoldButton").gameObject.SetActive(false);
		transform.FindChild("Panel/HatButton").gameObject.SetActive(false);
		#endif

		UIPlaySound sound = GetComponent<UIPlaySound>();
		if(sound != null) sound.Play();

		// Analytics
		if(Analytics.gua != null) Analytics.gua.sendAppScreenHit("Store");
	}

	/**
	 * 
	 */
	void OnDisable()
	{
		//AppManager.ShowBannerAd();
	}

	/**
	 * 
	 */
	void EnableGUI(bool bEnable)
	{
		transform.FindChild("Panel/ExitButton").GetComponent<UIButton>().isEnabled = bEnable;
	}

	//
	//
	//
//	void FixedUpdate() {
//		if (!mGridInited)
//		{
//			//UIPanel panel = transform.Find("HatScrollView").GetComponent<UIPanel>();
//			//Vector2 size = panel.GetViewSize();
//			//UIGrid hatGrid = mHatGrid.GetComponent<UIGrid>();
//			//hatGrid.maxPerLine = (int)(size.x / HatEntryPrefab.GetComponent<UISprite>().width);
//			mGridInited = false;
//		}
//		//my change
//		//LeftHatsToBuy ();
//	}

	//
	//
	//
	/// <summary>
	/// my change	/// </summary>
	public GameObject CoinGrid;
//	public GameObject HatGrid;
	public UIButton ShowCoinBTN;
	//public UIButton ShowHatBTN;

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			OnClose();
		}

		/// my change
		if (CoinGrid.activeSelf == true)
		{
			ShowCoinBTN.isEnabled = false;
			//ShowHatBTN.isEnabled = true;
		}
//		if (HatGrid.activeSelf == true)
//		{
//			ShowCoinBTN.isEnabled  = true;
//			//ShowHatBTN.isEnabled  = false;
//		}

	}
	


	//
	//
	//
//	GameObject FindHatEntry(string name)
//	{
//		int i;
//		Transform transf = mHatGrid.transform;
//		GameObject entry;
//		UILabel label;
//
//		for(i=0; i<transf.childCount; ++i)
//		{
//			entry = transf.GetChild(i).gameObject;
//			label = entry.transform.FindChild("Name").GetComponent<UILabel>();
//
//			if (label != null && label.text == name)
//			{
//				return entry;
//			}
//		}
//
//		return null;
//	}
//	

//	void UpdateHatGUI(GameObject hatEntry)
//	{
//		UILabel label = hatEntry.transform.FindChild("Name").GetComponent<UILabel>();
//		string hatName = label.text;
//
//		label = hatEntry.transform.FindChild("Status").GetComponent<UILabel>();
//		GameObject price = hatEntry.transform.FindChild("Price").gameObject;
//		GameObject coin = hatEntry.transform.FindChild("Coin").gameObject;
//		GameObject premiumPrice = hatEntry.transform.FindChild("PremiumPrice").gameObject;
//
//		Color color = Color.white;
//
//		if (Config.instance.CurrentHat == hatName)
//		{
//			label.text = "Equipped";
//			label.gameObject.SetActive(true);
//			price.SetActive(false);
//			coin.SetActive(false);
//			premiumPrice.SetActive(false);
//			color = EquippedHatButtonColor;
//			mEquippedHatMarker.gameObject.SetActive(true);
//			mEquippedHatMarker.transform.parent = hatEntry.transform;
//			mEquippedHatMarker.transform.localPosition = new Vector3(EquipMarkerX, EquipMarkerY, 0);
//		}
//		else if (Config.instance.OwnsHat(hatName))
//		{
//			label.text = "Owned";
//			label.gameObject.SetActive(true);
//			price.SetActive(false);
//			coin.SetActive(false);
//			premiumPrice.SetActive(false);
//		}
//		else
//		{
//			bool bPremium = hatEntry.name != RegularHat;
//			label.gameObject.SetActive(false);
//			premiumPrice.SetActive(bPremium);
//			price.SetActive(!bPremium);
//			coin.SetActive(!bPremium);
//			///my change
//			//LeftHatsToBuyPrices.Add (int.Parse (price.GetComponent<UILabel> ().text));
//		}
//
//		UIButton btn = hatEntry.GetComponent<UIButton>();
//		btn.defaultColor = color;
//		btn.isEnabled = false;
//		btn.isEnabled = true;
//
//	}

	//my change

//	public  List<int> LeftHatsToBuyPrices;
//	public AddedFuncionallity addedFuncionallity;
//
//	public void LeftHatsToBuy()
//	{
//		GameObject[] HatsArray = GameObject.FindGameObjectsWithTag("RegularHat");
//
//		for (int i = 0; i < HatsArray.Length; i++) 
//		{
//
//			string HatState = HatsArray[i].transform.FindChild("Status").GetComponent<UILabel>().text;
//			Transform HatPrice = HatsArray[i].transform.FindChild("price");
//			if (HatPrice.gameObject.activeSelf == true)
//			{
//				LeftHatsToBuyPrices.Add(int.Parse(HatPrice.GetComponent<UILabel>().text));
//				print (LeftHatsToBuyPrices[i]);
//			}
//
//		}
//
//		//addedFuncionallity.UpdateHatsIcon();
//
//	}

//	public void EmptyLeftHatsToBuy()
//	{
//		LeftHatsToBuyPrices.Clear ();
//	}


	//
	//
	//
	void UpdateCoinsLabel()
	{
		mCoinLbl.text = Config.instance.Coins.ToString();
	}

	//
	//
	//
	void OnChallengeActivated(ChallengeDetail challenge)
	{
		if( !gameObject.activeSelf ) return;
		gameObject.SetActive(false);

		if (!LevelGenerator.instance.gameObject.activeSelf) {
			transform.parent.FindChild("MainMenu").GetComponent<MainMenuController>().StartGame(challenge.Name, challenge.Distance, challenge.Hat, challenge.FacebookID, challenge.Character);
		}
	}

	//
	//
	//
	float PriceStringToFloat(string price)
	{
		string result = "";
		
		foreach(char c in price)
		{
			if ((c >= '0' && c <= '9') || (c == ',' || c == '.'))
			{
				result += c;
			}
		}

		return float.Parse(result, System.Globalization.NumberStyles.Currency);
	}
	
	/**
	 * 
	 */
	#if (UNITY_ANDROID || UNITY_IPHONE)
	void OnQueryInventorySucceeded(Inventory inventory)
	{
		OpenIABEventManager.queryInventorySucceededEvent -= OnQueryInventorySucceeded;
		PopulateStore();
		LoadingIndicator.Hide();
		EnableGUI(true);
	}

	void OnQueryInventoryFailed(string error)
	{
		OpenIABEventManager.queryInventoryFailedEvent -= OnQueryInventoryFailed;
	}

	//My Change

//	public void WonHatFromBonusScreen()
//	{
//		HatInfo Bhat = new HatInfo();
//		Bhat.Name = "Royal Crown";
//		Bhat.Price = 100000;
//		Bhat.PrefabName = "crown";
//		Bhat.SpriteName = "crown";
//		Bhat.SkuID = "yepi_crown"; 
//		
//		Config.instance.AddOwnedPremiumHat(Bhat.Name);
//		EquipHat(Bhat.Name);
//	}

	void OnPurchaseSucceeded(Purchase purchase)
	{
		Debug.Log("Purchase succeeded: " + purchase.Sku + "," + purchase.ItemType + "," + purchase.PackageName + "," + purchase.AppstoreName + "," +  BillingManager.instance.inventory.GetSkuDetails(purchase.Sku).Type);

		// Is it a hat?
//		HatInfo hatInfo = GetHatInfoBySku(purchase.Sku);
//
//
//		if (hatInfo != null)
//		{
//			Config.instance.AddOwnedPremiumHat(hatInfo.Name);
//			UpdateHatGUI( FindHatEntry(hatInfo.Name) );
//
//			// Equip hat
//			EquipHat(hatInfo.Name);
//		}
//		else
//		{
			OpenIAB.consumeProduct(purchase);
		//}
	}

	void OnPurchaseFailed(int error, string message)
	{
		Debug.Log("Purchase failed: " + message);
	}

	public GameObject stars;

	void OnConsumeSucceeded(Purchase purchase)
	{
		Debug.Log("Consume succeeded: " + purchase.Sku);

		if (purchase.Sku == BillingManager.SKU_2500_Coins)
		{
			Config.instance.Coins += 2500;
			stars.SetActive(true);
		}
		else if (purchase.Sku == BillingManager.SKU_25000_Coins)
		{
			Config.instance.Coins += 25000;
			stars.SetActive(true);
		}
		else if (purchase.Sku == BillingManager.SKU_75000_Coins)
		{
			Config.instance.Coins += 75000;
			stars.SetActive(true);
		}
		else if (purchase.Sku == BillingManager.SKU_250000_Coins)
		{
			Config.instance.Coins += 250000;
			stars.SetActive(true);
		}
//		else if (purchase.Sku == BillingManager.SKU_80000_Coins)
//		{
//			Config.instance.Coins += 80000;
//		}

		UpdateCoinsLabel();
		Config.instance.Save();
	}

	void OnConsumeFailed(string error)
	{
		Debug.Log("Consume failed: " + error);
	}

	/**
	 * 
	 */
	void OnConnectionFailed(object sender, System.EventArgs args)
	{
		//GameBillingManager.ConnectionFailed -= OnConnectionFailed;
		//AndroidMessage.Create("Connection Error", "Unable to connect to store. Please try again later.");
		OnClose();
		LoadingIndicator.Hide();
		EnableGUI(true);
	}

	//
	//
	//
	int PriceComparer(SkuDetails sku1, SkuDetails sku2)
	{
		return (int)(PriceStringToFloat(sku1.Price) - PriceStringToFloat(sku2.Price));
	}

	/**
	 * 
	 */
	void PopulateStore()
	{
		Inventory inventory = BillingManager.instance.inventory;
		bool bCoinsAdded = false;

		if (inventory == null)
		{
			transform.FindChild("CoinScrollView/ConnectionFailed").gameObject.SetActive(!bCoinsAdded);
			return;
		}

		List<SkuDetails> skus =	inventory.GetAllAvailableSkus();
		string name;
		int index;
		//HatInfo hatInfo;

		// Get list of owned premium hats
		List<string> ownedSkus = inventory.GetAllOwnedSkus();
		GameObject hatBtn;
//		foreach(string sku in ownedSkus)
//		{
//			//Debug.Log("Owned: " + sku);
//			
//			hatInfo = GetHatInfoBySku(sku);
//			if(hatInfo != null)
//			{
//				Config.instance.AddOwnedPremiumHat(hatInfo.Name);
//				hatBtn = FindHatEntry(hatInfo.Name);
//				if(hatBtn != null) UpdateHatGUI(hatBtn);
//			}
//		}

		//Debug.Log("Num Available SKUs: " + skus.Count);
		skus.Sort(PriceComparer);
		//Debug.Log("After sort: " + skus[0].Title);

		foreach(SkuDetails sku in skus) {
			
			Debug.Log("Add SKU: " + sku.Sku);
			
			index = sku.Title.IndexOf(" (Election Run)");
			if (index != -1) name = sku.Title.Substring(0, index);
			else name = sku.Title;
			
			AddCoinPack(sku.Sku, name, sku.Price);
			bCoinsAdded = true;
			Debug.Log(sku.Sku + " , " +  name + " , " +  sku.Price);
			
			//			hatInfo = GetHatInfo(name);
			//
			//			if (hatInfo == null) {
			//				bCoinsAdded = true;
			//				AddCoinPack(sku.Sku, name, sku.Price);
			//			}
			//			else {
			//				//AddHat(name, sku.Price, hatInfo.SpriteName, sku.Sku);
			//			}
		}

		mCoinGrid.GetComponent<UIGrid>().Reposition();
		//mHatGrid.GetComponent<UIGrid>().Reposition();
		transform.FindChild("CoinScrollView/ConnectionFailed").gameObject.SetActive(!bCoinsAdded);

		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		mInitialized = true;
		#endif
	}

	#endif
}
