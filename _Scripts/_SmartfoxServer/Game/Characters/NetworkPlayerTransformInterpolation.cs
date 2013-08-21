using System;
using System.Collections;
using UnityEngine;

// This script is used to interpolate and predict values to make the position smooth
// And correspond to the real one.
public class NetworkPlayerTransformInterpolation : MonoBehaviour
{
	public enum InterpolationMode 
	{
		INTERPOLATION,
		EXTRAPOLATION
	}
	
	public InterpolationMode mode = InterpolationMode.INTERPOLATION;
	
	private double interpolationBackTime = 100; 
	
	// The maximum time we try to extrapolate
	private float extrapolationForwardTime = 1000; // Can make this depend on ping if needed
		
	private bool running = false;
	
	// We store twenty states with "playback" information
	NetworkPlayerTransform[] bufferedStates = new NetworkPlayerTransform[20];
	// Keep track of what slots are used
	int statesCount = 0;
	
	[HideInInspector]
	public Transform movementTransform;
	[HideInInspector]
	public Transform boneTransform;
	private Quaternion boneTransformRotation;
	
	private Transform myTransform;
	private GameObject myGameObject;
	//public GameObject myBoneRotation;
	//public GameObject myTransformRotation;
	
	private void Awake()
	{
		myTransform = transform;
		myGameObject = gameObject;
	}
	
	// We call it on remote player to start receiving his transform
	public void StartReceiving() 
	{
		running = true;
		//print ("testeReceiving");
	}
	
	public void ReceivedTransform(NetworkPlayerTransform ntransform) 
	{
		if (!running) return;
		
		// When receiving, buffer the information
		// Receive latest state information
		//Vector3 pos = ntransform.position;
		//Quaternion rot = ntransform.rotation;
				
		// Shift buffer contents, oldest data erased, 18 becomes 19, ... , 0 becomes 1
		for (int i = bufferedStates.Length - 1; i >= 1; i--) 
		{
			bufferedStates[i] = bufferedStates[i-1];
		}
		 
		// Save currect received state as 0 in the buffer, safe to overwrite after shifting
		bufferedStates[0] = ntransform;
		
		// Increment state count but never exceed buffer size
		statesCount = Mathf.Min(statesCount + 1, bufferedStates.Length);

		// Check integrity, lowest numbered state in the buffer is newest and so on
		for (int i = 0; i < statesCount - 1; i++) 
		{
			if (bufferedStates[i].timeStamp < bufferedStates[i + 1].timeStamp)
			{
				print("Latest received transform is indeed the last sent transform.");
			}
		}
		
		if (bufferedStates[0] != null && bufferedStates[1] != null)
		{
			float timeToTween = (float)(bufferedStates[0].timeStamp - bufferedStates[1].timeStamp) * 0.001f;
			TweenPosition.Begin(myGameObject, timeToTween, bufferedStates[0].position);
			TweenRotation.Begin(myGameObject, timeToTween, bufferedStates[0].rotation);
			TweenRotation.Begin(movementTransform.gameObject, timeToTween, bufferedStates[0].rotationMovement);
			TweenRotation.Begin(boneTransform.gameObject, timeToTween, bufferedStates[0].rotationBone);
		}
	}
	

	// This only runs where the component is enabled, which is only on remote peers (server/clients)
	void LateUpdate() 
	{
		if (!running) return;
		if (statesCount == 0) return;
		
		UpdateValues();
		
		double currentTime = TimeManager.Instance.NetworkTime;
		double interpolationTime = currentTime - interpolationBackTime;
		
		//myTransform.position = Vector3.Lerp(myTransform.position, bufferedStates[0].position, 0.1f);
		
		//TweenPosition.Begin(myGameObject, (float)interpolationBackTime * 0.001f, bufferedStates[0].position).ignoreTimeScale = false;
		//TweenRotation.Begin(myGameObject, (float)interpolationBackTime * 0.001f, bufferedStates[0].rotation).ignoreTimeScale = false;
		
		// We have a window of interpolationBackTime where we basically play 
		// By having interpolationBackTime the average ping, you will usually use interpolation.
		// And only if no more data arrives we will use extrapolation
		
		// Use interpolation
		// Check if latest state exceeds interpolation time, if this is the case then
		// it is too old and extrapolation should be used
		
		//print ("bufferedStates[0].timeStamp: " + bufferedStates[0].timeStamp);
		//print ("interpolationTime: " + interpolationTime);
		
		//if (mode == InterpolationMode.INTERPOLATION && bufferedStates[0].timeStamp > interpolationTime) 
		{
			//myTransform.position = Vector3.Lerp(myTransform.position, bufferedStates[0].position, 0.1f);
			/*
			for (int i = 0;i < statesCount; i++) 
			{
				
				
				//TweenPosition.Begin(myGameObject, 0.1f, bufferedStates[0].position).ignoreTimeScale = false;
				//TweenRotation.Begin(myGameObject, 0.1f, bufferedStates[0].rotation).ignoreTimeScale = false;
				//myTransform.position = 
				/*
				// Find the state which matches the interpolation time (time + 0.1) or use last state
				if (bufferedStates[i].timeStamp <= interpolationTime || i == statesCount-1) 
				{
					// The state one slot newer (<100ms) than the best playback state
					NetworkPlayerTransform rhs = bufferedStates[Mathf.Max(i - 1, 0)];
					// The best playback state (closest to 100 ms old (default time))
					NetworkPlayerTransform lhs = bufferedStates[i];
					
					// Use the time between the two slots to determine if interpolation is necessary
					double length = rhs.timeStamp - lhs.timeStamp;
					float t = 0.0F;
					// As the time difference gets closer to 100 ms t gets closer to 1 in 
					// which case rhs is only used
					if (length > 0.0001) 
					{
						t = (float)((interpolationTime - lhs.timeStamp) / length);
					}
					
					// if t=0 => lhs is used directly
					transform.position = Vector3.Lerp(lhs.position, rhs.position, t);
					transform.rotation = Quaternion.Slerp(lhs.rotation, rhs.rotation, t);
					//print (movementTransform);
					if (movementTransform != null)
						movementTransform.localRotation = Quaternion.Slerp(lhs.rotationMovement, rhs.rotationMovement, t);
					if (boneTransform != null)
					{
						//boneTransformRotation = Quaternion.Slerp(lhs.rotationBone, rhs.rotationBone, t);
						//boneTransform.localRotation = boneTransformRotation;
					}
					return;
				}*/
			//}
		} 
		/*else 
		{
			// If the value we have is too old, use extrapolation based on 2 latest positions
			float extrapolationLength = Convert.ToSingle(currentTime - bufferedStates[0].timeStamp)/1000.0f;
			if (mode == InterpolationMode.EXTRAPOLATION && extrapolationLength < extrapolationForwardTime && statesCount > 1) 
			{
				Vector3 dif = bufferedStates[0].position - bufferedStates[1].position;
				float distance = Vector3.Distance(bufferedStates[0].position, bufferedStates[1].position);
				float timeDif = Convert.ToSingle(bufferedStates[0].timeStamp - bufferedStates[1].timeStamp)/1000.0f;
				
				if (Mathf.Approximately(distance, 0) || Mathf.Approximately(timeDif, 0)) 
				{
					//transform.position = new Vector3(transform.position.x, bufferedStates[0].position.y, transform.position.z);
					transform.position = bufferedStates[0].position;
					transform.rotation = bufferedStates[0].rotation;
					if (movementTransform != null)
						movementTransform.localRotation = bufferedStates[0].rotationMovement;
					if (boneTransform != null)
						boneTransform.localRotation = bufferedStates[0].rotationBone;
					return;
				}
						
				float speed = distance / timeDif;
				
				dif = dif.normalized;
				Vector3 expectedPosition = bufferedStates[0].position + dif * extrapolationLength * speed;
				transform.position = Vector3.Lerp(transform.position, expectedPosition, Time.deltaTime*speed); 
			}
			else 
			{
				transform.position = bufferedStates[0].position;
			}
					
			// No extrapolation for the rotation
			transform.rotation = bufferedStates[0].rotation;
			if (movementTransform != null)
				movementTransform.localRotation = bufferedStates[0].rotationMovement;
			if (boneTransform != null)
				boneTransform.localRotation = bufferedStates[0].rotationBone;
		}*/
	}
	
	private void UpdateValues() 
	{
		double ping = TimeManager.Instance.AveragePing;
		
		if (ping < 50) 
		{
			interpolationBackTime = 50;
		}
		else if (ping < 100) 
		{
			interpolationBackTime = 100;
		}
		else if (ping < 200) 
		{
			interpolationBackTime = 200;
		}
		else if (ping < 400) 
		{
			interpolationBackTime = 400;
		}
		else if (ping < 600) 
		{
			interpolationBackTime = 600;
		}
		else 
		{
			interpolationBackTime = 1000;
		}
	}
}
