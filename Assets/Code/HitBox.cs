using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

public class HitBox
{
	public float m_Damage = 0;
	public float m_HitLag = 0;
	public float m_KnockBack = 0;
	public float m_LifeTime = -1;

	public float m_Speed = 0;
	public Vector3 m_Trajectory = Vector3.zero;
	
	public Vector3 m_Position = new Vector3(0, 0, 0);
	public float m_Size = 1;
	
	public BaseGameEntity m_Owner;
	public GameObject m_GameObject;
	
	public GameObject m_AttachedObject;
	public bool m_Attached = false;

	public bool m_HasCollided = false;
	public bool m_DestroyOnCollide = false;
	public bool m_Destroyed = false;

	public bool m_Virtual = false;

	public bool m_SelfDestructive = false;
	
	public delegate void SelfDestroyer(object sender, System.EventArgs ea);
	public event SelfDestroyer DestroyMe;
	public void OnDestroyMe()
	{
		m_Destroyed = true;
		SelfDestroyer temp = DestroyMe;
		if (temp != null)
		{
			temp(this, new System.EventArgs());
		}
	}

	public bool IsDestroyed(){return m_Destroyed;}

	public void SetTrajectorySpeed(float speed)
	{
		m_Speed = speed;
	}
	
	public void SetFollowObject(GameObject go)
	{
		m_AttachedObject = go;
		m_Attached = true;
	}
	
	public void SetPosition(Vector3 pos)
	{
		m_Position = pos;
		m_GameObject.transform.position = pos;
	}

	public HitBox(BaseGameEntity owner, float damage, float knockback, float lifetime, float size, bool DestroyOnCollide, GameObject attachedObject)
	{
		m_Owner = owner;
		m_Damage = damage;
		m_KnockBack = knockback;
		m_LifeTime = lifetime;
		m_DestroyOnCollide = DestroyOnCollide;
		m_Size = size;
		
		m_GameObject = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/HitBox"), m_Position, Quaternion.identity);

		m_AttachedObject = attachedObject;
		m_Attached = true;
		
		HitBoxScript hbScript = (HitBoxScript)m_GameObject.GetComponent(typeof(HitBoxScript));
		hbScript.OwnerHB = this;
	}

	public HitBox(Vector3 position, float size, BaseGameEntity owner,
	              float damage, float hitlag, float knockback, float lifetime)
	{
		m_Position = position;
		m_Size = size;
		m_Owner = owner;
		m_Damage = damage;
		m_HitLag = hitlag;
		m_KnockBack = knockback;
		m_LifeTime = lifetime;
		
		m_GameObject = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/HitBox"), m_Position, Quaternion.identity);
		
		HitBoxScript hbScript = (HitBoxScript)m_GameObject.GetComponent(typeof(HitBoxScript));
		hbScript.OwnerHB = this;
	}

	//Constructor for virtual hitbox (Sent by means other than hitbox contact)
	public HitBox(BaseGameEntity owner, float damage, float hitlag, float knockback)
	{
		m_Owner = owner;
		m_Damage = damage;
		m_HitLag = hitlag;
		m_KnockBack = knockback;
		m_Virtual = true;
	}

	//Constructor for network linked hitbox
	public HitBox(int photonID, float damage, float hitlag, float knockback)
	{
		m_Owner = GameManager.Instance().GetEntityByPhotonID(photonID);
		m_Damage = damage;
		m_HitLag = hitlag;
		m_KnockBack = knockback;
		m_Virtual = true;
	}
	
	~HitBox()
	{
	}

	//Called When hitbox is triggered
	public void OnTriggerEnter(Collider coll)
	{
		if(coll != m_Owner.GetCollider() || m_SelfDestructive)
		{
			if(GameManager.Instance().GetParticipantByCollider().ContainsKey(coll))
			{
				GameManager.Instance().GetParticipantByCollider()[coll].HandleDamage(this);
				m_HasCollided = true;
			}
			
			if(coll.gameObject.tag == "Ground")
			{
				m_HasCollided = true;
			}
		}
	}
	
	public void Start()
	{
		if(!m_Virtual)
		{
			m_GameObject.transform.localScale = new Vector3(m_Size, m_Size, m_Size);
			if(!GameManager.Instance().DebugMode)
				m_GameObject.renderer.enabled = false;
		}
	}
	
	public void Update(float dt)
	{	
		if(!m_Virtual)
		{
			if(m_LifeTime != -1)
			{
				if(m_LifeTime < 0)
				{
					MonoBehaviour.Destroy(m_GameObject);
					OnDestroyMe();
				}
				
				m_LifeTime -= dt;
			}
			
			if(m_Attached)
			{
				if(m_AttachedObject != null)
				{
					RaycastHit rayHit = new RaycastHit();
					Vector3 reverseVector = Vector3.Cross(m_AttachedObject.transform.right, m_AttachedObject.transform.up);
					if(Physics.Linecast(m_GameObject.transform.position, m_GameObject.transform.position + reverseVector * (m_Speed * Time.deltaTime), out rayHit))
					{
						if(rayHit.collider != m_Owner.GetCollider())
						{
							m_AttachedObject.transform.position = rayHit.point;
							SetPosition(rayHit.point);
						}
					}
					else
						SetPosition(m_AttachedObject.transform.position);
				}
			}

			if(m_DestroyOnCollide)
			{
				if(m_HasCollided)
				{
					MonoBehaviour.Destroy(m_GameObject);
					OnDestroyMe();
				}
			}
		}
	}
}
