using UnityEngine;
using System.Collections;

public class LoadingIndicator : MonoBehaviour
{
	// Singleton instance
	static LoadingIndicator mInstance;

	//
	//
	//
	public static void ShowAt(float x, float y)
	{
		mInstance.gameObject.SetActive(true);
		mInstance.transform.localPosition = new Vector3(x, y, 0);
	}

	//
	//
	//
	public static void Hide()
	{
		mInstance.gameObject.SetActive(false);
	}

	//
	//
	//
	public LoadingIndicator()
	{
		mInstance = this;
	}

	//
	//
	//
	void Awake()
	{
		DontDestroyOnLoad(this);
	}
}
