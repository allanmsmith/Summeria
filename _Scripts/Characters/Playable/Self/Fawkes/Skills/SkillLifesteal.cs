using UnityEngine;
using System.Collections;

public class SkillLifesteal : CCSkillTemplate
{
	private ObjectRecycler lsParticles;
	private AudioSource[] audioSources;
	
	protected override void Awake()
	{
		id = Lists.Spell.Lifesteal;
		
		base.Awake();
		
		lsParticles = new ObjectRecycler();
		lsParticles.CreatePool(ObjectName.skillHolder, "Fawkes-Lifesteal-Particles", spellPrefab[0], 3);
	}
	
	public override IEnumerator SpellCast (Vector3 position)
	{
		StartCoroutine(RunCooldown(cooldown));
		skillHandler.HealthHandler.ApplySpellLock(myAnimation[animationName[0]].length * 0.9f);
		myAnimation[animationName[0]].time = 0;
		myAnimation.Blend(animationName[0], 1f, 0.1f);
		yield return new WaitForSeconds(myAnimation[animationName[0]].length * 0.32f);
		skillHandler.HealthHandler.ApplyLifesteal(timePerTick);
		//lsParticles.SpawnLocal(Vector3.zero, Quaternion.identity);
		yield return new WaitForSeconds(myAnimation[animationName[0]].length * 0.58f);
		myAnimation.Blend(animationName[0], 0f, myAnimation[animationName[0]].length * 0.1f);
	}
}