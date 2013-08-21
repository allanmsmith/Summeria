using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectRecycler
{
	private GameObject prefab;
	[HideInInspector]
	public List<GameObject> all;
	[HideInInspector]
	public int index;
	[HideInInspector]
	public int currentIndex;
	
	public void CreatePool(GameObject receivedPrefab, int initialCapacity)
	{
		prefab = receivedPrefab;
		if (initialCapacity > 0)
		{
			this.all = new List<GameObject>(initialCapacity);
		}
		else
		{
			// Use the .NET defaults
			this.all = new List<GameObject>();
		}
		PrePopulate(initialCapacity);
	}
	
	public void CreatePool(string containerName, GameObject receivedPrefab, int initialCapacity)
	{
		prefab = receivedPrefab;
		if (initialCapacity > 0)
		{
			this.all = new List<GameObject>(initialCapacity);
		}
		else
		{
			// Use the .NET defaults
			this.all = new List<GameObject>();
		}
		PrePopulate(containerName, initialCapacity);
	}
	
	public void CreatePool(string parentName, string containerName, GameObject receivedPrefab, int initialCapacity)
	{
		prefab = receivedPrefab;
		if (initialCapacity > 0)
		{
			this.all = new List<GameObject>(initialCapacity);
		}
		else
		{
			// Use the .NET defaults
			this.all = new List<GameObject>();
		}
		PrePopulate(parentName, containerName, initialCapacity);
	}
	
	public void Spawn(Vector3 position, Quaternion rotation)
	{
		if (all.Count > 0)
		{
			Transform resultTrans = all[index].transform;
			resultTrans.position = position;
			resultTrans.rotation = rotation;
			all[index].SetActive(true);
			index ++;
			currentIndex = index - 1;
			if (index >= all.Count)
				index = 0;
		}
	}
	
	public void SpawnLocal(Vector3 position, Quaternion rotation)
	{
		if (all.Count > 0)
		{
			Transform resultTrans = all[index].transform;
			resultTrans.localPosition = position;
			resultTrans.localRotation = rotation;
			all[index].SetActive(true);
			index ++;
			currentIndex = index - 1;
			if (index >= all.Count)
				index = 0;
		}
	}
	
	public void PrePopulate (int count)
	{
		for (int i = 0; i < count; i++)
		{
			all.Add((GameObject)MonoBehaviour.Instantiate(prefab));
			all[i].SetActive(false);
		}
	}
	
	public void PrePopulate (string containerName, int count)
	{
		GameObject tempObject = new GameObject(containerName);
		for (int i = 0; i < count; i++)
		{
			all.Add((GameObject)MonoBehaviour.Instantiate(prefab));
			all[i].transform.parent = tempObject.transform;
			all[i].SetActive(false);
		}
	}
	
	public void PrePopulate (string parentName, string containerName, int count)
	{
		GameObject tempParent = GameObject.Find(parentName);
		if (tempParent == null)
			tempParent = new GameObject(parentName);
		GameObject tempObject = new GameObject(containerName);
		tempObject.transform.parent = tempParent.transform;
		for (int i = 0; i < count; i++)
		{
			all.Add((GameObject)MonoBehaviour.Instantiate(prefab));
			all[i].transform.parent = tempObject.transform;
			all[i].SetActive(false);
		}
	}
}