using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectProjectilePool
{
	private GameObject prefab;
	[HideInInspector]
	public List<GameObject> all;
	private bool setActiveRecursively;
	[HideInInspector]
	public int index;
	
	public void CreatePool(GameObject receivedPrefab, int initialCapacity, bool receivedSetActiveRecursively)
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
		this.setActiveRecursively = receivedSetActiveRecursively;
		PrePopulate(initialCapacity);
	}
	
	public void Spawn(Vector3 position, Quaternion rotation)
	{
		if (all.Count > 0)
		{
			Transform resultTrans = all[index].transform;
			resultTrans.position = position;
			resultTrans.rotation = rotation;
			SetActive(all[index], true);
			index ++;
			if (index >= all.Count)
				index = 0;
		}
	}
	
	public void PrePopulate (int count)
	{
		for (int i = 0; i < count; i++)
		{
			all.Add((GameObject)MonoBehaviour.Instantiate(prefab));
			SetActive(all[i], false);
		}
	}

	public void SetActive (GameObject obj, bool val)
	{
		if(setActiveRecursively)
			obj.SetActiveRecursively(val);
		else
			obj.active = val;
	}
}