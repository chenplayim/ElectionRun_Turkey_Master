using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
//
//
public class Config : MonoBehaviour
{
	/**
	 * 
	 */
//	public enum Characters : int
//	{
		//"ahmet","cem","devlet","dogu","emine","floating","kemal","mustafa","selahattin"
//
//		ahmet = 0,
//		cem = 1,
//		devlet = 2,
//		dogu = 3,
//		emine = 4,
//		floating = 5,
//		kemal = 6,
//		mustafa = 7,
//		selahattin = 8
//	}

	/**
	 * 
	 */
	public float GravityYPerFrame;

	/**
	 * 
	 */
	//public string BoyAnimator;
	//public string GirlAnimator;
	public GameObject UserNameTextPrefab;


	//Election Change

	public string devletAnimator = "devlet";
	public string ahmetAnimator = "ahmet";
	public string cemAnimator = "cem";
	public string dogunAnimator = "dogun";
	public string emineAnimator = "emine";
	public string floatingAnimator = "floating";
	public string kemalAnimator = "kemal";
	public string mustafaAnimator = "mustafa";
	public string selahattinAnimator = "selahattin";


	/**
	 * 
	 */
	public float[] SpeedUpgrades;
	public float[] JumpHeightUpgrades;
	public float[] DoubleJumpHeightUpgrades;
	public float[] BubbleUpgrades;
	
	/**
	 * 
	 */
	public int[] SpeedUpgradeCosts;
	public int[] JumpHeightUpgradeCosts;
	public int[] DoubleJumpHeightUpgradeCosts;
	public int[] BubbleUpgradeCosts;

	/**
	 * 
	 */
	public string PlayerName;
	public int CurrentSpeedLevel = 0;
	public int CurrentJumpHeightLevel = 0;
	public int CurrentDoubleJumpHeightLevel = 0;
	public int CurrentBubbleLevel = 0;
	public string CurrentHat = "";
	public float BubbleShieldDuration = 10;
	public int Coins = 0;
	public string CurrentCharacter ;
	public int GhostPopulation;
	public int LifetimeCoins = 0;

	/**
	 * 
	 */
	private static Config mInstance;
	Dictionary<string, bool> mOwnedHats = new Dictionary<string, bool>();
	Dictionary<string, bool> mOwnedPremiumHats = new Dictionary<string, bool>();

	/**
	 * 
	 */
	public static Config instance
	{
		get { return mInstance; }
	}

	//
	// Returns true if hat is owned by player. Always return false for premium hats.
	//
	public bool OwnsHat(string hatName)
	{
		if (mOwnedHats.ContainsKey(hatName)) return true;
		else if (mOwnedPremiumHats.ContainsKey(hatName)) return true;
		else return false;
	}

	//
	//
	//
	public void AddOwnedHat(string hatName)
	{
		mOwnedHats.Add(hatName, true);
	}

	//
	//
	//
	public void AddOwnedPremiumHat(string hatName)
	{
		mOwnedPremiumHats.Add(hatName, true);
	}

	/**
	 * 
	 */
	public void Save()
	{
		PlayerPrefs.SetInt("Saved", 1);
		PlayerPrefs.SetString("PlayerName", PlayerName);
		PlayerPrefs.SetInt("Coins", Coins);
		PlayerPrefs.SetInt("CurrentBubbleLevel", CurrentBubbleLevel);
		PlayerPrefs.SetInt("CurrentJumpHeightLevel", CurrentJumpHeightLevel);
		PlayerPrefs.SetInt("CurrentDoubleJumpHeightLevel", CurrentDoubleJumpHeightLevel);
		PlayerPrefs.SetString("CurrentCharacter", CharacterSelectionController.CurrentCharacter);
		PlayerPrefs.SetString("CurrentHat", CurrentHat);
		PlayerPrefs.SetInt("LifetimeCoins", LifetimeCoins);

		// Write list of owned hats
		string hatList = "";
		foreach(string key in mOwnedHats.Keys)
		{
			if (hatList.Length > 0) hatList += ",";
			hatList += key;
		}
		PlayerPrefs.SetString("OwnedHats", hatList);
	}

	/**
	 * 
	 */
	public void Load()
	{
		PlayerName = PlayerPrefs.GetString("PlayerName", "");
		Coins = PlayerPrefs.GetInt("Coins", 0);
		CurrentBubbleLevel = PlayerPrefs.GetInt("CurrentBubbleLevel", 0);
		//may change
		//CurrentBubbleLevel = PlayerPrefs.GetInt("CurrentSpeedLevel", 0);
		CurrentJumpHeightLevel = PlayerPrefs.GetInt("CurrentJumpHeightLevel", 0);
		CurrentDoubleJumpHeightLevel = PlayerPrefs.GetInt("CurrentDoubleJumpHeightLevel", 0);
		CurrentCharacter = PlayerPrefs.GetString("CurrentCharacter", CharacterSelectionController.CurrentCharacter);
		CurrentHat = PlayerPrefs.GetString("CurrentHat", "");
		LifetimeCoins = PlayerPrefs.GetInt("LifetimeCoins");

		// Get list of owned hats
		mOwnedHats.Clear();
		string hatList = PlayerPrefs.GetString("OwnedHats", string.Empty);
		if (hatList.Length > 0)
		{
			string[] hats = hatList.Split(","[0]);
			int i;
			for(i=hats.Length - 1; i>=0; --i)
			{
				mOwnedHats.Add(hats[i], true);
			}
		}
	}

	//
	//
	//
	public void ResetSavedGame()
	{
		PlayerPrefs.SetInt("Saved", 0);
		PlayerPrefs.SetString("PlayerName", "");
		PlayerPrefs.SetInt("Coins", 0);
		PlayerPrefs.SetInt("CurrentBubbleLevel", 0);
		PlayerPrefs.SetInt("CurrentJumpHeightLevel", 0);
		PlayerPrefs.SetInt("CurrentDoubleJumpHeightLevel", 0);
		PlayerPrefs.SetString("CurrentCharacter", CharacterSelectionController.CurrentCharacter);
		PlayerPrefs.SetString("CurrentHat", "");
		PlayerPrefs.SetString("OwnedHats", "");
		PlayerPrefs.SetInt("LifetimeCoins", 0);
		PlayerPrefs.SetInt("Plays", 0);
		PlayerPrefs.SetInt("SuggestUpgrade", 0);
		PlayerPrefs.SetInt("SuggestChallenge", 0);
	}

	//
	//
	//
	public bool HasBuyableUpgrades()
	{
		if (SpeedUpgradeCosts[CurrentSpeedLevel] <= Coins) return true;
		if (JumpHeightUpgradeCosts[CurrentJumpHeightLevel] <= Coins) return true;
		if (DoubleJumpHeightUpgradeCosts[CurrentDoubleJumpHeightLevel] <= Coins) return true;

		return false;
	}

	/**
	 * 
	 */
	void Awake()
	{
		//PlayerPrefs.DeleteAll ();

		mInstance = this;
		GravityYPerFrame = Physics2D.gravity.y * Time.fixedDeltaTime;
		Load ();
	}
}
