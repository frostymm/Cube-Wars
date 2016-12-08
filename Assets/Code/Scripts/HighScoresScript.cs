using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScoresScript : MonoBehaviour {

	public Text highScoreText;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(GameManager.Instance().isHighScoreschanged)
		{
			string scoresText = "";
			for(int i = 0; i < GameManager.Instance().GetScores().Length; i++)
			{
				HighScore hs = GameManager.Instance().GetScores()[i];
				scoresText += (i+1) + ". " + hs.m_Name + ": " + BasicUtils.GetTimeString(hs.m_Score) + "\n";
			}

			highScoreText.text = scoresText;
			GameManager.Instance().isHighScoreschanged = false;
		}
	}
}
