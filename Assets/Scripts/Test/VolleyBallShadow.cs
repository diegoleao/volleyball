using UnityEngine;

public class VolleyballShadow : MonoBehaviour
{

    [SerializeField] GameObject PulsingCircle;
    [SerializeField] LayerMask GroundLayerMask;
    GameObject circleInstance;

    private void Start()
    {
        this.circleInstance = Instantiate(PulsingCircle, this.transform);
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, 100, GroundLayerMask))
        {
            this.circleInstance.transform.position = hit.point + (Vector3.up * 0.1f);

        }

    }

}