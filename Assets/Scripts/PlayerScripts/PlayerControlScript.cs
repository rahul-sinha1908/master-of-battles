using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasterOfBattles;

public class PlayerControlScript : MonoBehaviour {
	public Material attack, initialPlace, movedPlace; 
	public LayerMask attackMask;
	private Animator anim;
	private ParticleSystem particles;
	private bool isServer, isLocalPlayer;
	private CharacterController controller;
	private Vector3 offset=new Vector3(-GameContants.sizeOfBoardX/2.0f+0.5f,0,-GameContants.sizeOfBoardY/2.0f+0.5f);
	private float playerHeight=0;
	private float opponentPost;
	private Camera cam;
	private PlayerProperties me;
	private PlayerDetails playerDet;
	private bool isAlive;
	private MyPlayerScript playerNetScript;
	//private InputManager inputManager;
	private bool isPlayerMoving=false;
	private Transform playerIdentity;
	private MeshRenderer playerIdentityMesh;
	// Use this for initialization
	void Start () {
		initiateMyPlayer();
		cam=Camera.main;
		particles=transform.FindChild("Particle System").GetComponent<ParticleSystem>();
		
		controller=GetComponent<CharacterController>();
		if(particles==null)
			Dev.log(Tag.UnOrdered,"Particle is Null");
		//inputManager=GameObject.Find("MyScreen").GetComponent<InputManager>();
		//doAttack(new PowerStruct());
	}
	
	private void initiateMyPlayer(){
		//TODO Do the Constants initialization which I may have missed doing it manuallly or give error in the debug log
		isAlive=false;
	}
	// Update is called once per frame
	void Update () {
		
	}
	public void playerHasMoved(){
		playerIdentityMesh.material=movedPlace;
	}
	public void playerIsAttacking(){
		playerIdentityMesh.material=attack;
	}
	public void playerIsAtOriginal(){
		playerIdentityMesh.material=initialPlace;
	}
	public void initializePlayer(bool server, bool local, PlayerProperties player, MyPlayerScript playNet, PlayerDetails p){
		//DONE Put this segment while creating players
		isServer=server;
		isLocalPlayer=local;
		opponentPost=GameContants.sizeOfBoardY*GameContants.boxSize;
		if((isServer && !isLocalPlayer) || (!isServer && isLocalPlayer))
			opponentPost=opponentPost*-1;
		me=player;
		playerDet=p;
		playerNetScript=playNet;
		if(me!=null)
			me.curHealth=me.healthMetre;

		playerIdentity=transform.FindChild("PlayerIdentity");
		if(playerIdentity!=null){
			if(!isLocalPlayer)
				playerIdentity.Rotate(Vector3.up,180f,Space.Self);
			playerIdentityMesh=playerIdentity.GetComponent<MeshRenderer>();
			playerIdentity.FindChild("Text").GetComponent<TextMesh>().text=""+(p.ind+1);
			playerIsAtOriginal();
		}
		
		Object go=Resources.Load("Players/"+GameContants.getInstance().playerNames[p.playerType]);
		if(go!=null){
			//Dev.log(Tag.PlayerControlScript,"The object is not null");
			GameObject g = (GameObject)GameObject.Instantiate(go,transform,false);
			g.name="Avatar";
			anim=g.GetComponent<Animator>();
		}else
			Dev.log(Tag.PlayerControlScript,"The object is null");
	}
	public void initializePlayer(bool server, bool local, MyPlayerScript playNet, PlayerDetails p){
		initializePlayer(server, local, null, playNet,p);
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

		Vector3 pos=GameMethods.getHitVector(x1,y1,1);
		Vector3 pos1=GameMethods.getHitVector(x2,y2,1);
		Vector3 finalPos=pos1;
		Dev.log(Tag.PlayerControlScript,"Shooting from "+pos+" to "+pos1);

		Vector3 dir=pos1-pos;
		RaycastHit hit;
		if(Physics.Raycast(pos, dir, out hit , GameContants.boxSize*(GameContants.sizeOfBoardX+GameContants.sizeOfBoardY),attackMask)){
			GameObject g=hit.collider.gameObject;
			if(GameMethods.sqrDist(pos,pos1)+GameContants.boxSize/2.0f>GameMethods.sqrDist(pos,hit.point)){
				finalPos=hit.point;
				Dev.log(Tag.PlayerControlScript,"Hit the target : "+g.name);
				if(g!=null){
					PlayerControlScript pl=g.GetComponent<PlayerControlScript>();
					if(pl!=null)
						pl.reduceHealth(p.strength);
				}
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
		Dev.log(Tag.PlayerControlScript,"Its Just Before Play");
		particles.Play();

	}
	public void reduceHealth(int val){
		Dev.log(Tag.PlayerControlScript,"Hit the player : "+transform.name);
		if(isLocalPlayer){
			me.curHealth-=val;
			Dev.log(Tag.PlayerControlScript,"Cur Health : "+me.curHealth);
			if(me.curHealth<=0){
				isAlive=false;
				playerNetScript.killThePlayer(me.playerIndex);
				doKillAnimation();
			}
		}
	}
	public void doKillAnimation(){
		//TODO Do kill Animations
		Dev.log(Tag.PlayerControlScript,"The player Died");
	}
	public void movePlayer(PlayerDetails p){
		//DONE Do animation and stuffs
		playerDet=p;
		if(isVisibleToCam()){
			movePlayerWithoutAnim(p);
			return;
		}

		Vector3 pos=GameMethods.getHitVector(p.x,p.y,playerHeight);
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
