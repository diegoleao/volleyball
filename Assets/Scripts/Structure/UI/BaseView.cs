
using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class BaseView : MonoBehaviour
{

    CanvasGroup _canvasGroup;
    CanvasGroup CanvasGroup => _canvasGroup ??= GetComponent<CanvasGroup>();

    protected bool isClosed;
    protected bool isVisible;

    public virtual void Show()
    {
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        CanvasGroup.alpha = 1;
        this.gameObject.name = this.gameObject.name.Replace("** ", "");
        this.gameObject.name = this.gameObject.name.Replace(" (Hidden) **", "");
        isVisible = true;

    }

    public virtual void Hide()
    {
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        CanvasGroup.alpha = 0;
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