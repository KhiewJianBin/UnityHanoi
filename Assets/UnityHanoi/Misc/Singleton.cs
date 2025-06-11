/*
	MIT License
	Copyright (c) 2022 Khiew Jian Bin
*/

using System;
using UnityEngine;

/// <summary>
/// Singleton Type T of Component
/// Find if absent in scene
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    #region Fields
    /// <summary>
    /// The instance.
    /// </summary>
    private static T instance;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>() ?? throw new Exception($"No {typeof(T).FullName} Singleton Found");
            }
            return instance;
        }
    }

    #endregion

    #region Methods
    /// <summary>
    /// Use this for initialization.
    /// </summary>
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning($"There is already an instance of this Singleton. Removing {typeof(T).FullName} instance from: {name} GameObject");
            Destroy(gameObject);
        }
    }
    #endregion
}