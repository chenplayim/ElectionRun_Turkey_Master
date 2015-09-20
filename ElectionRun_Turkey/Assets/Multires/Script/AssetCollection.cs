using UnityEngine;
using System.Collections;

public class AssetCollection : MonoBehaviour {

	public System.Collections.Generic.List<Sprite> Sprites;
	public System.Collections.Generic.List<RuntimeAnimatorController> Controllers;

	//
	//
	//
	System.Collections.Generic.Dictionary<string, Sprite> mSprites;
	System.Collections.Generic.Dictionary<string, RuntimeAnimatorController> mControllers;

	//
	//
	//
	public Sprite GetSprite(string name) {
		//#if UNITY_EDITOR
		if (!mSprites.ContainsKey(name)) Debug.Log("Sprite not found: " + name);
		//#endif

		return mSprites[name];
	}
	
	//
	//
	//
	public RuntimeAnimatorController GetController(string name) {
		#if UNITY_EDITOR
		if (!mControllers.ContainsKey(name)) Debug.Log("Animation controller not found: " + name);
		#endif
		return mControllers[name];

	}

	//
	//
	//
	public void Initialize() {
		Awake();
	}

	//
	//
	//
	void Awake() {

		// Put sprites in dictionary
		mSprites = new System.Collections.Generic.Dictionary<string, Sprite>();
		foreach(Sprite sprite in Sprites) {
			if (sprite != null) {
				mSprites[sprite.name] = sprite;
			}
		}

		// Put controllers in dictionary
		mControllers = new System.Collections.Generic.Dictionary<string, RuntimeAnimatorController>();
		foreach(RuntimeAnimatorController controller in Controllers) {
			if (controller != null) {
				mControllers[controller.name] = controller;
			}
		}
	}
}
