using UnityEngine;
using System.Collections;

public class DialogController : MonoBehaviour {

	public event System.Action<bool> Closed;

	private static DialogController mInstance;
	
	public static bool IsConfermationOpen;

	public static DialogController instance {
		get { return mInstance; }
	}

	public void SetMessage(string message) {
		transform.FindChild("Message").GetComponent<UILabel>().text = message;
	}

	public void OnOK() {
		if(Closed != null) Closed(true);
		gameObject.SetActive(false);
	}

	public void OnCancel() {
		if(Closed != null) Closed(false);
		gameObject.SetActive(false);
		Time.timeScale = 1;
	}

	void Awake() {
		mInstance = this;

	}

	void OnEnable() {
		GetComponent<UIPlaySound>().Play();
		IsConfermationOpen = true;
	}



	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			OnCancel();
		}
	}


}
