using UnityEngine;
using System.Collections;

public class CharacterSelectionController : MonoBehaviour
{
	/**
	 * 
	 */
	public static string CurrentCharacter;
	public StoreController StoreGUI;

	// Callback
	public event System.Action<CharacterSelectionController> Closed;

	/**
	 * 
	 */
	int mCurCharIndex;
	GameObject[] mCharacterButtons;
	public GameObject mPointer;
	Config mConfig;

	public GameObject HatsGrid;
	public GameObject CoinsGrid;

	public GameObject messege;
	public UITweener messegeTween;


	void Start()
	{

		CurrentCharacter = null;

	}
	public void OnClickCharecter()
	{
		UIButton CurBTN = UIButton.current;
		string mCurrentCharecter = CurBTN.name;

		mPointer.transform.localPosition = CurBTN.transform.localPosition;
		mPointer.SetActive (true);
		CurrentCharacter = mCurrentCharecter;

	//	print (mCurrentCharecter);

		if (PlayerPrefs.GetInt ("FirstChosenCharecter") == 0) {

			Analytics.gua.sendEventHit("Action","FirstChosenCharecter" , mCurrentCharecter);
			PlayerPrefs.SetInt("FirstChosenCharecter" , 1);
		}

	}

	public MainMenuController MainMenuController;

	public void OnClose()
	{
		if (CurrentCharacter != null)
		{
		Config.instance.CurrentCharacter = CurrentCharacter;
		Config.instance.Save();
		if(Closed != null) Closed(this);
		gameObject.SetActive(false);
		MainMenuController.OnClickStart ();
		HideMessege();
		}
		else
		{

			messege.SetActive(true);
			messegeTween.enabled = true;

		}
	}
	

	public void HideMessege()
	{
		messege.SetActive(false);
		messegeTween.enabled = false;

	}

	public GameObject MainMenu;

	void Update() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			gameObject.SetActive(false);
			MainMenu.SetActive(true);
		}
		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			gameObject.SetActive(false);
			MainMenu.SetActive(true);
		}
	}



//	void Awake()
//	{
//
//		mConfig = GameObject.Find("GameData").GetComponent<Config>();
//
//	}

	/**
	 * 
	 */
//	void OnEnable()
//	{
//		CurrentCharacter = null;
//		mCurCharIndex = (int)Config.Characters.Parse(typeof(Config.Characters), CurrentCharacter);
//		//UpdateGUI();
//
//	//GetComponent<NameInputController>().OnNameEntered = OnNameConfirmed;
//
//		UIPlaySound sound = GetComponent<UIPlaySound>();
//		if(sound != null) sound.Play();
//	}

	/**
	 * 
	 */
//	void OnNameConfirmed(string name)
//	{
//		Config.instance.PlayerName = name;
//	}

	/**
	 * 
	 */
//	private void UpdateGUI()
//	{
//		Vector3 position = mPointer.transform.localPosition;
//		position.x = mCharacterButtons[mCurCharIndex].transform.localPosition.x;
//		mPointer.transform.localPosition = position;
//
//		if (mCurCharIndex == 1) {
//			mCharacterButtons[0].GetComponent<UISprite>().color = Color.gray;
//			mCharacterButtons[0].GetComponent<UIButton>().defaultColor = Color.gray;
//
//			mCharacterButtons[1].GetComponent<UISprite>().color = Color.white;
//			mCharacterButtons[1].GetComponent<UIButton>().defaultColor = Color.white;
//				}
//		else if (mCurCharIndex == 0) {
//			mCharacterButtons[1].GetComponent<UISprite>().color = Color.gray;
//			mCharacterButtons[1].GetComponent<UIButton>().defaultColor = Color.gray;
//			
//			mCharacterButtons[0].GetComponent<UISprite>().color = Color.white;
//			mCharacterButtons[0].GetComponent<UIButton>().defaultColor = Color.white;
//			}
//	}

	//
	//
	//

}
