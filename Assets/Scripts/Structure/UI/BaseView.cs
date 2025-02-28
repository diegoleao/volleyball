
using UnityEngine;

public class BaseView : MonoBehaviour
{

    [Header("References")]
    [SerializeField] CanvasGroup CanvasGroup;

    public void Show()
    {
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        CanvasGroup.alpha = 1;
        this.gameObject.name = this.gameObject.name.Replace("** ", "").Replace(" (Hidden) **", "");

    }

    public void Hide()
    {
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        CanvasGroup.alpha = 0;
        this.gameObject.name = $"** {this.gameObject.name} (Hidden) **";

    }

}