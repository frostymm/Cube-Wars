using UnityEngine;
using System.Collections;

public class WheelColliderManagerScript : MonoBehaviour {

	public WheelCollider[] Wheels;

	public float Mass = 1f;
	public float Radius = 1f;
	public float SuspensionDistance = 0.1f;

	public float Spring = 0f;
	public float Damper = 0f;
	public float TargetPosition = 0f;

	public float FFExtremeSlip = 0f;
	public float FFExtremeValue = 0f;
	public float FFAsymptoteSlip = 0f;
	public float FFAsymptoteValue = 0f;
	public float FFStiffnessFactor = 0f;

	public float SFExtremeSlip = 0f;
	public float SFExtremeValue = 0f;
	public float SFAsymptoteSlip = 0f;
	public float SFAsymptoteValue = 0f;
	public float SFStiffnessFactor = 0f;

	WheelFrictionCurve WFC = new WheelFrictionCurve();
	JointSpring JS = new JointSpring();
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		foreach(WheelCollider wheel in Wheels)
		{
			if(wheel.mass != Mass)
				wheel.mass = Mass;
			if(wheel.radius != Radius)
				wheel.radius = Radius;
			if(wheel.suspensionDistance != SuspensionDistance)
				wheel.suspensionDistance = SuspensionDistance;

			if(wheel.suspensionSpring.spring != Spring)
				JS.spring = Spring;
			if(wheel.suspensionSpring.damper != Damper)
				JS.damper = Damper;
			if(wheel.suspensionSpring.targetPosition != TargetPosition)
				JS.targetPosition = TargetPosition;

			wheel.suspensionSpring = JS;

			if(wheel.forwardFriction.extremumSlip != FFExtremeSlip)
				WFC.extremumSlip = FFExtremeSlip;
			if(wheel.forwardFriction.extremumValue != FFExtremeValue)
				WFC.extremumValue = FFExtremeValue;
			if(wheel.forwardFriction.asymptoteSlip != FFAsymptoteSlip)
				WFC.asymptoteSlip = FFAsymptoteSlip;
			if(wheel.forwardFriction.asymptoteValue != FFAsymptoteValue)
				WFC.asymptoteValue = FFAsymptoteValue;
			if(wheel.forwardFriction.stiffness != FFStiffnessFactor)
				WFC.stiffness = FFStiffnessFactor;

			wheel.forwardFriction = WFC;

			if(wheel.sidewaysFriction.extremumSlip != SFExtremeSlip)
				WFC.extremumSlip = SFExtremeSlip;
			if(wheel.sidewaysFriction.extremumValue != SFExtremeValue)
				WFC.extremumValue = SFExtremeValue;
			if(wheel.sidewaysFriction.asymptoteSlip != SFAsymptoteSlip)
				WFC.asymptoteSlip = SFAsymptoteSlip;
			if(wheel.sidewaysFriction.asymptoteValue != SFAsymptoteValue)
				WFC.asymptoteValue = SFAsymptoteValue;
			if(wheel.sidewaysFriction.stiffness != SFStiffnessFactor)
				WFC.stiffness = SFStiffnessFactor;

			wheel.sidewaysFriction = WFC;
		}
	}
}
