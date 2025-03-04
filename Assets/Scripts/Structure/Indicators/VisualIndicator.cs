using Sirenix.OdinInspector;
using UnityEngine;

public class VisualIndicator : MonoBehaviour
{
    [SerializeField] bool isAnimated;
    public bool IsAnimated => this.isAnimated;

    [ShowIf(nameof(isAnimated))]
    [SerializeField] BaseAnimationBehaviour _animationReference;
    public BaseAnimationBehaviour Animation=> this._animationReference;

    void Start()
    {
        if (isAnimated)
        {
            Animation.SetTarget(this.gameObject);
        }

    }

}