using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SGGamePoolManager : Singleton<SGGamePoolManager>
{
    private Dictionary<string, object> m_ControlPool;

    private GameObject m_PoolManager;

    protected override void Awake()
    {
        base.Awake();

        m_ControlPool = new Dictionary<string, object>();
        m_ControlPool.Clear();
    }

    private void CreateParent()
    {
        if (m_PoolManager == null)
        {
            m_PoolManager = new GameObject();
            m_PoolManager.name = "SGGamePoolManager";
            m_PoolManager.transform.SetParent(null);
            m_PoolManager.gameObject.transform.position = Vector3.zero;
            m_PoolManager.gameObject.transform.eulerAngles = Vector3.zero;
            m_PoolManager.gameObject.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 设置父节点
    /// </summary>
    /// <param name="sun"></param>
    public void SetSunToManager(GameObject sun)
    {
        CreateParent();
        sun.transform.SetParent(m_PoolManager.transform);
    }

    /// <summary>
    /// 获取一个对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public ISGGamePool<T> GetPoolControl<T>(string name)
    {
        ISGGamePool<T> t = default(ISGGamePool<T>);
        if (m_ControlPool.ContainsKey(name))
        {
            t = m_ControlPool[name] as ISGGamePool<T>;
        }

        return t;
    }

    /// <summary>
    /// 压入一个对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="isChange">是否强制更换</param>
    public void PushControl<T>(ISGGamePool<T> t, bool isChange = false)
    {
        if (m_ControlPool.ContainsKey(t.PoolName))
        {
            if (!isChange)
            {
                Debug.LogWarningFormat("the pool:{0} had in dic.", t.PoolName);
                return;
            }

            RemovePool<T>(t.PoolName);
            m_ControlPool.Add(t.PoolName, t);
        }
        else
        {
            m_ControlPool.Add(t.PoolName, t);
        }
    }

    /// <summary>
    /// 判断管理当中是否存在这个对象池
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsExistence(string name)
    {
        return m_ControlPool.ContainsKey(name);
    }

    /// <summary>
    /// 清空一个对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    public void RemovePool<T>(string name)
    {
        if (IsExistence(name))
        {
            (m_ControlPool[name] as ISGGamePool<T>).ClearControl();
            (m_ControlPool[name] as ISGGamePool<T>).ClearPool();

            m_ControlPool.Remove(name);
        }
    }

    /// <summary>
    /// 获取有哪些对象池
    /// </summary>
    /// <returns></returns>
    public List<string> GetPools()
    {
        List<string> ns = new List<string>();
        ns.AddRange(m_ControlPool.Keys);
        return ns;
    }
}
