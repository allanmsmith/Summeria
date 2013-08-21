using UnityEngine;
using System.Collections;

public class DamageDealer : MonoBehaviour
{
	public DamageDealerCollider ddCollider;
	public DamageDealerColliderConstant ddConstantCollider;
	public Lists.Status favoredTargetStatus = Lists.Status.None;
	public Lists.Status applyStatus = Lists.Status.None;
	
	//Damage to cause
	protected float damage;
	public float Damage { set { damage = value; } }
	
	//Bonus damage to cause
	protected float damageBonus;
	public float DamageBonus { set { damageBonus = value; } }
	
	//Total damage to cause
	protected float totalDamage;
	
	//Position to push enemies back
	protected Vector3 position;
	public Vector3 Position { set { position = value; } }
	
	//Force to push enemies back
	protected float force;
	public float Force { set { force = value; } }
	
	//Duration of the negative status
	protected float statusDuration;
	public float StatusDuration { set { statusDuration = value; } }
	
	protected float ddDuration = 0.1f;
	public float DdDuration { set { ddDuration = value; } }
	
	//Has slow motion effect
	protected bool slowMotion;
	public bool SlowMotion { set { slowMotion = value; } }
	
	//How many enemies must be hit to shake the camera
	protected int slowMotionReq = 3;
	public int SlowMotionReq { set { slowMotionReq = value; } }
	private int targetsHit;
	
	//Should shake camera
	protected bool shakeCamera;
	public bool ShakeCamera { set { shakeCamera = value; } }
	
	//How much should the camera shake
	protected float shakeCameraIntensity;
	public float ShakeCameraIntensity { set { shakeCameraIntensity = value; } }
	
	//Cache of enemy health handler
	protected HealthHandler healthHandler;
	
	private bool countCombo;
	
	//Cache of my game object
	private GameObject myGameObject;
	
	private void Awake ()
	{
		if (ddCollider != null)
			ddCollider.DmgDealer = this;
		if (ddConstantCollider != null)
			ddConstantCollider.DmgDealer = this;
		myGameObject = gameObject;
		if (myGameObject.layer == 12)
			countCombo = true;
		myGameObject.SetActive(false);
	}
	
	private void OnEnable()
	{
		//Disable the object in x seconds
		StartCoroutine(AutoDestroyer());
		
		//Resets targets hit
		targetsHit = 0;
	}
	
	public void DealDamage(GameObject col)
	{
		if (col.layer == 9 || col.layer == 10) //9 = Player / 10 = Enemy
		{
			//Save that a target has been hit
			targetsHit ++;
			
			//Push the target back based on the received position
			//col.rigidbody.AddForce((col.transform.position - position).normalized * force, ForceMode.Impulse);
			
			//Grabs the targets HealthHandler and deal damage (if one exists)
			healthHandler = col.GetComponent<HealthHandler>();
			if (healthHandler != null)
			{
				totalDamage = damage + damageBonus;
				if (favoredTargetStatus != Lists.Status.None)
				{
					if (healthHandler.afflictedStatus.Contains(favoredTargetStatus))
						totalDamage *= GlobalVars.bonusDamageMultiplier;
				}
				healthHandler.TakeDamage(totalDamage);
				if (countCombo)
					ComboHandler.Instance.AddCombo();
				
				if (applyStatus != Lists.Status.None)
				{
					switch (applyStatus)
					{
						case Lists.Status.DOTed:
						healthHandler.ApplyDot(totalDamage * 0.2f);
						break;
						case Lists.Status.Feared:
						healthHandler.ApplyFear(statusDuration);
						break;
						case Lists.Status.Rooted:
						healthHandler.ApplyRoot(statusDuration);
						break;
						case Lists.Status.Silenced:
						healthHandler.ApplySilence(statusDuration);
						break;
						case Lists.Status.Slowed:
						healthHandler.ApplySlow(statusDuration);
						break;
						case Lists.Status.Stunned:
						break;
					}
				}
			}
			
			//Checks for slow motion effect
			/*
			if (slowMotion)
			{
				if (targetsHit >= slowMotionReq)
					EffectsHandler.Call.SlowTime();
			}
			
			//Checks for camera shaking
			if (shakeCamera)
				EffectsHandler.ShakeCamera(shakeCameraIntensity);*/
		}
	}
	
	private IEnumerator AutoDestroyer()
	{
		yield return new WaitForSeconds(ddDuration);
		myGameObject.SetActive(false);
	}
}
