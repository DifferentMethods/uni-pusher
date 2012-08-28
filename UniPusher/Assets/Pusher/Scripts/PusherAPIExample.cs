using UnityEngine;
using System.Collections;

public class PusherAPIExample : MonoBehaviour {

	
	IEnumerator Start () {
		yield return null;
		var api = GetComponent<PusherAPI>();
		while(!api.connectedToPusher) yield return null;
		api.Subscribe("public-channel", OnPublicChannel);
		api.Subscribe("private-channel", OnPrivateChannel);
		api.Send ("private-channel", "client-hello-world", null);
	}

	void OnPrivateChannel (Hashtable obj)
	{
		Debug.Log(obj);
	}

	void OnPublicChannel (Hashtable obj)
	{
		Debug.Log(obj);	
	}
	
	
}
