using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class AppCanvas : MonoBehaviour
{

    [ShowInInspector]
    [ReadOnly]
    private List<BaseView> loadedViews = new List<BaseView>();

    [Header("Execution Starts with")]
    [SerializeField] BaseView startupScreen;
    [SerializeField] List<BaseView> instancedViewTypes;

    [Header("Prefabs Handling")]
    [ListDrawerSettings(NumberOfItemsPerPage = 40)]
    [Searchable]
    public List<BaseView> ViewPrefabs;

    public bool StartedUp { get; private set; }

    public CanvasOverlay CurrentCanvasOverlay;

    public BaseViewEvent onCreateSuccess;

    //=============================================================================== Private

    [ShowInInspector]
    [ReadOnly]
    private Stack<AppContext> _contextStack;
    private Stack<AppContext> ContextStack
    {
        get
        {
            if (this._contextStack == null)
            {
                this._contextStack = new Stack<AppContext>();
                this._contextStack.Push(AppContext.MAIN);
            }
            return this._contextStack;
        }
    }

    public AppContext AppContext
    {
        get
        {
            return this.ContextStack.Peek();
        }
    }

    //=============================================================================== Methods

    public AppCanvas Initialize()
    {
        DontDestroyOnLoad(gameObject);

        StartedUp = true;

        if (ViewPrefabs.Contains(null))
        {
            Debug.LogError("CRITICAL ERROR: The AppCanvas contains a null value in \"ViewPrefabs\". " +
                           "It cannot function with a null in this list.");

        }
        else
        {
            if (startupScreen != null)
            {
                GetOrCreate(startupScreen.GetType()).Show();
            }
            else
            {
                Debug.LogWarning("Attention: There is no Startup Screen defined on AppCanvas.");
            }

            foreach (BaseView view in instancedViewTypes)
            {
                loadedViews.Add((BaseView)FindFirstObjectByType(view.GetType()));
            }

        }

        return this;

    }

    public void StackContext(AppContext newContext)
    {

        if (this.ContextStack.Count > 2)
        {
            Debug.LogError($"[ERROR] You cannot stack more than two contexts, ignoring stacking of context: \"{newContext}\".");
            return;
        }

        this.ContextStack.Push(newContext);
        Debug.Log($"<color=yellow>Stacking Context \"{newContext}\"</color>");

    }

    public int UnstackContext(AppContext contextToUnstack)
    {
        if (contextToUnstack != this.ContextStack.Peek())
        {
            Debug.LogError($"[ERROR] You cannot unstack \"{contextToUnstack}\" because the current top is \"{this.ContextStack.Peek()}\".");
            return -1;
        }

        AppContext currentAppContext = this.ContextStack.Pop();

        var contextViews = loadedViews.FindAll(t => t.ViewContext == currentAppContext);

        Debug.Log($"Closing {contextViews.Count} Views...");

        contextViews.ForEach(t => t.Close());

        Debug.Log($"<color=yellow>Unstacking Context \"{contextToUnstack}\"</color>");
        return contextViews.Count;

    }

    public T GetOrCreate<T>(CanvasOverlay canvasOverlay = CanvasOverlay.None, Transform parentGameObject = null) where T : BaseView
    {
        return (T)GetOrCreate(typeof(T), canvasOverlay, parentGameObject);

    }

    public BaseView GetOrCreate(Type desiredType, CanvasOverlay canvasOverlay = CanvasOverlay.None, Transform parentGameObject = null)
    {

        if (!StartedUp)
        {
            Debug.LogError($"Attention: Get or Create view \"{desiredType.Name}\" " +
                           $"requested, but AppCanvas is not Active in the scene.");
        }

        var existingView = loadedViews.Find(t => t.GetType().Name == desiredType.Name);

        if (existingView != null)
        {
            return existingView;
        }

        BaseView prefab;
        try
        {
            prefab = ViewPrefabs.Find(t => t.GetType().Name == desiredType.Name);
        }
        catch (Exception)
        {
            Debug.LogError($"Not found screen {desiredType.Name} in AppCanvas");
            throw;
        }

        if (prefab == null)
        {
            Debug.LogWarning($"Failed to create a View named \"{desiredType.Name}\" because there is " +
                             $"no prefab for it on AppCanvas.\"ViewPrefabs\".");

            return null;

        }

        if (parentGameObject == null)
        {
            parentGameObject = this.transform;
        }

        var viewInstance = Instantiate(prefab, parentGameObject);

        onCreateSuccess?.Invoke(viewInstance);

        return RegisterScreen(viewInstance, canvasOverlay);

    }

    public BaseView RegisterScreen(BaseView viewInstance, CanvasOverlay canvasOverlay = CanvasOverlay.None)
    {

        CurrentCanvasOverlay = canvasOverlay;

        viewInstance.InjectAppCanvas(this);

        if (canvasOverlay != CanvasOverlay.None)
        {
            viewInstance.SetCanvasSortingOrder(GetFirstAvailableSortingOrder(canvasOverlay));

        }

        viewInstance.SetViewContext(this.ContextStack.Peek());

        loadedViews.Add(viewInstance);

        viewInstance.SetInit();

        return viewInstance;
    }

    public T GetOrCreatePopup<T>() where T : BaseView
    {
        return (T)GetOrCreate(typeof(T), CanvasOverlay.Popup);

    }

    public BaseView GetOrCreatePopup(Type desiredType)
    {
        return GetOrCreate(desiredType, CanvasOverlay.Popup);
    }

    private int GetFirstAvailableSortingOrder(CanvasOverlay canvasOverlay)
    {
        int desiredSortingOrder = (int)canvasOverlay;

        var topViews
            = loadedViews.FindAll(t => t.CanvasOrder >= desiredSortingOrder && t.CanvasOrder < desiredSortingOrder + 100);

        BaseView topMostView = null;

        if (topViews != null && topViews.Count > 0)
        {
            topViews = topViews.OrderByDescending(x => x.CanvasOrder).ToList();
            topMostView = topViews.First();
        }

        if (topMostView != null)
        {
            return topMostView.CanvasOrder + 1;

        }

        return desiredSortingOrder;

    }

    public BaseView GetView(Type desiredType)
    {
        return loadedViews.Find(t => t.GetType().Name == desiredType.Name);
    }

    public T GetView<T>() where T : BaseView
    {
        return (T)GetView(typeof(T));
    }

    public bool ViewExists<T>() where T : BaseView
    {
        return ((T)GetView(typeof(T))) != null;
    }

    public bool ViewIsVisible<T>() where T : BaseView
    {
        var view = ((T)GetView(typeof(T)));
        return view != null && view.IsExecuting;

    }

#if UNITY_EDITOR

    [PropertySpace(SpaceBefore = 5, SpaceAfter = 10)]
    [Button(ButtonSizes.Small)]
    public void ReGenerateViewPrefabs()
    {

        ViewPrefabs = new List<BaseView>();

        foreach (var viewAssetId in (AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/UI/Screens" })))
        {
            string pathToPrefab = AssetDatabase.GUIDToAssetPath(viewAssetId);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathToPrefab);

            BaseView currentView = prefab.GetComponent<BaseView>();

            if (currentView != null)
            {
                if (!DoesViewExistsInPrefabsList(currentView))
                {
                    ViewPrefabs.Add(currentView);
                }
                else
                {
                    Debug.LogError("ERROR: THE UI DIRECTORY HAS TWO PREFABS OF " +
                                   $"THE SAME TYPE \"{currentView.GetType()}\"! PLEASE CHOOSE ONE OR THE OTHER. " +
                                   $"ONLY ONE IS BEING KEPT...");
                }

            }

        }
    }

#endif

    /// <summary>
    /// Disables "DontDestroyOnLoad" by parenting the object to another, destructable object.
    /// There is no other way at present to do so.
    /// </summary>
    public void DisableDontDestroyOnLoad()
    {
        GameObjectUtils.DisableDontDestroyOnLoad(this.gameObject);

    }

    private bool DoesViewExistsInPrefabsList(BaseView currentView)
    {
        return ViewPrefabs.Find(x => x != null && x.GetType() == currentView.GetType()) != null;
    }

    public void DestroyAllAndRestart()
    {
        foreach (var view in loadedViews)
        {
            view.Close();
        }
        loadedViews.Clear();

    }

    public void DestroyView(BaseView baseView)
    {
        loadedViews.RemoveAll(t => t == null || t == baseView);

        if (baseView != null) Destroy(baseView.gameObject);

    }

    public T UnstackAllAndTransitionTo<T>(BaseView unstackFrom) where T : BaseView
    {
        return (T)UnstackAllAndTransitionTo(unstackFrom, typeof(T));

    }

    public BaseView UnstackAllAndTransitionTo(BaseView currentScreen, Type newScreenType, bool showImmediately = true)
    {
        currentScreen = UnstackBackAndCloseParentFrom(currentScreen);

        var newScreen = GetOrCreate(newScreenType);

        newScreen.SetCanvasSortingOrder(currentScreen.CanvasOrder);

        if (showImmediately) newScreen.Show();

        return newScreen;

    }

    /// <summary>
    /// Closes all screens, backwards from the current, but does NOT close the upmost parent.
    /// </summary>
    /// <param name="currentScreen"></param>
    /// <returns></returns>
    public BaseView UnstackBackwardsFrom(BaseView currentScreen, Type until = null)
    {
        while (currentScreen.ParentScreen != null && currentScreen?.GetType() != until)
        {
            currentScreen.UnstackThisView();
            currentScreen = currentScreen.ParentScreen;
        }

        return currentScreen;
    }

    /// <summary>
    /// Closes all screens, backwards from the current, AND ALSO CLOSES the upmost parent.
    /// </summary>
    /// <param name="currentScreen"></param>
    /// <returns></returns>
    public BaseView UnstackBackAndCloseParentFrom(BaseView currentScreen)
    {
        var lastScreen = UnstackBackwardsFrom(currentScreen);

        lastScreen.Close();

        return lastScreen;

    }

}

public enum AppContext
{
    UNDEFINED,
    MAIN,
    ACCESSORIES
}

public enum CanvasOverlay
{
    None = -1,
    LowPriority = 300,
    MediumPriority = 500,
    Popup = 700,
    Notifications = 1000,
    HighPriority = 1500,
    Debug = 2000
}
