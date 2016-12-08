using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Missile : Attack 
{
	float m_BulletTimer = 0f;
	Dictionary<GameObject, HitBox> m_Bullets = new Dictionary<GameObject, HitBox>();
	Dictionary<GameObject, GameObject> m_Targets = new Dictionary<GameObject, GameObject>();
	float m_BulletLifetime = 5f;
	public string m_ParticleEffectTrailPath;
	
	public Missile(BaseGameEntity ME) : base(ME)
	{
		m_Damage = 3f;
		m_ProjectilePath = "Prefabs/Ammunitions/Missile";
		m_TimeBetweenAttacks = 0.1f;
		m_TimeBetweenFire = 3f;
		m_NumberOfAttacks = m_Owner.GetTurret().m_FiringPoints.Length;
		m_ParticleEffectTrailPath = "Prefabs/ShotParticle";
		m_CollisionEffectPath = "Prefabs/Explosion";
		m_BulletSpeed = 90f;
	}
	
	public void FireBullet()
	{
		if(m_FiringLocationIndex == m_Owner.GetTurret().m_FiringPoints.Length)
			m_FiringLocationIndex = 0;

		if(m_TargetIndex >= m_Owner.GetCurrentTargets().Count)
			m_TargetIndex = 0;
		
		Transform firingPoint = m_Owner.GetTurret().m_FiringPoints[m_FiringLocationIndex++];
		GameObject bullet = GameManager.Instance().Instantiate(m_ProjectilePath, firingPoint.position, firingPoint.rotation);
		GameManager.Instance().Instantiate(m_ParticleEffectTrailPath, firingPoint.position, firingPoint.rotation);
		
		HitBox hb = new HitBox(m_Owner, m_Damage, 1f, m_BulletLifetime, 1, true, bullet);
		
		m_Bullets.Add(bullet, hb);

		//Set Targets for bullets
		if(m_Owner.GetCurrentTargets().Count != 0 && m_Owner.GetCurrentTargets()[m_TargetIndex] != null)
			m_Targets.Add(bullet, m_Owner.GetCurrentTargets()[m_TargetIndex++]);
	}

	float turnSpeed = 9.0f;
	public void UpdateBullets()
	{
		List<GameObject> deadBullets = new List<GameObject>();
		
		foreach(KeyValuePair<GameObject, HitBox> bullet in m_Bullets)
		{
			bullet.Key.transform.Translate(Vector3.forward * (m_BulletSpeed * Time.deltaTime));

			if(m_Targets.ContainsKey(bullet.Key) && m_Targets[bullet.Key] != null)
			{
				Quaternion targetRotation = Quaternion.LookRotation(m_Targets[bullet.Key].transform.position - bullet.Key.transform.position);
				float str = Mathf.Min (turnSpeed * Time.deltaTime, 1);
				targetRotation = Quaternion.Lerp (bullet.Key.transform.rotation, targetRotation, str);
				
				bullet.Key.transform.rotation = targetRotation;
			}
			
			if(bullet.Value.IsDestroyed() || bullet.Value.m_HasCollided)
			{
				deadBullets.Add(bullet.Key);
			}
		}
		
		foreach(GameObject db in deadBullets)
		{
			m_Bullets.Remove(db);

			MeshRenderer mr = (MeshRenderer)db.GetComponent(typeof(MeshRenderer));
			mr.enabled = false;

			GameManager.Instance().Instantiate(m_CollisionEffectPath, db.transform.position, Quaternion.identity);

			if(GameManager.Instance().IsOnline)
				NetworkManager.Instance().DestroyGameObject(db);
		}
	}
	
	public override void Update()
	{
		if(!GameManager.Instance().IsOnline || m_Owner.GetGameObject().GetPhotonViewsInChildren()[0].isMine) //Update bullet only if single player or client who owns bullet
		{
			if(!m_IsDone && m_BulletTimer < Time.time && m_Bullets.Count < m_NumberOfAttacks)
			{
				FireBullet();
				m_BulletTimer = Time.time + m_TimeBetweenAttacks;
			}
			
			if(m_Bullets.Count >= m_NumberOfAttacks)
				m_IsDone = true;
			
			if(m_IsDone && m_Bullets.Count == 0)
				m_IsDead = true;
			
			
			UpdateBullets();
		}
	}
}
