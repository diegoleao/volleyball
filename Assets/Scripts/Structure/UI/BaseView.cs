
using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class BaseView : MonoBehaviour
{

    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

    }

    public void Show()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        this.gameObject.name = this.gameObject.name.Replace("** ", "").Replace(" (Hidden) **", "");

    }

    public void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
        this.gameObject.name = $"** {this.gameObject.name} (Hidden) **";

    }

    public void Close()
    {
        if(this != null && this.gameObject != null)
        {
            Destroy(this.gameObject);
        }

    }

}