using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseTurret : Turret
{

	public MovingEntity m_MovingEntity;
	private Vector3 m_TurretOffset = new Vector3(0,0.5f,-0.5f);
	private string m_TurretLoad = "Prefabs/Turrets/BaseTurret";

	public BaseTurret(MovingEntity ME)
	{
		m_MovingEntity = ME;
		Initialize();
		SetDefaultAttack(typeof(Bullet));
		m_BulletSpeed = 150f;
	}
	
	public override System.Type GetDefaultAttack()
	{
		return typeof(Bullet);
	}

	public float m_BulletSpeed;
	public float GetBulletSpeed()
	{
		return m_BulletSpeed;
	}

	public void SetTurretToLoad(string turretLoad){ m_TurretLoad = turretLoad; }
	public void SetTurretOffset(Vector3 turretOffset){ m_TurretOffset = turretOffset; }

	private Vector3 m_TargetingOffset = new Vector3(0,0,0);
	public void SetTargetOffset(Vector3 targetOffset){ m_TargetingOffset = targetOffset; }

	//The difference between the turret position and the firing point
	private Vector3 m_TurretTargetingDisplacement = new Vector3(0,0,0);
	public void SetTurretTargetingDisplacement(Vector3 displacement){ m_TurretTargetingDisplacement = displacement; }

	public RaycastHit rayHit = new RaycastHit();
	void updateTurret()
	{
		float speed = 20.0f;
		
		Vector3 targetPosition = new Vector3(0,0,0);
		foreach(GameObject targ in m_MovingEntity.GetCurrentTargets())
			targetPosition += targ.transform.position;
		
		targetPosition /= m_MovingEntity.GetCurrentTargets().Count;
		targetPosition += m_TargetingOffset;
		
		if(m_MovingEntity.GetCurrentTargets().Count == 0 || m_MovingEntity.m_ManualFire) //Use Camera To target if no targets
		{
			targetPosition = m_MovingEntity.GetCam().transform.position + (1000 * m_MovingEntity.GetCam().transform.forward);
			if(Physics.Linecast(m_MovingEntity.GetCam().transform.position, targetPosition, out rayHit))
				targetPosition = rayHit.point;
			Debug.DrawLine(m_MovingEntity.GetCam().transform.position, targetPosition, Color.blue);
		}
		//Calculate trajectory based on velocity of target
		else if(m_MovingEntity.GetCurrentTargets().Count == 1)
		{
			GameObject targetGO = m_MovingEntity.GetCurrentTargets()[0];
			if(GameManager.Instance().GetParticipantsByTargeterObject().ContainsKey(targetGO) || targetGO.rigidbody != null)
			{
				Vector3 Velocity;
				if(GameManager.Instance().GetParticipantsByTargeterObject().ContainsKey(targetGO))
					Velocity = GameManager.Instance().GetParticipantsByTargeterObject()[targetGO].GetVelocity();
				else
					Velocity = targetGO.rigidbody.velocity;

				if(Velocity.magnitude > 0)
				{
					Vector3 IC = Intercept.CalculateInterceptCourse(targetGO.transform.position, Velocity, barrel.position, GetBulletSpeed());

					IC.Normalize();
					float interceptionTime1 = Intercept.FindClosestPointOfApproach(targetGO.transform.position, Velocity, barrel.position, IC * GetBulletSpeed());
					targetPosition = targetGO.transform.position + Velocity*interceptionTime1;

					Debug.DrawLine(barrel.position, targetGO.transform.position, Color.white);
					Debug.DrawLine(barrel.position, targetPosition, Color.yellow);
				}
			}
		}

		//Rotate to face target if a target exists
		float str = Mathf.Min (speed * Time.deltaTime, 1);
		Vector3 targetPositionOnYAxis = (targetPosition - (turret.position + m_TurretTargetingDisplacement));
		Quaternion targetRotation = Quaternion.LookRotation(targetPositionOnYAxis);
		targetRotation = Quaternion.Lerp (turret.rotation, targetRotation, str);
		turret.rotation = targetRotation;
	}

	public override void Initialize()
	{
		SetGameObject((GameObject)MonoBehaviour.Instantiate(Resources.Load(m_TurretLoad, typeof(GameObject))));
		GetGameObject().transform.parent = m_MovingEntity.GetBody().turretConnector;
		GetGameObject().transform.localPosition = m_TurretOffset;
		GetGameObject().transform.rotation = m_MovingEntity.GetBody().GetGameObject().transform.rotation;
		TankTurretController ttc = (TankTurretController)GetGameObject().GetComponent(typeof(TankTurretController));
		ttc.bt = this;

		if(!GameManager.Instance().IsOnline || m_MovingEntity.GetGameObject().GetPhotonView().isMine)
		{
			MouseOrbitCamScript moi = m_MovingEntity.GetCam().GetComponent<MouseOrbitCamScript>();
			moi.target = GetGameObject().transform;
		}

		m_MovingEntity.SetTankPartBuilt(MovingEntity.TankParts.turret);
	}

	public override void FixedUpdate()
	{
		if(!GameManager.Instance().IsOnline || m_MovingEntity.GetGameObject().GetPhotonView().isMine)
		{
			if(!(GetGameObject() != null && turret == null))
			{
				updateTurret();
			}

			if(GetGameObject().transform.parent != m_MovingEntity.GetBody().turretConnector)
			{
				GetGameObject().transform.parent = m_MovingEntity.GetBody().turretConnector;
				GetGameObject().transform.localPosition = m_TurretOffset;
				GetGameObject().transform.rotation = m_MovingEntity.GetBody().GetGameObject().transform.rotation;
			}
		}

	}
}
