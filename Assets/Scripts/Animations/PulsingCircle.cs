using UnityEngine;

public class PulsingCircle : MonoBehaviour
{
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private float speed = 2f;
    private Vector3 scaleDirection = new Vector3(1, 0, 1);

    void Update()
    {
        float scale = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(Time.time * speed, 1));
        transform.localScale = (scaleDirection * scale) + Vector3.up;

    }
}
