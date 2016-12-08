using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputFieldNameCheckScript : MonoBehaviour {

	public Text inputFieldText;
	public Text placeHolderText;

	// Use this for initialization
	void Start () {
		inputFieldText.text = NetworkManager.Instance().GetMyPlayer().name;

		if(inputFieldText.text != "")
			placeHolderText.text = "";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
