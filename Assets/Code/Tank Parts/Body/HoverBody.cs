using UnityEngine;
using System.Collections;

public class HoverBody : Body {

	private Vector3 m_CenterOfMass = new Vector3(0.1f, -0.2f, -0.4f);

	public HoverBody(MovingEntity ME)
	{
		m_MovingEntity = ME;
		Initialize();
	}

	public void SetTargetGameObject()
	{
		m_MovingEntity.SetTargetGameObject(turretConnector.gameObject);
	}

	public bool IsTouchingGround()
	{
		if(GetLeftWheels() == null || GetRightWheels() == null)
			return false;

		WheelHit temp;
		foreach(WheelCollider w in GetLeftWheels())
			if(w.GetGroundHit(out temp))
				return true;
		
		foreach(WheelCollider w in GetRightWheels())
			if(w.GetGroundHit(out temp))
				return true;
		
		return false;
	}

	private float VelocityLimit = 200f;
	public bool IsSpeeding()
	{
		bool overLimit = false;
		
		if(m_MovingEntity.GetVelocity().magnitude > VelocityLimit)
			overLimit = true;
		
		return overLimit;
	}

	private float Speed = 700f;
	private float Torque = 100f;
	public void UpdateForce(float accel,float steer)
	{
		GetGameObject().rigidbody.AddForce(GetGameObject().transform.forward * (Speed * accel));

		GetGameObject().rigidbody.AddTorque(GetGameObject().transform.up * (Torque * steer));


	}

	public override void Awake()
	{
	}

	public override void Initialize()
	{
		SetGameObject((GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/Bodies/HoverBody", typeof(GameObject))));
		GetGameObject().transform.position = m_MovingEntity.GetGameObject().transform.position;
		GetGameObject().transform.rotation = m_MovingEntity.GetGameObject().transform.rotation;
		GetGameObject().transform.parent = m_MovingEntity.GetGameObject().transform;
		TankBasicBodyController tbc = (TankBasicBodyController)GetGameObject().GetComponent(typeof(TankBasicBodyController));
		tbc.bb = this;

		SetJumpForce(3000);
	}

	public override void FixedUpdate()
	{
		if(GetGameObject() != null && !m_MovingEntity.GetTankParts()[(int)MovingEntity.TankParts.body])
		{
			m_MovingEntity.SetTankPartBuilt(MovingEntity.TankParts.body);
		}

		if(GameManager.Instance().GetGameStarted())
		{
			if(!IsSpeeding() && IsTouchingGround())
				UpdateForce(m_MovingEntity.GetAcceleration(),m_MovingEntity.GetSteering());
			else
				UpdateForce(0,m_MovingEntity.GetSteering());

			if(m_MovingEntity.GetTargetGameObject() != turretConnector.gameObject)
				SetTargetGameObject();
			
			if(IsTouchingGround() && m_MovingEntity.m_Jump)
				GetGameObject().rigidbody.AddForce(GetGameObject().transform.up * GetJumpForce());

			if(GetGameObject().rigidbody.centerOfMass != m_CenterOfMass)
				GetGameObject().rigidbody.centerOfMass = m_CenterOfMass;
		}
	}
}
