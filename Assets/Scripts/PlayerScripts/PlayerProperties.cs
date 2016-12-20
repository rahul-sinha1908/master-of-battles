using System.Collections;
using System.Collections.Generic;
using System;
using MasterOfBattles;

public class PlayerProperties {
	public int HealthMetre, curHealth, playerIndex;
	public Point loc;
	public List<int> powers;

	public PlayerProperties(int i){
		playerIndex=i;
		//TODO Make the function to get the datas from the file
	}
	
	public void storePlayerDetails(){
		
		//TODO store the details to a file or any other system
	}
}