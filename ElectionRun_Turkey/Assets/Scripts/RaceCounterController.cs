using UnityEngine;
using System.Collections;

public class RaceCounterController : MonoBehaviour {

	public AudioClip timerClip;
	public AudioClip GoClip;

	public System.Action finished;
	Animator mAnimator;
	float mTime;


	public void OnFinished()
	{
		StartCoroutine( FinishCountdown() );
	}

	void Awake()
	{
		mAnimator = GetComponent<Animator>();
	}

	void OnEnable()
	{
		mTime = Time.realtimeSinceStartup;
	}

	void playTimerSound()
	{
		Time.timeScale = 1;
		AudioSource.PlayClipAtPoint (timerClip, transform.position);
		Time.timeScale = 0;
	}

	void playGoSound()
	{
		Time.timeScale = 1;
		AudioSource.PlayClipAtPoint (GoClip, transform.position);
		Time.timeScale = 0;
	}

	void OnGUI()
	{
		float time = Time.realtimeSinceStartup;
		mAnimator.Update(time - mTime);
		mTime = time;
	}

	IEnumerator FinishCountdown()
	{
		yield return new WaitForEndOfFrame();

		this.gameObject.SetActive(false);
		if (finished != null) finished();
	}
}
