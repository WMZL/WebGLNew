using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 单例基类
/// </summary>
public abstract class Singleton<T> : ObjectBase where T : ObjectBase
{
    private static T m_Instance;
    public static T Instance { get { return m_Instance; } }

    protected override void Awake()
    {
        base.Awake();

        if (this.gameObject == null)
        {
            GameObject go = new GameObject();
            go.name = typeof(T).Name;
            m_Instance = go.AddComponent<T>();
        }

        if (m_Instance == null)
        {
            m_Instance = this.gameObject.GetComponent<T>();
        }

        GameObject.DontDestroyOnLoad(this.gameObject);
    }
}