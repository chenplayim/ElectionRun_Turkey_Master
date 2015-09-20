using UnityEngine;
using System.Collections;

/**
 * 
 */
public class Highscore
{
	/**
	 * 
	 */
	public struct ScoreEntry
	{
		public string Name;
		public int Score;

		public ScoreEntry (ScoreEntry entry)
		{
			Name = entry.Name;
			Score = entry.Score;
		}
	}

	/**
	 * 
	 */
	public delegate void GetScoreResult(int minDistance, int maxDistance, ScoreEntry[] scores, int count);
	public delegate void GetRandomScoresResult(ScoreEntry[] scores, int count);
	public delegate void GetBestScoreResult(ScoreEntry entry, Object userData);

	/**
	 * 
	 */
	private static int NumScoresRetrieved = 30;

	/**
	 * 
	 */
	private static string secretKey = "mySecretKey"; // Edit this value and make sure it's the same as the one stored on the server
	//public static string addScoreURL = "http://www.yepistats.com/yepiRunner/addscore.php?"; //be sure to add a ? to your url
	//public static string highscoreURL = "http://www.yepistats.com/yepiRunner/display.php";

	/*public static string addScoreURL = "http://www.labugames.com/yepi/addScore.php?";
	public static string highscoreURL = "http://www.labugames.com/yepi/getScores.php?";
	public static string randomScoreURL = "http://www.labugames.com/yepi/getRandomScores.php?";
	public static string bestScoreURL = "http://www.labugames.com/yepi/bestScore.php";*/

	public static string addScoreURL = "http://www.yepistats.com/electionRunner/addScore.php?";
	public static string highscoreURL = "";
	public static string randomScoreURL = "http://www.yepistats.com/electionRunner/getRandomScores.php?";
	public static string bestScoreURL = "http://www.yepistats.com/electionRunner/bestScore.php";

	/**
	 * 
	 */
	private static ScoreEntry[] mScores = null;
	
	// remember to use StartCoroutine when calling this function!
	public static IEnumerator PostScore(string name, int score)
	{
		//This connects to a server side php script that will add the name and score to a MySQL DB.
		// Supply it with a string representing the players name and the players score.
		string hash = Utils.Md5Sum(name + score + secretKey);
		
		string post_url = addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&hash=" + hash;
		//string post_url = addScoreURL + "name=&score=" + score + "&hash=" + hash;
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the download is done
		
		if (hs_post.error != null)
		{
			Debug.LogError("There was an error posting the high score: " + hs_post.error);
		}
		else
		{
		//	Debug.Log(hs_post.isDone);
		}
	}
	
	// Get the scores from the MySQL DB to display in a GUIText.
	// remember to use StartCoroutine when calling this function!
	public static IEnumerator GetScores(int minDistance, int maxDistance, GetScoreResult callback)
	{
		string get_url = highscoreURL + "minDistance=" + minDistance + "&maxDistance=" + maxDistance + "&numScores=" + NumScoresRetrieved;

		WWW hs_get = new WWW(get_url);
		yield return hs_get;
		
		if (hs_get.error != null)
		{
			Debug.LogError("There was an error getting the high score: " + hs_get.error);
		}
		else
		{
			if (mScores == null)
			{
				mScores = new ScoreEntry[NumScoresRetrieved];
			}

			string[] scores = hs_get.text.Split('\n');
			string s;
			int i;
			int separator;
			int count = 0;
			for(i=0; i<scores.Length; ++i)
			{
				s = scores[i];

				separator = s.IndexOf("\t");

				if (s.Length > 0 && separator > 0)
				{
					mScores[i].Name = s.Substring(0, separator);
					mScores[i].Score = int.Parse(s.Substring(separator));
					++count;
				}
			}

			if (callback != null) callback(minDistance, maxDistance, mScores, count);
		}
	}

	public static IEnumerator GetRandomScores(int numScores, GetRandomScoresResult callback)
	{
		string get_url = randomScoreURL + "&numScores=" + numScores;


		WWW hs_get = new WWW(get_url);
		yield return hs_get;
		
		if (hs_get.error != null)
		{
			Debug.LogError("There was an error getting random scores: " + hs_get.error);
			callback(null, -1);
		}
		else
		{
			if (mScores == null)
			{
				mScores = new ScoreEntry[NumScoresRetrieved];
			}
			
			string[] scores = hs_get.text.Split('\n');
			string s;
			int i;
			int separator;
			int count = 0;
			for(i=0; i<scores.Length; ++i)
			{
				s = scores[i];

				separator = s.IndexOf("\t");
				
				if (s.Length > 0 && separator > 0)
				{
				
					mScores[i].Name = s.Substring(0, separator);
					mScores[i].Score = int.Parse(s.Substring(separator));
					++count;
				}
			}
			
			if (callback != null) callback(mScores, count);
		}
	}

	/**
	 * 
	 */
	public static IEnumerator GetBestScore(GetBestScoreResult callback, Object userData)
	{
		WWW hs_get = new WWW(bestScoreURL);
		yield return hs_get;
		
		if (hs_get.error != null)
		{
			Debug.LogError("There was an error getting the high score: " + hs_get.error);
		}
		else
		{
			string s = hs_get.text;

			if (s.Length > 0)
			{
				int separator = s.IndexOf("\t");

				if (separator > 0)
				{
					if (mScores == null)
					{
						mScores = new ScoreEntry[NumScoresRetrieved];
					}

					mScores[0].Name = s.Substring(0, separator);
					mScores[0].Score = int.Parse(s.Substring(separator));
					//Debug.Log("Best score: " + mScores[0].Name + ", " + mScores[0].Score);

					if (callback != null) callback(mScores[0], userData);
				}
			}
		}
	}
}