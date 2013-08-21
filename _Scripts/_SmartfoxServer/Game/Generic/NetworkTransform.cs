using UnityEngine;
using System.Collections;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;

public class NetworkTransform
{
	public Vector3 position;
	public Quaternion rotation;
	
	public double timeStamp = 0;
	
	public NetworkTransform()
	{
		
	}
	
	public void Update(ISFSObject data)
	{
		
	}
	
	public static NetworkTransform fromSFSObject(ISFSObject data)
	{
		NetworkTransform nTransform = new NetworkTransform();
		nTransform.position = new Vector3(data.GetFloat("x"), data.GetFloat("y"), data.GetFloat("z"));
		nTransform.rotation = Quaternion.Euler(new Vector3(data.GetFloat("rx"), data.GetFloat("ry"), data.GetFloat("rz")));
		nTransform.timeStamp = data.GetDouble("time");
		
		//Debug.Log("fromSFSObject, timeStamp: " + data.GetDouble("time"));
		
		return nTransform;
	}
	
	public static NetworkTransform fromTransform(Transform sendTransform)
	{
		NetworkTransform nTransform = new NetworkTransform();
		nTransform.position = sendTransform.position;
		nTransform.rotation = sendTransform.rotation;
		
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
		
		data.PutDouble("time", timeStamp);
	}
}