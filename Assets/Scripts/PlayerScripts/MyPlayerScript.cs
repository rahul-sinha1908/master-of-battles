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
	public GameObject prefab;
	private Vector3 offset=new Vector3(-14.5f,0,-14.5f);
	struct Moves{
		short ind,x,y;
		short attackInd;
		int intensity;
	};
	public struct PlayerDetails{
		public short ind,x,y, playerType;
	}

	
	protected List<PlayerDetails> players;
	ChessBoardFormation chess;

	private void Start () {
		if(isServer && isLocalPlayer){
			transform.name="ServerPlayer";
		}else if(isServer && !isLocalPlayer){
			transform.name="ClientPlayer";
		}
		if(!isServer && isLocalPlayer){
			transform.name="ClientPlayer";
		}else if(!isServer && !isLocalPlayer){
			transform.name="ServerPlayer";
		}
		initLocalVar();
	}
	private void initLocalVar(){
		if(isLocalPlayer){
			chess=new ChessBoardFormation();
			List<Point> onLoc;
			onLoc=chess.TransFormToGame(!isServer);
			players=new List<PlayerDetails>();
			for(int i=0;i<onLoc.Count;i++){
				PlayerDetails p;
				p.ind=(short)i;
				p.x=(short)onLoc[i].x;
				p.y=(short)onLoc[i].y;
				p.playerType=1;
				players.Add(p);
			}
			createPlayer();
			if(!isServer){
				CmdInitiatePlayers(players);
			}
		}
	}

	[Command]
	private void CmdMovePos(List<Moves> moves){
		doAllThresholdMoves(moves);
	}
	private void createPlayer(){
		Debug.Log("Toatal Players :"+players.Count);
		for(int i=0;i<players.Count;i++){
			GameObject.Instantiate(prefab,new Vector3(players[i].x,0,players[i].y)+offset,Quaternion.identity);
		}
	}
	[Command]
	private void CmdInitiatePlayers(List<PlayerDetails> players){
		//DONE Send the initial playerDetails
		//DONE Send Details back to the Client through the otherPlayer
		this.players=players;
		createPlayer();
		initOtherPlayer();
		MyPlayerScript oP=otherPlayer.GetComponent<MyPlayerScript>();
		oP.RpcInitiatePlayers(oP.players);
	}

	[ClientRpc]
	private void RpcMovePos(List<Moves> moves){
		doAllThresholdMoves(moves);
	}

	[ClientRpc]
	public void RpcInitiatePlayers(List<PlayerDetails> players){
		//DONE Send the initial playerDetails
		this.players=players;
		createPlayer();
	}

	[Command]
	public void CmdAct(){

	}

	private void doAllThresholdMoves(List<Moves> moves){
		//TODO Complete the threshold Move
	}

	private void initOtherPlayer(){
		if(otherPlayer==null){
			if(isServer && isLocalPlayer)
				otherPlayer=GameObject.Find("ClientPlayer");
			else if(isServer && !isLocalPlayer)
				otherPlayer=GameObject.Find("ServerPlayer");
			if(!isServer && isLocalPlayer)
				otherPlayer=GameObject.Find("ServerPlayer");
			else if(!isServer && !isLocalPlayer)
				otherPlayer=GameObject.Find("ClientPlayer");
		}
	}
	private void Update () {
		initOtherPlayer();
		if(!isLocalPlayer)
			return;
		//TODO Display All my Characters 
	}
}
