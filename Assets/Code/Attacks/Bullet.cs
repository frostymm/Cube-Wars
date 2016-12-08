using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : Attack 
{ 
	float m_BulletTimer = 0f;
	Dictionary<GameObject, HitBox> m_Bullets = new Dictionary<GameObject, HitBox>();
	float m_BulletLifetime = 3f;



	public Bullet(BaseGameEntity ME) : base(ME)
	{
		m_Damage = 10;
		m_ProjectilePath = "Prefabs/Ammunitions/Bullet";
		m_TimeBetweenAttacks = 0f;
		m_TimeBetweenFire = 0.5f;
		m_NumberOfAttacks = 1;
		m_BulletSpeed = 150f;
	}

	public void FireBullet()
	{
		if(m_FiringLocationIndex == m_Owner.GetTurret().m_FiringPoints.Length)
			m_FiringLocationIndex = 0;

		Transform firingPoint = m_Owner.GetTurret().m_FiringPoints[m_FiringLocationIndex++];
		GameObject bullet = GameManager.Instance().Instantiate(m_ProjectilePath, firingPoint.position, firingPoint.rotation);
		GameManager.Instance().Instantiate("Prefabs/ShotParticle", firingPoint.position, firingPoint.rotation);

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
			
			GameManager.Instance().Instantiate("Prefabs/Explosion", db.transform.position, Quaternion.identity);

			PhotonNetwork.Destroy(db);
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
