using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour {

	public void OnDeath() {
		gameObject.SetActive(false);
	}
}
