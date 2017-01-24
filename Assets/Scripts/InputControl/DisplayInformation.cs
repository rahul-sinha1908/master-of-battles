using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MasterOfBattles;

public class DisplayInformation : MonoBehaviour {
	public GameObject canvas;
	public Text text;
	private float displayTime;
	public GameObject statisticUIPrefab, statisticCanvas;
	private GameRunningConstants grc;
	void Start () {
		GameRunningConstants.getInstance().displayInformation=this;
		grc=GameRunningConstants.getInstance();
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

	public void closeStatisticCanvas(){
		//TODO Give reference to the close button of the statisticCanvas
		statisticCanvas.SetActive(false);
	}
	public void displayStatistic(){
		statisticCanvas.SetActive(!statisticCanvas.activeSelf);
		grc.checkSelectScript.clearAllBoxes();
		if(statisticCanvas.activeSelf){
			grc.gameMoveListener.setIsClickActive(false);
			grc.gameMoveListener.clearListPossibleMoves();
		}else{
			grc.gameMoveListener.setIsClickActive(true);
			destroyAllChilds(statisticCanvas.transform);
			return;
		}
		/*
		So, what is left.
		I can display the player place moves and attack moves for opponent players
		Even the moves of the own player could be shown to show the effect on the opponent players.
		*/

		//DONE Do the functionality of making the panel filled with the detials of the previous moves.
		Dev.log(Tag.DisplayInformation,"Sizes : "+grc.backUpOppAttackMoves.Count+" : "+ grc.backUpOppPlaceMoves.Count+" : "+grc.backUpMyMoves.Count);
		for(int i=0;i<grc.backUpOppAttackMoves.Count;i++){
			GameObject go = GameObject.Instantiate(statisticUIPrefab, statisticCanvas.transform);
			Button but=go.GetComponent<Button>();
			Text txt=go.transform.Find("Text").GetComponent<Text>();
			txt.text="Attack Moves by Opp "+(grc.backUpOppAttackMoves[i].ind+1);
			int x1=grc.backUpOppAttackMoves[i].x,
			y1=grc.backUpOppAttackMoves[i].y,
			x2=grc.networkPlayerScript.getPlayerDetails()[grc.backUpOppAttackMoves[i].ind].x,
			y2=grc.networkPlayerScript.getPlayerDetails()[grc.backUpOppAttackMoves[i].ind].y;
			but.onClick.AddListener(()=>displayBox(x1, y1, x2, y2));
		}
		for(int i=0;i<grc.backUpOppPlaceMoves.Count;i++){
			GameObject go = GameObject.Instantiate(statisticUIPrefab, statisticCanvas.transform);
			Button but=go.GetComponent<Button>();
			Text txt=go.transform.Find("Text").GetComponent<Text>();
			txt.text="Place Moves by Opp "+(grc.backUpOppPlaceMoves[i].ind+1);
			int x1=grc.backUpOppPlaceMoves[i].x,
			y1=grc.backUpOppPlaceMoves[i].y,
			x2=grc.networkPlayerScript.getPlayerDetails()[grc.backUpOppPlaceMoves[i].ind].x,
			y2=grc.networkPlayerScript.getPlayerDetails()[grc.backUpOppPlaceMoves[i].ind].y;
			but.onClick.AddListener(()=>displayBox(x1, y1, x2, y2));
		}
		for(int i=0;i<grc.backUpMyMoves.Count;i++){
			GameObject go = GameObject.Instantiate(statisticUIPrefab, statisticCanvas.transform);
			Button but=go.GetComponent<Button>();
			Text txt=go.transform.Find("Text").GetComponent<Text>();
			txt.text="Attack Moves by Me "+(grc.backUpMyMoves[i].ind+1);
			int x1=grc.backUpMyMoves[i].x,
			y1=grc.backUpMyMoves[i].y,
			x2=grc.localPlayerScript.getPlayerDetails()[grc.backUpMyMoves[i].ind].x,
			y2=grc.localPlayerScript.getPlayerDetails()[grc.backUpMyMoves[i].ind].y;
			but.onClick.AddListener(()=>displayBox(x1, y1, x2, y2));
		}
	}
	private void displayBox(int x1, int y1, int x2, int y2){
		List<Point> plist=new List<Point>();
		Point p=new Point();
		p.x=x1; p.y=y1;
		plist.Add(p);
		p=new Point();
		p.x=x2; p.y=y2;
		plist.Add(p);

		grc.checkSelectScript.showSelectedTiles(plist, BoardConstants.Select);
	}

	private void destroyAllChilds(Transform t){
		foreach(Transform child in t){
			Destroy(child.gameObject);
		}
	}
}
