using UnityEngine;

public class PulsingAnimation : BaseAnimationViaUpdate
{
    [SerializeField] private float minScale = 0.35f;
    [SerializeField] private float maxScale = 0.45f;
    [SerializeField] private float speed = 2f;
    [SerializeField] Vector3 scaleDirection = new Vector3(1, 0, 1);

    private Vector3 originalScale;

    void Start()
    {
        Play();
        originalScale = transform.localScale;
    }

    public override void AnimationUpdate()
    {
        float scale = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(Time.time * speed, 1));
        transform.localScale = (scaleDirection * scale) + Vector3.up;

    }

}
