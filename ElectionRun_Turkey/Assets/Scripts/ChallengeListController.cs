using UnityEngine;
using System.Collections;

public class ChallengeListController : MonoBehaviour {

	Animator mAnimator;
	public static bool challengeListOpen;
//	public MainMenuController mainMenuController;

	public void OnClose() {
		this.gameObject.SetActive(false);
	}

	void OnEnable()
	{
		challengeListOpen = true;
	}

	void Awake () {
		mAnimator = GetComponent<Animator>();
		mAnimator.Play("StoreIntro");
	}

	void OnGUI () {
		mAnimator.Update(Time.deltaTime);
	}

	void Update()
	{

		if (Input.GetKeyDown(KeyCode.Escape)) {

			gameObject.SetActive(false);
		}
	}

}
