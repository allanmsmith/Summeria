using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIOverheadHandler : MonoBehaviour
{
	public Color damageTextColor;
	public Color bossDamageTextColor;
	public Color healTextColor;
	public Color fearColor;
	public Color rootColor;
	public Color slowColor;
	public Color silenceColor;
	public Color stunColor;
	public Color eventColor;
	
	private GameObject overheadPrefab;
	private List<UIOverhead> overheadInfos;
	
	private Transform myTransform;
	private GameObject tempObject;
	private Vector3 scrollingTextSpeed = new Vector3(0, 1.5f, 0);
	private Vector3 scrollingTextEventSpeed = new Vector3(0, 4.5f, 0);
	
	private static UIOverheadHandler instance;
	public static UIOverheadHandler Instance
	{
		get 
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType(typeof(UIOverheadHandler)) as UIOverheadHandler;
			return instance;
		}
	}
	
	private void Awake()
	{
		instance = this;
		if (overheadInfos == null)
			overheadInfos = new List<UIOverhead>(41);
		if (overheadPrefab == null)
			overheadPrefab = (GameObject)Resources.Load("OverheadPrefab");
		if (myTransform == null)
			myTransform = transform;
	}
	
	private void LateUpdate()
	{
		for (byte i = 0; i < overheadInfos.Count; i++)
		{
			if (overheadInfos[i].TargetTransform != null)
			{
				if (overheadInfos[i].targetRotation.x != -CameraOrbit.Instance.CameraTransform.eulerAngles.x)
				{
					overheadInfos[i].targetRotation.x = -CameraOrbit.Instance.CameraTransform.eulerAngles.x;
					overheadInfos[i].MyTransform.eulerAngles = overheadInfos[i].targetRotation;
				}
				overheadInfos[i].MyTransform.position = overheadInfos[i].TargetTransform.position;
			}
			
			for (byte j = 0; j < overheadInfos[i].StGOs.Length; j++)
			{
				if (overheadInfos[i].StGOs[j].activeSelf)
				{
					overheadInfos[i].StTransforms[j].localPosition += scrollingTextSpeed * Time.deltaTime;
					if (overheadInfos[i].StTransforms[j].localPosition.y > 1.5f)
					{
						if (!overheadInfos[i].stShouldChangeText[j])
						{
							overheadInfos[i].stShouldChangeText[j] = true;
							overheadInfos[i].ShouldUseNewText = true;
							overheadInfos[i].StIndex ++;
							if (overheadInfos[i].StIndex >= overheadInfos[i].StIndexLimit)
								overheadInfos[i].StIndex = 0;
						}
						overheadInfos[i].StColor[j].a -= Time.deltaTime;
						overheadInfos[i].StMaterials[j].color = overheadInfos[i].StColor[j];
						if (overheadInfos[i].StColor[j].a <= 0f)
							overheadInfos[i].StGOs[j].SetActive(false);
					}
				}
			}
			
			for (byte j = 0; j < overheadInfos[i].SthGOs.Length; j++)
			{
				if (overheadInfos[i].SthGOs[j].activeSelf)
				{
					overheadInfos[i].SthTransforms[j].localPosition += scrollingTextSpeed * Time.deltaTime;
					if (overheadInfos[i].SthTransforms[j].localPosition.y > 1.5f)
					{
						if (!overheadInfos[i].sthShouldChangeText[j])
						{
							overheadInfos[i].sthShouldChangeText[j] = true;
							overheadInfos[i].ShouldUseNewTextH = true;
							overheadInfos[i].SthIndex ++;
							if (overheadInfos[i].SthIndex >= overheadInfos[i].SthIndexLimit)
								overheadInfos[i].SthIndex = 0;
						}
						overheadInfos[i].SthColor[j].a -= Time.deltaTime;
						overheadInfos[i].SthMaterials[j].color = overheadInfos[i].SthColor[j];
						if (overheadInfos[i].SthColor[j].a <= 0f)
							overheadInfos[i].SthGOs[j].SetActive(false);
					}
				}
			}
			
			for (byte j = 0; j < overheadInfos[i].StsGOs.Length; j++)
			{
				if (overheadInfos[i].StsGOs[j].activeSelf)
				{
					overheadInfos[i].StsTransforms[j].localPosition -= scrollingTextSpeed * Time.deltaTime;
					if (overheadInfos[i].StsTransforms[j].localPosition.y < -1.5f)
					{
						overheadInfos[i].StsColor[j].a -= Time.deltaTime;
						overheadInfos[i].StsMaterials[j].color = overheadInfos[i].StsColor[j];
						if (overheadInfos[i].StsColor[j].a <= 0f)
							overheadInfos[i].StsGOs[j].SetActive(false);
					}
				}
			}
			
			for (byte j = 0; j < overheadInfos[i].SteGOs.Length; j++)
			{
				if (overheadInfos[i].SteGOs[j].activeSelf)
				{
					overheadInfos[i].SteTransforms[j].localPosition += scrollingTextSpeed * Time.deltaTime;
					if (overheadInfos[i].SteTransforms[j].localPosition.y > 1.5f)
					{
						overheadInfos[i].SteColor[j].a -= Time.deltaTime;
						overheadInfos[i].SteMaterials[j].color = overheadInfos[i].SteColor[j];
						if (overheadInfos[i].SteColor[j].a <= 0f)
							overheadInfos[i].SteGOs[j].SetActive(false);
					}
				}
			}
		}
	}
	
	public UIOverhead AddOverhead(Transform targetTransform)
	{
		if (overheadPrefab == null)
			overheadPrefab = (GameObject)Resources.Load("OverheadPrefab");
		tempObject = (GameObject)Instantiate(overheadPrefab);
		if (myTransform == null)
			myTransform = transform;
		tempObject.transform.parent = myTransform;
		if (overheadInfos == null)
			overheadInfos = new List<UIOverhead>(41);
		overheadInfos.Add(tempObject.GetComponent<UIOverhead>());
		overheadInfos[overheadInfos.Count - 1].TargetTransform = targetTransform;
		overheadInfos[overheadInfos.Count - 1].healthGameObject.SetActive(false);
		overheadInfos[overheadInfos.Count - 1].OverheadIndex = overheadInfos.Count - 1;
		return overheadInfos[overheadInfos.Count - 1];
	}
	
	public void AddOverheadDamage(int oIndex, float damage, bool boss = false)
	{
		if (overheadInfos[oIndex].ShouldUseNewText)
		{
			overheadInfos[oIndex].ShouldUseNewText = false;
			overheadInfos[oIndex].CurrentDamageValue = damage;
			overheadInfos[oIndex].Sts[overheadInfos[oIndex].StIndex].text = "-" + overheadInfos[oIndex].CurrentDamageValue.ToString("f0");
			overheadInfos[oIndex].StTransforms[overheadInfos[oIndex].StIndex].localPosition = Vector3.zero;
			if (!boss)
			{
				overheadInfos[oIndex].StMaterials[overheadInfos[oIndex].StIndex].color = damageTextColor;
				overheadInfos[oIndex].StColor[overheadInfos[oIndex].StIndex] = damageTextColor;
			}
			else
			{
				overheadInfos[oIndex].StMaterials[overheadInfos[oIndex].StIndex].color = bossDamageTextColor;
				overheadInfos[oIndex].StColor[overheadInfos[oIndex].StIndex] = bossDamageTextColor;
			}
			overheadInfos[oIndex].stShouldChangeText[overheadInfos[oIndex].StIndex] = false;
			overheadInfos[oIndex].StGOs[overheadInfos[oIndex].StIndex].SetActive(true);
		}
		else
		{
			overheadInfos[oIndex].CurrentDamageValue += damage;
			overheadInfos[oIndex].Sts[overheadInfos[oIndex].StIndex].text = "-" + overheadInfos[oIndex].CurrentDamageValue.ToString("f0");
		}
	}
	
	public void AddOverheadHeal(int oIndex, float heal)
	{
		if (overheadInfos[oIndex].ShouldUseNewTextH)
		{
			overheadInfos[oIndex].ShouldUseNewTextH = false;
			overheadInfos[oIndex].CurrentHealValue = heal;
			overheadInfos[oIndex].Sths[overheadInfos[oIndex].SthIndex].text = "+" + overheadInfos[oIndex].CurrentHealValue.ToString("f0");
			overheadInfos[oIndex].SthTransforms[overheadInfos[oIndex].SthIndex].localPosition = Vector3.zero;
			overheadInfos[oIndex].SthMaterials[overheadInfos[oIndex].SthIndex].color = healTextColor;
			overheadInfos[oIndex].SthColor[overheadInfos[oIndex].SthIndex] = healTextColor;
			overheadInfos[oIndex].sthShouldChangeText[overheadInfos[oIndex].SthIndex] = false;
			overheadInfos[oIndex].SthGOs[overheadInfos[oIndex].SthIndex].SetActive(true);
		}
		else
		{
			overheadInfos[oIndex].CurrentHealValue += heal;
			overheadInfos[oIndex].Sths[overheadInfos[oIndex].SthIndex].text = "+" + overheadInfos[oIndex].CurrentHealValue.ToString("f0");
		}
	}
	
	public void AddOverheadStatus(int oIndex, Lists.Status status)
	{
		overheadInfos[oIndex].Stss[overheadInfos[oIndex].StsIndex].text = status.ToString();
		overheadInfos[oIndex].StsTransforms[overheadInfos[oIndex].StsIndex].localPosition = Vector3.zero;
		
		switch (status)
		{
			case Lists.Status.Feared:
			overheadInfos[oIndex].StsMaterials[overheadInfos[oIndex].StsIndex].color = fearColor;
			overheadInfos[oIndex].StsColor[overheadInfos[oIndex].StsIndex] = fearColor;
			break;
			case Lists.Status.Rooted:
			overheadInfos[oIndex].StsMaterials[overheadInfos[oIndex].SthIndex].color = rootColor;
			overheadInfos[oIndex].StsColor[overheadInfos[oIndex].SthIndex] = rootColor;
			break;
			case Lists.Status.Silenced:
			overheadInfos[oIndex].StsMaterials[overheadInfos[oIndex].StsIndex].color = silenceColor;
			overheadInfos[oIndex].StsColor[overheadInfos[oIndex].StsIndex] = silenceColor;
			break;
			case Lists.Status.Slowed:
			overheadInfos[oIndex].StsMaterials[overheadInfos[oIndex].StsIndex].color = slowColor;
			overheadInfos[oIndex].StsColor[overheadInfos[oIndex].StsIndex] = slowColor;
			break;
			case Lists.Status.Stunned:
			overheadInfos[oIndex].StsMaterials[overheadInfos[oIndex].StsIndex].color = stunColor;
			overheadInfos[oIndex].StsColor[overheadInfos[oIndex].StsIndex] = stunColor;
			break;
		}
		
		overheadInfos[oIndex].StsGOs[overheadInfos[oIndex].StsIndex].SetActive(true);
	}
	
	public void AddOverheadEvent(int oIndex, string eventText)
	{
		overheadInfos[oIndex].Stes[overheadInfos[oIndex].SteIndex].text = eventText;
		overheadInfos[oIndex].SteTransforms[overheadInfos[oIndex].SteIndex].localPosition = Vector3.zero;
		overheadInfos[oIndex].SteMaterials[overheadInfos[oIndex].SteIndex].color = eventColor;
		overheadInfos[oIndex].SteColor[overheadInfos[oIndex].SteIndex] = eventColor;
		overheadInfos[oIndex].SteGOs[overheadInfos[oIndex].SteIndex].SetActive(true);
	}
}
