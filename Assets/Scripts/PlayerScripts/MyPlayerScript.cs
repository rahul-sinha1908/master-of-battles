using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MasterOfBattles;

public class MyPlayerScript : NetworkBehaviour {
	private GameObject otherPlayer;
	private MyPlayerScript otherPlayerScript;
	private PowersContants powerDatabase;
	public GameObject prefab;
	public GameObject timerPrefab;
	private GameObject timeObject;
	private TimeTracker timeTracker;
	public Camera cam;
	private GameMoveListener gameMoveListener;
	private Vector3 offset=new Vector3(-GameContants.getInstance().sizeOfBoardX/2.0f+0.5f,0,-GameContants.getInstance().sizeOfBoardY/2.0f+0.5f);
	private float playerHeight=0;
	private Vector3 opponentPost=new Vector3(0,0,GameContants.getInstance().sizeOfBoardY*GameContants.getInstance().boxSize);
	protected PlayerDetails[] players;
	private GameObject[] playerObjects;
	private PlayerControlScript[] playerControls;
	private List<Moves> attackMoves;
	public List<Moves> movesList;
	public float moveSpeed=15f;
	private ChessBoardFormation chess;
	private GameRunningConstants grc;
	private TypeO[,] myBoard;

	private void Start () {
		Dev.log(Tag.UnOrdered,"My Ip Address : "+ isServer + " : "+Network.player.ipAddress);

		grc=GameRunningConstants.getInstance();
		if(isLocalPlayer)
			GameRunningConstants.getInstance().localPlayerScript=this;
		else
			GameRunningConstants.getInstance().networkPlayerScript=this;

		powerDatabase=PowersContants.getInstance();
		
		if(!isServer)
			opponentPost=opponentPost*-1;
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
			Dev.log(Tag.UnOrdered,"It is Server too ? ");
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
		chess=ChessBoardFormation.getInstance();
		myBoard=chess.myBoard;
		//initLocalVar();
	}
	public void initLocalVar(){
		if(isLocalPlayer){
			cam=Camera.main;
			gameMoveListener=GameObject.Find("CheckBoard").GetComponent<GameMoveListener>();
			List<Point> onLoc;
			onLoc=chess.TransFormToGame(!isServer);
			players=new PlayerDetails[onLoc.Count];
			for(int i=0;i<onLoc.Count;i++){
				PlayerDetails p;
				p.ind=(short)i;
				p.x=(short)onLoc[i].x;
				p.y=(short)onLoc[i].y;
				p.playerType=(short)chess.gameFormation[i].playerType;
				players[i]=p;
			}
			createPlayer(true);
			gameMoveListener.updateCameraPositionAndVariable(isServer,this,timeTracker, playerControls);
			if(!isServer){
				CmdInitiatePlayers(players);
			}else{
				RpcInitiatePlayers(players);
			}
		}
	}
	private void createPlayer(bool myTeam){
		Dev.log(Tag.MyPlayerScript,"Total Players :"+players.Length);
		playerObjects=new GameObject[players.Length];
		playerControls=new PlayerControlScript[players.Length];
		for(int i=0;i<players.Length;i++){
			//TODO Take the prefab dynamic from Resource Folder according to players[i].playerType
			//prefab=Resources.Load("path of file");
			//Vector3 creationPoint=new Vector3(players[i].x,0,players[i].y)+offset+playerHeight;
			Vector3 creationPoint=GameMethods.getHitVector(players[i].x,players[i].y,playerHeight);
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
				playerControls[i].initializePlayer(isServer, isLocalPlayer, chess.gameFormation[i], this, players[i]);
			else{
				playerControls[i].initializePlayer(isServer, isLocalPlayer, this, players[i]);
				GameRunningConstants.getInstance().networkPlayerInit=true;
			}
			if(myTeam){
				myBoard[players[i].x,players[i].y]=TypeO.MyPlayer;
			}else{
				myBoard[players[i].x,players[i].y]=TypeO.OpponentPlayer;
			}
			//go.layer=LayerMask.GetMask("Hello");
		}
	}
	public PlayerDetails[] getPlayerDetails(){
		return players;
	}
	public PlayerControlScript getPlayerControlScript(int ind){
		return playerControls[ind];
	}

	public void sendMoves(){
		if(!isLocalPlayer)
			return;
		Moves[] moves;
		gameMoveListener.generateMoves();
		Dev.log(Tag.PlayerMove,"Wait For Other Player : "+isServer+" : "+gameMoveListener.getisClickActive());
		gameMoveListener.waitForOtherPlayers();
		Dev.log(Tag.PlayerMove,"Wait For Other Player : "+isServer+" : "+gameMoveListener.getisClickActive());
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
			Dev.log(Tag.PlayerAttack,"ind = "+moves[i].ind+" attackDef="+moves[i].attackDef);
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
		Dev.log(Tag.PlayerMove,"Cmd Move Pos : "+moves.Length);
		doAllThresholdMoves(moves);
		//CmdreinitCount();
	}
	[Command]
	private void CmdInitiatePlayers(PlayerDetails[] players){
		//DONE Send the initial playerDetails
		//DONE Send Details back to the Client through the otherPlayer
		Dev.log(Tag.MyPlayerScript,"Cmd Init Got in : "+players.Length);
		this.players=players;
		createPlayer(false);
		initOtherPlayer();
		//MyPlayerScript oP=otherPlayer.GetComponent<MyPlayerScript>();
		//otherPlayerScript.RpcInitiatePlayers(otherPlayerScript.players);
	}

	[ClientRpc]
	private void RpcMovePos(Moves[] moves){
		if(isServer)
			return;
		Dev.log(Tag.PlayerMove,"Rpc Move Pos : "+moves.Length);
		doAllThresholdMoves(moves);
		//CmdreinitCount();
	}

	[ClientRpc]
	public void RpcInitiatePlayers(PlayerDetails[] players){
		//DONE Send the initial playerDetails
		if(isServer)
			return;
		Dev.log(Tag.MyPlayerScript,"Rpc Initiate Player : "+players.Length);
		this.players=players;
		createPlayer(false);
	}

	[Command]
	public void CmdKillPlayer(short i){
		killFromNetwork(i);
	}

	[ClientRpc]
	public void RpcKillPlayer(short i){
		if(isServer)
			return;
		killFromNetwork(i);
	}

//Remote Calls Till Here
	private void doAllThresholdMoves(Moves[] moves){
		//DONE Complete the threshold Move
		if(otherPlayerScript!=null)
			otherPlayerScript.gameMoveListener.deselectAllPlayers();
		attackMoves.Clear();
		grc.clearMyBackUp();
		for(int i=0;i<moves.Length;i++){
			Dev.log(Tag.PlayerMove,"ind = "+moves[i].ind+" attackDef="+moves[i].attackDef);
			if(moves[i].attackDef==""){
				Dev.log(Tag.PlayerMove,"Entered Here "+players[moves[i].ind].x+" : "+players[moves[i].ind].y);
				grc.addOpponentPlaceMoves(players[moves[i].ind]);

				TypeO meP;
				if(isLocalPlayer)
					meP=TypeO.MyPlayer;
				else
					meP=TypeO.OpponentPlayer;
				
				myBoard[players[moves[i].ind].x,players[moves[i].ind].y]=TypeO.None;
				players[moves[i].ind].x=moves[i].x;
				players[moves[i].ind].y=moves[i].y;
				myBoard[players[moves[i].ind].x,players[moves[i].ind].y]=meP;

				Dev.log(Tag.PlayerMove,"Exit Here "+players[moves[i].ind].x+" : "+players[moves[i].ind].y);
			}else{
				grc.addOpponentAttackMoves(moves[i]);
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
		Vector3 pos=GameMethods.getHitVector(p.x,p.y,playerHeight);
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
		myBoard[players[i].x,players[i].y]=TypeO.None;
		Dev.log(Tag.CheckBoard,"On Killing Local : "+myBoard[players[i].x,players[i].y]);
		Destroy(playerObjects[i],2f);
	}
	private void killFromNetwork(int i){
		playerControls[i].doKillAnimation();
		myBoard[players[i].x,players[i].y]=TypeO.None;
		Dev.log(Tag.CheckBoard,"On Killing Network : "+myBoard[players[i].x,players[i].y]);
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
				Dev.log(Tag.MyPlayerScript,"It Entered Here in Update");
				StartCoroutine(doAttackSequence());
				if(otherPlayerScript!=null)
					otherPlayerScript.prepareForAttack();
				else
					Dev.log(Tag.MyPlayerScript,"Other Player Script Not Found");
				reachedOnce=true;
			}
		}
		//DONE Display All my Characters 
	}
	private IEnumerator doAttackSequence(){
		if(attackMoves.Count>0){
			//DONE Do the attack Sequence
			Dev.log(Tag.PlayerAttack,"Total Attack Sequence : "+isLocalPlayer+" : "+attackMoves.Count);
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
		if(isLocalPlayer){
			Dev.log(Tag.PlayerAttack,"It Came Here prepare for Attack");
			for(int i=0;i<attackMoves.Count;i++)
				grc.addMyAttackMoves(attackMoves[i]);
		}
		StartCoroutine(doAttackSequence());
	}
	public void prepareForNext(){
		Dev.log(Tag.PlayerMove, "Checking clicks : "+isServer+" : "+gameMoveListener.getisClickActive());
		if(isLocalPlayer)
			gameMoveListener.prepareForNextMove();
	}
}
