using UnityEngine;
using System.Collections;
using Facebook.MiniJSON;
using System.Collections.Generic;

public class FacebookManager : MonoBehaviour
{
	//
	//
	//
	public static System.Action initialized;
	public System.Action<bool, string> loginResult;
	public System.Action<string> appRequestResult;		// "cancelled", "sent"

	//
	//
	//
	static FacebookManager mInstance;
	bool mInitialized = false;

	//
	//
	//
	public static FacebookManager instance
	{
		get { return mInstance; }
	}

	//
	//
	//
	public bool isInitialized
	{
		get { return mInitialized; }
	}

	//
	//
	//
	public bool isLoggedIn
	{
		get { return FB.IsLoggedIn; }
	}

	//
	//
	//



	public void SendAppRequest(string _message, string _title = "", string _data = "")
	{
		FB.AppRequest(

			_message,    // String message
			null,               // List of ids to exclude
			null,               // User Filter
			null,               // Excluded Ids
			50,                // Max invites
			_data,             // Data Text
			_title,      // Invite title
			callback:appRequestCallback      // callback method


		);
	}
	

	//
	//
	//
	public void GetDeepLink(Facebook.FacebookDelegate resultCallback)
	{
		FB.GetDeepLink(resultCallback);
	}

	//
	//
	//
	public void ChallengeFriend(string message, string[] to, string data, string title)
	{
		FB.AppRequest(

			message,    // String message
			to,              // List of ids to exclude
			null,               // User Filter
			null,               // Excluded Ids
			50,                // Max invites
			data,             // Data Text
			title     // Invite title
			//InviteCallback      // callback method

		);
	}

	//
	//
	//
	void Initialize()
	{
		if (mInitialized)
		{
			OnInitialized();
		}
		else
		{
			// Initialize FB SDK   
			FB.Init(OnInitialized, OnHideUnity);
		}
	}

	//
	//
	//
	public string get_data;
	public static string fbname;
	
//	void UserCallBack(FBResult result) {
//		if (result.Error != null)
//		{                                                                      
//			get_data = result.Text;
//		}
//		else
//		{
//			get_data = result.Text;
//		}
//		var dict = Json.Deserialize(get_data) as IDictionary;
//		fbname =dict ["name"].ToString();
//
//		Debug.Log ("fbname" + fbname);
//	}
//


	private void appRequestCallback (FBResult result)                                                                              
	{      
		//FB.API("me?fields=name", Facebook.HttpMethod.GET, appRequestCallback);

		if (result != null)
		{
			if (result.Error != null)
			{
				Debug.Log("app request error: " + result.Error);
				appRequestResult("error");
			}
			else
			{
				Dictionary<string, object> responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;
				object obj = null;

				if (responseObject.TryGetValue ("cancelled", out obj))
				{
					Debug.Log("Request cancelled");
					appRequestResult("cancelled");
				}
				else if (responseObject.TryGetValue ("request", out obj))
				{
					Debug.Log("Request sent");
					appRequestResult("sent");
				}
				else
				{
					appRequestResult("error");
				}
			}
		}
		else
		{
			appRequestResult("cancelled");
			get_data = result.Text;
		}

	}    

	//
	//
	//
	private void OnHideUnity(bool isGameShown)                                                   
	{                                                                                            
		Debug.Log("OnHideUnity");
		if (!isGameShown)
		{
			// pause the game - we will need to hide
			Time.timeScale = 0;
		}
		else
		{
			// start the game back up - we're getting focus again
			Time.timeScale = 1;
		}
	}

	//
	//
	//
	public void Login()
	{
		if (!FB.IsLoggedIn)                                                                                              
		{                                                                                                                
			FB.Login("user_friends,publish_actions", LoginCallback);
		}
		else
		{                                                                                        
			Debug.Log("Already logged in");
			loginResult(true, null);
		}
	}

	//
	//
	//
	void Awake()
	{
		mInstance = this;
		Initialize();
	}

	//
	//
	//
	private void OnInitialized()
	{
		mInitialized = true;
		if(initialized != null) initialized();
	}

	//
	//
	//
	void LoginCallback(FBResult result)                                                        
	{                                                                                          
		Debug.Log("LoginCallback");

		if(loginResult != null) loginResult(FB.IsLoggedIn, result == null ? "" : result.Error);
	}
}
