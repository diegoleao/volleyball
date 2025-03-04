using UnityEngine;

public class GroundProjection : MonoBehaviour
{

    [SerializeField] VisualIndicator GraphicsPrefab;

    [SerializeField] LayerMask GroundLayerMask;

    private VisualIndicator graphicsInstance;

    void Start()
    {
        this.graphicsInstance = Instantiate(GraphicsPrefab);

    }

    void LateUpdate()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, 100, GroundLayerMask))
        {
            this.graphicsInstance.transform.position = hit.point + (Vector3.up * 0.1f);

        }

    }

    void OnDestroy()
    {
        if(this.graphicsInstance != null)
        {
            Destroy(this.graphicsInstance.gameObject);
        }

    }

    public void PlayAnimation()
    {
        if (this.graphicsInstance.IsAnimated)
        {
            graphicsInstance.Animation.Play();
        }
        
    }

    public void StopAnimation()
    {
        if (this.graphicsInstance.IsAnimated)
        {
            graphicsInstance.Animation.Stop();
        }

    }

}