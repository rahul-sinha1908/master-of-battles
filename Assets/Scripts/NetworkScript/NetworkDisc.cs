using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public struct IPS{
	public string ip;
	public string name;
};
public class NetworkDisc : NetworkDiscovery
{
	bool b=true;
	public List<IPS> list=new List<IPS>();
	[SerializeField]
	public MyNetworkManager manager;

	public override void OnReceivedBroadcast (string fromAddress, string data)
	{
		Debug.Log (data);
		Debug.Log (fromAddress);
		//NetworkManager.singleton.networkAddress = fromAddress;
		//NetworkManager.singleton.networkPort = 7777;
		//NetworkManager.singleton.StartClient ();
		bool b = true;

		for (int i=0;i<list.Count;i++) {
			IPS k = list[i];
			if (fromAddress == k.ip) {
				b = false;
				break;
			}
		}
		if (b) {
			IPS i;
			i.ip = fromAddress;
			i.name = data;
			list.Add(i);
			if(manager==null)
				manager=GameObject.Find("NetworkManager").GetComponent<MyNetworkManager>();
			manager.AddButtons(i);
		}
		Debug.Log (list.Count);
		//StopBroadcast ();

	}
	
}