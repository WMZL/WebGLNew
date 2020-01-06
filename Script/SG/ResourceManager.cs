using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    None = 0,

    /// <summary>
    /// 角色
    /// </summary>
    Character,

    /// <summary>
    /// 物品
    /// </summary>
    Goods,

    /// <summary>
    /// 地图
    /// </summary>
    Map,

    /// <summary>
    /// 技能
    /// </summary>
    Skill,

    /// <summary>
    /// 特效
    /// </summary>
    Effect,

    /// <summary>
    /// UI
    /// </summary>
    UI,

    /// <summary>
    /// UI图标
    /// </summary>
    Icon,

    /// <summary>
    /// 三维贴图
    /// </summary>
    Texture,

    /// <summary>
    /// 音效
    /// </summary>
    Audio,

    /// <summary>
    /// 音乐
    /// </summary>
    Music,

    /// <summary>
    /// 视频
    /// </summary>
    Movies,

    /// <summary>
    /// 文本
    /// </summary>
    Text,
    /// <summary>
    /// 设备
    /// </summary>
    Device,
}

/// <summary>
/// 加载资源的数据
/// </summary>
public struct ResourceInfo
{
    /// <summary>
    /// 资源名称
    /// </summary>
    public string m_RName;

    /// <summary>
    /// 资源路径
    /// </summary>
    public string m_RPath;

    /// <summary>
    /// 资源类型
    /// </summary>
    public ResourceType m_RType;

    /// <summary>
    /// 资源本体
    /// </summary>
    public GameObject m_RNoumenon;

    public List<IResourceLoadCallback> m_CallBackList;

    public ResourceInfo(string name, string path, ResourceType type)
    {
        m_RName = name;
        m_RPath = path;
        m_RType = type;
        m_RNoumenon = null;

        m_CallBackList = new List<IResourceLoadCallback>();
        m_CallBackList.Clear();
    }

    public ResourceInfo(string name, ResourceType type)
    {
        m_RName = name;
        m_RType = type;
        m_RNoumenon = null;
        m_RPath = string.Empty;
        m_CallBackList = new List<IResourceLoadCallback>();
        m_CallBackList.Clear();
        CalculationPath();
    }

    public ResourceInfo(ResourceInfo info)
    {
        m_RName = info.m_RName;
        m_RPath = info.m_RPath;
        m_RType = info.m_RType;
        m_RNoumenon = null;
        m_CallBackList = null;
    }

    private void CalculationPath()
    {
        m_RPath = m_RType.ToString() + "/" + m_RName;
    }

    public void AddCallback(IResourceLoadCallback cb)
    {
        if (!m_CallBackList.Contains(cb))
        {
            int index = 0;
            for (; index < m_CallBackList.Count; index++)
            {
                if (m_CallBackList[index].LoadCallbackPriority() > cb.LoadCallbackPriority())
                {
                    break;
                }
            }

            index = index == 0 ? 0 : index - 1;
            m_CallBackList.Insert(index, cb);
        }
    }

    public void SetNoumenon(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        this.m_RNoumenon = go;
        for (int index = 0; index < m_CallBackList.Count; index++)
        {
            if (index == m_CallBackList.Count - 1)
            {
                ResourceInfo info = new ResourceInfo(this);
                m_CallBackList[index].HandleLoadCallback(info, this.m_RNoumenon);
            }
            else
            {
                m_CallBackList[index].HandleLoadCallback(this, GameObject.Instantiate(this.m_RNoumenon));
            }
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        ResourceInfo info = (ResourceInfo)(obj);
        return this.m_RPath == info.m_RPath;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

/// <summary>
/// 加载过得资源内容
/// </summary>
internal class LoadedObjectInfo
{
    public ResourceType m_Type;
    public string m_PathDic;
    public GameObject m_GameObject;

    public LoadedObjectInfo(ResourceType type, string path) : this(type, path, null) { }

    public LoadedObjectInfo(ResourceType type, string path, GameObject go)
    {
        m_Type = type;
        m_PathDic = path;
        m_GameObject = go;
    }

    public override bool Equals(object obj)
    {
        LoadedObjectInfo oj = obj as LoadedObjectInfo;
        if (oj == null)
        {
            return false;
        }

        return oj.m_Type == this.m_Type && oj.m_PathDic == this.m_PathDic;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("Type:{0} Path:{1} Object:{2}", m_Type, m_PathDic, m_GameObject);
    }
}

/// <summary>
/// 加载资源的回调
/// </summary>
public abstract class IResourceLoadCallback
{
    /// <summary>
    /// 回调处理
    /// </summary>
    /// <param name="info"></param>
    /// <param name="go"></param>
    public abstract void HandleLoadCallback(ResourceInfo info, GameObject go);

    /// <summary>
    /// 同一个内容的回调优先级
    /// </summary>
    /// <returns></returns>
    public abstract int LoadCallbackPriority();
}

public class ResourceLoadCallback : IResourceLoadCallback
{
    private Action<ResourceInfo, GameObject> m_CBData;

    public ResourceLoadCallback(Action<ResourceInfo, GameObject> callback)
    {
        m_CBData = callback;
    }
    public override int LoadCallbackPriority()
    {
        return 0;
    }

    public override void HandleLoadCallback(ResourceInfo info, GameObject go)
    {
        if (m_CBData != null)
        {
            m_CBData(info, go);
        }
    }
}

/// <summary>
/// 资源加载
/// </summary>
public class ResourceManager : Singleton<ResourceManager>
{
    private Dictionary<string, ResourceInfo> m_AllLoadInfoDic;
    private List<LoadedObjectInfo> m_LoadedObjects;

    protected override void Awake()
    {
        base.Awake();

        m_AllLoadInfoDic = new Dictionary<string, ResourceInfo>();
        m_AllLoadInfoDic.Clear();

        m_LoadedObjects = new List<LoadedObjectInfo>();
        m_LoadedObjects.Clear();
    }

    public void LoadResource(string name, ResourceType type, IResourceLoadCallback callback)
    {
        ResourceInfo info = new ResourceInfo(name, type);
        LoadResource(info.m_RName, info.m_RPath, info.m_RType, callback);
    }

    public void LoadResource(string name, string path, ResourceType type, IResourceLoadCallback callback)
    {
        ResourceInfo info = new ResourceInfo(name, path, type);
        if (m_AllLoadInfoDic.ContainsKey(info.m_RPath))
        {
            info = m_AllLoadInfoDic[info.m_RPath];
        }

        info.AddCallback(callback);

        LoadedObjectInfo oj = new LoadedObjectInfo(info.m_RType, info.m_RPath);
        if (m_LoadedObjects.Contains(oj))
        {
            for (int index = 0; index < m_LoadedObjects.Count; index++)
            {
                if (m_LoadedObjects[index].Equals(oj))
                {
                    GameObject go = GameObject.Instantiate(m_LoadedObjects[index].m_GameObject);
                    info.SetNoumenon(go);
                    break;
                }
            }
        }
        else
        {
            GameObject go = Resources.Load(info.m_RPath, typeof(GameObject)) as GameObject;
            if (go != null)
            {
                go.SetActive(false);
                m_LoadedObjects.Add(new LoadedObjectInfo(info.m_RType, info.m_RPath, go));
                info.SetNoumenon(GameObject.Instantiate(go));
            }
            else
            {
                Debug.LogWarning("the object is null." + info);
            }
        }

        m_AllLoadInfoDic.Remove(info.m_RPath);
    }
}