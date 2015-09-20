using UnityEngine;
using System.Collections;

public class DialogYouWon : MonoBehaviour {


	public GameObject wonCoinsDialog;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ExitYouWonCoins() {
		
		wonCoinsDialog.SetActive (false);
	}
}
