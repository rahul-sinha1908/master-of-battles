using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;
using System.Net.Sockets;

public class NetworkWork : NetworkBehaviour {
	
	// Use this for initialization
	void Start () {
		if (isServer) {
			//Debug.Log(NetworkManager.singleton.networkAddress);
			Debug.Log("My Ip Address : "+Network.player.ipAddress);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public string LocalIPAddress()
	{
		IPHostEntry host;
		string localIP = "";
		host = Dns.GetHostEntry(Dns.GetHostName());
		int i = 100;
		foreach (IPAddress ip in host.AddressList)
		{
			GUI.Button (new Rect (100, i, 200, 50), ip.ToString ());
			i += 60;

		}
		return localIP;
	}
	void OnGUI(){
		LocalIPAddress ();
	}
}
