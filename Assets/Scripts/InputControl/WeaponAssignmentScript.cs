using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MasterOfBattles;

public class WeaponAssignmentScript : MonoBehaviour {

	public Button ready;
	public Text readyText;
	public Transform waiting;
	public GameObject myScreen;
	private GameRunningConstants grc;
	// Use this for initialization
	void Start () {
		Dev.log(Tag.WeaponAssignmentScript,"Weapon Assignement Start");
		grc=GameRunningConstants.getInstance();
		grc.weaponAssignmentScript=this;

		ready.onClick.AddListener(()=>onReady());
		ready.enabled=false;
		readyText=ready.transform.FindChild("Text").GetComponent<Text>();
	}
	
	private void onReady(){
		myScreen.SetActive(true);
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
