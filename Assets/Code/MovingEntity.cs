using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MovingEntity : BaseGameEntity 
{
	protected Dictionary<string, SteeringBehavior> m_Behaviors;
	protected Vector3 m_CurrentForce = new Vector3();

	protected BoxCollider m_CollisionBox;
	protected float m_Speed;
	protected Collision m_Collision;
	protected float m_TargetingDistance = 120;
	public bool m_ManualFire = false;
	public bool m_Jump = false;
	public bool m_Unstuck = false;

	float m_MaxSpeed = 1;
	float m_MaxForce = 10;

	private float m_Steer;
	private float m_Accelerate;
	
	private Body m_Body;
	private Camera m_Camera;

	public MovingEntity(GameObject go) : base(go)
	{
		m_Behaviors = new Dictionary<string, SteeringBehavior>();
	}

	~MovingEntity()
	{
	}

	public bool m_Stuck = false;
	public bool isStuck
	{
		get{ return m_Stuck; }
		set{ m_Stuck = value; }
	}

	private bool m_StuckTimerSet = false;
	private float m_StuckTimer = 0f;
	public void CheckIsStuck()
	{
		if(GetVelocity().magnitude < 3)
		{
			if(!m_StuckTimerSet)
			{
				m_StuckTimer = Time.time + 5f;
				m_StuckTimerSet = true;
			}
			else
			{
				if(Time.time > m_StuckTimer)
					isStuck = true;
			}
		}
		else
		{
			m_StuckTimerSet = false;
			isStuck = false;
		}
	}

	public void GetUnStuck()
	{
		GetRigidBody().AddForce(Vector3.up * (GetBody().GetJumpForce() * 2f));
	}

	private string m_ChosenBody, m_ChosenTurret;
	public string GetChosenBody() { return m_ChosenBody; }
	public void SetChosenBody(string body) { m_ChosenBody = body; }
	public string GetChosenTurret() { return m_ChosenTurret; }
	public void SetChosenTurret(string turret) { m_ChosenTurret = turret; }

	private bool[] m_TankParts = new bool[(int)TankParts.count];
	public enum TankParts
	{
		body,
		turret,
		count
	}

	public bool[] GetTankParts()
	{
		return m_TankParts;
	}

	public void SetTankPartBuilt(TankParts tp)
	{
		GetTankParts()[(int)tp] = true;
		IsBuilding = false;
	}

	public Body BuildNewBody(string part)
	{
		if(part == "BaseBody")
			return new BaseBody(this);
		if(part == "HoverBody")
			return new HoverBody(this);

		if(part == "NetBaseBody")
			return new NetBaseBody(this);
		if(part == "NetHoverBody")
			return new NetHoverBody(this);
		
		return null;
	}
	
	public Turret BuildNewTurret(string part)
	{
		if(part == "BaseTurret")
			return new BaseTurret(this);
		if(part == "MissileTurret")
			return new MissileTurret(this);
		if(part == "LaserTurret")
			return new LaserTurret(this);

		if(part == "NetBaseTurret")
			return new NetBaseTurret(this);
		if(part == "NetMissileTurret")
			return new NetMissileTurret(this);
		if(part == "NetLaserTurret")
			return new NetLaserTurret(this);
		
		return null;
	}

	public SteeringBehavior GetBehavior(string name)
	{
		SteeringBehavior result = null;
		
		if (m_Behaviors.ContainsKey(name))
			result = m_Behaviors[name];
		
		return result;
	}

	public void AddBehavior(SteeringBehavior behavior){ m_Behaviors.Add(behavior.GetName(), behavior); }

	public float getSpeed() {return m_Speed;}
	public void setSpeed( float s ) { m_Speed = s; }

	public float GetMaxSpeed() { return m_MaxSpeed; }
	public void SetMaxSpeed(float new_speed) { m_MaxSpeed = new_speed; }

	public float GetMaxForce() { return m_MaxForce; }
	public void SetMaxForce(float mf) { m_MaxForce = mf; }
	
	public void SetBody(Body bb) { m_Body = bb; }

	public Body GetBody() { return m_Body; }

	public Camera GetCam() 
	{ 
		return m_Camera; 
	}
	public void SetCam(Camera cam) { m_Camera = cam; }

	public override Rigidbody GetRigidBody ()
	{
		return m_Body.GetGameObject().rigidbody;
	}

	public Vector3 GetRigidBodyVelocity() { return m_Body.GetGameObject().rigidbody.velocity; }
	public void SetRigidBodyVelocity(Vector3 v) { m_Body.GetGameObject().rigidbody.velocity = v; }

	Vector3 m_Velocity = Vector3.zero;
	public override Vector3 GetVelocity() 
	{
		if(GetEntityType() == EntityTypes.entity_type.ET_Network)
			return m_Velocity;
		else
			return GetRigidBodyVelocity();
	}
	public void SetVelocity(Vector3 v) { m_Velocity = v; }
	
	public Collision GetCollision(){return m_Collision;}
	public void SetCollision(Collision collision){ m_Collision = collision; }

	public void SetTargetingDistance(float tdist){ m_TargetingDistance = tdist; }
	public float GetTargetingDistance(){ return m_TargetingDistance; }

	public void SetMovement(float acc, float steer)
	{
		m_Accelerate = acc;
		m_Steer = steer;
	}

	public float GetAcceleration() 
	{ 
		if(!GameManager.Instance().IsPaused && !GameManager.Instance().IsGameOver)
			return m_Accelerate;
		else
			return 0f;
	}
	public float GetSteering() 
	{
		if(!GameManager.Instance().IsPaused && !GameManager.Instance().IsGameOver)
			return m_Steer;
		else
			return 0f;
	}

	public Vector3 bodyPos;
	public Quaternion bodyRot, barrelRot, turretRot;
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(GameManager.Instance().GetGameStarted())
		{
			if (stream.isWriting)
			{
				// We own this player: send the others our data
				stream.SendNext(GetBody().GetGameObject().transform.position);
				stream.SendNext(GetBody().GetGameObject().transform.rotation);
				stream.SendNext(GetTurret().barrel.transform.rotation);
				stream.SendNext(GetTurret().turret.transform.rotation);
				stream.SendNext(GetBody().GetGameObject().rigidbody.velocity);
			}
			else
			{
				// Network player, receive data
				bodyPos = (Vector3)stream.ReceiveNext();
				bodyRot = (Quaternion)stream.ReceiveNext();
				barrelRot = (Quaternion)stream.ReceiveNext();
				turretRot = (Quaternion)stream.ReceiveNext();
				SetVelocity((Vector3)stream.ReceiveNext());
			}
		}
	}

	public override void OnDeath()
	{
		base.OnDeath();
	}

	float deathExplosionTimer = 0;
	int deathExplosions = 0;
	bool deathSmoke = false;
	public void DeathExplosion()
	{
		int numberOfExplosions = 6;
		float deathExplosionWaitMin = 0.3f;
		float deathExplosionWaitMax = 1.2f;

		if(!deathSmoke)
		{
			GameObject go = GameManager.Instance().Instantiate("Prefabs/Smoke", GetPosition(), Quaternion.identity);
			deathSmoke = true;

			FollowScript fs = go.GetComponent<FollowScript>();
			fs.ObjectToFollow = GetTargetGameObject();
		}

		if(Time.time > deathExplosionTimer && deathExplosions < numberOfExplosions)
		{
			Vector3 pos = new Vector3(UnityEngine.Random.Range(GetPosition().x - 5.0f, GetPosition().x + 5.0f),
			                          UnityEngine.Random.Range(GetPosition().y - 5.0f, GetPosition().y + 5.0f),
			                          UnityEngine.Random.Range(GetPosition().z - 5.0f, GetPosition().z + 5.0f));

			GameManager.Instance().Instantiate("Prefabs/Explosion", pos, Quaternion.identity);

			deathExplosions++;
			deathExplosionTimer = Time.time + UnityEngine.Random.Range(deathExplosionWaitMin, deathExplosionWaitMax);
		}
	}
	
	public void Awake()
	{
		if(m_Body != null)
			m_Body.Awake();
	}
	
	public override void FixedUpdate()
	{
		if(!IsStunned())
		{
			if(m_Body != null)
				GetBody().FixedUpdate();
			if(GetTurret() != null)
				GetTurret().FixedUpdate();
		}

		if(GameManager.Instance().GetGameStarted())
		{
			CheckIsStuck();

			if(GetPosition().y < 250f)
				OnDeath();
		}
	}
	
	public override void Start()
	{
	}

	public override void Update(float dt)
	{
		for(int i = m_Attacks.Count - 1; i >= 0; i--)
		{
			m_Attacks[i].Update();
			if(m_Attacks[i].GetIsDead())
			{
				m_Attacks.RemoveAt(i);
			}
		}

		if(GetIsDead())
		{
			DeathExplosion();
		}
	}
}
