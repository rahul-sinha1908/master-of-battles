using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using MasterOfBattles;

public class GameMoveListener : MonoBehaviour {

	private int offsetHitX=GameContants.sizeOfBoardX/2;
	private int offsetHitY=GameContants.sizeOfBoardY/2;
	[SerializeField]
	private LayerMask mask;
	private bool isServer;
	private int boxX,boxY;
	private MyPlayerScript myPlayerScript;
	private PlayerDetails[] players, backUpMoves;
	private List<Moves> moves;
	private float orthoZoomSpeed=0.5f, perspectiveZoomSpeed=0.5f, camPanSpeed=1f, defaultFeildOfView;
	private Vector3 defaultCamVector;	
	private Camera cam;
	private bool isClicksActive, applyeRestrictions;
	private bool trackClicks=true;
	private Vector2 trackClickVect;
	private float trackClickCount=0f;
	[SerializeField]
	private CheckSelectScript selectScript;
	private bool selectPlayer=true;
	private int selectedPlayerInd=-1;
	private float doubleClickTimeLimit=0.3f;
	// Use this for initialization
	void Start () {
		cam = Camera.main;
		defaultCamVector=cam.transform.position;
		defaultFeildOfView=cam.fieldOfView;

		trackClicks=false;
		isClicksActive=true;
	}
	
	private void detectZoom(){
		Debug.Log("Touch Count : "+Input.touchCount);
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
		float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

		// If the camera is orthographic...
		if (cam.orthographic)
		{
			// ... change the orthographic size based on the change in distance between the touches.
			cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

			// Make sure the orthographic size never drops below zero.
			cam.orthographicSize = Mathf.Max(cam.orthographicSize, 0.1f);
		}
		else
		{
			// Otherwise change the field of view based on the change in distance between the touches.
			cam.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

			// Clamp the field of view to make sure it's between 0 and 180.
			cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 0.1f, 179.9f);
		}

	}

	private bool compareTwoPlayerDetails(PlayerDetails a, PlayerDetails b){
		if(a.x==b.x && a.y==b.y)
			return true;
		return false;
	}
	private void SingleClick(Vector2 vect)
	{	
		Debug.Log("Single Click");
		Ray ray;
		ray=Camera.main.ScreenPointToRay(vect);
		performActionOnHit(ray, true);
	}
	private void DoubleClick(Vector2 vect)
	{
		Debug.Log("Double Click");
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
			else if(CrossPlatformInputManager.GetButtonDown("Fire1")){
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
				if(Vector2.Distance(trackClickVect, Input.GetTouch(0).position)<5)
					DoubleClick(trackClickVect);
				trackClicks=false;
			}else if(Input.touchCount==1 && Input.GetTouch(0).phase==TouchPhase.Moved){
				trackClicks=false;
			}
			else if(CrossPlatformInputManager.GetButtonDown("Fire1"))
			{
				//Debug.LogError("It Entered Here3 : "+CrossPlatformInputManager.GetButtonDown("Fire1"));
				if(Vector2.Distance(trackClickVect, Input.mousePosition)<5)
					DoubleClick(trackClickVect);
				trackClicks=false;
			}else if(CrossPlatformInputManager.GetButton("Fire1")){
				if(Vector2.Distance(trackClickVect, Input.mousePosition)>5){
					trackClicks=false;
				}
			}
			trackClickCount += Time.deltaTime;
		}
	}
	// Update is called once per frame
	void Update () {
		checkClickListener();
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
			if(Mathf.Abs(cam.transform.position.x)> GameContants.sizeOfBoardX*GameContants.boxSize/2 && Mathf.Abs(cam.transform.position.x-touchDeltaPosition.x)>Mathf.Abs(cam.transform.position.x))
				touchDeltaPosition.x=0;
			//TODO Take care of this constant value if ever camera position is changed
			if(Mathf.Abs(cam.transform.position.z)> 33*GameContants.boxSize && Mathf.Abs(cam.transform.position.z-touchDeltaPosition.y)>Mathf.Abs(cam.transform.position.z))
				touchDeltaPosition.y=0;
			cam.transform.Translate(-touchDeltaPosition.x * camPanSpeed, 0, -touchDeltaPosition.y * camPanSpeed, Space.World);
        }else if (Input.touchCount == 2){
			detectZoom();
        }
	}
	private void setDefaultCameraPostion(){
		cam.transform.position=defaultCamVector;
		cam.transform.LookAt(transform.position);
		cam.fieldOfView=defaultFeildOfView;
	}
	private void performActionOnHit(Ray ray, bool move){
		RaycastHit hit;
		//TODO Put a layer mask on this.
		if(Physics.Raycast(ray,out hit,10000,mask)){
			//DONE Divide this by a constant if you want to increase the area of the play
			boxX=(int)Mathf.Floor(hit.point.x/GameContants.boxSize)+offsetHitX;
			boxY=(int)Mathf.Floor(hit.point.z/GameContants.boxSize)+offsetHitY;
			Debug.Log(hit.point+" : "+boxX+" : "+boxY);
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
						selectScript.showSelectedTiles(p,BoardConstants.Select);
						selectPlayer=false;
						searchPossibleMoves(selectedPlayerInd,true);
					}
				}else{
					if(move){
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
						players[selectedPlayerInd].x=(short)p.x;
						players[selectedPlayerInd].y=(short)p.y;
					}
					//selectPlayer=true;
				}
			}else if(hit.collider.gameObject.layer==LayerMask.NameToLayer("PlayerObjects")){
				string name = hit.collider.gameObject.name;
				int k=name.LastIndexOf('m')+1;
				int ind= Int32.Parse(name.Substring(k));
				Debug.Log("Selected Player : "+ind);
				p.x=backUpMoves[ind].x;
				p.y=backUpMoves[ind].y;
				selectedPlayerInd=ind;
				selectScript.showSelectedTiles(p,BoardConstants.Select);
				selectPlayer=false;
				searchPossibleMoves(selectedPlayerInd,true);
			}
		}
	}
	private void searchPossibleMoves(int ind, bool move){
		List<Point> list=new List<Point>();
		int x=backUpMoves[ind].x, y=backUpMoves[ind].y;
		if(applyeRestrictions){
			for(int i=x-1;i<=x+1;i++){
				for(int j=y-1;j<=y+1;j++){
					if(i<0 || j<0 || i>=GameContants.sizeOfBoardX || y>=GameContants.sizeOfBoardY)
						continue;
					if(i==x && j==y)
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
					if(i==y && j==x)
						continue;
					Point p;
					p.x=j;
					p.y=i;
					list.Add(p);
				}
			}
		}
		selectScript.addSelectedTiles(list,BoardConstants.Move);
	}
	private int checKThePlayerAt(Point p){
		for(int i=0;i<players.Length;i++){
			if(p.x==players[i].x && p.y==players[i].y)
				return i;
		}
		return -1;
	}
	public void updateCameraPositionAndVariable(bool isServer, MyPlayerScript obj){
		this.isServer=isServer;
		if(!isServer){
			//TODO change the location of the camera
			cam.transform.position=cam.transform.position-2*(new Vector3(0,0,cam.transform.position.z));
			cam.transform.LookAt(transform.position);
		}
		myPlayerScript=obj;
		players=myPlayerScript.getPlayerDetails();
		moves=myPlayerScript.movesList;
		backUpMoves=new PlayerDetails[players.Length];
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
}
