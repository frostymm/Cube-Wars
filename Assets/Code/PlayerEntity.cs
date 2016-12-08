using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerEntity : MovingEntity 
{
	public Dictionary<GameObject, Targeter> m_Targeters = new Dictionary<GameObject,Targeter>();

	public PlayerEntity(GameObject go) : base(go)
	{
		if(!GameManager.Instance().IsOnline || go.GetPhotonView().isMine)
			SetCam((Camera)MonoBehaviour.Instantiate(Resources.Load("Prefabs/Camera", typeof(Camera)),
			                                         go.transform.position, go.transform.rotation));

		m_EntityType = EntityTypes.entity_type.ET_Playable;

		m_Speed = 4;

		AddBehavior(new Seek());
		GetBehavior("Seek").SetIsActive(true);

		GameManager.Instance().AddPlayer(this);

		SetChosenBody(GameManager.Instance().GetMyPlayer().GetSelectedBodyString());
		SetChosenTurret(GameManager.Instance().GetMyPlayer().GetSelectedTurretString());
	}

	~PlayerEntity()
	{

	}

	public bool CanSeeTargetThroughTerrain(GameObject go)
	{
		RaycastHit rayhit = new RaycastHit();

		if(Physics.Linecast(GetCam().transform.position, go.transform.position, out rayhit))
		{
			float dist = Vector3.Distance(GetCam().transform.position, go.transform.position);
			if(Mathf.Abs(dist - rayhit.distance)  > 10f)
			{
				return false;
			}
		}

		return true;
	}

	public void FindTargets ()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(GetCam());
		List<GameObject> targets = new List<GameObject>();


		foreach(BaseGameEntity ME in GameManager.Instance().GetParticipants().Values)
		{
			if(ME.GetCollider() != null && ME != this) //Check that ME has collider to look for and is not this
			{
				float dist = Vector3.Distance(GetPosition(), ME.GetPosition());



				if( dist <= GetTargetingDistance()										// Is the object close enough?
					&& GeometryUtility.TestPlanesAABB(planes, ME.GetCollider().bounds)
				   && !ME.GetIsDead() 
				   && CanSeeTargetThroughTerrain(ME.GetTargetGameObject()))// Can I see the object?
					targets.Add(ME.GetTargetGameObject());
				else
				{
					if(m_Targeters.ContainsKey(ME.GetTargetGameObject()))// remove objects I can't see or are too far away
					{
						RemoveTargeter(ME.GetTargetGameObject());
					}
				}
			}
		}
		
		
		targets =  targets.OrderBy(x => (Vector3.Distance(GetPosition(), x.transform.position))).ToList();
		for(int i = targets.Count -1; i >= 0; i--)
		{
			if((i > GetTurret().GetMaxTargets() - 1))
			{
				RemoveTargeter(targets[i]);// Remove objects that are farthest away

				targets.RemoveAt(i);
			}
		}

		foreach(GameObject targ in targets)
		{
			if(!m_Targeters.ContainsKey(targ))
				m_Targeters.Add(targ, new Targeter(targ, this));
		}

		m_Targets = targets;
	}

	public void RemoveTargeter(GameObject go)
	{
		if(m_Targeters.ContainsKey(go))
		{
			m_Targeters[go].active = false;
			m_Targeters.Remove(go);
		}
	}

	public void UpdateInput()
	{
		if(GameManager.Instance().IsPaused)
		{
			m_Fire = false;
			m_Jump = false;
			m_Unstuck = false;
		}
		else
		{
			if(Input.GetButtonDown("ManualFire"))
				m_ManualFire = !m_ManualFire;

			m_Fire = Input.GetButton("Fire");
			m_Jump = Input.GetButton("Jump");
			m_Unstuck = Input.GetButton("UnStuck");

			SetMovement(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
		}
	}

	public void BuildTank(string body, string turret)
	{
		if(!IsBuilding)
		{
			if(!GetTankParts()[(int)TankParts.body])
			{
				IsBuilding = true;
				SetBody(BuildNewBody(body));
			}
			else if(!GetTankParts()[(int)TankParts.turret])
			{
				IsBuilding = true;
				SetTurret(BuildNewTurret(turret));
			}
			else
			{
				SetIsBuilt(true);

				if(GameManager.Instance().IsOnline && m_GameObject.GetPhotonView().isMine)
					GetGameObject().GetPhotonView().RPC("BuildNetworkTank", PhotonTargets.All, GetChosenBody(), GetChosenTurret());
			}
		}
	}

	public override void FixedUpdate()
	{
		if(!GameManager.Instance().IsGameOver)
		{
			if(GameManager.Instance().GetGameStarted() && GetTurret() != null)
				FindTargets();

			base.FixedUpdate();
		}
		else
		{
			SetMovement(0f, 0f);
			if(m_Targeters.Count > 0)
			{
				for(int i = m_Targeters.Keys.Count-1; i >= 0; i--)
				{
					RemoveTargeter(m_Targeters.Keys.ElementAt(i));
				}
			}
		}
	}

	public override void Start()
	{
		base.Start();
	}

	public override void Update(float dt)
	{
		if(!GameManager.Instance().IsGameOver)
			UpdateInput();

		if(!GameManager.Instance().IsOnline || GetGameObject().GetPhotonView().isMine)
		{
			if(!GameManager.Instance().GetGameStarted() && !GetIsBuilt() && GetIsNetworkSpawned())
				BuildTank(GetChosenBody(), GetChosenTurret());

			if(!GameManager.Instance().IsGameOver && GameManager.Instance().GetGameStarted())
			{
				if(m_Fire)
					CreateAttack();

				if(isStuck && m_Unstuck)
					GetUnStuck();

				if(GameManager.Instance().DebugMode)
				{
					if(Input.GetKeyDown(KeyCode.Keypad1))
						ChangeTurret(typeof(BaseTurret));
					if(Input.GetKeyDown(KeyCode.Backslash))
						HandleDamage(new HitBox(this, 100, 0, 0));
				}
			}

			base.Update(dt);
		}
	}
}
