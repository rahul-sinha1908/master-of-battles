﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using MasterOfBattles;

public class GameMoveListener : MonoBehaviour {
	private InputManager inputManager;
	private int offsetHitX=GameContants.sizeOfBoardX/2;
	private int offsetHitY=GameContants.sizeOfBoardY/2;
	private PowersContants powerDatabase;
	[SerializeField]
	private LayerMask mask;
	private bool isServer;
	private int boxX,boxY;
	private MyPlayerScript myPlayerScript;
	private PlayerDetails[] players, backUpMoves;
	private PlayerControlScript[] playerControls;
	private Moves[] attackMoveT;
	private List<Moves> moves;
	private float orthoZoomSpeed=0.2f, perspectiveZoomSpeed=0.2f, camPanSpeed=1f, defaultFeildOfView;
	private Vector3 defaultCamVector;	
	private Camera cam;
	private bool isClicksActive, applyeRestrictions;
	private bool trackClicks=true;
	private Vector2 trackClickVect;
	private Vector3 trackDragVect;
	private float trackClickCount=0f;
	[SerializeField]
	private CheckSelectScript selectScript;
	private bool selectPlayer=true;
	private int selectedPlayerInd=-1;
	private float doubleClickTimeLimit=0.3f;
	private List<Point> listPossibleMoves;
	private ChessBoardFormation chess;
	private TypeO[,] myBoard;
	private List<PlayerProperties> playerProp;
	private bool isAttack;
	private GameRunningConstants grc;
	// Use this for initialization
	void Start () {
		GameRunningConstants.getInstance().gameMoveListener=this;
		cam = Camera.main;
		defaultCamVector=cam.transform.position;
		defaultFeildOfView=cam.fieldOfView;


		powerDatabase=PowersContants.getInstance();
		chess=ChessBoardFormation.getInstance();
		myBoard=chess.myBoard;
		playerProp=chess.gameFormation;
		initialiseHealtMetre();
		grc=GameRunningConstants.getInstance();

		trackClicks=false;
		isClicksActive=true;
	}
	private void initialiseHealtMetre(){
		for(int i=0;i<playerProp.Count;i++){
			playerProp[i].curHealth=playerProp[i].healthMetre;
		}
	}
	private void detectZoom(float wheel){
		Dev.log(Tag.GameMoveListener,"Touch Count : "+Input.touchCount);

		float deltaMagnitudeDiff=0f;
		
		if(wheel==0){
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
			//deltaMagnitudeDiff = Mathf.Sqrt(deltaMagnitudeDiff);
		}else{
			deltaMagnitudeDiff=-wheel* PlayerPrefs.GetInt(UserPrefs.scrollSpeed, 100);
		}
		
		if(!(cam.transform.position.y<5 && cam.transform.position.y+deltaMagnitudeDiff<cam.transform.position.y))
			cam.transform.Translate(-cam.transform.forward*deltaMagnitudeDiff, Space.World);
	}
	
	private bool compareTwoPlayerDetails(PlayerDetails a, PlayerDetails b){
		if(a.x==b.x && a.y==b.y)
			return true;
		return false;
	}
	private void SingleClick(Vector2 vect)
	{	
		Dev.log(Tag.PlayerSelect,"Single Click");
		Ray ray;
		ray=Camera.main.ScreenPointToRay(vect);
		performActionOnHit(ray, true);
	}
	private void DoubleClick(Vector2 vect)
	{
		Dev.log(Tag.PlayerSelect,"Double Click");
		Ray ray;
		ray=Camera.main.ScreenPointToRay(vect);
		performActionOnHit(ray, false);
	}

	private void checkClickListener(){
		if(!isClicksActive)
			return;
		if(trackClicks==false){
			//TODO DO the function for assigning the initial values
			if(Input.touchCount==1 && Input.GetTouch(0).phase==TouchPhase.Began){
				trackClickVect=Input.GetTouch(0).position;
				trackClicks=true;
				trackClickCount=0f;
			}
			else if(Input.GetButtonDown("Fire1")){
				trackClickVect=Input.mousePosition;
				trackClicks=true;
				trackClickCount=0f;
			}
		}
		else if(trackClickCount>doubleClickTimeLimit){
			SingleClick(trackClickVect);
			trackClicks=false;
			trackClickCount=0f;
		}
		else{
			if(Input.touchCount==1 && Input.GetTouch(0).phase==TouchPhase.Began){
				if(GameMethods.sqrDist(trackClickVect, Input.GetTouch(0).position)<25)
					DoubleClick(trackClickVect);
				trackClicks=false;
			}else if(Input.touchCount==1 && Input.GetTouch(0).phase==TouchPhase.Moved){
				trackClicks=false;
			}
			else if(Input.GetButtonDown("Fire1"))
			{
				//Dev.error(Tag.UnOrdered,"It Entered Here3 : "+CrossPlatformInputManager.GetButtonDown("Fire1"));
				if(Vector2.Distance(trackClickVect, Input.mousePosition)<5)
					DoubleClick(trackClickVect);
				trackClicks=false;
			}else if(Input.GetButton("Fire1")){
				if(Vector2.Distance(trackClickVect, Input.mousePosition)>5){
					trackClicks=false;
				}
			}
			trackClickCount += Time.deltaTime;
		}
	}
	private void Pan(Vector2 touchDeltaPosition){
		if(!isServer)
			touchDeltaPosition=-touchDeltaPosition;
		if(Mathf.Abs(cam.transform.position.x)> GameContants.sizeOfBoardX*GameContants.boxSize/2 && Mathf.Abs(cam.transform.position.x-touchDeltaPosition.x)>Mathf.Abs(cam.transform.position.x))
			touchDeltaPosition.x=0;
		//TODO Take care of this constant value if ever camera position is changed
		if(Mathf.Abs(cam.transform.position.z)> 33*GameContants.boxSize && Mathf.Abs(cam.transform.position.z-touchDeltaPosition.y)>Mathf.Abs(cam.transform.position.z))
			touchDeltaPosition.y=0;
		cam.transform.Translate(-touchDeltaPosition.x * camPanSpeed, 0, -touchDeltaPosition.y * camPanSpeed, Space.World);
	}
	// Update is called once per frame
	void Update () {
		// applyeRestrictions=true;
		// if(applyeRestrictions)
		// 	return;
		if(grc.disableClicks)
			return;
		
		checkClickListener();
		if(Application.isMobilePlatform){
			if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
				Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
				touchDeltaPosition=touchDeltaPosition*PlayerPrefs.GetFloat(UserPrefs.mobPanSensitivity, 0.6f);
				Pan(touchDeltaPosition);
			}else if (Input.touchCount == 2){
				detectZoom(0);
			}
		}else{
			if(Input.GetAxis("Mouse ScrollWheel")!=0){
				Dev.log(Tag.GameMoveListener,Input.GetAxis("Mouse ScrollWheel"));
				detectZoom(Input.GetAxis("Mouse ScrollWheel"));
			}else if(Input.GetButtonDown("Fire1")){
				trackDragVect=Input.mousePosition;
			}else if(Input.GetButton("Fire1")){
				if(Input.mousePosition!=trackDragVect){
					Vector2 delta=Input.mousePosition-trackDragVect;
					trackDragVect=Input.mousePosition;
					delta = delta * PlayerPrefs.GetFloat(UserPrefs.moveSensitivity, 0.2f);
					Pan(delta);
				}
			}
		}
		if(selectedPlayerInd!=-1){
			inputManager.showHealthValue(playerProp[selectedPlayerInd].curHealth);
		}
	}
	private void setDefaultCameraPostion(){
		cam.transform.position=defaultCamVector;
		cam.transform.LookAt(transform.position);
		cam.fieldOfView=defaultFeildOfView;
	}
	private void performActionOnHit(Ray ray, bool move){
		RaycastHit hit;
		if(!applyeRestrictions && !move){
			return;
			//TODO Give an instruction that you cant fire in the first turn
		}
		//DONE Put a layer mask on this.
		if(Physics.Raycast(ray,out hit,10000,mask)){
			//DONE Divide this by a constant if you want to increase the area of the play
			boxX=(int)Mathf.Floor(hit.point.x/GameContants.boxSize)+offsetHitX;
			boxY=(int)Mathf.Floor(hit.point.z/GameContants.boxSize)+offsetHitY;
			Dev.log(Tag.PlayerSelect,hit.point+" : "+boxX+" : "+boxY);
			Point p;
			p.x=boxX;
			p.y=boxY;
			if(hit.collider.gameObject.layer==LayerMask.NameToLayer("CheckBoard") || hit.collider.gameObject.tag!="MyTeam"){
				int k=checKThePlayerAt(p);
				if(k>=0)
					selectPlayer=true;
				if(selectPlayer){
					selectedPlayerInd=k;
					if(selectedPlayerInd>=0){
						if(applyeRestrictions){
							// p.x=backUpMoves[k].x;
							// p.y=backUpMoves[k].y;
						}
						selectAndSearch(p,move);
					}
				}else{
					onPLayerMoveOrAttack(p);
				}
			}else if(hit.collider.gameObject.layer==LayerMask.NameToLayer("PlayerObjects")){
				string name = hit.collider.gameObject.name;
				int k=name.LastIndexOf('m')+1;
				int ind= Int32.Parse(name.Substring(k));
				Dev.log(Tag.PlayerSelect,"Selected Player : "+ind);
				// p.x=backUpMoves[ind].x;
				// p.y=backUpMoves[ind].y;
				selectedPlayerInd=ind;
				selectAndSearch(p,move);
			}
		}
	}
	private void onPLayerMoveOrAttack(Point p){
		if(selectedPlayerInd<0 || selectedPlayerInd>players.Length)
			return;
		if(!isAttack){//Move Sequence
			int s=selectedPlayerInd;

			//TODO Instead of using 1 constant use the player properties to get the max moves.
			if(!isPossibleMove(p))
				return;
			
			TypeO type=myBoard[players[selectedPlayerInd].x,players[selectedPlayerInd].y];
			Dev.log(Tag.CheckBoard, "1 : "+type );
			myBoard[players[selectedPlayerInd].x,players[selectedPlayerInd].y]=TypeO.None;
			myBoard[p.x,p.y]=type;
			Dev.log(Tag.CheckBoard, "2 : "+myBoard[p.x,p.y]);
			players[selectedPlayerInd].x=(short)p.x;
			players[selectedPlayerInd].y=(short)p.y;
			attackMoveT[selectedPlayerInd].x=-1;
			attackMoveT[selectedPlayerInd].y=-1;
			changePlayerIdentityMaterial(selectedPlayerInd);

		}else{//Attack Sequence

			p.x=boxX;
			p.y=boxY;
			if(isPossibleMove(p)){
				Dev.log(Tag.PlayerAttack,"Possible Attack");
				attackMoveT[selectedPlayerInd].x=(short)p.x;
				attackMoveT[selectedPlayerInd].y=(short)p.y;
				attackMoveT[selectedPlayerInd].attackDef=PowersContants.getPowerDefString(getDefaultPower(selectedPlayerInd));
				Dev.log(Tag.PlayerAttack,"The Attack Definition is :"+attackMoveT[selectedPlayerInd].attackDef);
				searchPossibleAttacks(selectedPlayerInd);
				playerControls[selectedPlayerInd].playerIsAttacking();
			}
		}
		//selectPlayer=true;
	}
	private void selectAndSearch(Point p, bool move){
		if(!move && !compareTwoPlayerDetails(players[selectedPlayerInd], backUpMoves[selectedPlayerInd])){
			//TODO Display a feedback that you cant attack from that position
			return;
		}
		
		selectScript.showSelectedTiles(p,BoardConstants.Select);
		selectPlayer=false;
		searchPossibleMoves(selectedPlayerInd,move, p);
		//searchPossibleMoves(selectedPlayerInd,move);
	}
	private bool isPossibleMove(Point p){
		Dev.log(Tag.CheckBoard,"Is Possible Move 1:"+myBoard[p.x,p.y]);
		if(myBoard[p.x,p.y]!=TypeO.None)
			return false;
		for(int i=0;i<listPossibleMoves.Count;i++){
			if(p.Equals(listPossibleMoves[i])){
				return true;
			}
		}
		return false;
	}
	private PowerStruct getDefaultPower(int ind){
		//TODO Get the Power that is selected.
		List<PowerStruct> allPowers = playerProp[ind].powers;
		int defPower=PlayerPrefs.GetInt("RahulWeapon"+ind,1);
		PowerStruct power=new PowerStruct();
		for(int i=0;i<allPowers.Count;i++){
			if(allPowers[i].id==defPower)
				power=allPowers[i];
		}

		//Range Should not be changed here.
		//TODO Take the stored strength and Other Definition for this
		int strength=PlayerPrefs.GetInt("RahulWeapon"+ind+"-"+defPower,power.strength);
		power.strength=strength;
		return power;
	}
	public void setDefaultPower(int ind, int defP, int strength){
		//TODO Set the strength for a particular defP for a Particular Player.
		PlayerPrefs.SetInt("RahulWeapon"+ind+"-"+defP,strength);
	}
	public void setDefaultPower(int ind, int defP){
		//TODO Set default defP for a Particular Player.

		/*
		Name - RahulWeapon[IND] in PlayerPref
		Storing Strength in RahulWeapon[IND][DEFP]
		*/

		PlayerPrefs.SetInt("RahulWeapon"+ind,defP);
	}
	public void refreshWeaponDisplay(int ind){
		grc.weaponControlScript.showWeapons();
		searchPossibleAttacks(ind);
	}
	private void searchPossibleAttacks(int ind){
		List<Point> list=new List<Point>();
		Dev.log(Tag.PlayerAttack,"Its Here : "+attackMoveT[ind].x+" : "+attackMoveT[ind].y);
		
		if(grc.weaponControlScript!=null)
			grc.weaponControlScript.callToAddWeapon(playerProp[ind], ind);
		else
			Dev.log(Tag.PlayerAttack,"WeaponCS is not Initialised");

		PowerStruct power=getDefaultPower(ind);

		if(attackMoveT[ind].x==-1 || attackMoveT[ind].y==-1)
			selectScript.showSelectedTiles(list,BoardConstants.Select);
		else{
			Point p;p.x=attackMoveT[ind].x;p.y=attackMoveT[ind].y;
			selectScript.showSelectedTiles(p,BoardConstants.Select);
		}
		listPossibleMoves=grc.weaponControlScript.getAttackList(players[ind], power, attackMoveT[ind]);
		selectScript.addSelectedTiles(listPossibleMoves,BoardConstants.Attack);
	}
	private void searchPossibleMoves(int ind, bool move, Point pt){
	//private void searchPossibleMoves(int ind, bool move){
		isAttack=!move;
		Point tempThresh=new Point();
		if(!move){
			grc.weaponControlScript.showWeapons();
			searchPossibleAttacks(ind);
			return;
		}
		grc.weaponControlScript.hideWeapons();
		int speed=playerProp[ind].speed;
		List<Point> list=new List<Point>();
		int x=backUpMoves[ind].x, y=backUpMoves[ind].y;
		if(applyeRestrictions){
			for(int i=x-speed;i<=x+speed;i++){
				for(int j=y-speed;j<=y+speed;j++){
					if(i<0 || j<0 || i>=GameContants.sizeOfBoardX || j>=GameContants.sizeOfBoardY)
						continue;
					Point p;
					p.x=i;
					p.y=j;
					if((i==x && j==y) || (i==pt.x&& j==pt.y)){
						tempThresh=p;
						continue;
					}
					
					list.Add(p);
				}
			}
		}else{
			x=players[ind].x;
			y=players[ind].y;
			//TODO Vary top as needed in the later stage
			int bot=0, top=GameContants.sizeOfBoardY/5;
			if(!isServer){
				top=GameContants.sizeOfBoardY;
				bot=top-GameContants.sizeOfBoardY/5;
			}
			for(int i=bot;i<top;i++){
				for(int j=0;j<GameContants.sizeOfBoardX;j++){
					Point p;
					p.x=j;
					p.y=i;
					if((j==x && i==y) || (j==pt.x && i==pt.y)){
						tempThresh=p;
						continue;
					}
					
					list.Add(p);
				}
			}
		}
		selectScript.addSelectedTiles(list,BoardConstants.Move);
		listPossibleMoves=list;
		listPossibleMoves.Add(tempThresh);
	}
	private int checKThePlayerAt(Point p){
		for(int i=0;i<players.Length;i++){
			if(p.x==players[i].x && p.y==players[i].y)
				return i;
		}
		return -1;
	}
	public void updateCameraPositionAndVariable(bool isServer, MyPlayerScript obj, TimeTracker track, PlayerControlScript[] playerC){
		this.isServer=isServer;
		if(!isServer){
			//TODO change the location of the camera
			cam.transform.position=cam.transform.position-2*(new Vector3(0,0,cam.transform.position.z));
			cam.transform.LookAt(transform.position);
		}
		if(inputManager==null)
			inputManager=GameRunningConstants.getInstance().inputManager;
		inputManager.setMyPlayerScript(obj,track,this);
		myPlayerScript=obj;
		playerControls=playerC;
		players=myPlayerScript.getPlayerDetails();
		moves=myPlayerScript.movesList;
		backUpMoves=new PlayerDetails[players.Length];
		attackMoveT=new Moves[players.Length];
		for(int i=0;i<players.Length;i++){
			attackMoveT[i].ind=(short)i;
			attackMoveT[i].attackDef="";
			attackMoveT[i].x=-1;
			attackMoveT[i].y=-1;
		}
		prepareForFirstMove();
	}
	public void generateMoves(){
		//DONE Write a script to generate Moves
		for(int i=0;i<players.Length;i++){
			playerControls[i].playerIsAtOriginal();
			if(!compareTwoPlayerDetails(players[i],backUpMoves[i])){
				Moves m;
				m.ind=(short)i;
				m.x=(short)players[i].x;
				m.y=(short)players[i].y;
				m.attackDef="";
				moves.Add(m);
			}
		}
		for(int i=0;i<attackMoveT.Length;i++){
			if(attackMoveT[i].x!=-1 && attackMoveT[i].y!=-1){
				moves.Add(attackMoveT[i]);
				attackMoveT[i].x=-1;
				attackMoveT[i].y=-1;
			}
		}
	}
	public void waitForOtherPlayers(){
		isClicksActive=false;
	}
	public bool getisClickActive(){
		return isClicksActive;
	}
	public void prepareForFirstMove(){
		isClicksActive=true;
		applyeRestrictions=false;
		for(int i=0;i<backUpMoves.Length;i++){
			backUpMoves[i]=players[i];
		}
	}
	public void prepareForNextMove(){
		isClicksActive=true;
		applyeRestrictions=true;
		for(int i=0;i<backUpMoves.Length;i++){
			backUpMoves[i]=players[i];
		}
	}
	public void disableClicksforX(){
		Dev.log(Tag.GameMoveListener,"DeActivate");
		StartCoroutine(stopClicks());
	}
	private IEnumerator stopClicks(){
		if(!isClicksActive)
			yield break;
		trackClicks=false;
		isClicksActive=false;
		yield return new WaitForSeconds(0.2f);
		isClicksActive=true;
	}
	public void Move(Vector2 v){
		if(selectedPlayerInd==-1)
			return;
		int i=selectedPlayerInd;
		Point p=new Point();
		p.x=players[i].x;
		p.y=players[i].y;
		if(isServer){
			p.x+=(int)v.x;
			p.y+=(int)v.y;
		}else{
			p.x-=(int)v.x;
			p.y-=(int)v.y;
		}
		if(isPossibleMove(p)){
			myBoard[players[i].x,players[i].y]=TypeO.None;
			players[i].x=(short)p.x;
			players[i].y=(short)p.y;
			myBoard[p.x,p.y]=TypeO.MyPlayer;
			changePlayerIdentityMaterial(i);
		}
	}
	private void changePlayerIdentityMaterial(int i){
		if(players[i].x==backUpMoves[i].x && players[i].y==backUpMoves[i].y){
			playerControls[i].playerIsAtOriginal();
		}else{
			playerControls[i].playerHasMoved();
		}
	}
	public void deselectAllPlayers(){
		Dev.log(Tag.PlayerSelect,"Disselecting All the Players");
		selectedPlayerInd=-1;
		List<Point> p=new List<Point>();
		selectScript.showSelectedTiles(p,BoardConstants.Select);
	}
}
