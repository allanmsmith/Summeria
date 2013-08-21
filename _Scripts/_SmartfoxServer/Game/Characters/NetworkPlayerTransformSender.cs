using UnityEngine;
using System.Collections;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;

public class NetworkPlayerTransformSender : MonoBehaviour 
{
	private float lastSend;
	private float timeToSend = 0.1f;
	
	private Transform myTransform;
	private Transform movementTransform;
	public Transform MovementTranform { set { movementTransform = value; } }
	private Transform boneTransform;
	public Transform BoneTransform { set { boneTransform = value; } }
	
	private void Awake()
	{
		myTransform = transform;
		lastSend = 0f;
	}
	
	private void FixedUpdate() 
	{
		if (boneTransform != null)
		{
			lastSend += Time.deltaTime;
			
			if(lastSend > timeToSend)
			{
				NMGame.Instance.SendPlayerTransform(NetworkPlayerTransform.fromTransform(myTransform, movementTransform.localRotation, boneTransform.localRotation));
				lastSend = 0f;
			}
		}
	}
}
