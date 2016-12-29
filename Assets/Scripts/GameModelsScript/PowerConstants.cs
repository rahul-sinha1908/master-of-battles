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

		public static string getPowerDef(PowerStruct p){
			string s="";

			return s;
		}
		public static PowerStruct setPowerDef(string s){
			PowerStruct p = new PowerStruct();
			
			return p;
		}
	}

}