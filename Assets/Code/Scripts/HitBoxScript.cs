using UnityEngine;
using System.Collections;

public class HitBoxScript : MonoBehaviour {

	public HitBox OwnerHB;

	void OnTriggerEnter(Collider coll) 
	{
		OwnerHB.OnTriggerEnter(coll);
	}

	// Use this for initialization
	void Start () {
		OwnerHB.Start();
	}
	
	// Update is called once per frame
	void Update () {
		OwnerHB.Update(Time.deltaTime);
	}
}
