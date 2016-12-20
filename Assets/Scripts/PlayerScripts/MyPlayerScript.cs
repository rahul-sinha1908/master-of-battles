using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MasterOfBattles;

public class MyPlayerScript : NetworkBehaviour {
	[SyncVar]
	public int timeleft;
	[SyncVar]
	public bool playerCountDown;

	private GameObject otherPlayer;
	struct Moves{
		short ind,x,y;
		short attackInd;
		int intensity;
	};
	struct PlayerDetails{
		short ind,x,y, playerType;
	}

	List<Point> onLoc;
	ChessBoardFormation chess;

	private void Start () {
		if(isServer){
			transform.name="ServerPlayer";
		}
		else{
			transform.name="ClientPlayer";
		}
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
		doAllThresholdMoves(moves);
	}

	[Command]
	private void CmdInitiatePlayers(List<PlayerDetails> players){
		//TODO Send the initial playerDetails
	}

	[ClientRpc]
	private void RpcMovePos(List<Moves> moves){
		doAllThresholdMoves(moves);
	}

	[ClientRpc]
	private void RpcInitiatePlayers(List<PlayerDetails> players){
		//TODO Send the initial playerDetails
	}

	[Command]
	public void CmdAct(){

	}

	private void doAllThresholdMoves(List<Moves> moves){
		//TODO Complete the threshold Move
	}

	private void Update () {
		if(otherPlayer==null){
			if(isServer)
				otherPlayer=GameObject.Find("ClientPlayer");
			else
				otherPlayer=GameObject.Find("ServerPlayer");
		}
		if(!isLocalPlayer)
			return;
		//TODO Display All my Characters 
	}
}
