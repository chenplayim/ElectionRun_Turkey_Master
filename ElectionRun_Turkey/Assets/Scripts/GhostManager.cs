using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 
 */
public class GhostManager : MonoBehaviour
{
	/**
	 * 
	 */
	public GameObject GhostPrefab;

	/**
	 * 
	 */
	private static List<GameObject> mGhosts;
	private static List<GhostController> mActiveGhosts = new List<GhostController>();
	private static GhostManager mInstance;
	private static Dictionary<int, GameObject> mActiveGhostMap = new Dictionary<int, GameObject>();
	private static List<UILabel> mUserNameLabels = new List<UILabel>();
	private static bool bKillingGhosts = false;

	//GhostController controller;

	public static GameObject AddGhost(float x, float offsetXFromPlayer, float diesAtMeters, GameObject platform, string name = null)
	{

		if (mGhosts == null)
		{
			mGhosts = new List<GameObject>();
			//print("1");
		}
		
		GameObject go;
		if (mGhosts.Count == 0)
		{
			go = Instantiate(mInstance.GhostPrefab) as GameObject;
			//print("2");
		}
		else
		{
			//print("3");

			go = mGhosts[0];
			mGhosts.RemoveAt(0);
			go.SetActive(true);
			
			#if UNITY_EDITOR
			if (go.GetComponent<GhostController>().nameLabel != null)
			{
				Debug.LogError("Not dead");
			}
			#endif

			//print("GHOST    +   " + go.name);
		}
		
		GhostController controller = go.GetComponent<GhostController>();
		controller.manager = mInstance;
		controller.offsetXFromPlayer = offsetXFromPlayer ;
		controller.DiesAtMeters = diesAtMeters;

		// Initialize ghost
		if (name == null)
		{
			go.name = GhostController.Anonymous;
		}
		else
		{
			go.name = name;
			controller.setLabel(GetUserNameLabel());
		}
		
		// Reset ghost
		//Election Change
		//ChosenCharecterController();
		controller.Reset(platform);
		go.transform.localPosition = new Vector3(x, platform.transform.localPosition.y  + LevelGenerator.CharacterHalfHeight, 0);
		mActiveGhosts.Add(controller);

		// Hat
//		if (hat != null && hat.Length > 0)
//		{
//			PlayerController.WearHat(hat, go.transform.FindChild("Head"));
//		}

		return go;
	}

	/**
	 * 
	 */





	public static void ReleaseGhost(GhostController ghost)
	{

		print ("ReleaseGhostReleaseGhostReleaseGhostReleaseGhost");


		if (bKillingGhosts) return;
		Debug.Log("ReleaseGhost - Begin");

		GameObject go = ghost.gameObject;

		if (ghost.nameLabel != null)
		{
			ReleaseUserNameLabel(ghost.nameLabel);
			ghost.setLabel(null);
		}

		// Delete hat
//		GameObject head = ghost.transform.FindChild("Head").gameObject;
//		if (head.transform.childCount > 0)
//		{
//			GameObject.Destroy(head.transform.GetChild(0).gameObject);
//		}

		go.SetActive(false);
		mGhosts.Add(go);
		mActiveGhosts.Remove(ghost);
		mActiveGhostMap.Remove( (int)ghost.DiesAtMeters );

		Debug.Log("ReleaseGhost - End");
	}

	/**
	 * 
	 */
	public static int numActiveGhosts
	{
		get { return mActiveGhosts.Count; }
	}

	//
	//
	//
	public static GhostController GetActiveGhost(int index)
	{
		return mActiveGhosts[index];
	}

	/**
	 * 
	 */
	public static GameObject GetGhostAtMeter(int meter)
	{
		GameObject ghost;
		mActiveGhostMap.TryGetValue(meter, out ghost);
		return ghost;
	}

	/**
	 * 
	 */
	public static void KillAllGhosts()
	{
		mInstance.DoKillGhosts();
	}



	/**
	 * 
	 */
	public static UILabel GetUserNameLabel()
	{
		UILabel label;

		if (mUserNameLabels.Count == 0)
		{
			GameObject go = Instantiate(Config.instance.UserNameTextPrefab) as GameObject;
			label = go.GetComponent<UILabel>();
			label.text = "";
			return label;
		}
		else
		{
			label = mUserNameLabels[0];
			mUserNameLabels.RemoveAt(0);
			label.text = "";
			label.gameObject.SetActive(true);
			return label;
		}
	}

	/**
	 * 
	 */
	public static void ReleaseUserNameLabel(UILabel label)
	{
		label.gameObject.SetActive(false);
		mUserNameLabels.Add(label);
	}

	/**
	 * 
	 */





	void DoKillGhosts()
	{
		bKillingGhosts = true;

		int i = 0;
		int count = mActiveGhosts.Count;
		GhostController ghostController;
		GameObject head;
		
		for(i=0; i<count; ++i)
		{
			///MY Ghosts
			//print ("%%%%%%%%%%%%  " + mActiveGhosts[i]);

			ghostController = mActiveGhosts[i];
			ghostController.gameObject.SetActive(false);
			mGhosts.Add(ghostController.gameObject);
			
			if(ghostController.nameLabel != null)
			{
				ReleaseUserNameLabel(ghostController.nameLabel);
				ghostController.setLabel(null);
			}

			head = ghostController.transform.FindChild("Head").gameObject;
			if (head.transform.childCount > 0)
			{
				GameObject.Destroy(head.transform.GetChild(0).gameObject);
			}
		}
		
		mActiveGhosts.RemoveRange(0, mActiveGhosts.Count);
		mActiveGhostMap.Clear();
		bKillingGhosts = false;

	//	Debug.Log("DoKillGhosts - End");
	}

	/**
	 * 
	 */
	void Awake()
	{
		mInstance = this;
	}
}
