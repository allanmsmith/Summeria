using UnityEngine;
using System.Collections;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;

public class NetworkPlayerTransform
{
	public Vector3 position;
	public Quaternion rotation;
	public Quaternion rotationMovement;
	public Quaternion rotationBone;
	
	public double timeStamp = 0;
	
	public NetworkPlayerTransform()
	{
		
	}
	
	public void Update(ISFSObject data)
	{
		
	}
	
	public static NetworkPlayerTransform fromSFSObject(ISFSObject data)
	{
		NetworkPlayerTransform nTransform = new NetworkPlayerTransform();
		nTransform.position = new Vector3(data.GetFloat("x"), data.GetFloat("y"), data.GetFloat("z"));
		nTransform.rotation = Quaternion.Euler(new Vector3(data.GetFloat("rx"), data.GetFloat("ry"), data.GetFloat("rz")));
		nTransform.rotationMovement = Quaternion.Euler(new Vector3(data.GetFloat("rMx"), data.GetFloat("rMy"), data.GetFloat("rMz")));
		nTransform.rotationBone = Quaternion.Euler(new Vector3(data.GetFloat("rBx"), data.GetFloat("rBy"), data.GetFloat("rBz")));
		nTransform.timeStamp = data.GetDouble("time");
		
		//Debug.Log("fromSFSObject, timeStamp: " + data.GetDouble("time"));
		
		return nTransform;
	}
	
	public static NetworkPlayerTransform fromTransform(Transform sendTransform, Quaternion rotMovement, Quaternion rotBone)
	{
		NetworkPlayerTransform nTransform = new NetworkPlayerTransform();
		nTransform.position = sendTransform.position;
		nTransform.rotation = sendTransform.rotation;
		nTransform.rotationMovement = rotMovement;
		nTransform.rotationBone = rotBone;
		
		return nTransform;
	}
	
	public void toSFSObject(ISFSObject data)
	{
		data.PutFloat("x", position.x);
		data.PutFloat("y", position.y);
		data.PutFloat("z", position.z);
		
		data.PutFloat("rx", rotation.eulerAngles.x);
		data.PutFloat("ry", rotation.eulerAngles.y);
		data.PutFloat("rz", rotation.eulerAngles.z);
		
		data.PutFloat("rMx", rotationMovement.eulerAngles.x);
		data.PutFloat("rMy", rotationMovement.eulerAngles.y);
		data.PutFloat("rMz", rotationMovement.eulerAngles.z);
		
		data.PutFloat("rBx", rotationBone.eulerAngles.x);
		data.PutFloat("rBy", rotationBone.eulerAngles.y);
		data.PutFloat("rBz", rotationBone.eulerAngles.z);
		
		data.PutDouble("time", timeStamp);
	}
}
