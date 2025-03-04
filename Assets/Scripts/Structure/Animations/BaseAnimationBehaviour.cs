using UnityEngine;

public abstract class BaseAnimationBehaviour : MonoBehaviour
{

    protected GameObject animationTarget;

    protected bool isPlayingAnimation;


    public virtual void AnimationStart() { }

    public virtual void AnimationEnd() { }


    public virtual void SetTarget(GameObject animationTarget)
    {
        this.animationTarget = animationTarget;

    }

    public void Play()
    {
        if (this == null)
            return;

        if (animationTarget == null)
            animationTarget = this.gameObject;

        isPlayingAnimation = true;

        AnimationStart();

    }

    public void Stop()
    {
        if (this == null)
            return;

        if (animationTarget == null)
            animationTarget = this.gameObject;

        isPlayingAnimation = false;

        AnimationEnd();

    }

    public virtual void Pause() { }


}