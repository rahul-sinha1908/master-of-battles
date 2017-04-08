using System.Collections;
using System;
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
	public float[] values;
	private int currentModel;
	private GameObject currentSpawnedGO; 
	private Camera cam;
	private string ACCESS_TOKEN="", Email_ID="";
	private string serverAccessToken="";
	private string serverAddress="";
	
	// Use this for initialization
	void Start () {
		speed=1;
		cam=Camera.main;
		currentModel=0;

		trackLoc.transform.LookAt(mainmenu.transform);
		cam.transform.rotation=trackLoc.transform.rotation;
		currentSpawnedGO=GameObject.Instantiate(myModels[currentModel], spawnLocation.transform);

		//TODO initialization of server address and serverAccessToken and ACCESS_TOKEN & EmailId
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

		/*
		curl -X POST\
-H 'Content-Type:application/json'\
-H 'Authorization: Bearer ACCESS_TOKEN'\
-d '{
	"to_address":"abc@gmail.com/bitcoinaddress",
	"btcamount":"0.0023",
    }'
    https://sandbox.unocoin.co/api/v1/wallet/sendingbtc
		*/

        WWWForm form = new WWWForm();

		form.headers.Add("Content-Type", "application/json" );
		form.headers.Add("Authorization","Bearer "+ACCESS_TOKEN);
        form.AddField("to_address", serverAddress);
		form.AddField("btcamount", ""+values[currentModel]);
 
        UnityWebRequest www = UnityWebRequest.Post("https://sandbox.unocoin.co/api/v1/wallet/sendingbtc", form);
		Dev.log(Tag.Network, "Sending Datas");
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        yield return www.Send();
 
        if(www.isError) {
            Dev.log(Tag.Network,www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.downloadHandler.text);
			MessageInfo info = MessageInfo.CreateFromJSON(www.downloadHandler.text);
        }
		waitScreen.SetActive(false);
		rayCaster.enabled=true;
    }
	IEnumerator sellElementIEnum() {

		/*
		curl -X POST\
-H 'Content-Type:application/json'\
-H 'Authorization: Bearer ACCESS_TOKEN'\
-d '{
	"to_address":"abc@gmail.com/bitcoinaddress",
	"btcamount":"0.0023",
    }'
    https://sandbox.unocoin.co/api/v1/wallet/sendingbtc
		*/

        WWWForm form = new WWWForm();
        form.headers.Add("Content-Type", "application/json" );
		form.headers.Add("Authorization","Bearer "+serverAccessToken);
        form.AddField("to_address", Email_ID);
		form.AddField("btcamount", ""+values[currentModel]);
 
        UnityWebRequest www = UnityWebRequest.Post("https://sandbox.unocoin.co/api/v1/wallet/sendingbtc", form);
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        yield return www.Send();
 
        if(www.isError) {
            Debug.Log(www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.downloadHandler.text);
			MessageInfo info = MessageInfo.CreateFromJSON(www.downloadHandler.text);
        }
		waitScreen.SetActive(false);
		rayCaster.enabled=true;
    }

	IEnumerator addMoneyIEnum() {
		/*
		curl -X POST\
-H 'Content-Type:application/json'\
-H 'Authorization: Bearer ACCESS_TOKEN'\
-d '{
	"destination":"My wallet/bitcoinaddress",
	"inr":"20655",
	"total":"20893", inr+fee+tax
	"btc":"0.5",  
	"fee":"207",1% inr
	"tax":"31", 15% fee
	"rate":"41310"
    }'
    https://sandbox.unocoin.co/api/v1/trading/buyingbtc
		*/
        WWWForm form = new WWWForm();
		
		string inrStr=txt_money.text;
		int inrVal=0;
		int rate=41310;
		try{
			inrVal=Int32.Parse(inrStr);
		}catch(Exception ex){
			Dev.log(Tag.Network, ex.Data);
			waitScreen.SetActive(false);
			rayCaster.enabled=true;
			yield break;
		}

		form.headers.Add("Content-Type", "application/json" );
		form.headers.Add("Authorization","Bearer "+ACCESS_TOKEN);

		form.AddField("destination", "My wallet");
		form.AddField("inr", ""+inrVal);
		form.AddField("fee",""+(0.01*inrVal));
		form.AddField("tax",""+(0.15*0.01*inrVal));
		form.AddField("total",""+(inrVal+0.01*inrVal+0.15*0.01*inrVal));
		form.AddField("rate", ""+rate);
		form.AddField("rate", ""+(inrVal*1.0/rate));
 
        UnityWebRequest www = UnityWebRequest.Post("https://sandbox.unocoin.co/api/v1/trading/buyingbtc", form);
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        yield return www.Send();
 
        if(www.isError) {
            Debug.Log(www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.downloadHandler.text);
			MessageInfo info = MessageInfo.CreateFromJSON(www.downloadHandler.text);
        }
		waitScreen.SetActive(false);
		rayCaster.enabled=true;
    }

	IEnumerator loginIEnum() {
        WWWForm form = new WWWForm();

		Email_ID = txt_username.text;
		
        form.AddField("email", Email_ID);
		form.AddField("password", txt_password.text);
 
        UnityWebRequest www = UnityWebRequest.Post("http://www.my-server.com/myform", form);
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        yield return www.Send();
 
        if(www.isError) {
            Debug.Log(www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.downloadHandler.text);
			MessageInfo info = MessageInfo.CreateFromJSON(www.downloadHandler.text);
        }
		waitScreen.SetActive(false);
		rayCaster.enabled=true;
    }

	IEnumerator getBalanceEnum() {
		/*
		curl -X POST\
-H 'Content-Type:application/json'\
-H 'Authorization: Bearer ACCESS_TOKEN'\
    https://sandbox.unocoin.co/api/v1/wallet/bitcoinaddress
		*/

        WWWForm form = new WWWForm();

        form.headers.Add("Content-Type", "application/json" );
		form.headers.Add("Authorization","Bearer "+ACCESS_TOKEN);
 
        UnityWebRequest www = UnityWebRequest.Post("https://sandbox.unocoin.co/api/v1/wallet/bitcoinaddress", form);
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        yield return www.Send();
 
        if(www.isError) {
            Debug.Log(www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.downloadHandler.text);
			MessageInfo info = MessageInfo.CreateFromJSON(www.downloadHandler.text);
        }
		waitScreen.SetActive(false);
		rayCaster.enabled=true;
    }

	public class MessageInfo
	{
		/*
{"result":"success", "message":"Your transaction is successfull and the bitcoins are credited to recipient wallet.", "status_code":200}
		*/
		public string result;
		public string message;
		public string btc_balance, inr_balance;
		public int status_code;

		public static MessageInfo CreateFromJSON(string jsonString)
		{
			return JsonUtility.FromJson<MessageInfo>(jsonString);
		}

		// Given JSON input:
		// {"name":"Dr Charles","lives":3,"health":0.8}
		// this example will return a PlayerInfo object with
		// name == "Dr Charles", lives == 3, and health == 0.8f.
	}
}
