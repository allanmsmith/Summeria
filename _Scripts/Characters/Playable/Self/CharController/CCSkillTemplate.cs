using UnityEngine;
using System.Collections;

public class CCSkillTemplate : MonoBehaviour
{
	//Index dessa skill dentre as equipadas
	private int skillIndex;
	
	//Cache SkillHandler
	protected CCSkillHandler skillHandler;
	
	//Cache Animation
	protected Animation myAnimation;
	public Animation MyAnimation { set { myAnimation = value; } }
	
	protected Lists.Spell id;
	
	protected float damage;
	protected float radius;
	protected float force;
	protected float range;
	protected float speed;
	protected float ticks;
	protected float timePerTick;
	protected float cooldown;
	protected string[] animationName;
	protected GameObject[] spellPrefab;
	protected AudioClip[] audioClip;
	
	[HideInInspector]
	public bool onCooldown;
	
	protected virtual void Awake()
	{
		skillHandler = GetComponent<CCSkillHandler>();
		
		SkillTemplate tempSpell = null;
		int number = -1;
		for (byte i = 0; i < PlayerStats.skillList.Length; i++)
		{
			if (PlayerStats.skillList[i].id == id)
			{
				number = i;
				break;
			}	
		}
		tempSpell = PlayerStats.skillList[number];
		damage = tempSpell.damage * (1 + (PlayerStats.damageModifier * 0.01f));
		radius = tempSpell.radius;
		force = tempSpell.force;
		range = tempSpell.range;
		speed = tempSpell.speed;
		ticks = tempSpell.ticks;
		timePerTick = tempSpell.timePerTick;
		animationName = tempSpell.animationName;
		spellPrefab = tempSpell.spellPrefab;
		cooldown = tempSpell.cooldown;
		audioClip = tempSpell.audio;
		
		skillIndex = -1;
		for (int i = 0; i < PlayerStats.equippedSkills.Length; i ++)
		{
			if (id == PlayerStats.equippedSkills[i])
			{
				skillIndex = i;
				//skillHandler.intController.skillInfo[i] = new SkillHUDInfo();
				//skillHandler.intController.skillInfo[i].cooldown = cooldown;
				//skillHandler.intController.skillInfo[i].iconTexture = texture;
			}
		}
	}
	
	public virtual IEnumerator Spell()
	{
		yield return new WaitForEndOfFrame();
		if (!skillHandler.HealthHandler.afflictedStatus.Contains(Lists.Status.SpellLocked) && !onCooldown)
			StartCoroutine(SpellCast(skillHandler.PlaceholderFrontDirectionPos));
		else
			skillHandler.PlayOnCooldown();
	}
	
	public virtual IEnumerator SpellCast(Vector3 position) { yield return new WaitForEndOfFrame(); }
	
	protected IEnumerator RunCooldown(float waitTime)
	{
		onCooldown = true;
		//if (applyCasting)
		//	skillHandler.StatusHandlerComp.ApplyCasting();
		//skillHandler.intController.skillInfo[skillIndex].cooldownTimer = cooldown;
		yield return new WaitForSeconds(waitTime);
		onCooldown = false;
	}
}
