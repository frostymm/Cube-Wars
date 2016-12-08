using UnityEngine;
using System.Collections;

public class NetBaseTurret : BaseTurret {

	private int m_MaximumTargets = 1;
	private string m_TurretLoad = "Prefabs/Turrets/BaseTurret";
	private Vector3 m_TurretOffset = new Vector3(0,0,-0.5f);
	
	public NetBaseTurret(MovingEntity ME) : base(ME)
	{
		SetTurretType(TurretType.net);
	}
	
	public override System.Type GetDefaultAttack()
	{
		return typeof(Bullet);
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
