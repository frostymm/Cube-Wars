using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDScript : MonoBehaviour 
{
	public Canvas canvas;
	public Text HealthText;
	public Image targetRecticle;
	public GameObject stuckPanel;

	public GameObject GameOverPanel, DestroyTheTargetsPanel, MultiplayerPanel;
	public Text outcomeText;

	public Button saveHighScoreButton;

	private string inputName = "";
	public void SetPlayerName(Text name)
	{
		inputName = name.text;
	}

	private bool isSaved = false;
	public void SaveHighScore()
	{
		if(!isSaved)
		{
			GameManager.Instance().SetHighScore(inputName, GameManager.Instance().GetTime());
			saveHighScoreButton.interactable = false;

			GameManager.Instance().Save();
			isSaved = true;
		}
	}

	public void RestartGame()
	{
		GameManager.Instance().RestartGame();
	}
	
	public void BackToMainMenu()
	{
		GameManager.Instance().BackToMainMenu();
	}
	
	public void BackToLobby()
	{
		if(PhotonNetwork.isMasterClient)
			NetworkManager.Instance().NMPhotonView.RPC("ReturnToLobby", PhotonTargets.All);
		else
			GameManager.Instance().BackToLobby();
	}

	void LoadDestroyTheTargetsPanel()
	{
		if(!DestroyTheTargetsPanel.activeSelf)
			DestroyTheTargetsPanel.SetActive(true);
		
		if(MultiplayerPanel.activeSelf)
			MultiplayerPanel.SetActive(false);
	}
	
	void LoadMultiplayerPanel()
	{
		if(!MultiplayerPanel.activeSelf)
			MultiplayerPanel.SetActive(true);
		
		if(DestroyTheTargetsPanel.activeSelf)
			DestroyTheTargetsPanel.SetActive(false);
	}
	
	public void CreateDamageIndicator()
	{
		GameObject go = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/UI/DamageIndicator"));
		RectTransform rt = go.GetComponent<RectTransform>();
		rt.SetParent(canvas.transform);
		rt.localScale = Vector3.one;

		rt.position = rt.parent.position;
		rt.anchorMin = new Vector2(0f, 0f);
		rt.anchorMax = new Vector2(1f, 1f);
		rt.offsetMin = new Vector2(0f, 0f);
		rt.offsetMax = new Vector2(0f, 0f);
	}
	
	private float m_CurrentHealth = 0;
	void Update()
	{
		MovingEntity me;
		if(GameManager.Instance().GetPlayers()[0] != null)
		{
			me = GameManager.Instance().GetPlayers()[0];

			if(m_CurrentHealth != me.GetHealth())
			{
				if(me.GetHealth() < m_CurrentHealth)
					CreateDamageIndicator();

				m_CurrentHealth = me.GetHealth();
				HealthText.text = "Health: " + m_CurrentHealth;
			}
		}

		if(GameManager.Instance().PlayersAreReady() && !GameManager.Instance().IsGameOver)
		{
			if(GameManager.Instance().GetPlayers()[0] != null)
			{
				me = GameManager.Instance().GetPlayers()[0];

				if(me.m_ManualFire)
				{
					if(!targetRecticle.enabled)
					{
						targetRecticle.enabled = true;
					}
				}
				else
				{
					if(targetRecticle.enabled)
					{
						targetRecticle.enabled = false;
					}
				}

				if(me.isStuck)
				{
					if(!stuckPanel.activeSelf)
						stuckPanel.SetActive(true);
				}
				else
				{
					if(stuckPanel.activeSelf)
						stuckPanel.SetActive(false);
				}
			}
		}
	}

	public GameObject highScoreInputPanel;
	void FixedUpdate()
	{
		if(GameManager.Instance().IsGameOver)
		{
			if(!GameOverPanel.activeSelf)
				GameOverPanel.SetActive(true);

			if(!GameManager.Instance().IsOnline)
			{
				LoadDestroyTheTargetsPanel();

				if(GameManager.Instance().GetGameOverOutcome() == "win")
				{
					outcomeText.text = "Congratulations! \n" +
						"Your Time Was: " + BasicUtils.GetTimeString(GameManager.Instance().GetTime());

					if(!highScoreInputPanel.activeSelf)
						highScoreInputPanel.SetActive(true);
				}
				else
				{
					outcomeText.text = "Congratulations! You Suck!";

					if(highScoreInputPanel.activeSelf)
						highScoreInputPanel.SetActive(false);
				}
			}
			else
			{
				LoadMultiplayerPanel();

				if(GameManager.Instance().GetGameOverOutcome() == "win")
					outcomeText.text = "Congratulations! \n" +
					          "You Win!";
				else
					outcomeText.text = "Congratulations! You Suck!";
			}
		}
		else
		{
			if(GameOverPanel.activeSelf)
				GameOverPanel.SetActive(false);
		}
	}

	void OnGUI()
	{
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.fontSize = 18;
		style.wordWrap = true;

		if(!GameManager.Instance().IsGameOver)
		{
			if(!GameManager.Instance().IsOnline)
			{
				GUILayout.BeginArea(new Rect((Screen.width / 2) - 30, 10, 100, 30));
				GUILayout.BeginHorizontal();
				GUI.Box(new Rect(0, 0, 100, 30), "");
				GUI.Label(new Rect(10, 0, 100, 30), "Time: " + BasicUtils.GetTimeString(GameManager.Instance().GetTime()), style);
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}

			if(GameManager.Instance().IsPaused)
			{
				GUILayout.BeginArea(new Rect((Screen.width / 2) - 100, Screen.height / 3, 200, 230));
				GUI.Box(new Rect(0, 0, 200, 230), "Paused");
				
				if(GUI.Button(new Rect(50, 40, 100, 50), "Resume"))
				{				
					GameManager.Instance().PauseGame();
				}
				if(GUI.Button(new Rect(50, 100, 100, 50), "Quit To Main"))
				{				
					GameManager.Instance().BackToMainMenu();
				}
				if(GUI.Button(new Rect(50, 160, 100, 50), "Quit"))
				{				
					GameManager.Instance().QuitGame();
				}
				GUILayout.EndArea();
			}
		}
	}
}
