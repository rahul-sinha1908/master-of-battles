using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MasterOfBattles;

public class WeaponAssignmentScript : MonoBehaviour {

	public Button ready;
	public GameObject myScreen;
	private GameRunningConstants grc;
	// Use this for initialization
	void Start () {
		Dev.log(Tag.WeaponAssignmentScript,"Weapon Assignement Start");
		grc=GameRunningConstants.getInstance();
		grc.weaponAssignmentScript=this;

		ready.onClick.AddListener(()=>onReady());

	}
	
	private void onReady(){
		myScreen.SetActive(true);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
