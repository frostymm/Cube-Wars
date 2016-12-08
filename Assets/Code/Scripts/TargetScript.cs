using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour {

	TargetEntity te;

	public Collider col;

	// Use this for initialization
	void Start () 
	{
		te.SetCollider(col);

		te.Start ();
		te.SetTeam(1);
	}
	
	// Update is called once per frame
	void Update () {
		te.Update(Time.deltaTime);
	}

	void FixedUpdate()
	{
		te.FixedUpdate();
	}

	void Awake()
	{
		te = new TargetEntity(gameObject);
	}
}
