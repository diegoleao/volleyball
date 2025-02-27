
using UnityEngine;

public class VolleyBall : MonoBehaviour
{

    [SerializeField] GameObject PulsingCircle;
    GameObject circleInstance;
    private void Start()
    {
        this.circleInstance = Instantiate(PulsingCircle);
    }

    private void Update()
    {
        if(Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, 100))
        {
            this.circleInstance.transform.position = hit.point + (Vector3.up*0.1f);

        }

    }

}