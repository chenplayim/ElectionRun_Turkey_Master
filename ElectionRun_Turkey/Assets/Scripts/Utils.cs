using UnityEngine;
using System.Collections;

public class Utils
{
	/**
	 * 
	 */
	public struct RayRayIntersection
	{
		public bool intersect;
		public float t1;
		public float t2;
	}

	/**
	 * 
	 */
	public static RayRayIntersection sHelperRayRayIntersection;

	/**
	 * 
	 */
	public static string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}

	/**
	 * 
	 */
	public static void RayRayIntersect(out RayRayIntersection result, Vector2 start1, Vector2 direction1, Vector2 start2, Vector2 direction2)
	{
		float div = direction1.y*direction2.x - direction1.x*direction2.y;
		result = sHelperRayRayIntersection;

		// No intersection
		if (div == 0)
		{
			result.intersect = false;
			return;
		}

		float t = (direction2.x*(start2.y - start1.y) + direction2.y*(start1.x - start2.x)) / div;

		if (t < 0)
		{
			result.intersect = false;
			return;
		}

		float s = (direction1.x*(start2.y - start1.y) + direction1.y*(start1.x - start2.x)) / div;

		if (s < 0)
		{
			result.intersect = false;
			return;
		}

		result.t1 = t;
		result.t2 = s;
		result.intersect = true;
	}

	//
	//
	//
	public static void SendMessageDownwards(GameObject go, string methodName, object value = null,
	                                        SendMessageOptions options = SendMessageOptions.RequireReceiver)
	{
		go.SendMessage(methodName, value, options);

		Transform transf = go.transform;
		int i;
		for(i = 0; i<transf.childCount; ++i)
		{
			SendMessageDownwards(transf.GetChild(i).gameObject, methodName, value, options);
		}
	}
}
