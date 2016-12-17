using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Movement : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isLocalPlayer)
			transform.position = transform.position + new Vector3(Input.GetAxisRaw("Horizontal")* 10 * Time.deltaTime,0, Input.GetAxisRaw("Vertical")*10 * Time.deltaTime);
	}
}
