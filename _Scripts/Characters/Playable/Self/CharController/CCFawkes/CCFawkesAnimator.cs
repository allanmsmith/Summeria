using UnityEngine;
using System.Collections;

public class CCFawkesAnimator : CCAnimator
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
	
	protected override void Start()
	{
		base.Start();
		
		animationList.Add('a');
		animationList.Add('b');
		animationList.Add('c');
		animationList.Add('d');
		animationList.Add('e');
		animationList.Add('f');
		animationList.Add('g');
		animationList.Add('h');
		animationList.Add('j');
		
		animationNames.Add('a', animNameSlash);
		animationNames.Add('b', animNameSlash02);
		animationNames.Add('c', animNameSlash03);
		animationNames.Add('d', animNameSwordBoomerang);
		animationNames.Add('e', animNameLifeSteal);
		animationNames.Add('f', animNameWhirlwindStrike);
		animationNames.Add('g', animNameChargeLoop);
		animationNames.Add('h', animNameChargeStrike);
		animationNames.Add('j', animNameVigor);
		
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
}
