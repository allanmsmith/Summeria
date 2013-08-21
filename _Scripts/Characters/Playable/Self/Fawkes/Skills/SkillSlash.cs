using UnityEngine;
using System.Collections;

public class SkillSlash : CCSkillTemplate
{
	//private TimedTrailRenderer timedRenderer;
	
	private ObjectRecycler damageDealer;
	private DamageDealer[] damageDealerBehaviour;
	private AudioSource[] audioSources;
	
	private int index;
	
	private NewTimedTrailRenderer[] trails;
	
	protected override void Awake()
	{
		id = Lists.Spell.Slash;
		
		base.Awake();
		
		trails = GetComponentsInChildren<NewTimedTrailRenderer>();
		for (int i = 0; i < trails.Length; i ++)
		{
			trails[i].emit = false;
		}
		
		damageDealer = new ObjectRecycler();
		damageDealer.CreatePool(ObjectName.skillHolder, "Fawkes-Slash", skillHandler.DamageCollider, 3);
		damageDealerBehaviour = new DamageDealer[damageDealer.all.Count];
		for (int i = 0; i < damageDealer.all.Count; i++)
		{
			damageDealerBehaviour[i] = damageDealer.all[i].GetComponent<DamageDealer>();
			damageDealerBehaviour[i].Damage = damage * (1 + (i * 0.2f));// * (1 + PlayerStats.damageModifier) * (1 + (PlayerStats.playerLevel * 0.1f));
			damageDealerBehaviour[i].gameObject.transform.localScale = new Vector3(radius, radius, radius);
		}
		
		//audioSources = SoundManager.AttachAudioSources(audioClip, skillHandler.MyTransform, 3);
	}
	
	public override IEnumerator SpellCast (Vector3 position)
	{
		//SoundManager.PlayAudio(audioSources);
		switch (index)
		{
			case 0:
			index = 1;
			cooldown = myAnimation[animationName[0]].length * (1 / myAnimation[animationName[0]].speed);
			skillHandler.HealthHandler.ApplySpellLock(cooldown);
			StartCoroutine(RunCooldown(cooldown));
			myAnimation[animationName[0]].time = 0;
			myAnimation.Blend(animationName[0], 1f, 0.1f);
			StartCoroutine(DealDamage(0.2f));
			StartCoroutine(FinishCombo(myAnimation[animationName[0]].length * 1.8f, 1));
			yield return new WaitForSeconds(myAnimation[animationName[0]].length * 0.5f);
			for (int i = 0; i < trails.Length; i ++)
				trails[i].emit = true;
			yield return new WaitForSeconds(myAnimation[animationName[0]].length * 0.5f);
			for (int i = 0; i < trails.Length; i ++)
				trails[i].emit = false;
			yield return new WaitForSeconds(myAnimation[animationName[0]].length * 0.8f);
			myAnimation.Blend(animationName[0], 0f, 0.3f);
			break;
			
			case 1:
			index = 2;
			cooldown = myAnimation[animationName[1]].length * (1 / myAnimation[animationName[1]].speed);
			skillHandler.HealthHandler.ApplySpellLock(cooldown);
			StartCoroutine(RunCooldown(cooldown));
			myAnimation[animationName[1]].time = 0;
			myAnimation.Blend(animationName[1], 1f, 0.1f);
			StartCoroutine(DealDamage(0.2f));
			StartCoroutine(FinishCombo(myAnimation[animationName[1]].length * 5f, 2));
			yield return new WaitForSeconds(myAnimation[animationName[1]].length * 0.1f);
			myAnimation.Blend(animationName[0], 0f, 0);
			for (int i = 0; i < trails.Length; i ++)
				trails[i].emit = true;
			yield return new WaitForSeconds(myAnimation[animationName[1]].length * 0.9f);
			for (int i = 0; i < trails.Length; i ++)
				trails[i].emit = false;
			yield return new WaitForSeconds(myAnimation[animationName[1]].length * 2f);
			myAnimation.Blend(animationName[1], 0f, 0.3f);
			break;
			
			case 2:
			index = 0;
			cooldown = myAnimation[animationName[2]].length * 1.2f * (1 / myAnimation[animationName[2]].speed);
			skillHandler.HealthHandler.ApplySpellLock(cooldown);
			StartCoroutine(RunCooldown(cooldown));
			myAnimation[animationName[2]].time = 0;
			myAnimation.Blend(animationName[2], 1f, 0.1f);
			StartCoroutine(DealDamage(0.2f));
			yield return new WaitForSeconds(myAnimation[animationName[2]].length * 0.1f);
			myAnimation.Blend(animationName[1], 0f, 0);
			for (int i = 0; i < trails.Length; i ++)
				trails[i].emit = true;
			yield return new WaitForSeconds(myAnimation[animationName[2]].length * 0.7f);
			for (int i = 0; i < trails.Length; i ++)
				trails[i].emit = false;
			yield return new WaitForSeconds(myAnimation[animationName[2]].length * 0.4f);// * 0.8f);
			myAnimation.Blend(animationName[2], 0f, 0.3f);
			break;
		}
	}
	
	private IEnumerator DealDamage(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		damageDealerBehaviour[damageDealer.index].Position = skillHandler.MyTransformPos;
		damageDealer.Spawn(skillHandler.PlaceholderDamageFrontPos, Quaternion.identity);
	}
	
	private IEnumerator FinishCombo(float waitTime, int currentIndex)
	{
		yield return new WaitForSeconds(waitTime);
		if (currentIndex == index)
		{
			index = 0;
		}
	}
}
