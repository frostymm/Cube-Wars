﻿using UnityEngine;
using System.Collections;

public class NetMissileTurret : BaseTurret 
{
	private int m_MaximumTargets = 3;
	private string m_TurretLoad = "Prefabs/Turrets/MissileTurret";
	private Vector3 m_TurretOffset = new Vector3(0f,0.2f,-0.4f);
	
	public NetMissileTurret(MovingEntity ME) : base(ME)
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