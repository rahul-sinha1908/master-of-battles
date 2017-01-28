using System.Collections;
using System.Collections.Generic;
using MasterOfBattles;
using UnityEngine.Networking;
public class ChessBoardFormation {
	
	public static ChessBoardFormation instance;
	public List<PlayerProperties> gameFormation;

	public TypeO[,] myBoard;
	private ChessBoardFormation(){
		gameFormation=new List<PlayerProperties>();
		for(int i=0;i<GameContants.getInstance().NumberOfPlayer;i++){
			gameFormation.Add(new PlayerProperties(i));
		}
		myBoard=new TypeO[GameContants.getInstance().sizeOfBoardX,GameContants.getInstance().sizeOfBoardY];
		for(int i=0;i<GameContants.getInstance().sizeOfBoardX;i++){
			for(int j=0;j<GameContants.getInstance().sizeOfBoardY;j++){
				myBoard[i,j]=TypeO.None;
			}
		}
	}
	public static ChessBoardFormation getInstance(){
		if(instance==null){
			instance=new ChessBoardFormation();
		}
		return instance;
	}
	public void saveAllTransforms(){
		for(int i=0;i<GameContants.getInstance().NumberOfPlayer;i++){
			gameFormation[i].storePlayerDetails();
		}
	}
	public List<Point> TransFormToGame(bool isClient){
		List<Point> list=new List<Point>();
		if(isClient){
			for(int i=0;i<GameContants.getInstance().NumberOfPlayer;i++){
				Point p=gameFormation[i].loc;
				p.x=GameContants.getInstance().sizeOfBoardX-1-p.x;
				p.y=GameContants.getInstance().sizeOfBoardY-1-p.y;
				list.Add(p);
			}
		}else{
			for(int i=0;i<GameContants.getInstance().NumberOfPlayer;i++){
				Point p=gameFormation[i].loc;
				list.Add(p);
			}
		}
		return list;
	}
}
