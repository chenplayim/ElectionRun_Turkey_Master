using UnityEngine;
using System.Collections;

public class PauseMenuController : MonoBehaviour
{
	//
	//
	//
	public System.Action Closed;

	//
	//
	//
	private UIButton mMuteButton;
	private UIButton mSoundOnButton;

	/**
	 * 
	 */
	public void OpenPauseMenu()
	{
		Time.timeScale = 0;
		gameObject.SetActive(true);
		// Analytics
		Analytics.gua.sendAppScreenHit("Pause");
	}

	/**
	 * 
	 */
	public void OnClickResume()
	{
		Time.timeScale = 1;
		if(Closed != null) Closed();
		gameObject.SetActive(false);
	}

	/**
	 * 
	 */
	public void OnClickQuit()
	{
		Time.timeScale = 1;
		if(Closed != null) Closed();
		gameObject.SetActive(false);
		//Analytics
		Analytics.gua.sendEventHit("Action", "Main menu_Pause");
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
	void Awake() {

		Transform transf = transform;
		mMuteButton = transf.FindChild("MuteButton").GetComponent<UIButton>();
		mSoundOnButton = transf.FindChild("SoundOnButton").GetComponent<UIButton>();
	}

	//
	//
	//
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			OnClickResume();
		}
	}

	//
	//
	//
	void OnEnable() {
		UpdateButton(!AppManager.enableSound);
	}

	//
	//
	//
	void UpdateButton(bool bMute) {
		mMuteButton.gameObject.SetActive(!bMute);
		mSoundOnButton.gameObject.SetActive(bMute);
	}
}
