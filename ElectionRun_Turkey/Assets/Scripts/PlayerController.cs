using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;


public class PlayerController : MonoBehaviour
{
	/// <summary>
	/// The minimum highscore offset (from current run distance) when querying highscores from server.
	/// </summary>
	public static int MinHighscoreOffset = 20;

	/// <summary>
	/// The maximum highscore offset (from current run distance) when querying highscores from server.
	/// </summary>
	public static int MaxHighscoreOffset = 200;

	/// <summary>
	/// Background's parallax factor.
	/// </summary>
	public static float FarParallaxFactor = -0.3f;

	/// <summary>
	/// Player's acceleration per second.
	/// </summary>
	public float AccelerationPerSec = 0.03f;

	/// <summary>
	/// Background image color at night.
	/// </summary>
	public Color BackgroundNightColor = new Color(128/255, 61/255, 45/255);
	public Color BackgroundEveningColor = new Color(120/255,51/255,51/255);

	/// <summary>
	/// How many seconds a day is.
	/// </summary>
	public float DayLength = 10;
	public int curTime = 1;
	float prevT = 0;

	public float MinJumpHeight = 0.5f;

	/**
	 * Some helper vector to prevent instantiation/"new" calls.
	 */
	private static Vector2 sHelperVector2 = new Vector2();
	private static Vector3 sHelperVector3 = new Vector3();

	/**
	 * 
	 */
	public int Coins = 0;
	public float MetersRun = 0;
	public bool IsJumping = false;
	public float JumpSpeed = 1;
	public float JumpHeight = 10;
	public float DoubleJumpHeight = 10;
	public bool IsDead = false;
	public float GravityScaleOnDead = 1;
	public Vector2 ForceOnDeath = new Vector2(-5, 20);
	public float CameraXOffset = -2;

	/**
	 * Sounds
	 */
	public AudioClip jumpClip;
	public AudioClip altJumpClip;
	public AudioClip coinClip;
	public AudioClip dropClip;
	public AudioClip deathClip;
	public AudioClip bubblePopClip;
	public AudioClip powerupClip;
	

	/// <summary>
	/// GameObject containing the GUI for name input
	/// </summary>
//	public GameObject NameInputGUI;

	public GameObject ScreenFlash;

	/**
	 * 
	 */
	float mGameStartTime;
	float mSpeed;
	float mFallSpeed;
	float mJumpLimit = -1;
	float mJumpLimitY = -1;
	int mJumpNum = 0;
	bool mHitJumpLimit = false;
	Transform mPlayerTransf;
	Vector3 mCameraPosition = new Vector3();
	HUDController mHUDController;
	GameObject mPlayerMarker;
	GameObject mHat;
	IEnumerator mGameOverCoroutine;
	bool mGotFirstCoin = false;

	// Bubble shield
	GameObject mBubbleShield;
	float mBubbleEndTime = -1;
	bool mBubbleActive = false;

	/**
	 * 
	 */
	Transform mCameraTransf;
	float mCameraHWidth;
	float mCameraWidth;

	/**
	 * 
	 */
	Transform mBackgroundTransform1;
	Transform mBackgroundTransform2;
	SpriteRenderer mBGSpriteRenderer1;
	SpriteRenderer mBGSpriteRenderer2;

	/**
	 * 
	 */
	Animator mAnimator;
	Rigidbody2D mRigidBody;

	/**
	 * 
	 */
	HighscoreManager mHighscoreMgr;
	Highscore.ScoreEntry mBestScore = new Highscore.ScoreEntry();
	float mLastBestScoreQueryTime = -1;

	//
	//
	//
//	public static GameObject WearHat(string hatName, Transform headTransform)
//	{
//		// Equip hat
//		GameObject prefab = Resources.Load<GameObject>("Hats/" + StoreController.GetHatInfo(hatName).PrefabName);
//		GameObject hat = Instantiate(prefab) as GameObject;
//		hat.name = Config.instance.CurrentHat;
//		hat.transform.parent = headTransform;
//		hat.transform.localPosition = prefab.transform.localPosition;
//		hat.transform.localRotation = prefab.transform.localRotation;
//		return hat;
//	}
	
	/// <summary>
	/// Gets current player's speed.
	/// </summary>
	/// <value>The current player's speed.</value>
	public float CurrentSpeed
	{
		get { return mSpeed; }
	}

	/// <summary>
	/// Gets a cached reference to player's transform for quicker access.
	/// </summary>
	/// <returns>Player character's transform.</returns>
	public Transform getTransform()
	{
		return mPlayerTransf;
	}

	public bool isShieldOn
	{
		get { return mBubbleActive; }
	}

	/// <summary>
	/// Resets the player when starting/restarting game.
	/// </summary>
	public void Restart()
	{
		InitStats();
		mGameStartTime = Time.time;
		IsDead = false;
		Coins = 0;
		MetersRun = 0;
		curTime = 1;
		prevT = 0;
		this.enabled = true;
		gameObject.collider2D.enabled = true;
		mBubbleShield.SetActive(false);
		mBubbleActive = false;
		mRigidBody.gravityScale = 1;
		sHelperVector2.Set(mSpeed, 0);
		mRigidBody.velocity = sHelperVector2;
		mAnimator.Play("run");

		if (mGameOverCoroutine != null)
		{
			StopCoroutine(mGameOverCoroutine);
			mGameOverCoroutine = null;
		}

//		if (mHat != null && mHat.name != Config.instance.CurrentHat)
//		{
//			// Hat changed. Remove current hat.
//			DestroyImmediate(mHat);
//			mHat = null;
//		}

//		if (Config.instance.CurrentHat.Length > 0 && mHat == null)
//		{
//			mHat = WearHat(Config.instance.CurrentHat, mPlayerTransf.FindChild("Head"));
//		}

		UpdateBackground();
		mHUDController.gameObject.SetActive(true);
		mHUDController.SetCoins(Coins);
		mHUDController.SetMeters((int)MetersRun);
	}

	//
	//
	//
	public void OnShowLeaderboard()
	{
		#if (UNITY_ANDROID || UNITY_IPHONE)
		Social.ShowLeaderboardUI();
		#endif
	}

	//
	//
	//
	public void UpdateCamera()
	{
		// Update camera position
		mCameraPosition = Camera.main.transform.localPosition;
		mCameraPosition.x = transform.localPosition.x + CameraXOffset;
		Camera.main.transform.localPosition = mCameraPosition;
	}

	/// <summary>
	/// Initialize player stats (speed, Jump height, double jump height) based on upgrade levels.
	/// </summary>
	void InitStats()
	{
		Config config = GameObject.Find("GameData").GetComponent<Config>();
		mSpeed = config.SpeedUpgrades[config.CurrentSpeedLevel];
		JumpHeight = config.JumpHeightUpgrades[config.CurrentJumpHeightLevel];
		DoubleJumpHeight = config.DoubleJumpHeightUpgrades[config.CurrentDoubleJumpHeightLevel];
	}

	/// <summary>
	/// Initialization after loading
	/// </summary>
	void Awake()
	{

		mBestScore.Score = 0;
		InitStats();
		mCameraTransf = Camera.main.transform;
		mCameraHWidth = Camera.main.aspect * Camera.main.orthographicSize;
		mCameraWidth = mCameraHWidth * 2;
		mPlayerTransf = gameObject.transform;
		mHUDController = GameObject.Find("HUD").GetComponent<HUDController>();
		mPlayerMarker = mPlayerTransf.Find("Marker").gameObject;
		mBubbleShield = mPlayerTransf.Find("BubbleShield").gameObject;
		mAnimator = GetComponent<Animator>();
		mRigidBody = GetComponent<Rigidbody2D>();

		// Background
		GameObject bg = GameObject.Find("Background1");
		mBGSpriteRenderer1 = bg.GetComponent<SpriteRenderer>();
		Vector2 bgSourceSize = mBGSpriteRenderer1.sprite.bounds.size;
		// BG 1
		mBackgroundTransform1 = bg.GetComponent<Transform>();
		// BG 2
		mBackgroundTransform2 = GameObject.Find("Background2").GetComponent<Transform>();
		mBGSpriteRenderer2 = mBackgroundTransform2.GetComponent<SpriteRenderer>();
		// BG Scale
		mBackgroundTransform1.localScale = mBackgroundTransform2.localScale =
				new Vector3(mCameraWidth / bgSourceSize.x, Camera.main.orthographicSize * 2 / bgSourceSize.y, 1);
		// BG Position
		sHelperVector3.Set(mCameraTransf.localPosition.x * FarParallaxFactor, 0, 10);
		mBackgroundTransform2.localPosition = sHelperVector3;
		sHelperVector3.x -= mCameraWidth;
		mBackgroundTransform1.localPosition = sHelperVector3;

		// Highscores
		mHighscoreMgr = GameObject.Find("GameData").GetComponent<HighscoreManager>();
		mHighscoreMgr.OnScoreListUpdated = OnHighscoreListUpdated;

		// Update HUD
		mHUDController.SetCoins(Coins);
		mHUDController.SetMeters(0);

		mCameraPosition = mCameraTransf.localPosition;
		mCameraPosition.x = mPlayerTransf.localPosition.x + CameraXOffset;
		mCameraTransf.localPosition = mCameraPosition;
	}

	/// <summary>
	/// Object is enabled when game restarts. Do some initialization here.
	/// </summary>
	/// 
	/// 	//Election Sounds
	AudioClip ChosenSound;
	public AudioClip[] ahmetAudio;
	public AudioClip[] cemAudio;
	public AudioClip[] devletAudio;
	public AudioClip[] doguAudio;
	public AudioClip[] emineAudio;
	public AudioClip[] floatingAudio;
	public AudioClip[] kemalAudio;
	public AudioClip[] mustafaAudio;
	public AudioClip[] selahattinAudio;



	void OnEnable()
	{
		//Election Change
		// Pick a boy/girl sprite based on player setting
		Config config = Config.instance;
		//print (config.CurrentCharacter);
		int randomNum = Random.Range(0,2);
		//print ("randomNum" + randomNum);
		//"ahmet","cem","devlet","dogu","emine","floating","kemal","mustafa","selahattin


		if (config.CurrentCharacter == "ahmet")
		{
			mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(config.ahmetAnimator);
			ChosenSound = ahmetAudio[0];
		}
		if (config.CurrentCharacter == "cem")
		{
			mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(config.cemAnimator);
			ChosenSound = cemAudio[0];
		}
		if (config.CurrentCharacter == "devlet")
		{
			mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(config.devletAnimator);
			ChosenSound = devletAudio[0];
		}
		if (config.CurrentCharacter == "dogu")
		{
			mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(config.dogunAnimator);
			ChosenSound = doguAudio[0];
		}
		if (config.CurrentCharacter == "emine")
		{
			mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(config.emineAnimator);
			ChosenSound = emineAudio[0];
		}
		if (config.CurrentCharacter == "floating") 
		{
			mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(config.floatingAnimator);
			ChosenSound = floatingAudio[0];
		}
		if (config.CurrentCharacter == "kemal")
		{
			mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(config.kemalAnimator);
			ChosenSound = kemalAudio[0];
		}
		if (config.CurrentCharacter == "mustafa")
		{
			mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(config.mustafaAnimator);
			ChosenSound = mustafaAudio[0];
		}
		if (config.CurrentCharacter == "selahattin")
		{
			//print("A " + randomNum);
			mAnimator.runtimeAnimatorController = Multires.GetAnimatorController(config.selahattinAnimator);
			//print("B " +randomNum);
			ChosenSound = selahattinAudio[0];
			//print("C " +randomNum);
		}


		//print (config.CurrentCharacter + "    config.CurrentCharacter");

		mPlayerMarker.SetActive(true);
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (!IsDead)
		{
			sHelperVector2 = mRigidBody.velocity;

			// Jump button/key is pressed?
			bool bJump = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);

			if (!bJump)
			{
				// Not jumping
				IsJumping = false;
				mHitJumpLimit = false;
			}
			else
			{
				sHelperVector3 = mPlayerTransf.localPosition;

				// Only allow a jump to start when:
				// - Player haven't done double jump or any jump at all
				// - Haven't hit the max jump height
				// - First frame when jump button is pressed
				if ( !IsJumping && mJumpNum < 2 &&
				     (!mHitJumpLimit || (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))) )
				{
					// Start a jump - could be the first or the second/double jump.

					++mJumpNum;
					IsJumping = true;
					mHitJumpLimit = false;

					// Start jump's animation
					mAnimator.Play("jump", -1, 0);

					if (mJumpNum == 1) mJumpLimit = JumpHeight;
					else mJumpLimit = DoubleJumpHeight;

					mJumpLimitY = sHelperVector3.y + mJumpLimit;

					// Play jump sound
					int r = Random.Range(0,2);
					if(r == 0)	AudioSource.PlayClipAtPoint(jumpClip,transform.position);
					else AudioSource.PlayClipAtPoint(altJumpClip,transform.position);
				}

			}

			if (IsJumping)
			{
				// Keep jumping
				// - Only when jump height limit is not reached
				float jumpHeight = mJumpLimitY - sHelperVector3.y;
				if (jumpHeight > 0)
				{
					if (jumpHeight > MinJumpHeight)
					{
						jumpHeight = MinJumpHeight;
					}
					else
					{
						IsJumping = false;
						mHitJumpLimit = true;
					}

					sHelperVector2.y = Mathf.Sqrt(2 * jumpHeight * -Physics2D.gravity.y);
				}
			}

			// Make sure the horizontal speed is constant
			sHelperVector2.x = mSpeed;
			mRigidBody.velocity = sHelperVector2;


			// Track run distance.
			MetersRun += mSpeed * Time.fixedDeltaTime;

			// Acceleration
			mSpeed += AccelerationPerSec * Time.fixedDeltaTime;

			// Update HUD
			mHUDController.SetMeters((int)MetersRun);

			// Update camera position
			mCameraPosition = mCameraTransf.localPosition;
			mCameraPosition.x = mPlayerTransf.localPosition.x + CameraXOffset;
			mCameraTransf.localPosition = mCameraPosition;

			//
			// Update background with parallax effect
			//
			UpdateBackground();

			//
			// Animate background color to emulate day to night changes
			//
			float t = (Time.time - mGameStartTime) % DayLength;
			if(prevT > t)
			{
				curTime = (curTime + 1)% 3;
			}

			if (curTime == 1)
			{
				mBGSpriteRenderer1.color = mBGSpriteRenderer2.color = Color.Lerp(Color.white, BackgroundEveningColor, t/DayLength);
			}
			else if (curTime == 2)
			{
				mBGSpriteRenderer1.color = mBGSpriteRenderer2.color = Color.Lerp(BackgroundEveningColor, BackgroundNightColor, t/DayLength );
			}
			else
			{
				mBGSpriteRenderer1.color = mBGSpriteRenderer2.color = Color.Lerp(BackgroundNightColor, Color.white, t/DayLength);
			}

			prevT = t;

			// Bubble shield
			if (mBubbleActive)
			{
				if (Time.time >= mBubbleEndTime)
				{
					PopBubble();
				}
			}
			#if UNITY_ANDROID
//			if (MetersRun >= AppManager.RUNNING_LIKE_THE_WIND_DISTANCE && Social.localUser.authenticated)
//			{
//				// Run like the wind
//				Social.ReportProgress(AppManager.ACHIEVEMENT_RUNNING_LIKE_THE_WIND, 100, null);
//			}
			#endif
			#if UNITY_IPHONE
//			if (MetersRun >= AppManager.RUNNING_LIKE_THE_WIND_DISTANCE_IOS && Social.localUser.authenticated)
//			{
//				// Run like the wind
//				Social.ReportProgress(AppManager.ACHIEVEMENT_RUNNING_LIKE_THE_WIND_IOS, 100, null);
//			}
			#endif
		}
	}

	//
	//
	//
	void UpdateBackground()
	{
		//
		// Update background with parallax effect
		//
		sHelperVector3.Set(mCameraPosition.x * FarParallaxFactor % mCameraWidth, 0, 10);
		mBackgroundTransform2.localPosition = sHelperVector3;
		
		if (mBackgroundTransform2.localPosition.x < 0)
		{
			sHelperVector3 = mBackgroundTransform2.localPosition;
			sHelperVector3.x += mCameraWidth - 0.005f;
		}
		else if (mBackgroundTransform2.localPosition.x > 0)
		{
			sHelperVector3 = mBackgroundTransform2.localPosition;
			sHelperVector3.x -= mCameraWidth - 0.005f;
		}
		
		mBackgroundTransform1.localPosition = sHelperVector3;
	}

	/// <summary>
	/// Handles collision between character and other game objects.
	/// </summary>
	/// <param name="c">C.</param>
	void OnCollisionEnter2D (Collision2D c)
	{
		int i;
		int count;
		bool isObstacle;
		
		if ( (c.gameObject.tag == "platform") || (isObstacle = c.gameObject.tag == "obstacle") )
		{
			// Player hits/lands on a platform/obstacle
			
			count = c.contacts.Length;
			for(i=0; i<count; ++i)
			{
				// If player runs into (instead of lands on) any platform, kill him.
				if (c.contacts[i].normal.x >= 0.9f || c.contacts[i].normal.x <= -0.9f)
				{
					// In protection of bubble shield?
					if (isObstacle && mBubbleActive)
					{
						// Ignore obstacle
						c.collider.isTrigger = true;
						
						// Disable shield. Shield only protect once.
						PopBubble();
						
						return;
					}

					Die ();
					return;
				}
			}
			
			mJumpNum = 0;
			mAnimator.Play("run");
			AudioSource.PlayClipAtPoint(dropClip,transform.position);
			mPlayerTransf.Find("Smoke").gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// Handles collision between player and triggers.
	/// </summary>
	/// <param name="c">C.</param>
	void OnTriggerEnter2D(Collider2D c)
	{
		if (c.gameObject.tag == "coin")
		{
			// Player hits a coin. Collect coin.

			c.enabled = false;
		//	print (c + "??????");

			++Coins;
			mHUDController.SetCoins(Coins);
			c.gameObject.transform.parent = null;
			c.gameObject.SetActive(false);

		//	c.GetComponent<Animator>().Play("CoinDeath");

			// Play collect coin sound
			AudioSource.PlayClipAtPoint(coinClip,transform.position);

			// Achievement
//			#if UNITY_IPHONE
//
//			// You can also call into the functions like this
//			Social.ReportProgress ("FC1234", 100.0, result => {
//						if (result)
//							Debug.Log ("Successfully reported achievement progress");
//						else
//							Debug.Log ("Failed to report achievement");
//					});
//			#endif

			#if UNITY_ANDROID
			if (Social.localUser.authenticated && !mGotFirstCoin) {
				mGotFirstCoin = true;
				Social.ReportProgress(AppManager.ACHIEVEMENT_FIRST_COIN, 100, null);
			}
			#endif

			#if UNITY_IPHONE
			if (Social.localUser.authenticated && !mGotFirstCoin) {
				mGotFirstCoin = true;
				Social.ReportProgress(AppManager.ACHIEVEMENT_FIRST_COIN_IOS, 100, null);
			}
			#endif


		}
		else if (c.gameObject.tag == "bubble")
		{
			// Got a bubble shield!

			c.gameObject.SetActive(false);
			mBubbleShield.SetActive(true);
			mBubbleShield.GetComponent<Animator>().Play("start");
			mBubbleEndTime = Time.time + Config.instance.BubbleShieldDuration;
			mBubbleActive = true;
			AudioSource.PlayClipAtPoint(powerupClip, mPlayerTransf.localPosition);
		}
	}

	/// <summary>
	//my change
	public HUDController hUDController;
	public AddedFuncionallity addedFuncionallity;
	public int TimesToShowVideoinADay;
	public GameOverGUIController gameOverGUIController;


	public int TimesDied = 0;

	void Die ()
	{

		TimesDied ++;
		//print (TimesDied);
		if (TimesDied == 3)
		{
			MyHeyzap.instence.ShowInterstitialAd();
			TimesDied = 0;
		}
		


		hUDController.WhenDie();

		if (TimesToShowVideoinADay == 11)
		{
		TimesToShowVideoinADay = PlayerPrefs.GetInt("TimesToShowVideoinADay");
		TimesToShowVideoinADay += 1;
		PlayerPrefs.SetInt ("TimesToShowVideoinADay", TimesToShowVideoinADay);
		}
	//	print ("TimesToShowVideoinADay" + PlayerPrefs.GetInt ("TimesToShowVideoinADay"));


		addedFuncionallity.UpdateHatsIcon();
		addedFuncionallity.UpdateUpgradesIcon();

		if (PlayerPrefs.GetFloat("BestScore") < MetersRun)
		{ 
			PlayerPrefs.SetFloat("BestScore", MetersRun);
		}

		if (IsDead == false)
		{
			IsDead = true;
			mPlayerMarker.SetActive(false);
			mAnimator.Play("dead");

			AudioSource.PlayClipAtPoint(ChosenSound,transform.position);

			this.enabled = false;
			gameObject.collider2D.enabled = false;

			if (mBubbleActive) PopBubble();

			// Apply a force so that the player is "thrown" when he dies.
			mRigidBody.AddForce(ForceOnDeath + new Vector2(-mRigidBody.velocity.x, -mRigidBody.velocity.y), ForceMode2D.Impulse);

			// Gravity force on player is strengthen when he dies for aesthetics.
			mRigidBody.gravityScale = GravityScaleOnDead;

			if (LevelGenerator.instance.IsChallengeRun()) {
				StartCoroutine(mGameOverCoroutine = ShowChallengeResult());
			}
			else {
				// Show the game over screen
				StartCoroutine(mGameOverCoroutine = ShowGameEndScreen());
				//Analytics
				Analytics.gua.sendAppScreenHit("Game Over");
			}

			// Screen flash
			ScreenFlash.SetActive(true);
			ScreenFlash.GetComponent<Animator>().Play("screenFlash");

			// Analytics
			Analytics.gua.sendEventHit("Action","Dies" +  Config.instance.CurrentCharacter, MetersRun.ToString(), (int)MetersRun);

			// Achievement

			#if UNITY_IPHONE
			// You can also call into the functions like this
//
//					Social.ReportProgress ("Achievement01", 100.0, result => {
//						if (result)
//							Debug.Log ("Successfully reported achievement progress");
//						else
//							Debug.Log ("Failed to report achievement");
//					});
			#endif

			#if UNITY_ANDROID
			if (Social.localUser.authenticated) {

				// First death
				//Social.ReportProgress(AppManager.ACHIEVEMENT_BOOM_YOURE_DOWN, 100, null);

				hUDController.TellGooglePlayXPAchivments();
				// 100 Coins
				if (Config.instance.LifetimeCoins >= 50)
				{
					Social.ReportProgress(AppManager.ACHIEVEMENT_100_COIN, 100, null);
				}
				// 500 Coins
				if (Config.instance.LifetimeCoins >= 50)
				{
					Social.ReportProgress(AppManager.ACHIEVEMENT_500_COIN, 100, null);
				}
				// 1000 Coins
				if (Config.instance.LifetimeCoins >= 50)
				{
					Social.ReportProgress(AppManager.ACHIEVEMENT_1000_COIN, 100, null);
				}
			}



			#endif
//			#if UNITY_IPHONE
//			if (Social.localUser.authenticated) {
//				
//				// First death
//				Social.ReportProgress(AppManager.ACHIEVEMENT_BOOM_YOURE_DOWN_IOS, 100, null);
//				
//				// 50 Coins
//				if (Config.instance.LifetimeCoins >= 50)
//				{
//					Social.ReportProgress(AppManager.ACHIEVEMENT_50_COIN_IOS, 100, null);
//				}
//			}
//			#endif
		}
	}

	void PopBubble()
	{
		mBubbleActive = false;
		mBubbleShield.GetComponent<Animator>().Play("stop");
		AudioSource.PlayClipAtPoint(bubblePopClip, mPlayerTransf.localPosition);
	}

	IEnumerator ShowChallengeResult()
	{
		yield return new WaitForSeconds(1);

		if ( !LevelGenerator.instance.IsChallengeWon() )
		{
			// Lost challenge
			LevelGenerator.uiRoot.transform.FindChild("ConfirmationDialog").gameObject.SetActive(true);
			DialogController.instance.Closed += OnConfirmRetryChallenge;
			DialogController.instance.SetMessage("? "+ " בוש הסנ" +" ,חצינ" + " " + LevelGenerator.instance.challengerName + " ");
		


			mGameOverCoroutine = null;
		}
		else
		{
			// Win challenge
			GameObject challengeComplete = LevelGenerator.uiRoot.transform.FindChild("ChallengeComplete").gameObject;
			challengeComplete.SetActive(true);
			challengeComplete.transform.FindChild("youWon").GetComponent<UILabel>().text = "You beat " + LevelGenerator.instance.challengerName;


			// Send message to challenger
			FacebookManager.instance.ChallengeFriend(
				"I ran " + ((int)MetersRun).ToString() + " m and beat your score. Try to beat mine?",
				new string[] { LevelGenerator.instance.challengerFBID },
				MetersRun.ToString() + "," + Config.instance.CurrentCharacter,
				"I beat you"

			);


			yield return new WaitForSeconds(2);

			LevelGenerator.uiRoot.transform.FindChild("ChallengeComplete").gameObject.SetActive(false);
			StartCoroutine(mGameOverCoroutine = ShowGameEndScreen(true));
		}
	}

	void OnConfirmRetryChallenge(bool ok)
	{
		DialogController.instance.Closed -= OnConfirmRetryChallenge;

		if (ok)
		{
			LevelGenerator levelGen = LevelGenerator.instance;
			levelGen.RestartGame(levelGen.challengerName, levelGen.challengerDistance, levelGen.challengerHat, levelGen.challengerFBID, levelGen.challengerCharacter);
		}
		else
		{
			StartCoroutine(mGameOverCoroutine = ShowGameEndScreen(true));
		}

	}

	/// <summary>
	/// Shows the game over screen after a few moment delay.
	/// </summary>
	/// <returns>The game end screen.</returns>
	IEnumerator ShowGameEndScreen(bool immediate = false)
	{
		if(!immediate) yield return new WaitForSeconds(1);

		LevelGenerator levelGen = LevelGenerator.instance;

		if (!gameObject.activeSelf) yield return null;

		// Activate the game over GUI
		mHUDController.gameObject.SetActive(false);

		// Stats
		GameOverGUIController gameOverGUI = levelGen.GameEndScreen.GetComponent<GameOverGUIController>();
		gameOverGUI.MetersRun = MetersRun;
		gameOverGUI.CoinsCollected = Coins;

		// Retrieve best score and compare with player's result and
		// display text: xx Meters more to rank 1st
		GameObject go = levelGen.GameEndScreen.transform.FindChild("MetersToGo").gameObject;
		UILabel label = go.GetComponent<UILabel>();
		go.SetActive(false);

		// Only query for best score if it's been 10 seconds since last query
		if (mLastBestScoreQueryTime == -1 || mLastBestScoreQueryTime > Time.time + 10) {
			StartCoroutine( Highscore.GetBestScore(OnGetBestScore, label) );
			mLastBestScoreQueryTime = Time.time;
		}
		else {
			OnGetBestScore(mBestScore, label);
		}

		// Add coins collected during this run to the bank
		Config config = GameObject.Find("GameData").GetComponent<Config>();
		config.Coins += Coins;
		config.LifetimeCoins += Coins;

		// Save game
		config.Save();

//		if (config.PlayerName.Length == 0)
//		{
//			// No player name yet, Ask for name input
//
//			NameInputGUI.SetActive(true);
//			NameInputGUI.GetComponent<NameInputController>().OnNameEntered = OnNameEntered;
//		}
//		else
//		{
			// Display game over GUI
			levelGen.GameEndScreen.SetActive(true);

			gameOverGUI.StartCounter();

			// Submit highscore to server
			SubmitHighscore();
//		}

		mGameOverCoroutine = null;

	}

	//
	//
	//
	void SubmitHighscore()
	{
		if (MetersRun < 3500)
		{
		StartCoroutine( Highscore.PostScore(Config.instance.CurrentCharacter, (int)MetersRun) );
		}

		#if (UNITY_ANDROID)
		if (Social.localUser.authenticated) {
			Social.ReportScore((int)MetersRun, AppManager.LEADERBOARD_ID, null);
		}
		#endif
		#if UNITY_IPHONE

		if (Social.localUser.authenticated) {

			Social.ReportScore((int)MetersRun, AppManager.LEADERBOARD_ID_IOS, null);
		}
	
		#endif
	}

	/// <summary>
	/// OnGetBestScore is called when highscore is received from the server.
	/// </summary>
	/// <param name="best">Best score entry.</param>
	/// <param name="userData">Any user data set by the caller.</param>
	void OnGetBestScore(Highscore.ScoreEntry best, Object userData)
	{
		mBestScore.Name = best.Name;
		mBestScore.Score = best.Score;
		//firstPlaceLabel.enabled = false;
		int togo = best.Score - (int)MetersRun;

		//print (togo);
		//print (best.Score);


		if (togo > 0 )
		{
			UILabel label = (UILabel)userData;
			label.gameObject.SetActive(true);
			label.text = togo.ToString();

			TweenScale tween = TweenScale.Begin(label.gameObject, 0.2f, new Vector3(1.3f, 1.3f, 1));
			tween.method = UITweener.Method.EaseOut;
			EventDelegate.Add(tween.onFinished, OnScaleupTweenFinished);

			if (best.Score <= (int)MetersRun)
			{
				print ("SMALLLLER !!!");
				//firstPlaceLabel.enabled = true;
			}
		}

		/// my change
		float togoXP = PlayerPrefs.GetFloat ("NextBestScore") - MetersRun;


		if (togoXP > 0)
		{
			MetersToGoXPlabel.gameObject.SetActive(true);
			MetersToGoXPlabel.text = Mathf.Round (togoXP).ToString();
			
			TweenScale tween = TweenScale.Begin(MetersToGoXPlabel.gameObject, 0.2f, new Vector3(1.3f, 1.3f, 1));
			tween.method = UITweener.Method.EaseOut;
			EventDelegate.Add(tween.onFinished, OnScaleupTweenFinished);
		}

		LevelGenerator levelGen = LevelGenerator.instance;

//		if (best.Score <= (int)MetersRun)
//		{
//			print ("SMALLLLER !!!");
//			firstPlaceLabel.enabled = true;
//		}
//		else{
//			firstPlaceLabel.enabled = false;
//		}
	}
	/// my change
	public UILabel MetersToGoXPlabel;
	//public UILabel firstPlaceLabel;

	//
	//
	//
	void OnScaleupTweenFinished()
	{
		TweenScale tween = TweenScale.Begin(UITweener.current.gameObject, 0.2f, Vector3.one);
		tween.method = UITweener.Method.EaseOut;
	}

	/// <summary>
	/// OnNameEntered is called after player finishes name input.
	/// </summary>
	/// <param name="name">The name typed by player.</param>
	void OnNameEntered(string name)
	{
		Config config = GameObject.Find("GameData").GetComponent<Config>();
		config.PlayerName = name;
		config.Save();

		SubmitHighscore();

		LevelGenerator levelGen = GameObject.Find("LevelController").GetComponent<LevelGenerator>();
		levelGen.GameEndScreen.SetActive(true);
		levelGen.GameEndScreen.GetComponent<GameOverGUIController>().StartCounter();
	}

	/// <summary>
	/// OnHighscoreListUpdated() is called after highscore list is received from server.
	/// </summary>
	void OnHighscoreListUpdated()
	{
		/*if (!IsDead && gameObject.activeSelf && GhostManager.numActiveGhosts < Config.instance.GhostPopulation)
		{
			HighscoreManager.EnumerateScoresBetween(
				(int)(mPlayerTransf.localPosition.x + MinHighscoreOffset + Random.value * MaxHighscoreOffset * 0.5),
				(int)mPlayerTransf.localPosition.x + MaxHighscoreOffset,
				OnEnumScores
			);
		}*/
	}

	IEnumerator RetrieveHighscores()
	{
		while(true)
		{
			if (!IsDead && gameObject.activeSelf)
			{
				if (GhostManager.numActiveGhosts < Config.instance.GhostPopulation)
				{
					StartCoroutine( HighscoreManager.RequestScores(
						(int)mPlayerTransf.localPosition.x + MinHighscoreOffset,
						(int)mPlayerTransf.localPosition.x + MaxHighscoreOffset
						) );
				}

				yield return new WaitForSeconds(5);
			}
			else
			{
				break;
			}
		}

		yield return null;
	}

	void OnBecameInvisible()
	{
		if (mCameraTransf != null)
		{
			if (mPlayerTransf.localPosition.y < mCameraTransf.localPosition.y - LevelGenerator.mainCamera.orthographicSize)
			{
				if (gameObject.collider2D.enabled && !IsDead)
				{
					Die();
				}
			}
		}
	}
}
