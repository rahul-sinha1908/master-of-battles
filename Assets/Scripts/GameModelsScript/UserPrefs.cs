using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterOfBattles{
	public class UserPrefs{
		public static string scrollSpeed="Scroll Speed";
		public static string moveSensitivity="Move Sensitivity";
		public static string mobPanSensitivity="Mobile Pan Sensitivity";
	}

	public enum Tag{
		UnOrdered, CheckBoard, MyPlayerScript, PlayerControlScript, PlayerMove, PlayerAttack, PlayerSelect, GameMoveListener
	}
	public class DevTag{
		public const int MAX=100;
		public bool[] list;
		private static DevTag instance; 
		private DevTag(){
			list =new bool[MAX];
			list[(int)Tag.CheckBoard]=true;
			list[(int)Tag.MyPlayerScript]=true;
			list[(int)Tag.PlayerControlScript]=true;
			list[(int)Tag.PlayerMove]=true;
			list[(int)Tag.PlayerAttack]=true;
			list[(int)Tag.PlayerSelect]=true;
			list[(int)Tag.GameMoveListener]=false;
			list[(int)Tag.UnOrdered]=false;
			//list[(int)Tag.]=true;
		}
		public bool isAllowed(Tag tag){
			if(list[(int)tag])
				return true;
			else
				return false;
		}
		public static DevTag getInstance(){
			if(instance==null)
				instance=new DevTag();
			return instance;
		}
	}
	public class Dev{
		public static void log(Tag tag, object s){
			if(DevTag.getInstance().isAllowed(tag))
				Debug.Log(tag+" : "+s);
		}
		public static void error(Tag tag, Object s){
			if(DevTag.getInstance().isAllowed(tag))
				Debug.LogError(tag+" : "+s);
		}
	}
}