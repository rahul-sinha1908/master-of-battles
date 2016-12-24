﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MasterOfBattles;

public class CameraControl : MonoBehaviour {

	[SerializeField]
	private GameObject checkBoardPrefab;
	private GameObject checkBoard;
	
	// Use this for initialization
	void Start () {
		transform.position=GameContants.boxSize*transform.position;
		checkBoard= GameObject.Instantiate(checkBoardPrefab);
		checkBoard.name="CheckBoard";
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt (checkBoard.transform.position);
	}
}
