using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

public class NetworkCharacterAnimator : MonoBehaviour 
{
	protected Transform torsoTransform;
	public Transform TorsoTransform { set { torsoTransform = value; } }
	
	[HideInInspector]
	public Animation myAnimation;
	
	protected string animNameIdle = "idle";
	protected string animNameRunning = "running";
	protected string animNameRunningBackwards = "runningBackwards";
	protected string animNameStrafeRight = "strafeRight";
	protected string animNameStrafeLeft = "strafeLeft";
	protected string animNameDying = "dying";
	protected string animNameJumpLoad = "jumpLoad";
	protected string animNameJumpLoadIdle = "jumpLoadIdle";
	protected string animNameJumpUp = "jumpUp";
	protected string animNameJumpDown = "jumpDown";
	protected string animNameJumpLanding = "jumpLanding";
	protected string animNameJumpLandingIdle = "jumpLandingIdle";
	protected string animNameDash = "dash";
	
	protected AnimationState animIdle;
	protected AnimationState animRunning;
	protected AnimationState animRunningBackwards;
	protected AnimationState animStrafeRight;
	protected AnimationState animStrafeLeft;
	protected AnimationState animDying;
	protected AnimationState animJumpLoad;
	protected AnimationState animJumpLoadIdle;
	protected AnimationState animJumpUp;
	protected AnimationState animJumpDown;
	protected AnimationState animJumpLanding;
	protected AnimationState animJumpLandingIdle;
	protected AnimationState animDash;
	
	protected string animationsPlaying = "";
	protected bool landing;
	protected bool animationsPrepared;
	
	public virtual void PrepareAnimations ()
	{
		//0 = Idle
		//1 = Running
		//2 = RunningBackwards
		//3 = StrafeRight
		//4 = StrafeLeft
		//5 = Dying
		//6 = JumpLoad
		//7 = JumpUp
		//8 = JumpDown
		//9 = JumpLanding
		//t = JumpLandingIdle
		//r = JumpLoadIdle
		//y = Dash
		
		animIdle = myAnimation[animNameIdle];
		animRunning = myAnimation[animNameRunning];
		animRunningBackwards = myAnimation[animNameRunningBackwards];
		animStrafeRight = myAnimation[animNameStrafeRight];
		animStrafeLeft = myAnimation[animNameStrafeLeft];
		animDying = myAnimation[animNameDying];
		animJumpLoad = myAnimation[animNameJumpLoad];
		animJumpLoadIdle = myAnimation[animNameJumpLoadIdle];
		animJumpUp = myAnimation[animNameJumpUp];
		animJumpDown = myAnimation[animNameJumpDown];
		animJumpLanding = myAnimation[animNameJumpLanding];
		animJumpLandingIdle = myAnimation[animNameJumpLandingIdle];
		animDash = myAnimation[animNameDash];
		
		animIdle.wrapMode = WrapMode.Loop;
        animIdle.blendMode = AnimationBlendMode.Blend;

        animRunning.wrapMode = WrapMode.Loop;
        animRunning.blendMode = AnimationBlendMode.Blend;
		animRunning.speed = 2;
		
		animRunningBackwards.wrapMode = WrapMode.Loop;
        animRunningBackwards.blendMode = AnimationBlendMode.Blend;
		animRunningBackwards.speed = 2;
		
		animStrafeRight.wrapMode = WrapMode.Loop;
        animStrafeRight.blendMode = AnimationBlendMode.Blend;
		animStrafeRight.speed = 2;
		
		animStrafeLeft.wrapMode = WrapMode.Loop;
        animStrafeLeft.blendMode = AnimationBlendMode.Blend;
		animStrafeLeft.speed = -2;
		
		animJumpLoad.wrapMode = WrapMode.Clamp;
        animJumpLoad.blendMode = AnimationBlendMode.Blend;
		animJumpLoad.layer = 2;
		animJumpLoad.AddMixingTransform(torsoTransform);
		
		animJumpLoadIdle.wrapMode = WrapMode.Clamp;
        animJumpLoadIdle.blendMode = AnimationBlendMode.Blend;
		animJumpLoadIdle.layer = 2;
		
		animJumpUp.wrapMode = WrapMode.Loop;
        animJumpUp.blendMode = AnimationBlendMode.Blend;
		animJumpUp.layer = 3;
		
		animJumpDown.wrapMode = WrapMode.Loop;
        animJumpDown.blendMode = AnimationBlendMode.Blend;
		animJumpDown.layer = 3;
		
		animJumpLanding.wrapMode = WrapMode.ClampForever;
        animJumpLanding.blendMode = AnimationBlendMode.Blend;
		animJumpLanding.layer = 4;
		animJumpLanding.speed = 2;
		animJumpLanding.AddMixingTransform(torsoTransform);
		
		animJumpLandingIdle.wrapMode = WrapMode.ClampForever;
        animJumpLandingIdle.blendMode = AnimationBlendMode.Blend;
		animJumpLandingIdle.layer = 4;
		animJumpLandingIdle.speed = 1;
		
		animDash.wrapMode = WrapMode.Loop;
        animDash.blendMode = AnimationBlendMode.Blend;
		animDash.layer = 5;
		
		animDying.wrapMode = WrapMode.ClampForever;
        animDying.blendMode = AnimationBlendMode.Blend;
		animDying.layer = 10;
		
		animationsPrepared = true;
	}
	
	public void AnimationReceiver(string animString)
	{
		animationsPlaying = animString;
	}
	
	protected virtual void Update()
	{
		if (animationsPrepared)
		{
			if (animationsPlaying != null && animationsPlaying != "")
			{
				
				if (animationsPlaying.Contains("0"))
					myAnimation.CrossFade(animNameIdle, 0.3f, PlayMode.StopSameLayer);
				
				if (animationsPlaying.Contains("1"))
					myAnimation.CrossFade(animNameRunning, 0.3f, PlayMode.StopSameLayer); //Running
				
				if (animationsPlaying.Contains("2"))
					myAnimation.CrossFade(animNameRunningBackwards, 0.3f, PlayMode.StopSameLayer);
				
				if (animationsPlaying.Contains("3"))
					myAnimation.CrossFade(animNameStrafeRight, 0.3f, PlayMode.StopSameLayer);
				
				if (animationsPlaying.Contains("4"))
					myAnimation.CrossFade(animNameStrafeLeft, 0.3f, PlayMode.StopSameLayer);
				
				if (animationsPlaying.Contains("5"))
					myAnimation.CrossFade(animNameDying, 0.1f, PlayMode.StopAll);
				else
				{
					if (myAnimation.IsPlaying(animNameDying))
						myAnimation.Stop(animNameDying);
				}
				
				if (animationsPlaying.Contains("6"))
					myAnimation.Blend(animNameJumpLoad, 1f, animJumpLoad.length * 0.1f);
				
				if (animationsPlaying.Contains("7"))
					myAnimation.Blend(animNameJumpUp, 1f, 0.1f);
				else
					myAnimation.Blend(animNameJumpUp, 0f, 0.1f);
				
				if (animationsPlaying.Contains("8"))
					myAnimation.Blend(animNameJumpDown, 1f, 0.1f);
				else
					myAnimation.Blend(animNameJumpDown, 0f, 0.1f);
				
				if (animationsPlaying.Contains("9"))
				{
					if (!landing)
					{
						StartCoroutine(PlayLanding());
					}
				}
				
				if (animationsPlaying.Contains("t"))
				{
					if (!landing)
					{
						StartCoroutine(PlayLandingIdle());
					}
				}
				
				if (animationsPlaying.Contains("r"))
					myAnimation.Blend(animNameJumpLoadIdle, 1f, animJumpLoadIdle.length);
				
				if (animationsPlaying.Contains("y"))
					myAnimation.Blend(animNameDash, 1f, animDash.length * 0.07f);
				else
					myAnimation.Blend(animNameDash, 0f, animDash.length * 0.07f);
			}
			else
			{
				myAnimation.CrossFade(animNameIdle, 0.3f, PlayMode.StopSameLayer);
			}
		}
	}
	
	public IEnumerator PlayLanding()
	{
		landing = true;
		animJumpLanding.time = 0;
		myAnimation.Play(animNameJumpLanding);
		yield return new WaitForSeconds(animJumpLanding.length * 0.6f);
		myAnimation.Blend(animNameJumpLanding, 0f, animJumpLanding.length * 0.4f);
		yield return new WaitForSeconds(animJumpLanding.length * 0.4f);
		landing = false;
	}
	
	public IEnumerator PlayLandingIdle()
	{
		landing = true;
		animJumpLandingIdle.time = 0;
		myAnimation.Play(animNameJumpLandingIdle);
		yield return new WaitForSeconds(animJumpLandingIdle.length * 0.6f);
		myAnimation.Blend(animNameJumpLandingIdle, 0f, animJumpLandingIdle.length * 0.4f);
		yield return new WaitForSeconds(animJumpLandingIdle.length * 0.4f);
		landing = false;
	}
}
