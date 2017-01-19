﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MasterOfBattles;
using UnityEngine;

public class PlayerProperties {
	private string fileName;
	public int healthMetre, speed, curHealth, playerIndex, playerType;
	public Point loc;
	public List<PowerStruct> powers;

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
		powers=new List<PowerStruct>();
		if(playerIndex%2==0){
			healthMetre=200;
			speed=1;
			playerType=1;
			addPowers(1,15,10);
			addPowers(2,15,10);
		}else{
			healthMetre=100;
			speed=2;
			playerType=0;
			addPowers(1,20,20);
		}
		loc.x=playerIndex%GameContants.sizeOfBoardX;
		loc.y=playerIndex/GameContants.sizeOfBoardX;
	}
	private void Load() {
		if(File.Exists(Application.persistentDataPath + fileName)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
			powers = (List<PowerStruct>)bf.Deserialize(file);
			file.Close();
		}else{
			powers=new List<PowerStruct>();
		}
		healthMetre= PlayerPrefs.GetInt(fileName+"Health",GameContants.DefaultHealth);
	}
	public void addPowers(int id, int strength, int range){
		PowerStruct power=new PowerStruct();
		power.id=id;
		power.strength=strength;
		power.range=range;
		if(powers==null)
			powers=new List<PowerStruct>();

		powers.Add(power);
	}
}