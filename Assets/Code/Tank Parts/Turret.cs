using UnityEngine;
using System.Collections;

public abstract class Turret{

	public abstract void Initialize();
	public abstract void FixedUpdate();

	public Transform[] m_FiringPoints;
	public Transform turret;
	public Transform barrel;

	GameObject m_GameObject;
	public GameObject GetGameObject (){ return m_GameObject; }
	public void SetGameObject (GameObject go){ m_GameObject = go; }
	public bool HasGameObject(){ return (m_GameObject); }

	private int m_MaximumTargets = 1;
	public int GetMaxTargets(){ return m_MaximumTargets; }
	public void SetMaxTargets(int maxTargets){ m_MaximumTargets = maxTargets; }

	System.Type m_DefaultAttack;
	public virtual System.Type GetDefaultAttack()
	{
		return m_DefaultAttack;
	}
	public void SetDefaultAttack(System.Type defaultAttack){ m_DefaultAttack = defaultAttack; }

	TurretType m_TurretType = 0;
	public TurretType GetTurretType(){ return m_TurretType; }
	public void SetTurretType(TurretType tt){ m_TurretType = tt; }
	public enum TurretType
	{
		local,
		net
	}
}
