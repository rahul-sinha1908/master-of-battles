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
			if(i==1){
				p.id=1;
				p.name="Straight Attack";
				p.description="It Fires a Straight attack";
				p.explanationLink="https://nolink.com";
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