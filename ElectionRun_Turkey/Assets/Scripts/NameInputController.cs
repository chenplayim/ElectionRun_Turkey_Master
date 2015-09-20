using UnityEngine;
using System.Collections;

public class NameInputController : MonoBehaviour
{
	/**
	 * 
	 */
	public delegate void NameEntered(string name);

	/**
	 * 
	 */
	const int MaxNameLength = 15;

	/**
	 * 
	 */
	public NameEntered OnNameEntered;

	/**
	 * 
	 */
	UILabel mNameLbl;
	GameObject mMaxNameMsg;

	#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR

	TouchScreenKeyboard mKeyboard;

	#endif

	/**
	 * 
	 */
	public void OnClickOK()
	{
		if (mNameLbl.text.Length > MaxNameLength)
		{
			mMaxNameMsg.SetActive(true);
		}
		else if (mNameLbl.text.Length > 0)
		{
			mMaxNameMsg.SetActive(false);
			gameObject.SetActive(false);
			if(OnNameEntered != null) OnNameEntered(mNameLbl.text);
		}
	}

	public void OnClickOK2()
	{
		if (mNameLbl.text.Length > MaxNameLength)
		{
			mMaxNameMsg.SetActive(true);
		}
		else if (mNameLbl.text.Length > 0)
		{
			mMaxNameMsg.SetActive(false);
			//gameObject.SetActive(false);
			if(OnNameEntered != null) OnNameEntered(mNameLbl.text);
		}
	}


	/**
	 * 
	 */
	public void OnClick()
	{
		mMaxNameMsg.SetActive(false);
		Start();
	}

	/**
	 * 
	 */
	void Awake()
	{
	//	mNameLbl = transform.FindChild("Name").GetComponent<UILabel>();
	//	mNameLbl.text = Config.instance.PlayerName;

		mMaxNameMsg = GameObject.Find("MaxNameLength");
		mMaxNameMsg.SetActive(false);
	}

	/**
	 * 
	 */
	void Start()
	{
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR

		if (!TouchScreenKeyboard.visible)
		{
			mKeyboard = TouchScreenKeyboard.Open(mNameLbl.text, TouchScreenKeyboardType.Default, false, false, false, false,
			                                     "Type your name");
		}

		#endif
	}

	/**
	 * 
	 */
	void Update()
	{
		#if UNITY_EDITOR

			ProcessPhysicalKeyboardInput();

		#elif (UNITY_ANDROID || UNITY_IPHONE)

			if (mKeyboard != null)
			{
				if (mKeyboard.text.Length > MaxNameLength) mKeyboard.text = mKeyboard.text.Substring(0, MaxNameLength);
				mNameLbl.text = mKeyboard.text;
			}

		#else

			ProcessPhysicalKeyboardInput();

		#endif
	}

	/**
	 * 
	 */
	void ProcessPhysicalKeyboardInput()
	{
		foreach (char c in Input.inputString)
		{
		//	print(c);
			
			if (c != "\b"[0])
			{
				/*if (mNameLbl.text.Length != 0)
					{
						mNameLbl.text = mNameLbl.text.Substring(0, mNameLbl.text.Length - 1);
					}
				}
				else
				{*/
				if (c == "\n"[0] || c == "\r"[0])
				{
					OnClick();
				}
				else
				{
					if (mNameLbl.text.Length < MaxNameLength)
					{
						mNameLbl.text = mNameLbl.text + c;
					}
				}
			}
		}
		
		if(Input.GetKeyDown(KeyCode.Backspace))
		{
			if (mNameLbl.text.Length != 0) mNameLbl.text = mNameLbl.text.Substring(0, mNameLbl.text.Length - 1);
		}
		else if (Input.GetKeyDown(KeyCode.Return))
		{
			OnClickOK();
		}
	}
}
