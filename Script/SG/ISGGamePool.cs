using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池虚基类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ISGGamePool<T>
{
    /// <summary>
    /// 对象池的名字，用来做唯一标识的
    /// </summary>
    private string m_PoolName;
    public string PoolName { get { return m_PoolName; } }

    /// <summary>
    /// 池所管理的对象
    /// </summary>
    private Dictionary<object, List<T>> m_Pools;

    /// <summary>
    /// 需要实例化的内容
    /// </summary>
    private Dictionary<object, T> m_ControlObjects;

    /// <summary>
    /// 池内物体对象父节点
    /// </summary>
    protected GameObject m_PoolParent;

    public ISGGamePool() : this("") { }

    public ISGGamePool(string name)
    {
        m_PoolName = name;

        m_Pools = new Dictionary<object, List<T>>();
        m_Pools.Clear();

        m_ControlObjects = new Dictionary<object, T>();
        m_ControlObjects.Clear();
    }

    /// <summary>
    /// 压入对象池
    /// </summary>
    /// <param name="id"></param>
    /// <param name="o"></param>
    public void PushPoolObject(object id, T o)
    {
        if (o == null)
        {
            Debug.LogWarning("the object is null.");
            return;
        }

        if (ResetObject(o))
        {
            if (m_Pools.ContainsKey(id))
            {
                if (m_Pools[id].Contains(o))
                {
                    Debug.LogWarningFormat("the object:{0} had in pool.", o);
                    return;
                }

                m_Pools[id].Add(o);
            }
            else
            {
                m_Pools.Add(id, new List<T>() { o });
            }

            CreateParent();
            SetPoolParent(o);
        }
    }

    /// <summary>
    /// 添加管理的对象
    /// </summary>
    /// <param name="id"></param>
    /// <param name="o"></param>
    /// <param name="isChange"></param>
    public void PushControlObject(object id, T o, bool isChange = false)
    {
        if (m_ControlObjects.ContainsKey(id))
        {
            if (!isChange)
            {
                Debug.LogWarningFormat("the id:{0} had in control.", id);
                return;
            }

            m_ControlObjects[id] = o;
        }
        else
        {
            m_ControlObjects.Add(id, o);
        }

        CreateParent();
        SetPoolParent(o);
    }

    /// <summary>
    /// 判断管理类当中是否存在这个对象
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual bool IsExistence(object id)
    {
        return m_ControlObjects.ContainsKey(id) && m_ControlObjects[id] != null;
    }

    /// <summary>
    /// 获取一个对象实例
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public T GetPoolsObject(object id)
    {
        T t = default(T);
        bool has = false;

        if (m_Pools.ContainsKey(id))
        {
            if (m_Pools[id].Count > 0)
            {
                t = m_Pools[id][0];
                m_Pools[id].RemoveAt(0);

                if (m_Pools[id].Count <= 0)
                {
                    m_Pools.Remove(id);
                }

                has = true;
            }
            else
            {
                m_Pools.Remove(id);
            }
        }

        if (!has)
        {
            if (m_ControlObjects.ContainsKey(id))
            {
                t = InitilizeObject(m_ControlObjects[id]);
            }
        }

        return t;
    }

    /// <summary>
    /// 清理对象池
    /// </summary>
    public void ClearPool()
    {
        List<T> clear = new List<T>();
        clear.Clear();

        foreach (var item in m_Pools)
        {
            if (item.Value.Count > 0)
            {
                clear.AddRange(item.Value);
            }
        }

        m_Pools.Clear();

        while (clear.Count > 0)
        {
            T t = clear[0];
            ClearObject(t);
            clear.RemoveAt(0);
        }
    }

    /// <summary>
    /// 清理管理对象
    /// </summary>
    public void ClearControl()
    {
        List<T> clear = new List<T>();
        clear.Clear();
        foreach (var item in m_ControlObjects)
        {
            clear.Add(item.Value);
        }

        m_ControlObjects.Clear();

        while (clear.Count > 0)
        {
            T t = clear[0];
            ClearObject(t);
            clear.RemoveAt(0);
        }
    }

    /// <summary>
    /// 移除一个实例
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public void RemoveObject(object id)
    {
        if (m_ControlObjects.ContainsKey(id))
        {
            m_ControlObjects.Remove(id);
        }
    }

    /// <summary>
    /// 获取一个对象
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public T GetPoolPrefab(object Uid)
    {
        if (m_ControlObjects.ContainsKey(Uid))
        {
            return m_ControlObjects[Uid];
        }

        return default(T);
    }

    /// <summary>
    /// 创建父节点
    /// </summary>
    protected void CreateParent()
    {
        if (m_PoolParent == null)
        {
            m_PoolParent = new GameObject();
            m_PoolParent.name = m_PoolName;
            SGGamePoolManager.Instance.SetSunToManager(m_PoolParent);
            m_PoolParent.gameObject.transform.position = Vector3.zero;
            m_PoolParent.gameObject.transform.eulerAngles = Vector3.zero;
            m_PoolParent.gameObject.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 给对象设置父节点
    /// </summary>
    /// <param name="t"></param>
    protected abstract void SetPoolParent(T t);

    /// <summary>
    /// 初始化一个T
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public abstract T InitilizeObject(T o);

    /// <summary>
    /// 销毁一个T
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public abstract bool ClearObject(T o);

    /// <summary>
    /// 重置一个T
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public abstract bool ResetObject(T o);
}
