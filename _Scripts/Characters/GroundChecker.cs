using UnityEngine;
using System.Collections;

public class GroundChecker : MonoBehaviour
{
	[HideInInspector]
	public bool grounded;
	
	private void OnTriggerStay(Collider col)
	{
		grounded = true;
	}
	
	private void OnTriggerExit(Collider col)
	{
		grounded = false;
	}
}
