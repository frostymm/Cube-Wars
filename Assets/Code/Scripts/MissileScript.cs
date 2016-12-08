using UnityEngine;
using System.Collections;

public class MissileScript : Photon.MonoBehaviour {

	public GameObject child;
	private MeshRenderer meshr;

	// Use this for initialization
	void Start () {
		meshr = (MeshRenderer)GetComponent(typeof(MeshRenderer));
	}
	
	// Update is called once per frame
	void Update () {
		MeshRenderer mr = (MeshRenderer)child.GetComponent(typeof(MeshRenderer));

		mr.enabled = meshr.enabled;
	}
}
