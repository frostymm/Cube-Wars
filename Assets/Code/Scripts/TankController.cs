using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
public class TankController : Photon.MonoBehaviour
{
	public MovingEntity me;

	//public Camera cam;

	void Start()
	{
		me.Start();
		if(photonView.isMine)
			NetworkManager.Instance().NMPhotonView = photonView;
	}

	void Update()
	{
		me.Update(Time.deltaTime);
	}

	void FixedUpdate()
	{ 
		me.FixedUpdate();
	}

	void Awake() 
	{
		if(!GameManager.Instance().IsOnline || photonView.isMine)
			me = new PlayerEntity(gameObject);
		else
			me = new NetworkEntity(gameObject);

		me.Awake();
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		me.OnPhotonSerializeView(stream, info);
	}

	void OnApplicationExit()
	{
		NetworkManager.Instance().DestroyGameObject(gameObject);
	}

	void OnDestroy()
	{

	}

	void OnPhotonPlayerDisconnected(PhotonPlayer other)
	{
		Debug.Log("PlayerDisconnected: " + other.name);
		NetworkManager.Instance().OnPhotonPlayerDisconnected(other);
	}

	[RPC]
	public void Death()
	{
		if(!photonView.isMine)
		{
			me.OnDeath();
		}
	}

	[RPC]
	public void SendDamage(int photonID,
	                       float damage,
	                       float hitLag,
	                       float knockBack)
	{
		Debug.Log("SendDamage Called: " + photonView.viewID);
		if(photonView.isMine)
		{
			Debug.Log("ReceivingDamage" + photonView.viewID);
			HitBox hb = new HitBox(photonID, damage, hitLag, knockBack);
			me.HandleDamage(hb);
		}
	}

	[RPC]
	public void SetSpawned()
	{
		if(photonView.isMine)
		{
			me.NetworkSpawned();
		}
	}

	[RPC]
	public void BuildNetworkTank(string body, string turret)
	{
		if(!photonView.isMine)
		{
			me.SetChosenBody(body);
			me.SetChosenTurret(turret);
		}
	}

	[RPC]
	public void ReturnToLobby()
	{
		GameManager.Instance().BackToLobby();
	}

}