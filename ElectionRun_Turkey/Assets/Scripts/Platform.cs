using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Platform : MonoBehaviour {
	
	public Vector2 Size;
	public Transform _transform;

	/**
	 * 
	 */
	Transform mLeftTransform;
	Transform mRightTransform;
	Transform mTopTransform;
	Transform mLeftBottomTransform;
	Transform mBottomTransform;
	Transform mRightBottomTransform;
	LevelGenerator.JumpInfo mObstacleJumpInfo;

	/**
	 * 
	 */
	public void SetSize(Vector2 size)
	{
		Size = size;
		BoxCollider2D collider = GetComponent<BoxCollider2D>();
		collider.size = size;

		UpdateSprites(collider);
	}

	/**
	 * 
	 */
	public void SetObstacleJumpInfo(LevelGenerator.JumpInfo jumpInfo)
	{
		#if UNITY_EDITOR
			if (mObstacleJumpInfo != null && jumpInfo != null) Debug.LogWarning("Already assigned");
		#endif

		mObstacleJumpInfo = jumpInfo;
	}

	/**
	 * 
	 */
	public LevelGenerator.JumpInfo obstacleJumpInfo
	{
		get { return mObstacleJumpInfo; }
	}

	/**
	 * 
	 */
	void Start()
	{
		_transform = transform;
		Size = GetComponent<BoxCollider2D>().size;
	}

	/**
	 * 
	 */
	void Awake()
	{
		Transform _thisTransf = transform;
		mLeftTransform = _thisTransf.FindChild("left");
		mRightTransform = _thisTransf.FindChild("right");
		mTopTransform = _thisTransf.FindChild("top");
		mLeftBottomTransform = _thisTransf.FindChild("left_bottom");
		mBottomTransform = _thisTransf.FindChild("bottom");
		mRightBottomTransform = _thisTransf.FindChild("right_bottom");
	}

#if UNITY_EDITOR
	void Update()
	{
		if (mLeftTransform == null || mRightTransform == null || mTopTransform == null ||
		    mLeftBottomTransform == null || mBottomTransform == null || mRightBottomTransform == null)
		{
			Awake();
		}

		BoxCollider2D collider = GetComponent<BoxCollider2D>();
		UpdateSprites(collider);
	}
#endif

	void UpdateCollider(Vector2 size)
	{
		Size = size;
		BoxCollider2D collider = gameObject.collider2D as BoxCollider2D;
		collider.size = size;
		
		// Update collider center
		size.x *= 0.5f;
		size.y = size.y * -0.5f - 0.2f;
		collider.center = size;
	}

	void UpdateSprites(BoxCollider2D collider)
	{
		Vector2 colliderSize = collider.size;
		Vector3 vec3 = new Vector3();
		
		// Left
		Vector2 spriteSizeL = mLeftTransform.GetComponent<SpriteRenderer>().sprite.bounds.size;
		mLeftTransform.localScale = Vector3.one;
		
		// Right
		Vector2 spriteSizeR = mRightTransform.GetComponent<SpriteRenderer>().sprite.bounds.size;
		mRightTransform.localScale = Vector3.one;
		vec3.Set(colliderSize.x - spriteSizeR.x, 0, 0);
		mRightTransform.localPosition = vec3;
		
		// Center
		Vector2 spriteSize = mTopTransform.GetComponent<SpriteRenderer>().sprite.bounds.size;
		vec3.Set((colliderSize.x - (spriteSizeL.x + spriteSizeR.x)) / spriteSize.x, 1, 1);
		mTopTransform.localScale = vec3;
		vec3.Set (spriteSizeL.x, 0, 0);
		mTopTransform.localPosition = vec3;

		// Left bottom
		spriteSize = mLeftBottomTransform.GetComponent<SpriteRenderer>().sprite.bounds.size;
		vec3.Set(0, -spriteSizeL.y, 0);
		mLeftBottomTransform.localPosition = vec3;
		vec3.Set(1, (colliderSize.y - spriteSizeL.y) / spriteSize.y, 0);
		mLeftBottomTransform.localScale = vec3;

		// Bottom
		vec3.Set(mTopTransform.localPosition.x, mLeftBottomTransform.localPosition.y, 0);
		mBottomTransform.localPosition = vec3;
		vec3.Set(mTopTransform.localScale.x, mLeftBottomTransform.localScale.y, 0);
		mBottomTransform.localScale = vec3;

		// Right bottom
		vec3.Set(mRightTransform.localPosition.x, mLeftBottomTransform.localPosition.y, 0);
		mRightBottomTransform.localPosition = vec3;
		mRightBottomTransform.localScale = mLeftBottomTransform.localScale;
		
		colliderSize.x *= 0.5f;
		colliderSize.y *= -0.5f;
		collider.center = colliderSize;
	}

	void OnBecameInvisible()
	{
		if (LevelGenerator.cameraTransform.localPosition.x >
		    transform.localPosition.x + Size.x + LevelGenerator.PlatformDeleteDistance)
		{
			gameObject.SetActive(false);
		}
	}
}
