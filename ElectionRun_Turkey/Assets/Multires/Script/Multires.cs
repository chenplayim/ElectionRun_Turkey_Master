using UnityEngine;
using System.Collections;

//
//
//
[ExecuteInEditMode]
public class Multires : MonoBehaviour {

	//
	//
	//
	public static bool _bDisableUpdate = false;

	//
	//
	//
	public string CurrentAssetSuffix;
	public string AssetSuffix = "_2x";
	public int MaxWidth = 940;
	public string DefaultAssetName;
	public string Asset2xName;

	//
	//
	//
	AssetCollection mActiveAsset;

	//
	//
	//
	static Multires mInstance;

	public static Multires instance {
		get {
			return mInstance;
		}
	}

	public static Multires Create() {
		GameObject go;
		go = new GameObject();
		go.name = "Multires";

		return (mInstance = go.AddComponent<Multires>());
	}

	public static Sprite GetSprite(string name) {
		return instance.mActiveAsset.GetSprite(name + instance.CurrentAssetSuffix);
	}

	public static RuntimeAnimatorController GetAnimatorController(string name) {
		return instance.mActiveAsset.GetController(name + instance.CurrentAssetSuffix);
	}
	//
	void Awake()
	{
		#if UNITY_EDITOR
		if (!UnityEditor.EditorApplication.isPlaying) return;
		#endif

		mInstance = this;
		int width = Screen.width > Screen.height ? Screen.width : Screen.height;
		bool hd = width > MaxWidth;
		CurrentAssetSuffix = hd ? AssetSuffix : "";

		// Load the asset collection prefab
		string assetName = hd ? Asset2xName : DefaultAssetName;
		GameObject go = Resources.Load<GameObject>(assetName);
		if (go != null) {
			mActiveAsset = go.GetComponent<AssetCollection>();
			mActiveAsset.Initialize();
		}
		else {
			Debug.LogError("Multires: Load asset failed: " + assetName);
		}

		DebugPrint();

		#if !UNITY_EDITOR
		DontDestroyOnLoad(this);
		#endif
	}

	void DebugPrint() {

		//
		// Sprites
		//
		//Debug.Log("***** Sprites Loaded *****");

		// List of loaded sprites
		Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
		foreach(Sprite sprite in sprites) {
			//Debug.Log(sprite.name);
		}

		//Debug.Log("**********");

		//
		// Controllers
		//
		//Debug.Log("***** Animator Controller Loaded *****");

		// List of loaded controllers
		RuntimeAnimatorController[] controllers = Resources.FindObjectsOfTypeAll<RuntimeAnimatorController>();
		foreach(RuntimeAnimatorController controller in controllers) {
			//Debug.Log(controller.name);
		}

		//Debug.Log("**********");

		//
		// Textures
		//
		/*Debug.Log("***** Textures Loaded *****");
		
		// List of loaded controllers
		Texture[] textures = Resources.FindObjectsOfTypeAll<Texture>();
		foreach(Texture texture in textures) {
			Debug.Log(texture.name);
		}
		
		Debug.Log("**********");*/
	}

	#if UNITY_EDITOR

	void Update()
	{
		if (UnityEditor.EditorApplication.isPlaying) return;
		Awake();
	}

	#endif
}
