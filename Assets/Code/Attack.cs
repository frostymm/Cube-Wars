using UnityEngine;
using System.Collections;

public abstract class Attack : MonoBehaviour{

	public float m_Damage = 0;
	public float m_TimeBetweenAttacks = 0;
	public float m_TimeBetweenFire = 1;
	public int m_NumberOfAttacks = 1;
	public int m_FiringLocationIndex = 0;
	public int m_TargetIndex = 0;
	public BaseGameEntity m_Owner;
	public string m_ProjectilePath;
	public bool m_IsDone = false;//Is Attack done generating hits
	public bool m_IsDead = false;//Is Attack done updating hits
	public string m_CollisionEffectPath;
	
	public Attack(BaseGameEntity ME)
	{
		m_Owner = ME;
	}

	public float m_BulletSpeed = 10f;
	public float GetBulletSpeed(){ return m_BulletSpeed; }

	public float GetFireDowntime(){ return m_TimeBetweenFire; }
	public bool GetIsDead(){ return m_IsDead; }

	public abstract void Update();
}
