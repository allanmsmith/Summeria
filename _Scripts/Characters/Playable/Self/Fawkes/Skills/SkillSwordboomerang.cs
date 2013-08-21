using UnityEngine;
using System.Collections;

public class SkillSwordboomerang : CCSkillTemplate
{
	private ObjectRecycler swordBoomerang;
	private SwordBoomerangProjectile[] swordBoomerangBehaviour;
	private AudioSource[] audioSources;
	private NetworkTransformSender[] nSender;
	
	protected override void Awake()
	{
		id = Lists.Spell.Swordboomerang ;
		
		base.Awake();
		
		swordBoomerang = new ObjectRecycler();
		swordBoomerang.CreatePool(ObjectName.skillHolder, "Fawkes-SwordBoomerang", spellPrefab[0], 3);
		swordBoomerangBehaviour = new SwordBoomerangProjectile[swordBoomerang.all.Count];
		for (int i = 0; i < swordBoomerang.all.Count; i++)
		{
			swordBoomerangBehaviour[i] = swordBoomerang.all[i].GetComponent<SwordBoomerangProjectile>();
			swordBoomerangBehaviour[i].Damage = damage;// * (1 + PlayerStats.damageModifier) * (1 + (PlayerStats.playerLevel * 0.1f));
			swordBoomerangBehaviour[i].Range = range;
			swordBoomerangBehaviour[i].Radius = radius;
			swordBoomerangBehaviour[i].Speed = speed;
			swordBoomerangBehaviour[i].StatusDuration = 5;
			swordBoomerangBehaviour[i].ApplyStatus = Lists.Status.Slowed;
		}
		nSender = new NetworkTransformSender[swordBoomerang.all.Count];
		for (int i = 0; i < swordBoomerang.all.Count; i++)
		{
			nSender[i] = swordBoomerang.all[i].GetComponent<NetworkTransformSender>();
		}
	}
	
	public override IEnumerator SpellCast (Vector3 position)
	{
		skillHandler.HealthHandler.ApplySpellLock(myAnimation[animationName[0]].length * 0.9f);
		StartCoroutine(RunCooldown(cooldown));
		myAnimation[animationName[0]].time = 0;
		myAnimation.Blend(animationName[0], 1f, 0.1f);
		yield return new WaitForSeconds(myAnimation[animationName[0]].length * 0.32f);
		NMGame.Instance.SendProjectileSpawnRequest(nSender[swordBoomerang.index].LocalId, Lists.ProjectileTypes.SwordBoomerang);
		swordBoomerang.Spawn(skillHandler.MyTransformPos, Quaternion.identity);
		yield return new WaitForSeconds(myAnimation[animationName[0]].length * 0.58f);
		myAnimation.Blend(animationName[0], 0f, myAnimation[animationName[0]].length * 0.1f);
	}
}