using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostController : MonoBehaviour
{
	/**
	 * 
	 */
	public enum AnimationState : int
	{
		Run = 0,
		Jump = 1,
		Descend = 2,
		Dead = 3
	};

	/**
	 * 
	 */
	public const float JumpSpeed = 4;
	public const float Gravity = -4;
	public const string Anonymous = "Anon";

	/**
	 * 
	 */
	static Vector3 sPosition = new Vector3();
	static Vector2 sHelperVector2 = new Vector2();

	/**
	 * 
	 */
	public float DiesAtMeters;

	/**
	 *
	 */
	Transform mTransform;
	LevelGenerator mLevelGenerator;
	PlayerController mPlayer;
	Animator mAnimator;
	Rigidbody2D mRigidBody;
	UILabel mNameLabel;
	Transform mNameLabelTransform;

	/**
	 * 
	 */
	GhostManager mManager;
	GameObject mCurPlatform;
	GameObject mNextPlatform;
	LevelGenerator.JumpInfo mCurJumpInfo;
	LevelGenerator.JumpInfo mObstacleJumpInfo;
	LevelGenerator.JumpInfo mPlatformJumpInfo;
	float mGroundY = float.MaxValue;
	float mOffsetXFromPlayer = 3;
	AnimationState mCurrentAnimation = AnimationState.Run;

	/**
	 * 
	 */
	public GhostManager manager
	{
		set { mManager = value; }
		get { return mManager; }
	}

	/**
	 * 
	 */
	public float offsetXFromPlayer
	{
		set { mOffsetXFromPlayer = value; }
		get { return mOffsetXFromPlayer; }
	}

	/**
	 * 
	 */
	public AnimationState currentAnimation
	{
		get { return mCurrentAnimation; }
	}

	/**
	 * 
	 */
	public void setLabel(UILabel label)
	{
		#if UNITY_EDITOR
			if (mNameLabel != null && label != null) Debug.LogError("Name label is already set");
		#endif

		mNameLabel = label;
		if(mNameLabel != null)
		{
			mNameLabel.text = "";
			mNameLabelTransform = mNameLabel.transform;
			StartCoroutine( SetNameLabel(this.name == Anonymous ? "" : this.name) );
			//mNameLabel.MarkAsChanged();
			//mNameLabel.UpdateNGUIText();
		}
	}

	/**
	 * 
	 */
	public UILabel nameLabel
	{
		get { return mNameLabel; }
	}

	/**
	 * 
	 */




	public void Reset(GameObject platform, string gender = null)
	{
		// Random gender
//		if (gender == null) {
//			if (Random.value < 0.5f) mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(Config.instance.BoyAnimator);
//			else mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(Config.instance.GirlAnimator);
//		}
//		else {
//			string animator = gender == "Girl" ? Config.instance.GirlAnimator : Config.instance.BoyAnimator;
//			mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(animator);
//		}

		// Elections change
		mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(LevelGenerator.ChosenCharecter);

		mCurPlatform = platform;
		mObstacleJumpInfo = mCurPlatform.GetComponent<Platform>().obstacleJumpInfo;

		mNextPlatform = null;
		mCurJumpInfo = null;
		mPlatformJumpInfo = null;
		mGroundY = platform.transform.localPosition.y + LevelGenerator.CharacterHalfHeight;
		mCurrentAnimation = AnimationState.Run;
		mAnimator.Play("run");
		mRigidBody.simulated = false;
		this.collider2D.enabled = true;
		this.enabled = true;
	}

	/**
	 * 
	 */
	void Awake()
	{

		mTransform = gameObject.transform;
		mLevelGenerator = LevelGenerator.instance;
		mPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		mAnimator = GetComponent<Animator>();
		mRigidBody = GetComponent<Rigidbody2D>();

		mCurPlatform = mLevelGenerator.FirstPlatform;
		mGroundY = mCurPlatform.transform.localPosition.y + GetComponent<SpriteRenderer>().bounds.extents.y ;


	}

	/**
	 * 
	 */
	void FixedUpdate()
	{


		sPosition = mTransform.localPosition;

		if (sPosition.x < DiesAtMeters || DiesAtMeters == -1)
		{
			float speedPerFrame;
			speedPerFrame = mPlayer.CurrentSpeed * Time.fixedDeltaTime;
			sPosition.x += speedPerFrame;

			float ySpeed;
			if (mCurJumpInfo != null)
			{
				if (mCurJumpInfo.type == LevelGenerator.JumpType.Platform)
				{
					if (mObstacleJumpInfo != null)
					{
						if (mObstacleJumpInfo.jumpStart.x <= sPosition.x)
						{
							// Switch to obstacle jumping
							mGroundY = mCurJumpInfo.jumpEnd.y;
							mCurJumpInfo = mObstacleJumpInfo;
						}
					}
				}
				else
				{
					if (mPlatformJumpInfo != null)
					{
						if (mPlatformJumpInfo.jumpStart.x <= sPosition.x)
						{
							// Switch to platform jumping
							mGroundY = mCurJumpInfo.jumpEnd.y;
							mCurJumpInfo = mPlatformJumpInfo;
							mObstacleJumpInfo = null;
						}
					}
				}

				#if UNITY_EDITOR
				Debug.DrawLine(new Vector3(mCurJumpInfo.jumpStart.x, mCurJumpInfo.jumpPeak.y + 1, 0),
				               new Vector3(mCurJumpInfo.jumpEnd.x, mCurJumpInfo.jumpPeak.y + 1, 0));
				#endif

				if (sPosition.x >= mCurJumpInfo.jumpStart.x && sPosition.x <= mCurJumpInfo.jumpEnd.x)
				{
					// Jumping and descending

					mGroundY = float.MaxValue;
					ySpeed = mTransform.localPosition.x < mCurJumpInfo.jumpPeak.x ? JumpSpeed : Gravity;
					sPosition.y += Time.fixedDeltaTime * ySpeed;

					if (mCurrentAnimation != AnimationState.Jump || sPosition.x <= mCurJumpInfo.jumpStart.x)
					{
						mAnimator.Play("jump");
						mCurrentAnimation = AnimationState.Jump;
					}
				}
				else if (sPosition.x >= mCurJumpInfo.jumpEnd.x)
				{
					// Passed landing point

					if (mCurJumpInfo.type == LevelGenerator.JumpType.Platform)
					{
						mGroundY = mCurJumpInfo.jumpEnd.y;
						mCurJumpInfo = null;
						mCurPlatform = mNextPlatform;
						mObstacleJumpInfo = mCurPlatform.GetComponent<Platform>().obstacleJumpInfo;
						mNextPlatform = null;
						mPlatformJumpInfo = null;

					}
					else
					{
						mGroundY = mCurJumpInfo.jumpEnd.y;
						mCurJumpInfo = mPlatformJumpInfo;
						mObstacleJumpInfo = null;
					}
				}
			}
			else
			{
				int index = mLevelGenerator.GetPlatformIndex(mCurPlatform);
				mNextPlatform = mLevelGenerator.GetPlatformAt(index + 1);

				if (mNextPlatform != null)
				{
					mPlatformJumpInfo = mCurJumpInfo = mLevelGenerator.GetJumpInfo(index);
				}
			}

			if (mGroundY != float.MaxValue)
			{
				if (sPosition.y > mGroundY)
				{
					sPosition.y += Gravity * Time.fixedDeltaTime;
					
					if (sPosition.y <= mGroundY)
					{
						sPosition.y = mGroundY;
						
						if (mCurrentAnimation != AnimationState.Run)
						{
							mAnimator.Play("run");
							mCurrentAnimation = AnimationState.Run;
						}
					}
				}
				else if(sPosition.y < mGroundY)
				{
					sPosition.y += JumpSpeed * Time.fixedDeltaTime;
					
					if (sPosition.y >= mGroundY)
					{
						sPosition.y = mGroundY;
						
						if (mCurrentAnimation != AnimationState.Run)
						{
							mAnimator.Play("run");
							mCurrentAnimation = AnimationState.Run;
						}
					}
				}
			}

			mTransform.localPosition = sPosition;
		}
		else
		{
		//	print ("AboutToDieAboutToDieAboutToDie");


			if (mRigidBody.simulated)
			{
				//print ("1");
				sHelperVector2.Set(mPlayer.CurrentSpeed, Gravity);
				mRigidBody.velocity = sHelperVector2;
			}
			else
			{
			//	print ("2");
				mRigidBody.simulated = true;
				mRigidBody.gravityScale = 1;
				this.collider2D.enabled = true;
				mGroundY = float.MaxValue;
			}
		}

		if (mNameLabel != null) UpdateNameLabel();
	}

	/**
	 * 
	 */
	void UpdateNameLabel()
	{
		Camera cam = LevelGenerator.mainCamera;
		Vector3 camPos = LevelGenerator.cameraTransform.localPosition;
		float x = (mTransform.localPosition.x - camPos.x) / (cam.aspect * cam.orthographicSize);
		float y = (mTransform.localPosition.y - camPos.y + LevelGenerator.CharacterHalfHeight +
		           LevelGenerator.CharacterHalfHeight) / cam.orthographicSize;
		
		sPosition.Set(x * LevelGenerator.uiCamera.aspect * LevelGenerator.uiRoot.activeHeight * 0.5f,
		              y * LevelGenerator.uiRoot.activeHeight * 0.5f, 0);
		mNameLabelTransform.localPosition = sPosition;
	}

	/**
	 * 
	 */
	void OnCollisionEnter2D (Collision2D c)
	{
		if (c.gameObject.tag == "platform")
		{


			int i;
			int count = c.contacts.Length;
			for(i=0; i<count; ++i)
			{
				if (c.contacts[i].normal.x >= 0.9 || c.contacts[i].normal.x <= -0.9)
				{
					Die ();
					break;
				}
			}

			if (this.enabled)
			{
				// Landed on platform, play run animation
				if (mCurrentAnimation != AnimationState.Run)
				{
					mAnimator.Play ("run");
					mCurrentAnimation = AnimationState.Run;
				}
			}
		}
		else if (c.gameObject.tag == "obstacle")
		{

			Die ();
		}
	}

	/**
	 * 
	 */
	void OnCollisionExit2D (Collision2D c)
	{


		if (c.gameObject.tag == "platform")
		{
			if (this.enabled)
			{
				if (mCurrentAnimation != AnimationState.Jump)
				{
					mAnimator.Play ("jump");
					mCurrentAnimation = AnimationState.Jump;
				}
			}
		}
	}

	/**
	 * 
	 */
	void Die()
	{

	//	print ("DieDieDieDie");

		if (mCurrentAnimation == AnimationState.Dead) return;

		if (mNameLabel != null)
		{
			GhostManager.ReleaseUserNameLabel(mNameLabel);
			mNameLabel = null;
		}

		mAnimator.Play("dead");
		mCurrentAnimation = AnimationState.Dead;
		this.enabled = false;
		gameObject.rigidbody2D.AddForce(mPlayer.ForceOnDeath - mRigidBody.velocity, ForceMode2D.Impulse);
		gameObject.collider2D.enabled = false;
		gameObject.rigidbody2D.gravityScale = mPlayer.GravityScaleOnDead;
	}

	/**
	 * 
	 */
	void OnBecameInvisible()
	{
		if(!this.enabled)
		{
			if(mCurrentAnimation == AnimationState.Dead) GhostManager.ReleaseGhost(this);
		}
		else
		{
			if (gameObject.collider2D.enabled && LevelGenerator.cameraTransform != null)
			{
				if (mTransform.localPosition.y < LevelGenerator.cameraTransform.localPosition.y -
				    LevelGenerator.mainCamera.orthographicSize)
				{
					Die();
				}
			}
		}
	}

	/**
	 * 
	 */
	IEnumerator SetNameLabel(string text)
	{
		yield return new WaitForFixedUpdate();
		//if(mNameLabel != null) mNameLabel.text = text;
	}
}
