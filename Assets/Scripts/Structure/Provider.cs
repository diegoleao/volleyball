
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

    [SerializeField] GameState gameState;
    public GameState GameState => this.gameState;


    [SerializeField] GameNetworking gameNetworking;
    public GameNetworking GameNetworking => this.gameNetworking;



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
        }

        return _instance;

    }

    public bool HasStateAuthority
    {
        get
        {
            return gameNetworking.HasStateAuthority;
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
            if (registeredObjects.Any(t => t.GetType() == typeof(T))) return;

            registeredObjects.Add(objectToRegister);
        }
        catch (Exception exc)
        {
            Debug.LogError($"Failed to register object {typeof(T).Name}");
            Debug.LogException(exc);
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

    public T FindReference<T>() where T : MonoBehaviour
    {
        return FindObjectOfType<T>();

    }

}