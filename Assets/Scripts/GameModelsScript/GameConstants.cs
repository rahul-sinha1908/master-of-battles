using System.Collections;
using System.Collections.Generic;

namespace MasterOfBattles{
	public struct Point{
		public int x;
		public int y;
	}
	public enum TypeO{
		MyPlayer,OpponentPlayer,Objects,None
	}
	public enum ErrorType{
		OutOfPlace, OutOfRange, DoubleTapToAttack, SingleTapTOSelectAndMove
	}
	public struct PlayerDetails{
		public short ind,x,y, playerType;
	}

	public struct Moves{
		public short ind,x,y;
		public string attackDef;
	};

	public class GameContants{
		public static int NumberOfPlayer=30;
		public static int DefaultHealth=50;
		public static int sizeOfBoardX=30;
		public static int sizeOfBoardY=30;
		public static int boxSize=3;
		public static int timeConstant=10;
		public static float sqrt2=1.4142135624f;
		public static GameContants instance;

		public string[] playerNames;
		private GameContants(){
			playerNames=new string[10];
			playerNames[0]="Assassin1";
			playerNames[1]="SwordSoldier1";
		}

		public static GameContants getInstance(){
			if(instance==null)
				instance=new GameContants();
			return instance;
		}
	}
	public class BoardConstants{
		public static int Select=0;
		public static int Move=1;
		public static int Attack=2;
	}
}