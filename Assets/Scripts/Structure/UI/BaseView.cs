
using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class BaseView : MonoBehaviour
{

    CanvasGroup canvasGroup;

    protected bool isClosed;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

    }

    public virtual void Show()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        this.gameObject.name = this.gameObject.name.Replace("** ", "");
        this.gameObject.name = this.gameObject.name.Replace(" (Hidden) **", "");

    }

    public virtual void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
        this.gameObject.name = $"** {this.gameObject.name} (Hidden) **";

    }

    public virtual void Close()
    {
        if (isClosed)
            return;

        isClosed = true;

        if (this != null && this.gameObject != null)
        {
            Destroy(this.gameObject);
        }

    }

}