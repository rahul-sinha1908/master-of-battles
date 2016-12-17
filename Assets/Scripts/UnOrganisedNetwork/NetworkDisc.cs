using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public struct IPS{
	public string ip;
	public string name;
};
public class NetworkDisc : NetworkDiscovery
{
	bool b=true;
	public LinkedList<IPS> list=new LinkedList<IPS>();

	public override void OnReceivedBroadcast (string fromAddress, string data)
	{
		Debug.Log (data);
		Debug.Log (fromAddress);
		//NetworkManager.singleton.networkAddress = fromAddress;
		//NetworkManager.singleton.networkPort = 7777;
		//NetworkManager.singleton.StartClient ();
		bool b = true;

		for (LinkedListNode<IPS> l = list.First; l != null; l = l.Next) {
			IPS i = l.Value;
			if (fromAddress == i.ip) {
				b = false;
				break;
			}
		}
		if (b) {
			IPS i;
			i.ip = fromAddress;
			i.name = data;
			LinkedListNode<IPS> node = new LinkedListNode<IPS> (i);
			list.AddLast (node);
		}
		Debug.Log (list.Count);
		//StopBroadcast ();

	}

}