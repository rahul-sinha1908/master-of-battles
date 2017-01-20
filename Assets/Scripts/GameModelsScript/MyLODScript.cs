using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterOfBattles;

public class MyLODScript : MonoBehaviour {

	public List<GameObject> near, medium, far;
	[SerializeField]
	private float nearMedium=10, mediumFar=40;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float dist=GameMethods.sqrDist(transform.position,Camera.main.transform.position);
		if(dist<nearMedium*nearMedium){
			displayNear();
		}else if(dist<mediumFar*mediumFar){
			displayMedium();
		}else{
			displayFar();
		}
	}
	private void displayNear(){
		for(int i=0;i<far.Count;i++){
			far[i].SetActive(false);
		}
		for(int i=0;i<medium.Count;i++){
			medium[i].SetActive(false);
		}
		for(int i=0;i<near.Count;i++){
			near[i].SetActive(true);
		}
	} 
	private void displayMedium(){
		for(int i=0;i<near.Count;i++){
			near[i].SetActive(false);
		}
		for(int i=0;i<far.Count;i++){
			far[i].SetActive(false);
		}
		for(int i=0;i<medium.Count;i++){
			medium[i].SetActive(true);
		}
	}
	private void displayFar(){
		for(int i=0;i<near.Count;i++){
			near[i].SetActive(false);
		}
		for(int i=0;i<medium.Count;i++){
			medium[i].SetActive(false);
		}
		for(int i=0;i<far.Count;i++){
			far[i].SetActive(true);
		}

	}
}
