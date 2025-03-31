using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController1 : MonoBehaviour
{
    public Animator mAnimation;

    public void PlayAnimation(string parameter, bool isLoop)
    {
        //  mAnimation.Play(animationName);
        mAnimation.SetBool(parameter, isLoop);
    }
}
