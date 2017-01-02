using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MasterOfBattles;

public class MyPlayerScript : NetworkBehaviour {
	private GameObject otherPlayer;
	private MyPlayerScript otherPlayerScript;
	private PowersContants powerDatabase;
	public GameObject prefab, bulletPrefab;
	public GameObject timerPrefab;
	private GameObject timeObject;
	private TimeTracker timeTracker;
	public Camera cam;
	private GameMoveListener gameMoveListener;
	private Vector3 offset=new Vector3(-GameContants.sizeOfBoardX/2.0f+0.5f,0,-GameContants.sizeOfBoardY/2.0f+0.5f);
	private Vector3 playerHeight=new Vector3(0,0,0);
	private Vector3 opponentPost=new Vector3(0,0,GameContants.sizeOfBoardY*GameContants.boxSize);
	protected PlayerDetails[] players;
	private GameObject[] playerObjects;
	private PlayerControlScript[] playerControls;
	private List<Moves> attackMoves;
	public List<Moves> movesList;
	public float moveSpeed=15f;
	ChessBoardFormation chess;

	private void Start () {		
		Debug.Log("My Ip Address : "+ isServer + " : "+Network.player.ipAddress);
		powerDatabase=PowersContants.getInstance();
		
		if(!isServer)
			opponentPost=opponentPost*-1;
		opponentPost=opponentPost+playerHeight;	
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
		movesList=new List<Moves>();
		if(isServer){
			Debug.Log("It is Server too ? ");
			if(isLocalPlayer){
				timeObject = GameObject.Instantiate(timerPrefab);
				timeTracker=timeObject.GetComponent<TimeTracker>();
				timeObject.name="TimeTracker";
				NetworkServer.Spawn(timeObject);
			}else{
				timeObject=GameObject.Find("TimeTracker");
				if(timeObject!=null)
					timeTracker=timeObject.GetComponent<TimeTracker>();
			}
			//TODO Fix th authority problem.
		}
		initLocalVar();
	}
	private void initLocalVar(){
		if(isLocalPlayer){
			cam=Camera.main;
			gameMoveListener=GameObject.Find("CheckBoard").GetComponent<GameMoveListener>();
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
		playerControls=new PlayerControlScript[players.Length];
		for(int i=0;i<players.Length;i++){
			//TODO Make the prefab dynamic instead of static
			Vector3 creationPoint=new Vector3(players[i].x,0,players[i].y)+offset+playerHeight;
			creationPoint.x*=GameContants.boxSize;
			creationPoint.z*=GameContants.boxSize;
			//Debug.Log("For "+i+" : "+creationPoint);
			GameObject go = GameObject.Instantiate(prefab,creationPoint,Quaternion.identity);
			if(isLocalPlayer)
				go.transform.LookAt(new Vector3(go.transform.position.x,go.transform.position.y,opponentPost.z));
			else
				go.transform.LookAt(new Vector3(go.transform.position.x,go.transform.position.y,-opponentPost.z));
			if(myTeam){
				go.transform.tag="MyTeam";
				go.name="MyTeam"+players[i].ind;
			}else{
				go.transform.tag="OpponentTeam";
				go.name="OpponentTeam"+players[i].ind;
			}
			playerObjects[i]=go;
			playerControls[i]=go.GetComponent<PlayerControlScript>();
			if(isLocalPlayer)
				playerControls[i].initializePlayer(isServer, isLocalPlayer, chess.gameFormation[i], this);
			else{
				playerControls[i].initializePlayer(isServer, isLocalPlayer, this);
			}
			//go.layer=LayerMask.GetMask("Hello");
		}
	}
	public PlayerDetails[] getPlayerDetails(){
		return players;
	}

	public void sendMoves(){
		if(!isLocalPlayer)
			return;
		Moves[] moves;
		gameMoveListener.generateMoves();
		gameMoveListener.waitForOtherPlayers();
		if(movesList.Count>0){
			moves=movesList.ToArray();
			movesList.Clear();
		}else{
			// moves=new Moves[1];
			// moves[0].ind=5;
			// moves[0].x=5;
			// moves[0].y=5;
			if(isServer){
				RpcMovePos(null);
				//reinitCount();
			}else if(!isServer){
				CmdMovePos(null);
			}
			return;
		}
		for(int i=0;i<moves.Length;i++){
			Debug.Log("ind = "+moves[i].ind+" attackDef="+moves[i].attackDef);
			if(moves[i].attackDef!=""){
				attackMoves.Add(moves[i]);
			}
		}
		if(isServer){
			RpcMovePos(moves);
			//reinitCount();
		}else if(!isServer){
			CmdMovePos(moves);
		}
	}
	public void changeClientCountDown(bool b){
		CmdchangeClientCountDown(b);
	}

//Remote Calls From Here

	[Command]
	private void CmdchangeClientCountDown(bool b){
		timeTracker.playerCountDownClient=b;
	}
	[Command]
	private void CmdMovePos(Moves[] moves){
		Debug.Log("Cmd Move Pos : "+moves.Length);
		doAllThresholdMoves(moves);
		//CmdreinitCount();
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
		//CmdreinitCount();
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

	[Command]
	public void CmdKillPlayer(short i){
		killFromNetwork(i);
	}

	[ClientRpc]
	public void RpcKillPlayer(short i){
		killFromNetwork(i);
	}

//Remote Calls Till Here
	private void doAllThresholdMoves(Moves[] moves){
		//TODO Complete the threshold Move
		attackMoves.Clear();
		for(int i=0;i<moves.Length;i++){
			Debug.Log("ind = "+moves[i].ind+" attackDef="+moves[i].attackDef);
			if(moves[i].attackDef==""){
				Debug.Log("Entered Here "+players[moves[i].ind].x+" : "+players[moves[i].ind].y);
				players[moves[i].ind].x=moves[i].x;
				players[moves[i].ind].y=moves[i].y;
				Debug.Log("Exit Here "+players[moves[i].ind].x+" : "+players[moves[i].ind].y);
			}else{
				attackMoves.Add(moves[i]);
			}
		}
		reachedOnce=false;
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
			if(otherPlayer!=null)
				otherPlayerScript=otherPlayer.GetComponent<MyPlayerScript>();
		}
	}
	private bool comparePositions(GameObject g, PlayerDetails p){
		Vector3 pos=new Vector3(p.x,0, p.y)+playerHeight+offset;
		pos.x*=GameContants.boxSize;
		pos.z*=GameContants.boxSize;
		if(g.transform.position.x==pos.x && g.transform.position.z==pos.z)
			return true;
		return false;
	}
	private void moveMyPlayer(int i){
		playerControls[i].movePlayer(players[i]);		
	}

	public void killThePlayer(int i){
		if(isServer){
			RpcKillPlayer((short)i);
		}else{
			CmdKillPlayer((short)i);
		}
		Destroy(playerObjects[i],2f);
	}
	private void killFromNetwork(int i){
		playerControls[i].doKillAnimation();
		Destroy(playerObjects[i],2f);
	}
	private bool reachedOnce=true;
	private void Update () {
		initOtherPlayer();

		if(players!=null){
			bool startAttackSequence=true;
			for(int i=0;i<players.Length;i++){
				if(playerObjects[i]!=null){
					if(!comparePositions(playerObjects[i],players[i])){
						startAttackSequence=false;
						reachedOnce=false;
						moveMyPlayer(i);
					}
				}
			}
			if(!isLocalPlayer && startAttackSequence && !reachedOnce){
				Debug.Log("It Entered Here in Update");
				StartCoroutine(doAttackSequence());
				if(otherPlayerScript!=null)
					otherPlayerScript.prepareForAttack();
				else
					Debug.Log("Other Player Script Not Found");
				reachedOnce=true;
			}
		}
		//DONE Display All my Characters 
	}
	private IEnumerator doAttackSequence(){
		if(attackMoves.Count>0){
			//TODO Do the attack Sequence
			Debug.Log("Total Attack Sequence : "+isLocalPlayer+" : "+attackMoves.Count);
			for(int i=0;i<attackMoves.Count;i++){
				int x1,y1,x2,y2;
				int ind=attackMoves[i].ind;
				x1=players[ind].x;
				y1=players[ind].y;
				x2=attackMoves[i].x;
				y2=attackMoves[i].y;

				PowerStruct p = PowersContants.getPowerStruct(attackMoves[i].attackDef);
				playerControls[ind].doAttack(x1,y1,x2,y2,p);
			}
		}
		attackMoves.Clear();
		yield return new WaitForSeconds(2);
		if(isLocalPlayer){
			prepareForNext();
		}
		yield break;
	}
	public void prepareForAttack(){
		//DONE call prepare for next
		if(isLocalPlayer)
			Debug.Log("It Came Here prepare for Attack");
		StartCoroutine(doAttackSequence());
	}
	public void prepareForNext(){
		if(!gameMoveListener.getisClickActive())
			gameMoveListener.prepareForNextMove();
	}
}
