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
	private int boxX,boxY;
	private MyPlayerScript myPlayerScript;
	private PlayerDetails[] players;
	private List<Moves> moves;
	private float orthoZoomSpeed=0.5f, perspectiveZoomSpeed=0.5f, camPanSpeed=1f, defaultFeildOfView;
	private Vector3 defaultCamVector;	
	private Camera cam;
	[SerializeField]
	private CheckSelectScript selectScript;
	private bool selectPlayer=true;
	private int selectedPlayerInd=-1;
	// Use this for initialization
	void Start () {
		cam = Camera.main;
		defaultCamVector=cam.transform.position;
		defaultFeildOfView=cam.fieldOfView;
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

	// Update is called once per frame
	void Update () {
		//TODO Double Tap for Attack Sequence
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
			if(Mathf.Abs(cam.transform.position.x)> GameContants.sizeOfBoardX*GameContants.boxSize/2 && Mathf.Abs(cam.transform.position.x+touchDeltaPosition.x)>Mathf.Abs(cam.transform.position.x))
				touchDeltaPosition.x=0;
			//TODO Take care of this constant value if ever camera position is changed
			if(Mathf.Abs(cam.transform.position.z)> 33*GameContants.boxSize && Mathf.Abs(cam.transform.position.z+touchDeltaPosition.y)>Mathf.Abs(cam.transform.position.z))
				touchDeltaPosition.y=0;
			cam.transform.Translate(-touchDeltaPosition.x * camPanSpeed, 0, -touchDeltaPosition.y * camPanSpeed, Space.World);
        }else if(Input.touchCount==1 && Input.GetTouch(0).phase == TouchPhase.Began){
			Debug.Log("Touch Count : "+Input.touchCount);
			Ray ray=Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			performActionOnHit(ray, true);
		}else if (Input.touchCount == 2){
			detectZoom();
        }else if(CrossPlatformInputManager.GetButtonDown("Fire1")){
			Ray ray=Camera.main.ScreenPointToRay(CrossPlatformInputManager.mousePosition);
			performActionOnHit(ray, true);
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
			if(hit.collider.gameObject.layer==LayerMask.NameToLayer("CheckBoard")){
				int k=checKThePlayerAt(p);
				if(k>=0)
					selectPlayer=true;
				if(selectPlayer){
					selectedPlayerInd=k;
					if(selectedPlayerInd>=0){
						selectScript.showSelectedTiles(p,0);
						selectPlayer=false;
						searchPossibleMoves(selectedPlayerInd,true);
					}
				}else{
					players[selectedPlayerInd].x=(short)p.x;
					players[selectedPlayerInd].y=(short)p.y;
					selectPlayer=true;
				}
			}else if(hit.collider.gameObject.layer==LayerMask.NameToLayer("PlayerObjects")){
				string name = hit.collider.gameObject.name;
				int k=name.LastIndexOf('m')+1;
				int ind= Int32.Parse(name.Substring(k));
				Debug.Log("Selected Player : "+ind);
				p.x=players[ind].x;
				p.y=players[ind].y;
				selectedPlayerInd=ind;
				selectScript.showSelectedTiles(p,0);
				selectPlayer=false;
				searchPossibleMoves(selectedPlayerInd,true);
			}
		}
	}
	private void searchPossibleMoves(int ind, bool move){

	}
	private int checKThePlayerAt(Point p){
		for(int i=0;i<players.Length;i++){
			if(p.x==players[i].x && p.y==players[i].y)
				return i;
		}
		return -1;
	}
	public void updateCameraPositionAndVariable(bool isServer, MyPlayerScript obj){
		if(!isServer){
			//TODO change the location of the camera
			cam.transform.position=cam.transform.position-2*(new Vector3(0,0,cam.transform.position.z));
		}
		myPlayerScript=obj;
		players=myPlayerScript.getPlayerDetails();
		moves=myPlayerScript.movesList;
	}
}
