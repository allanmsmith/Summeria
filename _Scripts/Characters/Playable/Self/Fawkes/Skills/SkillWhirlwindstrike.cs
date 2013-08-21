using UnityEngine;
using System.Collections;

public class SkillWhirlwindstrike : CCSkillTemplate
{
	private ObjectRecycler damageDealer;
	private DamageDealer[] damageDealerBehaviour;
	private AudioSource[] audioSources;
	
	private NewTimedTrailRenderer[] trails;
	private CCMover ccMover;
	
	protected override void Awake()
	{
		id = Lists.Spell.Whirlwindstrike;
		ccMover = GetComponent<CCMover>();
		
		base.Awake();
		
		trails = GetComponentsInChildren<NewTimedTrailRenderer>();
		for (int i = 0; i < trails.Length; i ++)
		{
			trails[i].emit = false;
		}
		
		damageDealer = new ObjectRecycler();
		damageDealer.CreatePool(ObjectName.skillHolder, "Fawkes-WhirlwindStrike", skillHandler.DamageCollider, 5);
		damageDealerBehaviour = new DamageDealer[damageDealer.all.Count];
		for (int i = 0; i < damageDealer.all.Count; i++)
		{
			damageDealerBehaviour[i] = damageDealer.all[i].GetComponent<DamageDealer>();
			damageDealerBehaviour[i].Damage = damage;// * (1 + PlayerStats.damageModifier) * (1 + (PlayerStats.playerLevel * 0.1f));
			damageDealerBehaviour[i].gameObject.transform.localScale = new Vector3(radius, radius, radius);
		}
	}
	
	public override IEnumerator SpellCast (Vector3 position)
	{
		skillHandler.HealthHandler.ApplySpellLock(ticks * timePerTick + 0.1f);
		StartCoroutine(RunCooldown(cooldown));
		myAnimation[animationName[0]].time = 0;
		myAnimation.Blend(animationName[0], 1f, 0.1f);
		ccMover.ShouldLook = false;
		for (int i = 0; i < trails.Length; i ++)
			trails[i].emit = true;
		for (byte i = 0; i < ticks; i++)
		{
			yield return new WaitForSeconds(timePerTick);
			damageDealer.Spawn(skillHandler.MyTransformPos, Quaternion.identity);
		}
		for (int i = 0; i < trails.Length; i ++)
			trails[i].emit = false;
		ccMover.ShouldLook = true;
		myAnimation.Blend(animationName[0], 0f, 0.1f);
	}
}