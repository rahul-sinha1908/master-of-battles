using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MasterOfBattles;

public class WeaponAssignmentScript : MonoBehaviour {

	public Button ready;
	public Slider strength, range;
	public GameObject weaponPanel, playerPanel, playerBP, weaponBP;
	private Text readyText;
	public GameObject myScreen;
	private GameRunningConstants grc;
	private int selectedPlayer, selectedWeapon;
	private ChessBoardFormation chess;

	// Use this for initialization
	void Start () {
		Dev.log(Tag.WeaponAssignmentScript,"Weapon Assignement Start");
		grc=GameRunningConstants.getInstance();
		grc.weaponAssignmentScript=this;

		ready.onClick.AddListener(()=>onReady());
		ready.enabled=false;
		readyText=ready.transform.FindChild("Text").GetComponent<Text>();

		selectedPlayer=-1;
		selectedWeapon=-1;
		initiatePanel();
		refreshSliders();
		chess=ChessBoardFormation.getInstance();
	}
	
	private void onReady(){
		myScreen.SetActive(true);
	}

	private void initiatePanel(){
		PowersContants pC=PowersContants.getInstance();
		int totPlayers=GameContants.NumberOfPlayer;

		for(int i=0;i<=pC.total;i++){
			addWeapons(pC.powers[i]);
		}
		for(int i=0;i<totPlayers;i++){
			addPlayer(i);
		}
	}

	private void addPlayer(int i){
		GameObject go=GameObject.Instantiate(playerBP,playerPanel.transform);
		Text t = go.transform.FindChild("Text").GetComponent<Text>();
		t.text="Player No. : "+(i+1);
		Button b=go.GetComponent<Button>();
		b.onClick.AddListener(()=>playerClickListener(i));
	}

	private void addWeapons(PowerNameId p){
		GameObject go=GameObject.Instantiate(weaponBP,weaponPanel.transform);
		Text t = go.transform.FindChild("Text").GetComponent<Text>();
		t.text=""+p.name;
		Button b=go.GetComponent<Button>();
		b.onClick.AddListener(()=>weaponClickListener(p));
	}

	private void weaponClickListener(PowerNameId p){
		if(p.id!=selectedWeapon){
			selectedWeapon=p.id;
			refreshSliders();
		}
	}

	private void playerClickListener(int ind){
		if(ind!=selectedPlayer){
			selectedPlayer=ind;
			refreshSliders();
		}
	}

	private void refreshSliders(){
		if(selectedPlayer<0 || selectedWeapon<0){
			strength.value=0;
			range.value=0;
			strength.enabled=false;
			range.enabled=false;
			return;
		}
		
		strength.enabled=true;
		range.enabled=true;
		PlayerProperties pP=chess.gameFormation[selectedPlayer];
		if(selectedWeapon==0){
			strength.maxValue=500;
			strength.minValue=0;
			range.minValue=1;
			range.maxValue=5;
			strength.value = pP.healthMetre;
			range.value = pP.speed;
		}else{
			strength.maxValue=100;
			strength.minValue=0;
			range.minValue=0;
			range.maxValue=(GameContants.sizeOfBoardX+GameContants.sizeOfBoardY)/2;

			int i;
			for(i=0;i<pP.powers.Count;i++){
				if(pP.powers[i].id==selectedWeapon)
					break;
			}
			if(i==pP.powers.Count){
				//TODO Weapon Not Bought
				strength.enabled=false;
				range.enabled=false;
				strength.value=0;
				range.value=0;
			}else{
				strength.value=pP.powers[i].strength;
				range.value=pP.powers[i].range;
			}
		}
	}
	// Update is called once per frame
	void Update () {
		if(GameRunningConstants.getInstance().networkPlayerScript!=null){
			ready.enabled=true;
			readyText.text="Ready";
		}else{
			readyText.text="Waiting for Opponent";
		}
	}
}
