using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MasterOfBattles;

public class WeaponAssignmentScript : MonoBehaviour {

	public Button ready, buy;
	public Slider strength, range;
	public GameObject weaponPanel, playerPanel, playerBP, weaponBP;
	public Text coins, strengthText, rangeText;
	private Text readyText;
	public GameObject myScreen;
	private GameRunningConstants grc;
	private int selectedPlayer, selectedWeapon;
	private ChessBoardFormation chess;
	private int coinsleft, ccl;

	// Use this for initialization
	void Start () {
		Dev.log(Tag.WeaponAssignmentScript,"Weapon Assignement Start");
		grc=GameRunningConstants.getInstance();
		grc.weaponAssignmentScript=this;

		chess=ChessBoardFormation.getInstance();

		coinsleft = getCoinsLeft();
		ccl=coinsleft;

		ready.onClick.AddListener(()=>onReady());
		ready.enabled=false;
		readyText=ready.transform.FindChild("Text").GetComponent<Text>();

		buy.onClick.AddListener(()=>buyClicked());

		strength.wholeNumbers=true;
		range.wholeNumbers=true;
		strength.onValueChanged.AddListener((val)=>onStrengthChanged(val));
		range.onValueChanged.AddListener((val)=>onRangeChanged(val));

		selectedPlayer=-1;
		selectedWeapon=-1;
		initiatePanel();
		refreshSliders();
	}
	private int getCoinsLeft(){
		//TODO Write the segment for entering the number of coins
		return 200;
	}
	private void buyClicked(){
		//TODO On Buy Button is Clicked

		if(selectedPlayer<0 || selectedWeapon<0 || ccl<0)
			return;
		else if(selectedWeapon==0){
			buyDefenceAndSpeed();
			return;
		}

		PlayerProperties pP=chess.gameFormation[selectedPlayer];
		int i;
		for(i=0;i<pP.powers.Count;i++){
			if(pP.powers[i].id==selectedWeapon)
				break;
		}
		
		if(i==pP.powers.Count){
			PowerStruct pS=new PowerStruct();
			pS.id=selectedWeapon;
			pS.range=5;
			pS.strength=10;
			pS.otherDef="";
			pP.powers.Add(pS);
		}else{
			// pP.powers[i].strength=strength.value;
			// pP.powers[i].range=(int)range.value;
			var v= pP.powers[i];
			v.strength=(int)strength.value;
			v.range=(int)range.value;
			pP.powers.RemoveAt(i);
			pP.powers.Add(v);
		}
		coinsleft=ccl;
		refreshSliders();
	}
	private void buyDefenceAndSpeed(){
		chess.gameFormation[selectedPlayer].healthMetre=(int)strength.value;
		chess.gameFormation[selectedPlayer].speed=(int)range.value;
		coinsleft=ccl;
		refreshSliders();
	}
	private void defenceChange(){
		if(strength.value<chess.gameFormation[selectedPlayer].healthMetre)
			strength.value=chess.gameFormation[selectedPlayer].healthMetre;
		ccl=coinsleft-((int)strength.value - chess.gameFormation[selectedPlayer].healthMetre)*1-((int)range.value - chess.gameFormation[selectedPlayer].speed)*10;
		coins.text="Coins left : "+coinsleft+"\nAfter Purchase : "+ccl;
	}
	private void speedChange(){
		if(range.value<chess.gameFormation[selectedPlayer].speed)
			range.value=chess.gameFormation[selectedPlayer].speed;
		ccl=coinsleft-((int)strength.value - chess.gameFormation[selectedPlayer].healthMetre)*1-((int)range.value - chess.gameFormation[selectedPlayer].speed)*10;
		coins.text="Coins left : "+coinsleft+"\nAfter Purchase : "+ccl;
	}
	private void onStrengthChanged(float val){
		strengthText.text=""+val;
		if(selectedPlayer<0 || selectedWeapon<0)
			return;
		else if(selectedWeapon==0){
			defenceChange();
			return;
		}
		
		Dev.log(Tag.WeaponAssignmentScript,"Value Changed : "+val);
		PlayerProperties pP=chess.gameFormation[selectedPlayer];
		int i;
		for(i=0;i<pP.powers.Count;i++){
			if(pP.powers[i].id==selectedWeapon)
				break;
		}
		ccl=coinsleft;
		if(i==pP.powers.Count){
			ccl=coinsleft-PowersContants.getInstance().powers[selectedWeapon].basePrice;
		}else{
			if(val<pP.powers[i].strength){
				strength.value=pP.powers[i].strength;
			}
			ccl=coinsleft-PowersContants.getInstance().powers[selectedWeapon].priceSAdder*((int)strength.value-pP.powers[i].strength)-PowersContants.getInstance().powers[selectedWeapon].priceRAdder*((int)range.value-pP.powers[i].range);
		}
		coins.text="Coins left : "+coinsleft+"\nAfter Purchase : "+ccl;
	}
	private void onRangeChanged(float val){
		rangeText.text=""+val;
		if(selectedPlayer<0 || selectedWeapon<0)
			return;
		else if(selectedWeapon==0){
			speedChange();
			return;
		}
		
		Dev.log(Tag.WeaponAssignmentScript,"Value Changed : "+val);

		PlayerProperties pP=chess.gameFormation[selectedPlayer];
		int i;
		for(i=0;i<pP.powers.Count;i++){
			if(pP.powers[i].id==selectedWeapon)
				break;
		}
		ccl=coinsleft;
		if(i==pP.powers.Count){
			ccl=coinsleft-PowersContants.getInstance().powers[selectedWeapon].basePrice;
		}else{
			if(val<pP.powers[i].range){
				range.value=pP.powers[i].range;
			}
			ccl=coinsleft-PowersContants.getInstance().powers[selectedWeapon].priceSAdder*((int)strength.value-pP.powers[i].strength)-PowersContants.getInstance().powers[selectedWeapon].priceRAdder*((int)range.value-pP.powers[i].range);
		}
		coins.text="Coins left : "+coinsleft+"\nAfter Purchase : "+ccl;
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
		coins.text="Coins left : "+coinsleft+"\nAfter Purchase : "+ccl;
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
