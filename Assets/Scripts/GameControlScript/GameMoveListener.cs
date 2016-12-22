using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using MasterOfBattles;

public class GameMoveListener : MonoBehaviour {

	private int offsetHitX=GameContants.sizeOfBoardX/2;
	private int offsetHitY=GameContants.sizeOfBoardY/2;

	private int boxX,boxY;
	private MyPlayerScript myPlayerScript;
	private PlayerDetails[] players;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(CrossPlatformInputManager.GetButtonDown("Fire1")){
			Ray ray=Camera.main.ScreenPointToRay(CrossPlatformInputManager.mousePosition);
			RaycastHit hit;
			//TODO Put a layer mask on this.
			if(Physics.Raycast(ray,out hit,100)){
				boxX=(int)Mathf.Floor(hit.point.x)+offsetHitX;
				boxY=(int)Mathf.Floor(hit.point.z)+offsetHitY;
				Debug.Log(hit.point+" : "+boxX+" : "+boxY);
			}
		}
	}
	public void updateCameraPositionAndVariable(bool isServer, MyPlayerScript obj){
		if(!isServer){
			//TODO change the location of the camera
			transform.position=transform.position-2*(new Vector3(0,0,transform.position.z));
		}
		myPlayerScript=obj;
		players=myPlayerScript.getPlayerDetails();
	}
}
