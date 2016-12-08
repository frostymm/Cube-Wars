using UnityEngine;
using System.Collections;

public class NetLaserTurret : BaseTurret {

	private int m_MaximumTargets = 1;
	private string m_TurretLoad = "Prefabs/Turrets/LaserTurret";
	private Vector3 m_TurretOffset = new Vector3(0f,0.2f,-0.2f);
	
	public NetLaserTurret(MovingEntity ME) : base(ME)
	{
		SetTurretType(TurretType.net);
	}
	
	public override System.Type GetDefaultAttack()
	{
		return typeof(Missile);
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
