using System.Collections;
using System.Collections.Generic;
using MasterOfBattles;
public class ChessBoardFormation {

	int[,] gameFormation;
	public ChessBoardFormation(){
		gameFormation=new int[30,30];
		for(int i=0;i<GameContants.NumberOfPlayer;i++){
			Point p=new PlayerProperties(i).loc;
			gameFormation[p.x,p.y]=i;
		}
	}
	public void TransFormToGame(int[,] a){		
		
	}
}
