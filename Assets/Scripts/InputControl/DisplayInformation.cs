using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MasterOfBattles;

public class DisplayInformation : MonoBehaviour {
	public GameObject canvas;
	public Text text;
	private float displayTime;

	void Start () {
		GameRunningConstants.getInstance().displayInformation=this;
	}
	
	private IEnumerator displayInfo(ErrorType err){
		canvas.SetActive(true);
		if(err==ErrorType.DoubleTapToAttack){
			text.text="Double Tap the Player to Attack";
		}else if(err==ErrorType.SingleTapTOSelectAndMove){
			text.text="Single Tap to Select the Player and to move it";
		}
		/*else if(err==ErrorType.){
			text.text="";
		}*/
		yield return new WaitForSeconds(displayTime);
		canvas.SetActive(false);
		yield break;
	}
	public void displayInformation(ErrorType err){
		StartCoroutine(displayInfo(err));
	}
	void Update () {
		
	}
}
