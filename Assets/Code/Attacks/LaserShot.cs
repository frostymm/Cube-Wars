using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserShot : Attack 
{
	float m_BulletTimer = 0f;
	Dictionary<GameObject, HitBox> m_Bullets = new Dictionary<GameObject, HitBox>();
	float m_BulletLifetime = 3f;
	
	public LaserShot(BaseGameEntity ME) : base(ME)
	{
		m_Damage = 30;
		m_ProjectilePath = "Prefabs/Ammunitions/LaserShot";
		m_TimeBetweenAttacks = 0f;
		m_TimeBetweenFire = 1.5f;
		m_NumberOfAttacks = 1;
		m_BulletSpeed = 300f;
	}
	
	public void FireBullet()
	{
		if(m_FiringLocationIndex == m_Owner.GetTurret().m_FiringPoints.Length)
			m_FiringLocationIndex = 0;
		
		Transform firingPoint = m_Owner.GetTurret().m_FiringPoints[m_FiringLocationIndex++];
		GameObject bullet = GameManager.Instance().Instantiate(m_ProjectilePath, firingPoint.position, firingPoint.rotation);
		GameObject sp = GameManager.Instance().Instantiate("Prefabs/ShotParticle", firingPoint.position, firingPoint.rotation);
		sp.GetComponent<ParticleSystem>().startColor  = Color.cyan;

		HitBox hb = new HitBox(m_Owner, m_Damage, 1f, m_BulletLifetime, 1, true, bullet);
		hb.SetTrajectorySpeed(m_BulletSpeed);
		
		m_Bullets.Add(bullet, hb);
	}
	
	public void UpdateBullets()
	{
		List<GameObject> deadBullets = new List<GameObject>();
		
		foreach(KeyValuePair<GameObject, HitBox> bullet in m_Bullets)
		{
			if(bullet.Key)
				bullet.Key.transform.Translate(Vector3.forward * (m_BulletSpeed * Time.deltaTime));
			
			if(bullet.Value.IsDestroyed())
			{
				deadBullets.Add(bullet.Key);
			}
		}
		
		foreach(GameObject db in deadBullets)
		{
			m_Bullets.Remove(db);
			
			MeshRenderer mr = (MeshRenderer)db.GetComponent(typeof(MeshRenderer));
			mr.enabled = false;
			
			GameObject ps = GameManager.Instance().Instantiate("Prefabs/LaserExplosion", db.transform.position, Quaternion.identity);
			ps.GetComponent<ParticleSystem>().startColor  = Color.cyan;
		}
	}
	
	public override void Update()
	{
		if(!m_IsDone && m_BulletTimer < Time.time && m_Bullets.Count < m_NumberOfAttacks)
		{
			FireBullet();
			m_BulletTimer = Time.time + m_TimeBetweenAttacks;
		}
		
		if(m_Bullets.Count == m_NumberOfAttacks)
			m_IsDone = true;
		
		if(m_IsDone && m_Bullets.Count == 0)
			m_IsDead = true;
		
		
		UpdateBullets();
	}
}

