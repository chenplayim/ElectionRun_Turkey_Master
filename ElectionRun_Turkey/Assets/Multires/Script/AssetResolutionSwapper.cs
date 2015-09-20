using UnityEngine;
using System.Collections;

/**
 * 
 */
[ExecuteInEditMode]
public class AssetResolutionSwapper : MonoBehaviour
{
	/**
	 * 
	 */
	public string SpriteName;
	public string AnimationControllerName;
	public bool DisableSwapping;

	#if UNITY_EDITOR
	//
	//
	//
	[HideInInspector]
	public Sprite defaultSprite;
	[HideInInspector]
	public RuntimeAnimatorController defaultAnimatorController;

	//
	//
	//
	public void ForceUpdate()
	{
		Update();
	}
	#endif
	
	/**
	 * 
	 */
	void Awake()
	{
		#if UNITY_EDITOR

		if (!UnityEditor.EditorApplication.isPlaying) {
			return;
		}

		#endif

		if (Multires.instance != null && !DisableSwapping) {

			if (SpriteName.Length > 0)
			{
				// Attemp to swap sprite
				SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();
				if (spriteRend != null) {

					Sprite sprite = Multires.GetSprite(SpriteName);

					if(sprite != null) {
						spriteRend.sprite = sprite;
						//Debug.Log("Relink sprite renderer: " + gameObject.name);
					}
					else {
						//Debug.LogWarning("Sprite not found: " + SpriteName + Multires.instance.CurrentAssetSuffix);
					}
				}
			}

			if (AnimationControllerName.Length > 0)
			{
				// Attempt to swap animation clip
				Animator animator = GetComponent<Animator>();
				if (animator != null) {

					RuntimeAnimatorController controller = Multires.GetAnimatorController(AnimationControllerName);

					if(controller != null) {
						animator.runtimeAnimatorController = controller;
						//Debug.Log("Relink controller: " + gameObject.name);
					}
					else {
						//Debug.LogWarning("Controller not found: " + AnimationControllerName + Multires.instance.CurrentAssetSuffix);
					}
				}
			}
		}

		Destroy(this);
		//Debug.Log("Self destruct: " + gameObject.name);
	}
	
	#if UNITY_EDITOR
	
	/**
	 * 
	 */
	void Update()
	{
		if (Multires._bDisableUpdate || UnityEditor.EditorApplication.isPlaying) return;

		//Debug.Log("Swapper Update");

		// Take sprite name if exist
		SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();
		if (spriteRend != null) {
			SpriteName = spriteRend.sprite != null ? spriteRend.sprite.name : "";
		}
		else {
			SpriteName = "";
		}

		// Take animation clip name if exist
		Animator animator = GetComponent<Animator>();
		if (animator != null) {
			AnimationControllerName = animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "";
		}
		else {
			AnimationControllerName = "";
		}
	}
	
	#endif
}
