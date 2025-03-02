using UnityEngine;

public class VolleyballShadow : MonoBehaviour
{

    [SerializeField] GameObject PulsingCircle;
    [SerializeField] LayerMask GroundLayerMask;
    GameObject circleInstance;

    void Start()
    {
        this.circleInstance = Instantiate(PulsingCircle);
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, 100, GroundLayerMask))
        {
            this.circleInstance.transform.position = hit.point + (Vector3.up * 0.1f);

        }

    }

    private void OnDestroy()
    {
        Destroy(this.circleInstance);
    }

}