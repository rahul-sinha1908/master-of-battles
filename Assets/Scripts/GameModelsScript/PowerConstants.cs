using System.Collections;
using System.Collections.Generic;

namespace MasterOfBattles{
	public struct PowerNameId{
		public string name;
		public string description;
		public string explanationLink;
		public int id;
	};
	public struct PowerStruct{
		public int id,strength,range;
		public string otherDef;
	};

	public class PowersContants{
		static PowersContants instance;
		List<PowerNameId> powers;
		int total=1;
		//public static PowerNameId
		private PowersContants(){
			powers=new List<PowerNameId>();
			for(int i=1;i<=total;i++){
				powers.Add(addPower(i));
			}
		}
		private PowerNameId addPower(int i){
			PowerNameId p=new PowerNameId();
			p.id=i;
			if(i==0){
				p.name="Run into another Player";
				p.description="If you run into some other player in the same block, your health will be reduced with the health of the other player. And the one whose health is less than the other dies.";
			}else if(i==1){
				p.name="Straight Attack";
				p.description="It Fires a Straight attack";
				p.explanationLink="https://nolink.com";
			}else if(i==2){
				p.name="Range Attacks";
				p.description = "This attack can be fired in all the direction but it affects only the cell you have aimed this attack for.";
				p.explanationLink="some link";
			}else if(i==3){
				p.name="Ray Spreader";
				p.description="This attack is almost the same as the straight attacks. The Only difference is when it reaches the destination point without any obstruction it spreads the attack in normal directions.";
			}else if(i==4){
				p.name="Bomb Attack";
				p.description="It affects a range of place where you throw your bomb. However there is a draw back. The range which you select to get affected, it also affects that range in the direction of firing at the firing end.";
			}else if(i==5){
				p.name="Triple Shot";
				p.description="It affects It fires simultaneously in straight and two 45deg Directions over the same distance";
			}
			return p;
		}
		public static PowersContants getInstance(){
			if(instance==null)
				instance=new PowersContants();
			return instance;
		}

		public static string getPowerDefString(PowerStruct p){
			//TODO Make the Power From Here
			string s="";
			s=p.id+"|"+p.strength+"|"+p.otherDef;
			return s;
		}
		public static PowerStruct getPowerStruct(string s){
			//TODO Retrieve the Power From here
			PowerStruct p = new PowerStruct();
			string[] str=s.Split('|');
			Dev.log(Tag.PlayerAttack,"Toatal Params Got is : "+str.Length);
			if(str.Length==3){
				p.id=int.Parse(str[0]);
				p.strength=int.Parse(str[1]);
				p.otherDef=str[2];
			}
			return p;
		}
	}

}