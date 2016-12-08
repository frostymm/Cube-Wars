using UnityEngine;
using System.Collections;

public class NetworkManager
{
	public NetworkManager()
	{

	}

	private static NetworkManager m_Instance = null;
	public static NetworkManager Instance()
	{
		if (m_Instance == null) 
		{
			m_Instance = new NetworkManager();
		}
		
		return m_Instance;
	}

	PhotonView m_PhotonView;
	public PhotonView NMPhotonView
	{
		get{ return m_PhotonView; }
		set{ m_PhotonView = value; }
	}

	public void StartServer()
	{
		RoomOptions roomOptions = new RoomOptions(){ maxPlayers = 2, cleanupCacheOnLeave = true };

		if(GetMyPlayer().name != "")
			PhotonNetwork.CreateRoom(GetMyPlayer().name + "'s room", roomOptions, TypedLobby.Default);
		else
			PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
	}

	private float m_JoinRoomTimer = 0f;
	public float joinRoomTimer
	{
		get{ return m_JoinRoomTimer; }
		set{ m_JoinRoomTimer = value; }
	}

	public void JoinRoom(RoomInfo room)
	{
		joinRoomTimer = Time.realtimeSinceStartup + 5f;
		PhotonNetwork.JoinRoom(room.name);
		GameManager.Instance().SetCurrentMenu("InRoom");
	}

	public void JoinRoom(string name)
	{
		joinRoomTimer = Time.realtimeSinceStartup + 5f;
		PhotonNetwork.JoinRoom(name);
		GameManager.Instance().SetCurrentMenu("InRoom");
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	public RoomInfo[] GetHostList()
	{
		return PhotonNetwork.GetRoomList();
	}

	public bool SpawnPointsNeedToBeSet()
	{
		bool setSpawns = false;
		foreach(PhotonPlayer player in PhotonNetwork.otherPlayers)
			if((int)player.customProperties["SpawnPoint"] == 0 || (int)player.customProperties["SpawnPoint"] == -1)
				setSpawns = true;
		return setSpawns;
	}

	public void SetPlayerSpawnPoints()
	{
		int i = 0;

		foreach(PhotonPlayer player in PhotonNetwork.playerList)
			player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {{ "SpawnPoint", i++}} );
	}

	public bool AreAllPlayersReady()
	{
		bool ready = true;

		foreach(PhotonPlayer player in PhotonNetwork.otherPlayers)
		{
			if(!(bool)player.customProperties["Ready"])
				ready = false;
		}

		return ready;
	}

	public PhotonPlayer[] GetPlayerList()
	{
		return PhotonNetwork.playerList;
	}

	public PhotonPlayer GetMyPlayer()
	{
		return PhotonNetwork.player;
	}

	public void ResetCustomProperties()
	{
		GetMyPlayer().SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {{ "Loaded", false}} );
		GetMyPlayer().SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {{ "SpawnPoint", -1}} );
		GetMyPlayer().SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {{ "Ready", false}} );
	}

	public bool AllPlayersLoaded()
	{
		bool loaded = true;
		foreach(PhotonPlayer player in PhotonNetwork.playerList)
		{
			if((bool)player.customProperties["Loaded"] == false)
				loaded = false;
		}


		return loaded;
	}

	public void SetMyPlayerCustomProperty(string name, object val)
	{
		GetMyPlayer().SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {{ name, val}} );
	}

	public object GetMyPlayerCustomProperty(string name)
	{
		return GetMyPlayer().customProperties[name];
	}

	public void DestroyGameObject(GameObject go)
	{
		if(go.GetPhotonView() && go.GetPhotonView().isMine)
			PhotonNetwork.Destroy(go.GetComponent<PhotonView>());
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer other)
	{
		foreach(BaseGameEntity bge in GameManager.Instance().GetEntitiesByPhotonOwnerID(other.ID))
		{
			bge.OnDeath();
		}
	}

	public Room GetCurrentRoom()
	{
		return PhotonNetwork.room;
	}

	public void StartGame()
	{
		SetMyPlayerCustomProperty("Ready", false);
		Debug.Log("Game Started Via Network");

		GetCurrentRoom().open = false;

		Application.LoadLevel("MultiplayerTestArena");
	}

	public void Connect()
	{
		PhotonNetwork.ConnectUsingSettings("0.1");
		GameManager.Instance().IsOnline = true;
	}

	public void Disconnect()
	{
		PhotonNetwork.Disconnect();
		GameManager.Instance().IsOnline = false;
	}
}
