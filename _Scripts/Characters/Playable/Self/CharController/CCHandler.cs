using UnityEngine;
using System.Collections;

public class CCHandler : MonoBehaviour
{
	//Variaveis Character Mover
	public float movementSpeed;
	public Transform movingTransform;
	
	//Variaveis Skill Handler
	public GameObject damageCollider;
	public GameObject damageColliderConstant;
	private Transform torsoTransform;
	public Transform placeholderCamPos;
	public Transform placeholderDamageFront;
	public Transform placeholderDamageFrontGround;
	public Transform placeholderSpellHand;
	public Transform placeholderFrontDirection;
	public GameObject weaponPrefab;
	//public AudioClip clipSkillOnCooldown;
	
	//Variaveis Weapon Handler
	//public Transform mainHandTransform;
	//public Transform offHandTransform;
	
	//Variaveis gerais
	private Transform myTransform;
	private Rigidbody myRigidbody;
	private Collider myCollider;
	private Animation myAnimation;
	private GameObject myGameObject;
	//private bool playerDead;

	//Variaveis de scripts
	private CCMover charMover;
	private CCAnimator charAnimator;
	private CCInputController inputController;
	private CCSkillHandler skillHandler;
	//private CharacterWeaponHandler weaponHandler;
	//private HealthHandler healthHandler;
	private HealthHandler healthHandler;
	private CharacterMotor motor;
	
	private Vector3 moveDirection;
	
	private NetworkPlayerTransformSender transformSender;
	
	private void Awake()
	{
		//Inicializa variaveis gerais
		myTransform = transform;
		myRigidbody = rigidbody;
		//myCollider = GetComponent<BoxCollider>();
		myGameObject = gameObject;
		if (SmartFoxConnection.hasConnection)
			transformSender = myGameObject.AddComponent<NetworkPlayerTransformSender>();
		healthHandler = GetComponent<HealthHandler>();
		motor = GetComponent<CharacterMotor>();
		CharacterController tempController = GetComponent<CharacterController>();
		myCollider = tempController.collider;
		
		StartCoroutine(GenerateCharacter());
	}
	
	private IEnumerator GenerateCharacter()
	{
		while (!CharacterGenerator.ReadyToUse) yield return 0;
		
		CharacterGenerator generator = CharacterGenerator.CreateWithConfig(PlayerStats.config);
		
		while (!generator.ConfigReady) yield return 0;
		
		GameObject tempObject = generator.Generate();
		tempObject.transform.parent = myTransform;
		tempObject.transform.localPosition = new Vector3(0, 0.5f, 0);
		tempObject.transform.localRotation = Quaternion.identity;
		tempObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		
		movingTransform = tempObject.transform;
		myAnimation = movingTransform.GetComponent<Animation>(); 
		
		AssignVariables();
	}
	
	private void AssignVariables()
	{
		//torsoTransform
		Camera.main.GetComponent<CameraOrbit>().targetTransform = placeholderCamPos;
		torsoTransform = myTransform.Search("Bone_Rotation");
		if (SmartFoxConnection.hasConnection)
		{
			transformSender.MovementTranform = movingTransform;
			transformSender.BoneTransform = torsoTransform;
		}
		
		//Adicionar os trails como filho para o bone certo
		GameObject tempObject = (GameObject)Instantiate(weaponPrefab, myTransform.position, Quaternion.identity);
		tempObject.transform.parent = myTransform.Search("Bone_Hand_R");
		tempObject.transform.localPosition = Vector3.zero;
		tempObject.transform.localRotation = Quaternion.identity;
		tempObject.transform.localScale = new Vector3(1, 1, 1);
		//------------------------------------------------
		
		NMGame.Instance.SendCollider(Vector3.zero, myCollider.bounds.size);
		//print (myCollider.bounds.center + "         " + myCollider.bounds.size);
		
		//healthHandler = GetComponent<HealthHandler>();
		
		//Faz com que a camera principal siga este personagem
		//PlayerStats.playerTransform = myTransform;
		//Camera.main.GetComponent<CameraSmoothFollow>().TargetTransform = transform;
		//GameObject.Find("GUICamera").GetComponent<InterfaceController>().healthHandler = healthHandler;
		
		//Adiciona scripts necessários a este gameObject
		
		
		charMover = myGameObject.AddComponent<CCMover>();
		
		
		charAnimator = myGameObject.AddComponent<CCFawkesAnimator>();
		
		
		inputController = myGameObject.AddComponent<CCInputController>();
		
		
		skillHandler = myGameObject.AddComponent<CCSkillHandler>();
		
		
		/*
		myGameObject.AddComponent<CharacterWeaponHandler>();
		weaponHandler = GetComponent<CharacterWeaponHandler>();*/
		
		//Inicializa variaveis
		charMover.MovementSpeed = movementSpeed;
		charMover.InputControllerSet = inputController;
		charMover.MyTransform = myTransform;
		charMover.MyRigidbody = myRigidbody;
		charMover.HealthHandler = healthHandler;
		charMover.FacingTransform = torsoTransform;
		charMover.MovingTransform = movingTransform;
		charMover.CharAnimator = charAnimator;
		charMover.CharSkillHandler = skillHandler;
		charMover.CharMotor = motor;
		
		charAnimator.CharMover = charMover;
		charAnimator.CharMotor = motor;
		charAnimator.TorsoTransform = torsoTransform;
		/*
		inputController.HealthHandlerSet = healthHandler;
		inputController.SkillHandler = skillHandler;*/
		
		skillHandler.MyTransform = myTransform;
		skillHandler.MyAnimation = myAnimation;
		skillHandler.MyFacing = torsoTransform;
		skillHandler.HealthHandler = healthHandler;
		skillHandler.CharMover = charMover;
		skillHandler.DamageCollider = damageCollider;
		skillHandler.DamageColliderConstant = damageColliderConstant;
		skillHandler.PlaceholderDamageFront = placeholderDamageFront;
		skillHandler.PlaceholderDamageFrontGround = placeholderDamageFrontGround;
		skillHandler.PlaceholderSpellHand = placeholderSpellHand;
		skillHandler.PlaceholderFrontDirection = placeholderFrontDirection;
		//skillHandler.ClipOnCooldown = clipSkillOnCooldown;
		//skillHandler.intController = GameObject.Find("GUICamera").GetComponent<InterfaceController>();
		
		/*
		weaponHandler.MainHandTransform = mainHandTransform;
		weaponHandler.OffHandTransform = offHandTransform;*/
		
		PlayerStats.playerTransform = myTransform;
	}
	
	
	//Funcao chamada quando o personagem morre
	public IEnumerator KillingBlow()
	{
		charAnimator.PlayDeath(); //Toca a animacao de morte
		charMover.enabled = false;
		myRigidbody.isKinematic = true;
		myRigidbody.useGravity = false;
		myCollider.enabled = false;
		yield return new WaitForSeconds(2);
		NMGame.Instance.SendRespawn();
		
		
		/*
		//Playtomic.Log.CustomMetric(Application.loadedLevelName + " deaths", "2013-01-09 - Deaths");
		charAnimator.PlayDeath(); //Toca a animacao de morte
		Destroy(charMover); //Deleta o script de movimentacao
		Destroy(myRigidbody); //Destroi o Rigidbody
		Destroy(myCollider); //Destroi o Collider
		MobileGlobal.levelLost = true; //Marca que o level foi perdido
		GameObject.Find("RequiredScripts").GetComponent<LevelHandler>().FinishLevel(); //Avisa para o handler do level que o level terminou*/
	}
	
	public void RespawnPlayer()
	{
		charAnimator.RespawnPlayer(); //Toca a animacao de morte
		myRigidbody.isKinematic = false;
		myRigidbody.useGravity = true;
		myCollider.enabled = true;
		charMover.enabled = true;
	}
}
