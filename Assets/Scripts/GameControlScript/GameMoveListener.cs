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
	private float orthoZoomSpeed=0.5f, perspectiveZoomSpeed=0.5f;
	private Camera cam;
	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.touchCount==1){
			Debug.Log("Touch Count : "+Input.touchCount);
			Ray ray=Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			RaycastHit hit;
			//TODO Put a layer mask on this.
			if(Physics.Raycast(ray,out hit,100)){
				boxX=(int)Mathf.Floor(hit.point.x)+offsetHitX;
				boxY=(int)Mathf.Floor(hit.point.z)+offsetHitY;
				Debug.Log(hit.point+" : "+boxX+" : "+boxY);
			}
		}else if (Input.touchCount == 2){
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
        }else if(CrossPlatformInputManager.GetButtonDown("Fire1")){
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
