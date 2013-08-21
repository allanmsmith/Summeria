using UnityEngine;
using System.Collections;

public class DamageDealerCollider : MonoBehaviour
{
	private DamageDealer dmgDealer;
	public DamageDealer DmgDealer { set { dmgDealer = value; } }
	
	private void OnEnable()
	{
		NMGame.Instance.SendCollision(dmgDealer.transform.position, dmgDealer.transform.localScale);
	}
	
	/*
	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.layer == 9 || col.gameObject.layer == 10) //9 = Player / 10 = Enemy
		{
			dmgDealer.DealDamage(col.gameObject);
		}
	}*/
}
