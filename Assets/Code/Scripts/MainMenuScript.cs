using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainMenuScript : MonoBehaviour
{
	public string currentMenu = "";
	public GameObject mainPanel, singlePanel, multiPanel, howToPanel, optionsPanel, serverListPanel;
	private GameObject previousPanel;

	public void LoadGame()
	{
		GameManager.Instance().Load();
	}

	public void DisableAllMenus()
	{
		if(mainPanel.activeSelf)
		{
			mainPanel.SetActive(false);
			previousPanel = mainPanel;
		}
		if(singlePanel.activeSelf)
		{
			singlePanel.SetActive(false);
			previousPanel = singlePanel;
		}
		if(multiPanel.activeSelf)
		{
			multiPanel.SetActive(false);
			previousPanel = multiPanel;
		}
		if(howToPanel.activeSelf)
		{
			howToPanel.SetActive(false);
			previousPanel = howToPanel;
		}
		if(optionsPanel.activeSelf)
		{
			optionsPanel.SetActive(false);
			previousPanel = optionsPanel;
		}
	}

	public void LoadMenu(GameObject menu)
	{
		DisableAllMenus();

		if(menu == multiPanel)
		{
			NetworkManager.Instance().Connect();
			GameManager.Instance().SetCurrentMenu("Multi");
			GameManager.Instance().IsOnline = true;
			PhotonNetwork.offlineMode = false;
			
			LoadLobby();
		}
		if(menu == mainPanel)
		{
			NetworkManager.Instance().Disconnect();
			GameManager.Instance().SetCurrentMenu("Main");
			GameManager.Instance().IsOnline = false;
		}
		if(menu == singlePanel)
		{
			GameManager.Instance().SetCurrentMenu("Single");
			PhotonNetwork.offlineMode = true;
		}

		if(!menu.activeSelf)
		{
			menu.SetActive(true);
		}
	}

	public void ReloadRoom()
	{
		NetworkManager.Instance().NMPhotonView = this.GetComponent<PhotonView>();
		DisableAllMenus();

		multiPanel.SetActive(true);

		LoadRoom();
	}

	public void LoadPrevious()
	{
		LoadMenu(previousPanel);
	}

	public void LoadTargetTest()
	{
		Application.LoadLevel("TestScene");
	}

	public void Quit()
	{
		Application.Quit();
	}

	public GameObject LobbyPanel, RoomPanel;
	public void LoadRoom()
	{
		Debug.Log("Loading Room");
		if(LobbyPanel.activeSelf)
			LobbyPanel.SetActive(false);
		
		if(!RoomPanel.activeSelf)
			RoomPanel.SetActive(true);

		SetSelectPartButtons();
	}

	public void LoadLobby()
	{
		if(!LobbyPanel.activeSelf)
			LobbyPanel.SetActive(true);
		
		if(RoomPanel.activeSelf)
			RoomPanel.SetActive(false);
	}

	public void SetPlayerName(string name)
	{
		NetworkManager.Instance().GetMyPlayer().name = name;
	}

	public void SetPlayerName(Text name)
	{
		NetworkManager.Instance().GetMyPlayer().name = name.text;
	}

	public void CreateServer()
	{
		NetworkManager.Instance().StartServer();
		GameManager.Instance().SetCurrentMenu("InRoom");
	}

	void CreateServerObject(RoomInfo room)
	{
		GameObject GO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/ServerObject"));
		ServerObject SO = GO.GetComponent<ServerObject>();
		SO.room = room;
		SO.transform.SetParent(serverListPanel.transform, false);

		m_Rooms.Add(room.name, SO);
	}

	void DeleteServerObject(string roomName)
	{
		Destroy(m_Rooms[roomName].gameObject);
		m_Rooms.Remove(roomName);
	}

	void CorrectServerObjectPositions()
	{
		Vector2 newPos;
		RectTransform trans;

		ServerObject[] serverObjects = m_Rooms.Values.ToArray();
		for(int i = 0; i < serverObjects.Length; i++)
		{
			trans = m_Rooms[serverObjects[i].room.name].GetComponent<RectTransform>();

			newPos = trans.parent.position;
			newPos.y = trans.parent.position.y + (-30 * i);

			trans.position = newPos;
		}

		if(serverObjects.Length > 5)
		{
			RectTransform panel = serverListPanel.GetComponent<RectTransform>();
			Vector2 size = panel.sizeDelta;

			size.y = 200 + (serverObjects.Length - 6) * 20;

			panel.sizeDelta = size;
		}
	}

	public Button baseBodyButton, hoverBodyButton, baseTurretButton, laserTurretButton, missileTurretButton;
	public void SelectPart(Text partName)
	{
		if(partName.text == "Standard Body")
		{
			DarkenAllBodyPartButtons();
			LightenButton(baseBodyButton);
			GameManager.Instance().GetMyPlayer().selectedBody = (int)GameManager.BodyParts.BaseBody;
		}
		if(partName.text == "Hover")
		{
			DarkenAllBodyPartButtons();
			LightenButton(hoverBodyButton);
			GameManager.Instance().GetMyPlayer().selectedBody = (int)GameManager.BodyParts.HoverBody;
		}

		if(partName.text == "Standard Turret")
		{
			DarkenAllTurretPartButtons();
			LightenButton(baseTurretButton);
			GameManager.Instance().GetMyPlayer().selectedTurret = (int)GameManager.TurretParts.BaseTurret;
		}
		if(partName.text == "Laser")
		{
			DarkenAllTurretPartButtons();
			LightenButton(laserTurretButton);
			GameManager.Instance().GetMyPlayer().selectedTurret = (int)GameManager.TurretParts.LaserTurret;
		}
		if(partName.text == "Missile")
		{
			DarkenAllTurretPartButtons();
			LightenButton(missileTurretButton);
			GameManager.Instance().GetMyPlayer().selectedTurret = (int)GameManager.TurretParts.MissileTurret;
		}
	}

	public void SetSelectPartButtons()
	{
		DarkenAllBodyPartButtons();
		
		int i = GameManager.Instance().GetMyPlayer().selectedBody;
		if(i == (int)GameManager.BodyParts.BaseBody)
			LightenButton(baseBodyButton);
		else if(i == (int)GameManager.BodyParts.HoverBody)
			LightenButton(hoverBodyButton);
		
		
		DarkenAllTurretPartButtons();
		
		i = GameManager.Instance().GetMyPlayer().selectedTurret;
		if(i == (int)GameManager.TurretParts.BaseTurret)
			LightenButton(baseTurretButton);
		else if(i == (int)GameManager.TurretParts.LaserTurret)
			LightenButton(laserTurretButton);
		else if(i == (int)GameManager.TurretParts.MissileTurret)
			LightenButton(missileTurretButton);
	}

	public void LightenButton(Button button)
	{
		button.GetComponent<Image>().color = Color.white;
	}

	public void DarkenAllBodyPartButtons()
	{
		baseBodyButton.image.color = Color.gray;
		hoverBodyButton.image.color = Color.gray;
	}

	public void DarkenAllTurretPartButtons()
	{
		baseTurretButton.image.color = Color.gray;
		laserTurretButton.image.color = Color.gray;
		missileTurretButton.image.color = Color.gray;
	}

	public void LeaveRoom()
	{
		NetworkManager.Instance().LeaveRoom();
		GameManager.Instance().SetCurrentMenu("Multi");
		LoadLobby();
	}

	private bool ready = false;
	public void ReadyStartPress()
	{
		if(NetworkManager.Instance().GetMyPlayer().isMasterClient)
			NetworkManager.Instance().NMPhotonView.RPC("StartGame", PhotonTargets.All);
		else
		{
			ready = !ready;
			NetworkManager.Instance().SetMyPlayerCustomProperty("Ready", ready);
		}
	}

	private bool listHasChanged = false;
	private Dictionary<int, RoomParticipant> m_RoomParticipants = new Dictionary<int, RoomParticipant>();
	public void PopulatePlayerList()
	{
		PhotonPlayer[] players = NetworkManager.Instance().GetPlayerList();
		List<int> IDs = new List<int>();
		for(int i = 0; i < players.Length; i++)
		{
			IDs.Add(players[i].ID);
			if(!m_RoomParticipants.ContainsKey(players[i].ID))
			{
				//Add Player To Dictionary
				AddToPlayerList(players[i]);
				listHasChanged = true;
			}
		}

		int[] iDKeys = m_RoomParticipants.Keys.ToArray();
		for(int i = m_RoomParticipants.Count -1; i >= 0; i--)
		{
			if(!IDs.Contains(iDKeys[i]))
			{
				//Remove Player From Dictinary
				RemoveFromPlayerList(iDKeys[i]);
				listHasChanged = true;
			}
		}

		if(listHasChanged)
			UpdatePlayerListPositions();
	}

	public void UpdatePlayerListPositions()
	{
		Vector2 newPos;
		RectTransform trans;
		
		RoomParticipant[] roomParticipants = m_RoomParticipants.Values.ToArray();
		for(int i = 0; i < roomParticipants.Length; i++)
		{
			trans = roomParticipants[i].GetComponent<RectTransform>();
			
			newPos = trans.parent.position;
			newPos.y = trans.parent.position.y + (-30 * i);
			
			trans.position = newPos;
		}
	}

	public void AddToPlayerList(PhotonPlayer player)
	{
		GameObject GO = (GameObject)Instantiate(Resources.Load("Prefabs/UI/RoomParticipant"));
		RoomParticipant RP = GO.GetComponent<RoomParticipant>();
		RP.player = player;
		RP.transform.SetParent(playerListMyPlayerText.transform, false);
		
		m_RoomParticipants.Add(player.ID, RP);
	}

	public void RemoveFromPlayerList(int ID)
	{
		Destroy(m_RoomParticipants[ID].gameObject);
		m_RoomParticipants.Remove(ID);
	}

	public void DeleteSave()
	{
		GameManager.Instance().DeleteSave();
	}

	void Awake()
	{
		if(GameManager.Instance().GetCurrentMenu() == "InRoom")
		{
			ReloadRoom();
		}
	}

	// Use this for initialization
	void Start () 
	{
		NetworkManager.Instance().ResetCustomProperties();
		GameManager.Instance().Load();
	}

	void DeleteAllRoomObjects()
	{
		ServerObject[] serverObjects = m_Rooms.Values.ToArray();
		for(int i = serverObjects.Length -1; i >= 0; i--)
		{
			DeleteServerObject(serverObjects[i].room.name);
		}
	}
	
	public Button createServerButton, readyButton;
	public Text noServersFoundText;
	public Text playerListMyPlayerText;
	public Text roomNameText;
	public Text readyButtonText;
	private Dictionary<string, ServerObject> m_Rooms = new Dictionary<string, ServerObject>();
	private float RoomRefreshRate = 5f;
	private float RoomRefreshTimer = 0f;
	void FixedUpdate()
	{
		if ((GameManager.Instance().GetCurrentMenu() == "Multi") && (!Network.isClient && !Network.isServer))
		{
			if(!PhotonNetwork.insideLobby)
				createServerButton.interactable = false;
			else
				if(!createServerButton.IsInteractable())
					createServerButton.interactable = true;

			if(NetworkManager.Instance().GetHostList().Length != 0)
			{
				if(noServersFoundText.text != "")
					noServersFoundText.text = "";

				if(RoomRefreshTimer < Time.realtimeSinceStartup)
				{
					DeleteAllRoomObjects();
					RoomRefreshTimer = Time.realtimeSinceStartup + RoomRefreshRate;
				}

				foreach(RoomInfo room in NetworkManager.Instance().GetHostList())
				{
					if(!m_Rooms.ContainsKey(room.name))
						CreateServerObject(room);
				}
				
				ServerObject[] serverObjects = m_Rooms.Values.ToArray();
				for(int i = serverObjects.Length -1; i >= 0; i--)
				{
					if(serverObjects[i].room.removedFromList)
						DeleteServerObject(serverObjects[i].room.name);
				}
				
				CorrectServerObjectPositions();
			}
			else
			{
				DeleteAllRoomObjects();

				noServersFoundText.text = "No Hosts Found";
			}
		}
		else if(GameManager.Instance().GetCurrentMenu() == "InRoom")
		{
			if(PhotonNetwork.inRoom)
			{
				if(roomNameText.text != PhotonNetwork.room.name)
					roomNameText.text = PhotonNetwork.room.name;

				LoadRoom();
			}
			else
			{
				if(Time.realtimeSinceStartup > NetworkManager.Instance().joinRoomTimer)
					LoadLobby();
			}

			if (NetworkManager.Instance().GetMyPlayer().isMasterClient)
			{
				if(readyButtonText.text != "Start!")
					readyButtonText.text = "Start!";

				if(NetworkManager.Instance().AreAllPlayersReady() && NetworkManager.Instance().GetPlayerList().Count() > 1)
				{
					if(!readyButton.IsInteractable())
						readyButton.interactable = true;
				}
				else
				{
					if(readyButton.IsInteractable())
						readyButton.interactable = false;
				}

				if(!NetworkManager.Instance().GetCurrentRoom().open)
					NetworkManager.Instance().GetCurrentRoom().open = true;


			}
			else
			{
				if(!readyButton.IsInteractable())
					readyButton.interactable = true;

				if(readyButtonText.text != "Ready!")
					readyButtonText.text = "Ready!";
			}

			PopulatePlayerList();
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if(PhotonNetwork.isMasterClient && NetworkManager.Instance().SpawnPointsNeedToBeSet())
			NetworkManager.Instance().SetPlayerSpawnPoints();

		currentMenu = GameManager.Instance().GetCurrentMenu();
	}
	
	void OnJoinedLobby()
	{
		NetworkManager.Instance().NMPhotonView = this.GetComponent<PhotonView>();
	}

	void OnJoinedRoom()
	{
		if(PhotonNetwork.isMasterClient)
			NetworkManager.Instance().SetPlayerSpawnPoints();

		if(gameObject.GetPhotonView().isMine)
		{
			GameManager.Instance().SetCurrentMenu("InRoom");
			LoadRoom();
		}
	}
	
	[RPC]
	public void StartGame()
	{
		NetworkManager.Instance().StartGame();
	}
}

