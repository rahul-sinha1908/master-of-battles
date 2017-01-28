using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MasterOfBattles;

public class WeaponControlScript : MonoBehaviour {

	public Transform weaponButton;
	public RectTransform weaponInventory, weaponDetails, weaponHolder;
	private GameObject quad;
	private GameRunningConstants grc;
	// Use this for initialization
	void Start () {
		if(weaponInventory==null){
			Dev.log(Tag.WeaponControlScript, "Initialize the weapon inventory");
		}else{
			weaponHolder=(RectTransform)weaponInventory.FindChild("WeaponHolder");
		}
		if(weaponDetails==null)
			Dev.log(Tag.WeaponControlScript, "Initialize the weapon Details");
		
		GameRunningConstants.getInstance().weaponControlScript=this;
		
		grc=GameRunningConstants.getInstance();
		grc.weaponControlScript=this;
		if(grc.weaponControlScript!=null)
			Dev.log(Tag.WeaponControlScript,"Weapon is initialised");
		hideWeapons();
	}
	
	void Update () {
		
	}
	public void hideWeapons(){
		weaponInventory.gameObject.SetActive(false);
		weaponDetails.gameObject.SetActive(false);
	}
	public void showWeapons(){
		weaponInventory.gameObject.SetActive(true);
		weaponDetails.gameObject.SetActive(true);
	}

	public void callToAddWeapon(PlayerProperties props, int ind){
		Dev.log(Tag.WeaponControlScript,"Called from the gameMove Successful");
		destroyAllChilds();
		List<PowerStruct> list = props.powers;
		for(int i=0;i<list.Count;i++)
			addWeapons(list[i], ind);
	}
	private void destroyAllChilds(){
		foreach(Transform child in weaponHolder){
			Destroy(child.gameObject);
		}
	}
	private void addWeapons(PowerStruct p, int ind){
		Dev.log(Tag.WeaponControlScript,"Weapons is getting Added");
		Transform go=GameObject.Instantiate(weaponButton,weaponHolder);
		Text t=go.FindChild("Text").GetComponent<Text>();
		t.text=PowersContants.getInstance().powers[p.id].name;

		Button b=go.GetComponent<Button>();
		b.onClick.AddListener(()=>buttonListener(p, ind));
	}
	private void buttonListener(PowerStruct p, int ind){
		//GameRunningConstants.getInstance().gameMoveListener
		GameRunningConstants.getInstance().gameMoveListener.setDefaultPower(ind, p.id);
		GameRunningConstants.getInstance().gameMoveListener.refreshWeaponDisplay(ind);
	}

	public List<Point> getAttackList(PlayerDetails p, PowerStruct power, Moves attackM){
		List<Point> list;
		list = getPowerPlaces(power.id, power.range, power.strength, p.x, p.y, attackM);

		return list;
	}
	private List<Point> getPowerPlaces(int id, int range, int strength, int x, int y, Moves m){
		List<Point> list = new List<Point>();
		int dr=Mathf.RoundToInt(range/GameContants.getInstance().sqrt2);
		Dev.log(Tag.WeaponControlScript, "Attack Id = "+id);
		if(id==1){
			for(int i=0;i<range;i++){
				Point p=new Point();
				p.x=x+i+1;
				p.y=y;
				if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p))
					list.Add(p);
				p=new Point();
				p.x=x-i-1;
				p.y=y;
				if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p))
					list.Add(p);
				p=new Point();
				p.x=x;
				p.y=y+i+1;
				if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p))
					list.Add(p);
				p=new Point();
				p.x=x;
				p.y=y-i-1;
				if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p))
					list.Add(p);
			}
			for(int i=0;i<dr;i++){
				Point p=new Point();
				p.x=x+i+1;
				p.y=y+i+1;
				if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p))
					list.Add(p);
				p=new Point();
				p.x=x-i-1;
				p.y=y-i-1;
				if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p))
					list.Add(p);
				p=new Point();
				p.x=x-i-1;
				p.y=y+i+1;
				if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p))
					list.Add(p);
				p=new Point();
				p.x=x+i+1;
				p.y=y-i-1;
				if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p))
					list.Add(p);
			}
		}else if(id==2){
			Point p;
			for(int i=0;i<=range;i++){
				for(int j=0;j<=range && (i+j)<=range;j++){
					if(i==0 && j==0)
						continue;
					p=new Point();
					p.x=x+i;
					p.y=y+j;
					if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p))
						list.Add(p);
					p=new Point();
					p.x=x-i;
					p.y=y+j;
					if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p) && i!=0)
						list.Add(p);
					p=new Point();
					p.x=x-i;
					p.y=y-j;
					if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p) && (j!=0 || i!=0))
						list.Add(p);
					p=new Point();
					p.x=x+i;
					p.y=y-j;
					if(GameMethods.validatePoint(p) && !GameMethods.compareMovesAndPoints(m,p) && j!=0)
						list.Add(p);
				}
			}
		}else if(id==3){

		}else if(id==4){

		}

		return list;
	} 

}