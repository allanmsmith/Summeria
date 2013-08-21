using UnityEngine;
using System.Collections;

public class StarterGame : MonoBehaviour
{
	private void Awake()
	{
		if (!GameObject.Find("DataStore(Clone)"))
			Instantiate((GameObject)Resources.Load("DataStore"));
		Screen.lockCursor = true;
	}
}
