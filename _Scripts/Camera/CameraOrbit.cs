using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour
{
	public Transform targetTransform;
    public float distanceToTarget = 5f;
    public float xSensitivity = 120f;
    public float ySensitivity = 120f;
	public float zSensitivity = 5f;
 
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
 
    public float distanceMin = 2f;
    public float distanceMax = 15f;
 
    private float xValue;
    private float yValue;
	
	//Cache
	private Transform myTransform;
	public Transform CameraTransform { get { return myTransform; } }
	
	//Config
	private float rotationDumpValue = 0.02f;
	private string mouseAxisX = "Mouse X";
	private string mouseAxisY = "Mouse Y";
	private string mouseAxisScroll = "Mouse ScrollWheel";
	private float targetDistanceToTarget;
	
	//Declaration
	public LayerMask layerToHit;
	private Quaternion rotation;
	private RaycastHit hit;
	
	private static CameraOrbit instance;
	public static CameraOrbit Instance
	{
		get 
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType(typeof(CameraOrbit)) as CameraOrbit;
			return instance;
		}
	}
 
	private void Awake()
	{
		instance = this;
		myTransform = transform;
		
        xValue = myTransform.eulerAngles.y;
        yValue = myTransform.eulerAngles.x;
		
		targetDistanceToTarget = distanceToTarget;
		
		//layerToHit = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Water"));
	}
 
    private void LateUpdate()
	{
	    if (targetTransform)
		{
			//Adiciona rotacao nos eixos, impede que Y passe dos limites
	        xValue += Input.GetAxis(mouseAxisX) * xSensitivity * rotationDumpValue;
	        yValue -= Input.GetAxis(mouseAxisY) * ySensitivity * rotationDumpValue;
	        yValue = ClampAngle(yValue, yMinLimit, yMaxLimit);
	 		
			//Rotacao resultante
	        rotation = Quaternion.Euler(yValue, xValue, 0);
			
			//Distancia para o alvo
	        targetDistanceToTarget = Mathf.Clamp(targetDistanceToTarget - Input.GetAxis(mouseAxisScroll) * zSensitivity, distanceMin, distanceMax);
	 		
			//Verifica se ha algum obstaculo entre algo e camera
			if (Physics.Raycast(targetTransform.position, (myTransform.position - targetTransform.position).normalized, out hit, targetDistanceToTarget, layerToHit))
			{
				distanceToTarget = hit.distance * 0.9f;//targetDistanceToTarget - ;
			}
			else
			{
				distanceToTarget = targetDistanceToTarget * 0.9f;
			}
			/*
	        if (Physics.Linecast(targetTransform.position, myTransform.position, out hit))
			{
				
	        }
			else
		
				distanceToTarget = targetDistanceToTarget;
			}*/
			
	        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distanceToTarget);
	        Vector3 position = rotation * negDistance + targetTransform.position;
	 
	        myTransform.rotation = rotation;//Quaternion.Lerp(myTransform.rotation, rotation, 0.2f);
	        myTransform.position = position;//Vector3.Lerp(myTransform.position, position, 0.2f);
	    }
	}
 
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
