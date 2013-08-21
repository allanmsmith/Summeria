using UnityEngine;
using System.Collections;

public class SwordBoomerangNetworkProjectile : MonoBehaviour
{
	//Implementar codigo para ativar o skin certo
	public Renderer[] swordRenderers;
	public NewTimedTrailRenderer[] trailRenderers;
	private Transform myTransform;
	
	private void Awake()
	{
		myTransform = transform;
		for (byte i = 0; i < trailRenderers.Length; i ++)
			trailRenderers[i].emit = false;
	}
	
	private void OnEnable()
	{
		swordRenderers[0].enabled = true;
		for (byte i = 0; i < trailRenderers.Length; i ++)
			trailRenderers[i].emit = true;
	}
	
	private void DestroyProjectile()
	{
		swordRenderers[0].enabled = false;
		for (byte i = 0; i < trailRenderers.Length; i ++)
				trailRenderers[i].emit = false;
		StartCoroutine(DisableThisTimed());
	}
	
	private IEnumerator DisableThisTimed()
	{
		yield return new WaitForSeconds(4);
		gameObject.SetActive(false);
	}
}
