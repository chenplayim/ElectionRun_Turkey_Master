using UnityEngine;
using System.Collections;
//using Heyzap;


public class MyHeyzap : MonoBehaviour {


	void Awake()
	{
		HeyzapAds.start("4f2ead6c706a5286add45879429b0be5", HeyzapAds.FLAG_NO_OPTIONS);

	}

	public void TestMediation()
	{
		HeyzapAds.showMediationTestSuite ();

	}



}
