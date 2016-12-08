using UnityEngine;
using System.Collections;

public class TargetEntity : BaseGameEntity {

	public TargetEntity(GameObject go) : base(go)
	{
		m_EntityType = EntityTypes.entity_type.ET_NPC;
		m_Health = 10;
	}
	
	~TargetEntity()
	{
	}

	public override void FixedUpdate ()
	{
		if(m_GameObject && GetIsDead())
			MonoBehaviour.Destroy(m_GameObject);
	}

	public override void Start()
	{
		SetIsBuilt(true);
	}
	
	public override void Update(float dt)
	{
	}
}
