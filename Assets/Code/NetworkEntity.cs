using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NetworkEntity : MovingEntity 
{
	public NetworkEntity(GameObject go) : base(go)
	{
		m_EntityType = EntityTypes.entity_type.ET_Network;
	}
	
	~NetworkEntity()
	{
	}

	public void BuildTank(string body, string turret)
	{
		if(body != null && turret != null)
		{
			if(!IsBuilding)
			{
				if(!GetTankParts()[(int)TankParts.body])
				{
					IsBuilding = true;
					SetBody(BuildNewBody("Net" + body));
				}
				else if(!GetTankParts()[(int)TankParts.turret])
				{
					IsBuilding = true;
					SetTurret(BuildNewTurret("Net" + turret));
				}
				else
				{
					SetIsBuilt(true);
				}
			}
		}
	}

	private int UpdateFrames = 0;
	public void UpdateStream()
	{
		if(UpdateFrames++ > 30)
		{
			if(GetBody() != null)
			{
				GetBody().GetGameObject().transform.position = Vector3.Lerp(GetBody().GetGameObject().transform.position, bodyPos, Time.deltaTime * 5);
				GetBody().GetGameObject().transform.rotation = Quaternion.Lerp(GetBody().GetGameObject().transform.rotation, bodyRot, Time.deltaTime * 5);
			}
			if(GetTurret() != null)
			{
				GetTurret().barrel.transform.rotation = Quaternion.Lerp(GetTurret().barrel.transform.rotation, barrelRot, Time.deltaTime * 1);
				GetTurret().turret.transform.rotation = Quaternion.Lerp(GetTurret().turret.transform.rotation, turretRot, Time.deltaTime * 1);
			}
		}
	}
	
	public override void FixedUpdate()
	{
		if(!GameManager.Instance().IsGameOver)
		{
			base.FixedUpdate();
		}
		else
		{
			SetMovement(0f, 0f);
		}
	}
	
	public override void Start()
	{
		base.Start();
		GetGameObject().GetPhotonView().RPC("SetSpawned", PhotonTargets.All);
	}
	
	public override void Update(float dt)
	{
		if(!GameManager.Instance().GetGameStarted())
			BuildTank(GetChosenBody(), GetChosenTurret());
		else
			UpdateStream();
		
		if(!GameManager.Instance().IsGameOver && GameManager.Instance().GetGameStarted())
		{
			base.Update(dt);
		}

	}
}
