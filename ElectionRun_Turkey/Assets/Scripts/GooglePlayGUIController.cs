using UnityEngine;
using System.Collections;

public class GooglePlayGUIController : MonoBehaviour {

	//
	//
	//
	public event System.Action Closed;

	//
	//
	//
	public void OnClose() {
		Close();
	}

	//
	//
	//
	public void OnOpenAchievement() {
		if (!Social.localUser.authenticated) {
			Authenticate(() => {
				Debug.Log ("Show Achievements");
				Social.ShowAchievementsUI();
			});
		}
		else {
			Social.ShowAchievementsUI();
		}
	}

	//
	//
	//
	public void OnOpenLeaderBoard( ) {

		if (!Social.localUser.authenticated) {
			Authenticate(() => {
				Debug.Log ("Show Leaderboard");
				Social.ShowLeaderboardUI();
			});
		}
		else {
			Social.ShowLeaderboardUI();
		}
	}

	//
	//
	//
	void Authenticate(System.Action callback) {

		// authenticate user:
		Social.localUser.Authenticate((bool success) => {

			if (Social.localUser.authenticated) callback();

			// handle success or failure
			Debug.Log("Google Play Dialog: Authenticate result: " + success);
		});
	}

	//
	//
	//
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Close();
		}
	}

	//
	//
	//
	void Close() {
		if(Closed != null) Closed();
		gameObject.SetActive(false);
	}


}
