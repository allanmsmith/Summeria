using UnityEngine;
using System.Collections;

public class NetworkAnimationSender : MonoBehaviour
{
	private string animationsPlaying;
	private string lastAnimationsPlaying;
	private CCAnimator charAnimator;
	
	private void Start()
	{
		charAnimator = GetComponent<CCAnimator>();
	}
	
	private void LateUpdate()
	{
		animationsPlaying = "";
		for (int i = 0; i < charAnimator.animationList.Count; i++)
		{
			//if (charAnimator.myAnimation.IsPlaying(charAnimator.animationNames[charAnimator.animationList[i]]))
			if (charAnimator.myAnimation[charAnimator.animationNames[charAnimator.animationList[i]]].weight > 0)
			{
				animationsPlaying += charAnimator.animationList[i];
			}
		}
		if (lastAnimationsPlaying != animationsPlaying)
		{
			NMGame.Instance.SendAnimations(animationsPlaying);
		}
		lastAnimationsPlaying = animationsPlaying;
	}
}
