#define Debug

using UnityEngine;
using System.Collections;
using System;
using Sfs2X.Core;

public class NMCharacterSelection : MonoBehaviour
{
	private GUIText debugText;
	
	private void Awake()
	{
		if (!SmartFoxConnection.IsInitialized)
		{
			Application.LoadLevel("0.1 - Login");
			return;
		}
		
		SmartFoxConnection.Connection.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
		
		debugText = GameObject.Find("debugText").GetComponent<GUIText>();
		
		#if !Debug
		Destroy(debugText.gameObject);
		#endif
	}
	
	private void OnDestroy()
	{
		if (SmartFoxConnection.IsInitialized)
			SmartFoxConnection.Connection.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
	}
	
	private void OnExtensionResponse(BaseEvent evt) 
	{
		#if Debug
		print ("Extension response received.");
		debugText.text = "Extension response received.";
		try 
		{
		#endif
			string cmd = (string)evt.Params["cmd"];
			
			switch (cmd)
			{
				case "startGame":
				HandleStartGame();
				break;
			}
		#if Debug
		}
		catch (Exception e) 
		{
			print("Extension response exception handling: " + e.Message + " >>> " + e.StackTrace);
			debugText.text = "Extension response exception handling: " + e.Message + " >>> " + e.StackTrace;
		}
		#endif
	}
	
	private void HandleStartGame()
	{
		#if Debug
		print ("HandleStartGame called.");
		debugText.text = "HandleStartGame called.";
		#endif
		
		StartCoroutine(SceneChanger.ChangeScene("Baskin-Arena"));
	}
}
