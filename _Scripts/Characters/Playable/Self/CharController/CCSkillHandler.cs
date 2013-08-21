using UnityEngine;
using System.Collections;

public class CCSkillHandler : MonoBehaviour
{
	//Meu transform
	private Transform myTransform;
	public Transform MyTransform { set { myTransform = value; } get { return myTransform; } }
	public Vector3 MyTransformPos { set { myTransform.position = value; } get { return myTransform.position; } }
	
	//Meu animation
	private Animation myAnimation;
	public Animation MyAnimation { set { myAnimation = value; } get { return myAnimation; } }
	
	//Bone rotation
	private Transform myFacing;
	public Transform MyFacing { set { myFacing = value; } get { return myFacing; } }
	
	//Status Handler
	private HealthHandler healthHandler;
	public HealthHandler HealthHandler { set { healthHandler = value; } get { return healthHandler; } }
	
	//Char Mover
	private CCMover charMover;
	public CCMover CharMover { set { charMover = value; } get { return charMover; } }
	
	//Prefab do collider para dano
	private GameObject damageCollider;
	public GameObject DamageCollider { set { damageCollider = value; } get { return damageCollider; } }
	
	//Prefab do collider para dano
	private GameObject damageColliderConstant;
	public GameObject DamageColliderConstant { set { damageColliderConstant = value; } get { return damageColliderConstant; } }
	
	//Transform da area onde dar dano a frente do personagem
	private Transform placeholderDamageFront;
	public Transform PlaceholderDamageFront { set { placeholderDamageFront = value; } }
	public Vector3 PlaceholderDamageFrontPos { get { return placeholderDamageFront.position; } }
	
	//Transform da area onde dar dano a frente do personagem, no chao
	private Transform placeholderDamageFrontGround;
	public Transform PlaceholderDamageFrontGround { set { placeholderDamageFrontGround = value; } }
	public Vector3 PlaceholderDamageFrontGroundPos { get { return placeholderDamageFrontGround.position; } }
	
	//Transform da mao magica do personagem
	private Transform placeholderSpellHand;
	public Transform PlaceholderSpellHand { set { placeholderSpellHand = value; } get { return placeholderSpellHand; } }
	public Vector3 PlaceholderSpellHandPos { get { return placeholderSpellHand.position; } }
	
	//Transform da direcao pra frente
	private Transform placeholderFrontDirection;
	public Transform PlaceholderFrontDirection { set { placeholderFrontDirection = value; } get { return placeholderFrontDirection; } }
	public Vector3 PlaceholderFrontDirectionPos { get { return placeholderFrontDirection.position; } }
	
	//Clip de audio de cooldown
	private AudioClip clipOnCooldown;
	public AudioClip ClipOnCooldown { set { clipOnCooldown = value; } }
	
	//Define se o som de cooldown esta ou nao em cooldown
	private bool audioOnCooldown;
	
	//Detecta se o player esta com as skills travadas
	private bool skillLocked;
	public bool SkillLocked { set { skillLocked = value; } get { return skillLocked; } }
	
	//Layermask dos inimigos
	private LayerMask layerEnemies;
	public LayerMask LayerEnemies { get { return layerEnemies; } }
	
	//Target Enemy
	private Transform targetEnemy;
	public Transform TargetEnemy { set { targetEnemy = value; } get { return targetEnemy; } }
	
	//Target HealthHandler
	private HealthHandler targetHealth;
	public HealthHandler TargetHealth { set { targetHealth = value; } get { return targetHealth; } }
	
	//Possible to Change Target
	private float changeTargetInterval = 0.2f;
	private float timeCasting = 0.6f;
	
	//[HideInInspector]
	//public InterfaceController intController;
	
	[HideInInspector]
	public CCSkillTemplate[] skillTemplates;
	
	private void Start ()
	{
		layerEnemies = 1 << LayerMask.NameToLayer("Enemy");
		
		skillTemplates = new CCSkillTemplate[PlayerStats.equippedSkills.Length];
		for (int i = 0; i < PlayerStats.equippedSkills.Length; i++)
		{
			if (PlayerStats.equippedSkills[i] != Lists.Spell.None)
			{
				skillTemplates[i] = (CCSkillTemplate)gameObject.AddComponent("Skill" + PlayerStats.equippedSkills[i].ToString());
				skillTemplates[i].MyAnimation = myAnimation;
			}
		}
		
		//intController.SkillHandlerSet = this;
		//intController.AssignCooldowns();
		
		/*
		if (PlayerStats.skillBasicName != "none" && PlayerStats.skillBasicName != null)
		{
			skillTemplates[0] = (CharacterSkillTemplate)gameObject.AddComponent("Skill" + PlayerStats.skillBasicName);
			skillTemplates[0].MyAnimation = myAnimation;
			skillTemplates[0].SkillNumber = 0;
		}
		if (PlayerStats.skillSupportName != "none" && PlayerStats.skillSupportName != null)
		{
			skillTemplates[1] = (CharacterSkillTemplate)gameObject.AddComponent("Skill" + PlayerStats.skillSupportName);
			skillTemplates[1].MyAnimation = myAnimation;
			skillTemplates[1].SkillNumber = 1;
		}
		if (PlayerStats.skillAdvancedName != "none" && PlayerStats.skillAdvancedName != null)
		{
			skillTemplates[2] = (CharacterSkillTemplate)gameObject.AddComponent("Skill" + PlayerStats.skillAdvancedName);
			skillTemplates[2].MyAnimation = myAnimation;
			skillTemplates[2].SkillNumber = 2;
		}
		if (PlayerStats.skillUltimateName != "none" && PlayerStats.skillUltimateName != null)
		{
			skillTemplates[3] = (CharacterSkillTemplate)gameObject.AddComponent("Skill" + PlayerStats.skillUltimateName);
			skillTemplates[3].MyAnimation = myAnimation;
			skillTemplates[3].SkillNumber = 3;
		}*/
	}
	
	private void OnEnable()
	{
		CCInputController.Instance.onSpellUse += UseSpell;
	}
	
	private void OnDisable()
	{
		if (CCInputController.Instance != null)
			CCInputController.Instance.onSpellUse -= UseSpell;
	}
	
	private void UseSpell(int spellIndex)
	{
		if (spellIndex < skillTemplates.Length)
			StartCoroutine(skillTemplates[spellIndex].Spell());
	}
	
	
	
	
	public void PlayOnCooldown()
	{
		if (!audioOnCooldown)
		{
			//SoundManager.PlaySound(clipOnCooldown, myTransform);
			audioOnCooldown = true;
			StartCoroutine(FinishSoundCooldown());
		}
	}
	
	public IEnumerator FinishSoundCooldown()
	{
		yield return new WaitForSeconds(0.4f);
		audioOnCooldown = false;
	}
	
	public void AssignTarget(Transform targetT)
	{
		if (targetEnemy == null)
		{
			targetEnemy = targetT;
			targetHealth = targetEnemy.GetComponent<HealthHandler>();
			StartCoroutine(LoseTarget(changeTargetInterval));
		}
	}
	
	public IEnumerator LoseTarget(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		targetEnemy = null;
		targetHealth = null;
	}
	
	public IEnumerator FinishCasting()
	{
		yield return new WaitForSeconds(timeCasting);
		charMover.FacingPoint = Vector3.zero;
		charMover.FacingDirection = 0;
	}
}
