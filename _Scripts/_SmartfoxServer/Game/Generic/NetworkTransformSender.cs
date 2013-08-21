using UnityEngine;
using System.Collections;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;

public class NetworkTransformSender : MonoBehaviour 
{
	public int networkId = -1;
	private int localId;
	public int LocalId { get { return localId; } }
	private float lastSend;
	private float timeToSend = 0.1f;
	//private bool added = false;
	
	private Transform myTransform;
	
	private void Awake()
	{
		myTransform = transform;
		lastSend = 0f;
		
		//if (!added)
		{
			NetworkProjectileManager.Instance.myProjectiles.Add(GlobalVars.projetileIds, this);
			localId = GlobalVars.projetileIds;
			GlobalVars.projetileIds ++;
			//added = true;
		}
	}
	
	private void OnEnable()
	{
		networkId = -1;
	}
	/*
	private void Start()
	{
		if (!added)
		{
			NetworkProjectileManager.Instance.myProjectiles.Add(GlobalVars.projetileIds, this);
			localId = GlobalVars.projetileIds;
			GlobalVars.projetileIds ++;
			added = true;
		}
	}*/
	
	private void FixedUpdate() 
	{
		if (networkId != -1)
		{
			lastSend += Time.deltaTime;
			
			if(lastSend > timeToSend)
			{
				NMGame.Instance.SendTransform(NetworkTransform.fromTransform(myTransform), networkId);
				lastSend = 0f;
			}
		}
	}
}
