using UnityEngine;
using System.Collections;

public class NetHoverBody : HoverBody {

	public NetHoverBody(MovingEntity ME) : base(ME)
	{
	}

	public override void Initialize()
	{
		SetGameObject((GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/Bodies/HoverBody", typeof(GameObject))));
		GetGameObject().transform.position = m_MovingEntity.GetGameObject().transform.position;
		GetGameObject().transform.rotation = m_MovingEntity.GetGameObject().transform.rotation;
		GetGameObject().GetComponent<StabilizerScript>().enabled = false;

		GetGameObject().transform.parent = m_MovingEntity.GetGameObject().transform;
		GetGameObject().rigidbody.useGravity = false;

		TankBasicBodyController tbc = (TankBasicBodyController)GetGameObject().GetComponent(typeof(TankBasicBodyController));
		tbc.bb = this;
	}

	public override void FixedUpdate()
	{
		if(HasGameObject() && HasTurretConnector()
		   && !m_MovingEntity.GetTankParts()[(int)MovingEntity.TankParts.body])
		{
			m_MovingEntity.SetTankPartBuilt(MovingEntity.TankParts.body);
			SetTargetGameObject();
		}
	}
}
