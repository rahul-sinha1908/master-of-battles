using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MasterOfBattles;

public class TimeTracker : NetworkBehaviour {

	[SyncVar]
	public int timeleft;
	private float localTimeLeft;
	[SyncVar]
	public bool playerCountDownServer=false;
	[SyncVar]
	public bool playerCountDownClient=false;

	private GameObject serverPlayer, clientPlayer;
	private MyPlayerScript serverPlayerScript, clientPlayerScript;
	private bool dataSend=true;
	// Use this for initialization
	void Start () {
		initiateMyGamePlayers();
	}
	
	private void initiateMyGamePlayers(){
		if(serverPlayer==null){
			serverPlayer=GameObject.Find("ServerPlayer");
			if(serverPlayer!=null)
				serverPlayerScript=serverPlayer.GetComponent<MyPlayerScript>();
		}if(clientPlayer==null){
			clientPlayer=GameObject.Find("ClientPlayer");
			if(clientPlayer!=null)
				clientPlayerScript=clientPlayer.GetComponent<MyPlayerScript>();
		}
	}
	
	[Command]
	private void CmdCountDownBegins(){

	}

	
	private void sendMoves(){
		serverPlayerScript.sendMoves();
		clientPlayerScript.sendMoves();
	}
	private void calculateTimeActions(){
		if(isServer){
			if(timeleft==0){
				timeleft=-1;
				playerCountDownClient=false;
				playerCountDownServer=false;
			}
			if(!playerCountDownClient && !playerCountDownServer){
				timeleft=-1;
			}else if(!playerCountDownClient && playerCountDownServer){
				if(timeleft==-1){
					timeleft=GameContants.getInstance().timeConstant;
					localTimeLeft=timeleft;
				}else{
					localTimeLeft-=Time.deltaTime;
					timeleft=(int)Mathf.Ceil(localTimeLeft);
				}
			}else if(playerCountDownClient && !playerCountDownServer){
				if(timeleft==-1){
					timeleft=GameContants.getInstance().timeConstant;
					localTimeLeft=timeleft;
				}else{
					localTimeLeft-=Time.deltaTime;
					timeleft=(int)Mathf.Ceil(localTimeLeft);
				}
			}
		}
	}
	void Update () {
		initiateMyGamePlayers();

		calculateTimeActions();
		if(playerCountDownClient && playerCountDownServer)
			timeleft=0;
		if(timeleft!=-1){
			if(timeleft==0 && !dataSend){
				sendMoves();
				dataSend=true;
			}
			else{
				//DONE Show the Canvas with time Left Bar and a cancel button too
			}
		}else{
			dataSend=false;
			//DONE Remove the time canvas and cancel button. Add the Accept Button
		}
	}
	public void clickedOnCancel(){
		if(!isServer)
			clientPlayerScript.changeClientCountDown(false);
		else
			playerCountDownServer=false;
	}
	public void clickedOnAccept(){
		if(!isServer)
			clientPlayerScript.changeClientCountDown(true);
		else
			playerCountDownServer=true;
	}
	public void sendMovesFromTimeTracker(){
		//DONE Send Moves From TimeTracker on Click Of the Button
		if(!isServer)
			clientPlayerScript.changeClientCountDown(true);
		else
			playerCountDownServer=true;
	}
}
