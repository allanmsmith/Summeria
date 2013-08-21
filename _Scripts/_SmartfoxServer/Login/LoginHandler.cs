#define Debug

using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;


public class LoginHandler : MonoBehaviour
{
	public string serverAddress = "127.0.0.1";
	public int serverPort = 9933;
	public string zoneName = "Summeria";
	public bool debug = false;
	
	private string userName = "";
	private string passWord = "";
	private string loginErrorMessage = "";
	public bool autoLogin = true;
	
	private int loggedIn;
	private int roomGroupsSubscribed;
	
	private GUIText debugText;
	
	private void Awake() 
	{
		// In a webplayer (or editor in webplayer mode) we need to setup security policy negotiation with the server first
		#if UNITY_WEBPLAYER || UNITY_EDITOR
		/*if (!Security.PrefetchSocketPolicy(serverAddress, serverPort, 500)) 
		{
			Debug.LogError("Security Exception. Policy file load failed!");
		}*/
		#endif
		if (!SmartFoxConnection.IsInitialized) 
			SmartFoxConnection.Connection = new SmartFox(debug);

		// Register callback delegate
		SmartFoxConnection.Connection.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		SmartFoxConnection.Connection.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		SmartFoxConnection.Connection.AddEventListener(SFSEvent.LOGIN, OnLogin);
		SmartFoxConnection.Connection.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
		SmartFoxConnection.Connection.AddEventListener(SFSEvent.UDP_INIT, OnUdpInit);
		SmartFoxConnection.Connection.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		SmartFoxConnection.Connection.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
		SmartFoxConnection.Connection.AddEventListener(SFSEvent.LOGOUT, OnLogout);
		SmartFoxConnection.Connection.AddEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE, OnSubscribeRoomGroup);
	    SmartFoxConnection.Connection.AddEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE_ERROR, OnSubscribeRoomGroupError);
		
		SmartFoxConnection.Connection.Connect(serverAddress, serverPort);
		
		loggedIn = -1;
		debugText = GameObject.Find("debugText").GetComponent<GUIText>();
		
		#if !Debug
		Destroy(debugText.gameObject);
		#endif
	}
	
	//Remove os event listeners
	private void OnDestroy()
	{
		SmartFoxConnection.Connection.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
		SmartFoxConnection.Connection.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		SmartFoxConnection.Connection.RemoveEventListener(SFSEvent.LOGIN, OnLogin);
		SmartFoxConnection.Connection.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
		SmartFoxConnection.Connection.RemoveEventListener(SFSEvent.UDP_INIT, OnUdpInit);
		SmartFoxConnection.Connection.RemoveEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		SmartFoxConnection.Connection.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
		SmartFoxConnection.Connection.RemoveEventListener(SFSEvent.LOGOUT, OnLogout);
		SmartFoxConnection.Connection.RemoveEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE, OnSubscribeRoomGroup);
	    SmartFoxConnection.Connection.RemoveEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE_ERROR, OnSubscribeRoomGroupError);
	}
	
	private void OnGUI()
	{
		GUI.Label(new Rect ((Screen.width - 200) * 0.5f, (Screen.height - 30) * 0.5f - 20, 200, 30), "Username:");
		userName = GUI.TextField (new Rect ((Screen.width - 200) * 0.5f, (Screen.height - 30) * 0.5f, 200, 30), userName, 25);
		GUI.Label(new Rect ((Screen.width - 200) * 0.5f, (Screen.height - 30) * 0.5f + 30, 200, 30), "Password:");
		passWord = GUI.TextField (new Rect ((Screen.width - 200) * 0.5f, (Screen.height - 30) * 0.5f + 50, 200, 30), passWord, 25);
		if (loggedIn == -2)
		{
			if (GUI.Button(new Rect ((Screen.width - 200) * 0.5f, (Screen.height - 30) * 0.5f + 90, 200, 30), "Play Offline"))
				Application.LoadLevel("0.2 - Character Selection");
		}
		else if (loggedIn == -1)
			GUI.Button(new Rect ((Screen.width - 200) * 0.5f, (Screen.height - 30) * 0.5f + 90, 200, 30), "Connecting to server...");
		else if (loggedIn == 0)
		{
			if (GUI.Button(new Rect ((Screen.width - 200) * 0.5f, (Screen.height - 30) * 0.5f + 90, 200, 30), "Login"))
			{
				//Manda o pedido de login
				SmartFoxConnection.Connection.Send(new LoginRequest(userName, "", zoneName));
				loggedIn = 1;
			}
		}
		else if (loggedIn == 1)
			GUI.Button(new Rect ((Screen.width - 200) * 0.5f, (Screen.height - 30) * 0.5f + 90, 200, 30), "Logging in...");
		else
			GUI.Button(new Rect ((Screen.width - 200) * 0.5f, (Screen.height - 30) * 0.5f + 90, 200, 30), "Logged in");
		GUI.Label(new Rect ((Screen.width - 200) * 0.5f, (Screen.height - 30) * 0.5f + 120, 200, 30), "Connected to " + serverAddress + ":" + serverPort);
	}
	
	//Chamado quando a conexao eh completada com ou sem sucesso
	public void OnConnection(BaseEvent evt) 
	{
		bool success = (bool)evt.Params["success"];
		
		if (success) 
		{
			loggedIn = 0;
			SmartFoxConnection.hasConnection = true;
			if (autoLogin)
				SmartFoxConnection.Connection.Send(new LoginRequest(userName, "", zoneName));
			#if Debug
			print ("Connection successfully estabilished.");
			debugText.text = "Connection successfully estabilished.";
			#endif
		}
		#if Debug
		else
		{
			loggedIn = -2;
			SmartFoxConnection.hasConnection = false;
			print ("Connection failed. Error: " + (string)evt.Params["error"]);
			debugText.text = "Connection failed. Error: " + (string)evt.Params["error"];
		}
		#endif
	}
	
	//Chamado quando a conexao eh finalizada
	public void OnConnectionLost(BaseEvent evt) 
	{
		loginErrorMessage = "Connection lost / no connection to server";
	}
	
	//Chamado quando o login acontece sem erros
	public void OnLogin(BaseEvent evt) 
	{
		#if Debug
		print ("Logged in successfully. User: " + evt.Params["user"]);
		debugText.text = "Logged in successfully. User: " + evt.Params["user"];
		#endif
		SmartFoxConnection.Connection.InitUDP(serverAddress, serverPort);	
		loggedIn = 2;
	}
	
	//Chamado quando o login nao funciona
	public void OnLoginError(BaseEvent evt) 
	{
		#if Debug
		print (evt.Params.Count);
		print (evt.Params["errorMessage"]);
		debugText.text = (string)evt.Params["errorMessage"];
		#endif
		loggedIn = 0;
	}
	
	//Quando conexao UDP acontece
	public void OnUdpInit(BaseEvent evt) 
	{
		bool success = (bool)evt.Params["success"];
		
		if (success)
		{
			#if Debug
			print ("UDP connection is working. Subscribing to room groups.");
			debugText.text = "UDP connection is working. Subscribing to room groups.";
			#endif
			SmartFoxConnection.Connection.Send(new SubscribeRoomGroupRequest("2xgame"));
			SmartFoxConnection.Connection.Send(new SubscribeRoomGroupRequest("3xgame"));
			SmartFoxConnection.Connection.Send(new SubscribeRoomGroupRequest("4xgame"));
		}
		#if Debug
		else
		{
			print ("UDP connection has errors: " + (string)evt.Params["errorMessage"]);
			debugText.text = "UDP connection has errors: " + (string)evt.Params["errorMessage"];
		}
		#endif
	}
	
	public void OnSubscribeRoomGroup(BaseEvent evt)
	{
		#if Debug
		print ("Successfully subscribed to room group:" + (string)evt.Params["groupId"] + ".");
		debugText.text = "Successfully subscribed to room group:" + (string)evt.Params["groupId"] + ".";
		#endif
		roomGroupsSubscribed ++;
		if (roomGroupsSubscribed >= 3)
		{
			#if Debug
			print ("Successfully subscribed to all room groups. Now joining the lobby room.");
			debugText.text = "Successfully subscribed to all room groups. Now joining the lobby room.";
			#endif
			if (SmartFoxConnection.Connection.LastJoinedRoom != null)
				SmartFoxConnection.Connection.Send(new JoinRoomRequest("Lobby", "", SmartFoxConnection.Connection.LastJoinedRoom.Id));
			else
				SmartFoxConnection.Connection.Send(new JoinRoomRequest("Lobby", ""));
		}
	}
	public void OnSubscribeRoomGroupError(BaseEvent evt)
	{
		#if Debug
		print ("Could not subscribe to room group. Error message:" + (string)evt.Params["errorMessage"]);
		debugText.text = "Could not subscribe to room group. Error message:" + (string)evt.Params["errorMessage"];
		#endif
	}
	
	//Chamado quando o jogador conseguir entrar no Lobby
	private void OnRoomJoin(BaseEvent evt)
	{
		#if Debug
		print ("Successfully joined the lobby. Loading new scene.");
		debugText.text = "Successfully joined the lobby. Loading new scene.";
		#endif
		Application.LoadLevel("0.2 - Character Selection");
	}
	
	//Chamado quando o jogador nao conseguir entrar no Lobby
	private void OnRoomJoinError(BaseEvent evt)
	{
		#if Debug
		print ("Could not join the lobby room. Error: " + (string)evt.Params["errorMessage"]);
		print ("Logging out.");
		debugText.text = "Logging out.";
		#endif
		SmartFoxConnection.Connection.Send(new LogoutRequest());
	}
	
	//Chamado quando o jogador der logout
	private void OnLogout(BaseEvent evt)
	{
		#if Debug
		print ("Logged out.");
		debugText.text = "Logged out.";
		#endif
		loggedIn = 0;
	}
}
