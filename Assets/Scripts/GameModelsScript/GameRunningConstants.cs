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
		public CheckSelectScript checkSelectScript;
		public WeaponAssignmentScript weaponAssignmentScript;
		public bool disableClicks, networkPlayerInit;
		public List<Moves>  backUpOppAttackMoves, backUpMyMoves;
		public List<PlayerDetails> backUpOppPlaceMoves; 

		private GameRunningConstants(){
			disableClicks=true;
			networkPlayerInit=false;
			backUpMyMoves=new List<Moves>();
			backUpOppPlaceMoves=new List<PlayerDetails>();
			backUpOppAttackMoves=new List<Moves>();
		}

		public static GameRunningConstants getInstance(){
			if(instance==null)
				instance=new GameRunningConstants();
			return instance;
		}
		public void clearMyBackUp(){
			backUpMyMoves.Clear();
			backUpOppPlaceMoves.Clear();
			backUpOppAttackMoves.Clear();
		}
		public void addOpponentPlaceMoves(PlayerDetails m){
			backUpOppPlaceMoves.Add(m);
		}
		public void addOpponentAttackMoves(Moves m){
			backUpOppAttackMoves.Add(m);
		}
		public void addMyAttackMoves(Moves m){
			backUpMyMoves.Add(m);
		}

	}

}