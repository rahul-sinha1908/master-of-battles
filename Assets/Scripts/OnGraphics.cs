using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnGraphics : NetworkBehaviour {
	
	public NetworkDisc disc;
	LinkedList<IPS> list;

	void Start () {
		list = disc.list;
	}
	void OnGUI(){
		int y = 100;
		//Debug.Log (" 2 ");
		for (LinkedListNode<IPS> l = list.First; l != null; l = l.Next) {
			IPS i = l.Value;
			Debug.Log ("From GUI : " + i.name);
			if (GUI.Button (new Rect (400, y, 100, 50), i.name)) {
				NetworkManager.singleton.networkAddress = i.ip;
			}
			y += 60;
		}
	}
}
