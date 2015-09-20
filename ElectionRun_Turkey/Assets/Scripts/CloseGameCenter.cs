using UnityEngine;
using System.Collections;

public class CloseGameCenter : MonoBehaviour {

	public GameObject MainMenu;

	public void CloseGameCenterGUI()
	{
		
		gameObject.SetActive(false);
		MainMenu.SetActive(true);
		
	}
}
