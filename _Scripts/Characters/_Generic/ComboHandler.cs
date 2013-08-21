using UnityEngine;
using System.Collections;

public class ComboHandler : MonoBehaviour
{
	private UILabel comboLabel;
	public Transform comboEffectsPlaceholder;
	private Vector3 comboEffectsPos;
	public GameObject comboHolder;
	public GameObject comboLabelGO;
	public GameObject comboLabelHitsGO;
	private Transform comboLabelTrans;
	private Vector3 comboLabelPos;
	private Vector3 comboLabelIniScale;
	private TweenScale comboLabelScaleTween;
	private TweenAlpha comboLabelAlphaTween;
	
	private int comboCounter;
	private float comboTimer;
	private float comboTimerCounter;
	
	private bool comboOnCooldown;
	private bool comboRunning;
	
	public GameObject comboEffectPrefab;
	private ObjectRecycler comboEffects;
	
	private static ComboHandler instance;
	public static ComboHandler Instance
	{
		get 
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType(typeof(ComboHandler)) as ComboHandler;
			return instance;
		}
	}
	
	private void Awake()
	{
		instance = this;
		
		comboEffects = new ObjectRecycler();
		comboEffects.CreatePool(ObjectName.comboHolder, "Combo-Effects", comboEffectPrefab, 20);
		//comboEffectsPlaceholder = GameObject.Find("Combo-Effects-Holder").transform;
		for (byte i = 0; i < comboEffects.all.Count; i++)
		{
			comboEffects.all[i].transform.parent = comboEffectsPlaceholder;
			comboEffects.all[i].transform.localPosition = Vector3.zero;
		}
		
		//comboLabelGO = GameObject.Find("Combo-Label");
		comboLabel = comboLabelGO.GetComponent<UILabel>();
		comboLabelTrans = comboLabel.transform;
		comboLabelPos = comboLabelTrans.position;
		comboLabelIniScale = comboLabelTrans.localScale;
		comboHolder.SetActive(false);
	}
	
	public void AddCombo()
	{
		comboCounter ++;
		if (comboCounter > 1)
		{
			comboLabel.text = comboCounter.ToString();
			//Ate 100 combos, o tempo pra combar diminue
			comboTimerCounter = 2f + Mathf.Max(0, (1 - (comboCounter * 0.01f)));
			
			if (!comboOnCooldown)
			{
				StartCoroutine(RunComboCooldown());
				if (!comboRunning)
				{
					comboRunning = true;
					comboHolder.SetActive(true);
					comboLabelAlphaTween = TweenAlpha.Begin(comboLabelGO, 0.2f, 1);
					comboLabelAlphaTween.ignoreTimeScale = false;
					TweenAlpha.Begin(comboLabelHitsGO, 0.2f, 1).ignoreTimeScale = false;
					StopCoroutine("FinishCombo");
					StartCoroutine("FinishCombo");
				}
				comboEffects.SpawnLocal(Vector3.zero, Quaternion.identity);
				comboLabelTrans.localScale = comboLabelIniScale * 1.5f;
				comboLabelScaleTween = TweenScale.Begin(comboLabelGO, 0.2f, comboLabelIniScale);
				comboLabelScaleTween.ignoreTimeScale = false;
			}
		}
	}
	
	private IEnumerator RunComboCooldown()
	{
		comboOnCooldown = true;
		yield return new WaitForSeconds(0.1f);
		comboOnCooldown = false;
	}
	
	private IEnumerator FinishCombo()
	{
		while (comboTimerCounter > 0)
		{
			yield return new WaitForSeconds(0.2f);
			comboTimerCounter -= 0.2f;
		}
		comboRunning = false;
		comboLabelAlphaTween = TweenAlpha.Begin(comboLabelGO, 0.2f, 0);
		comboLabelAlphaTween.ignoreTimeScale = false;
		TweenAlpha.Begin(comboLabelHitsGO, 0.2f, 0).ignoreTimeScale = false;
		comboCounter = 0;
		yield return new WaitForSeconds(0.2f);
		comboHolder.SetActive(false);
	}
}
