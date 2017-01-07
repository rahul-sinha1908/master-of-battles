using System;
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
	// Use this for initialization
	void Start () {
		cam = Camera.main;
		defaultCamVector=cam.transform.position;
		defaultFeildOfView=cam.fieldOfView;

		inputManager=GameObject.Find("MyScreen").GetComponent<InputManager>();

		powerDatabase=PowersContants.getInstance();
		chess=ChessBoardFormation.getInstance();
		myBoard=chess.myBoard;
		playerProp=chess.gameFormation;
		initialiseHealtMetre();

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
		checkClickListener();
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
			touchDeltaPosition=touchDeltaPosition*PlayerPrefs.GetFloat(UserPrefs.mobPanSensitivity, 0.2f);
			Pan(touchDeltaPosition);
        }else if (Input.touchCount == 2){
			detectZoom(0);
        }else if(Input.GetAxis("Mouse ScrollWheel")!=0){
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
							p.x=backUpMoves[k].x;
							p.y=backUpMoves[k].y;
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
				p.x=backUpMoves[ind].x;
				p.y=backUpMoves[ind].y;
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
			if(applyeRestrictions && (Math.Abs(p.x-backUpMoves[s].x)>1 || Math.Abs(p.y-backUpMoves[s].y)>1))
				return;
			else if(!applyeRestrictions){
				//DONE Give the condition for restricting too much of move
				int bot=0, top=3;
				if(!isServer){
					top=GameContants.sizeOfBoardY;
					bot=top-3;
				}
				if(p.y<bot || p.y>=top)
					return;
			}
			
			TypeO type=myBoard[players[selectedPlayerInd].x,players[selectedPlayerInd].y];
			Dev.log(Tag.CheckBoard, "1 : "+type );
			myBoard[players[selectedPlayerInd].x,players[selectedPlayerInd].y]=TypeO.None;
			myBoard[p.x,p.y]=type;
			Dev.log(Tag.CheckBoard, "2 : "+myBoard[p.x,p.y]);
			players[selectedPlayerInd].x=(short)p.x;
			players[selectedPlayerInd].y=(short)p.y;
			attackMoveT[selectedPlayerInd].x=-1;
			attackMoveT[selectedPlayerInd].y=-1;

		}else{//Attack Sequence

			p.x=boxX;
			p.y=boxY;
			if(isPossibleMove(p)){
				Dev.log(Tag.PlayerAttack,"Possible Attack");
				attackMoveT[selectedPlayerInd].x=(short)p.x;
				attackMoveT[selectedPlayerInd].y=(short)p.y;
				searchPossibleAttacks(selectedPlayerInd);
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
	private bool validatePoint(Point p){
		if(p.x>=0 && p.x<GameContants.sizeOfBoardX && p.y>=0 && p.y<GameContants.sizeOfBoardY)
			return true;
		return false;
	} 
	private void searchPossibleAttacks(int ind){
		List<Point> list=new List<Point>();
		Dev.log(Tag.PlayerAttack,"Its Here : "+attackMoveT[ind].x+" : "+attackMoveT[ind].y);
		for(int i=0;i<GameContants.sizeOfBoardX;i++){
			Point p=new Point();
			p.x=i;
			p.y=players[ind].y;
			if(p.x==attackMoveT[ind].x && p.y==attackMoveT[ind].y)
				selectScript.showSelectedTiles(p,BoardConstants.Select);
			else
				list.Add(p);
			p=new Point();
			p.y=i;
			p.x=players[ind].x;
			if(p.x==attackMoveT[ind].x && p.y==attackMoveT[ind].y)
				selectScript.showSelectedTiles(p,BoardConstants.Select);
			else
				list.Add(p);
		}
		//TODO Do Somethins for diagonal Points
		int x=players[ind].x,y=players[ind].y;
		for(int i=0;;i++){
			bool b=false;
			Point p=new Point();
			p.x=x+i;
			p.y=y+i;
			if(validatePoint(p)){
				b=true;
				if(p.x==attackMoveT[ind].x && p.y==attackMoveT[ind].y)
					selectScript.showSelectedTiles(p,BoardConstants.Select);
				else
					list.Add(p);
			}
			p=new Point();
			p.x=x-i;
			p.y=y-i;
			if(validatePoint(p)){
				b=true;
				if(p.x==attackMoveT[ind].x && p.y==attackMoveT[ind].y)
					selectScript.showSelectedTiles(p,BoardConstants.Select);
				else
					list.Add(p);
			}
			p=new Point();
			p.x=x-i;
			p.y=y+i;
			if(validatePoint(p)){
				b=true;
				if(p.x==attackMoveT[ind].x && p.y==attackMoveT[ind].y)
					selectScript.showSelectedTiles(p,BoardConstants.Select);
				else
					list.Add(p);
			}
			p=new Point();
			p.x=x+i;
			p.y=y-i;
			if(validatePoint(p)){
				b=true;
				if(p.x==attackMoveT[ind].x && p.y==attackMoveT[ind].y)
					selectScript.showSelectedTiles(p,BoardConstants.Select);
				else
					list.Add(p);
			}
			if(b==false)
				break;
		}
		listPossibleMoves=list;
		selectScript.addSelectedTiles(list,BoardConstants.Attack);
	}
	private void searchPossibleMoves(int ind, bool move, Point pt){
	//private void searchPossibleMoves(int ind, bool move){
		isAttack=!move;
		if(!move){
			searchPossibleAttacks(ind);
			return;
		}
		List<Point> list=new List<Point>();
		int x=backUpMoves[ind].x, y=backUpMoves[ind].y;
		if(applyeRestrictions){
			for(int i=x-1;i<=x+1;i++){
				for(int j=y-1;j<=y+1;j++){
					if(i<0 || j<0 || i>=GameContants.sizeOfBoardX || j>=GameContants.sizeOfBoardY)
						continue;
					if((i==x && j==y) || (i==pt.x&& j==pt.y))
					//if(i==x && j==y)
						continue;
					Point p;
					p.x=i;
					p.y=j;
					list.Add(p);
				}
			}
		}else{
			x=players[ind].x;
			y=players[ind].y;
			int bot=0, top=3;
			if(!isServer){
				top=GameContants.sizeOfBoardY;
				bot=top-3;
			}
			for(int i=bot;i<top;i++){
				for(int j=0;j<GameContants.sizeOfBoardX;j++){
					if((j==x && i==y) || (j==pt.x && i==pt.y))
					//if(j==x && i==y)
						continue;
					Point p;
					p.x=j;
					p.y=i;
					list.Add(p);
				}
			}
		}
		listPossibleMoves=list;
		selectScript.addSelectedTiles(list,BoardConstants.Move);
	}
	private int checKThePlayerAt(Point p){
		for(int i=0;i<players.Length;i++){
			if(p.x==players[i].x && p.y==players[i].y)
				return i;
		}
		return -1;
	}
	public void updateCameraPositionAndVariable(bool isServer, MyPlayerScript obj, TimeTracker track){
		this.isServer=isServer;
		if(!isServer){
			//TODO change the location of the camera
			cam.transform.position=cam.transform.position-2*(new Vector3(0,0,cam.transform.position.z));
			cam.transform.LookAt(transform.position);
		}
		inputManager.setMyPlayerScript(obj,track,this);
		myPlayerScript=obj;
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
				attackMoveT[i].attackDef="1|20|20";
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
		}

	}
	public void deselectAllPlayers(){
		Dev.log(Tag.PlayerSelect,"Disselecting All the Players");
		selectedPlayerInd=-1;
		List<Point> p=new List<Point>();
		selectScript.showSelectedTiles(p,BoardConstants.Select);
	}
}
