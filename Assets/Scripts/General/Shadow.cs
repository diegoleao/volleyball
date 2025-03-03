using UnityEngine;

public class Shadow : MonoBehaviour
{

    [SerializeField] GameObject Circle;
    [SerializeField] LayerMask GroundLayerMask;
    GameObject circleInstance;

    void Start()
    {
        this.circleInstance = Instantiate(Circle);
    }

    void LateUpdate()
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