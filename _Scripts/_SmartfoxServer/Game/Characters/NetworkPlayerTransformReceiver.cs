using UnityEngine;
using System.Collections;

public class NetworkPlayerTransformReceiver : MonoBehaviour 
{
	Transform myTransform;
	[HideInInspector]
	public Transform movementTransform;
	[HideInInspector]
	public Transform boneTransform;
	
	private Quaternion movementTransformRotation;
	private Quaternion lastMovementTransformRotation;
	
	private Quaternion boneTransformRotation;
	private Quaternion lastBoneTransformRotation;
	
	NetworkPlayerTransformInterpolation interpolator;
	
	// Use this for initialization
	void Awake () 
	{
		myTransform = transform;
		
		interpolator = GetComponent<NetworkPlayerTransformInterpolation>();
	}
	
	public void ReceiveTransform(NetworkPlayerTransform nTransform)
	{
		//if (interpolator!=null) 
		{
			// interpolating received transform
			interpolator.ReceivedTransform(nTransform);
			
			//lastBoneTransformRotation = boneTransformRotation;
			//boneTransformRotation = nTransform.rotationBone;
			
			//lastMovementTransformRotation = movementTransformRotation;
			//movementTransformRotation = nTransform.rotationMovement;
		}
		/*else 
		{
			//lastMovementTransformRotation = movementTransformRotation;
			movementTransformRotation = nTransform.rotationMovement;
			
			//lastBoneTransformRotation = boneTransformRotation;
			boneTransformRotation = nTransform.rotationBone;
			
			myTransform.position = nTransform.position;
			myTransform.localRotation = nTransform.rotation;
			movementTransform.localRotation = nTransform.rotationMovement;
			boneTransform.localRotation = nTransform.rotationBone;
		}*/
		
	}
	
	/*
	private void LateUpdate()
	{
		if (movementTransform != null)
		{
			lastMovementTransformRotation = Quaternion.Lerp(lastMovementTransformRotation, movementTransformRotation, 0.2f);// boneTransformRotation;
			movementTransform.localRotation = lastMovementTransformRotation;// boneTransformRotation;
		}
		
		if (boneTransform != null)
		{
			lastBoneTransformRotation = Quaternion.Lerp(lastBoneTransformRotation, boneTransformRotation, 0.2f);// boneTransformRotation;
			boneTransform.localRotation = lastBoneTransformRotation;// boneTransformRotation;
		}
	}*/
}
