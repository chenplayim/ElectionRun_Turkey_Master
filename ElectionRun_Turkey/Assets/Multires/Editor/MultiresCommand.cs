using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MultiresCommand : MonoBehaviour {

	[MenuItem ("Multires/Prepare Scene", false, 0)]
	static void PrepareScene() {
		PrepareSceneForMultires();
	}

	[MenuItem ("Multires/Build", false, 0)]
	static void CustomBuild() {
		Build(false);
	}

	[MenuItem ("Multires/Build and Run", false, 0)]
	static void CustomBuildAndRun() {
		Build(true);
	}

	static void Build(bool autorun)
	{
		if (!Application.HasProLicense())
		{
			Debug.LogError("PRO license needed to run Multires tool");
			return;
		}

		// Get build path
		string buildPath = EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget);
		if (buildPath.Length == 0) {
			Debug.LogError("Build path not set.");
			return;
		}

		// Get scenes
		List<string> sceneList = new List<string>();
		EditorBuildSettingsScene[] selectedScenes = EditorBuildSettings.scenes;
		foreach(EditorBuildSettingsScene scene in selectedScenes)
		{
			if(scene.enabled)
			{
				sceneList.Add(scene.path);
			}
		}

		GameObject[] objects = FindObjectsOfType<GameObject>();

		Multires._bDisableUpdate = true;
		PreparePrefabs();
		RemoveSpriteReferences(objects);

		BuildPipeline.BuildPlayer (
			sceneList.ToArray(),
			buildPath,
			EditorUserBuildSettings.activeBuildTarget, autorun ? BuildOptions.AutoRunPlayer : BuildOptions.None
		);

		Debug.Log("Post build");
		ResetReferences( FindObjectsOfType<GameObject>() );
		ResetPrefabsReferences();
		Multires._bDisableUpdate = false;
	}

	// Remove sprite references
	static void RemoveSpriteReferences(GameObject[] objects)
	{
		Debug.Log("Remove sprite references begin");

		SpriteRenderer renderer;
		AssetResolutionSwapper swapper = null;
		Animator animator;

		foreach(GameObject go in objects)
		{
			swapper = go.GetComponent<AssetResolutionSwapper>();

			// Sprite renderers
			renderer = go.GetComponent<SpriteRenderer>();
			if(renderer != null)
			{
				if (renderer.sprite != null) {
					if (swapper == null) swapper = AddAssetSwapper(go);

					if (!swapper.DisableSwapping) {
						// Remove references to prevent loading
						swapper.defaultSprite = renderer.sprite;
						renderer.sprite = null;
					}
				}
				else {
					if (swapper != null) {
						swapper.SpriteName = "";
						swapper.defaultSprite = null;
					}
				}
			}

			// Controller references
			animator = go.GetComponent<Animator>();
			if (animator != null && animator.runtimeAnimatorController != null) {

				if (swapper == null) swapper = AddAssetSwapper(go);

				if (!swapper.DisableSwapping) {
					// Remove references to prevent loading
					swapper.defaultAnimatorController = animator.runtimeAnimatorController;
					animator.runtimeAnimatorController = null;
				}
			}
			else {
				if (swapper != null) {
					swapper.AnimationControllerName = "";
					swapper.defaultAnimatorController = null;
				}
			}
		}

		Debug.Log("Remove sprite references finished");
	}

	// Iterate through GameObjects and add AssetResolutionSwapper component as needed
	static void PrepareSceneForMultires() {

		// Process prefabs
		PreparePrefabs();

		//
		// Process Scene
		//
		GameObject[] objects = FindObjectsOfType<GameObject>();
		SpriteRenderer spriteRenderer;
		Animator animator;
		AssetResolutionSwapper swapper;

		foreach(GameObject go in objects) {

			swapper = go.GetComponent<AssetResolutionSwapper>();

			spriteRenderer = go.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null && spriteRenderer.sprite != null) {
				if(swapper == null) swapper = AddAssetSwapper(go);
				continue;
			}

			animator = go.GetComponent<Animator>();
			if (animator != null && animator.runtimeAnimatorController != null) {
				if(swapper == null) swapper = AddAssetSwapper(go);
				continue;
			}
		}
	}

	// 
	static AssetResolutionSwapper AddAssetSwapper(GameObject go) {

		AssetResolutionSwapper swapper = go.AddComponent<AssetResolutionSwapper>();
		Multires._bDisableUpdate = false;
		swapper.ForceUpdate();
		Multires._bDisableUpdate = true;
		return swapper;
	}

	// Prepare prefabs for Multires
	static void PreparePrefabs() {

		string[] guids = AssetDatabase.FindAssets("t:GameObject");
		Debug.Log("Num guids : " + guids.Length);

		string path;
		GameObject prefab;

		foreach (string guid in guids) {
			path = AssetDatabase.GUIDToAssetPath(guid);
			Debug.Log("Process prefab : " + path);

			prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

			PrepareGameObject(prefab);

			Debug.Log ("Prefab \"" + path + "\" is processed.");
		}
	}

	static void PrepareGameObject(GameObject go) {

		// Try to get the swapper
		AssetResolutionSwapper swapper = go.GetComponent<AssetResolutionSwapper>();
		
		// Get sprite renderer
		SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
		if (spriteRenderer != null && spriteRenderer.sprite != null) {
			if (swapper == null) swapper = AddAssetSwapper(go);

			if (!swapper.DisableSwapping) {
				// Remove reference to prevent loading
				swapper.defaultSprite = spriteRenderer.sprite;
				spriteRenderer.sprite = null;
			}
		}
		
		// Does the prefab have an animator
		Animator animator = go.GetComponent<Animator>();
		if (animator != null && animator.runtimeAnimatorController != null) {
			if (swapper == null) swapper = AddAssetSwapper(go);

			if (!swapper.DisableSwapping) {
				// Remove reference to prevent loading
				swapper.defaultAnimatorController = animator.runtimeAnimatorController;
				animator.runtimeAnimatorController = null;
			}
		}

		// Iterate through children
		Transform transf = go.transform;
		GameObject child;
		int i;
		for(i=0; i<transf.childCount; ++i) {
			child = transf.GetChild(i).gameObject;
			PrepareGameObject(child);
		}
	}

	//
	static void ResetReferences(GameObject[] objects) {

		SpriteRenderer spriteRenderer;
		AssetResolutionSwapper swapper;
		Animator animator;

		Debug.Log("Number of objects to reset: " + objects.Length);

		foreach(GameObject go in objects) {

			swapper = go.GetComponent<AssetResolutionSwapper>();

			if (swapper != null) {

				spriteRenderer = go.GetComponent<SpriteRenderer>();
				if (spriteRenderer != null && swapper.SpriteName.Length > 0) {

					if (!swapper.DisableSwapping) spriteRenderer.sprite = swapper.defaultSprite;
				}

				animator = go.GetComponent<Animator>();
				if (animator != null && swapper.AnimationControllerName.Length > 0) {
					if (!swapper.DisableSwapping) animator.runtimeAnimatorController = swapper.defaultAnimatorController;
				}
			}
			else {

				Debug.Log("GameObject have no swapper: " + go.name);
			}
		}
	}

	// Reset prefabs references
	static void ResetPrefabsReferences() {
		
		string[] guids = AssetDatabase.FindAssets("t:GameObject");
		Debug.Log("Num guids : " + guids.Length);
		
		string path;
		GameObject prefab;
		SpriteRenderer spriteRenderer;
		AssetResolutionSwapper swapper;
		Animator animator;
		
		foreach (string guid in guids) {
			path = AssetDatabase.GUIDToAssetPath(guid);

			prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
			
			// Try to get the swapper
			swapper = prefab.GetComponent<AssetResolutionSwapper>();

			if (swapper != null) {

				spriteRenderer = prefab.GetComponent<SpriteRenderer>();
				if (spriteRenderer != null && swapper.SpriteName.Length > 0) {
					spriteRenderer.sprite = swapper.defaultSprite;
				}
				
				animator = prefab.GetComponent<Animator>();
				if (animator != null && swapper.AnimationControllerName.Length > 0) {
					animator.runtimeAnimatorController = swapper.defaultAnimatorController;
				}
			}
		}
	}

	static void ResetGameObjectReferences(GameObject go) {

		// Try to get the swapper
		AssetResolutionSwapper swapper = go.GetComponent<AssetResolutionSwapper>();
		
		if (swapper != null) {
			
			SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null && swapper.SpriteName.Length > 0) {
				if(!swapper.DisableSwapping) spriteRenderer.sprite = swapper.defaultSprite;
			}
			
			Animator animator = go.GetComponent<Animator>();
			if (animator != null && swapper.AnimationControllerName.Length > 0) {
				if(!swapper.DisableSwapping) animator.runtimeAnimatorController = swapper.defaultAnimatorController;
			}
		}

		Transform transf = go.transform;
		int i;
		for(i=0; i<transf.childCount; ++i) {
			ResetGameObjectReferences( transf.GetChild(i).gameObject );
		}
	}
}
