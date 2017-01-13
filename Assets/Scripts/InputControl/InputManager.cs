using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
using MasterOfBattles;

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
		GameRunningConstants.getInstance().inputManager=this;
		
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
		if(v!=Vector2.zero){
			gameMove.disableClicksforX();
			gameMove.Move(v);
		}
	}
	private void checkTime(){
		if(timeTracker==null){
			GameObject g=GameObject.Find("TimeTracker(Clone)");
			if(g!=null)
				timeTracker=g.GetComponent<TimeTracker>();
		}
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
				v.y=1;
			}else if(CrossPlatformInputManager.GetButtonDown("DownButton")){
				v.y=-1;
			}else if(CrossPlatformInputManager.GetButtonDown("LeftButton")){
				v.x=-1;
			}else if(CrossPlatformInputManager.GetButtonDown("RightButton")){
				v.x=1;
			}else if(CrossPlatformInputManager.GetButtonDown("MultipleSelect")){
				
			}
		}else{
			if(Input.GetKeyDown(KeyCode.W)){
				v.y=1;
			}else if(Input.GetKeyDown(KeyCode.S)){
				v.y=-1;
			}else if(Input.GetKeyDown(KeyCode.A)){
				v.x=-1;
			}else if(Input.GetKeyDown(KeyCode.D)){
				v.x=1;
			}
		}
		return v;
	}
	public void disconnect(){
		gameMove.disableClicksforX();
		NetworkManager.singleton.StopClient();
	}
	public void sendMoves(){
		Dev.log(Tag.UnOrdered,"Sending Moves");
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
		//Dev.log(Tag.UnOrdered,"Its here : "+health);
		if(healthBarText!=null){
			string s="Health :"+health;
			healthBarText.text=s;
		}else
			Dev.log(Tag.UnOrdered,"Its Null");
	}
}
