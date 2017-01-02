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

	}

}