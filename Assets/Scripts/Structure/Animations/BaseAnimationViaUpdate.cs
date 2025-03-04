
using System;
using UnityEngine;

public abstract class BaseAnimationViaUpdate : BaseAnimationBehaviour
{

    public abstract void AnimationUpdate();

    void Update()
    {
        if(isPlayingAnimation)
            AnimationUpdate();

    }

}