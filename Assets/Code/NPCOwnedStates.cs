using UnityEngine;
using System.Collections;


public class NPCOwnedStates
{
	//----------------------------------------------------------------------------- Global States
	public class OnGround : State<NPCEntity>
	{
		private OnGround() {}
		private static OnGround m_Instance = new OnGround();
		public static OnGround Instance()
		{
			return m_Instance;
		}
		
		public override void Enter(NPCEntity NE)
		{
			Debug.Log("Entering OnGround Global State");
			
		}
		
		public override void Execute(NPCEntity NE)
		{
			CheckStateChange (NE);
		}
		
		public override void Exit(NPCEntity NE)
		{
			Debug.Log("Exiting OnGround Global State");
		}
		
		public void CheckStateChange(NPCEntity NE)
		{

		}
		
		public void Jump(NPCEntity NE)
		{
		}
	};
	
	public class InAir : State<NPCEntity>
	{
		private InAir() {}
		private static InAir m_Instance = new InAir();
		public static InAir Instance()
		{
			return m_Instance;
		}
		
		public override void Enter(NPCEntity NE)
		{
			Debug.Log("Entering InAir Global State");
			
		}
		
		public override void Execute(NPCEntity NE)
		{
			CheckStateChange (NE);
		}
		
		public override void Exit(NPCEntity NE)
		{
			Debug.Log("Exiting InAir Global State");
		}
		
		public void CheckStateChange(NPCEntity NE)
		{
		}
	};
	
	//----------------------------------------------------------------------------- States
	public class Neutral : State<NPCEntity>
	{
		private Neutral() {}
		private static Neutral m_Instance = new Neutral();
		public static Neutral Instance()
		{
			return m_Instance;
		}
		
		public override void Enter(NPCEntity NE)
		{
			Debug.Log("Entering Neutral State");
			
		}
		
		public override void Execute(NPCEntity NE)
		{
			CheckStateChange (NE);
		}
		
		public override void Exit(NPCEntity NE)
		{
			Debug.Log("Exiting Neutral State");
		}
		
		public void CheckStateChange(NPCEntity NE)
		{
		}
	};
	
	public class Moving : State<NPCEntity>
	{
		private Moving() {}
		private static Moving m_Instance = new Moving();
		public static Moving Instance()
		{
			return m_Instance;
		}
		
		public override void Enter(NPCEntity NE)
		{
			Debug.Log("Entering Moving State");
			
		}
		
		public override void Execute(NPCEntity NE)
		{
			CheckStateChange (NE);
		}
		
		public override void Exit(NPCEntity NE)
		{	
			Debug.Log("Exiting Moving State");
		}
		
		public void CheckStateChange(NPCEntity NE)
		{
		}
	};
	
	public class Attacking : State<NPCEntity>
	{
		private Attacking() {}
		private static Attacking m_Instance = new Attacking();
		public static Attacking Instance()
		{
			return m_Instance;
		}
		
		public override void Enter(NPCEntity NE)
		{
			Debug.Log("Entering Attacking State");
			
		}
		
		public override void Execute(NPCEntity NE)
		{
		}
		
		public override void Exit(NPCEntity NE)
		{
			Debug.Log("Exiting Attacking State");
		}
	};

}
