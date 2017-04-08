using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using MasterOfBattles; 

public class MenuControls : MonoBehaviour {

	public float speed;
	public GameObject mainmenu, searchForHost, productStore, trackLoc, spawnLocation, waitScreen, loginScreen, addMoneyScreen;
	public InputField txt_username, txt_password, txt_money;
	public GraphicRaycaster rayCaster;
	public GameObject[] myModels;
	private int currentModel;
	private GameObject currentSpawnedGO; 
	private Camera cam;
	
	// Use this for initialization
	void Start () {
		speed=1;
		cam=Camera.main;
		currentModel=0;

		trackLoc.transform.LookAt(mainmenu.transform);
		cam.transform.rotation=trackLoc.transform.rotation;
		currentSpawnedGO=GameObject.Instantiate(myModels[currentModel], spawnLocation.transform);
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

	public void changeModelRight(){
		if(currentModel==myModels.Length-1)
			return;
		currentModel++;
		Destroy(currentSpawnedGO);
		currentSpawnedGO=GameObject.Instantiate(myModels[currentModel], spawnLocation.transform);
	}
	public void changeModelLeft(){
		//StartCoroutine(Upload());
		if(currentModel==0)
			return;
		
		currentModel--;
		Destroy(currentSpawnedGO);
		currentSpawnedGO=GameObject.Instantiate(myModels[currentModel], spawnLocation.transform);
	}

	public void buyElement(){
		StartCoroutine(buyElementIEnum());
	}

	public void sellElement(){
		StartCoroutine(sellElementIEnum());
	}

	public void addMoney(){
		resetScreen();
		addMoneyScreen.SetActive(true);
	}
	public void finalAddMoney(){
		resetScreen();
		StartCoroutine(addMoneyIEnum());
	}

	public void loginCall(){
		resetScreen();
		loginScreen.SetActive(true);
	}

	public void finalLoginCall(){
		resetScreen();
		StartCoroutine(loginIEnum());
	}
	public void resetScreen(){
		loginScreen.SetActive(false);
		addMoneyScreen.SetActive(false);
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


	IEnumerator buyElementIEnum() {
        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");
 
        UnityWebRequest www = UnityWebRequest.Post("http://www.my-server.com/myform", form);
		Dev.log(Tag.Network, "Sending Datas");
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        yield return www.Send();
 
        if(www.isError) {
            Dev.log(Tag.Network,www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.downloadHandler.text);
        }
		waitScreen.SetActive(false);
		rayCaster.enabled=true;
    }
	IEnumerator sellElementIEnum() {
        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");
 
        UnityWebRequest www = UnityWebRequest.Post("http://www.my-server.com/myform", form);
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        yield return www.Send();
 
        if(www.isError) {
            Debug.Log(www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.downloadHandler.text);
        }
		waitScreen.SetActive(false);
		rayCaster.enabled=true;
    }

	IEnumerator addMoneyIEnum() {
        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");
 
        UnityWebRequest www = UnityWebRequest.Post("http://www.my-server.com/myform", form);
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        yield return www.Send();
 
        if(www.isError) {
            Debug.Log(www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.downloadHandler.text);
        }
		waitScreen.SetActive(false);
		rayCaster.enabled=true;
    }

	IEnumerator loginIEnum() {
        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");
 
        UnityWebRequest www = UnityWebRequest.Post("http://www.my-server.com/myform", form);
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        yield return www.Send();
 
        if(www.isError) {
            Debug.Log(www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.downloadHandler.text);
        }
		waitScreen.SetActive(false);
		rayCaster.enabled=true;
    }
}
