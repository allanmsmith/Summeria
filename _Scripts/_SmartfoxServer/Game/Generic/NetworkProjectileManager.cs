using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;

public class NetworkProjectileManager : MonoBehaviour 
{
	public GameObject swordBoomerang;
	
	
	public Dictionary<int, NetworkTransformSender> myProjectiles = new Dictionary<int, NetworkTransformSender>();
	private Dictionary<int, NetworkTransformReceiver> projectiles = new Dictionary<int, NetworkTransformReceiver>();
	//private Dictionary<int, NetworkCharacterAnimator> animatables = new Dictionary<int, NetworkCharacterAnimator>();
	private Dictionary<int, Transform> transforms = new Dictionary<int, Transform>();
	
	private static NetworkProjectileManager instance;
	public static NetworkProjectileManager Instance 
	{
		get 
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType(typeof(NetworkProjectileManager)) as NetworkProjectileManager;
			return instance;
		}
	}
	
	private void Awake()
	{
		instance = this;
		projectiles = new Dictionary<int, NetworkTransformReceiver>();
	}
	
	public void SpawnProjectile(int id, Lists.ProjectileTypes type)
	{
		GameObject tempObject = null;
		switch (type)
		{
			case Lists.ProjectileTypes.SwordBoomerang:
			tempObject = (GameObject)Instantiate(swordBoomerang, new Vector3(100000, 100000, 100000), Quaternion.identity);
			break;
		}
		
		projectiles.Add(id, tempObject.GetComponent<NetworkTransformReceiver>());
	}
	
	public void MoveProjectile(int id, NetworkTransform nTransform)
	{
		projectiles[id].ReceiveTransform(nTransform);
	}
	
	public void DestroyProjectile(int id)
	{
		projectiles[id].gameObject.SendMessage("DestroyProjectile", SendMessageOptions.DontRequireReceiver);
	}
	/*
	public void AnimEnemy(int userID, string animations)
	{
		animatables[userID].AnimationReceiver(animations);
	}*/
}
