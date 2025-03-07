
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static GameState;

public class Provider : MonoBehaviour
{

#if UNITY_EDITOR
    [Header("Debug")]
    public bool _speedUpForDebugging;
    public static bool SpeedUpForDebugging => Instance._speedUpForDebugging;
#endif

    [Header("Prefabs")]

    [SerializeField] AppCanvas appCanvasPrefab;

    [Header("References")]
    [SerializeField] StateMachine _stateMachine;

    [SerializeField] GameState gameState;
    public static GameState GameState => Instance.gameState;


    [SerializeField] GameplayFacade gameplayFacade;
    public static GameplayFacade GameplayFacade => Instance.gameplayFacade;

    public static NetworkMode NetworkMode => GameplayFacade.PlayMode;

    public static IVolleyballGameplay API => GameplayFacade.CurrentAPI;

    [Header("Scene Components")]

    private CourtTriggers _courtTriggers;
    public static CourtTriggers CourtTriggers => Instance._courtTriggers;

    private Transform _netCenter;
    public static Transform CourtCenter => Instance._netCenter;

    private AppCanvas _appCanvas;
    public static AppCanvas AppCanvas => Instance._appCanvas;

    private VirtualJoystick _volleyJoystick;
    public static VirtualJoystick VolleyJoystick => Instance._volleyJoystick;

    public static StateMachine StateMachine => Instance._stateMachine;

    public static bool HasStateAuthority => GameplayFacade.GameNetworking.HasStateAuthority;

    [ShowInInspector] private static List<MonoBehaviour> registeredObjects = new List<MonoBehaviour>();

    /// <summary>
    /// Lazy loaded instance.
    /// </summary>
    private static Provider _instance;
    public static Provider Instance
    {
        get
        {
            return InitializeInstanceValue();
        }
    }

    private static Provider InitializeInstanceValue()
    {
        if (_instance == null || _instance.gameObject == null)
        {
            _instance = FindObjectOfType<Provider>();
            _instance._appCanvas = Instantiate(Instance.appCanvasPrefab).Initialize();
            _instance._courtTriggers = FindAnyObjectByType<CourtTriggers>();
            _instance._netCenter = FindAnyObjectByType<NetCenter>().transform;
            _instance._volleyJoystick = AppCanvas.GetView<OptionsScreen>()?.VirtualJoystick;
        }

        return _instance;

    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        InitializeInstanceValue();

    }

    async void Start()
    {
        await Observable.NextFrame();
        this.gameState.Initialize();

    }

    public static void Register<T>(MonoBehaviour objectToRegister) where T : MonoBehaviour
    {
        try
        {
            RemovePreviousVersions(objectToRegister);

            //Removes deleted objects before accessing their properties next
            registeredObjects.RemoveAll(t => t == null);

            if (registeredObjects.Any(t => t.GetType() == typeof(T))) return;

            registeredObjects.Add(objectToRegister);

            InjectUniqueObjects(objectToRegister);

        }
        catch (Exception exc)
        {
            Debug.LogError($"Failed to register object {typeof(T).Name}");
            Debug.LogException(exc);
        }

    }

    private static void RemovePreviousVersions(MonoBehaviour objectToRegister)
    {
        if (objectToRegister is LocalVolleyball)
        {
            Unregister<LocalVolleyball>();
        }

        if (objectToRegister is NetworkVolleyball)
        {
            Unregister<NetworkVolleyball>();
        }

    }

    private static void InjectUniqueObjects(MonoBehaviour objectToRegister)
    {
        if (objectToRegister is MatchInfo)
        {
            GameState.SetMatchInfo(objectToRegister as MatchInfo);

            FindAnyObjectByType<HudView>().Initialize((objectToRegister as MatchInfo).LocalInfo);

            API.InjectMatchInfo(objectToRegister as MatchInfo);

        }

        if (objectToRegister is LocalMatchInfo)
        {
            GameState.SetLocalMatchInfo(objectToRegister as LocalMatchInfo);

            FindAnyObjectByType<HudView>().Initialize(objectToRegister as LocalMatchInfo);

            API.InjectMatchInfo(objectToRegister as LocalMatchInfo);

        }

        if (objectToRegister is LocalVolleyball)
        {
            FindAnyObjectByType<AIPlayer>()?.InjectVolleyball(objectToRegister as LocalVolleyball);

        }

    }

    public static T Get<T>(bool findDisabledObjects = false) where T : MonoBehaviour
    {

        //Removes deleted objects before accessing their properties next
        registeredObjects.RemoveAll(t => t == null);

        var registeredObject = (T)registeredObjects.Find(t => t.GetType().Name == typeof(T).Name);

        if (registeredObject == null)
        {
            registeredObject = FindObjectOfType<T>(findDisabledObjects);
#if UNITY_EDITOR

            Debug.LogWarning($"Searched {typeof(T)} in scene: {((registeredObject == null) ? "Unable to find." : $"Object named \"{registeredObject.name}\" found.")}");
#endif
        }

        return registeredObject;

    }

    public static bool Unregister<T>(bool findDisabledObjects = false) where T : MonoBehaviour
    {
        var objectToRemove = Get<T>(findDisabledObjects);

        if (objectToRemove == null) return false;

        registeredObjects.Remove(objectToRemove);

        return true;

    }

    public T FindReference<T>() where T : MonoBehaviour
    {
        return FindObjectOfType<T>();

    }

    void OnApplicationQuit()
    {
        registeredObjects.Clear();

    }

}