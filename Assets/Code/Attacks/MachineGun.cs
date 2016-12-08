using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MachineGun : Attack 
{
	float m_BulletTimer = 0;
	List<GameObject> m_Bullets = new List<GameObject>();
	float m_BulletMaxDistance = 100f;
	
	public MachineGun(BaseGameEntity ME) : base(ME)
	{
		m_Damage = 2;
		m_ProjectilePath = "Prefabs/Ammunitions/MachineGunBullet";
		m_TimeBetweenAttacks = 0.05f;
		m_TimeBetweenFire = 1f;
		m_NumberOfAttacks = -1;
		m_BulletSpeed = 3f;
	}

	public RaycastHit rayHit = new RaycastHit();
	public void FireBullet()
	{
		if(m_FiringLocationIndex == m_Owner.GetTurret().m_FiringPoints.Length)
			m_FiringLocationIndex = 0;
		
		Transform firingPoint = m_Owner.GetTurret().m_FiringPoints[m_FiringLocationIndex++];
		GameObject bullet = (GameObject)MonoBehaviour.Instantiate(Resources.Load(m_ProjectilePath), firingPoint.position, firingPoint.rotation);
		bullet.GetComponent<TrailRenderer>().lightProbeAnchor = firingPoint;

		Vector3 reverseVector = Vector3.Cross(firingPoint.right, firingPoint.up);

		m_Bullets.Add(bullet);
		
		if(Physics.Linecast(firingPoint.position, firingPoint.position + reverseVector * m_BulletMaxDistance, out rayHit))
		{
			if(GameManager.Instance().GetParticipants().ContainsKey(rayHit.collider.gameObject))
			{
				GameManager.Instance().GetParticipants()[rayHit.collider.gameObject].HandleDamage
					(new HitBox(m_Owner, 1f, 0f, 0f));
			}
		}
	}
	
	public void UpdateBullets()
	{
		List<GameObject> deadBullets = new List<GameObject>();
		
		foreach(GameObject bullet in m_Bullets)
		{
			bullet.transform.Translate(Vector3.forward * m_BulletSpeed);

			if(Vector3.Distance(m_Owner.GetGameObject().transform.position, bullet.transform.position) > rayHit.distance)
				deadBullets.Add(bullet);
		}
		
		foreach(GameObject db in deadBullets)
		{
			m_Bullets.Remove(db);
			MonoBehaviour.DestroyImmediate(db);
		}
	}
	
	public override void Update()
	{
		if(!m_IsDone && m_BulletTimer < Time.time && m_Owner.m_Fire)
		{
			FireBullet();
			m_BulletTimer = Time.time + m_TimeBetweenAttacks;
		}
		
		if(!m_Owner.m_Fire)
			m_IsDone = true;
		
		if(m_IsDone && m_Bullets.Count == 0)
			m_IsDead = true;
		
		
		UpdateBullets();
	}
}

