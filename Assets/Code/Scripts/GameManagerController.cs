using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameManagerController : MonoBehaviour {

	public List<Transform> spawnPoints = new List<Transform>();
	public float NetworkMoveSpeed = 1.0f;

	public bool isOnline;
	public bool isGameOver;

	public BaseGameEntity[] participants;
	public bool[] participantsDead;

	// Use this for initialization
	void Start () {
		GameManager.Instance().SetSpawnPoints(spawnPoints);
		GameManager.Instance().Start ();
	}

	void FixedUpdate()
	{
		isOnline = GameManager.Instance().IsOnline;
		isGameOver = GameManager.Instance().IsGameOver;

		participants = GameManager.Instance().GetParticipants().Values.ToArray();

		List<bool> deaths = new List<bool>();
		foreach(BaseGameEntity be in participants)
		{
			deaths.Add(be.GetIsDead());
		}
		participantsDead = deaths.ToArray();
	}
	
	// Update is called once per frame
	void Update () {
		GameManager.Instance().Update();
		GameManager.Instance().NetworkMoveSpeed = NetworkMoveSpeed;
	}

	void OnApplicationFocus(bool focusStatus)
	{
		GameManager.Instance().OnApplicationFocus(focusStatus);
	}
}
