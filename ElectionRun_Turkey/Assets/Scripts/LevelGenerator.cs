using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;


public class LevelGenerator : MonoBehaviour
{
	/**
	 * 
	 */
	public enum JumpType : int
	{
		Platform = 0,
		Obstacle
	}

	/**
	 * 
	 */
	struct LevelSettings
	{
		public float MinPlatformDistance;
		public float MaxPlatformDistance;

		public float MinPlatformLength;
		public float MaxPlatformLength;
		
		public float MaxPlatformOffsetY;
	};

	/**
	 * 
	 */
	public class JumpInfo
	{
		public JumpType type;
		public Vector2 jumpStart;
		public Vector2 jumpPeak;
		public Vector2 jumpEnd;
	}

	//
	//
	//
	public static float CharacterHalfHeight;
	public static float CharacterHalfWidth;
	public const float ChallengerDistance = 2;

	//
	//
	//
	const float PlatformEdgeJaggedness = 0.8f;
	public const float PlatformDeleteDistance = 3;

	//
	// Player speed
	//
	static float Speed;

	//
	//
	//
	static Vector2 PlatformSurfaceVector = new Vector2(1, 0);
	static Vector2 InvPlatformSurfaceVector = new Vector2(-1, 0);

	/**
	 * 
	 */
	public GameObject PlatformPrefab;
	public GameObject CoinPrefab;
	public GameObject BubblePowerupPrefab;
	public List<GameObject> ObstaclePrefabs;
	public GameObject FirstPlatform;

	//
	// Foliage
	//
	public List<GameObject> WeedPrefabs;
	public List<GameObject> GrassPrefabs;
	public List<GameObject> TreePrefabs;
	public List<GameObject> HangingRootPrefabs;

	/**
	 * 
	 */
	public float InitialMaxPlatformOffsetY = 2f;
	public float InitialMinPlatformOffsetY = -3f;
	public float MaxPlatformOffsetYIncrementPerSec = 0.1f;
	public float AbsoluteMaxPlatformOffsetY = 4f;
	public float MaxPlatformOffsetY;
	public float MinPlatformOffsetY;

	/**
	 * Platform
	 */
	public float MaxPlatformY = -1;
	public float MinPlatformY = -3;

	public float MinPlatformDistance = 1f;
	public float MaxPlatformDistance = 4f;

	public float MinPlatformLength = 2;
	public float MaxPlatformLength = 9;

	/**
	 * Coin
	 */
	public float DistanceBetweenCoins = 0.05f;
	public float CoinHeightFromGround = 0.05f;
	public int MinCoinsOnPlatform = 1;

	//
	// Bubble powerup
	//
	public float BubbleSpawnInterval = 10;

	/**
	 * Obstacle
	 */
	public float ObstacleMinOffsetX = 1;	// Minimal x offset from start of platform
	public float ObstacleStartX = 60;

	/**
	 * 
	 */
	public GameObject GameEndScreen;

	/**
	 * 
	 */
	private static Vector3 sPosition = new Vector3();

	/**
	 * 
	 */
	private static LevelGenerator mInstance;


	public MyGameCenterScript myGameCenterScript;


	public UISprite FacebookButton;
	public UISprite exclamation;
	/**
	 * 
	 */
	Transform mTransform;
	Camera mCamera;
	Camera mUICamera;
	UIRoot mUIRoot;
	Transform mCameraTransform;
	PlayerController mPlayerController;

	/**
	 * Challenger
	 */
	string mChallengerName;
	float mChallengerDistance;
	string mChallengerHat;
	string mChallengerFBID;
	string mChallengerCharacter;

	/**
	 * Coin
	 */
	float mNextCoinTime = 3;
	float mCoinWidth = 0;
	float mCoinHWidth = 0;

	//
	// Bubble powerup
	//
	float mNextBubbleSpawnTime = -1;
	float mBubbleSpawnChance = 0;

	//
	// Scores
	//
	Highscore.ScoreEntry[] mScores;
	bool mScoresRetrieved = false;
	float mLastScoreQueryTime = -1;

	/**
	 * 
	 */
	float mNextObstacleTime = 6;

	/**
	 * Objects
	 */
	List<GameObject> mPlatforms = new List<GameObject>();
	List<GameObject> mCoins = new List<GameObject>();
	List<GameObject> mObstacles = new List<GameObject>();
	List<JumpInfo> mJumpInfos = new List<JumpInfo>();
	List<JumpInfo> mJumpInfoPool = new List<JumpInfo>();
	List<FoliageController> mWeeds = new List<FoliageController>();
	List<FoliageController> mGrasses = new List<FoliageController>();
	List<FoliageController> mTrees = new List<FoliageController>();
	List<FoliageController> mHangingRoots = new List<FoliageController>();
	GameObject mBubblePowerup;

	/**
	 * Initial state
	 */
	Vector3 mIniPlayerPosition;
	Vector2 mIniPlatformSize;
	Vector2 mIniPlatformPosition;
	Vector3 mIniGeneratorPosition;

	/**
	 * 
	 */
	public bool IsChallengeRun()
	{
		return mChallengerName != null;
	}

	/**
	 * 
	 */
	public bool IsChallengeWon()
	{
		return mPlayerController.MetersRun > mChallengerDistance;
	}

	/**
	 * 
	 */
	public string challengerName
	{
		get { return mChallengerName; }
	}

	/**
	 * 
	 */
	public string challengerHat
	{
		get { return mChallengerHat; }
	}

	/**
	 * 
	 */
	public string challengerFBID
	{
		get { return mChallengerFBID; }
	}

	/**
	 * 
	 */
	public string challengerCharacter
	{
		get { return mChallengerCharacter; }
	}

	/**
	 * 
	 */
	public float challengerDistance
	{
		get { return mChallengerDistance; }
	}

	/**
	 * 
	 */
	public JumpInfo GetJumpInfo(int index)
	{
		return mJumpInfos[index];
	}

	/**
	 * 
	 */
	public int GetPlatformIndex(GameObject platform)
	{
		return mPlatforms.IndexOf(platform);
	}

	/**
	 * 
	 */
	public GameObject GetPlatformAt(int index)
	{
		if (index < mPlatforms.Count) return mPlatforms[index];
		else return null;
	}

	/**
	 * 
	 */
	public GameObject GetFurthestActivePlatform()
	{
		return mPlatforms[mPlatforms.Count - 1];
	}

	/**
	 * 
	 */
	public static LevelGenerator instance
	{
		get { return mInstance; }
	}

	/**
	 * 
	 */
	public static Camera mainCamera
	{
		get { return mInstance.mCamera; }
	}

	/**
	 * 
	 */
	public static Transform cameraTransform
	{
		get { return mInstance.mCameraTransform; }
	}

	/**
	 * 
	 */
	public static Camera uiCamera
	{
		get { return mInstance.mUICamera; }
	}

	/**
	 * 
	 */
	public static UIRoot uiRoot
	{
		get { return mInstance.mUIRoot; }
	}

	//
	//
	//
	public static PlayerController playerController
	{
		get { return mInstance.mPlayerController; }
	}

	/**
	 *
	 */
	//my change
	public UpgradeScreenController upgradeScreenController;
	public GameObject PlaytButton;
	//public GameObject VButton;
	public GameObject upgradeScreen;
	//public GameObject MainMenuecreen;

	public void ShowUpgradeBeforePlay ()
	{
		//GameEndScreen.SetActive(false);
		OnUpgradeClick ();
		upgradeScreenController.Display ();


//		VButton.SetActive (false);
//		PlaytButton.SetActive (true);

			
	}


	public void OnRetryClick()
	{
		RestartGame();

		GameEndScreen.SetActive (false);

		// Analytics
		Analytics.gua.sendEventHit("Action", "Retry");

	}

	//my change 

	public void OnRetryClickfromUpgradeScreen()
	{

		//GameEndScreen.SetActive (false);
		RestartGame();

		GameEndScreen.SetActive (false);
		// Analytics
		Analytics.gua.sendEventHit("Action", "Retry");

		//my change 
		//upgradeScreen.SetActive (false);
		//PlaytButton.SetActive (false);
		//VButton.SetActive (true);

	}


	/**
	 *
	 */
	public void OnUpgradeClick()
	{
		GameEndScreen.SetActive(false);
		UpgradeScreenController.instance.Closed += UpgradeClosed;

		AppManager.HideBannerAd();
	}


	/**
	 * 
	 */
	public void UpgradeClosed(object sender, System.EventArgs args)
	{
		UpgradeScreenController.instance.Closed -= UpgradeClosed;
		this.GameEndScreen.SetActive(true);

		//AppManager.ShowBannerAd();
	}

	/**
	 *
	 */
	public void OnQuitClick()
	{
		GameEndScreen.SetActive(false);
		Time.timeScale = 1;
		gameObject.SetActive (false);
	}
//	public void OnQuitClick2()
//	{
//		GameEndScreen.SetActive(false);
//		Time.timeScale = 1;
//		gameObject.SetActive (false);
//	}
	/**
	 * 
	 */
	public void CloseGame()
	{
		// Reset camera
		Camera.main.transform.localPosition = new Vector3(0, 0, -10);
		
		ResetPlatforms();
		ResetObstacles();
		ResetCoins();
		ResetFoliage();
		GhostManager.KillAllGhosts();

		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if(player != null) player.SetActive(false);
	}

	/**
	 * 
	 */
	public void PauseGame()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.GetComponent<PlayerController>().enabled = false;

		gameObject.SetActive(false);
	}

	/**
	 * 
	 */

	public void StartFromTutorial1()
	{
		//print (122);
		Time.timeScale = 1;
		Destroy (Tutorial1);
		RestartGame();
	}
	public void StartFromTutorial2()
	{
		//print (122);
		Time.timeScale = 1;
		Destroy (Tutorial2);
		RestartGame();
		FacebookButton.enabled = true;
		exclamation.enabled = true;
	}

	string UniqueCharecter;

	public void RestartGame(string opponentName = null, float opponentDistance = float.NaN, string opponentHat = null, string opponentFBID = null, string opponentCharacter = null)
	{

		HUDController.XPRefresh (); 
		//mChallengerName = null;
		//////MyChange
		PlayerPrefs.SetInt ("gameLevel", (PlayerPrefs.GetInt ("gameLevel") + 1));
		///My Change
		int test = PlayerPrefs.GetInt("gameLevel");
		//print (test);
		///My Change		
	if (test != 1 && test != 3) {

						if (UniqueCharecter != Config.instance.CurrentCharacter)
						{
						//print(Config.instance.CurrentCharacter);
						UniqueCharecter = Config.instance.CurrentCharacter;
						Analytics.gua.sendEventHit("Action","UniqueCharecter" , UniqueCharecter);
						}

						mChallengerName = opponentName;
						mChallengerDistance = opponentDistance ;
						mChallengerHat = opponentHat;
						mChallengerFBID = opponentFBID;
						mChallengerCharacter = opponentCharacter;

						// Reset camera
						mCameraTransform.localPosition = new Vector3 (0, 0, -10);

						ResetPlatforms ();
						ResetObstacles ();
						ResetCoins ();
						ResetFoliage ();
						GhostManager.KillAllGhosts ();
						if (mBubblePowerup != null)
						mBubblePowerup.SetActive (false);
						mNextObstacleTime = Time.timeSinceLevelLoad + 6;
						mNextCoinTime = Time.timeSinceLevelLoad + 3;
						MaxPlatformOffsetY = InitialMaxPlatformOffsetY;
						MinPlatformOffsetY = InitialMinPlatformOffsetY;

						// Bubble
						mNextBubbleSpawnTime = Time.time + BubbleSpawnInterval;
						mBubbleSpawnChance = Config.instance.BubbleUpgrades [Config.instance.CurrentBubbleLevel] * 0.01f;

						// Reset first platform
						GameObject platform = GetPlatform ();
						platform.SetActive (true);
						platform.transform.localPosition = mIniPlatformPosition;
						platform.GetComponent<Platform> ().SetSize (mIniPlatformSize);

						// Reset level generator
						mTransform.localPosition = mIniGeneratorPosition;

						// Reset player's position
						mPlayerController.transform.localPosition = mIniPlayerPosition;

						// Reset player
						mPlayerController.Restart ();

						Speed = mPlayerController.CurrentSpeed;
						CreateInitialPlatforms ();
						CreateGhosts (mPlayerController.transform.localPosition.x, platform, opponentName, opponentDistance, opponentHat);
						
						if (opponentName == null) {
						// My Change
				//print (opponentName);

								// Query scores only if it's been more than 10 seconds since last query
							if (opponentName == null){ //(mLastScoreQueryTime == -1 || mLastScoreQueryTime > Time.time + 10) {
				
										if (mScores == null || mScores.Length != Config.instance.GhostPopulation) {
												mScores = new Highscore.ScoreEntry[Config.instance.GhostPopulation];
										}
				
										mScoresRetrieved = false;
										StartCoroutine (Highscore.GetRandomScores (Config.instance.GhostPopulation, GetRandomScoresCb));
										mLastScoreQueryTime = Time.time;
								} else {
				
										// Reuse existing scores
										if (mScoresRetrieved) {


												int i;
												GhostController ghost;

												for (i = 0; i<Config.instance.GhostPopulation; ++i) {
														ghost = GhostManager.GetActiveGhost (i);
														ghost.name = mScores [i].Name;
														ghost.DiesAtMeters = mScores [i].Score;
														ghost.setLabel (GhostManager.GetUserNameLabel ());
												}
										}
								}
						} else {

							//print (opponentName);

								Time.timeScale = 0;
								uiRoot.transform.FindChild ("HUD").GetComponent<HUDController> ().SetEnable (false);

								GameObject countdown = uiRoot.transform.FindChild ("ReadyUp").gameObject;
								countdown.SetActive (true);
								countdown.GetComponent<Animator> ().Play ("ReadyUp");
								countdown.GetComponent<RaceCounterController> ().finished += OnCountDownFinished;
								countdown.transform.Find ("Name").gameObject.GetComponent<UILabel> ().text =
								"You" + " vs " + opponentName;
								
								// Update camera
								playerController.UpdateCamera ();
						}

						//  Close game over gui
						uiRoot.transform.FindChild ("upgrade_screen").gameObject.SetActive (false);

						// Close store gui
						uiRoot.transform.FindChild ("Store").gameObject.SetActive (false);

//						// Show banner ad
						//AppManager.HideBannerAd();

				}
		///My Change
		if (test == 3)
		{
			Time.timeScale = 0;
			Tutorial2.SetActive(true);
		}
		if (test == 1)
		{
			Time.timeScale = 0;
			Tutorial1.SetActive(true);
		}
	}


	//
	//
	//
	public void OnChallengeActivated(ChallengeDetail challenge)
	{
		if (!gameObject.activeSelf) return;
		
		RestartGame(challenge.Name, challenge.Distance, challenge.Hat, challenge.FacebookID, challenge.Character);

		Debug.Log (challenge.Name + challenge.Distance + challenge.Hat +  challenge.FacebookID +  challenge.Character);
		print(challenge.Name + challenge.Distance + challenge.Hat +  challenge.FacebookID +  challenge.Character);

		// Hide game end screen
		GameEndScreen.SetActive(false);
		
		// Analytics
		Analytics.gua.sendEventHit("Action", "Start Challenge");
	}

	/**
	 * 
	 */


	// Score query callback
	void GetRandomScoresCb(Highscore.ScoreEntry[] scores, int count)
	{
		// Cancel if:
		// - Error retrieving scores from server
		// - Player is dead
		// - Scores already retrieved (could happen from an earlier query call)
		if(count == -1 || mPlayerController.IsDead || mScoresRetrieved)
		{
			return;
		}
		
		int i;
		GhostController ghost;
		for(i=0; i<count && i<mScores.Length; ++i)
		{
			mScores[i].Name = scores[i].Name;
			mScores[i].Score = scores[i].Score;
			
			ghost = GhostManager.GetActiveGhost(i);
			ghost.name = scores[i].Name;
			ghost.DiesAtMeters = scores[i].Score;
			ghost.setLabel(GhostManager.GetUserNameLabel());
		}
		
		mScoresRetrieved = true;
	}

	//
	//
	//

	/// <summary>
	/// Election Change
	public static string ChosenCharecter;
	public static List<string> runnersNames = new List<string>();
	//public static List<string> randomRunnersNames = new List<string>();

	void CreateGhosts(float playerX, GameObject startPlatform, string opponentName = null, float opponentDistance = float.NaN, string opponentHat = null)
	{
		/// Election Change
		runnersNames = new List<string> {"ahmet","cem","devlet","dogu","emine","floating","kemal","mustafa","selahattin"};

		// Shuffle runnersNames List

		for (int i = 0; i < runnersNames.Count; i++) {
			string temp = runnersNames[i];
			int randomIndex = Random.Range(i, runnersNames.Count);
			runnersNames[i] = runnersNames[randomIndex];
			runnersNames[randomIndex] = temp;
		}

		if (opponentName != null)
		{
			//Election Change
			ChosenCharecter = mChallengerCharacter;
			GhostManager.AddGhost(playerX + ChallengerDistance , ChallengerDistance , opponentDistance, startPlatform, opponentName);

		}
		else
		{

			int i;
			float offset;

			float rightMostOffset1 = -float.MaxValue;
			float rightMostOffset2 = -float.MaxValue;
			GameObject rightMost1 = null;
			GameObject rightMost2 = null;
			GameObject ghost;
			string hat;

			float MyDistensefromPlayer =  Random.Range(0.3f,0.5f);
				//Election Change 
	

				for (int r = 0; r < runnersNames.Count; r++) 
				{
					
				
					if (r == 0) {
						offset = -2.3f;
					}
					else if (r == 1) {
						offset = -1.7f;
					}
					else {
						offset = MyDistensefromPlayer;
					}


					if (runnersNames[r] != Config.instance.CurrentCharacter)
					{
						ChosenCharecter = runnersNames[r].ToString();
						ghost = GhostManager.AddGhost(playerX + offset, offset , -1, startPlatform, null);
					}

				MyDistensefromPlayer += Random.Range(0.7f,1.3f);

				}

				runnersNames.Remove(ChosenCharecter);

		}
	}


	void CreateInitialPlatforms()
	{
		while (mTransform.localPosition.x < mCameraTransform.localPosition.x + mCamera.orthographicSize*mCamera.aspect)
		{
			CreatePlatform();
		}
	}

	/**
	 * 
	 */
	void ResetPlatforms()
	{
		foreach (GameObject platform in mPlatforms)
		{
			platform.SetActive(false);
		}
	}

	/**
	 * 
	 */
	void ResetObstacles()
	{
		foreach (GameObject obstacle in mObstacles)
		{
			obstacle.SetActive(false);
		}
	}

	/**
	 * 
	 */
	void ResetCoins()
	{
		foreach (GameObject coin in mCoins)
		{
			coin.SetActive(false);
		}
	}

	//
	//
	//
	void ResetFoliage()
	{
		foreach(FoliageController foliage in mWeeds)
		{
			foliage.gameObject.SetActive(false);
		}

		foreach(FoliageController foliage in mTrees)
		{
			foliage.gameObject.SetActive(false);
		}

		foreach(FoliageController foliage in mHangingRoots)
		{
			foliage.gameObject.SetActive(false);
		}

		foreach(FoliageController foliage in mGrasses)
		{
			foliage.gameObject.SetActive(false);
		}
	}

	/**
	 * 
	 */
	/// <summary>
	///My Change	
	public GameObject Tutorial1;
	public GameObject Tutorial2;
	public GameObject BonusScreen;
	int TimeNow;
	///My Change
//	public void BonusAndGooglePlayCreate()
//	{
//		if (TimeNow > PlayerPrefs.GetInt ("SavedBonusTime2") && PlayerPrefs.GetInt("SessionNumber") == 2)  {
//
//						//print (TimeNow + "   " + PlayerPrefs.GetInt ("SavedBonusTime2") + "TIMETIMETIMETIMETIMETIMETIMETIMETIMETIME");
//						BonusScreen.SetActive (true);
//			
//				} else {
//
//					PlayerPrefs.SetInt("SessionNumber" , 2);
//
//			#if (UNITY_ANDROID)
//
//					AppManager.instance.ConnectToGooglePlay();
//					
//			#endif
//
//			#if UNITY_IPHONE
//
//					myGameCenterScript.ConnectToGameCenter();
//			#endif
//
//				}
//
//	}

	void Awake()
	{
		int test = PlayerPrefs.GetInt ("gameLevel");

		if (test < 4)
		{
			FacebookButton.enabled = false;
			exclamation.enabled = false;
		}
		else
		{
			FacebookButton.enabled = true;
			exclamation.enabled = true;

		}
		//MyChange
		TimeNow = System.DateTime.Now.DayOfYear;
	//	AppManager.instance.ConnectToGooglePlay();
	//	BonusAndGooglePlayCreate ();

		mInstance = this;
		mPlatforms.Add(FirstPlatform);
		mTransform = gameObject.transform;
		mCamera = Camera.main;
		mUIRoot = GameObject.Find("UI Root").GetComponent<UIRoot>();
		mUICamera = GameObject.Find("UI Root/Camera").GetComponent<Camera>();
		mCameraTransform = mCamera.transform;
		//GameEndScreen.SetActive(false);
		AppManager.instance.challengeActivated += OnChallengeActivated;

		// Get player
		GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
		mPlayerController = playerGO.GetComponent<PlayerController>();
	
		// Initialization
		MaxPlatformOffsetY = InitialMaxPlatformOffsetY;
		MinPlatformOffsetY = InitialMinPlatformOffsetY;

		// Get coin width
		Sprite sprite = Multires.GetSprite("coin0");
		mCoinWidth = sprite.bounds.size.x;;
		mCoinHWidth = mCoinWidth * 0.5f;

		//
		// Records initial state
		//-----------------------

		// Player
		mIniPlayerPosition = playerGO.transform.localPosition;

		// Platform
		GameObject platform = GameObject.FindGameObjectWithTag("platform");
		mIniPlatformPosition = platform.transform.localPosition;
		mIniPlatformSize = platform.GetComponent<BoxCollider2D>().size;

		// Level controller
		mIniGeneratorPosition = mTransform.localPosition;

		//------------------------

		// Get player size
		Vector2 playerSize = playerGO.GetComponent<BoxCollider2D>().size;
		CharacterHalfWidth = playerSize.x * 0.5f;
		CharacterHalfHeight = playerSize.y  + 0.3f;

		DontDestroyOnLoad(this);

	
	}

	public static void CloseStore()
	{
		uiRoot.transform.FindChild ("Store").gameObject.SetActive (false);

	}
	/**
	 * Use this for initialization
	 */
	void Start()
	{

		Speed = mPlayerController.CurrentSpeed;
		CreatePlatform();

	}

	public void CloseBonus()
	{
		BonusScreen.SetActive(false);

		}
	/**
	 * Update is called once per frame
	 */
	

	void FixedUpdate()
	{
		Speed = mPlayerController.CurrentSpeed;

		if (mTransform.localPosition.x <
		    mCameraTransform.localPosition.x + mCamera.orthographicSize*mCamera.aspect)
		{
			CreatePlatform();
		}

		if (MaxPlatformOffsetY < AbsoluteMaxPlatformOffsetY)
		{
			float delta = Time.fixedDeltaTime * MaxPlatformOffsetYIncrementPerSec;
			MaxPlatformOffsetY += delta;
			MinPlatformOffsetY -= delta;
		}

	}

	/**
	 * 
	 */
	void CreatePlatform()
	{
		// Pick platform y coordinate randomly
		sPosition = mTransform.localPosition;
		sPosition.x += Random.Range(MinPlatformDistance, MaxPlatformDistance);
		sPosition.y += Random.Range(MinPlatformOffsetY, MaxPlatformOffsetY);
		
		// Enforce min & max y limit for platform
		if (sPosition.y < MinPlatformY) sPosition.y = MinPlatformY;
		else if (sPosition.y > MaxPlatformY) sPosition.y = MaxPlatformY;

		// Pick random platform size
		Vector2 size = new Vector2(Random.Range(MinPlatformLength, MaxPlatformLength),
		                           sPosition.y - -mCamera.orthographicSize);

		CreatePlatformAt(sPosition, size);
	}

	/**
	 * 
	 */
	void CreatePlatformAt(Vector3 position, Vector2 size)
	{
		GameObject platformGO = GetPlatform();
		JumpInfo jumpInfo;
		int i;
		Platform platform = platformGO.GetComponent<Platform>();
		platform._transform.localPosition = position;
		platformGO.SetActive(true);
		platform.SetSize(size);

		// Create coins
		if (Time.timeSinceLevelLoad > mNextCoinTime)
		{
			int maxCoin = (int)(size.x / (mCoinWidth + DistanceBetweenCoins));

			if (maxCoin >= 3)
			{
				GameObject coin;
				Vector3 coinPos = new Vector3();
				int numCoin = Random.Range(3, maxCoin);

				float totalWidth = numCoin * (mCoinWidth + DistanceBetweenCoins) - DistanceBetweenCoins;
				float offsetX = (size.x - totalWidth) * 0.5f;

				// Place coins at center of platform
				coinPos.Set(
					position.x + offsetX + mCoinHWidth,
					position.y + CoinHeightFromGround + mCoinHWidth, 0
				);

				for(i=0; i<numCoin; ++i)
				{
					// Create/recycle coin
					coin = GetCoin();

					// Set coin position
					coin.transform.localPosition = coinPos;

					coinPos.x += mCoinWidth + DistanceBetweenCoins;
				}

				mNextCoinTime = Time.timeSinceLevelLoad + 3 + Random.value * 3;
			}
		}
		// Spawn obstacle
		else if (Time.timeSinceLevelLoad > mNextObstacleTime && position.x >= ObstacleStartX)
		{
			int type = (int)(Random.value * ObstaclePrefabs.Count);
			GameObject prefab = ObstaclePrefabs[type];

			// If obstacle fits in platform
			float obstacleHWidth = prefab.renderer.bounds.extents.x;
			if (ObstacleMinOffsetX + obstacleHWidth + obstacleHWidth < size.x)
			{
				Vector3 obstaclePos = new Vector3(
					position.x + obstacleHWidth + Random.Range(ObstacleMinOffsetX, size.x - obstacleHWidth - obstacleHWidth),
					position.y - 0.05f, 0
				);

				GameObject obstacle = GetObstacle(type);
				obstacle.transform.localPosition = obstaclePos;

				// Compute jump over obstacle
				jumpInfo = GetJumpInfoFromPool();
				jumpInfo.type = JumpType.Obstacle;
				ComputeJumpOverObstacle(ref jumpInfo, platform._transform, obstacle.GetComponent<BoxCollider2D>(),
				                        obstacle.transform);
				platform.SetObstacleJumpInfo(jumpInfo);

				#if UNITY_EDITOR
				Debug.DrawLine(jumpInfo.jumpStart, jumpInfo.jumpPeak, Color.white, 10);
				Debug.DrawLine(jumpInfo.jumpPeak, jumpInfo.jumpEnd, Color.white, 10);
				#endif

				mNextObstacleTime = Time.timeSinceLevelLoad + 6 * Random.value * 3;
			}
		}
		else if (Time.time > mNextBubbleSpawnTime)
		{
			if (!mPlayerController.isShieldOn && Random.value <= mBubbleSpawnChance)
			{
				if (mBubblePowerup == null)
				{
					mBubblePowerup = Instantiate(BubblePowerupPrefab) as GameObject;
				}
				else
				{
					mBubblePowerup.SetActive(true);
				}

				mBubblePowerup.transform.localPosition = position + new Vector3(size.x * 0.5f, 0.8f, 0);
			}

			mNextBubbleSpawnTime = Time.time + BubbleSpawnInterval;
		}

		float foliageCount =  Random.value * 7 - 2;
		float count;
		FoliageController foliage;

//		// Create grass
//		count = Random.value * foliageCount;
//		if (count > 0)
//		{
//			for(i=0; i<count; ++i)
//			{
//				foliage = GetFreeGrass();
//				foliage._transform.localPosition = new Vector3(
//					position.x + PlatformEdgeJaggedness + foliage.width + Random.value * (size.x - foliage.width - 2*PlatformEdgeJaggedness),
//					position.y - 0.05f, 0
//					);
//				foliage._transform.localScale = new Vector3(Random.value < 0.5f ? -1 : 1, 1, 1);
//			}
//			
//			foliageCount -= count;
//		}
//
//		// Create weeds
//		count = Random.value * foliageCount;
//		if (count > 0)
//		{
//			for(i=0; i<count; ++i)
//			{
//				foliage = GetFreeWeed();
//				foliage._transform.localPosition = new Vector3(
//					position.x + PlatformEdgeJaggedness + foliage.width + Random.value * (size.x - foliage.width - 2*PlatformEdgeJaggedness),
//					position.y - 0.05f, 0
//				);
//				foliage._transform.localScale = new Vector3(Random.value < 0.5f ? -1 : 1, 1, 1);
//			}
//
//			foliageCount -= count;
//		}
//
//		// Create trees
//		count = Random.value * foliageCount;
//		if (count > 0)
//		{
//			float viewTop = mCameraTransform.localPosition.y + mCamera.orthographicSize;
//			float viewRight = mCameraTransform.localPosition.x - mCamera.orthographicSize * mCamera.aspect;
//			int numHangingRoots = 0;
//
//			foliageCount -= count;
//
//			for(i=0; i<count; ++i)
//			{
//				foliage = GetFreeTree();
//				foliage._transform.localPosition = new Vector3(
//					position.x + PlatformEdgeJaggedness + foliage.width + Random.value * (size.x - foliage.width - 2*PlatformEdgeJaggedness),
//					position.y - 0.05f, 0
//					);
//				foliage._transform.localScale = new Vector3(
//					Random.value < 0.5f ? -1 : 1,
//					Mathf.Max(0.8f, (viewTop - foliage._transform.localPosition.y) / foliage.height + Random.value * 0.2f),
//					1
//				);
//
//				if (foliageCount > 0 && numHangingRoots < 2 && Random.value < 0.3f)
//				{
//					TryCreateHangingRoot(mTrees.Count - 1, viewRight, viewTop);
//				}
//			}
//		}

		// Move the level generator marker to end of last created platform
		position.x += size.x;
		mTransform.localPosition = position;

		// Create jumping information
		if (mPlatforms.Count - 2 >= mJumpInfos.Count)
		{
			jumpInfo = new JumpInfo();
			mJumpInfos.Add (jumpInfo);
		}
		else
		{
			jumpInfo = mJumpInfos[mPlatforms.Count - 2];
		}

		jumpInfo.type = JumpType.Platform;
		ComputeJump(ref jumpInfo, mPlatforms[mPlatforms.Count - 2], platformGO);
		
		#if UNITY_EDITOR
		// Start to peak
		Debug.DrawLine(jumpInfo.jumpStart, jumpInfo.jumpStart + new Vector2(0, 1f), Color.green, 10);
		Debug.DrawLine(jumpInfo.jumpStart, jumpInfo.jumpPeak, Color.white, 10);
		// Peak to end
		Debug.DrawLine(jumpInfo.jumpEnd, jumpInfo.jumpEnd + new Vector2(0, 1f), Color.red, 10);
		Debug.DrawLine(jumpInfo.jumpPeak, jumpInfo.jumpEnd, Color.white, 10);
		#endif
	}

	//
	//
	//
	int TryCreateHangingRoot(int startTreeIndex, float viewRight, float viewTop)
	{
		int i;
		float distance;
		Vector3 position;
		FoliageController startTree = mTrees[startTreeIndex];
		Vector3 startPosition = startTree._transform.localPosition;
		Vector3 rootPosition = new Vector3();
		Vector3 rootScale = new Vector3(0, 0, 1);

		for(i=startTreeIndex - 1; i>=0; --i)
		{
			position = mTrees[i]._transform.localPosition;
			distance = startPosition.x - position.x;

			if (position.x < startPosition.x && position.x > viewRight)
			{
				FoliageController root = GetFreeHangingRoot();
				rootScale.x = rootScale.y = distance / root.width;

				if (rootScale.x >= 0.7f && rootScale.x <= 1.5f)
				{
					if (Random.value < 0.5f) rootScale.x = -rootScale.x;
					rootPosition.Set(position.x + distance * 0.5f, viewTop + 1 - Random.value * 3, 0);

					root._transform.localScale = rootScale;
					root._transform.localPosition = rootPosition;

					return 1;
				}
				else
				{
					// Cancel root creation
					root.gameObject.SetActive(false);
					mHangingRoots.Insert(0, root);
				}
			}
			else
			{
				break;
			}
		}

		return 0;
	}

	/**
	 * 
	 */
	GameObject GetPlatform()
	{
		GameObject platformGO = mPlatforms[0];
		Platform platform = platformGO.GetComponent<Platform>();
		float screenLeft = mCameraTransform.localPosition.x - mCamera.orthographicSize * mCamera.aspect;

		if (platform.transform.localPosition.x + platform.Size.x < screenLeft - PlatformDeleteDistance ||
		    !platformGO.activeSelf)
		{
			mPlatforms.Add(platformGO);
			mPlatforms.RemoveAt(0);

			if (platform.obstacleJumpInfo != null)
			{
				ReleaseJumpInfo(platform.obstacleJumpInfo);
				platform.SetObstacleJumpInfo(null);
			}

			if (mJumpInfos.Count > 0)
			{
				mJumpInfos.Add(mJumpInfos[0]);
				mJumpInfos.RemoveAt(0);
			}

			return platformGO;
		}
		else
		{
			platformGO = GameObject.Instantiate(PlatformPrefab) as GameObject;
			mPlatforms.Add(platformGO);
			return platformGO;
		}
	}

	/**
	 * 
	 */
	GameObject GetCoin()
	{
		GameObject coin = null;
		float screenLeft = float.MinValue;

		if (mCoins.Count > 0)
		{
			coin = mCoins[0];
			screenLeft = mCameraTransform.localPosition.x - mCamera.orthographicSize * mCamera.aspect;
		}

		if (coin != null && (!coin.activeSelf || coin.transform.localPosition.x + mCoinHWidth < screenLeft))
		{
			coin.transform.parent = mTransform.parent;
			coin.SetActive(true);
			coin.collider2D.enabled = true;
			//coin.GetComponent<Animator>().Play("Coin");
			mCoins.Add(coin);
			mCoins.RemoveAt(0);
			return coin;
		}
		else
		{
			coin = GameObject.Instantiate(CoinPrefab) as GameObject;
			mCoins.Add(coin);
			return coin;
		}
	}

	/**
	 * 
	 */
	GameObject GetObstacle(int type)
	{
		GameObject obstacle = null, instance = null;
		float screenLeft = mCameraTransform.localPosition.x - mCamera.orthographicSize * mCamera.aspect;
		GameObject prefab = ObstaclePrefabs[type];
		int i;

		// Try to find existing obstacle to reuse
		for(i=0; i<mObstacles.Count; ++i)
		{
			obstacle = mObstacles[i];

			if (obstacle.name == prefab.name)
			{
				if (!obstacle.activeSelf || obstacle.transform.localPosition.x + obstacle.renderer.bounds.size.x < screenLeft)
				{
					instance = obstacle;
					break;
				}
			}
		}

		if (instance == null)
		{
			// Didn't find a reusable obstacle. Create new instance.

			instance = GameObject.Instantiate(prefab) as GameObject;
			instance.name = prefab.name;
			mObstacles.Add(instance);
		}
		else
		{
			mObstacles.RemoveAt(i);
			mObstacles.Add(instance);
			instance.SetActive(true);
			instance.collider2D.isTrigger = false;
		}

		return instance;


		/*if (mObstacles.Count > 0)
		{
			obstacle = mObstacles[0];
			screenLeft = mCameraTransform.localPosition.x - mCamera.orthographicSize * mCamera.aspect;
		}

		if (obstacle != null &&
		    (!obstacle.activeSelf || obstacle.transform.localPosition.x + obstacle.renderer.bounds.size.x < screenLeft))
		{
			// Recycle obstacle
			mObstacles.RemoveAt(0);
			mObstacles.Add(obstacle);
			obstacle.SetActive(true);
			obstacle.collider2D.isTrigger = false;
			return obstacle;
		}
		else
		{
			obstacle = GameObject.Instantiate(ObstaclePrefab) as GameObject;
			mObstacles.Add(obstacle);
			return obstacle;
		}*/
	}

	/**
	 * 
	 */
	JumpInfo GetJumpInfoFromPool()
	{
		if (mJumpInfoPool.Count > 0)
		{
			JumpInfo info = mJumpInfoPool[0];
			mJumpInfoPool.RemoveAt(0);
			return info;
		}
		else
		{
			return new JumpInfo();
		}
	}

	//
	//
	//
	FoliageController GetFreeWeed()
	{
		FoliageController weed;

		if (mWeeds.Count > 0)
		{
			// Reuse weed that has left view
			weed = mWeeds[0];
			float viewLeft = mCameraTransform.localPosition.x - mCamera.orthographicSize * mCamera.aspect;

			if (!weed.gameObject.activeSelf || weed._transform.localPosition.x + weed.hwidth < viewLeft)
			{
				// Move weed to the end of the list
				mWeeds.RemoveAt(0);
				mWeeds.Add(weed);
				weed.gameObject.SetActive(true);
				return weed;
			}
		}

		// Instantiate new weed
		GameObject go = Instantiate( WeedPrefabs[ (int)(Random.value * WeedPrefabs.Count) ] ) as GameObject;
		weed = go.GetComponent<FoliageController>();
		mWeeds.Add(weed);
		return weed;
	}

	//
	//
	//
	FoliageController GetFreeGrass()
	{
		FoliageController grass;
		
		if (mGrasses.Count > 0)
		{
			// Reuse weed that has left view
			grass = mGrasses[0];
			float viewLeft = mCameraTransform.localPosition.x - mCamera.orthographicSize * mCamera.aspect;
			
			if (!grass.gameObject.activeSelf || grass._transform.localPosition.x + grass.hwidth < viewLeft)
			{
				// Move weed to the end of the list
				mGrasses.RemoveAt(0);
				mGrasses.Add(grass);
				grass.gameObject.SetActive(true);
				return grass;
			}
		}
		
		// Instantiate new weed
		GameObject go = Instantiate( GrassPrefabs[ (int)(Random.value * GrassPrefabs.Count) ] ) as GameObject;
		grass = go.GetComponent<FoliageController>();
		mGrasses.Add(grass);
		return grass;
	}

	//
	//
	//
	FoliageController GetFreeTree()
	{
		FoliageController tree;
		
		if (mTrees.Count > 0)
		{
			// Reuse weed that has left view
			tree = mTrees[0];
			float viewLeft = mCameraTransform.localPosition.x - mCamera.orthographicSize * mCamera.aspect;
			
			if (!tree.gameObject.activeSelf || tree._transform.localPosition.x + tree.hwidth < viewLeft)
			{
				// Move weed to the end of the list
				mTrees.RemoveAt(0);
				mTrees.Add(tree);
				tree.gameObject.SetActive(true);
				return tree;
			}
		}
		
		// Instantiate new weed
		GameObject go = Instantiate( TreePrefabs[ (int)(Random.value * TreePrefabs.Count) ] ) as GameObject;
		tree = go.GetComponent<FoliageController>();
		mTrees.Add(tree);
		return tree;
	}

	//
	//
	//
	FoliageController GetFreeHangingRoot()
	{
		FoliageController root;
		
		if (mHangingRoots.Count > 0)
		{
			// Reuse weed that has left view
			root = mHangingRoots[0];
			float viewLeft = mCameraTransform.localPosition.x - mCamera.orthographicSize * mCamera.aspect;
			
			if (!root.gameObject.activeSelf || root._transform.localPosition.x + root.hwidth * root._transform.localScale.x < viewLeft)
			{
				// Move weed to the end of the list
				mHangingRoots.RemoveAt(0);
				mHangingRoots.Add(root);
				root.gameObject.SetActive(true);
				return root;
			}
		}
		
		// Instantiate new weed
		GameObject go = Instantiate( HangingRootPrefabs[ (int)(Random.value * HangingRootPrefabs.Count) ] ) as GameObject;
		root = go.GetComponent<FoliageController>();
		mHangingRoots.Add(root);
		return root;
	}

	/**
	 * 
	 */
	void ReleaseJumpInfo(JumpInfo jump)
	{
		#if UNITY_EDITOR
			if (jump == null) Debug.LogError("null");
		#endif

		mJumpInfoPool.Add(jump);
	}

	/**
	 * 
	 */
	void ComputeJump(ref JumpInfo result, GameObject platform1, GameObject platform2)
	{
		Vector3 platStart1 = platform1.transform.localPosition;
		Vector3 platStart2 = platform2.transform.localPosition;
		
		Vector2 platEnd1 = new Vector2(platStart1.x + platform1.GetComponent<BoxCollider2D>().size.x,
		                               platStart1.y);
		
		if (platStart1.y <= platStart2.y)
		{
			ComputeJumpToHigherPlatform(ref result, platStart1, platEnd1, platStart2);
		}
		else
		{
			ComputeJumpToLowerPlatform(ref result, platStart1, platEnd1, platStart2);
		}
	}
	
	/**
	 * 
	 */
	void ComputeJumpToHigherPlatform(ref JumpInfo result, Vector2 platStart1, Vector2 platEnd1, Vector2 platStart2)
	{
		// Ray intersection
		Utils.RayRayIntersection rayRayResult;
		Vector2 jumpRiseRay = new Vector2(Speed, GhostController.JumpSpeed);
		Utils.RayRayIntersect(out rayRayResult, platEnd1, jumpRiseRay, platStart2, PlatformSurfaceVector);
		
		// Jump start x coordinate
		float jumpStartX;
		if (rayRayResult.intersect)
		{
			jumpStartX = platEnd1.x - rayRayResult.t2 - 0.01f;
		}
		else
		{
			jumpStartX = platEnd1.x;
		}
		
		// Find jump's peak point
		Vector2 jumpStart = new Vector2(jumpStartX, platEnd1.y);
		Vector2 jumpDescentRay = new Vector2(-Speed, -GhostController.Gravity);
		Utils.RayRayIntersect(out rayRayResult, jumpStart, jumpRiseRay, platStart2, jumpDescentRay);
		
		#if UNITY_EDITOR
		if (!rayRayResult.intersect)
		{
			Debug.LogError("no intersection");
			Debug.Break();
		}
		#endif

		Vector2 offset = new Vector2(-CharacterHalfWidth, CharacterHalfHeight);
		result.jumpStart = jumpStart + offset;
		result.jumpEnd = platStart2 + offset;
		result.jumpPeak = jumpStart + (jumpRiseRay * rayRayResult.t1) + offset;
	}
	
	/**
	 * 
	 */
	void ComputeJumpToLowerPlatform(ref JumpInfo result, Vector2 platStart1, Vector2 platEnd1, Vector2 platStart2)
	{
		// Check intersection with starting platform
		Utils.RayRayIntersection rayRayResult;
		Vector2 descendRay = new Vector2(-Speed, -GhostController.Gravity);
		Utils.RayRayIntersect(out rayRayResult, platStart2, descendRay, platEnd1, InvPlatformSurfaceVector);
		
		Vector2 jumpEnd = new Vector2(0, platStart2.y);
		if (rayRayResult.intersect)
		{
			// Push the landing point further
			jumpEnd.x = platStart2.x + rayRayResult.t2 + 0.01f;
		}
		else
		{
			jumpEnd.x = platStart2.x;
		}
		
		// Find jump's peak point
		Vector2 riseRay = new Vector2(Speed, GhostController.JumpSpeed);
		Utils.RayRayIntersect(out rayRayResult, platEnd1, riseRay, jumpEnd, descendRay);
		
		#if UNITY_EDITOR
		if (!rayRayResult.intersect)
		{
			Debug.LogError("no intersection");
			Debug.Break();
		}
		#endif

		Vector2 offset = new Vector2(CharacterHalfWidth, CharacterHalfHeight);
		result.jumpStart = platEnd1 + offset;
		result.jumpEnd = jumpEnd + offset;
		result.jumpPeak = platEnd1 + (riseRay * rayRayResult.t1) + offset;
	}

	/**
	 * 
	 */
	void ComputeJumpOverObstacle(ref JumpInfo result, Transform platformTransform, BoxCollider2D obstacleCollider, Transform obstacleTransform)
	{
		// Obstacle's left top corner
		Vector2 obstacleSize = obstacleCollider.size;
		Vector2 rayStart1 = obstacleTransform.localPosition;
		rayStart1.x -= obstacleSize.x * 0.5f;
		rayStart1.y += obstacleSize.y * 0.5f;
		Vector2 riseRayDir = new Vector2(-Speed, -GhostController.JumpSpeed);

		// A point far left from platform
		Vector2 rayStart2 = platformTransform.localPosition;
		rayStart2.x -= obstacleSize.x * 10;

		// Find intersection point
		Utils.RayRayIntersection rayIntResult;
		Utils.RayRayIntersect(out rayIntResult, rayStart1, riseRayDir, rayStart2, PlatformSurfaceVector);

		#if UNITY_EDITOR
			if (!rayIntResult.intersect) Debug.LogError("No intersection");
		#endif

		// Jump's starting point
		result.jumpStart.Set(rayStart2.x + rayIntResult.t2 * PlatformSurfaceVector.x - CharacterHalfWidth,
		                     rayStart2.y + rayIntResult.t2 * PlatformSurfaceVector.y + CharacterHalfHeight);

		// Obstacle's right top corner
		rayStart1.x += obstacleSize.x;

		// Landing point
		Vector2 descendRayDir = new Vector2(Speed, Physics2D.gravity.y);
		descendRayDir.Normalize ();
		float t = (rayStart2.y - rayStart1.y) / descendRayDir.y;
		result.jumpEnd.Set(rayStart1.x + descendRayDir.x * t + CharacterHalfWidth, rayStart2.y + CharacterHalfHeight);

		// Find jump's peak point
		riseRayDir = -riseRayDir;
		descendRayDir = -descendRayDir;
		Utils.RayRayIntersect(out rayIntResult, result.jumpStart, riseRayDir, result.jumpEnd, descendRayDir);

		#if UNITY_EDITOR
			if (!rayIntResult.intersect) Debug.LogError("No intersection");
		#endif

		result.jumpPeak.Set(result.jumpStart.x + riseRayDir.x * rayIntResult.t1,
		                    result.jumpStart.y + riseRayDir.y * rayIntResult.t1);
	}

	//
	//
	//
	void OnCountDownFinished()
	{
		Time.timeScale = 1;
		uiRoot.transform.FindChild("HUD").GetComponent<HUDController>().SetEnable(true);
	}
}
