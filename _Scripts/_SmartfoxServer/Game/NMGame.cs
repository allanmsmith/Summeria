#define Debug

using System;
using System.Collections;
using UnityEngine;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;

public class NMGame : MonoBehaviour 
{
	#if Debug
	private float lastTime = 0;
	private GUIText guiText;
	#endif
	
	private static NMGame instance;
	public static NMGame Instance
	{
		get 
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType(typeof(NMGame)) as NMGame;
			return instance;
		}
	}
	
	private void Awake()
	{
		instance = this;
	}
	
	// Use this for initialization
	private void Start () 
	{
		if (!SmartFoxConnection.IsInitialized) 
		{
			Application.LoadLevel("0.1 - Login");
			return;
		}
		
		if (!SmartFoxConnection.hasConnection)
			NetworkPlayerManager.Instance.SpawnPlayer(0);
		
		//Add delegates
		SmartFoxConnection.Connection.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
		//SmartFoxConnection.Connection.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserLeaveRoom);
		//SmartFoxConnection.Connection.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		
		SendSpawnRequest();
		
		#if Debug
		TimeManager.Instance.Init();
		guiText = GameObject.Find("Ping").GetComponent<GUIText>();
		#endif
		#if !Debug
		Destroy(GameObject.Find("Ping"));
		#endif
	}
	
	public void SendSpawnRequest() 
	{
		//Cria um objeto de data para ser enviado ao servidor
		ISFSObject data = new SFSObject();
		//Adiciona um string com a chave config
		data.PutUtfString("config", PlayerStats.config);
		//Cria um novo pedido ao servidor
		ExtensionRequest request = new ExtensionRequest("spawnMe", data, SmartFoxConnection.Connection.LastJoinedRoom);
		//Envia o pedido ao servidor
		SmartFoxConnection.Connection.Send(request);
	}
	
	public void SendProjectileSpawnRequest(int localId, Lists.ProjectileTypes projectileType)
	{
		//Cria um objeto de data para ser enviado ao servidor
		ISFSObject data = new SFSObject();
		data.PutInt("tId", localId);
		//Adiciona um int do tipo do projetil
		data.PutInt("t", (int)projectileType);
		//Adiciona um int com o ID do usuario que esta criando o projetil
		data.PutInt("o", SmartFoxConnection.Connection.MySelf.Id); //o = owner
		//Cria um novo pedido ao servidor
		ExtensionRequest request = new ExtensionRequest("spawnProjectile", data, SmartFoxConnection.Connection.LastJoinedRoom);
		//Envia o pedido ao servidor
		SmartFoxConnection.Connection.Send(request);
	}
	
	public void Update()
	{
		if (SmartFoxConnection.Connection != null)
			SmartFoxConnection.Connection.ProcessEvents();
	}
	
	#if Debug
	public void TimeSyncRequest() 
	{
		Room room = SmartFoxConnection.Connection.LastJoinedRoom;
		ExtensionRequest request = new ExtensionRequest("getTime", new SFSObject(), room);
		SmartFoxConnection.Connection.Send(request);
		
		lastTime = Time.time;
	}
	#endif
	
	public void SendCollider(Vector3 center, Vector3 size)
	{
		Room room = SmartFoxConnection.Connection.LastJoinedRoom;
		ISFSObject data = new SFSObject();
		data.PutFloat("cx", center.x);
		data.PutFloat("cy", center.y);
		data.PutFloat("cz", center.z);
		
		data.PutFloat("sx", size.x);
		data.PutFloat("sy", size.y);
		data.PutFloat("sz", size.z);
		
		ExtensionRequest request = new ExtensionRequest("sendCollider", data, room); // True flag = UDP
		SmartFoxConnection.Connection.Send(request);
	}
	
	public void SendCollision(Vector3 center, Vector3 size)
	{
		Room room = SmartFoxConnection.Connection.LastJoinedRoom;
		ISFSObject data = new SFSObject();
		data.PutFloat("cx", center.x);
		data.PutFloat("cy", center.y);
		data.PutFloat("cz", center.z);
		
		data.PutFloat("sx", size.x);
		data.PutFloat("sy", size.y);
		data.PutFloat("sz", size.z);
		
		ExtensionRequest request = new ExtensionRequest("collision", data, room); // True flag = UDP
		SmartFoxConnection.Connection.Send(request);
	}
	
	public void SendPlayerTransform(NetworkPlayerTransform nTransform)
	{
		ISFSObject data = new SFSObject();
		nTransform.toSFSObject(data);
		
		ExtensionRequest request = new ExtensionRequest("sendTransform", data, SmartFoxConnection.Connection.LastJoinedRoom); // True flag = UDP
		SmartFoxConnection.Connection.Send(request);
	}
	
	public void SendTransform(NetworkTransform nTransform, int networkId)
	{
		ISFSObject data = new SFSObject();
		nTransform.toSFSObject(data);
		data.PutInt("id", networkId);
		
		ExtensionRequest request = new ExtensionRequest("sendProjectileTransform", data, SmartFoxConnection.Connection.LastJoinedRoom); // True flag = UDP
		SmartFoxConnection.Connection.Send(request);
	}
	
	public void SendProjectileDestroy(int networkId)
	{
		ISFSObject data = new SFSObject();
		data.PutInt("id", networkId);
		
		ExtensionRequest request = new ExtensionRequest("destroyProjectile", data, SmartFoxConnection.Connection.LastJoinedRoom); // True flag = UDP
		SmartFoxConnection.Connection.Send(request);
	}
	
	public void SendAnimations(string anims)
	{
		Room room = SmartFoxConnection.Connection.LastJoinedRoom;
		ISFSObject data = new SFSObject();
		data.PutUtfString("a", anims);
		
		ExtensionRequest request = new ExtensionRequest("animation", data, room); // True flag = UDP
		SmartFoxConnection.Connection.Send(request);
	}
	
	public void SendRespawn()
	{
		Room room = SmartFoxConnection.Connection.LastJoinedRoom;
		
		ExtensionRequest request = new ExtensionRequest("spawnMe", new SFSObject(), room);
		SmartFoxConnection.Connection.Send(request);
	}
	
	// This method handles all the responses from the server
	private void OnExtensionResponse(BaseEvent evt) 
	{
		#if Debug
		try 
		{
		#endif
			string cmd = (string)evt.Params["cmd"];
			ISFSObject data = (SFSObject)evt.Params["params"];
			
			switch (cmd)
			{
				case "spawnMe":
				HandleInstantiatePlayer(data);
				break;
			
				case "spawnMinion":
				HandleSpawnMinion(data);
				break;
				
				case "spawnEnemy":
				HandleInstantiateEnemy(data);
				break;
				
				case "playerTransformMoved":
				HandlePlayerTransformReceived(data);
				break;
				
				case "projectileTransformMoved":
				HandleProjectileTransformReceived(data);
				break;
				
				case "animation":
				HandleAnimationReceived(data);
				break;
				
				case "spawnProjectile":
				HandleSpawnProjectile(data);
				break;
				
				case "destroyProjectile":
				HandleDestroyProjectile(data);
				break;
				#if Debug
				case "time":
				HandleServerTime(data);
				break;
				#endif
				case "damage":
				HandleDamageDone(data);
				break;
				
				case "death":
				HandleDeath(data);
				break;
			}
		#if Debug
		}
		catch (Exception e) 
		{
			Debug.Log("Exception handling response: " + e.Message + " >>> " + e.StackTrace);
		}
		#endif
	}
	
	#region Minion Handlers
	private void HandleSpawnMinion(ISFSObject data)
	{
		NetworkPlayerManager.Instance.SpawnMinion(data.GetInt("id"), data.GetInt("type"), data.GetInt("controlPlayer") == SmartFoxConnection.Connection.MySelf.Id);
	}
	
	private void HandleMinionTransformReceived(ISFSObject data)
	{
		int id = data.GetInt("id");
		//if (data.GetInt("controlPlayer") != SmartFoxConnection.Connection.MySelf.Id)
		//	NetworkPlayerManager.Instance.MoveMinionEnemy(id, NetworkTransform.fromSFSObject(data));
	}
	#endregion
	
	#region Character Handlers
	private void HandleInstantiatePlayer(ISFSObject data)
	{
		NetworkPlayerManager.Instance.SpawnPlayer(data.GetInt("sp"));
	}
	
	private void HandleInstantiateEnemy(ISFSObject data)
	{
		NetworkPlayerManager.Instance.SpawnEnemy(data.GetInt("id"), data.GetInt("sp"), data.GetUtfString("config"));
	}
	
	private void HandlePlayerTransformReceived(ISFSObject data)
	{
		int id = data.GetInt("id");
		
		//from enemy
		if(SmartFoxConnection.Connection.MySelf.Id != id)
		{
			NetworkPlayerManager.Instance.MoveEnemy(id, NetworkPlayerTransform.fromSFSObject(data));
		}
	}
	
	private void HandleDeath(ISFSObject data)
	{
		NetworkPlayerManager.Instance.KillPlayer();
		
		//print("Player ID Morreu: " + data.GetInt("id").ToString());
	}
	#endregion
	
	#region Damage Handlers
	private void HandleDamageDone(ISFSObject data)
	{
		int attackerId = data.GetInt("attackerId");
		int targetId = data.GetInt("targetId");
		
		if (attackerId == SmartFoxConnection.Connection.MySelf.Id)
		{
			NetworkPlayerManager.Instance.SpawnFloatingText(data.GetFloat("dmg"), false, targetId);
		}
		
		if (targetId == SmartFoxConnection.Connection.MySelf.Id)
		{
			NetworkPlayerManager.Instance.SpawnFloatingText(data.GetFloat("dmg"), true, targetId);
		}
	}
	#endregion
	
	#region Projectile Handlers
	private void HandleSpawnProjectile(ISFSObject data)
	{
		//Owner o, projectile id, type t
		NetworkProjectileManager.Instance.myProjectiles[data.GetInt("tId")].networkId = data.GetInt("id");
		if (data.GetInt("o") != SmartFoxConnection.Connection.MySelf.Id)
			NetworkProjectileManager.Instance.SpawnProjectile(data.GetInt("id"), (Lists.ProjectileTypes)data.GetInt("t"));
	}
	
	private void HandleProjectileTransformReceived(ISFSObject data)
	{
		//Not my own projectiles
		if (data.GetInt("o") != SmartFoxConnection.Connection.MySelf.Id)
			NetworkProjectileManager.Instance.MoveProjectile(data.GetInt("id"), NetworkTransform.fromSFSObject(data));
	}
	
	private void HandleDestroyProjectile(ISFSObject data)
	{
		if (data.GetInt("o") != SmartFoxConnection.Connection.MySelf.Id)
			NetworkProjectileManager.Instance.DestroyProjectile(data.GetInt("id"));
	}
	#endregion
	
	#region Generic Handlers
	private void HandleAnimationReceived(ISFSObject data)
	{
		int id = data.GetInt("id");
		//print ("Animations Received - My id: " + smartFox.MySelf.Id + "       Other id: " + id);
		//from enemy
		if(SmartFoxConnection.Connection.MySelf.Id != id)
		{
			NetworkPlayerManager.Instance.AnimEnemy(id, data.GetUtfString("a"));
		}
	}
	#endregion
	
	#if Debug
	private void HandleServerTime(ISFSObject data)
	{
		double time = data.GetDouble("t");
		TimeManager.Instance.Synchronize(time);
		guiText.text = "Ping: " + (int)TimeManager.Instance.AveragePing + " ms";
		
		//print ("Ping: " + ((Time.time - lastTime) * 1000) + " ms");		
		lastTime = Time.time;
	}
	#endif
	
	private void HandleSpawnShot(ISFSObject data)
	{
		
	}
}
