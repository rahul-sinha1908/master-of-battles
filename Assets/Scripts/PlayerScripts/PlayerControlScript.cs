using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterOfBattles;

public class PlayerControlScript : MonoBehaviour {
	public LayerMask attackMask;
	private Animator anim;
	private ParticleSystem particles;
	private bool isServer, isLocalPlayer;
	private CharacterController controller;
	private Vector3 offset=new Vector3(-GameContants.sizeOfBoardX/2.0f+0.5f,0,-GameContants.sizeOfBoardY/2.0f+0.5f);
	private Vector3 playerHeight=new Vector3(0,0,0);
	private float opponentPost;
	private Camera cam;
	private PlayerProperties me;
	private bool isAlive;
	private MyPlayerScript playerNetScript;
	private InputManager inputManager;
	private bool isPlayerMoving=false;
	// Use this for initialization
	void Start () {
		initiateMyPlayer();
		cam=Camera.main;
		anim=GetComponent<Animator>();
		particles=transform.FindChild("Particle System").GetComponent<ParticleSystem>();
		controller=GetComponent<CharacterController>();
		if(particles==null)
			Debug.Log("Particle is Null");

		inputManager=GameObject.Find("MyScreen").GetComponent<InputManager>();
		//doAttack(new PowerStruct());
	}
	
	private void initiateMyPlayer(){
		//TODO Do the Constants initialization which I may have missed doing it manuallly or give error in the debug log
		isAlive=false;
	}
	// Update is called once per frame
	void Update () {
		
	}

	public void initializePlayer(bool server, bool local, PlayerProperties player, MyPlayerScript playNet){
		//TODO Put this segment while creating players
		isServer=server;
		isLocalPlayer=local;
		opponentPost=GameContants.sizeOfBoardY*GameContants.boxSize;
		if((isServer && !isLocalPlayer) || (!isServer && isLocalPlayer))
			opponentPost=opponentPost*-1;
		me=player;
		playerNetScript=playNet;
	}
	public void initializePlayer(bool server, bool local, MyPlayerScript playNet){
		initializePlayer(server, local, null, playNet);
	}

	public void doAttack(int x1, int y1, int x2, int y2, PowerStruct p){
		disableAllProps();
		// if(p.id==1){
			straightAttack(x1,y1,x2,y2,p);
		// }
	}
	private void disableAllProps(){
		//TODO disable all the properties of the Particle System.
	}
	private void straightAttack(int x1, int y1, int x2, int y2, PowerStruct p){
		//TODO Attack Straight Attacks

		//TODO Assign the correct value for substracting the health
		int val=10;

		Vector3 pos=new Vector3(x1,1, y1)+playerHeight+offset;
		pos.x*=GameContants.boxSize;
		pos.z*=GameContants.boxSize;
		Vector3 pos1=new Vector3(x2,1, y2)+playerHeight+offset;
		pos1.x*=GameContants.boxSize;
		pos1.z*=GameContants.boxSize;
		Vector3 finalPos=pos1;
		Debug.Log("Shooting from "+pos+" to "+pos1);

		Vector3 dir=pos1-pos;
		RaycastHit hit;
		if(Physics.Raycast(pos, dir, out hit , GameContants.boxSize*(GameContants.sizeOfBoardX+GameContants.sizeOfBoardY),attackMask)){
			GameObject g=hit.collider.gameObject;
			finalPos=hit.point;
			Debug.Log("Hit the target : "+g.name);
			if(g!=null){
				PlayerControlScript pl=g.GetComponent<PlayerControlScript>();
				if(pl!=null)
					pl.reduceHealth(val);
			}
		}

		transform.LookAt(finalPos);
		float dist=Vector3.Distance(pos,finalPos);
		var main=particles.main;
		var shape=particles.shape;
		var emission=particles.emission;
		main.startSpeed=5;
		main.loop=false;
		main.startSpeed=10*GameContants.boxSize;
		main.startLifetime=dist/main.startSpeed.constant;
		shape.enabled=false;
		emission.enabled=true;
		ParticleSystem.Burst[] b=new ParticleSystem.Burst[1];
		b[0].time=0;
		b[0].maxCount=100;
		b[0].minCount=70;
		emission.SetBursts(b);
		emission.rateOverTime=30;
		Debug.Log("Its Just Before Play");
		particles.Play();

	}
	public void reduceHealth(int val){
		Debug.Log("Hit the player : "+transform.name);
		if(isLocalPlayer){
			me.curHealth-=val;
			Debug.Log("Cur Health : "+me.curHealth);
			if(me.curHealth<=0){
				isAlive=false;
				playerNetScript.killThePlayer(me.playerIndex);
				doKillAnimation();
			}
		}
	}
	public void doKillAnimation(){
		//TODO Do kill Animations
		Debug.Log("The player Died");
	}
	public void movePlayer(PlayerDetails p){
		//DONE Do animation and stuffs
		
		if(isVisibleToCam()){
			movePlayerWithoutAnim(p);
			return;
		}

		Vector3 pos=new Vector3(p.x,0, p.y)+playerHeight+offset;
		pos.x*=GameContants.boxSize;
		pos.z*=GameContants.boxSize;
		Vector3 ipos=transform.position;
		Vector3 dir =  (pos-ipos);
		Vector3 td=dir;
		dir.Normalize();
		
		if(GameMethods.sqrDist(ipos,pos)<1.5*1.5){
			isPlayerMoving=false;
			//transform.position=new Vector3(pos.x,transform.position.y,pos.z);
			controller.Move(td);
			anim.SetBool("Walk", false);
			transform.LookAt(new Vector3(transform.position.x,transform.position.y,opponentPost));
		}else{
			//TODO Specify Player Speed here
			isPlayerMoving=true;
			//controller.Move(dir*10*Time.deltaTime);
			controller.SimpleMove(dir*5);
			transform.LookAt(pos);
			anim.SetBool("Walk", true);
		}
	}
	private void movePlayerWithoutAnim(PlayerDetails p){
		//TODO Optimise by remove the animation and just Move.
	}
	private bool isVisibleToCam(){

		return false;
	}
	public void setAsSelectedPlayer(){
		//inputManager.showHealthValue(me.curHealth);
	}
	// void OnControllerColliderHit(ControllerColliderHit other){
			
	// }
}
