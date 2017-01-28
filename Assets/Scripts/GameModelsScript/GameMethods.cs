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
		public static bool comparePoints(Point p1, Point p2){
			if(p1.x==p2.x && p1.y==p2.y)
				return true;
			return false;
		}
		public static bool validatePoint(Point p){
			if(p.x>=0 && p.x<GameContants.sizeOfBoardX && p.y>=0 && p.y<GameContants.sizeOfBoardY)
				return true;
			return false;
		}

		public static PlayerControlScript getPlayerCSAt(Point p){
			return getPlayerCSAt(p.x, p.y);
		}
		public static PlayerControlScript getPlayerCSAt(int x, int y){
			PlayerDetails[] opponent = GameRunningConstants.getInstance().networkPlayerScript.getPlayerDetails();
			PlayerDetails[] me = GameRunningConstants.getInstance().localPlayerScript.getPlayerDetails();

			for(int i=0;i<me.Length;i++){
				if(opponent[i].x==x && opponent[i].y==y){
					return GameRunningConstants.getInstance().networkPlayerScript.getPlayerControlScript(i);
				}
			}

			for(int i=0;i<opponent.Length;i++){
				if(me[i].x==x && me[i].y==y){
					return GameRunningConstants.getInstance().localPlayerScript.getPlayerControlScript(i);
				}
			}
			return null;
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