using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataStore : MonoBehaviour
{
	//public float[] levelExpRequirements;
	//public WeaponTemplate[] weaponList;
	public SkillTemplate[] skillList;
	//[HideInInspector]
	//public SkillTemplate[] skillListInitial;
	
	private void Awake ()
	{
		if (PlayerStats.playerLevel == 0)
		{
			#if UNITY_WEBPLAYER
			//Playtomic.Initialize(951879, "57a5dc74e7cb4a89", "72ea4cfcafca43e1b0b57ed8b2aaba");
			#endif
			//Playtomic.Initialize(951879, "57a5dc74e7cb4a89", "72ea4cfcafca43e1b0b57ed8b2aaba");
			//Playtomic.Log.View();
			
			PlayerStats.playerLevel = 30;
			//skillListInitial = skillList;
			PlayerStats.skillList = skillList;
			//SortSkillsByLevel();
			
			/*
			PlayerStats.selectedCharacter = 0;
			PlayerStats.equippedSkills = new string[4];
			PlayerStats.equippedSkills[0] = "Autoshot";
			PlayerStats.equippedSkills[1] = "Multishot";*/
			
			
			PlayerStats.selectedCharacter = 1;
			PlayerStats.equippedSkills = new Lists.Spell[6];
			PlayerStats.equippedSkills[0] = Lists.Spell.Slash;
			PlayerStats.equippedSkills[1] = Lists.Spell.Swordboomerang;
			PlayerStats.equippedSkills[2] = Lists.Spell.Lifesteal;
			PlayerStats.equippedSkills[3] = Lists.Spell.Whirlwindstrike;
			PlayerStats.equippedSkills[4] = Lists.Spell.Chargestrike;
			PlayerStats.equippedSkills[5] = Lists.Spell.Vigor;
			//PlayerStats.equippedSkills[1] = "Groundbash";
			//PlayerStats.equippedSkills[2] = "Charge";
			//PlayerStats.equippedSkills[3] = "Smashingtornado";
			//PlayerStats.equippedSkills[4] = "Earthquake";
			
			/*
			PlayerStats.selectedCharacter = 2;
			PlayerStats.equippedSkills = new string[4];
			PlayerStats.equippedSkills[0] = "Fireball";*/
			
			/*if (PlayerStats.skillBasicName == "none" || PlayerStats.skillBasicName == "" || PlayerStats.skillBasicName == null)
				PlayerStats.skillBasicName = "Slash";
			if (PlayerStats.skillSupportName == "none" || PlayerStats.skillSupportName == "" || PlayerStats.skillSupportName == null)
				PlayerStats.skillSupportName = "Bladestorm";
			if (PlayerStats.skillAdvancedName == "none" || PlayerStats.skillAdvancedName == "" || PlayerStats.skillAdvancedName == null)
				PlayerStats.skillAdvancedName = "Lightningfield";
			if (PlayerStats.skillUltimateName == "none" || PlayerStats.skillUltimateName == "" || PlayerStats.skillUltimateName == null)
				PlayerStats.skillUltimateName = "Lightningfield";*/
		
			//LanguageDictionary.languageSelected = true;
			//LanguageDictionary.currentLanguage = "pt-br";
			//LanguageDictionary.SetLanguage("pt-br");
			
			//SoundManager.fxSoundVolume = 1;
			//SoundManager.fxSoundVolumeTemp = 1;
			//SoundManager.musicSoundVolume = 1;
			//SoundManager.musicSoundVolumeTemp = 1;
			
			//PlayerStats.weaponList = weaponList;
			//if (PlayerStats.equippedWeapon == "" || PlayerStats.equippedWeapon == null)
			//		PlayerStats.equippedWeapon = "weapon01";
			
			Application.targetFrameRate = 60;
			//MobileStats.potions = 5;
			//PlayerStats.playerLevel = 30;
			//PlayerStats.gold = 90000;
			//PlayerStats.diamond = 5000;
			DontDestroyOnLoad(gameObject);
			
			//PlayerStats.playerLevelList = levelExpRequirements;
		}
		
		/*
		if (MobileStats.levelFinished == null)
		{
			MobileStats.levelFinished = new Dictionary<int, Achievement>();
			for (int i = 1; i <= levelCompleted.Length; i++)
			{
				MobileStats.levelFinished.Add(i, levelCompleted[i - 1]);
			}
		}
		
		if (MobileStats.bombersKilledAchievements == null)
		{
			MobileStats.bombersKilledAchievements = new Dictionary<int, Achievement>();
			for (int i = 0; i < bombersKilledAchievements.Length; i++)
			{
				MobileStats.bombersKilledAchievements.Add(i, bombersKilledAchievements[i]);
			}
		}
		
		if (MobileStats.lancersKilledAchievements == null)
		{
			MobileStats.lancersKilledAchievements = new Dictionary<int, Achievement>();
			for (int i = 0; i < lancersKilledAchievements.Length; i++)
			{
				MobileStats.lancersKilledAchievements.Add(i, lancersKilledAchievements[i]);
			}
		}
		
		if (MobileStats.soldiersKilledAchievements == null)
		{
			MobileStats.soldiersKilledAchievements = new Dictionary<int, Achievement>();
			for (int i = 0; i < soldiersKilledAchievements.Length; i++)
			{
				MobileStats.soldiersKilledAchievements.Add(i, soldiersKilledAchievements[i]);
			}
		}
		
		if (MobileStats.archersKilledAchievements == null)
		{
			MobileStats.archersKilledAchievements = new Dictionary<int, Achievement>();
			for (int i = 0; i < archersKilledAchievements.Length; i++)
			{
				MobileStats.archersKilledAchievements.Add(i, archersKilledAchievements[i]);
			}
		}
		
		if (MobileStats.beeMagesKilledAchievements == null)
		{
			MobileStats.beeMagesKilledAchievements = new Dictionary<int, Achievement>();
			for (int i = 0; i < beeMagesKilledAchievements.Length; i++)
			{
				MobileStats.beeMagesKilledAchievements.Add(i, beeMagesKilledAchievements[i]);
			}
		}
		
		if (MobileStats.eatersKilledAchievements == null)
		{
			MobileStats.eatersKilledAchievements = new Dictionary<int, Achievement>();
			for (int i = 0; i < eatersKilledAchievements.Length; i++)
			{
				MobileStats.eatersKilledAchievements.Add(i, eatersKilledAchievements[i]);
			}
		}
		
		if (MobileStats.hulksKilledAchievements == null)
		{
			MobileStats.hulksKilledAchievements = new Dictionary<int, Achievement>();
			for (int i = 0; i < hulksKilledAchievements.Length; i++)
			{
				MobileStats.hulksKilledAchievements.Add(i, hulksKilledAchievements[i]);
			}
		}
		
		if (MobileStats.knockOutKilledAchievements == null)
		{
			MobileStats.knockOutKilledAchievements = new Dictionary<int, Achievement>();
			for (int i = 0; i < knockOutAchievements.Length; i++)
			{
				MobileStats.knockOutKilledAchievements.Add(i, knockOutAchievements[i]);
			}
		}
		
		if (MobileStats.bossKilledAchievements == null)
		{
			MobileStats.bossKilledAchievements = new Dictionary<string, Achievement>();
			MobileStats.bossKilledAchievements.Add("olinad", bossKilledAchievements[0]);
			MobileStats.bossKilledAchievements.Add("columbus", bossKilledAchievements[1]);
			MobileStats.bossKilledAchievements.Add("smithson", bossKilledAchievements[2]);
		}*/
		//SaveGame.Load();
	}
	/*
	private void SortSkillsByLevel()
	{
		int i, j;
		SkillTemplate tmp;
		for (i = 1; i < skillList.Length; i++)
		{
			j = i;
			while (j > 0 && skillList[j - 1].levelRequired > skillList[j].levelRequired)
			{
				tmp = skillList[j];
				skillList[j] = skillList[j - 1];
				skillList[j - 1] = tmp;
				j--;
			}
		}
	}
	
	private void OnApplicationQuit()
	{
		//Playtomic.Log.ForceSend();
	}*/
}
/*
[System.Serializable]
public class SkillTemplate
{
	//Skill Infos
	public string id;
	public float cooldown;
	public float range;
	public float damage;
	public float force;
	public float speed;
	public float radius;
	public float ticks;
	public float timePerTick;
	public string animationName;
	public GameObject[] spellPrefab;
	public Texture2D texture;
	public AudioClip audio;
	public int spellLevel;
	//Shop infos
	public string skillType;
	public string skillName;
	public string skillClass;
	public string damageModifier;
	public string specialEffects;
	public float priceGold;
	public float priceMoney;
	public bool bought;
	public bool unlocked;
	public int levelRequired;
}*/

[System.Serializable]
public class SkillTemplate
{
	//Skill Infos
	public string name;
	public Lists.Spell id;
	public Lists.Status applyStatus = Lists.Status.None;
	public Lists.Status favoredTarget = Lists.Status.None;
	public float cooldown;
	public float damage;
	public float range;
	public float force;
	public float speed;
	public float radius;
	public int ticks;
	public float timePerTick;
	public string[] animationName;
	public GameObject[] spellPrefab;
	public AudioClip[] audio;
	
	//public string description;
	//public float goldPrice;
	//public float rubyPrice;
	//public int levelRequired;
	public bool unlocked;
}

[System.Serializable]
public class WeaponTemplate
{
	//Weapon Info
	public string weaponId;
	public float meleeModifier;
	public float spellModifier;
	public float healthModifier;
	public GameObject weaponPrefab;
	public bool mainHand;
	//Shop info
	public Texture2D weaponIcon;
	public float priceGold;
	public float priceMoney;
	public bool bought;
	public bool unlocked;
	public int levelRequired;
}

[System.Serializable]
public class Achievement
{
	public bool unlocked = false;
	public string text;
	public Texture2D icon;
	public float points;
}
