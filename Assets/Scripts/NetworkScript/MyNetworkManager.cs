using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class MyNetworkManager : NetworkManager {

	//TODO Restrict Adding more than one Client
	[SerializeField]
	private NetworkDisc networkDisc;
	public GameObject mainMenu, searchHosts, productStore;
	public GameObject  buttonPrefab;
	public RectTransform panelToAdd;


	// public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId)
	// {
	// 	base.OnServerAddPlayer (conn, playerControllerId);
	// 	//GameObject player=(GameObject)GameObject.Instantiate(Resources.Load("Test"));
	// 	//NetworkServer.AddPlayerForConnection (conn, player, playerControllerId);
	// }
	// public override void OnStartServer ()
	// {
	// 	onlineScene = "Level1";
	// 	base.OnStartServer ();
	// }

	public void StartAsHost(){
		NetworkManager.singleton.networkPort=1908;
		NetworkManager.singleton.StartHost();
		networkDisc.broadcastData="My Game";
		networkDisc.Initialize();
		networkDisc.broadcastData="My Game";
		networkDisc.StartAsServer();
	}

	public void StartAsClient(string address){
		NetworkManager.singleton.networkPort=1908;
		NetworkManager.singleton.networkAddress=address;
		networkDisc.StopBroadcast();
		NetworkManager.singleton.StartClient();
	}

	public override void OnServerConnect(NetworkConnection conn){
		base.OnServerConnect(conn);
		networkDisc.StopBroadcast();
	}
	public void SearchForHosts(){
		networkDisc.Initialize();
		networkDisc.StartAsClient();
		//mainMenu.SetActive(false);
		Camera.main.transform.LookAt(mainMenu.transform);
		//searchHosts.SetActive(true);
	}
	public void AddButtons(IPS x){
		GameObject goButton = (GameObject)Instantiate(buttonPrefab);
		goButton.transform.SetParent(panelToAdd, false);
		goButton.transform.localScale = new Vector3(1, 1, 1);
		Text t= goButton.transform.FindChild("Text").GetComponent<Text>();
		t.text=x.name+" ("+x.ip+")";

		Button tempButton = goButton.GetComponent<Button>();

		tempButton.onClick.AddListener(() => onClickButton(x.ip));
	}
	public void onClickButton(string address){
		StartAsClient(address);
	}
}

