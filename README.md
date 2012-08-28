uni-pusher
==========

Unity3D + Pusher.Com


UniPusher is a Unity3D API to use pusher.com webservices.

It requires UniWeb. <http://u3d.as/content/different-methods/uni-web/1Cw>


To get started, create a new GameObject with a PusherAPI component.

Make sure you set your Pusher key in the inspector panel. Next, you
need to create your own component which uses the PusherAPI component.
The below code shows how to use a coroutine to wait for a connection,
and then subscribe to channels, assign callbacks and finally public
an event.
	
	IEnumerator Start () {
		yield return null;
		var api = GetComponent<PusherAPI>();
        //This line makes the coroutine wait until connected.
		while(!api.connectedToPusher) yield return null;
        
        //The next two lines subscribe to some channels on pusher.
        //The last argument is the method that will be called when
        //a message is received on the specified channel.
		api.Subscribe("public-channel", OnPublicChannel);
		api.Subscribe("private-channel", OnPrivateChannel);

        //This line publishes a message to a channel.
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
	
	
