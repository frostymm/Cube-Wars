using UnityEngine;
using System.Collections;

public class FollowScript : MonoBehaviour {

	public GameObject ObjectToFollow;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(ObjectToFollow)
			transform.position = ObjectToFollow.transform.position;
	}
}
