using UnityEngine;
using System.Collections;

public class DamageDealerColliderConstant : MonoBehaviour
{
	public float timePerTick;
	
	private DamageDealer dmgDealer;
	public DamageDealer DmgDealer { set { dmgDealer = value; } }
	
	private bool shouldDealDamage;
	private Collider myCollider;
	
	private void Awake()
	{
		myCollider = collider;
	}
	
	private void OnEnable()
	{
		StartCoroutine(CheckTickTime());
	}
	
	private void OnTriggerStay (Collider col)
	{
		if (shouldDealDamage)
			dmgDealer.DealDamage(col.gameObject);
	}
	
	private IEnumerator CheckTickTime()
	{
		while (true)
		{
			shouldDealDamage = false;
			//myCollider.enabled = false;
			yield return new WaitForSeconds(timePerTick);
			shouldDealDamage = true;
			//myCollider.enabled = true;
			yield return null;
		}
	}
}
