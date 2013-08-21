using UnityEngine;
using System.Collections;

//This class requires a movement direction

public class CCMover : MonoBehaviour
{
	//Recebe a direcao de movimento
	private Vector3 movementDirection;
	
	//Recebe o Input
	private CCInputController inputController;
	public CCInputController InputControllerSet { set { inputController = value; } }
	
	//Animacao
	private CCAnimator charAnimator;
	public CCAnimator CharAnimator { set { charAnimator = value; } }
	
	//SkillHandler
	private CCSkillHandler charSkillHandler;
	public CCSkillHandler CharSkillHandler { set { charSkillHandler = value; } }
	
	//Motor
	private CharacterMotor charMotor;
	public CharacterMotor CharMotor { set { charMotor = value; } }
	
	//Armazena o resultado da checagem se esta ou nao andando
	private bool moving;
	public bool Moving { get { return moving; } }
	
	//Armazena o resultado da checagem se esta ou nao no chao (usado para animacao)
	private bool falling;
	public bool Falling { get { return falling; } }
	
	//Velocidade de movimento
	private float movementSpeed; 
	public float MovementSpeed { set { movementSpeed = value; } get { return movementSpeed; } }
	
	//Transform de facing
	private Transform facingTransform;
	public Transform FacingTransform { set { facingTransform = value; } get { return facingTransform; } }
	
	private Transform movingTransform;
	public Transform MovingTransform { set { movingTransform = value; } get { return movingTransform; } }
	
	private Vector3 facingPoint;
	public Vector3 FacingPoint { set { facingPoint = value; } }
	
	private int facingDirection; //0 Forward, 1 Right, 2 Back, 3 Left
	public int FacingDirection { set { facingDirection = value; } get { return facingDirection; } }
	
	private HealthHandler healthHandler;
	public HealthHandler HealthHandler { set { healthHandler = value; } }
	
	//Cacheando o Transform
	private Transform myTransform;
	public Transform MyTransform { set { myTransform = value; } }
	
	//Cacheando o Rigidbody
	protected Rigidbody myRigidbody; 
	public Rigidbody MyRigidbody { set { myRigidbody = value; } }
	
	//Fator de smooth da rotacao
	private float turningSmoothing = 0.3f;
	
	//Velocidade de deslocamento do Dash
	private float dashSpeed = 70;
	
	//Tempo limite de input duplo para dash
	private float dashMaxTimer = 0.3f;
	
	//Duracao do Dash
	private float dashTime = 0.4f;
	
	//Contadores de click para dar dash (por direcao)
	private int[] dashCounter = new int[4];
	
	//Varaivel de controle se esta dando dash ou nao
	private bool dashing;
	public bool Dashing { set { dashing = value; } }
	
	private bool shouldLook = true;
	public bool ShouldLook { set { shouldLook = value; } }
	
	private Transform mainCamTransform;
	private bool jumping;
	public bool Jumping { get { return jumping; } }
	private float jumpingCooldown = 0.5f;
	private bool jumpingOnCooldown;
	private Quaternion targetRotation;
	private Quaternion lerpRotation;
	private Quaternion localRotation;
	
	private void Awake ()
	{
		//Inicializacao de variaveis
		mainCamTransform = Camera.main.transform;
		movementDirection = Vector3.zero;
		lerpRotation = Quaternion.Euler(270, 90, 0);
		//-----------------------
	}
	
	private void Start()
	{
		charMotor.movement.maxForwardSpeed = charMotor.movement.maxBackwardsSpeed = charMotor.movement.maxSidewaysSpeed = movementSpeed;
	}
	
	protected virtual void FixedUpdate()
	{
		if (!dashing)
		{
			movementDirection = ((myTransform.forward * Input.GetAxis("Vertical")) + (myTransform.right * Input.GetAxis("Horizontal"))).normalized;
			if(movementDirection == Vector3.zero)
				moving = false;
			else
				moving = true;
			charMotor.inputMoveDirection = movementDirection;
			charMotor.inputJump = Input.GetKey(KeyCode.Space);
			falling = !charMotor.grounded;
		}
	}
	
	private void LateUpdate()
	{
		if (!healthHandler.afflictedStatus.Contains(Lists.Status.SpellLocked))
		{
			if (!dashing)
			{
				if (Input.GetKeyDown(KeyCode.W))
				{
					dashCounter[0] ++;
					StartCoroutine(DashTimer(0));
					if (dashCounter[0] >= 2)
					{
						movingTransform.localRotation = Quaternion.Euler(0, 0, 0);
						StartCoroutine(Dash(myTransform.forward));
					}
				}
				if (Input.GetKeyDown(KeyCode.D))
				{
					dashCounter[1] ++;
					StartCoroutine(DashTimer(1));
					if (dashCounter[1] >= 2)
					{
						movingTransform.localRotation = Quaternion.Euler(0, 90, 0);
						StartCoroutine(Dash(myTransform.right));
					}
				}
				if (Input.GetKeyDown(KeyCode.S))
				{
					dashCounter[2] ++;
					StartCoroutine(DashTimer(2));
					if (dashCounter[2] >= 2)
					{
						movingTransform.localRotation = Quaternion.Euler(0, 180, 0);
						StartCoroutine(Dash(-myTransform.forward));
					}
				}
				if (Input.GetKeyDown(KeyCode.A))
				{
					dashCounter[3] ++;
					StartCoroutine(DashTimer(3));
					if (dashCounter[3] >= 2)
					{
						movingTransform.localRotation = Quaternion.Euler(0, 270, 0);
						StartCoroutine(Dash(-myTransform.right));
					}
				}
			}
		}
		
		//Faz o personagem olhar na direcao correta
		if (!dashing)
		{
			if (healthHandler.afflictedStatus.Contains(Lists.Status.SpellLocked) && shouldLook)
			{
				facingPoint = (myTransform.forward * 10000) + myTransform.position;
				if (facingPoint != Vector3.zero)
				{
					facingPoint.y = facingTransform.position.y;
		      		targetRotation = Quaternion.LookRotation(facingPoint - facingTransform.position, myTransform.up);
					targetRotation = Quaternion.Euler(facingTransform.eulerAngles.x, targetRotation.eulerAngles.y, facingTransform.eulerAngles.z);
					lerpRotation = Quaternion.Lerp(lerpRotation, targetRotation, 0.2f);
					facingTransform.rotation = lerpRotation;
					localRotation = facingTransform.localRotation;
					
					float dotValue = Vector3.Dot(movementDirection, facingTransform.forward);
					
					if (dotValue > 0.3f)
					{
						//forward
						facingDirection = 0;
						movingTransform.localRotation = Quaternion.Lerp(movingTransform.localRotation, Quaternion.Euler(0, Formulas.AngleAroundAxis(myTransform.forward, movementDirection, Vector3.up), 0), 0.2f);
	
					}
					else if (dotValue > -0.3f)
					{
						//sides
						float crossValue = Vector3.Cross(movementDirection, facingTransform.forward).y;
						if (crossValue > 0.9f)
						{
							//right
							facingDirection = 1;
							movingTransform.localRotation = Quaternion.Lerp(movingTransform.localRotation, Quaternion.Euler(0, 0, 0), 0.2f);
						}
						else if (crossValue < -0.9f)
						{
							//left
							facingDirection = 3;
							movingTransform.localRotation = Quaternion.Lerp(movingTransform.localRotation, Quaternion.Euler(0, 0, 0), 0.2f);
						}
					}
					else
					{
						//backwards
						facingDirection = 2;
						movingTransform.localRotation = Quaternion.Lerp(movingTransform.localRotation, Quaternion.Euler(0, Formulas.AngleAroundAxis(myTransform.forward, -movementDirection, Vector3.up), 0), 0.2f);
					}
				}
				
				//if (moving)
				{
					Vector3 tempCamPos = new Vector3(mainCamTransform.position.x, myTransform.position.y, mainCamTransform.position.z);
					Vector3 faceDir = (myTransform.position - tempCamPos).normalized;
					myTransform.forward = Vector3.Lerp(myTransform.forward, faceDir, 0.5f);
				}
			}
			else
			{
				facingDirection = 0;
				localRotation = Quaternion.Lerp(localRotation, Quaternion.Euler(270, 90, 0), 0.2f);// Quaternion.identity;
				facingTransform.localRotation = localRotation;
				lerpRotation = facingTransform.rotation;
				if (moving)
				{
					Vector3 tempCamPos = new Vector3(mainCamTransform.position.x, myTransform.position.y, mainCamTransform.position.z);
					Vector3 faceDir = (myTransform.position - tempCamPos).normalized;
					myTransform.forward = Vector3.Lerp(myTransform.forward, faceDir, 0.2f);
					movingTransform.localRotation = Quaternion.Lerp(movingTransform.localRotation, Quaternion.Euler(0, Formulas.AngleAroundAxis(myTransform.forward, movementDirection, Vector3.up), 0), 0.2f);
				}
				else
				{
					movingTransform.localRotation = Quaternion.Lerp(movingTransform.localRotation, Quaternion.Euler(0, 0, 0), 0.2f);
				}
			}
		}
	}
	
	private IEnumerator Dash(Vector3 targetDir)
	{
		for (byte i = 0; i < dashCounter.Length; i++)
		{
			dashCounter[i] = 0;
		}
		dashing = true;
		charMotor.movement.maxForwardSpeed = charMotor.movement.maxBackwardsSpeed = charMotor.movement.maxSidewaysSpeed = 70;
		charMotor.movement.gravity = 0;
		charMotor.SetVelocity(targetDir.normalized * 70);
		StartCoroutine(charAnimator.PlayDash(dashTime));
		yield return new WaitForSeconds(dashTime);
		charMotor.movement.maxForwardSpeed = charMotor.movement.maxBackwardsSpeed = charMotor.movement.maxSidewaysSpeed = movementSpeed;
		charMotor.movement.gravity = 40;
		yield return new WaitForSeconds(0.1f);
		dashing = false;
	}
	
	private IEnumerator DashTimer(int index)
	{
		yield return new WaitForSeconds(dashMaxTimer);
		dashCounter[index] = 0;
	}
}
