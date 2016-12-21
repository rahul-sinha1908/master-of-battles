using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MasterOfBattles;
using UnityEngine;

public class PlayerProperties {
	private string fileName;
	public struct powerStruct{
		public int id,intensity;
	};
	public int healthMetre, curHealth, playerIndex, playerType;
	public Point loc;
	public List<powerStruct> powers;

	public PlayerProperties(int i){
		playerIndex=i;
		fileName="/savedPlayerProperties-"+i+".rgo";
		//Load();
		LoadInit();
		//TODO Make the function to get the datas from the file
	}
	
	public void storePlayerDetails(){
		Save();
		//TODO store the details to a file or any other system
	}
	private void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + fileName);
		bf.Serialize(file, powers);
		file.Close();
		PlayerPrefs.SetInt(fileName+"Health",healthMetre);
	}
	private void LoadInit(){
		healthMetre=50;
		powers=new List<powerStruct>();
		playerType=1;
		loc.x=playerIndex;
		loc.y=0;
	}
	private void Load() {
		if(File.Exists(Application.persistentDataPath + fileName)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
			powers = (List<powerStruct>)bf.Deserialize(file);
			file.Close();
		}else{
			powers=new List<powerStruct>();
		}
		healthMetre= PlayerPrefs.GetInt(fileName+"Health",GameContants.DefaultHealth);
	}
	public void addPOwers(int id, int intensity){
		powerStruct power;
		power.id=id;
		power.intensity=intensity;
		if(powers==null)
			powers=new List<powerStruct>();

		powers.Add(power);
	}
}