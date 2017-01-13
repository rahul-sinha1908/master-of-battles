using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterOfBattles;

public class WeaponControlScript : MonoBehaviour {

	public Transform weaponButton;
	private RectTransform gameO;
	private GameRunningConstants grc;
	// Use this for initialization
	void Start () {
		GameRunningConstants.getInstance().weaponControlScript=this;
		
		gameO=GetComponent<RectTransform>();
		grc=GameRunningConstants.getInstance();
		grc.weaponControlScript=this;
		if(grc.weaponControlScript!=null)
			Dev.log(Tag.WeaponControlScript,"Weapon is initialosed");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void callToAddWeapon(PlayerProperties props){
		Dev.log(Tag.WeaponControlScript,"Called from the gameMove Successful");
		destroyAllChilds();
		List<PowerStruct> list = props.powers;
		for(int i=0;i<list.Count;i++)
			addWeapons(list[i]);
	}
	private void destroyAllChilds(){
		//for(int i=0;i<gameO.transform.childCount)
		for(int i=0;i<gameO.childCount;i++){
			Transform o=gameO.GetChild(i);
			o.parent=null;
			Destroy(o);
		}
	} 
	private void addWeapons(PowerStruct p){
		Dev.log(Tag.WeaponControlScript,"Weapons is getting Added");
		Transform go=GameObject.Instantiate(weaponButton,transform);
	}
}
