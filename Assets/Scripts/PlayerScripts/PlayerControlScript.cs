using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterOfBattles;

public class PlayerControlScript : MonoBehaviour {
	private Animator anim;
	private ParticleSystem particles;
	// Use this for initialization
	void Start () {
		anim=GetComponent<Animator>();
		particles=transform.FindChild("Particle System").GetComponent<ParticleSystem>();
		if(particles==null)
			Debug.Log("Particle is Null");
		//doAttack(new PowerStruct());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void doAttack(PowerStruct p){
		disableAllProps();
		// if(p.id==1){
			straightAttack(p);
		// }
	}
	private void disableAllProps(){
		//TODO disable all the properties of the Particle System.
	}
	private void straightAttack(PowerStruct p){
		//TODO Attack Straight Attacks
		var main=particles.main;
		var shape=particles.shape;
		var emission=particles.emission;
		shape.enabled=false;
		emission.enabled=true;
		main.duration=1;
		main.startSpeed=5;
		main.loop=false;
		main.duration=1;
		main.startLifetime=10;
		Debug.Log("Its Just Before Play");
		particles.Play();
	}
}
