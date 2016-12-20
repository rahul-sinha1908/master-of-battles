using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MasterOfBattles;
using UnityEngine;

public class PlayerProperties {
	public struct powerStruct{
		public int id,intensity;
	};
	public int HealthMetre, curHealth, playerIndex;
	public Point loc;
	public List<powerStruct> powers;

	public PlayerProperties(int i){
		playerIndex=i;
		Load();
		//DONE Make the function to get the datas from the file
	}
	
	public void storePlayerDetails(){
		Save();
		//DONE store the details to a file or any other system
	}
	public void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedPlayerProperties.rgo");
		bf.Serialize(file, powers);
		file.Close();
	}
	public void Load() {
		if(File.Exists(Application.persistentDataPath + "/savedPlayerProperties.rgo")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedPlayerProperties.rgo", FileMode.Open);
			powers = (List<powerStruct>)bf.Deserialize(file);
			file.Close();
		}else{
			powers=new List<powerStruct>();
		}
	}

}