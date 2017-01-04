﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class InputManager: MonoBehaviour{
	[SerializeField]
	private GameObject MobilePlatform;
	[SerializeField]
	private GameObject OtherPlatform;
	
	[SerializeField]
	private GameObject MultipleSelect;
	[SerializeField]
	private GameObject MovePlayer;
	[SerializeField]
	private GameObject WeaponInventory;
	[SerializeField]
	private GameObject sendButton,timeWindow, healthBar;
	private Text timeText,healthBarText;
	private TimeTracker timeTracker;
	private bool checkEnteredCounter=false;
	private MyPlayerScript myPlayer;
	private GameMoveListener gameMove;
	void Start()
	{
		OtherPlatform.SetActive(true);
		if(Application.isMobilePlatform){
			MobilePlatform.SetActive(true);
			//OtherPlatform.SetActive(false);
		}else{
			MobilePlatform.SetActive(false);
			//OtherPlatform.SetActive(true);
		}
		if(Application.platform==RuntimePlatform.LinuxEditor){
			// MobilePlatform.SetActive(true);
			// OtherPlatform.SetActive(false);
		}
		init();
	}
	private void init(){
		timeText=timeWindow.GetComponent<Text>();
		healthBarText=healthBar.GetComponent<Text>();
	}
	void Update()
	{
		checkTime();

		//if(Input.GetKey(KeyCode.))
		Vector2 v= checkInputs();
	}
	private void checkTime(){
		if(timeTracker==null)
			return;
		if(timeTracker.timeleft!=-1){
			showTime();
		}else{
			hideTime();
		}
	}
	private void showTime(){
		checkEnteredCounter=true;
		timeWindow.SetActive(true);
		timeText.text="Time = "+timeTracker.timeleft;
	}
	private void hideTime(){
		if(checkEnteredCounter){
			sendButton.SetActive(true);
			checkEnteredCounter=false;
		}
		timeWindow.SetActive(false);
		timeText.text="Time = "+timeTracker.timeleft;
	}
	public Vector2 checkInputs(){
		Vector2 v=new Vector2();
		if(Application.isMobilePlatform){
			if(CrossPlatformInputManager.GetButtonDown("UpButton")){
				
			}else if(CrossPlatformInputManager.GetButtonDown("DownButton")){
				
			}else if(CrossPlatformInputManager.GetButtonDown("LeftButton")){
				
			}else if(CrossPlatformInputManager.GetButtonDown("RightButton")){
				
			}else if(CrossPlatformInputManager.GetButtonDown("MultipleSelect")){
				
			}
		}else{

		}
		return v;
	}
	public void disconnect(){
		gameMove.disableClicksforX();
		NetworkManager.singleton.StopClient();
	}
	public void sendMoves(){
		Debug.Log("Sending Moves");
		gameMove.disableClicksforX();
		timeTracker.sendMovesFromTimeTracker();
		sendButton.SetActive(false);
	}
	private IEnumerator sendMovesAfter2(){
		for(int i=0;i<2;i++){
			yield return new WaitForEndOfFrame();
		}

	}
	public void setMyPlayerScript(MyPlayerScript myPlayer, TimeTracker track, GameMoveListener moveList){
		this.myPlayer=myPlayer;
		timeTracker=track;
		gameMove=moveList;
	}

	private void waitForKeys(){
		foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))){
			if(Input.GetKey(vKey)){
				//My Code Here
			}
		}
	}
	public void showHealthValue(int health){
		//Debug.Log("Its here : "+health);
		if(healthBarText!=null){
			string s="Health :"+health;
			healthBarText.text=s;
		}else
			Debug.Log("Its Null");
	}
}
