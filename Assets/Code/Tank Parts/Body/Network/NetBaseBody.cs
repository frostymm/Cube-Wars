using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class NetBaseBody : BaseBody
{
	public NetBaseBody(MovingEntity ME) : base(ME)
	{
	}

	public override void Initialize()
	{
		SetGameObject((GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/Bodies/BaseBody", typeof(GameObject)), 
		                                                    m_MovingEntity.GetGameObject().transform.position,
		                                                    m_MovingEntity.GetGameObject().transform.rotation));

		GetGameObject().GetComponent<StabilizerScript>().enabled = false;
		GetGameObject().rigidbody.useGravity = false;

		GetGameObject().transform.parent = m_MovingEntity.GetGameObject().transform;
		TankBodyController tbc = (TankBodyController)GetGameObject().GetComponent(typeof(TankBodyController));
		tbc.bb = this;
	}
	
	public override void Awake()
	{
		
	}
	
	public override void FixedUpdate()
	{
		if(HasGameObject() && turretConnector.gameObject != null 
		   && !m_MovingEntity.GetTankParts()[(int)MovingEntity.TankParts.body])
		{
			m_MovingEntity.SetTankPartBuilt(MovingEntity.TankParts.body);
			SetTargetGameObject();
		}
	}
	
}
