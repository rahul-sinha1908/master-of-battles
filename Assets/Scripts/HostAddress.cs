using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;
using System.Net.Sockets;

public class HostAddress : NetworkManager {

	// Use this for initialization
	NetworkManagerHUD netH;
	public NetworkDiscovery discovery;
	void Start () {
		netH = GetComponent<NetworkManagerHUD> ();
		LocalIPAddress ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void OpenHost(){
		//NetworkManager.singleton.networkAddress = "197.121.1.111";
		NetworkManager.singleton.StartHost ();
		netH.enabled = false;
		discovery.broadcastData = "My Game";
		discovery.Initialize ();
		discovery.broadcastData = "My Game";
		discovery.StartAsServer ();
	}
	public void ListenHost(){
		discovery.Initialize ();
		discovery.StartAsClient ();
	}
	public string LocalIPAddress()
	{
		IPHostEntry host;
		string localIP = "";
		host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (IPAddress ip in host.AddressList)
		{
			Debug.Log (ip);
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				Debug.Log ("Internet " + ip);
				localIP = ip.ToString();
				break;
			}
		}
		return localIP;
	}
}
