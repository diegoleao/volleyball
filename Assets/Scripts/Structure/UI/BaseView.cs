
using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class BaseView : MonoBehaviour
{

    CanvasGroup canvasGroup;

    protected bool isClosed;

    protected bool isVisible;

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
        isVisible = true;

    }

    public virtual void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
        if (isVisible)
        {
            this.gameObject.name = $"** {this.gameObject.name} (Hidden) **";
        }
        isVisible = false;

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