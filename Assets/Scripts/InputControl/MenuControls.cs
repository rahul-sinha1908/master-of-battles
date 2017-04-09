using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using MasterOfBattles; 

public class MenuControls : MonoBehaviour {

	public float speed;
	public GameObject mainmenu, searchForHost, productStore, trackLoc, spawnLocation, waitScreen, loginScreen, addMoneyScreen, messageScren, buyButton, sellButton;
	public InputField txt_username, txt_password, txt_money;
	public Text bitCoinsText, messageText;
	public GraphicRaycaster rayCaster;
	public GameObject[] myModels;
	public float[] values;
	private int currentModel;
	private GameObject currentSpawnedGO; 
	private Camera cam;
	private string ACCESS_TOKEN="", Email_ID="";
	private float bitCoinsLeft;
	private string serverAccessToken="";
	private string serverAddress="";
	private int rate=41310;
	
	// Use this for initialization
	void Start () {
		speed=1;
		cam=Camera.main;
		currentModel=0;

		trackLoc.transform.LookAt(mainmenu.transform);
		cam.transform.rotation=trackLoc.transform.rotation;
		currentSpawnedGO=GameObject.Instantiate(myModels[currentModel], spawnLocation.transform);
		decide();

		//TODO initialization of server address and serverAccessToken and ACCESS_TOKEN & EmailId
		serverAccessToken="d407c38d28087b93513fbff2816a64a25b9bafa4";
		serverAddress="jaysinha.bestindwrld@gmail.com";

		bitCoinsLeft=-1;
		refreshBitCoins(true);
	}
	public void decide(){
		int ch = PlayerPrefs.GetInt("P"+currentModel, 0);
		if(ch==0){
			buyButton.SetActive(true);
			sellButton.SetActive(false);
		}else{
			buyButton.SetActive(false);
			sellButton.SetActive(true);
		}
	}
	public void refreshBitCoins(bool bg){
		StartCoroutine(getBalanceEnum(bg));
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
		decide();
	}
	public void changeModelLeft(){
		//StartCoroutine(Upload());
		if(currentModel==0)
			return;
		
		currentModel--;
		Destroy(currentSpawnedGO);
		currentSpawnedGO=GameObject.Instantiate(myModels[currentModel], spawnLocation.transform);
		decide();
	}

	public void buyElement(){
		StartCoroutine(buyElementIEnum());
		//StartCoroutine(getBalanceEnum());
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
		String temp="";
		if(PlayerPrefs.GetInt("P"+currentModel, 0)==0)
			temp="Buying";
		else
			temp="Selling";
		bitCoinsText.text="BitCoins : "+bitCoinsLeft+"\n"+temp+" Cost : "+values[currentModel];
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
		//ACCESS_TOKEN="c6ce54622ea8b8ea9a024deb44d1fbdcb00bea29";
		if(ACCESS_TOKEN==""){
			displayMessage("First Log in");
			yield break;
		}
		
        WWWForm form = new WWWForm();
		Dictionary<string, string> head=new Dictionary<string, string>();
		head.Add("Content-Type", "application/x-www-form-urlencoded" );
		head.Add("Authorization","Bearer "+ACCESS_TOKEN);
        form.AddField("to_address", serverAddress);
		form.AddField("btcamount", ""+values[currentModel]);
		//form.AddField("btcamount", "0.00230000");
 
		// string dataS="{\"to_address\":\""+serverAddress+"\",\"btcamount\":\""+values[currentModel]+"\",}";
        //UnityWebRequest www = UnityWebRequest.Post("https://sandbox.unocoin.co/api/v1/wallet/sendingbtc", form);
		WWW www = new WWW("https://sandbox.unocoin.co/api/v1/wallet/sendingbtc", form.data, head);
		// WWW www = new WWW("https://sandbox.unocoin.co/api/v1/wallet/sendingbtc", System.Text.Encoding.ASCII.GetBytes(dataS.ToCharArray()), head);
		Dev.log(Tag.Network, "Sending Buying Datas : ");
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        //yield return www.Send();
		yield return www;
 
        if(www.error!=null) {
            Dev.log(Tag.Network,www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.text);
			MessageInfo info = MessageInfo.CreateFromJSON(www.text);
			if(info.result=="success"){
				PlayerPrefs.SetInt("P"+currentModel, 1);
				refreshBitCoins(false);
				decide();
				yield break;
			}else{
				displayMessage("Error in fetching data");
			}
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

		if(ACCESS_TOKEN==""){
			displayMessage("First Log in");
			yield break;
		}

        WWWForm form = new WWWForm();
		Dictionary<string, string> head=new Dictionary<string, string>();
        head.Add("Content-Type", "application/x-www-form-urlencoded" );
		head.Add("Authorization","Bearer "+serverAccessToken);
        form.AddField("to_address", Email_ID);
		form.AddField("btcamount", ""+values[currentModel]);
 
        //UnityWebRequest www = UnityWebRequest.Post("https://sandbox.unocoin.co/api/v1/wallet/sendingbtc", form);
		WWW www = new WWW("https://sandbox.unocoin.co/api/v1/wallet/sendingbtc", form.data, head);
		Dev.log(Tag.Network, "Sending Selling Datas");
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        //yield return www.Send();
		yield return www;
 
        if(www.error!=null) {
            Dev.log(Tag.Network,www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.text);
			MessageInfo info = MessageInfo.CreateFromJSON(www.text);
			if(info.result=="success"){
				PlayerPrefs.SetInt("P"+currentModel, 0);
				refreshBitCoins(false);
				decide();
				yield break;
			}else{
				displayMessage("Error in fetching data");
			}
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
		if(ACCESS_TOKEN==""){
			displayMessage("First Log in");
			yield break;
		}
			
        WWWForm form = new WWWForm();
		
		string inrStr=txt_money.text;
		int inrVal=0;
		try{
			inrVal=Int32.Parse(inrStr);
		}catch(Exception ex){
			Dev.log(Tag.Network, ex.Data);
			waitScreen.SetActive(false);
			rayCaster.enabled=true;
			yield break;
		}

		Dictionary<string, string> head=new Dictionary<string, string>();
		head.Add("Content-Type", "application/json" );
		head.Add("Authorization","Bearer "+ACCESS_TOKEN);

		form.AddField("destination", "My wallet");
		form.AddField("inr", ""+inrVal);
		form.AddField("fee",""+(0.01*inrVal));
		form.AddField("tax",""+(0.15*0.01*inrVal));
		form.AddField("total",""+(inrVal+0.01*inrVal+0.15*0.01*inrVal));
		form.AddField("rate", ""+rate);
		form.AddField("btc", ""+(inrVal*1.0/rate));
 
        //UnityWebRequest www = UnityWebRequest.Post("https://sandbox.unocoin.co/api/v1/trading/buyingbtc", form);
		WWW www = new WWW("https://sandbox.unocoin.co/api/v1/trading/buyingbtc", form.data, head);
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        //yield return www.Send();
		yield return www;
 
        if(www.error!=null) {
            Dev.log(Tag.Network,www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.text);
			MessageInfo info = MessageInfo.CreateFromJSON(www.text);
			if(info.result=="success"){
				//PlayerPrefs.SetInt("P"+currentModel, 0);
				refreshBitCoins(false);
				yield break;
			}else{
				displayMessage("Error in fetching data");
			}
        }
		waitScreen.SetActive(false);
		rayCaster.enabled=true;
    }

	IEnumerator loginIEnum() {
        WWWForm form = new WWWForm();

		Email_ID = txt_username.text;
		
        form.AddField("email", Email_ID);
		form.AddField("password", txt_password.text);
 
        //UnityWebRequest www = UnityWebRequest.Post("https://cryptothon-razor08.c9users.io/login", form);
		WWW www=new WWW("https://35.160.197.192:3000/login", form.data, new Dictionary<string, string>());
		rayCaster.enabled=false;
		waitScreen.SetActive(true);
        //yield return www.Send();
		yield return www;
		
		if(www.error==null){
			Dev.log(Tag.Network, "Form upload complete! "+www.text);
			MessageInfo info = MessageInfo.CreateFromJSON(www.text);
			if(info.result=="success"){
				ACCESS_TOKEN=info.message;
				refreshBitCoins(false);
				yield break;
			}else{
				displayMessage("Error in fetching Datas");
			}
		}else{
			Dev.log(Tag.Network,www.error);
			displayMessage(www.error);
		}
        // if(www.isError) {
        //     Dev.log(Tag.Network,www.error);
        // }
        // else {
        //     Dev.log(Tag.Network, "Form upload complete! "+www.downloadHandler.text);
		// 	MessageInfo info = MessageInfo.CreateFromJSON(www.downloadHandler.text);
        // }
		waitScreen.SetActive(false);
		rayCaster.enabled=true;
    }
	private void displayMessage(string s){
		//TODO Show popup for any errors.
		StartCoroutine(displayMessageEnum(s));
	}
	private IEnumerator displayMessageEnum(string s){
		messageText.text=s;
		messageScren.SetActive(true);
		yield return new WaitForSeconds(1);
		messageScren.SetActive(false);
	}
	IEnumerator getBalanceEnum(bool bg) {
		/*
		curl -X POST\
-H 'Content-Type:application/json'\
-H 'Authorization: Bearer ACCESS_TOKEN'\
    https://sandbox.unocoin.co/api/v1/wallet/bitcoinaddress
		*/
		if(ACCESS_TOKEN==""){
			displayMessage("First Log in");
			yield break;
		}
        WWWForm form = new WWWForm();

		// if(form.headers.ContainsKey("Content-Type"))
		// 	form.headers.Remove("Content-Type");
        //form.headers.Add("Content-Type", "application/json" );
		Dev.log(Tag.Network, form.headers.Values.ToString());
		//ACCESS_TOKEN="d407c38d28087b93513fbff2816a64a25b9bafa4";
		//form.headers.Add("Authorization","Bearer "+ACCESS_TOKEN);
		//form.AddField("Authorization","Bearer "+ACCESS_TOKEN);

		Dictionary<string, string> here=new Dictionary<string, string>();
		here["Content-Type"]="application/json";
		here["Authorization"]="Bearer "+ACCESS_TOKEN;

 
        //UnityWebRequest www = UnityWebRequest.Post("https://sandbox.unocoin.co/api/v1/wallet/bitcoinaddress", form);
		WWW www = new WWW("https://sandbox.unocoin.co/api/v1/wallet/bitcoinaddress", null, here);
		if(!bg){
			rayCaster.enabled=false;
			waitScreen.SetActive(true);
		}
        //yield return www.Send();
		yield return www;
 
        if(www.error!=null) {
            Dev.log(Tag.Network,www.error);
			displayMessage(www.error);
        }
        else {
            Dev.log(Tag.Network, "Form upload complete! "+www.text);
			MessageInfo info = MessageInfo.CreateFromJSON(www.text);
			Dev.log(Tag.Network, ""+info.btc_balance);
			if(info.result!="error"){
				bitCoinsLeft=float.Parse(info.btc_balance);
			}else{
				displayMessage("Error in Fetching datas");
			}
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
