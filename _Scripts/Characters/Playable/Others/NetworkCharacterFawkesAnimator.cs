using UnityEngine;
using System.Collections;

public class NetworkCharacterFawkesAnimator : NetworkCharacterAnimator
{
	protected string animNameSlash = "slash";
	protected string animNameSlash02 = "slash02";
	protected string animNameSlash03 = "slash03";
	protected string animNameSwordBoomerang = "swordBoomerang";
	protected string animNameLifeSteal = "lifeSteal";
	protected string animNameWhirlwindStrike = "whirlwindStrike";
	protected string animNameChargeLoop = "chargeLoop";
	protected string animNameChargeStrike = "chargeStrike";
	protected string animNameVigor = "vigor";
	
	protected AnimationState animSlash;
	protected AnimationState animSlash02;
	protected AnimationState animSlash03;
	protected AnimationState animSwordBoomerang;
	protected AnimationState animLifeSteal;
	protected AnimationState animWhirldwindStrike;
	protected AnimationState animChargeLoop;
	protected AnimationState animChargeStrike;
	protected AnimationState animVigor;
	
	private bool skillLocked;
	private int index;
	
	private NewTimedTrailRenderer[] trails;
	
	public override void PrepareAnimations ()
	{
		base.PrepareAnimations ();
		
		trails = GetComponentsInChildren<NewTimedTrailRenderer>();
		for (int i = 0; i < trails.Length; i ++)
		{
			trails[i].emit = false;
		}
		
		animSlash = myAnimation[animNameSlash];
		animSlash02 = myAnimation[animNameSlash02];
		animSlash03 = myAnimation[animNameSlash03];
		animSwordBoomerang = myAnimation[animNameSwordBoomerang];
		animLifeSteal = myAnimation[animNameLifeSteal];
		animWhirldwindStrike = myAnimation[animNameWhirlwindStrike];
		animChargeLoop = myAnimation[animNameChargeLoop];
		animChargeStrike = myAnimation[animNameChargeStrike];
		animVigor = myAnimation[animNameVigor];
		
		animSlash.wrapMode = WrapMode.ClampForever;
        animSlash.blendMode = AnimationBlendMode.Blend;
		animSlash.layer = 5;
		//animSlash.speed = 1.5f;
		animSlash.AddMixingTransform(torsoTransform);
		
		animSlash02.wrapMode = WrapMode.ClampForever;
        animSlash02.blendMode = AnimationBlendMode.Blend;
		animSlash02.layer = 6;
		//animSlash02.speed = 1.5f;
		animSlash02.AddMixingTransform(torsoTransform);
		
		animSlash03.wrapMode = WrapMode.ClampForever;
        animSlash03.blendMode = AnimationBlendMode.Blend;
		animSlash03.layer = 7;
		animSlash03.AddMixingTransform(torsoTransform);
		
		animSwordBoomerang.wrapMode = WrapMode.Clamp;
        animSwordBoomerang.blendMode = AnimationBlendMode.Blend;
		animSwordBoomerang.layer = 8;
		animSwordBoomerang.AddMixingTransform(torsoTransform);
		
		animLifeSteal.wrapMode = WrapMode.Clamp;
        animLifeSteal.blendMode = AnimationBlendMode.Blend;
		animLifeSteal.layer = 8;
		animLifeSteal.AddMixingTransform(torsoTransform);
		
		animWhirldwindStrike.wrapMode = WrapMode.Loop;
        animWhirldwindStrike.blendMode = AnimationBlendMode.Blend;
		animWhirldwindStrike.layer = 10;
		animWhirldwindStrike.speed = 1.5f;
		
		animChargeLoop.wrapMode = WrapMode.Loop;
        animChargeLoop.blendMode = AnimationBlendMode.Blend;
		animChargeLoop.layer = 8;
		animChargeLoop.speed = 3f;
		
		animChargeStrike.wrapMode = WrapMode.Clamp;
        animChargeStrike.blendMode = AnimationBlendMode.Blend;
		animChargeStrike.layer = 9;
		animChargeStrike.AddMixingTransform(torsoTransform);
		
		animVigor.wrapMode = WrapMode.Clamp;
        animVigor.blendMode = AnimationBlendMode.Blend;
		animVigor.layer = 8;
		animVigor.AddMixingTransform(torsoTransform);
	}
	
	protected override void Update ()
	{
		base.Update ();
		if (animationsPrepared)
		{
			if (animationsPlaying != null && animationsPlaying != "")
			{
				if (!skillLocked)
				{
					if (animationsPlaying.Contains("a"))
					{
						if (index == 0)
						{
							if (myAnimation[animNameSlash].weight == 0)
							{
								StartCoroutine(PlayAttack());
							}
						}
					}
					
					if (animationsPlaying.Contains("b"))
					{
						if (index == 1)
						{
							if (myAnimation[animNameSlash02].weight == 0)
							{
								StartCoroutine(PlayAttack());
							}
						}
					}
					
					if (animationsPlaying.Contains("c"))
					{
						if (index == 2)
						{
							if (myAnimation[animNameSlash03].weight == 0)
							{
								StartCoroutine(PlayAttack());
							}
						}
					}
				}
			}
			else
			{
				myAnimation.CrossFade(animNameIdle, 0.3f, PlayMode.StopSameLayer);
			}
		}
	}
	
	private IEnumerator PlayAttack()
	{
		skillLocked = true;
		switch (index)
		{
			case 0:
			index = 1;
			myAnimation[animNameSlash].time = 0;
			myAnimation.Blend(animNameSlash, 1f, 0.1f);
			yield return new WaitForSeconds(myAnimation[animNameSlash].length * 0.5f);
			for (int i = 0; i < trails.Length; i ++)
			{
				trails[i].emit = true;
			}
			yield return new WaitForSeconds(myAnimation[animNameSlash].length * 0.5f);
			for (int i = 0; i < trails.Length; i ++)
			{
				trails[i].emit = false;
			}
			skillLocked = false;
			yield return new WaitForSeconds(myAnimation[animNameSlash].length * 0.8f);
			myAnimation.Blend(animNameSlash, 0f, myAnimation[animNameSlash].length * 0.8f);
			break;
			
			case 1:
			index = 2;
			myAnimation[animNameSlash02].time = 0;
			myAnimation.Blend(animNameSlash02, 1f, 0.1f);
			yield return new WaitForSeconds(myAnimation[animNameSlash02].length * 0.1f);
			for (int i = 0; i < trails.Length; i ++)
			{
				trails[i].emit = true;
			}
			yield return new WaitForSeconds(myAnimation[animNameSlash02].length * 0.23f);
			for (int i = 0; i < trails.Length; i ++)
			{
				trails[i].emit = false;
			}
			skillLocked = false;
			yield return new WaitForSeconds(myAnimation[animNameSlash02].length * 3f);
			myAnimation.Blend(animNameSlash02, 0f, myAnimation[animNameSlash02].length * 1f);
			break;
			
			case 2:
			index = 0;
			myAnimation[animNameSlash03].time = 0;
			myAnimation.Blend(animNameSlash03, 1f, 0.1f);
			yield return new WaitForSeconds(myAnimation[animNameSlash03].length * 0.55f);
			for (int i = 0; i < trails.Length; i ++)
			{
				trails[i].emit = true;
			}
			yield return new WaitForSeconds(myAnimation[animNameSlash03].length * 0.18f);
			for (int i = 0; i < trails.Length; i ++)
			{
				trails[i].emit = false;
			}
			yield return new WaitForSeconds(myAnimation[animNameSlash03].length * 0.6f);// * 0.8f);
			myAnimation.Blend(animNameSlash03, 0f, myAnimation[animNameSlash03].length * 0.2f);
			yield return new WaitForSeconds(myAnimation[animNameSlash03].length * 0.2f);
			skillLocked = false;
			break;
		}
	}
}
