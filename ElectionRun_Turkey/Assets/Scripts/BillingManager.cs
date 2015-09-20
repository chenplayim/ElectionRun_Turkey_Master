using UnityEngine;
using OnePF;
using System.Collections.Generic;

public class BillingManager : MonoBehaviour {

	#if (UNITY_IPHONE || UNITY_ANDROID)

	#if (UNITY_ANDROID)
	public const string SKU_2500_Coins = "2500v";
	public const string SKU_25000_Coins = "25000_votes";
	public const string SKU_75000_Coins = "75000_votes";
	public const string SKU_250000_Coins = "250000_votes";

	#endif

	#if (UNITY_IPHONE)
	public const string SKU_2500_Coins = "2500V";
	public const string SKU_25000_Coins = "25000V";
	public const string SKU_75000_Coins = "75000V";
	public const string SKU_250000_Coins = "250000V";

	#endif


	#pragma warning disable 0414
	string _label = "";
	#pragma warning restore 0414

	static BillingManager mInstance = null;
	bool _isInitialized = false;
	Inventory mInventory;

	public static BillingManager instance {
		get { return mInstance; }
	}

	public Inventory inventory {
		get { return mInventory; }
	}
	
	private void OnEnable() {
		// Listen to all events for illustration purposes
		OpenIABEventManager.billingSupportedEvent += billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent += purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
	}
	private void OnDisable() {
		// Remove all event handlers
		OpenIABEventManager.billingSupportedEvent -= billingSupportedEvent;
		OpenIABEventManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		OpenIABEventManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		OpenIABEventManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		OpenIABEventManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		OpenIABEventManager.purchaseFailedEvent -= purchaseFailedEvent;
		OpenIABEventManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		OpenIABEventManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}

	private void Awake() {
		mInstance = this;
		DontDestroyOnLoad(this);
	}
	
	private void Start() {
		// Map skus for different stores
		#if UNITY_ANDROID
		string store = OpenIAB_Android.STORE_GOOGLE;
		#elif UNITY_IPHONE
		string store = OpenIAB_iOS.STORE;
		#endif

		OpenIAB.mapSku(SKU_2500_Coins, store, SKU_2500_Coins);
		OpenIAB.mapSku(SKU_25000_Coins, store, SKU_25000_Coins);
		OpenIAB.mapSku(SKU_75000_Coins, store, SKU_75000_Coins);
		OpenIAB.mapSku(SKU_250000_Coins, store, SKU_250000_Coins);


		// Application public key
		var public_key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnjKWhrO8FX/kqCE7QvJmHxU1OITUjSLVDhzXM6bG3SWMju7l8W/v6EJdDQxeYjHmuqWKpU5OeyzfZJvUXIjRFCdlZ9I4RZiZS1Nt+kckLG1I7SoIrFIwC0b98tIJDYjnDIaSvt/T9CWOeu1/MdQxJrQflobtN8CmQKMzVXD/nLY+v7wj1jtQPEYEN0MFpbMnmuOX5HZsyymQ1WVHaFZOY9Jp2gFivFGCJganFKFOOOiDGek5lPEM7yC64zVCHruqCNP3q2ZkQVGdoIDucL9XnN4TG4NdHjd+9SFFok+PlvojZ47GeZJRTtzZNVhFZRI0gRgGmLFaZgeUDngJq/XBcQIDAQAB";
		
		var options = new Options();
		options.verifyMode = OptionsVerifyMode.VERIFY_SKIP;
		options.storeKeys = new Dictionary<string, string> {
			{OpenIAB_Android.STORE_GOOGLE, public_key}
		};
		
		// Transmit options and start the service
		OpenIAB.init(options);
	}

	private void billingSupportedEvent() {
		_isInitialized = true;
		Debug.Log("billingSupportedEvent");

		OpenIAB.queryInventory(new string[] {
			SKU_2500_Coins,
			SKU_25000_Coins,
			SKU_75000_Coins,
			SKU_250000_Coins,

		});
	}
	private void billingNotSupportedEvent(string error) {
		Debug.Log("billingNotSupportedEvent: " + error);
	}
	private void queryInventorySucceededEvent(Inventory inventory) {
		Debug.Log("queryInventorySucceededEvent: " + inventory);
		if (inventory != null) {
			_label = inventory.ToString();
			mInventory = inventory;
		}
	}
	private void queryInventoryFailedEvent(string error) {
		Debug.Log("queryInventoryFailedEvent: " + error);
		_label = error;
	}
	private void purchaseSucceededEvent(Purchase purchase) {
		Debug.Log("purchaseSucceededEvent: " + purchase);
		_label = "PURCHASED:" + purchase.ToString();
	}
	private void purchaseFailedEvent(int errorCode, string errorMessage) {
		Debug.Log("purchaseFailedEvent: " + errorMessage);
		_label = "Purchase Failed: " + errorMessage;
	}
	private void consumePurchaseSucceededEvent(Purchase purchase) {
		Debug.Log("consumePurchaseSucceededEvent: " + purchase);
		_label = "CONSUMED: " + purchase.ToString();
	}
	private void consumePurchaseFailedEvent(string error) {
		Debug.Log("consumePurchaseFailedEvent: " + error);
		_label = "Consume Failed: " + error;
	}

	#endif
}