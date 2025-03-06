using LunarConsolePlugin;
using Sirenix.OdinInspector;
using UnityEngine;

public class MobileDebugBehaviour : MonoBehaviour
{
    [SerializeField] bool autoDisable;
    [SerializeField] bool dontDestroyOnLoad;
    [SerializeField] LunarConsole lunarConsole;

    private int consoleActivationCount = 1;

    void Start()
    {
        if (autoDisable)
            SetActive(false);

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(this);

    }

    [Button]
    public void ToggleConsoleAfter10Touches()
    {
        consoleActivationCount++;
        if (consoleActivationCount > 10)
        {
            lunarConsole.gameObject.SetActive(!lunarConsole.gameObject.activeInHierarchy);
            consoleActivationCount = 1;
        }

    }

    public void SetActive(bool active)
    {
        lunarConsole.gameObject.SetActive(active);

    }

}