using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

public abstract class BaseGameEntity
{
	protected long m_ID = -1;
	protected GameObject m_GameObject;
	protected EntityTypes.entity_type m_EntityType = EntityTypes.entity_type.ET_Unknown;
	protected Collider m_Collider;
	protected int m_Team;
	private bool m_Stunned = false;
	private float m_StunnedTimer = 0;
	protected float m_Poise = 5f;
	protected float m_Health = 100f;
	protected float m_FireTimer = Time.time;
	private bool m_IsDead = false;
	public bool m_Fire = false;
	private bool m_IsBuilt = false;
	private bool m_IsBuilding = false;
	private GameObject m_TargetGameObject;

	public List<Attack> m_Attacks = new List<Attack>();
	public List<GameObject> m_Targets = new List<GameObject>();

	private Turret m_Turret;

	public BaseGameEntity (GameObject go)
	{
		m_GameObject = go;

		m_ID = EntityManager.Instance().RegisterEntity (this);
		GameManager.Instance().RegisterParticipant(this);
	}

	~BaseGameEntity ()
	{
		EntityManager.Instance().RemoveEntity (m_ID);
	}

	public float GetHealth()
	{ 
		if(m_Health >= 0)
			return m_Health; 
		else
			return 0f;
	}

	public GameObject GetTargetGameObject() //The main body of the entity that is used for targeting and position
	{
		if(m_TargetGameObject != null)
			return m_TargetGameObject;
		else
			return m_GameObject;
	}
	public void SetTargetGameObject(GameObject go)
	{
		m_TargetGameObject = go;
		GameManager.Instance().GetParticipantsByTargeterObject().Add(go, this);
	}

	public void SetPose (Vector3 new_pos, Quaternion new_rot)
	{
		GetTargetGameObject().transform.position = new_pos;
		GetTargetGameObject().transform.rotation = new_rot;
	}

	public Vector3 GetPosition ()
	{
		return GetTargetGameObject().transform.position;
	}

	public void SetPosition (Vector3 new_pos)
	{ 
		GetTargetGameObject().transform.position = new_pos;
	}

	public Quaternion GetRotation ()
	{
		return GetTargetGameObject().transform.rotation;
	}

	public void SetRotation (Quaternion new_rot)
	{
		GetTargetGameObject().transform.rotation = new_rot;
	}

	public void SetGameObject (GameObject gameObject)
	{
		m_GameObject = gameObject;

		if(GameManager.Instance().GetParticipants().ContainsValue(this))
		{
			foreach(KeyValuePair<GameObject, BaseGameEntity> entities in GameManager.Instance().GetParticipants())
			{
				if(entities.Value == this)
				{
					GameManager.Instance().GetParticipants().Remove(entities.Key);
				}
			}

			if(!GameManager.Instance().GetParticipants().ContainsValue(this))
				GameManager.Instance().GetParticipants().Add (gameObject, this);
		}
	}

	public void CreateAttack()
	{
		System.Type type = GetTurret().GetDefaultAttack();

		if(m_FireTimer < Time.time)
		{
			Attack att = (Attack)Activator.CreateInstance(type, this);
			m_Attacks.Add(att);
			m_FireTimer = Time.time + att.GetFireDowntime();
		}
	}

	public void CreateAttack(System.Type type)
	{
		if(m_FireTimer < Time.time)
		{
			Attack att = (Attack)Activator.CreateInstance(type, this);
			m_Attacks.Add(att);
			m_FireTimer = Time.time + att.GetFireDowntime();
		}
	}

	public void ChangeTurret(System.Type TurretType)
	{
		MonoBehaviour.Destroy(GetTurret().GetGameObject());
		Turret bt = GetTurret();
		bt = null;
		SetTurret((Turret)Activator.CreateInstance(TurretType, this));
	}

	public void SetTurret(Turret bt) { m_Turret = bt; }

	public Turret GetTurret() { return m_Turret; }

	public List<GameObject> GetCurrentTargets(){ return m_Targets; }

	public bool CreateGameObject (string prefabName)
	{
		m_GameObject = (GameObject)MonoBehaviour.Instantiate(Resources.Load(prefabName), new Vector3(GetPosition().x, GetPosition().y, 0), Quaternion.identity);
	
		return (m_GameObject != null);
	}

	public GameObject GetGameObject ()
	{
		return m_GameObject;
	}

	public virtual Rigidbody GetRigidBody()
	{
		if(m_GameObject.rigidbody != null)
			return m_GameObject.rigidbody;
		else
			return null;
	}

	public virtual Vector3 GetVelocity()
	{
		return Vector3.zero;
	}

	public void HandlePowerUp(string powerup)
	{
		if(m_EntityType == EntityTypes.entity_type.ET_Playable)
		{
			if(powerup == "Turret-BaseTurret")
				ChangeTurret(typeof(BaseTurret));
			if(powerup == "Turret-MissileTurret")
				ChangeTurret(typeof(MissileTurret));
		}
	}

	public void HandleDamage(HitBox hb)
	{
		if(m_EntityType == EntityTypes.entity_type.ET_Network)
		{
			Debug.Log("Sending Damage:" + GetGameObject().GetPhotonView().viewID);
			GetGameObject().GetPhotonView().RPC("SendDamage", PhotonTargets.All,
			                                    GetGameObject().GetPhotonView().viewID,
			                                    hb.m_Damage,
			                                    hb.m_HitLag,
			                                    hb.m_KnockBack);
		}
		else
		{
			m_Health -= hb.m_Damage;
			
			if (hb.m_KnockBack > m_Poise)
			{
				//stun in place (Not yet used but support in place)
				SetStunned(2);
			}

			if(m_Health <= 0)
			{
				OnDeath(); //Entity is dead
			}

			if(m_EntityType == EntityTypes.entity_type.ET_Playable)
			{

			}
		}
	}

	public virtual void OnDeath()
	{
		m_IsDead = true;

		if(GameManager.Instance().IsOnline && m_GameObject && GetGameObject().GetPhotonView().isMine)
			GetGameObject().GetPhotonView().RPC("Death", PhotonTargets.Others);

	}

	public bool GetIsDead(){ return m_IsDead; }

	public void SetStunned(float length)
	{
		m_StunnedTimer = Time.time + length;
	}
	
	public bool IsStunned()
	{
		if(Time.time > m_StunnedTimer)
			m_Stunned = false;
		else
			m_Stunned = true;
		
		return m_Stunned;
	}

	public void SetIsBuilt(bool built) { m_IsBuilt = built;}
	public bool GetIsBuilt() { return m_IsBuilt; }

	public bool IsBuilding
	{
		get { return m_IsBuilding; }
		set { m_IsBuilding = value; }
	}

	private int m_Spawns = 0;
	private bool m_isSpawned = false;
	public void NetworkSpawned()
	{
		m_Spawns++;
	}
	public bool GetIsNetworkSpawned()
	{
		if(m_Spawns >= PhotonNetwork.otherPlayers.Length)
			m_isSpawned = true;
		
		return m_isSpawned;
	}

	public void SetTeam(int team){ m_Team = team; }
	public int GetTeam(){ return m_Team; }

	public Collider GetCollider(){ return m_Collider; }
	public void SetCollider(Collider col)
	{
		if(m_Collider != null && GameManager.Instance().GetParticipantByCollider().ContainsKey(m_Collider))
		{
			GameManager.Instance().GetParticipantByCollider().Remove(m_Collider);
		}
		m_Collider = col;

		GameManager.Instance().RegisterCollider(this, col);
	}

	public EntityTypes.entity_type GetEntityType() { return m_EntityType; }
	public void SetEntityType(EntityTypes.entity_type new_type) { m_EntityType = new_type; }

	public abstract void Update(float dt);
	public abstract void Start();
	public abstract void FixedUpdate();
	public long GetID() {return m_ID;}
	public void SetID(long ID) {m_ID = ID;}
}
