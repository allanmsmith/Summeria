using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;

public class NetworkPlayerManager : MonoBehaviour 
{
	public GameObject playerPrefab;
	public GameObject enemyPrefab;
	public GameObject koshPrefab;
	public Transform[] spawnPointList;
	public Transform spawnPointEnemiesList;
	
	private CCHandler charHandler;
	private Transform playerTransform;
	private Dictionary<int, NetworkPlayerTransformReceiver> characters = new Dictionary<int, NetworkPlayerTransformReceiver>();
	private Dictionary<int, NetworkCharacterAnimator> animatables = new Dictionary<int, NetworkCharacterAnimator>();
	private Dictionary<int, Transform> transforms = new Dictionary<int, Transform>();
	
	private int minionsCount;
	
	private static NetworkPlayerManager instance;
	public static NetworkPlayerManager Instance 
	{
		get 
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType(typeof(NetworkPlayerManager)) as NetworkPlayerManager;
			return instance;
		}
	}
	
	public GameObject hudTextPrefab;
	private GameObject hudRootGO;
	private Dictionary<int, ScrollCombatText> scrollTexts;
	
	private void Awake()
	{
		instance = this;
		minionsCount = 0;
		
		scrollTexts = new Dictionary<int, ScrollCombatText>();
		hudRootGO = GameObject.Find ("HUDRoot");
	}
	
	private IEnumerator Start()
	{
		while (!CharacterGenerator.ReadyToUse) yield return 0;
		
		CharacterGenerator generator = CharacterGenerator.CreateWithConfig(PlayerStats.config);
	}
	
	public void SpawnPlayer(int spawnPoint)
	{
		//First Spawn
		if (playerTransform == null)
		{
			GameObject tempObject = (GameObject)Instantiate(playerPrefab, spawnPointList[spawnPoint].position, spawnPointList[spawnPoint].rotation);
			playerTransform = tempObject.transform;
			//CreateFloatingText(SmartFoxConnection.Connection.MySelf.Id, playerTransform.Search("PlaceholderOverhead"));
			charHandler = tempObject.GetComponent<CCHandler>();
		}
		//Respawn
		else
		{
			playerTransform.position = spawnPointList[spawnPoint].position;
			charHandler.RespawnPlayer();
		}
	}
	
	public void KillPlayer()
	{
		StartCoroutine(charHandler.KillingBlow());
	}
	
	public void SpawnEnemy(int userId, int spawnPoint, string config)
	{
		//First Spawn
		if (!characters.ContainsKey(userId))
		{
			GameObject enemy = (GameObject)Instantiate(enemyPrefab, spawnPointList[spawnPoint].position, spawnPointList[spawnPoint].rotation);
			StartCoroutine(enemy.GetComponent<NetworkCharacterHandler>().GenerateCharacter(config));
			characters[userId] = enemy.GetComponent<NetworkPlayerTransformReceiver>();
			animatables[userId] = enemy.GetComponent<NetworkCharacterAnimator>();
			transforms[userId] = enemy.transform;
			CreateFloatingText(userId, enemy.transform.Search("PlaceholderOverhead"));
			//uiFollow[spawnPoint].target = enemy.transform;
		}
		//Respawn
		else
		{
		}
	}
				
	public void CreateFloatingText(int userId, Transform uiFollowTarget)
	{
		ScrollCombatText tempText = new ScrollCombatText();
		GameObject tempObject = NGUITools.AddChild(hudRootGO, hudTextPrefab);
		tempText.hudText = tempObject.GetComponentInChildren<HUDText>();
		tempText.uiFollow = tempObject.GetComponent<UIFollowTarget>();
		tempText.uiFollow.target = uiFollowTarget;
		tempText.uiFollow.gameCamera = Camera.mainCamera;
		tempText.uiFollow.uiCamera = GameObject.Find("GUICamera").GetComponent<Camera>();
		tempText.hudText.Add(" ", Color.white, 0f);
		scrollTexts.Add(userId, tempText);
	}
	
	public void SpawnFloatingText(float damage, bool self, int userId)
	{
		if (self)
		{
			scrollTexts[userId].hudText.Add(damage, Color.red, 0);
			//FloatingCombatGlobal.handler.CreateText(playerTransform.position, damage, false, self);
		}
		else
		{
			scrollTexts[userId].hudText.Add(damage, Color.white, 0);
			//FloatingCombatGlobal.handler.CreateText(transforms[userId].position, damage, false, self);
		}
	}
	
	public void SpawnMinion(int id, int type, bool controlPlayer)
	{
		Transform respawnPoint = spawnPointEnemiesList.GetChild(minionsCount);
		Instantiate(koshPrefab, respawnPoint.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)), Quaternion.Euler(0, Random.Range(0, 360), 0));
		minionsCount ++;
		if (minionsCount >= spawnPointEnemiesList.childCount)
		{
			minionsCount = 0;
		}
	}
	
	public void MoveEnemy(int userId, NetworkPlayerTransform nTransform)
	{
		characters[userId].ReceiveTransform(nTransform);
	}
	
	public void AnimEnemy(int userID, string animations)
	{
		animatables[userID].AnimationReceiver(animations);
	}
	
	[System.Serializable]
	public class ScrollCombatText
	{
		public HUDText hudText;
		public UIFollowTarget uiFollow;
	}
}
