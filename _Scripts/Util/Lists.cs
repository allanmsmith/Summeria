using UnityEngine;
using System.Collections;

public class Lists : MonoBehaviour
{
	public enum Status
	{
		Slowed,
		Stunned,
		Feared,
		Polymorphed,
		Rooted,
		Silenced,
		SpellLocked,
		MovementLocked,
		ReflectProjectiles,
		DOTed,
		Lifestealing,
		None
	}
	
	public enum Spell
	{
		Slash,
		Swordboomerang,
		Lifesteal,
		Whirlwindstrike,
		Chargestrike,
		Vigor,
		Danceofchaos,
		None
	}
	
	public enum EnemyTypes
	{
		Kosh,
		Nomat,
		Tachat
	}
	
	public enum ProjectileTypes
	{
		SwordBoomerang
	}
}
