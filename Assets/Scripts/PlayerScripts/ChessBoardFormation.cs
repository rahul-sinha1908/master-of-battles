using System.Collections;
using System.Collections.Generic;
using MasterOfBattles;
using UnityEngine.Networking;
public class ChessBoardFormation {

	List<PlayerProperties> gameFormation;
	public ChessBoardFormation(){
		gameFormation=new List<PlayerProperties>();
		for(int i=0;i<GameContants.NumberOfPlayer;i++){
			gameFormation.Add(new PlayerProperties(i));
		}
	}
	public SyncListStruct<Point> TransFormToGame(){
		SyncListStruct<Point> list=new SyncListStruct<Point>();
		for(int i=0;i<GameContants.NumberOfPlayer;i++){
			Point p=gameFormation[i].loc;
			list.Add(p);
		}
		return list;
	}
}
