using UnityEngine;
using System.Collections;
//using Heyzap;


public class MyHeyzap : MonoBehaviour {


	public static MyHeyzap instence;

	void Awake()
	{
		instence = this;
		HeyzapAds.start("4f2ead6c706a5286add45879429b0be5", HeyzapAds.FLAG_NO_OPTIONS);
		HZIncentivizedAd.fetch();

	}

	public void TestMediation()
	{
		HeyzapAds.showMediationTestSuite ();

	}

	public void showVideoAd()
	{
		if (HZIncentivizedAd.isAvailable()) {
			HZIncentivizedAd.show();

			HZIncentivizedAd.setDisplayListener(listener);
		}
			
	}


	HZIncentivizedAd.AdDisplayListener listener = delegate(string adState, string adTag){

		if ( adState.Equals("incentivized_result_complete") ) {
			// The user has watched the entire video and should be given a reward.
			MainMenuController.instence.FreeCoinsPopUp();
			print("incentivized_result_complete");
		}
		if ( adState.Equals("incentivized_result_incomplete") ) {
			// The user did not watch the entire video and should not be given a   reward.
			print("incentivized_result_incomplete");
		}
	};
	


	public void ShowInterstitialAd()
	{
		HZInterstitialAd.show();
	}




}
