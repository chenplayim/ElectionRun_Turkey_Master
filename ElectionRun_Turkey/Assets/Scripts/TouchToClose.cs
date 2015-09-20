using UnityEngine;
using System.Collections;

public class TouchToClose : MonoBehaviour {

	public event System.EventHandler<System.EventArgs> Closed = delegate {};

	public void OnClose() {
		Closed(this, System.EventArgs.Empty);
		gameObject.SetActive(false);
	}
}
