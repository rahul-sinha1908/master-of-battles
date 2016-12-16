using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkDisc : NetworkDiscovery
{

	public override void OnReceivedBroadcast (string fromAddress, string data)
	{
		Debug.Log (data);
		NetworkManager.singleton.networkAddress = fromAddress;
		NetworkManager.singleton.networkPort = 7777;
		NetworkManager.singleton.StartClient();
	}

}