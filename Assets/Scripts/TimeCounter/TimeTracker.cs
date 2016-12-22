using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MasterOfBattles;

public class TimeTracker : NetworkBehaviour {

	[SyncVar]
	public float timeleft;

	[SyncVar]
	public bool playerCountDownServer=false;
	[SyncVar]
	public bool playerCountDownClient=false;

	private GameObject serverPLayer, clientPlayer;
	private MyPlayerScript serverPLayerScript, clientPlayerScript;
	
	// Use this for initialization
	void Start () {
		initiateMyGamePlayers();
	}
	
	private void initiateMyGamePlayers(){
		if(serverPLayer==null){
			serverPLayer=GameObject.Find("ServerPlayer");
			serverPLayerScript=serverPLayer.GetComponent<MyPlayerScript>();
		}if(clientPlayer==null){
			clientPlayer=GameObject.Find("ClientPlayer");
			clientPlayerScript=clientPlayer.GetComponent<MyPlayerScript>();
		}
	}
	
	[Command]
	private void CmdCountDownBegins(){

	}

	[Command]
	private void CmdchangeClientCountDown(bool b){
		playerCountDownClient=b;
	}
	[Command]
	private void CmdreinitCount(){
		timeleft=-1;
		playerCountDownClient=false;
		playerCountDownServer=false;
	}
	private void sendMoves(){
		serverPLayerScript.sendMoves();
		clientPlayerScript.sendMoves();
		CmdreinitCount();
	}
	private void calculateTimeActions(){
		if(playerCountDownClient && playerCountDownServer){
				sendMoves();
		}
		if(isServer){
			if(!playerCountDownClient && !playerCountDownServer){
				timeleft=-1;
			}else if(!playerCountDownClient && playerCountDownServer){
				if(timeleft==-1){
					timeleft=GameContants.timeConstant;
				}else{
					timeleft-=Time.deltaTime;
				}
			}else if(playerCountDownClient && !playerCountDownServer){
				if(timeleft==-1){
					timeleft=GameContants.timeConstant;
				}else{
					timeleft-=Time.deltaTime;
				}
			}
		}
	}
	void Update () {
		initiateMyGamePlayers();

		calculateTimeActions();
		if(timeleft!=-1){
			if(timeleft<=0){
				sendMoves();
			}
			else{
				//TODO Show the Canvas with time Left Bar and a cancel button too
			}
		}else{
			//TODO Remove the time canvas and cancel button. Add the Accept Button
		}
	}
	public void clickedOnCancel(){
		if(!isServer)
			CmdchangeClientCountDown(false);
		else
			playerCountDownServer=false;
	}
	public void clickedOnAccept(){
		if(!isServer)
			CmdchangeClientCountDown(true);
		else
			playerCountDownServer=true;
	}
}
