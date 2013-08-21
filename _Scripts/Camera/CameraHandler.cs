using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour
{
	public Transform targetTransform;
	
	private Transform myTransform;
	
	private void Awake()
	{
		myTransform = transform;
	}
	
	private void LateUpdate()
	{
		myTransform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X"), 0);
		//myTransform.LookAt(targetTransform);
		myTransform.position = Vector3.Lerp(myTransform.position, targetTransform.position, 0.1f);
	}
}
