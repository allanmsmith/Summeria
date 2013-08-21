#define Debug

using UnityEngine;
#if Debug
using System;
#endif
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;

public class CharacterSelectionHandler : MonoBehaviour 
{
	CharacterGenerator generator;
    GameObject character;
    bool usingLatestConfig;
    bool newCharacterRequested = true;
    bool firstCharacter = true;
    string nonLoopingAnimationToPlay;
	
	string config;

    const float fadeLength = .6f;
    const int typeWidth = 80;
    const int buttonWidth = 20;
    const string prefName = "Character Generator Demo Pref";
	
	bool defaultRotate = true;
	bool canRotate;
	float mouseXClicked;
	float rotVel = 0;
	const int maxRotVel = 5;
	
	bool searchingRoom = false;
	string buttonString = "Play";
	
	string maxPlayers = "4";
	
	private GUIText debugText;
	
    // Initializes the CharacterGenerator and load a saved config if any.
    IEnumerator Start()
    {
		debugText = GameObject.Find("debugText").GetComponent<GUIText>();
		
        while (!CharacterGenerator.ReadyToUse) yield return 0;
		
        if (PlayerPrefs.HasKey(prefName))
		{
            generator = CharacterGenerator.CreateWithConfig(PlayerPrefs.GetString(prefName));
			debugText.text = "Creating with predefined config.";
		}
		else
		{
            generator = CharacterGenerator.CreateWithConfig("fawkes|boots|standard|head|standard|pants|standard|torso|standard");
			debugText.text = "Creating with generic config.";
		}
		
		
		
		#if !Debug
		Destroy(debugText.gameObject);
		#endif
    }
	
	void Update()
    {
        if (generator == null)
		{
			//debugText.text = "Generator = null.";
			return;
		}
        if (usingLatestConfig)
		{
			//debugText.text = "usingLatestConfig.";
			return;
		}
        if (!generator.ConfigReady)
		{	
			//debugText.text = "Config not ready.";
			return;
		}

        usingLatestConfig = true;

        if (newCharacterRequested)
        {
            Destroy(character);
            character = generator.Generate();
            //character.animation.Play("idle1");
            //character.animation["idle1"].wrapMode = WrapMode.Loop;
            newCharacterRequested = false;

            // Start the walkin animation for the first character.
            if (!firstCharacter) return;
            firstCharacter = false;
            //if (character.animation["walkin"] == null) return;
            
            // Set the layer to 1 so this animation takes precedence
            // while it's blended in.
            //character.animation["walkin"].layer = 1;
            
            // Use crossfade, because it will also fade the animation
            // nicely out again, using the same fade length.
            //character.animation.CrossFade("walkin", fadeLength);
            
            // We want the walkin animation to have full weight instantly,
            // so we overwrite the weight manually:
            //character.animation["walkin"].weight = 1;
            
            // As the walkin animation starts outside the camera frustrum,
            // and moves the mesh outside its original bounding box,
            // updateWhenOffscreen has to be set to true for the
            // SkinnedMeshRenderer to update. This should be fixed
            // in a future version of Unity.
            character.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
        }
        else
        {
            character = generator.Generate(character);
            
            //character.animation[nonLoopingAnimationToPlay].layer = 1;
           //character.animation.CrossFade(nonLoopingAnimationToPlay, fadeLength);
            //nonLoopingAnimationToPlay = null;
        }
    }
	
	void FixedUpdate()
	{
		if(character)
		{
			if(Input.GetMouseButtonDown(0) & Input.mousePosition.x > 140)
			{
				defaultRotate = false;
				
				canRotate = true;
				
				mouseXClicked = Input.mousePosition.x;
			}
			else if(Input.GetMouseButtonUp(0))
			{
				canRotate = false;
			}
			
			if(defaultRotate)
			{
				Quaternion quaternionTo = Quaternion.Euler(character.transform.eulerAngles + new Vector3(0, -1, 0));
				
				character.transform.rotation = Quaternion.Slerp(character.transform.rotation, quaternionTo, 0.5f);
			}
			else if(canRotate)
			{
				rotVel = (mouseXClicked - Input.mousePosition.x) / 50;
				
				rotVel = Mathf.Clamp(rotVel, -maxRotVel, +maxRotVel);
				
				Quaternion quaternionTo = Quaternion.Euler(character.transform.eulerAngles + new Vector3(0, rotVel, 0));
				
				character.transform.rotation = Quaternion.Slerp(character.transform.rotation, quaternionTo, 1);
			}
		}
	}
	
	void OnGUI()
    {
        if (generator == null) return;
        GUI.enabled = usingLatestConfig;// && !character.animation.IsPlaying("walkin");
        GUILayout.BeginArea(new Rect(10, 10, typeWidth + 2 * buttonWidth + 8, 500));

        // Buttons for changing character elements.
        AddCategory("head", "Head", null);
        AddCategory("torso", "Torso", null);
        AddCategory("pants", "Pants", null);
		AddCategory("boots", "Boots", null);

        // Buttons for saving and deleting configurations.
        // In a real world application you probably want store these
        // preferences on a server, but for this demo configurations 
        // are saved locally using PlayerPrefs.
        if (GUILayout.Button("Save Configuration"))
            PlayerPrefs.SetString(prefName, generator.GetConfig());

        if (GUILayout.Button("Delete Configuration"))
            PlayerPrefs.DeleteKey(prefName);

        // Show download progress or indicate assets are being loaded.
        GUI.enabled = true;
        if (!usingLatestConfig)
        {
            float progress = generator.CurrentConfigProgress;
            string status = "Loading";
            if (progress != 1) status = "Downloading " + (int)(progress * 100) + "%";
            GUILayout.Box(status);
        }
		
		defaultRotate = GUILayout.Toggle(defaultRotate, "Default Rotation");
		
		GUILayout.Box("Max. Players (4)");
		
		maxPlayers = GUILayout.TextField(maxPlayers, GUILayout.Width(30));
		
		if(GUILayout.Button(buttonString, GUILayout.Width(100)))
		{
			if(!searchingRoom)
			{
				buttonString = "Cancel";
				searchingRoom = true;
				
				PlayerStats.config = generator.GetConfig();
				
				if (SmartFoxConnection.hasConnection)
					SearchForRooms();
				else
				{
					StartCoroutine(SceneChanger.ChangeScene("Baskin-Arena"));
				}
			}
			else
			{
				buttonString = "Play";
				searchingRoom = false;
				
				if (SmartFoxConnection.hasConnection)
					CancelRoomSearch();
			}
		}
		
        GUILayout.EndArea();
    }

    // Draws buttons for configuring a specific category of items, like pants or shoes.
    void AddCategory(string category, string displayName, string anim)
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("<", GUILayout.Width(buttonWidth)))
            ChangeElement(category, false, anim);

        GUILayout.Box(displayName, GUILayout.Width(typeWidth));

        if (GUILayout.Button(">", GUILayout.Width(buttonWidth)))
            ChangeElement(category, true, anim);

        GUILayout.EndHorizontal();
    }

    void ChangeCharacter(bool next)
    {
        generator.ChangeCharacter(next);
        usingLatestConfig = false;
        newCharacterRequested = true;
    }

    void ChangeElement(string catagory, bool next, string anim)
    {
        generator.ChangeElement(catagory, next);
        usingLatestConfig = false;
        
        if (!character.animation.IsPlaying(anim))
            nonLoopingAnimationToPlay = anim;
    }
	
	private void SearchForRooms()
	{
		List<Room> roomList = SmartFoxConnection.Connection.RoomManager.GetRoomList();
		
		#if Debug
		print ("Searching for rooms. " + roomList.Count + " room(s) found.");
		debugText.text = "Searching for rooms. " + roomList.Count + " room(s) found.";
		#endif
		
		//Se existir 1 ou menos salas criadas (Lobby sempre existe)
		if(roomList.Count <= 1)
		{
			#if Debug
			print ("No room has been created. Creating a new room.");
			debugText.text = "No room has been created. Creating a new room.";
			#endif
			CreateNewRoom();
		}
		else
		{
			int maxUsers = System.Int32.Parse(maxPlayers);
			string targetGroupId = "";
			if (maxUsers <= 2)
				targetGroupId = "2xgame";
			else if (maxUsers <= 3)
				targetGroupId = "3xgame";
			else if (maxUsers <= 4)
				targetGroupId = "4xgame";
			for (int i = 0; i < roomList.Count; i++)
			{
				if (roomList[i].GroupId == targetGroupId)
				{
					if (roomList[i].UserCount < roomList[i].MaxUsers)
					{
						#if Debug
						print ("Available room found. Joining room '" + roomList[i].Name + "', room id '" + roomList[i].Id + "'.");
						debugText.text = "Available room found. Joining room '" + roomList[i].Name + "', room id '" + roomList[i].Id.ToString() + "'.";
						#endif
						if (SmartFoxConnection.Connection.LastJoinedRoom != null)
							SmartFoxConnection.Connection.Send(new JoinRoomRequest(roomList[i].Name, "", SmartFoxConnection.Connection.LastJoinedRoom.Id));
						else
							SmartFoxConnection.Connection.Send(new JoinRoomRequest(roomList[i].Name, ""));
						return;
					}
				}
			}
			#if Debug
			print ("No room available. Creating a new room.");
			debugText.text = "No room available. Creating a new room.";
			#endif
			CreateNewRoom();
		}
	}
	
	private void CreateNewRoom()
	{
		#if Debug
		print ("Now creating room...");
		debugText.text = "Now creating room...";
		#endif
		//Configura as variaveis da sala
		//Nome da sala no construtor
		RoomSettings settings = new RoomSettings(SmartFoxConnection.Connection.MySelf.Id + " game");
		
		//Marca a sala como uma sala de jogo
		settings.IsGame = true;
		
		//Define o numero maximo de jogadores
		int maxUsers = System.Int32.Parse(maxPlayers);
		settings.MaxUsers = (short)Mathf.Min(4, maxUsers);
		
		//Coloca a sala no grupo das salas de jogo
		if (maxUsers <= 2)
			settings.GroupId = "2xgame";
		else if (maxUsers <= 3)
			settings.GroupId = "3xgame";
		else if (maxUsers >= 4)
			settings.GroupId = "4xgame";
		
		//Define que nao podem haver espectadores
		settings.MaxSpectators = 0;
		
		//Atrela a extensao do jogo a esta sala
		settings.Extension = new RoomExtension(SmartFoxConnection.ExtensionName, SmartFoxConnection.ExtensionClass);
		
		//Efetivamente cria uma nova sala com as configuracoes acima
		SmartFoxConnection.Connection.Send(new CreateRoomRequest(settings, true, SmartFoxConnection.Connection.LastJoinedRoom));
		//O servidor verificara quando a sala estiver cheia e mandara o comando para comecar o jogo, recebido na classe NMCharacterSelection
	}
	
	private void CancelRoomSearch()
	{
		#if Debug
		print ("Leaving newly joined game room and returning to the lobby.");
		debugText.text = "Leaving newly joined game room and returning to the lobby.";
		#endif
		if (SmartFoxConnection.Connection.LastJoinedRoom.Name != "Lobby")
			SmartFoxConnection.Connection.Send(new JoinRoomRequest("Lobby", "", SmartFoxConnection.Connection.LastJoinedRoom.Id));
	}
}