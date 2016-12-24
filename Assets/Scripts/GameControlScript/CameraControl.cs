﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MasterOfBattles;

public class CameraControl : MonoBehaviour {

	GameObject checkBoard;
	
	// Use this for initialization
	void Start () {
		checkBoard = GameObject.Find ("CheckBoard");
		transform.position=GameContants.boxSize*transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(checkBoard!=null)
			transform.LookAt (checkBoard.transform.position);
		else{
			checkBoard = GameObject.Find ("CheckBoard");
		}
	}
}
