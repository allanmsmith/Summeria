using UnityEngine;
using System.Collections;

public class UIOverhead : MonoBehaviour
{
	public Transform healthBar;
	public GameObject healthGameObject;
	public Transform scrollingTextHolder;
	public Transform scrollingTextHealHolder;
	public Transform scrollingTextStatusHolder;
	public Transform scrollingTextEventHolder;
	
	private int overheadIndex;
	public int OverheadIndex { set { overheadIndex = value; } get { return overheadIndex; } }
	
	private Transform targetTransform;
	public Transform TargetTransform { set { targetTransform = value; } get { return targetTransform; } }
	
	[HideInInspector]
	public Vector3 targetRotation;
	
	private Transform myTransform;
	public Transform MyTransform { get { return myTransform; } }
	
	#region Damage Variables
	private int stIndex;
	public int StIndex { get { return stIndex; } set { stIndex = value; } }
	
	private int stIndexLimit;
	public int StIndexLimit { get { return stIndexLimit; } }
	
	private GameObject[] stGOs;
	public GameObject[] StGOs { get { return stGOs; } }
	
	private Transform[] stTransforms;
	public Transform[] StTransforms { get { return stTransforms; } }
	
	private Material[] stMaterials;
	public Material[] StMaterials { get { return stMaterials; } set { stMaterials = value; } }
	
	private Color[] stColor;
	public Color[] StColor { get { return stColor; } set { stColor = value; } }
	
	[HideInInspector]
	public bool[] stShouldChangeText;
	
	private TextMesh[] sts;
	public TextMesh[] Sts { get { return sts; } }
	
	public bool shouldUseNewText;
	public bool ShouldUseNewText { set { shouldUseNewText = value; } get { return shouldUseNewText; } }
	
	private float currentDamageValue;
	public float CurrentDamageValue { get { return currentDamageValue; } set { currentDamageValue = value; } }
	#endregion
	
	#region Heal Variables
	private int sthIndex;
	public int SthIndex { get { return sthIndex; } set { sthIndex = value; } }
	
	private int sthIndexLimit;
	public int SthIndexLimit { get { return sthIndexLimit; } }
	
	private GameObject[] sthGOs;
	public GameObject[] SthGOs { get { return sthGOs; } }
	
	private Transform[] sthTransforms;
	public Transform[] SthTransforms { get { return sthTransforms; } }
	
	private Material[] sthMaterials;
	public Material[] SthMaterials { get { return sthMaterials; } set { sthMaterials = value; } }
	
	private Color[] sthColor;
	public Color[] SthColor { get { return sthColor; } set { sthColor = value; } }
	
	[HideInInspector]
	public bool[] sthShouldChangeText;
	
	private TextMesh[] sths;
	public TextMesh[] Sths { get { return sths; } }
	
	public bool shouldUseNewTextH;
	public bool ShouldUseNewTextH { set { shouldUseNewTextH = value; } get { return shouldUseNewTextH; } }
	
	private float currentHealValue;
	public float CurrentHealValue { get { return currentHealValue; } set { currentHealValue = value; } }
	#endregion
	
	#region Status Variables
	private int stsIndex;
	public int StsIndex { get { return stsIndex; } set { stsIndex = value; } }
	
	private int stsIndexLimit;
	public int StsIndexLimit { get { return stsIndexLimit; } }
	
	private GameObject[] stsGOs;
	public GameObject[] StsGOs { get { return stsGOs; } }
	
	private Transform[] stsTransforms;
	public Transform[] StsTransforms { get { return stsTransforms; } }
	
	private Material[] stsMaterials;
	public Material[] StsMaterials { get { return stsMaterials; } set { stsMaterials = value; } }
	
	private Color[] stsColor;
	public Color[] StsColor { get { return stsColor; } set { stsColor = value; } }
	
	private TextMesh[] stss;
	public TextMesh[] Stss { get { return stss; } }
	#endregion
	
	#region Event Variables
	private int steIndex;
	public int SteIndex { get { return steIndex; } set { steIndex = value; } }
	
	private int steIndexLimit;
	public int SteIndexLimit { get { return steIndexLimit; } }
	
	private GameObject[] steGOs;
	public GameObject[] SteGOs { get { return steGOs; } }
	
	private Transform[] steTransforms;
	public Transform[] SteTransforms { get { return steTransforms; } }
	
	private Material[] steMaterials;
	public Material[] SteMaterials { get { return steMaterials; } set { steMaterials = value; } }
	
	private Color[] steColor;
	public Color[] SteColor { get { return stsColor; } set { steColor = value; } }
	
	private TextMesh[] stes;
	public TextMesh[] Stes { get { return stes; } }
	#endregion
	
	private void Awake()
	{
		myTransform = transform;
		sts = scrollingTextHolder.GetComponentsInChildren<TextMesh>();
		stGOs = new GameObject[sts.Length];
		stTransforms = new Transform[sts.Length];
		stMaterials = new Material[sts.Length];
		stColor = new Color[sts.Length];
		stShouldChangeText = new bool[sts.Length];
		for (byte i = 0; i < stGOs.Length; i++)
		{
			stTransforms[i] = sts[i].transform;
			stGOs[i] = sts[i].gameObject;
			stMaterials[i] = sts[i].renderer.material;
			stGOs[i].SetActive(false);
		}
		stIndexLimit = stGOs.Length;
		shouldUseNewText = true;
		
		sths = scrollingTextHealHolder.GetComponentsInChildren<TextMesh>();
		sthGOs = new GameObject[sths.Length];
		sthTransforms = new Transform[sths.Length];
		sthMaterials = new Material[sths.Length];
		sthColor = new Color[sths.Length];
		sthShouldChangeText = new bool[sths.Length];
		for (byte i = 0; i < sthGOs.Length; i++)
		{
			sthTransforms[i] = sths[i].transform;
			sthGOs[i] = sths[i].gameObject;
			sthMaterials[i] = sths[i].renderer.material;
			sthGOs[i].SetActive(false);
		}
		sthIndexLimit = sthGOs.Length;
		shouldUseNewTextH = true;
		
		stss = scrollingTextStatusHolder.GetComponentsInChildren<TextMesh>();
		stsGOs = new GameObject[stss.Length];
		stsTransforms = new Transform[stss.Length];
		stsMaterials = new Material[stss.Length];
		stsColor = new Color[stss.Length];
		for (byte i = 0; i < stsGOs.Length; i++)
		{
			stsTransforms[i] = stss[i].transform;
			stsGOs[i] = stss[i].gameObject;
			stsMaterials[i] = stss[i].renderer.material;
			stsGOs[i].SetActive(false);
		}
		stsIndexLimit = stsGOs.Length;
		
		stes = scrollingTextEventHolder.GetComponentsInChildren<TextMesh>();
		steGOs = new GameObject[stes.Length];
		steTransforms = new Transform[stes.Length];
		steMaterials = new Material[stes.Length];
		steColor = new Color[stes.Length];
		for (byte i = 0; i < steGOs.Length; i++)
		{
			steTransforms[i] = stes[i].transform;
			steGOs[i] = stes[i].gameObject;
			steMaterials[i] = stes[i].renderer.material;
			steGOs[i].SetActive(false);
		}
		steIndexLimit = steGOs.Length;
	}
	
	private void Start()
	{
		if (targetRotation == Vector3.zero)
		{
			targetRotation.x = -CameraOrbit.Instance.CameraTransform.eulerAngles.x;
			targetRotation.y = 180;
		}
	}
}
