using System.Collections;
using System.Collections.Generic;

namespace MasterOfBattles{
	public struct Point{
		public int x;
		public int y;
	}
	public struct PowerNameId{
		public string name;
		public string description;
		public string explanationLink;
		public int id;
	};
	public struct PlayerDetails{
		public short ind,x,y, playerType;
	}

	public struct Moves{
		public short ind,x,y;
		public short attackInd;
		public int intensity;
	};

	public class GameContants{
		public static int NumberOfPlayer=30;
		public static int DefaultHealth=50;
		public static int sizeOfBoardX=30;
		public static int sizeOfBoardY=30;
		public static int boxSize=3;
		public static int timeConstant=10;
	}
}