using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiObjectRecycler
{
	private Stack available;
	private List<GameObject> all;
	
	private Transform containerTransform;
	
	public void CreatePool(int initialCapacity)
	{
		if (initialCapacity > 0)
		{
			this.available = new Stack(initialCapacity);
			this.all = new List<GameObject>(initialCapacity);
		}
		else
		{
			// Use the .NET defaults
			this.available = new Stack();
			this.all = new List<GameObject>();
		}
	}
	
	public void CreatePool(string containerName, int initialCapacity)
	{
		if (initialCapacity > 0)
		{
			this.available = new Stack(initialCapacity);
			this.all = new List<GameObject>(initialCapacity);
		}
		else
		{
			// Use the .NET defaults
			this.available = new Stack();
			this.all = new List<GameObject>();
		}
		containerTransform = new GameObject(containerName).transform;
	}
	
	public void CreatePool(string parentName, string containerName, int initialCapacity)
	{
		if (initialCapacity > 0)
		{
			this.available = new Stack(initialCapacity);
			this.all = new List<GameObject>(initialCapacity);
		}
		else
		{
			// Use the .NET defaults
			this.available = new Stack();
			this.all = new List<GameObject>();
		}
		GameObject tempObject = GameObject.Find(parentName);
		containerTransform = new GameObject(containerName).transform;
		if (tempObject != null)
			containerTransform.parent = tempObject.transform;
	}
	
	public void AddObject(GameObject receivedObject)
	{
		GameObject tempObject = (GameObject)MonoBehaviour.Instantiate(receivedObject);
		if (containerTransform != null)
			tempObject.transform.parent = containerTransform;
		all.Add(tempObject);
		Unspawn(tempObject);
	}
	
	public GameObject Spawn(Vector3 position, Quaternion rotation)
	{
		GameObject result = null;
		result = (GameObject)available.Pop();
		Transform resultTrans = result.transform;
		resultTrans.position = position;
		resultTrans.rotation = rotation;
		result.SetActive(true);
		return result;
	}
	
	public void Unspawn(GameObject obj)
	{
		if (!available.Contains(obj))
		{ 
			// Make sure we don't insert it twice.
			available.Push(obj);
			obj.SetActive(false);
		}
	}
	
	public void UnspawnAll()
	{
		for (int i = 0; i < all.Count; i++)
		{
			GameObject obj = all[i];
			if(obj.activeSelf)
				Unspawn(obj);
		}
	}
	
	public void Clear()
	{
		UnspawnAll();
		available.Clear();
		all.Clear();
	}
}