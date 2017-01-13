using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterOfBattles{
	public class GameRunningConstants{
		private static GameRunningConstants instance;
		
		public WeaponControlScript weaponControlScript;
		public InputManager inputManager;
		public DisplayInformation displayInformation;
		public MyPlayerScript localPlayerScript, networkPlayerScript;
		public GameMoveListener gameMoveListener;

		private GameRunningConstants(){
			
		}

		public static GameRunningConstants getInstance(){
			if(instance==null)
				instance=new GameRunningConstants();
			return instance;
		} 
	}

}