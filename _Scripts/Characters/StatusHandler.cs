using UnityEngine;
using System.Collections;

public class StatusHandler : MonoBehaviour
{
	[HideInInspector]
	public bool casting;
	[HideInInspector]
	public float castingMaxDuration;
	private float castingDuration;
	
	[HideInInspector]
	public bool slowed;
	[HideInInspector]
	public float slowIntensity = 1f;
	private GameObject slowPrefab;
	private ParticleSystem slowParticle;
	private float slowParticleER;
	private float slowDuration;
	private float slowResistance;
	private int slowDiminishingReturns;
	
	[HideInInspector]
	public bool impaired;
	private float impairedDuration;
	private float impairedResistance;
	
	[HideInInspector]
	public bool silenced;
	private GameObject silencePrefab;
	private ParticleEmitter silenceParticleEmitter;
	private float silenceDuration;
	private float silenceResistance;
	private int silenceDiminishingReturns;
	
	[HideInInspector]
	public bool rooted;
	private float rootDuration;
	private float rootResistance;
	
	[HideInInspector]
	public bool stunned;
	private float stunDuration;
	private float stunResistance;
	
	[HideInInspector]
	public bool feared;
	[HideInInspector]
	public Vector3 targetPosition;
	private float fearDuration;
	private float fearResistance;
	private int fearDiminishingReturns;
	
	[HideInInspector]
	public bool doted;
	private float dotDamage;
	private float dotInterval;
	
	private float diminishingReturnsMaxDuration = 10;
	private int diminishingReturnsMaxStacks = 3;
	private float diminishingReturnsCycle = 15;
	
	public Transform placeholderFeet;
	public Transform placeholderOverhead;
	public string[] movementAnimations;
	//private HealthHandler healthHandler;
	private Animation myAnimation;
	private Transform myTransform;
	
	private void Awake()
	{
		myTransform = transform;
		myAnimation = GetComponentInChildren<Animation>();
		//healthHandler = GetComponent<HealthHandler>();
		
		//Slow
		slowPrefab = (GameObject)Resources.Load("Debuffs/SlowPrefab");
		GameObject tempObject = (GameObject)Instantiate(slowPrefab);
		tempObject.transform.parent = placeholderFeet;
		tempObject.transform.localPosition = Vector3.zero;
		slowParticle = tempObject.GetComponentInChildren<ParticleSystem>();
		slowParticleER = slowParticle.emissionRate;
		slowParticle.emissionRate = 0;
		//-----
		
		//Silence
		silencePrefab = (GameObject)Resources.Load("Debuffs/SilencePrefab");
		tempObject = (GameObject)Instantiate(silencePrefab);
		tempObject.transform.parent = placeholderOverhead;
		tempObject.transform.localPosition = Vector3.zero;
		silenceParticleEmitter = tempObject.GetComponentInChildren<ParticleEmitter>();
		silenceParticleEmitter.emit = false;
		//-------
	}
	
	private void Start()
	{
		slowResistance = 1f - slowResistance;
		impairedResistance = 1f - impairedResistance;
		silenceResistance = 1f - silenceResistance;
		rootResistance = 1f - rootResistance;
		stunResistance = 1f - stunResistance;
		fearResistance = 1f - fearResistance;
	}
	
	#region Casting Apply and Run
	public void ApplyCasting()
	{
		if (!casting)
		{
			casting = true;
			slowIntensity = 0.5f;
			for (int i = 0; i < movementAnimations.Length; i++)
			{
				myAnimation[movementAnimations[i]].speed *= slowIntensity;
			}
		}
		castingDuration = castingMaxDuration;
	}
	
	private void RunCasting()
	{
		castingDuration -= Time.deltaTime;
		if (castingDuration <= 0)
		{
			casting = false;
			if (!slowed)
			{
				for (int i = 0; i < movementAnimations.Length; i++)
				{
					myAnimation[movementAnimations[i]].speed /= slowIntensity;
				}
				slowIntensity = 1f;
			}
		}
	}
	#endregion
	
	#region Slow Apply, Run and DR
	public void ApplySlow(float duration)
	{
		if (slowDiminishingReturns < diminishingReturnsMaxStacks)
		{
			slowDiminishingReturns += 1;
			if (!slowed)
			{
				slowed = true;
				slowIntensity = 0.5f;
				slowParticle.emissionRate = slowParticleER;
				for (int i = 0; i < movementAnimations.Length; i++)
				{
					myAnimation[movementAnimations[i]].speed *= slowIntensity;
				}
			}
			if (duration > diminishingReturnsMaxDuration / slowDiminishingReturns)
				duration = diminishingReturnsMaxDuration / slowDiminishingReturns;
			duration *= slowResistance;
			slowDuration += duration;
			StopCoroutine("RunSlowDR");
			StartCoroutine("RunSlowDR");
		}
	}
	
	private void RunSlow()
	{
		slowDuration -= Time.deltaTime;
		if (slowDuration <= 0)
		{
			slowed = false;
			slowParticle.emissionRate = 0;
			if (!casting)
			{
				for (int i = 0; i < movementAnimations.Length; i++)
				{
					myAnimation[movementAnimations[i]].speed /= slowIntensity;
				}
				slowIntensity = 1f;
			}
		}
	}
	
	private IEnumerator RunSlowDR()
	{
		yield return new WaitForSeconds(diminishingReturnsCycle);
		slowDiminishingReturns = 0;
	}
	#endregion
	
	public void ApplyImpair(float duration)
	{
		if (!impaired)
			impaired = true;
		duration *= impairedResistance;
		impairedDuration += duration;
	}
	
	private void RunImpair()
	{
		impairedDuration -= Time.deltaTime;
		if (impairedDuration <= 0)
			impaired = false;
	}
	
	#region Silence Apply, Run and DR
	public void ApplySilence(float duration)
	{
		if (silenceDiminishingReturns < diminishingReturnsMaxStacks)
		{
			silenceDiminishingReturns += 1;
			if (!silenced)
			{
				silenced = true;
				silenceParticleEmitter.emit = true;
			}
			if (duration > diminishingReturnsMaxDuration / silenceDiminishingReturns)
				duration = diminishingReturnsMaxDuration / silenceDiminishingReturns;
			duration *= silenceResistance;
			silenceDuration += duration;
			StopCoroutine("RunSilenceDR");
			StartCoroutine("RunSilenceDR");
		}
	}
	
	private void RunSilence()
	{
		silenceDuration -= Time.deltaTime;
		if (silenceDuration <= 0)
		{
			silenceParticleEmitter.emit = false;
			silenced = false;
		}
	}
	
	private IEnumerator RunSilenceDR()
	{
		yield return new WaitForSeconds(diminishingReturnsCycle);
		silenceDiminishingReturns = 0;
	}
	#endregion
	
	#region Fear Apply, Run, ChangeDirection and DR
	public void ApplyFear(float duration)
	{
		print ("RunFearDR");
		if (fearDiminishingReturns < diminishingReturnsMaxStacks)
		{
			fearDiminishingReturns += 1;
			if (!feared)
			{
				feared = true;
				StartCoroutine(ChangeFearDirection());
				//slowParticleEmitter.emit = true;
			}
			if (duration > diminishingReturnsMaxDuration / fearDiminishingReturns)
				duration = diminishingReturnsMaxDuration / fearDiminishingReturns;
			duration *= fearResistance;
			fearDuration += duration;
			StopCoroutine("RunFearDR");
			StartCoroutine("RunFearDR");
		}
	}
	
	private void RunFear()
	{
		fearDuration -= Time.deltaTime;
		if (fearDuration <= 0)
		{
			feared = false;
			//slowParticleEmitter.emit = false;
		}
	}
	
	private IEnumerator ChangeFearDirection()
	{
		while (feared)
		{
			yield return new WaitForSeconds(1f);
			targetPosition = myTransform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
		}
	}
	
	private IEnumerator RunFearDR()
	{
		yield return new WaitForSeconds(diminishingReturnsCycle);
		fearDiminishingReturns = 0;
	}
	#endregion
	
	public void Update()
	{
		//if (healthHandler.Health <= 0)
		{
			StopEmitters();
			return;
		}
		if (casting)
			RunCasting();
		if (slowed)
			RunSlow();
		if (impaired)
			RunImpair();
		if (silenced)
			RunSilence();
		if (feared)
			RunFear();
	}
	
	private void StopEmitters()
	{
		if (slowParticle.emissionRate > 0)
			slowParticle.emissionRate = 0;
		if (silenceParticleEmitter.emit)
			silenceParticleEmitter.emit = false;
	}
}
