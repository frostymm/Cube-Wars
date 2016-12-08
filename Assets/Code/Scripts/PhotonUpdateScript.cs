using UnityEngine;
using System.Collections;

public class PhotonUpdateScript : Photon.MonoBehaviour {

	private Vector3 correctPos;
	private Quaternion correctRot;

	// Use this for initialization
	void Start () {
	
	}

	private int UpdateFrames = 0;
	// Update is called once per frame
	void Update () {
		if(GameManager.Instance().IsOnline)
		{
			if(!photonView.isMine)
			{
				if(UpdateFrames++ > 10)
				{
					transform.position = Vector3.Lerp(transform.position, this.correctPos, Time.deltaTime * 5);
					transform.rotation = Quaternion.Lerp(transform.rotation, this.correctRot, Time.deltaTime * 5);
				}
			}
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else
		{
			// Network player, receive data
			correctPos = (Vector3)stream.ReceiveNext();
			correctRot = (Quaternion)stream.ReceiveNext();
		}
	}
}
