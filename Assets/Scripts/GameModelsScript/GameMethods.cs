using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterOfBattles{
	public class GameMethods{
		public static float sqrDist(Vector3 v1, Vector3 v2){
			float a=0;
			a=(v1.x-v2.x)*(v1.x-v2.x)+(v1.y-v2.y)*(v1.y-v2.y)+(v1.z-v2.z)*(v1.z-v2.z);
			return a;
		}
		public static Point getHitBox(Vector3 v){
			Point p;
			p.x=(int)Mathf.Floor(v.x/GameContants.boxSize)+GameContants.sizeOfBoardX/2;
			p.y=(int)Mathf.Floor(v.z/GameContants.boxSize)+GameContants.sizeOfBoardY/2;
			return p;
		}

		public static bool compareMovesAndPoints(Moves m, Point p){
			if(p.x==m.x && p.y==m.y)
				return true;
			return false;
		}
		public static bool validatePoint(Point p){
			if(p.x>=0 && p.x<GameContants.sizeOfBoardX && p.y>=0 && p.y<GameContants.sizeOfBoardY)
				return true;
			return false;
		}

		private static Vector3 offset=new Vector3(-GameContants.sizeOfBoardX/2.0f+0.5f,0,-GameContants.sizeOfBoardY/2.0f+0.5f);
		public static Vector3 getHitVector(Point p, float height=0){
			Vector3 pos=new Vector3(p.x,height, p.y)+offset;
			pos.x*=GameContants.boxSize;
			pos.z*=GameContants.boxSize;
			return pos;
		}
		public static Vector3 getHitVector(int x, int y, float height=0){
			Point p;
			p.x=x;
			p.y=y;
			return getHitVector(p, height);
		}
	}

}