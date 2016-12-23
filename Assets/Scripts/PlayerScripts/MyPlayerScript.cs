using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MasterOfBattles;

public class MyPlayerScript : NetworkBehaviour {
	private GameObject otherPlayer;
	public GameObject prefab;
	public GameObject timerPrefab;
	public Camera cam;
	private GameMoveListener gameMoveListener;
	private Vector3 offset=new Vector3(-14.5f,0,-14.5f);
	private Vector3 playerHeight=new Vector3(0,1,0);
	protected PlayerDetails[] players;
	private GameObject[] playerObjects;
	private List<Moves> attackMoves;
	ChessBoardFormation chess;

	private void Start () {
		Debug.Log("My Ip Address : "+ isServer + " : "+Network.player.ipAddress);
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
		attackMoves=new List<Moves>();
		if(isServer){
			GameObject g = GameObject.Instantiate(timerPrefab);
			NetworkServer.Spawn(g);
		}
		initLocalVar();
	}
	private void initLocalVar(){
		if(isLocalPlayer){
			cam=Camera.main;
			gameMoveListener=cam.gameObject.GetComponent<GameMoveListener>();
			chess=ChessBoardFormation.getInstance();
			List<Point> onLoc;
			onLoc=chess.TransFormToGame(!isServer);
			players=new PlayerDetails[onLoc.Count];
			for(int i=0;i<onLoc.Count;i++){
				PlayerDetails p;
				p.ind=(short)i;
				p.x=(short)onLoc[i].x;
				p.y=(short)onLoc[i].y;
				p.playerType=1;
				players[i]=p;
			}
			createPlayer(true);
			gameMoveListener.updateCameraPositionAndVariable(isServer,this);
			if(!isServer){
				CmdInitiatePlayers(players);
				// short[] a=new short[1];
				// a[0]=10;
				// CmdCheck(a);
			}
		}
	}
	private void createPlayer(bool myTeam){
		Debug.Log("Toatal Players :"+players.Length);
		playerObjects=new GameObject[players.Length];
		for(int i=0;i<players.Length;i++){
			//TODO Make the prefab dynamic instead of static
			Vector3 creationPoint=new Vector3(players[i].x,0,players[i].y)+offset+playerHeight;
			creationPoint.x*=GameContants.boxSize;
			creationPoint.z*=GameContants.boxSize;
			//Debug.Log("For "+i+" : "+creationPoint);
			GameObject go = GameObject.Instantiate(prefab,creationPoint,Quaternion.identity);
			if(myTeam){
				go.transform.tag="MyTeam";
				go.name="MyTeam"+players[i].ind;
			}else{
				go.transform.tag="OpponentTeam";
				go.name="OpponentTeam"+players[i].ind;
			}
			playerObjects[i]=go;
			//go.layer=LayerMask.GetMask("Hello");
		}
	}
	public PlayerDetails[] getPlayerDetails(){
		return players;
	}

	public void sendMoves(Moves[] moves){
		if(!isLocalPlayer)
			return;
		
		if(isServer){
			RpcMovePos(moves);
		}else if(!isServer){
			CmdMovePos(moves);
		}
	}

//Remote Calls From Here
	[Command]
	private void CmdMovePos(Moves[] moves){
		Debug.Log("Cmd Move Pos : "+moves.Length);
		doAllThresholdMoves(moves);
	}
	[Command]
	private void CmdInitiatePlayers(PlayerDetails[] players){
		//DONE Send the initial playerDetails
		//DONE Send Details back to the Client through the otherPlayer
		Debug.Log("Cmd Init Got in : "+players.Length);
		this.players=players;
		createPlayer(false);
		initOtherPlayer();
		MyPlayerScript oP=otherPlayer.GetComponent<MyPlayerScript>();
		oP.RpcInitiatePlayers(oP.players);
	}

	[ClientRpc]
	private void RpcMovePos(Moves[] moves){
		if(isServer)
			return;
		Debug.Log("Rpc Move Pos : "+moves.Length);
		doAllThresholdMoves(moves);
	}

	[ClientRpc]
	public void RpcInitiatePlayers(PlayerDetails[] players){
		//DONE Send the initial playerDetails
		if(isServer)
			return;
		Debug.Log("Rpc Initiate Player : "+players.Length);
		this.players=players;
		createPlayer(false);
	}

//Remote Calls Till Here
	private void doAllThresholdMoves(Moves[] moves){
		//TODO Complete the threshold Move
		for(int i=0;i<moves.Length;i++){
			if(moves[i].attackInd!=-1){
				players[moves[i].ind].x=moves[i].x;
				players[moves[i].ind].y=moves[i].y;
			}else{
				attackMoves.Add(moves[i]);
			}
		}
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
	private bool comparePositions(GameObject g, PlayerDetails p){
		if(g.transform.position.x==p.x*GameContants.boxSize && g.transform.position.y==p.y*GameContants.boxSize)
			return true;
		return false;
	}
	private void Update () {
		initOtherPlayer();
		if(!isLocalPlayer)
			return;

		if(players!=null){
			for(int i=0;i<players.Length;i++){
				if(playerObjects[i]!=null){
					if(comparePositions(playerObjects[i],players[i])){
						//TODO Do animation and stuffs
						playerObjects[i].transform.position=(new Vector3(players[i].x,0, players[i].y))*GameContants.boxSize+playerHeight;
					}
				}
			}
			if(attackMoves.Count>0){
				//TODO Do the attack Sequence
			}
		}
		//TODO Display All my Characters 
	}
}
