using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

public class CCAnimator : MonoBehaviour 
{
	public List<char> animationList;
	public Dictionary<char, string> animationNames;
	
	private CCMover charMover;
	public CCMover CharMover { set { charMover = value; } }
	
	protected Transform torsoTransform;
	public Transform TorsoTransform { set { torsoTransform = value; } }
	
	//Cacheando o Motor
	protected CharacterMotor charMotor; 
	public CharacterMotor CharMotor { set { charMotor = value; } }
	
	[HideInInspector]
	public Animation myAnimation;
	
	private NetworkAnimationSender animSender;
	private StatusHandler statusHandler;
	private bool lastFalling;
	
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
	
	protected virtual void Start ()
	{
		if (SmartFoxConnection.hasConnection)
			animSender = gameObject.AddComponent<NetworkAnimationSender>();
		statusHandler = GetComponent<StatusHandler>();
		myAnimation = GetComponentInChildren<Animation>();
		
		animationList = new List<char>();
		animationList.Add('0'); //Idle
		animationList.Add('1'); //Running
		animationList.Add('2'); //RunningBackwards
		animationList.Add('3'); //StrafeLeft
		animationList.Add('4'); //StrafeRight
		animationList.Add('5'); //Dying
		animationList.Add('6'); //JumpLoad
		animationList.Add('7'); //JumpUp
		animationList.Add('8'); //JumpDown
		animationList.Add('9'); //JumpLanding
		animationList.Add('t'); //JumpLandingIdle
		animationList.Add('r'); //JumpLoadIdle
		animationList.Add('y'); //Dash
		
		animationNames = new Dictionary<char, string>();
		animationNames.Add('0', animNameIdle);
		animationNames.Add('1', animNameRunning);
		animationNames.Add('2', animNameRunningBackwards);
		animationNames.Add('3', animNameStrafeRight);
		animationNames.Add('4', animNameStrafeLeft);
		animationNames.Add('5', animNameDying);
		animationNames.Add('6', animNameJumpLoad);
		animationNames.Add('7', animNameJumpUp);
		animationNames.Add('8', animNameJumpDown);
		animationNames.Add('9', animNameJumpLanding);
		animationNames.Add('t', animNameJumpLandingIdle);
		animationNames.Add('r', animNameJumpLoadIdle);
		animationNames.Add('y', animNameDash);
		
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
	}
	
	private void Update()
	{
		if (charMover.Falling)
		{
			if (charMotor.GetVelocity().y > 0)
			{
				myAnimation.Blend(animNameJumpUp, 1f, 0.1f);
				myAnimation.Blend(animNameJumpDown, 0f, 0.1f);
				
			}
			else
			{
				myAnimation.Blend(animNameJumpUp, 0f, 0.5f);
				myAnimation.Blend(animNameJumpDown, 1f, 0.5f);
			}
		}
		else
		{
			myAnimation.Blend(animNameJumpUp, 0f, 0.2f);
			myAnimation.Blend(animNameJumpDown, 0f, 0.2f);
		}
		if (lastFalling != charMover.Falling && !charMover.Falling)
		{
			if (!charMover.Moving)
				StartCoroutine(PlayLandingIdle());
			else
				StartCoroutine(PlayLanding());
		}
		lastFalling = charMover.Falling;
		if (charMover.Moving)
		{
			//if (statusHandler.slowIntensity > 0.5f)
			{
				switch(charMover.FacingDirection)
				{
					case 0:
					myAnimation.CrossFade(animNameRunning, 0.3f, PlayMode.StopSameLayer); //Running
					break;
					case 1:
					myAnimation.CrossFade(animNameStrafeRight, 0.3f, PlayMode.StopSameLayer); //Right
					break;
					case 2:
					myAnimation.CrossFade(animNameRunningBackwards, 0.3f, PlayMode.StopSameLayer); //Right
					break;
					case 3:
					myAnimation.CrossFade(animNameStrafeLeft, 0.3f, PlayMode.StopSameLayer); //Right
					break;
				}
			}
			/*
			else
			{
				switch(charMover.FacingDirection)
				{
					case 0:
					myAnimation.CrossFade(animNameWalking, 0.3f, PlayMode.StopSameLayer); //Running
					break;
					case 1:
					myAnimation.CrossFade(animNameWalkingStrafeRight, 0.3f, PlayMode.StopSameLayer); //Right
					break;
					case 2:
					myAnimation.CrossFade(animNameWalkingBackwards, 0.3f, PlayMode.StopSameLayer); //Right
					break;
					case 3:
					myAnimation.CrossFade(animNameWalkingStrafeLeft, 0.3f, PlayMode.StopSameLayer); //Right
					break;
				}
			}*/
		}
		else
		{
			myAnimation.CrossFade(animNameIdle, 0.3f, PlayMode.StopSameLayer); //Idle
		}
	}
	
	public void PlayDeath()
	{
		myAnimation.CrossFade(animNameDying, 0f, PlayMode.StopAll); //Dying
	}
	
	public void RespawnPlayer()
	{
		myAnimation.Stop(animNameDying);
		myAnimation[animNameDying].weight = 0;
	}
	
	public IEnumerator PlayDash(float waitTime)
	{
		myAnimation.Blend(animNameDash, 1f, animDash.length * 0.07f);
		yield return new WaitForSeconds(waitTime);
		myAnimation.Blend(animNameDash, 0f, animDash.length * 0.07f);
	}
	
	public void PlayJump()
	{
		if (charMover.Moving)
		{
			myAnimation.Blend(animNameJumpLoad, 1f, animJumpLoad.length * 0.1f);
			//StartCoroutine(charMover.Jump(animJumpLoad.length * 0.9f));
		}
		else
		{
			myAnimation.Blend(animNameJumpLoadIdle, 1f, animJumpLoadIdle.length);
			//StartCoroutine(charMover.Jump(animJumpLoadIdle.length * 1f));
		}
	}
	
	public IEnumerator PlayLanding()
	{
		animJumpLanding.time = 0;//animJumpLanding.length * 0.1f;
		myAnimation.Play(animNameJumpLanding);
		// = animJumpLanding.length * 0.3f;
		//myAnimation.Blend(animNameJumpLanding, 1f, animJumpLanding.length * 0.1f);
		yield return new WaitForSeconds(animJumpLanding.length * 0.6f);
		myAnimation.Blend(animNameJumpLanding, 0f, animJumpLanding.length * 0.4f);
	}
	
	public IEnumerator PlayLandingIdle()
	{
		animJumpLandingIdle.time = 0;//animJumpLanding.length * 0.1f;
		myAnimation.Play(animNameJumpLandingIdle);
		// = animJumpLanding.length * 0.3f;
		//myAnimation.Blend(animNameJumpLanding, 1f, animJumpLanding.length * 0.1f);
		yield return new WaitForSeconds(animJumpLandingIdle.length * 0.6f);
		myAnimation.Blend(animNameJumpLandingIdle, 0f, animJumpLandingIdle.length * 0.4f);
	}
}
