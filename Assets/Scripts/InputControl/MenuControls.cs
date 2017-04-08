using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterOfBattles; 

public class MenuControls : MonoBehaviour {

	public float speed;
	public GameObject mainmenu, searchForHost, productStore, trackLoc; 
	private Camera cam;
	
	// Use this for initialization
	void Start () {
		speed=1;
		cam=Camera.main;

		trackLoc.transform.LookAt(mainmenu.transform);
		cam.transform.rotation=trackLoc.transform.rotation;
	}
	
	public void goToMainMenu(){
		trackLoc.transform.LookAt(mainmenu.transform);
	}

	public void searchForHostFunc(){
		trackLoc.transform.LookAt(searchForHost.transform);
	}

	public void goToProductStore(){
		trackLoc.transform.LookAt(productStore.transform);
	}

	// Update is called once per frame
	void Update () {
		if(trackLoc.transform.rotation !=cam.transform.rotation){
			lerpCamera();
		}
	}

	private void lerpCamera(){
		Dev.log(Tag.MenuControl, "It reached here");
		//Vector3 direction = point - transform.position;
		//Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
		cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, trackLoc.transform.rotation, speed * Time.deltaTime);
		if(Mathf.Abs(Quaternion.Angle(cam.transform.rotation, trackLoc.transform.rotation)) < 0.5f ){
			cam.transform.rotation=trackLoc.transform.rotation;
		}
	}
}
