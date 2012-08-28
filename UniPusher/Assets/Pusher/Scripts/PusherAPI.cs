using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PusherAPI : MonoBehaviour
{
	public string pusherKey = "YOUR KEY HERE";
	public string url = "http://ws.pusherapp.com/app/";
	const string VERSION = "0.1";
	const string PROTOCOL = "5";
	string socketId = null;
	
	HTTP.WebSocket ws;
	Dictionary<string, System.Action<Hashtable>> messageHandlers = new Dictionary<string, System.Action<Hashtable>>();
	
	public bool connectedToPusher = false;
	
	IEnumerator Start ()
	{		
		yield return null;
		ws = new HTTP.WebSocket ();
		ws.OnTextMessageRecv += HandleTextMessage;
		var qs = string.Format ("?client={0}-uniweb&version={1}&protocol={2}", Application.platform, VERSION, PROTOCOL);
		var completeURL = url + pusherKey + qs;
		ws.Connect (completeURL);
		yield return ws.Wait();
		if (!ws.connected) {
			Debug.LogError ("Could not connect.");	
		} else {
			while(!connectedToPusher) yield return null;
		}
	}

	void HandleTextMessage (string message)
	{
		
		var evt = HTTP.JsonSerializer.Decode (message) as Hashtable;
		var name = evt ["event"] as string;
		Hashtable data = null;
		if (evt.ContainsKey ("data")) {
			data = HTTP.JsonSerializer.Decode(evt ["data"] as string) as Hashtable;
		}
		
		switch (name) {
		case "pusher:connection_established":
			Debug.Log("Pusher.com Connection Established.");
			if (data != null)
				socketId = data ["socket_id"] as string;
			connectedToPusher = true;
			break;
			
		case "pusher:error":
			if (data != null) 
				Debug.LogError (data ["message"]);	
			break;
			
		default:
			if(data != null && data.ContainsKey("channel")) {
				var channel = data["channel"] as string;
				if(messageHandlers.ContainsKey(channel)) {
					messageHandlers[channel](data);	
				} else {
					Debug.LogWarning("Unhandled channel: " + channel);
				}
			}
			break;
		}
		
		
	}
	
	string AuthenticationString(string channel) {
		//IMPORTANT!
		//This is where you need to implement your own security scheme as recommended by pusher. The code below is insecure and should not be used in production.
		var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.ASCIIEncoding.ASCII.GetBytes("YOUR SECRET KEY HERE"));
		var hash = hmac.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", socketId, channel)));
		return pusherKey + ":" + System.BitConverter.ToString(hash).Replace("-","").ToLower();
	}

	public void Subscribe(string channel, System.Action<Hashtable> handler) {
		messageHandlers[channel] = handler;
		Send(CreateEvent("pusher:subscribe", PropertyList("channel", channel, "auth", AuthenticationString(channel))));
	}
	
	public void UnSubscribe(string channel) {
		messageHandlers.Remove(channel);
		Send(CreateEvent("pusher:unsubscribe", PropertyList("channel", channel)));
	}
	
	public void Send(string channel, string eventName, Hashtable data) {
		if(data == null) data = new Hashtable();
		var e = CreateEvent(eventName);
		e["channel"] = channel;
		data["auth"] = AuthenticationString(channel);
		e["data"] = HTTP.JsonSerializer.Encode(data);
		
		Send (e);
	}
	
	void Send (object msg)
	{
		var payload = HTTP.JsonSerializer.Encode (msg);
		ws.Send (payload);	
	}
	
	Hashtable PropertyList (params object[] keyvalues)
	{
		var h = new Hashtable ();
		for (var i=0; i<keyvalues.Length-1; i+=2) {
			h [keyvalues [i].ToString ()] = keyvalues [i + 1].ToString ();
		}
		return h;
	}
	
	Hashtable CreateEvent (string name, Hashtable properties = null)
	{
		var h = new Hashtable ();
		h ["event"] = name;
					
		if (properties != null) {
			h ["data"] = properties;
		}
		return h;
	}
		
		
	
}


