using UnityEngine;
using System.Collections;

public class CCInputController : MonoBehaviour
{
	//Camera do GUI
	private Camera guiCamera;
	
	//Garante que os toques e cliques utilizando a GUICamera sejam apenas na layerGUI
	private LayerMask layerGUI;
	
	//private CharacterSkillHandler skillHandler;
	//public CharacterSkillHandler SkillHandler { set { skillHandler = value; } }
	
	//private HealthHandler healthHandler;
	//public HealthHandler HealthHandlerSet { set { healthHandler = value; } }
	
	private int activeSpellIndex = 1;
	public delegate void UseSpell(int spellIndex);
	public event UseSpell onSpellUse;
	
	private static CCInputController instance;
	public static CCInputController Instance
	{
		get 
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType(typeof(CCInputController)) as CCInputController;
			return instance;
		}
	}
	
	private void Awake()
	{
		instance = this;
	}
	
	private void Start ()
	{
		//guiCamera = GameObject.Find("GUICamera").GetComponent<Camera>(); //Encontra a camera do GUI
		//layerGUI = 1 << LayerMask.NameToLayer("GUILayer"); //Define a Layer para raycasts
		#if UNITY_WEBPLAYER
		//MouseUsePotion tempBehaviour = GameObject.Find("PotionButton").GetComponent<MouseUsePotion>();
		//if (tempBehaviour != null)
		//	tempBehaviour.healthHandler = healthHandler;
		#endif
	}
	
	
	private void LateUpdate ()
	{
		PCController();
	}
	
	
	private void PCController()
	{
		if (Input.GetMouseButton(0))
		{
			if (onSpellUse != null)
				onSpellUse(0);
		}
		
		if (Input.GetKeyDown(KeyCode.Alpha1))
			activeSpellIndex = 1;
		
		if (Input.GetKeyDown(KeyCode.Alpha2))
			activeSpellIndex = 2;
		
		if (Input.GetKeyDown(KeyCode.Alpha3))
			activeSpellIndex = 3;
		
		if (Input.GetKeyDown(KeyCode.Alpha4))
			activeSpellIndex = 4;
		
		if (Input.GetKeyDown(KeyCode.Alpha5))
			activeSpellIndex = 5;
		
		if (Input.GetMouseButton(1))
		{
			if (onSpellUse != null)
				onSpellUse(activeSpellIndex);
		}
		
		/*
		//Use skill
		if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1))
		{
			if (skillHandler.skillTemplates[0] != null)
				StartCoroutine(skillHandler.skillTemplates[0].Spell());
		}
		if (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2))
		{
			if (skillHandler.skillTemplates[1] != null)
				StartCoroutine(skillHandler.skillTemplates[1].Spell());
		}
		if (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3))
		{
			if (skillHandler.skillTemplates[2] != null)
				StartCoroutine(skillHandler.skillTemplates[2].Spell());
		}
		if (Input.GetKey(KeyCode.Alpha4) || Input.GetKey(KeyCode.Keypad4))
		{
			if (skillHandler.skillTemplates[3] != null)
				StartCoroutine(skillHandler.skillTemplates[3].Spell());
		}
		if (Input.GetKey(KeyCode.Alpha5) || Input.GetKey(KeyCode.Keypad4))
		{
			if (skillHandler.skillTemplates[4] != null)
				StartCoroutine(skillHandler.skillTemplates[4].Spell());
		}
		//----------------------------------------------------
		
		if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
		{
			if (MobileStats.potions > 0)
			{
				if (healthHandler.Health < healthHandler.MaxHealth )
				{
					MobileStats.potions --;
					SoundManager.PlaySound(SoundManager.clipGenericPotion, transform);
					healthHandler.RecoverHealth(healthHandler.MaxHealth  * 0.5f);
				}
			}
		}*/
	}
}
