using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MasterOfBattles;

public class MyPlayerScript : NetworkBehaviour {

	struct Moves{
		short ind,x,y;
		short attackInd;
	};
	struct PlayerDetails{
		short ind,x,y, playerType;
	}

	List<Point> onLoc; 
	ChessBoardFormation chess;
	// Use this for initialization
	private void Start () {
		initLocalVar();
	}
	private void initLocalVar(){
		if(isLocalPlayer){
			chess=new ChessBoardFormation();
			onLoc=chess.TransFormToGame();
		}
	}

	[Command]
	private void CmdMovePos(List<Moves> moves){
		//TODO Send the final movement from the cleint side
	}

	[Command]
	private void CmdInitiatePlayers(List<PlayerDetails> players){
		//TODO Send the initial playerDetails
	}

	[ClientRpc]
	private void RpcMovePos(List<Moves> moves){
		//TODO Send the final movement from the cleint side
	}

	[ClientRpc]
	private void RpcInitiatePlayers(List<PlayerDetails> players){
		//TODO Send the initial playerDetails
	}

	private void Update () {
		if(!isLocalPlayer)
			return;

		//TODO Display All my Characters 
	}
}
