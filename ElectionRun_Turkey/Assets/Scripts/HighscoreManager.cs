using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighscoreManager : MonoBehaviour
{
	/**
	 * 
	 */
	public delegate void ScoreListUpdated();
	public delegate bool EnumerateScores(Highscore.ScoreEntry entry);

	/**
	 * 
	 */
	public ScoreListUpdated OnScoreListUpdated;

	/**
	 * 
	 */
	private static int MaxScoreEntries = 50;

	/**
	 * 
	 */
	private static HighscoreManager mInstance;
	private static List<Highscore.ScoreEntry> mScoreEntries;
	private static int mMaxDistance = int.MinValue;
	private static int mMinDistance = int.MinValue;

	/**
	 * 
	 */
	void Awake()
	{
		mInstance = this;
	}

	/**
	 * 
	 */
	public static IEnumerator RequestScores(int minDistance, int maxDistance)
	{
		// Make sure the scores do not overlap
		if ( (minDistance < mMinDistance && maxDistance < mMinDistance) || (minDistance > mMaxDistance && maxDistance > mMaxDistance) )
		{
			mInstance.StartCoroutine( Highscore.GetScores(minDistance, maxDistance, mInstance.GetScoreCallback) );
		}
		else
		{
			if (mInstance.OnScoreListUpdated != null) mInstance.OnScoreListUpdated();
		}

		yield return null;
	}

	/**
	 * 
	 */
	public static void EnumerateScoresBetween(int minDistance, int maxDistance, EnumerateScores callback)
	{
		if (minDistance > mMaxDistance || maxDistance < mMinDistance) return;

		foreach(Highscore.ScoreEntry entry in mScoreEntries)
		{
			if (entry.Score >= minDistance && entry.Score <= maxDistance)
			{
				if (!callback(entry)) return;
			}
		}
	}

	/**
	 * 
	 */
	public static bool GetScoreEqualOrMoreThan(ref Highscore.ScoreEntry result, float distance)
	{
		if (distance < mMinDistance || distance > mMaxDistance) return false;
		
		foreach(Highscore.ScoreEntry entry in mScoreEntries)
		{
			if (entry.Score >= distance)
			{
				result = entry;
				return true;
			}
		}

		return false;
	}

	/**
	 * 
	 */
	void GetScoreCallback(int minDistance, int maxDistance, Highscore.ScoreEntry[] scores, int count)
	{
		if (mScoreEntries == null)
		{
			mScoreEntries = new List<Highscore.ScoreEntry>();
		}

		int totalEntries = mScoreEntries.Count + count;
		int i;

		if (minDistance > mMaxDistance)
		{
			if (totalEntries > MaxScoreEntries)
			{
				mScoreEntries.RemoveRange(0, totalEntries - MaxScoreEntries);
				mMinDistance = mScoreEntries[0].Score;
			}

			for(i=0; i<count; ++i)
			{
				mScoreEntries.Add( new Highscore.ScoreEntry(scores[i]) );
			}

			mMaxDistance = maxDistance;
			if (mMinDistance == int.MinValue) mMinDistance = minDistance;

			if (OnScoreListUpdated != null) OnScoreListUpdated();
		}
		else if (maxDistance < mMinDistance)
		{
			if (totalEntries > MaxScoreEntries)
			{
				int delCount = totalEntries - MaxScoreEntries;
				mScoreEntries.RemoveRange(mScoreEntries.Count - delCount, delCount);
				mMaxDistance = mScoreEntries[mScoreEntries.Count - 1].Score;
			}

			for(i=0; i<count; ++i)
			{
				mScoreEntries.Insert( i, new Highscore.ScoreEntry(scores[i]) );
			}

			mMinDistance = mScoreEntries[0].Score;
			if (mMaxDistance == int.MinValue) mMaxDistance = maxDistance;

			if (OnScoreListUpdated != null) OnScoreListUpdated();
		}
		else
		{
			Debug.LogWarning("Scores overlaps");
		}
	}
}
