using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthHandler : MonoBehaviour
{
	#region Generic Variables
	public Animation myAnimation;
	public Transform placeholderFeet;
	public Transform placeholderOverhead;
	public string[] movementAnimations;
	
	private float movementMultiplier = 1f;
	public float MovementMultiplier { get { return movementMultiplier; } }
	
	public List<Lists.Status> afflictedStatus;
	protected UIOverhead myOverhead;
	public UIOverhead MyOverhead { get { return myOverhead; } }
	//protected HUDText myHudText;
	private float diminishingReturnsMaxDuration = 10;
	private int diminishingReturnsMaxStacks = 3;
	private float diminishingReturnsCycle = 15;
	
	private bool dead;
	public bool Dead { get { return dead; } }
	#endregion
	
	#region Health Variables
	//Max Health
	protected float maxHealth;
	public float MaxHealth { get { return maxHealth; } }
	
	//Current Health
	protected float health;
	public float Health { get { return health; } }
	
	//Current Shield
	protected float shield;
	public float Shield { set { shield = value; } get { return shield; } }
	protected float currentShield;
	protected string absorbedText = "Absorbed";
	protected Color absorbedColor = Color.yellow;
	
	//Being healed check
	protected bool beingHealed;
	public bool BeingHealed { set { beingHealed = value; } get { return beingHealed; } }
	
	//Damage resistance
	protected float damageResistance;
	public float DamageResistance { set { damageResistance = value; } get { return damageResistance; } }
	
	//Current Health Percentage
	protected float healthPercentage;
	public float HealthPercentage { get { return healthPercentage; } }
	#endregion
	
	#region Slow Variables
	private GameObject slowPrefab;
	private ParticleSystem[] slowParticles;
	private GameObject[] slowParticlesGO;
	private float slowDuration;
	private int slowDiminishingReturns;
	private float slowIntensity = 1;
	public float SlowIntensity { get { return slowIntensity; } }
	private float slowDurationResistance = 1;
	public float SlowDurationResistance { set { slowDurationResistance = value; } }
	private float slowIntensityResistance = 1;
	public float SlowIntensityResistance { set { slowIntensityResistance = value; } }
	private string slowText = "Slowed!";
	private Color slowColor = Color.cyan;
	#endregion
	
	#region Silence Variables
	private GameObject silencePrefab;
	private ParticleSystem[] silenceParticles;
	private GameObject[] silenceParticlesGO;
	private float silenceDuration;
	private int silenceDiminishingReturns;
	private float silenceResistance = 1;
	public float SilenceResistance { set { silenceResistance = value; } }
	private string silenceText = "Silenced!";
	private Color silenceColor = Color.yellow;
	#endregion
	
	#region Root Variables
	private GameObject rootPrefab;
	private ParticleSystem[] rootParticles;
	private GameObject[] rootParticlesGO;
	private float rootDuration;
	private int rootDiminishingReturns;
	private float rootDurationResistance = 1;
	public float RootDurationResistance { set { rootDurationResistance = value; } }
	private string rootText = "Rooted!";
	private Color rootColor = Color.magenta;
	#endregion
	
	#region Dot Variables
	private GameObject dotPrefab;
	private ParticleSystem[] dotParticles;
	private GameObject[] dotParticlesGO;
	private float dotTickTime = 2;
	private int dotTickPerDot = 5;
	private float dotDamageResistance = 1;
	public float DotDamageResistance { set { dotDamageResistance = value; } }
	private List<float> dotDamages;
	private List<int> dotTicks;
	#endregion
	
	#region Fear Variables
	private GameObject fearPrefab;
	private ParticleSystem[] fearParticles;
	private GameObject[] fearParticlesGO;
	private float fearDuration;
	private int fearDiminishingReturns;
	private float fearDurationResistance = 1;
	public float FearDurationResistance { set { fearDurationResistance = value; } }
	private string fearText = "Feared!";
	private Color fearColor = Color.gray;
	#endregion
	
	#region Life Steal Variables
	private float lifeStealDuration;
	private float lifeStealIntensity;
	#endregion
	
	#region SpellLock Variables
	protected float spellLockDuration;
	#endregion
	
	#region MovementLock Variables
	private float movementLockDuration;
	#endregion
	
	#region ReflectProjectiles Variables
	private float reflectProjectilesDuration;
	#endregion
	
	protected virtual void Awake()
	{
		afflictedStatus = new List<Lists.Status>();
		
		//Verifica se nenhuma outra instance desse script ja nao inicializou a variavel
		if (GlobalVars.hudRootGO == null)
		{
			GlobalVars.hudRootGO = HUDRoot.go;
			if (GlobalVars.hudRootGO == null)
				GlobalVars.hudRootGO = GameObject.Find("HUDRoot");
		}
		if (GlobalVars.guiCamera == null)
			GlobalVars.guiCamera = GameObject.Find("GUICamera").GetComponent<Camera>();
		
		//Cria um objeto vazio filho do objeto especificado
		//myOverhead = UIOverheadHandler.Instance.AddOverhead(placeholderOverhead);
		/*
		GameObject tempObject = NGUITools.AddChild(GlobalVars.hudRootGO, GlobalVars.hudTextPrefabMob);
		myHudText = tempObject.GetComponentInChildren<HUDText>();
		UIFollowTarget tempUIFollow = tempObject.GetComponent<UIFollowTarget>();
		tempUIFollow.target = placeholderOverhead;
		tempUIFollow.gameCamera = Camera.mainCamera;
		tempUIFollow.uiCamera = GlobalVars.guiCamera;
		//myHudText.Add(" ", slowColor, 0f);*/
		
		#region Health Init
		health = maxHealth;
		#endregion
		
		#region Slow Init
		slowPrefab = (GameObject)Resources.Load("Debuffs/SlowPrefab");
		GameObject tempObject = (GameObject)Instantiate(slowPrefab);
		tempObject.transform.parent = placeholderFeet;
		tempObject.transform.localPosition = Vector3.zero;
		slowParticles = tempObject.GetComponentsInChildren<ParticleSystem>();
		slowParticlesGO = new GameObject[slowParticles.Length];
		for (byte i = 0; i < slowParticles.Length; i++)
		{
			slowParticles[i].enableEmission = false;
			slowParticlesGO[i] = slowParticles[i].gameObject;
			slowParticlesGO[i].SetActive(false);
		}
		#endregion
		
		#region Silence Init
		silencePrefab = (GameObject)Resources.Load("Debuffs/SilencePrefab");
		tempObject = (GameObject)Instantiate(silencePrefab);
		tempObject.transform.parent = placeholderOverhead;
		tempObject.transform.localPosition = Vector3.zero;
		silenceParticles = tempObject.GetComponentsInChildren<ParticleSystem>();
		silenceParticlesGO = new GameObject[silenceParticles.Length];
		for (byte i = 0; i < silenceParticles.Length; i++)
		{
			silenceParticles[i].enableEmission = false;
			silenceParticlesGO[i] = silenceParticles[i].gameObject;
			silenceParticlesGO[i].SetActive(false);
		}
		#endregion
		
		#region Root Init
		rootPrefab = (GameObject)Resources.Load("Debuffs/RootPrefab");
		tempObject = (GameObject)Instantiate(rootPrefab);
		tempObject.transform.parent = placeholderFeet;
		tempObject.transform.localPosition = Vector3.zero;
		rootParticles = tempObject.GetComponentsInChildren<ParticleSystem>();
		rootParticlesGO = new GameObject[rootParticles.Length];
		for (byte i = 0; i < rootParticles.Length; i++)
		{
			rootParticles[i].enableEmission = false;
			rootParticlesGO[i] = rootParticles[i].gameObject;
			rootParticlesGO[i].SetActive(false);
		}
		#endregion
		
		#region Dot Init
		dotPrefab = (GameObject)Resources.Load("Debuffs/DotPrefab");
		tempObject = (GameObject)Instantiate(dotPrefab);
		tempObject.transform.parent = placeholderFeet;
		tempObject.transform.localPosition = Vector3.zero;
		dotParticles = tempObject.GetComponentsInChildren<ParticleSystem>();
		dotParticlesGO = new GameObject[dotParticles.Length];
		for (byte i = 0; i < dotParticles.Length; i++)
		{
			dotParticles[i].enableEmission = false;
			dotParticlesGO[i] = dotParticles[i].gameObject;
			dotParticlesGO[i].SetActive(false);
		}
		dotDamages = new List<float>();
		dotTicks = new List<int>();
		#endregion
		
		#region Fear Init
		fearPrefab = (GameObject)Resources.Load("Debuffs/FearPrefab");
		tempObject = (GameObject)Instantiate(fearPrefab);
		tempObject.transform.parent = placeholderOverhead;
		tempObject.transform.localPosition = Vector3.zero;
		fearParticles = tempObject.GetComponentsInChildren<ParticleSystem>();
		fearParticlesGO = new GameObject[fearParticles.Length];
		for (byte i = 0; i < fearParticles.Length; i++)
		{
			fearParticles[i].enableEmission = false;
			fearParticlesGO[i] = fearParticles[i].gameObject;
			fearParticlesGO[i].SetActive(false);
		}
		#endregion
	}
	
	protected virtual void Start()
	{
		if (myAnimation == null)
			myAnimation = GetComponentInChildren<Animation>();
	}
	
	private void Update()
	{
		if (dead)
			return;
		
		for (byte i = 0; i < afflictedStatus.Count; i++)
		{
			#region Run Slow
			if (afflictedStatus[i] == Lists.Status.Slowed)
			{
				//Diminue o tempo do slow
				slowDuration -= Time.deltaTime;
				
				//Se o tempo acabou
				if (slowDuration <= 0)
				{
					//Remove o status de Slow
					afflictedStatus.RemoveAt(i);
					
					//Desliga as particulas
					for (byte j = 0; j < slowParticles.Length; j++)
					{
						slowParticles[j].enableEmission = false;
						StartCoroutine(DisableParticles(slowParticlesGO, slowParticles));
					}
					
					//Retorna a velocidade de movimento normal
					for (byte j = 0; j < movementAnimations.Length; j++)
						myAnimation[movementAnimations[j]].speed /= slowIntensity;
					
					//Volta a intensidade de slow ao valor padrao
					slowIntensity = 1;
					
					i --;
				}
				continue;
			}
			#endregion
			
			#region Run Silence
			if (afflictedStatus[i] == Lists.Status.Silenced)
			{
				//Diminue o tempo do Silence
				silenceDuration -= Time.deltaTime;
				
				//Se o tempo acabou
				if (silenceDuration <= 0)
				{
					//Remove o status de Silence
					afflictedStatus.Remove(Lists.Status.Silenced);
					
					//Desliga as particulas
					for (byte j = 0; j < silenceParticles.Length; j++)
					{
						silenceParticles[j].enableEmission = false;
						StartCoroutine(DisableParticles(silenceParticlesGO, silenceParticles));
					}
					
					i --;
				}
				continue;
			}
			#endregion
			
			#region Run Root
			if (afflictedStatus[i] == Lists.Status.Rooted)
			{
				//Diminue o tempo do Silence
				rootDuration -= Time.deltaTime;
				
				//Se o tempo acabou
				if (rootDuration <= 0)
				{
					//Remove o status de Silence
					afflictedStatus.RemoveAt(i);//.Remove(Status.Rooted);
					
					//Desliga as particulas
					for (byte j = 0; j < rootParticles.Length; j++)
					{
						rootParticles[j].enableEmission = false;
						StartCoroutine(DisableParticles(rootParticlesGO, rootParticles));
					}
					
					i --;
				}
				continue;
			}
			#endregion
			
			#region Run Fear
			if (afflictedStatus[i] == Lists.Status.Feared)
			{
				//Diminue o tempo do Silence
				fearDuration -= Time.deltaTime;
				
				//Se o tempo acabou
				if (fearDuration <= 0)
				{
					//Remove o status de Silence
					afflictedStatus.Remove(Lists.Status.Feared);
					
					//Desliga as particulas
					for (byte j = 0; j < fearParticles.Length; j++)
					{
						fearParticles[j].enableEmission = false;
						StartCoroutine(DisableParticles(fearParticlesGO, fearParticles));
					}
					
					i --;
				}
				continue;
			}
			#endregion
			
			#region Run Lifesteal
			if (afflictedStatus[i] == Lists.Status.Lifestealing)
			{
				//Diminue o tempo do Lifesteal
				lifeStealDuration -= Time.deltaTime;
				
				//Se o tempo acabou, Remove o status de Lifesteal
				if (lifeStealDuration <= 0)
				{
					afflictedStatus.Remove(Lists.Status.Lifestealing);
					i --;
				}
				continue;
			}
			#endregion
			
			#region Run SpellLock
			if (afflictedStatus[i] == Lists.Status.SpellLocked)
			{
				//Diminue o tempo do SpellLock
				spellLockDuration -= Time.deltaTime;
				
				//Se o tempo acabou, Remove o status de SpellLock
				if (spellLockDuration <= 0)
				{
					afflictedStatus.Remove(Lists.Status.SpellLocked);
					i --;
				}
				continue;
			}
			#endregion
			
			#region Run MovementLock
			if (afflictedStatus[i] == Lists.Status.MovementLocked)
			{
				//Diminue o tempo do SpellLock
				movementLockDuration -= Time.deltaTime;
				
				//Se o tempo acabou, Remove o status de SpellLock
				if (movementLockDuration <= 0)
				{
					afflictedStatus.Remove(Lists.Status.MovementLocked);
					i --;
				}
				continue;
			}
			#endregion
			
			#region Run ReflectProjectiles
			if (afflictedStatus[i] == Lists.Status.ReflectProjectiles)
			{
				//Diminue o tempo do Reflect Projectiles
				reflectProjectilesDuration -= Time.deltaTime;
				
				//Se o tempo acabou, Remove o status de ReflectProjectiles
				if (reflectProjectilesDuration <= 0)
				{
					afflictedStatus.Remove(Lists.Status.ReflectProjectiles);
					i --;
				}
				continue;
			}
			#endregion
		}
	}
	
	#region Disable Particles
	private IEnumerator DisableParticles(GameObject[] gos, ParticleSystem[] particles)
	{
		yield return new WaitForSeconds(5);
		for (byte i = 0; i < particles.Length; i++)
		{
			if (particles[i].particleCount <= 0)
				gos[i].SetActive(false);
		}
	}
	#endregion
	
	#region Health Functions
	public virtual void TakeDamage(float damage, bool stagger = true, float criticalChance = 0.1f, float criticalModifier = 2) {}
	
	public virtual void RecoverHealth(float heal, float criticalChance = 0.1f, float criticalModifier = 2) {}
	#endregion
	
	#region Slow Apply and DR
	public void ApplySlow(float duration, float targetSlowIntensity = 0.5f)
	{
		if (dead)
			return;
		
		if (slowDiminishingReturns < diminishingReturnsMaxStacks)
		{
			//Aumenta a contagem de diminishing returns do slow
			slowDiminishingReturns += 1;
			
			//Checa se o personagem ainda nao esta com slow
			if (!afflictedStatus.Contains(Lists.Status.Slowed))
			{
				//Cria um floating text avisando que esta lento
				//myHudText.Add(slowText, slowColor, 0f);
				//UIOverheadHandler.Instance.AddOverheadStatus(myOverhead.OverheadIndex, Lists.Status.Slowed);
				
				//Adiciona o status de "lento" ao personagem
				afflictedStatus.Add(Lists.Status.Slowed);
				
				//Ajusta a intensidade do slow baseado na resistencia de intensidade
				targetSlowIntensity *= slowIntensityResistance;
				
				//Ajusta a intensidade do slow
				slowIntensity = targetSlowIntensity;
				
				//Torna o multiplicador de velocidade = a potencia do slow
				movementMultiplier *= slowIntensity;
				
				//Ativa a emissao de particulas do slow
				for (int i = 0; i < slowParticles.Length; i++)
				{
					slowParticlesGO[i].SetActive(true);
					slowParticles[i].enableEmission = true;
				}
				
				//Deixa as animacoes de movimento mais lentas
				for (int i = 0; i < movementAnimations.Length; i++)
					myAnimation[movementAnimations[i]].speed *= slowIntensity;
			}
			
			//Corrige a duracao baseado no diminishing returns
			if (duration > diminishingReturnsMaxDuration / slowDiminishingReturns)
				duration = diminishingReturnsMaxDuration / slowDiminishingReturns;
			
			//Corrige a duracao baseado na resistencia de slow duration
			duration *= slowDurationResistance;
			
			//Soma na duracao total de slows a duracao corrigida
			if (duration > slowDuration)
				slowDuration = duration;
			
			//Restart the coroutine
			StopCoroutine("RunSlowDR");
			StartCoroutine("RunSlowDR");
		}
	}
	
	private IEnumerator RunSlowDR()
	{
		yield return new WaitForSeconds(diminishingReturnsCycle);
		slowDiminishingReturns = 0;
	}
	#endregion
	
	#region Silence Apply and DR
	public void ApplySilence(float duration)
	{
		if (dead)
			return;
		
		if (silenceDiminishingReturns < diminishingReturnsMaxStacks)
		{
			//Aumenta a contagem de diminishing returns do silence
			silenceDiminishingReturns += 1;
			
			//Checa se o personagem ainda nao esta com silence
			if (!afflictedStatus.Contains(Lists.Status.Silenced))
			{
				//Cria um floating text avisando que esta com silence
				//myHudText.Add(silenceText, silenceColor, 0f);
				//UIOverheadHandler.Instance.AddOverheadStatus(myOverhead.OverheadIndex, Lists.Status.Silenced);
				
				//Adiciona o status de "lento" ao personagem
				afflictedStatus.Add(Lists.Status.Silenced);
				
				//Ativa a emissao de particulas do slow
				for (int i = 0; i < silenceParticles.Length; i++)
				{
					silenceParticlesGO[i].SetActive(true);
					silenceParticles[i].enableEmission = true;
				}
			}
			
			//Corrige a duracao baseado no diminishing returns
			if (duration > diminishingReturnsMaxDuration / silenceDiminishingReturns)
				duration = diminishingReturnsMaxDuration / silenceDiminishingReturns;
			
			//Corrige a duracao baseado na resistencia de silence
			duration *= silenceResistance;
			
			//Soma na duracao total de silence a duracao corrigida
			if (duration > silenceDuration)
				silenceDuration = duration;
			
			StopCoroutine("RunSilenceDR");
			StartCoroutine("RunSilenceDR");
		}
	}
	
	private IEnumerator RunSilenceDR()
	{
		yield return new WaitForSeconds(diminishingReturnsCycle);
		silenceDiminishingReturns = 0;
	}
	#endregion
	
	#region Root Apply and DR
	public void ApplyRoot(float duration)
	{
		if (dead)
			return;
		
		if (rootDiminishingReturns < diminishingReturnsMaxStacks)
		{
			//Aumenta a contagem de diminishing returns do slow
			rootDiminishingReturns += 1;
			
			//Checa se o personagem ainda nao esta com slow
			if (!afflictedStatus.Contains(Lists.Status.Rooted))
			{
				//Cria um floating text avisando que esta lento
				//myHudText.Add(rootText, rootColor, 0f);
				//UIOverheadHandler.Instance.AddOverheadStatus(myOverhead.OverheadIndex, Lists.Status.Rooted);
				
				//Adiciona o status de "lento" ao personagem
				afflictedStatus.Add(Lists.Status.Rooted);
				
				//Ativa a emissao de particulas do slow
				for (int i = 0; i < rootParticles.Length; i++)
				{
					rootParticlesGO[i].SetActive(true);
					rootParticles[i].enableEmission = true;
				}
			}
			
			//Corrige a duracao baseado no diminishing returns
			if (duration > diminishingReturnsMaxDuration / rootDiminishingReturns)
				duration = diminishingReturnsMaxDuration / rootDiminishingReturns;
			
			//Corrige a duracao baseado na resistencia de slow duration
			duration *= rootDurationResistance;
			
			//Soma na duracao total de slows a duracao corrigida
			if (duration > rootDuration)
				rootDuration = duration;
			//print (duration + "      " + rootDuration);
			
			ApplyMovementLock(rootDuration);
			
			//Restart the coroutine
			StopCoroutine("RunRootDR");
			StartCoroutine("RunRootDR");
		}
	}
	
	private IEnumerator RunRootDR()
	{
		yield return new WaitForSeconds(diminishingReturnsCycle);
		rootDiminishingReturns = 0;
	}
	#endregion
	
	#region Dot Apply and DR
	public void ApplyDot(float damage)
	{
		if (dead)
			return;
		
		//Adiciona um dot a lista
		dotDamages.Add(damage * dotDamageResistance);
		dotTicks.Add(dotTickPerDot);
		
		if (!afflictedStatus.Contains(Lists.Status.DOTed))
		{
			//Adiciona um Dot para a lista
			afflictedStatus.Add(Lists.Status.DOTed);
			
			//Ativa a emissao de particulas do DOT
			for (int i = 0; i < dotParticles.Length; i++)
			{
				dotParticlesGO[i].SetActive(true);
				dotParticles[i].enableEmission = true;
			}
			
			StartCoroutine("RunDots");
		}
	}
	
	private IEnumerator RunDots()
	{
		while (dotDamages.Count > 0)
		{
			if (!dead)
			{
				for (byte i = 0; i < dotDamages.Count; i++)
				{
					TakeDamage(dotDamages[i], false, 0, 0);
					dotTicks[i] --;
					if (dotTicks[i] <= 0)
					{
						dotDamages.RemoveAt(i);
						dotTicks.RemoveAt(i);
						i --;
					}
				}
				yield return new WaitForSeconds(dotTickTime);
			}
			else
			{
				for (int i = 0; i < dotParticles.Length; i++)
					dotParticles[i].enableEmission = false;
				break;
			}
		}
		if (afflictedStatus.Contains(Lists.Status.DOTed))
		{
			//Remove o status de dot
			afflictedStatus.Remove(Lists.Status.DOTed);
			
			//Para a emissao de particulas de dot
			for (int i = 0; i < dotParticles.Length; i++)
				dotParticles[i].enableEmission = false;
		}
	}
	#endregion
	
	#region Fear Apply and DR
	public void ApplyFear(float duration)
	{
		if (dead)
			return;
		
		if (fearDiminishingReturns < diminishingReturnsMaxStacks)
		{	
			//Aumenta a contagem de diminishing returns do slow
			fearDiminishingReturns += 1;
			
			//Checa se o personagem ainda nao esta com slow
			if (!afflictedStatus.Contains(Lists.Status.Feared))
			{
				//Cria um floating text avisando que esta lento
				//myHudText.Add(fearText, fearColor, 0f);
				//UIOverheadHandler.Instance.AddOverheadStatus(myOverhead.OverheadIndex, Lists.Status.Feared);
				
				//Adiciona o status de "lento" ao personagem
				afflictedStatus.Add(Lists.Status.Feared);
				
				//Ativa a emissao de particulas do slow
				for (int i = 0; i < fearParticles.Length; i++)
				{
					fearParticlesGO[i].SetActive(true);
					fearParticles[i].enableEmission = true;
				}
			}
			
			//Corrige a duracao baseado no diminishing returns
			if (duration > diminishingReturnsMaxDuration / fearDiminishingReturns)
				duration = diminishingReturnsMaxDuration / fearDiminishingReturns;
			
			//Corrige a duracao baseado na resistencia de slow duration
			duration *= fearDurationResistance;
			
			//Soma na duracao total de slows a duracao corrigida
			if (duration > fearDuration)
				fearDuration = duration;
			
			//Restart the coroutine
			StopCoroutine("RunFearDR");
			StartCoroutine("RunFearDR");
		}
	}
	
	private IEnumerator RunFearDR()
	{
		yield return new WaitForSeconds(diminishingReturnsCycle);
		fearDiminishingReturns = 0;
	}
	#endregion
	
	#region Lifesteal Apply
	public virtual void ApplyLifesteal(float duration)
	{
		if (dead)
			return;
		
		//Checa se o personagem ainda nao esta com silence
		if (!afflictedStatus.Contains(Lists.Status.Lifestealing))
		{
			//Adiciona o status de "lento" ao personagem
			afflictedStatus.Add(Lists.Status.Lifestealing);
		}
		
		//Soma na duracao total de silence a duracao corrigida
		if (duration > lifeStealDuration)
			lifeStealDuration = duration;
	}
	#endregion
	
	#region SpellLock Apply
	public virtual void ApplySpellLock(float duration)
	{
		if (dead)
			return;
		
		//Checa se o personagem ainda nao esta com silence
		if (!afflictedStatus.Contains(Lists.Status.SpellLocked))
		{
			//Adiciona o status de "lento" ao personagem
			afflictedStatus.Add(Lists.Status.SpellLocked);
		}
		
		//Soma na duracao total de silence a duracao corrigida
		if (duration > spellLockDuration)
			spellLockDuration = duration;
	}
	#endregion
	
	#region MovementLock Apply
	public void ApplyMovementLock(float duration)
	{
		if (dead)
			return;
		
		//Checa se o personagem ainda nao esta com silence
		if (!afflictedStatus.Contains(Lists.Status.MovementLocked))
		{
			//Adiciona o status de "lento" ao personagem
			afflictedStatus.Add(Lists.Status.MovementLocked);
		}
		
		//Soma na duracao total de silence a duracao corrigida
		if (duration > movementLockDuration)
			movementLockDuration = duration;
	}
	#endregion
	
	#region ReflectProjectiles Apply
	public void ApplyReflectProjectiles(float duration)
	{
		if (dead)
			return;
		
		//Checa se o personagem ainda nao esta refletindo projeteis
		if (!afflictedStatus.Contains(Lists.Status.ReflectProjectiles))
		{
			//Adiciona o status de "ReflectProjectiles" ao personagem
			afflictedStatus.Add(Lists.Status.ReflectProjectiles);
		}
		
		//Soma na duracao total de reflect projectiles a duracao corrigida
		reflectProjectilesDuration += duration;
	}
	#endregion
	
	#region Death
	protected virtual void Death()
	{
		dead = true;
		afflictedStatus.Clear();
		
		slowDuration = 0;
		for (byte j = 0; j < slowParticles.Length; j++)
			slowParticles[j].enableEmission = false;
		for (byte j = 0; j < movementAnimations.Length; j++)
			myAnimation[movementAnimations[j]].speed /= slowIntensity;
		slowIntensity = 1;
		
		silenceDuration = 0;
		for (byte j = 0; j < silenceParticles.Length; j++)
			silenceParticles[j].enableEmission = false;
		
		rootDuration = 0;
		for (byte j = 0; j < rootParticles.Length; j++)
			rootParticles[j].enableEmission = false;
		
		fearDuration = 0;
		for (byte j = 0; j < fearParticles.Length; j++)
			fearParticles[j].enableEmission = false;
		
		spellLockDuration = 0;
		
		movementLockDuration = 0;
		
		reflectProjectilesDuration = 0;
		
		gameObject.SendMessage("DeathEvent", SendMessageOptions.DontRequireReceiver);
	}
	#endregion
}
