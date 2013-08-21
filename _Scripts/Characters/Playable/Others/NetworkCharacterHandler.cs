using UnityEngine;
using System.Collections;

public class NetworkCharacterHandler : MonoBehaviour
{
	//Variaveis gerais
	public GameObject swordHolder;
	
	private Transform myTransform;
	private Rigidbody myRigidbody;
	private Collider myCollider;
	private Animation myAnimation;
	private GameObject myGameObject;
	private NetworkCharacterFawkesAnimator charAnimator;
	
	private NetworkPlayerTransformReceiver transformReceiver;
	private NetworkPlayerTransformInterpolation transformInterpolation;
	
	private void Awake()
	{
		//Inicializa variaveis gerais
		myTransform = transform;
		myRigidbody = rigidbody;
		myCollider = collider;
		myGameObject = gameObject;
		charAnimator = GetComponent<NetworkCharacterFawkesAnimator>();
		transformReceiver = GetComponent<NetworkPlayerTransformReceiver>();
		transformInterpolation = GetComponent<NetworkPlayerTransformInterpolation>();
		//statusHandler = GetComponent<StatusHandler>();
	}
	
	public IEnumerator GenerateCharacter(string config)
	{
		while (!CharacterGenerator.ReadyToUse) yield return 0;
		
		CharacterGenerator generator = CharacterGenerator.CreateWithConfig(config);
		
		while (!generator.ConfigReady) yield return 0;
		
		GameObject tempObject = generator.Generate();
		tempObject.transform.parent = myTransform;
		tempObject.transform.localPosition = new Vector3(0, 0.5f, 0);
		tempObject.transform.localRotation = Quaternion.identity;
		tempObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		
		Transform tempTransform = tempObject.transform.Search("Bone_Rotation");
		
		//Adicionar os trails como filho para o bone certo
		GameObject tempSword = (GameObject)Instantiate(swordHolder, myTransform.position, Quaternion.identity);
		tempSword.transform.parent = myTransform.Search("Bone_Hand_R");
		tempSword.transform.localPosition = Vector3.zero;
		tempSword.transform.localRotation = Quaternion.identity;
		tempSword.transform.localScale = new Vector3(1, 1, 1);
		//------------------------------------------------
		
		transformReceiver.movementTransform = tempObject.transform;
		transformReceiver.boneTransform = tempTransform;
		
		transformInterpolation.movementTransform = tempObject.transform;
		transformInterpolation.boneTransform = tempTransform;
		transformInterpolation.StartReceiving();
		
		charAnimator.myAnimation = GetComponentInChildren<Animation>();
		charAnimator.TorsoTransform = tempTransform;
		charAnimator.PrepareAnimations();
		//transformInterpolation
		
		//movingTransform = tempObject.transform;
		//myAnimation = movingTransform.GetComponent<Animation>(); 
		
		//AssignVariables();
	}
}
