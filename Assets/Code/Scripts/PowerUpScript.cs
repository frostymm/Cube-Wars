using UnityEngine;
using System.Collections;

public class PowerUpScript : MonoBehaviour {

	public string PowerUp = "";

	void OnTriggerEnter(Collider coll)
	{
		if(GameManager.Instance().GetParticipantByCollider().ContainsKey(coll))
		{
			GameManager.Instance().GetParticipantByCollider()[coll].HandlePowerUp(PowerUp);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
