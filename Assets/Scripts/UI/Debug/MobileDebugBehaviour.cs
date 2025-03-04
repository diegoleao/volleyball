using UnityEngine;

public class MobileDebugBehaviour : MonoBehaviour
{
    [SerializeField] bool autoDisable;
    [SerializeField] bool dontDestroyOnLoad;

    void Start()
    {
        if(autoDisable)
            this.gameObject.SetActive(false);

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(this);

    }

}