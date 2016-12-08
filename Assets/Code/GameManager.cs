using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class GameManager{

	List<MovingEntity> m_Players = new List<MovingEntity>(); //All players
	Dictionary<GameObject, BaseGameEntity> m_Entities = new Dictionary<GameObject, BaseGameEntity>();
	Dictionary<Collider, BaseGameEntity> m_EntitiesCollider = new Dictionary<Collider, BaseGameEntity>();
	Dictionary<int, BaseGameEntity> m_EntitiesPhotonID = new Dictionary<int, BaseGameEntity>();
	List<BaseGameEntity>[] m_Teams = new List<BaseGameEntity>[4]; //List of participants for each team
	private bool m_GameStarted = false;
	public bool DebugMode = false;
	private bool m_GameOver = false;
	private string m_GameOverOutcome = "lose";
	private float m_Time;
	private bool m_Pause;
	private bool m_Online;
	private List<Transform> m_SpawnPoints = new List<Transform>();
	public float NetworkMoveSpeed = 1.0f;

	private static GameManager m_Instance = null;
	public static GameManager Instance()
	{
		if (m_Instance == null) 
		{
			m_Instance = new GameManager();
		}
		
		return m_Instance;
	}

	public enum BodyParts
	{
		BaseBody,
		HoverBody
	}

	public enum TurretParts
	{
		BaseTurret,
		MissileTurret,
		LaserTurret
	}

	LocalPlayer m_MyLocalPlayer;
	public LocalPlayer GetMyPlayer()
	{
		if(m_MyLocalPlayer == null)
			m_MyLocalPlayer = new LocalPlayer();

		return m_MyLocalPlayer;
	}

	public string[] GetBodyPartsList(){return Enum.GetNames(typeof(BodyParts));}
	public string[] GetTurretPartsList(){return Enum.GetNames(typeof(TurretParts));}

	private string m_CurrentMenu = "Main";
	public string GetCurrentMenu(){ return m_CurrentMenu; }

	public void SetCurrentMenu(string currMenu){ m_CurrentMenu = currMenu; }

	public bool IsOnline
	{
		get{ return m_Online; }
		set
		{ 
			m_Online = value;
			PhotonNetwork.offlineMode = !value;
		}
	}

	public bool IsPaused
	{
		get{ return m_Pause; }
		set{ m_Pause = value; }
	}

	public bool IsGameOver
	{
		get{ return m_GameOver; }
		set{ m_GameOver = value; }
	}

	public void StartGame()
	{
		Debug.Log("Start Game");
		if(GameManager.Instance().IsOnline)
		{
			InitializeOnlineTeams();
		}

		foreach(BaseGameEntity ME in m_Entities.Values)
			AddTeamMember(ME);

		SetGameStarted(true);

		ForceUnPause();
	}

	public void RegisterParticipant(BaseGameEntity ME)
	{
		m_Entities.Add(ME.GetGameObject(), ME);

		if(ME.GetCollider() != null)
			m_EntitiesCollider.Add(ME.GetCollider(), ME);

		if(IsOnline)
		{
			m_EntitiesPhotonID.Add(ME.GetGameObject().GetPhotonView().viewID, ME);
			RegisterEntityByPhotonOwnerID(ME.GetGameObject().GetPhotonView().ownerId, ME);
		}
	}

	public void RegisterCollider(BaseGameEntity ME)
	{
		if(ME.GetCollider() != null && !m_EntitiesCollider.ContainsKey(ME.GetCollider()))
			m_EntitiesCollider.Add(ME.GetCollider(), ME);
	}

	public void RegisterCollider(BaseGameEntity ME, Collider Col)
	{
		if(!m_EntitiesCollider.ContainsKey(Col))
			m_EntitiesCollider.Add(Col, ME);
	}

	public void AddTeamMember( BaseGameEntity ME )
	{
		if(m_Teams[ME.GetTeam()] == null)
		{
			m_Teams[ME.GetTeam()] = new List<BaseGameEntity>();
		}

		m_Teams[ME.GetTeam()].Add(ME);
	}

	Dictionary<int, List<BaseGameEntity>> m_EntitiesPhotonOwnerID = new Dictionary<int, List<BaseGameEntity>>();
	public List<BaseGameEntity> GetEntitiesByPhotonOwnerID(int ID)
	{
		return m_EntitiesPhotonOwnerID[ID];
	}

	public void RegisterEntityByPhotonOwnerID(int ID, BaseGameEntity bge)
	{
		if(!m_EntitiesPhotonOwnerID.ContainsKey(ID))
		{
			m_EntitiesPhotonOwnerID.Add(ID, new List<BaseGameEntity>());
		}
		m_EntitiesPhotonOwnerID[ID].Add(bge);
	}

	public BaseGameEntity GetEntityByPhotonID(int ID)
	{
		return m_EntitiesPhotonID[ID];
	}

	public void InitializeOnlineTeams()
	{
		foreach(BaseGameEntity ME in m_Entities.Values)
		{
			if(ME.GetGameObject().GetPhotonView().owner.isMasterClient)
				ME.SetTeam(0);
			else
				ME.SetTeam(1);
		}
	}

	public void AddPlayer(PlayerEntity PE)
	{
		m_Players.Add(PE);
	}

	public List<MovingEntity> GetPlayers()
	{
		return m_Players;
	}

	public Dictionary<GameObject, BaseGameEntity> GetParticipants(){ return m_Entities; }

	public Dictionary<Collider, BaseGameEntity> GetParticipantByCollider(){ return m_EntitiesCollider; }

	Dictionary<GameObject, BaseGameEntity> m_EntitiesByTargeters = new Dictionary<GameObject, BaseGameEntity>();
	public Dictionary<GameObject, BaseGameEntity> GetParticipantsByTargeterObject()
	{
		return m_EntitiesByTargeters;
	}

	public void SetGameStarted(bool b){ m_GameStarted = b; }
	public bool GetGameStarted(){ return m_GameStarted; }

	public bool IsOneTeamLeft(out int team)
	{
		int currTeam = -1;
		team = 0;
		foreach(BaseGameEntity be in GetParticipants().Values)
		{
			if(!be.GetIsDead())
			{
				if(currTeam == -1)
					currTeam = be.GetTeam();

				if(currTeam != be.GetTeam())
					return false;
			}
		}

		team = currTeam;

		return true;
	}

	public float GetTime() { return m_Time; }

	public string GetGameOverOutcome() { return m_GameOverOutcome; }

	public void GameOver(int team)
	{
		if(GetPlayers().Count > 0)
		{
			m_GameOver = true;

			if(team == GetPlayers()[0].GetTeam())
				m_GameOverOutcome = "win";
			else
				m_GameOverOutcome = "lose";
		}

		Screen.showCursor = true;
		Screen.lockCursor = false;
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void DestroyPhotonObjects()
	{
		PhotonNetwork.DestroyPlayerObjects(NetworkManager.Instance().GetMyPlayer());
	}

	public void ClearVariables()
	{
		SetGameStarted(false);
		DebugMode = false;
		IsGameOver = false;
		m_Time = 0f;
		
		//Clear references
		m_Entities.Clear();
		m_EntitiesCollider.Clear();
		m_Teams = new List<BaseGameEntity>[4];
		m_Players.Clear();
		m_EntitiesPhotonID.Clear();
		m_EntitiesByTargeters.Clear();

		IsPaused = false;
		Time.timeScale = 1;

		m_EntitiesSpawned = false;
		NetworkManager.Instance().ResetCustomProperties();
		DestroyPhotonObjects();
	}

	public void BackToLobby()
	{
		Debug.Log ("Back To Lobby");

		ClearVariables();

		//Set Main Menu to load In Lobby
		SetCurrentMenu("InRoom");
		
		//reload scene
		Application.LoadLevel("MainMenu");
	}

	public void BackToMainMenu()
	{
		Debug.Log ("Back To MainMenu");
		
		ClearVariables();

		//Clear Network References
		if(IsOnline)
		{
			NetworkManager.Instance().LeaveRoom();
			NetworkManager.Instance().Disconnect();
			m_EntitiesPhotonID.Clear();
		}

		SetCurrentMenu("Main");
		
		//reload scene
		Application.LoadLevel("MainMenu");
	}

	public void RestartGame()
	{
		Debug.Log ("Restarting Level");
		
		ClearVariables();
		
		//reload scene
		Application.LoadLevel(Application.loadedLevelName);
	}

	public void ForceUnPause()
	{
		Time.timeScale = 1;
		IsPaused = false;

		Screen.showCursor = false;
		Screen.lockCursor = true;
	}

	public void ForcePause()
	{
		Time.timeScale = 0;
		IsPaused = true;

		Screen.showCursor = true;
		Screen.lockCursor = false;
	}

	public void PauseGame()
	{
		if(IsPaused)
		{
			if(!IsOnline)
				Time.timeScale = 1;

			IsPaused = false;

			Screen.showCursor = false;
			Screen.lockCursor = true;
		}
		else
		{
			if(!IsOnline)
				Time.timeScale = 0;

			IsPaused = true;

			Screen.showCursor = true;
			Screen.lockCursor = false;
		}
	}

	public void SetSpawnPoints(List<Transform> spawnPoints)
	{
		m_SpawnPoints = spawnPoints;
	}

	public GameObject Instantiate(string path, Vector3 position, Quaternion rotation)
	{
		if(IsOnline)
			return (GameObject)PhotonNetwork.Instantiate(path, position, rotation, 0);
		else
			return (GameObject)MonoBehaviour.Instantiate(Resources.Load(path), position, rotation);
	}

	private bool m_EntitiesSpawned = false;
	public void SpawnEntities()
	{
		int pointID = (int)NetworkManager.Instance().GetMyPlayerCustomProperty("SpawnPoint");

		Debug.Log("Spawning Entities");
		if(IsOnline)
		{
			Instantiate("Prefabs/tank", 
			            m_SpawnPoints[pointID].position,
			            m_SpawnPoints[pointID].rotation);
		}

		m_EntitiesSpawned = true;
	}

	public bool PlayersAreReady()
	{
		bool ready = true;

		foreach(KeyValuePair<GameObject, BaseGameEntity>  entity in m_Entities)
		{
			if(!entity.Value.GetIsBuilt())
				ready = false;
		}

		if(PhotonNetwork.playerList.Length > m_Entities.Count)
			ready = false;

		return ready;
	}

	private bool m_HighScoresChanged = false;
	public bool isHighScoreschanged
	{
		get{ return m_HighScoresChanged; }
		set{ m_HighScoresChanged = value; }
	}
	private List<HighScore> m_HighScores = new List<HighScore>();
	private int m_NumberOfHighScores = 5;
	public void SetHighScore(string name, float score)
	{
		HighScore highScore = new HighScore(name, score);

		m_HighScores.Add(highScore);
		m_HighScores = m_HighScores.OrderBy(x => x.m_Score).ToList<HighScore>();
		
		if(m_HighScores.Count > m_NumberOfHighScores)
			m_HighScores.RemoveAt(m_HighScores.Count - 1);

		m_HighScoresChanged = true;
	}
	
	public HighScore[] GetScores()
	{
		return m_HighScores.ToArray();
	}
	
	
	public void Save()
	{
		if(m_HighScores.Count > 0)
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create (Application.persistentDataPath + "/ProjectToonWars.dat");
			
			SaveData data = new SaveData();

			//Set SaveData
			data.SetVariables();
			
			bf.Serialize(file, data);
			file.Close();
			
			if(DebugMode)
				Debug.Log("Saving File to: " + Application.persistentDataPath + "/ProjectToonWars.dat");
		}
	}
	
	public void Load()
	{
		if(File.Exists(Application.persistentDataPath + "/ProjectToonWars.dat"))
		{
			m_HighScores.Clear();

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/ProjectToonWars.dat", FileMode.Open);
			
			SaveData data = (SaveData)bf.Deserialize(file);
			file.Close();
			
			//Set SaveData
			data.RetrieveVariables();
			
			Debug.Log("Data Loaded");
		}
	}
	
	public void DeleteSave()
	{
		if(File.Exists(Application.persistentDataPath + "/ProjectToonWars.dat"))
		{
			File.Delete(Application.persistentDataPath + "/ProjectToonWars.dat");
		}
		
		m_HighScores.Clear();
		
		Debug.Log("Save Deleted");
	}

	public void OnApplicationFocus(bool focusStatus)
	{
		Debug.Log("FocusStatus: " + focusStatus);

		if(focusStatus)
		{
			if(GetGameStarted() && !IsGameOver && !IsPaused)
			{
				Screen.showCursor = false;
				Screen.lockCursor = true;
			}
		}
	}

	// Use this for initialization
	public void Start () {
		Screen.showCursor = false;
		Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	public void Update () {
		if(!GetGameStarted() && !Application.isLoadingLevel)
			NetworkManager.Instance().SetMyPlayerCustomProperty("Loaded", true);

		if(NetworkManager.Instance().AllPlayersLoaded() && !m_EntitiesSpawned)
			SpawnEntities();

		if(!GetGameStarted() && PlayersAreReady())
			StartGame();
		else if (GetGameStarted())
		{
			if(Input.GetButtonDown("DebugToggle"))
			   DebugMode = !DebugMode;

			if(!IsGameOver)
			{
				int teamWon = 0;
				if(IsOneTeamLeft(out teamWon))
				   GameOver(teamWon);

				if(DebugMode)
				{
					if(Input.GetKeyDown(KeyCode.Backspace))
					{
						GameOver(GetPlayers()[0].GetTeam());
					}
					if(Input.GetKeyDown(KeyCode.Minus))
					{
						GetPlayers()[0].ChangeTurret(typeof(LaserTurret));
					}
				}

				if(Input.GetButtonDown("Pause"))
				{
					PauseGame();
				}

				m_Time += Time.deltaTime;
			}
		}

	}
}

public class LocalPlayer
{
	public int selectedBody = 0;
	public int selectedTurret = 0;

	public string GetSelectedBodyString()
	{
		GameManager.BodyParts bp = (GameManager.BodyParts)selectedBody;

		return bp.ToString();
	}

	public string GetSelectedTurretString()
	{
		GameManager.TurretParts tp = (GameManager.TurretParts)selectedTurret;
		
		return tp.ToString();
	}
}

[Serializable]
public class HighScore
{
	public string m_Name;
	public float m_Score;
	
	public HighScore(string name, float score)
	{
		m_Name = name;
		m_Score = score;
	}
}

[Serializable]
public class SaveData
{
	public HighScore[] scores;

	public SaveData()
	{
	}

	public void SetVariables()
	{
		scores = GameManager.Instance().GetScores();
	}

	public void RetrieveVariables()
	{
		foreach(HighScore score in scores)
		{
			GameManager.Instance().SetHighScore(score.m_Name, score.m_Score);
		}
	}
}
