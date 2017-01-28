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
		if(p.id==1){
			straightAttack(x1,y1,x2,y2,p);
		}else if(p.id==2){
			rangeAttack(x1,y1,x2,y2,p);
		}else if(p.id==3){
			raySpreader(x1,y1,x2,y2,p);
		}else if(p.id==2){
			bombAttack(x1,y1,x2,y2,p);
		}else if(p.id==2){
			trippleShot(x1,y1,x2,y2,p);
		}
	}
	private void disableAllProps(){
		//TODO disable all the properties of the Particle System.
	}
	private void straightAttack(int x1, int y1, int x2, int y2, PowerStruct p){
		//TODO Attack Straight Attacks

		Point posP=new Point();
		posP.x=x1;posP.y=y1;
		Point posP1=new Point();
		posP1.x=x2;posP1.y=y2;
		Vector3 pos=GameMethods.getHitVector(posP,1);
		PlayerControlScript pl=null;
		do{
			if(posP.x<posP1.x)
				posP.x++;
			else if(posP.x>posP1.x)
				posP.x--;
			if(posP.y<posP1.y)
				posP.y++;
			else if(posP.y>posP1.y)
				posP.y--;
			Dev.log(Tag.PlayerAttack,"Check at  : "+posP.x + " : " + posP.y);				
			if(ChessBoardFormation.getInstance().myBoard[posP.x, posP.y]!= TypeO.None){
				Dev.log(Tag.PlayerAttack,"Found at  : "+posP.x + " : " + posP.y);
				pl=GameMethods.getPlayerCSAt(posP);
				if(pl!=null)
					pl.reduceHealth(p.strength);
				break;
			}
		}while(!GameMethods.comparePoints(posP,posP1));

		Vector3 finalPos=GameMethods.getHitVector(posP,1);
		if(!GameMethods.comparePoints(posP,posP1)){
			//TODO Problem in detecting the player in the player backup Moves
			Dev.log(Tag.PlayerAttack, "Got into the checking system : " +ChessBoardFormation.getInstance().myBoard[posP.x, posP.y]);
			GameRunningConstants grc=GameRunningConstants.getInstance();
			List<Moves> m = grc.backUpMyMoves;
			if(!isLocalPlayer)
				m = grc.backUpOppAttackMoves;
			for(int i=0;i<m.Count;i++){
				if(grc.localPlayerScript.getPlayerControlScript(m[i].ind) == pl || grc.networkPlayerScript.getPlayerControlScript(m[i].ind) == pl){
					Dev.log(Tag.PlayerAttack, "Got the local player or Network Player");
					Moves mo = m[i];
					mo.x=(short)posP.x;
					mo.y=(short)posP.y;
					m[i]=mo;
					break;
				}
			}
		}
		Dev.log(Tag.PlayerAttack,"Shooting from "+pos+" to "+finalPos);		

		//TODo Problem The attack fire is not stopping at the correct position
		transform.LookAt(finalPos);
		float dist=Vector3.Distance(pos,finalPos);
		var main=particles.main;
		var shape=particles.shape;
		var emission=particles.emission;
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
		// particles.Play();

	}
	private void rangeAttack(int x1, int y1, int x2, int y2, PowerStruct p){
		PlayerControlScript pl =GameMethods.getPlayerCSAt(x2, y2);
		if(pl !=null){
			pl.reduceHealth(p.strength);
		}
	}
	private void raySpreader(int x1, int y1, int x2, int y2, PowerStruct p){

	}
	private void bombAttack(int x1, int y1, int x2, int y2, PowerStruct p){
		int bombRange;
		if(p.otherDef=="")
			bombRange=0;
		else
			bombRange=int.Parse(p.otherDef);
		
		for(int i=0; i<=bombRange; i++){
			if(i==0){

			}else{

			}
		}
		
	}
	private void trippleShot(int x1, int y1, int x2, int y2, PowerStruct p){

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
