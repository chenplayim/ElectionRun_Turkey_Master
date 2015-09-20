using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SrutonimFacebook()
	{
		Application.OpenURL ("https://www.facebook.com/srutonim");
	}

	public void assafMail()
	{
		string email = "asaf@play.im";
		string subject = MyEscapeURL("My Subject");
		string body = MyEscapeURL("My Body\r\nFull of non-escaped chars");
		
		Application.OpenURL ("mailto:" + email + "?subject=" + subject + "&body=" + body);
	}

	string MyEscapeURL (string url)
	{
		return WWW.EscapeURL(url).Replace("+","%20");
	}

	public void  CloseCredits()
	{

		gameObject.SetActive (false);
	}

	public void OpenCredits()
	{
		
		gameObject.SetActive (true);
	}

}
