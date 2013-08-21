using UnityEngine;
using System.Collections;

public static class PlayerStats
{
	public static string config = "fawkes|boots|standard|head|standard|pants|standard|torso|standard";
	
	//0 = Wyatt, 1 = Dante, 2 = Lyle
	public static int selectedCharacter = 0; 
	
	//Estatisticas do personagem
	public static string slotName;
	public static float gold;
	public static float diamond;
	public static float playerLevel;
	public static float[] playerLevelList;
	public static float playerExperience;
	//--------------------------
	
	//Variaveis referentes às armas
	//public static WeaponTemplate[] weaponList;
	public static string equippedWeapon;
	public static float damageModifier;
	public static float healthModifier;
	//-----------------------------
	
	//Variaveis referentes às skills
	public static SkillTemplate[] skillList;
	
	public static Lists.Spell[] equippedSkills;
	
	public static string skillBasicName;
	public static string skillSupportName;
	public static string skillAdvancedName;
	public static string skillUltimateName;
	//public static int selectedSkillNumber;
	//public static float selectedSkillRange;
	//------------------------------
	
	public static Transform playerTransform;
	//public static PlayerHealthHandler healthHandler;
	//public static string selectedSkill;
	//public static float selectedSkillRange;
}
