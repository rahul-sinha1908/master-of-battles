using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	private GameObject SendButton;
	
	private MyPlayerScript myPlayer;
	void Start()
	{
		if(Application.isMobilePlatform){
			MobilePlatform.SetActive(true);
			OtherPlatform.SetActive(false);
		}else{
			MobilePlatform.SetActive(false);
			OtherPlatform.SetActive(true);
		}
		if(Application.platform==RuntimePlatform.LinuxEditor){
			// MobilePlatform.SetActive(true);
			// OtherPlatform.SetActive(false);
		}

	}
	void Update()
	{
		//if(Input.GetKey(KeyCode.))
	}
	public void success(){
		Debug.Log("success");
	}
	private IEnumerator sendMovesAfter2(){
		for(int i=0;i<2;i++){
			yield return new WaitForEndOfFrame();
		}

	}
	public void movement(bool isLeft, bool isUp){
		StartCoroutine(sendMovesAfter2());
	}
	public void setMyPlayerScript(MyPlayerScript obj){
		myPlayer=obj;
	}

	public void updateMyPlayerScript(MyPlayerScript myPlayer){
		this.myPlayer=myPlayer;
	}

	private void waitForKeys(){
		foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))){
			if(Input.GetKey(vKey)){
				//My Code Here
			}
		}
	}
}
