using UnityEngine;
using System.Collections;

public class DamageDealerSlow : MonoBehaviour
{
	public float damage;
	public Vector3 position;
	public float force;
	private HealthHandler healthHandler;
	
	private GameObject myGameObject;
	private Transform myTransform;
	
	private int hits;
	
	private void Awake ()
	{
		myGameObject = gameObject;
		myGameObject.active = false;
	}
	
	private void OnEnable()
	{
		StartCoroutine(AutoDestroyer());
		hits = 0;
	}
	
	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.layer == 11 || col.gameObject.layer == 10 || col.gameObject.layer == 23) //11 = Enemy / 10 = Player / 23 = boss
		{
			hits ++;
			col.rigidbody.AddForce((col.transform.position - position).normalized * force, ForceMode.Impulse);
			col.gameObject.GetComponent<HealthHandler>().TakeDamage(damage);
			//if (hits >= 3)
			//	EffectsHandler.Call.SlowTime();
		}
	}
	
	private IEnumerator AutoDestroyer()
	{
		yield return new WaitForSeconds(0.1f);
		myGameObject.active = false;
	}
}
