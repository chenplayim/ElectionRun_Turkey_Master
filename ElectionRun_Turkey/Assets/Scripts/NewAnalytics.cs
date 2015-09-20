using UnityEngine;
using System.Collections;

public class NewAnalytics : MonoBehaviour {

	// Events
	
	public void GooglePlayGA(){
		
		Analytics.gua.sendEventHit("Action", "GooglePlay");
	}

	public void GameCenterGA(){
		
		Analytics.gua.sendEventHit("Action", "GameCenter");
	}

	public void FaceBookGA(){
		
		Analytics.gua.sendEventHit("Action", "FaceBook");
	}

	public void StoreGA(){
		
		Analytics.gua.sendEventHit("Action", "Upgrade Store");
	}


	//Events Upgrade Screen

	public void BuyCoinsGA(){
		
		Analytics.gua.sendEventHit("Action", "Buy Coins");
	}

	public void BubbleShieldGA(){
		
		Analytics.gua.sendEventHit("Action", "Bubble shield +");
	}
	public void JumpHeightGA(){
		
		Analytics.gua.sendEventHit("Action", "Jump height +");
	}
	public void DoubleJumpGA(){
		
		Analytics.gua.sendEventHit("Action", "Double jump +");
	}
	public void CoinsStoreUpgradeGA(){
		
		Analytics.gua.sendEventHit("Action", "Coins store_Upgrade");
	}
	public void HatsStoreUpgradeGA(){
		
		Analytics.gua.sendEventHit("Action", "Hats store_Upgrade");
	}

	public void VideoAdClickGA(){
		
		Analytics.gua.sendEventHit("Action", "VideoAdClick");
	}


	//Events End Game Screen

	public void ReplayGA(){
		
		Analytics.gua.sendEventHit("Action", "Replay");
	}
	public void UpgradeStore_GameOverGA(){
		
		Analytics.gua.sendEventHit("Action", "UpgradeStore_GameOver");
	}
	public void Send_ChallengeGA(){
		
		Analytics.gua.sendEventHit("Action", "Send_Challenge");
	}
	public void MainMenu_GameOverGA(){
		
		Analytics.gua.sendEventHit ("Action", "Main Menu_GameOver");
	}

















}
