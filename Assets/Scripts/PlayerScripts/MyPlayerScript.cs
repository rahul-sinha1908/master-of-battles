using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MasterOfBattles;

public class MyPlayerScript : NetworkBehaviour {

	SyncListStruct<Point> myPos;
	ChessBoardFormation chess;
	// Use this for initialization
	void Start () {
		chess=new ChessBoardFormation();
		myPos=chess.TransFormToGame();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
