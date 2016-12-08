using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Targeter{

	public GameObject m_GameObject;
	public GameObject m_Target;
	private PlayerEntity m_PlayerEntity;
	TargetIndicator m_TargetIndicator;
	public bool active = true;

	public Targeter(GameObject target, PlayerEntity PE)
	{
		m_GameObject = (GameObject)MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/TargetIndicator"));
		m_TargetIndicator = m_GameObject.GetComponent<TargetIndicator>();
		m_TargetIndicator.targeter = this;
		m_Target = target;
		m_PlayerEntity = PE;
	}

	~Targeter()
	{
	}

	public delegate void SelfDestroyer(object sender, System.EventArgs ea);
	public event SelfDestroyer DestroyMe;
	public void OnDestroyMe()
	{
		SelfDestroyer temp = DestroyMe;
		if (temp != null)
		{
			temp(this, new System.EventArgs());
		}
	}

	// Update is called once per frame
	public void Update () {
		if(m_Target != null)
		{
			m_GameObject.transform.position = m_PlayerEntity.GetCam().WorldToViewportPoint(m_Target.transform.position);// + new Vector3(m_Target.transform.localScale.x * 2,0,0));
		}
		else
		{
			active = false;
		}

		if(!active)
		{
			MonoBehaviour.Destroy(m_GameObject);
			OnDestroyMe();
		}
	}
}
