using UnityEngine;
using System.Collections;

public class NPCEntity : MovingEntity {

	//an instance of the state machine class
	StateMachine<NPCEntity> m_StateMachine;
	
	public NPCEntity(GameObject go) : base(go)
	{
		m_EntityType = EntityTypes.entity_type.ET_NPC;
		m_StateMachine = new StateMachine<NPCEntity> (this);
		m_StateMachine.SetCurrentState (NPCOwnedStates.Neutral.Instance());
		m_StateMachine.SetGlobalState (NPCOwnedStates.OnGround.Instance());
		m_Speed = 3;
	}
	
	~NPCEntity()
	{
		m_StateMachine = null;
	}
	
	public StateMachine<NPCEntity> GetFSM()
	{
		return m_StateMachine;
	}
	
	public override void Start()
	{
		base.Start ();
	}
	
	public override void Update(float dt)
	{
		m_StateMachine.Update ();
		base.Update (dt);
	}
}
