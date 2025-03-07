using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseView : BaseElement
{

#if UNITY_EDITOR
    /// <summary>
    /// This should never be used in production, because it ignores the lifecicle of the Screen, and
    /// can happen before the screen is ready.
    /// </summary>
    public bool ShowOnStartForDebug = false;

    protected IDisposable showOnStartDebug;

#endif

    [Header("Base View")]
    public int CanvasOrder;
    public bool BackEnabled;

    //============================================================== Public

    [FoldoutGroup("Runtime Info", Order = -1, Expanded = false)]
    [ShowInInspector]
    [ReadOnly]
    public bool IsInitialized { get; private set; }

    [FoldoutGroup("Runtime Info", Order = -1, Expanded = false)]
    [ShowInInspector]
    [ReadOnly]
    public bool IsExecuting { get; private set; }

    [FoldoutGroup("Runtime Info", Order = -1, Expanded = false)]
    [ShowInInspector]
    [ReadOnly]
    private List<BaseView> StackedScreens = new List<BaseView>();

    [FoldoutGroup("Runtime Info", Order = -1, Expanded = false)]
    [ShowInInspector]
    [ReadOnly]
    public List<GameObject> ChildrenObjects { get; set; } = new List<GameObject>();

    [FoldoutGroup("Runtime Info", Order = -1, Expanded = false)]
    [ReadOnly][ShowInInspector] public AppContext ViewContext { get; private set; }

    public bool HasStack
    {
        get
        {
            return StackedScreens.Count > 0;
        }
    }

    [FoldoutGroup("Events", Order = -2, Expanded = false)]
    public UnityEvent OnCloseEvent;

    [FoldoutGroup("Events", Order = -2, Expanded = false)]
    public UnityEvent OnFirstShowEvent;

    private List<IDisposable> disposables = new List<IDisposable>();

    //============================================================== Private

    private AppCanvas _appCanvas;
    public AppCanvas AppCanvas
    {
        get
        {
            if (_appCanvas == null)
            {
                Debug.LogWarning($"Atention: The View \"{this.GetType().Name}\" didn't have an AppCanvas by the time it was requested. " +
                                    "Usually, this means it was not created using the AppCanvas (which is a problem), but if it was, disregard this message.");
                _appCanvas = Provider.AppCanvas;
            }
            return _appCanvas;
        }
        set
        {
            _appCanvas = value;
        }
    }

#if UNITY_ANDROID
    private IDisposable androidBackButtonEvent;
#endif

    public string Id { get; private set; }

    //============================================================== Std. Methods

    public BaseView()
    {
        Id = Guid.NewGuid().ToString();
    }

    private void Start()
    {

#if UNITY_EDITOR
        if (ShowOnStartForDebug)
        {
            showOnStartDebug
                = Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
                {
                    this.Show();
                });
        }
#endif
#if UNITY_ANDROID

        if (this.BackEnabled)
        {
            androidBackButtonEvent = Observable.EveryUpdate()
                                            .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                                            .Subscribe(_ =>
                                            {
                                                if (this.IsExecuting && this.IsInitialized && this.StackedScreens.Count == 0 && this != null)
                                                {
                                                    GoBack();
                                                }
                                            });
        }

#endif
        OnStart();
    }

#if UNITY_ANDROID
    public void DisposeAndroidBackButtonEvent()
    {
        if (androidBackButtonEvent != null)
            Debug.Log($"Disposing androidBackButtonEvent: {androidBackButtonEvent}");

        androidBackButtonEvent?.Dispose();
    }
#endif

    protected virtual void OnStart() { }

    private void OnDestroy()
    {
#if UNITY_ANDROID
        if(androidBackButtonEvent != null) androidBackButtonEvent.Dispose();
#endif
#if UNITY_EDITOR
        if (showOnStartDebug != null) showOnStartDebug.Dispose();
#endif

        disposables.ForEach(t =>
        {
            if (t != null) t.Dispose();
        });

    }

    public void SetViewContext(AppContext appContext)
    {
        this.ViewContext = appContext;

    }

    /// <summary>
    /// Do not ever use Update() on children, use CustomUpdate instead. This way, code execution stops if View is Hidden.
    /// </summary>
    void Update()
    {
        if (IsExecuting)
        {
            CustomUpdate();
        }

    }

    protected virtual void CustomUpdate() { }

    //============================================================== Methods


    protected T GetOrCreate<T>(CanvasOverlay canvasOverlay = CanvasOverlay.None) where T : BaseView
    {
        return AppCanvas.GetOrCreate<T>();

    }

    protected BaseView GetOrCreate(Type desiredType, CanvasOverlay canvasOverlay = CanvasOverlay.None)
    {
        return AppCanvas.GetOrCreate(desiredType, canvasOverlay);

    }
    protected T GetOrCreatePopup<T>() where T : BaseView
    {
        return (T)GetOrCreatePopup(typeof(T));

    }

    protected BaseView GetOrCreatePopup(Type desiredType)
    {
        return AppCanvas.GetOrCreate(desiredType, CanvasOverlay.Popup);

    }

    protected T GetView<T>() where T : BaseView
    {
        return (T)AppCanvas.GetView(typeof(T));

    }

    protected BaseView GetView(Type desiredType)
    {
        return AppCanvas.GetView(desiredType);

    }

    public BaseView SetCanvasSortingOrder(int sortingOrder)
    {
        CanvasOrder = sortingOrder;
        UnityCanvas.sortingOrder = sortingOrder;

        return this;
    }

    public T TransitionTo<T>(bool showImmediately = true) where T : BaseView
    {
        return (T)TransitionTo(typeof(T), showImmediately);

    }

    public BaseView TransitionTo(Type desiredType, bool showImmediately = true)
    {
        var newScreen = AppCanvas.GetOrCreate(desiredType);

        if (newScreen != null)
        {
            this.Close();
        }

        if (newScreen.CanvasOrder < this.CanvasOrder)
        {
            newScreen.SetCanvasSortingOrder(this.CanvasOrder);
        }
        else
        {
            newScreen.SetCanvasSortingOrder(newScreen.CanvasOrder);
        }

        if (showImmediately) newScreen.Show();

        return newScreen;

    }

    public BaseView TransitionToFading<T>(UnityAction OnFadeOut = null, UnityAction OnFadeIn = null) where T : BaseView
    {
        this.OnFadeOutFinishedEvent.AddListener(() =>
        {
            OnFadeOut?.Invoke();
        });
        this.CloseFading();

        var newView = (BaseView)GetOrCreate<T>();
        newView.OnFadeInFinishedEvent.AddListener(() =>
        {
            OnFadeIn?.Invoke();
        });

        newView.ShowFading();

        return newView;
    }

    public T StackView<T>(bool showImmediately = false, CanvasOverlay canvasOverlay = CanvasOverlay.None) where T : BaseView
    {
        return (T)StackView(typeof(T), showImmediately, canvasOverlay);

    }

    public BaseView StackView(Type viewType, bool showImmediately = false, CanvasOverlay canvasOverlay = CanvasOverlay.None)
    {

        if (StackedScreensContainsType(viewType))
        {
            Debug.LogWarning($"While trying to stack view \"{viewType.Name}\" a view of the same type was found " +
                                "at the Stack, returning that view instead...");
            return GetStackedScreenOfType(viewType);
        }

        BaseView newScreen = null;

        try
        {

            newScreen = AppCanvas.GetView(viewType);

            if (newScreen != null)
            {
                Debug.LogError($"A request to stack view \"{viewType.Name}\" was attempted, but " +
                                    "there was another screen with the same type already created...");
                return newScreen;
            }

            newScreen = AppCanvas.GetOrCreate(viewType, canvasOverlay);

            if (newScreen == null)
            {
                Debug.LogError($"Failed to Stack View: {viewType.Name}, please check your ViewPrefabs in AppCanvas.");
                return null;
            }

            newScreen.SetEmptyCanvasGroup();

            SetCanvasOrder(newScreen);

            newScreen.SetParent(this);

            StackedScreens.Add(newScreen);

            if (!this.IsExecuting)
            {
                newScreen.Hide();
                return newScreen;
            }

            if (showImmediately) newScreen.Show();

        }
        catch (Exception exc)
        {
            LogErrorMessage(exc, "Failed to Stack View: " + viewType.Name);
            if (newScreen)
            {
                StackedScreens.Remove(newScreen);
                Destroy(newScreen.gameObject);
            }
            this.Show();

        }

        return newScreen;
    }

    private void SetCanvasOrder(BaseView newScreen)
    {
        if ((this.CanvasOrder + 1) > newScreen.CanvasOrder)
        {
            newScreen.SetCanvasSortingOrder(this.CanvasOrder + 1);

        }
        else
        {
            newScreen.SetCanvasSortingOrder(newScreen.CanvasOrder);

        }

        if (this.StackedScreens.Where(t => t != null).Select(t => t.CanvasOrder).Contains(newScreen.CanvasOrder))
        {
            newScreen.CanvasOrder
                = this.StackedScreens.Where(t => t != null)
                                        .Select(t => t.CanvasOrder)
                                        .OrderByDescending(t => t)
                                        .First() + 1;
            newScreen.SetCanvasSortingOrder(newScreen.CanvasOrder);

        }

    }

    private BaseView GetStackedScreenOfType(Type viewType)
    {
        return StackedScreens.Find(t => t.GetType() == viewType);
    }

    private bool StackedScreensContainsType(Type viewType)
    {
        return StackedScreens.Any(t => t != null && t.GetType() == viewType);
    }

    public T StackPopup<T>(bool showImmediately = false) where T : BaseView
    {
        return (T)StackView(typeof(T), showImmediately, CanvasOverlay.Popup);

    }

    public BaseView StackPopup(Type viewType, bool showImmediately = false)
    {
        return StackView(viewType, showImmediately, CanvasOverlay.Popup);

    }

    /// <summary>
    /// Unstack this view, and show its parent view.
    /// Can be used with Buttons.
    /// </summary>
    [Button]
    public virtual void GoBack()
    {
        if (this.ParentScreen != null && BackEnabled)
        {
            Debug.Log($"{this.name}, MY PARENT: {this.ParentScreen}");
            UnstackThisView(true);
        }

    }

    /// <summary>
    /// Hide all stacked views, from this view "Forward". Can be used from a Button.
    /// </summary>
    public void HideStack()
    {
        for (int i = StackedScreens.Count - 1; (i >= 0); i--)
        {
            if (StackedScreens[i]) { StackedScreens[i].Hide(true); }
        }

    }

    /// <summary>
    /// Close all stacked views, from this view "Forward". Do not destroy parent view.
    /// </summary>
    public void CloseStackedViews()
    {
        if (this == null || StackedScreens == null || StackedScreens.Count == 0) { return; }

        for (int i = StackedScreens.Count - 1; (i >= 0); i--)
        {
            if ((i < StackedScreens.Count) && StackedScreens[i]) { StackedScreens[i].UnstackThisView(); }
        }

        StackedScreens.Clear();

    }

    /// <summary>
    /// Unstack all views, from this view "Backwards". Do not destroy parent view.
    /// Version for use with Buttons.
    /// </summary>
    public void UnstackAllBackwards()
    {
        UnstackViewsBackwards();

    }

    /// <summary>
    /// Unstack all views, from this view "Backwards". Do not destroy parent view.
    /// </summary>
    /// <returns>Parent view</returns>
    public BaseView UnstackViewsBackwards()
    {
        return AppCanvas.UnstackBackwardsFrom(this);
    }


    /// <summary>
    /// Close all stacks (forwards and backwards), not including the parent screen.
    /// Version for use with Buttons.
    /// </summary>
    public void UnstackAll()
    {
        CloseStacksBidirectionallyUntilParent();

    }

    /// <summary>
    /// Unstack this view and returns its parent view.
    /// </summary>
    /// <param name="showPreviousScreen">If "true", the parent view receives a "Show()" call after unstacking this view.</param>
    /// <returns>The parent view</returns>
    public BaseView UnstackThisView(bool showPreviousScreen = false)
    {

        this.Close();

        if (ParentScreen != null)
        {
            if (showPreviousScreen) ParentScreen.Show();
            return ParentScreen;
        }

        return null;


    }

    /// <summary>
    /// Close all stacks (forwards and backwards), NOT including the parent screen.
    /// </summary>
    /// <returns>The parent screen</returns>
    protected BaseView CloseStacksBidirectionallyUntilParent()
    {
        CloseStackedViews();
        var parentScreen = AppCanvas.UnstackBackwardsFrom(this);
        return parentScreen;
    }

    protected T UnstackAllAndTransitionTo<T>() where T : BaseView
    {
        return AppCanvas.UnstackAllAndTransitionTo<T>(this);
    }

    protected BaseView UnstackAllAndTransitionTo(Type newScreenType)
    {
        return AppCanvas.UnstackAllAndTransitionTo(this, newScreenType);
    }

    [Button(ButtonSizes.Medium)]
    public override void Close()
    {
        try
        {
            if (ParentScreen != null)
            {
                ParentScreen.StackedScreens.RemoveAll(t => t != null && t.Id == this.Id);
                ParentScreen.StackedScreens.RemoveAll(t => t != null && t.GetType() == this.GetType());
            }

            CloseStackedViews();

        }
        catch (Exception exc) { LogErrorMessage(exc, $"Failed to close stacked views of screen: {this.GetType().Name}"); }

        AppCanvas.DestroyView(this);

        try
        {
            OnClose();
            OnCloseEvent.Invoke();
        }
        catch (Exception exc) { LogErrorMessage(exc, "Failed OnClose()"); }

    }

    public override void Show()
    {
        base.Show();

        if (this == null)
        {
            //View was destroyed before it could be shown (during an animation for example). Aborting silently.
            return;
        }

        if (this.ParentScreen != null && !this.ParentScreen.IsExecuting)
        {
            return;
        }

        if (!IsInitialized)
        {
            ShowOrUnhide(true);
            try
            {
                OnFirstExibition();
                OnFirstShowEvent.Invoke();
                IsInitialized = true;
            }
            catch (Exception exc) { LogErrorMessage(exc, "Failed OnFirstExibition()"); }
        }
        else
        {
            ShowOrUnhide(false);
        }

        if (StackedScreens.Count > 0)
        {
            ShowStackTop(false);
        }

    }

    public BaseView ShowAnd()
    {
        this.Show();
        return this;

    }

    private void HideNextFrameIfNeeded(bool parentIsHidden)
    {
        if (parentIsHidden)
        {
            Observable.NextFrame().Subscribe(_ =>
            {
                this.Hide();
            });
        }

    }

    private void ShowStackTop(bool parentIsHidden)
    {

        RemoveEmptyStack();

        if (StackedScreens.Count > 0)
        {
            BaseView currentView = StackedScreens[StackedScreens.Count - 1];
            currentView.Show();
            if (parentIsHidden) currentView.Hide();

        }

    }

    private void RemoveEmptyStack()
    {
        StackedScreens.RemoveAll(t => t == null);
    }

    private void ShowOrUnhide(bool firstTime)
    {

        bool wasExecuting = IsExecuting;
        try
        {
            TurnVisible();
        }
        catch (Exception exc) { LogErrorMessage(exc, "Failed TurnVisible()"); }

        if (!wasExecuting && !firstTime)
        {

            try
            {
                OnReexibition();
            }
            catch (Exception exc) { LogErrorMessage(exc, "Failed OnReexibition()"); }

        }

        try
        {
            OnShow();
        }
        catch (Exception exc) { LogErrorMessage(exc, "Failed OnShow()"); }

    }

    private void LogErrorMessage(Exception exc, string message = "")
    {
        Debug.LogError($"{message} at Screen \"{this.name}\": View Exception below for more information.");
        Debug.LogException(exc);
    }

    private static string GetCleanerStackTrace(Exception exc)
    {
        List<string> stackTrace = exc.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        for (int i = 0; i <= 4; i++)
        {
            if (stackTrace.Count > 0)
            {
                stackTrace.RemoveAt(0);
            }

        }

        return string.Join(Environment.NewLine, stackTrace.ToArray());
    }

    public void SetInit()
    {
        IsExecuting = false;
        IsInitialized = false;
        CanvasGroup.alpha = 0;
        CanvasGroup.blocksRaycasts = false;
        CanvasGroup.interactable = false;
        UnityCanvas.enabled = false;
        OnCreation();
    }

    private void TurnVisible()
    {
        IsExecuting = true;
        UnityCanvas.enabled = true;
        this.name = this.name.Replace("*", "Clone");
        BasicTurnElementVisible();
    }

    [Button(ButtonSizes.Medium)]
    public override void Hide()
    {
        Hide(true);
    }

    public void Hide(bool hideStack)
    {
        IsExecuting = false;
        UnityCanvas.enabled = false;
        BasicHideElement();

        if (hideStack)
        {
            HideStack();
        }
        try
        {
            OnHide();
        }
        catch (Exception exc) { LogErrorMessage(exc, "Failed OnHide()"); }

    }

    public void InjectAppCanvas(AppCanvas appCanvas)
    {
        this.AppCanvas = appCanvas;

    }

    protected virtual void OnCreation() { }

    protected abstract void OnFirstExibition();

    protected virtual void OnShow() { }

    protected virtual void OnReexibition() { }

    protected virtual void OnHide() { }

    protected virtual void OnClose() { }

    //DEBUG
    /// <summary>
    /// Never use this in production!
    /// </summary>
    [Button(ButtonSizes.Medium)]
    private void ReconstructForDebugging()
    {
        var initialized = this.IsInitialized;
        var executing = this.IsExecuting;
        this.Close();
        var newMe = AppCanvas.GetOrCreate(this.GetType());
        this.CopyMyStateTo(newMe, initialized, executing);
    }

    private void CopyMyStateTo(BaseView otherView, bool initialized, bool executing)
    {
        otherView.SetParent(this.ParentScreen);
        otherView.CanvasOrder = this.CanvasOrder;
        otherView.StackedScreens = this.StackedScreens;

        if (initialized) otherView.Show();

        if (!executing) otherView.Hide(false);

    }

    public override bool Equals(object other)
    {
        if (!other.GetType().IsAssignableFrom(typeof(BaseView)))
        {
            return false;
        }
        return Id.Equals(((BaseView)other).Id);
    }

    public override int GetHashCode()
    {
        var hashCode = 1545243542;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
        return hashCode;
    }

    /// <summary>
    /// Executes a function on Timer. Beware that "disposeTimerOnDestroy=true" will destroy your
    /// timer as soon as the view closes, so make sure you don't need it anymore at this time, or
    /// use "disposeTimerOnDestroy=false" to let it survive the view.
    /// </summary>
    /// <param name="parameterlessMethod">A function with no parameters and void return.</param>
    /// <param name="timeInSeconds">Time in which to call the function.</param>
    /// <param name="disposeTimerOnDestroy">If true, when the View closes, the timer will be destroyed,
    /// therefore, it won't call the function anymore. Make sure you don't close your view before all 
    /// timers are executed.</param>
    public void ExecuteOnTimer(UnityAction parameterlessMethod, float timeInSeconds, bool disposeTimerOnDestroy = false)
    {

        var simpleTimer =
            Observable.Timer(TimeSpan.FromSeconds(timeInSeconds))
                    .Subscribe(_ =>
                    {
                        parameterlessMethod.Invoke();
                    });

        if (disposeTimerOnDestroy)
        {
            disposables.Add(simpleTimer);
        }

    }

}

