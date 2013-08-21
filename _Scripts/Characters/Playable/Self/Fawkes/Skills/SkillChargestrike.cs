using UnityEngine;
using System.Collections;

public class SkillChargestrike : CCSkillTemplate
{
	private ObjectRecycler damageDealer;
	private DamageDealer[] damageDealerBehaviour;
	private AudioSource[] audioSources;
	
	private NewTimedTrailRenderer[] trails;
	private CharacterMotor charMotor;
	private CCMover ccMover;
	
	protected override void Awake()
	{
		id = Lists.Spell.Chargestrike;
		charMotor = GetComponent<CharacterMotor>();
		ccMover = GetComponent<CCMover>();
		
		base.Awake();
		
		trails = GetComponentsInChildren<NewTimedTrailRenderer>();
		for (int i = 0; i < trails.Length; i ++)
			trails[i].emit = false;
		
		damageDealer = new ObjectRecycler();
		damageDealer.CreatePool(ObjectName.skillHolder, "Fawkes-ChargeStrike", skillHandler.DamageCollider, 5);
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
		skillHandler.HealthHandler.ApplySpellLock(timePerTick + myAnimation[animationName[1]].length);
		StartCoroutine(RunCooldown(cooldown));
		myAnimation[animationName[0]].time = 0;
		myAnimation.Blend(animationName[0], 1f, 0.1f);
		
		ccMover.Dashing = true;
		for (int i = 0; i < trails.Length; i ++)
			trails[i].emit = true;
		Vector3 tempVector = CameraOrbit.Instance.CameraTransform.forward;
		tempVector.y = 0;
		tempVector.Normalize();
		charMotor.movement.maxForwardSpeed = charMotor.movement.maxBackwardsSpeed = charMotor.movement.maxSidewaysSpeed = speed;
		charMotor.movement.gravity = 0;
		charMotor.SetVelocity(tempVector * speed);
		ccMover.MovingTransform.localRotation = Quaternion.Euler(0, 0, 0);
		float counter = timePerTick;
		while (counter > 0)
		{
			counter -= 0.1f;
			charMotor.SetVelocity(tempVector * speed);
			damageDealer.Spawn(skillHandler.MyTransformPos, Quaternion.identity);
			yield return new WaitForSeconds(0.1f);
		}
		damageDealer.Spawn(skillHandler.MyTransformPos, Quaternion.identity);
		myAnimation.Blend(animationName[1], 1f, myAnimation[animationName[1]].length * 0.1f);
		myAnimation.Blend(animationName[0], 0f, 0.1f);
		charMotor.movement.maxForwardSpeed = charMotor.movement.maxBackwardsSpeed = charMotor.movement.maxSidewaysSpeed = ccMover.MovementSpeed;
		charMotor.movement.gravity = 40;
		yield return new WaitForSeconds(myAnimation[animationName[1]].length * 0.7f);
		myAnimation.Blend(animationName[1], 0f, myAnimation[animationName[1]].length * 0.3f);
		for (int i = 0; i < trails.Length; i ++)
			trails[i].emit = false;
		ccMover.Dashing = false;
	}
}