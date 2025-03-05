
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static GameState;

public class Provider : MonoBehaviour
{

#if UNITY_EDITOR
    [Header("Debug")]
    public bool SpeedUpForDebugging;
#endif


    [Header("References")]
    [SerializeField] GameState gameState;
    public GameState GameState => this.gameState;


    [SerializeField] GameplayFacade gameplayFacade;
    public GameplayFacade GameplayFacade => this.gameplayFacade;

    public NetworkMode NetworkMode => GameplayFacade.PlayMode;

    public IVolleyballGameplay API => GameplayFacade.CurrentAPI;

    [Header("Scene Components")]

    [SerializeField] VolleyJoystick volleyJoystick;
    public VolleyJoystick VolleyJoystick => this.volleyJoystick;

    [SerializeField] CourtTriggers courtTriggers;
    public CourtTriggers CourtTriggers => this.courtTriggers;

    [Header("Position References")]

    [SerializeField] Transform courtCenter;
    public Transform CourtCenter => this.courtCenter;

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
            registeredObjects = new List<MonoBehaviour>();
        }

        return _instance;

    }

    public bool HasStateAuthority
    {
        get
        {
            return GameplayFacade.GameNetworking.HasStateAuthority;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        InitializeInstanceValue();

    }

    void Start()
    {
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
            Instance.GameState.SetMatchInfo(objectToRegister as MatchInfo);

            FindAnyObjectByType<HudView>().Initialize((objectToRegister as MatchInfo).LocalInfo);

            Instance.API.InjectMatchInfo(objectToRegister as MatchInfo);

        }

        if (objectToRegister is LocalMatchInfo)
        {
            Instance.GameState.SetLocalMatchInfo(objectToRegister as LocalMatchInfo);

            FindAnyObjectByType<HudView>().Initialize(objectToRegister as LocalMatchInfo);

            Instance.API.InjectMatchInfo(objectToRegister as LocalMatchInfo);

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