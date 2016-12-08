using UnityEngine;
using System.Collections;

public class LaserTurret : BaseTurret 
{
	private int m_MaximumTargets = 1;
	private string m_TurretLoad = "Prefabs/Turrets/LaserTurret";
	private Vector3 m_TurretOffset = new Vector3(0f,-0.9f,-0.366f);
	private Vector3 m_TargetOffset = new Vector3(0f, -1.5f, 0f);
	private Vector3 m_TurretTargetingDisplacement = new Vector3(0, 4f, 0);
	
	public LaserTurret(MovingEntity ME) : base(ME)
	{
		ME.SetTargetingDistance(ME.GetTargetingDistance() * 1.5f);
		m_BulletSpeed = 300f;
	}
	
	public override System.Type GetDefaultAttack()
	{
		return typeof(LaserShot);
	}
	
	public override void Initialize ()
	{
		SetMaxTargets(m_MaximumTargets);
		SetTurretToLoad(m_TurretLoad);
		SetTurretOffset(m_TurretOffset);
		base.Initialize ();
	}
	
	public override void FixedUpdate ()
	{
		base.FixedUpdate ();
	}
}
