using Sirenix.OdinInspector;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseElement : MonoBehaviour
{

    public AppCanvas MyAppCanvas
    {
        get
        {
            return Provider.AppCanvas;
        }
    }

    protected CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
            return _canvasGroup;
        }
    }

    protected Canvas _canvas;
    protected Canvas UnityCanvas
    {
        get
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }
            return _canvas;
        }
    }


    [FoldoutGroup("Runtime Info", Order = -1, Expanded = true)]
    [ShowInInspector]
    [ReadOnly]
    public BaseView ParentScreen { get; private set; }


    [FoldoutGroup("Fading (Optional)")]
    public float FadeInTime = 0.25f;

    [FoldoutGroup("Fading (Optional)")]
    public float FadeOutTime = 0.25f;


    [FoldoutGroup("Events")]
    public UnityEvent OnFadeInFinishedEvent;

    [FoldoutGroup("Events")]
    public UnityEvent OnFadeOutFinishedEvent;


    // Methods


    [Title("Debug Buttons", HorizontalLine = false)]
    [Button(ButtonSizes.Medium)]
    public virtual void Show()
    {
        BasicTurnElementVisible();
    }

    public void Show(BaseView parentScreen)
    {
        SetParent(parentScreen);
        this.Show();
    }

    public BaseElement SetParent(BaseView parentScreen)
    {
        this.ParentScreen = parentScreen;
        return this;

    }

    public BaseElement SetParent<T>() where T : BaseView
    {
        this.ParentScreen = MyAppCanvas.GetView<T>();
        return this;

    }

    protected virtual void FadeIn(float time)
    {
        UnityCanvas.enabled = true;
        //CanvasGroup.DOFade(1, time);
        Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
        {
            OnFadeInFinishedEvent.Invoke();
        }).AddTo(this);

    }

    protected virtual void FadeOut(float time)
    {
        //CanvasGroup.DOFade(0, time);
        Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
        {
            OnFadeOutFinishedEvent.Invoke();
        }).AddTo(this);

    }

    public virtual void Close()
    {
        if (this) Destroy(this.gameObject);

    }

    [Button(ButtonSizes.Small)]
    public virtual UnityEvent ShowFading()
    {

        Observable.Timer(TimeSpan.FromSeconds(FadeInTime + 0.1f)).Subscribe(_ =>
        {
            this.Show();

        }).AddTo(this);

        FadeIn(FadeInTime);

        return OnFadeInFinishedEvent;

    }

    [Button(ButtonSizes.Small)]
    public virtual UnityEvent CloseFading()
    {

        Observable.Timer(TimeSpan.FromSeconds(FadeInTime + 0.1f)).Subscribe(_ =>
        {
            if (this != null)
            {
                this.Close();
            }
        }).AddTo(this);

        if (this != null)
        {
            FadeOut(FadeInTime);
        }

        return OnFadeOutFinishedEvent;

    }

    [Button(ButtonSizes.Small)]
    public virtual UnityEvent HideFading()
    {

        Observable.Timer(TimeSpan.FromSeconds(FadeInTime + 0.1f)).Subscribe(_ =>
        {
            this.Hide();
        }).AddTo(this);

        FadeOut(FadeInTime);

        return OnFadeOutFinishedEvent;

    }

    public virtual void Hide()
    {
        this.BasicHideElement();

    }

    protected void BasicHideElement()
    {
        SetEmptyCanvasGroup();
        this.name = this.name.Replace("Clone", "*");
    }

    protected void SetEmptyCanvasGroup()
    {
        CanvasGroup.alpha = 0;
        CanvasGroup.blocksRaycasts = false;
        CanvasGroup.interactable = false;
    }

    protected void BasicTurnElementVisible()
    {
        CanvasGroup.alpha = 1;
        CanvasGroup.blocksRaycasts = true;
        CanvasGroup.interactable = true;
        this.gameObject.SetActive(true);
    }


}
