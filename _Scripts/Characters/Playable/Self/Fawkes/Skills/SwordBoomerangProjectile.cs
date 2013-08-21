using UnityEngine;
using System.Collections;

public class SwordBoomerangProjectile : MonoBehaviour
{
	//Implementar codigo para ativar o skin certo
	public GameObject damageDealerPrefab;
	public Renderer[] swordRenderers;
	
	public NewTimedTrailRenderer[] trailRenderers;
	
	private ObjectRecycler damageDealer;
	private DamageDealer[] damageDealerBehaviour;
	
	private float damage;
	public float Damage { set { damage = value; } }
	
	private float radius;
	public float Radius { set { radius = value; } }
	
	private float range;
	public float Range { set { range = value; } }
	
	private float speed;
	public float Speed { set { speed = value; } }
	
	private float statusDuration;
	public float StatusDuration { set { statusDuration = value; } }
	
	private Lists.Status applyStatus;
	public Lists.Status ApplyStatus { set { applyStatus = value; } }
	
	private Vector3 targetPosition;
	public Vector3 TargetPosition { set { targetPosition = value; } }
	
	private Vector3 initialPosition;
	private bool travelling;
	private bool forward;
	private TweenPosition tweenPosition;
	private NetworkTransformSender nSender;
	
	private Transform myTransform;
	
	private void Awake()
	{
		myTransform = transform;
		nSender = GetComponent<NetworkTransformSender>();
		
		//trailRenderers = gameObject.GetComponentsInChildren<NewTimedTrailRenderer>();
		for (byte i = 0; i < trailRenderers.Length; i ++)
			trailRenderers[i].emit = false;
		
		damageDealer = new ObjectRecycler();
		damageDealer.CreatePool(ObjectName.skillHolder, "Fawkes-SwordBoomerang-DamageDealer", damageDealerPrefab, 5);
		damageDealerBehaviour = new DamageDealer[damageDealer.all.Count];
		for (int i = 0; i < damageDealer.all.Count; i++)
			damageDealerBehaviour[i] = damageDealer.all[i].GetComponent<DamageDealer>();
	}
	
	private void Start()
	{
		for (int i = 0; i < damageDealerBehaviour.Length; i++)
		{
			damageDealerBehaviour[i].Damage = damage;
			damageDealerBehaviour[i].applyStatus = applyStatus;
			damageDealerBehaviour[i].StatusDuration = statusDuration;
			damageDealerBehaviour[i].gameObject.transform.localScale = new Vector3(radius, radius, radius);
		}
	}
	
	private void OnEnable()
	{
		if (range != 0)
		{
			initialPosition = myTransform.position;
			travelling = true;
			forward = true;
			StartCoroutine(CheckForCollision());
			swordRenderers[0].enabled = true;
			
			for (byte i = 0; i < trailRenderers.Length; i ++)
				trailRenderers[i].emit = true;
			
			targetPosition = initialPosition + (CameraOrbit.Instance.CameraTransform.forward * range);//(targetPosition - initialPosition).normalized
			tweenPosition = TweenPosition.Begin(gameObject, speed, targetPosition);
			tweenPosition.method = UITweener.Method.EaseOut;
			tweenPosition.onFinished += ChangeDirection;
			StartCoroutine(RotateObject());
		}
	}
	
	private void OnDisable()
	{
		if (tweenPosition != null)
			tweenPosition.onFinished -= ChangeDirection;
	}
	
	private IEnumerator RotateObject()
	{
		while (true)
		{
			myTransform.eulerAngles += new Vector3(0, speed * 20, 0);
			yield return null;
		}
	}
	
	private void OnCollisionEnter()
	{
		if (forward)
			ChangeDirection(tweenPosition);
	}
	
	private IEnumerator CheckForCollision()
	{
		while (travelling)
		{
			damageDealerBehaviour[damageDealer.index].Position = myTransform.position;
			damageDealer.Spawn(myTransform.position, Quaternion.identity);
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	private void ChangeDirection(UITweener tween)
	{
		tweenPosition.onFinished -= ChangeDirection;
		forward = false;
		tweenPosition = TweenPosition.Begin(gameObject, speed * 2, PlayerStats.playerTransform.position);
		tweenPosition.method = UITweener.Method.EaseIn;
		tweenPosition.onFinished += DestroyProjectile;
		StartCoroutine(AdjustTrajectory());
	}
	
	private IEnumerator AdjustTrajectory()
	{
		while (true)
		{
			tweenPosition.to = PlayerStats.playerTransform.position;
			//tweenPosition = TweenPosition.Begin(gameObject, speed, PlayerStats.playerTransform.position);
			yield return new WaitForSeconds(0.3f);
		}
	}
	
	private void DestroyProjectile(UITweener tween)
	{
		swordRenderers[0].enabled = false;
		for (byte i = 0; i < trailRenderers.Length; i ++)
				trailRenderers[i].emit = false;
		NMGame.Instance.SendProjectileDestroy(nSender.networkId);
		StartCoroutine(DisableThisTimed());
	}
	
	private IEnumerator DisableThisTimed()
	{
		yield return new WaitForSeconds(4);
		gameObject.SetActive(false);
	}
}
