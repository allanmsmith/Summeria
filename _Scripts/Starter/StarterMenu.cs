using UnityEngine;
using System.Collections;

public class StarterMenu : MonoBehaviour
{
	private void Awake()
	{
		if (!GameObject.Find("SmartFoxConnection(Clone)"))
			Instantiate(Resources.Load("SmartFoxConnection"));
		Screen.lockCursor = false;
	}
}
