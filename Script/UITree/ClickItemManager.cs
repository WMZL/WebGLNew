using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

public class ClickItemManager : Singleton<ClickItemManager>
{
    /// <summary>
    /// 所有的可以点击的物体
    /// </summary>
    private ClickItemInfo[] m_AllClickItemArr;

    private Dictionary<string, ClickItemInfo> m_AllClickItem;

    public Dictionary<string, ClickItemInfo> AllClickItem { get { return m_AllClickItem; } }

    public Highlighter CurHlighter { get { return m_CurHlighter; } set { m_CurHlighter = value; } }

    /// <summary>
    /// 当前选中，高亮显示
    /// </summary>
    private Highlighter m_CurHlighter;

    private void Start()
    {
        string ObjName = ReadJavaInterface.m_Unitid;
        m_AllClickItem = new Dictionary<string, ClickItemInfo>();
        GameObject go = GameObject.Find(ObjName);
        m_AllClickItemArr = go.GetComponentsInChildren<ClickItemInfo>();
        foreach (var item in m_AllClickItemArr)
        {
            if (!m_AllClickItem.ContainsKey(item.m_UniqueID))
            {
                m_AllClickItem.Add(item.m_UniqueID, item);
            }
            else
            {
                Debug.LogError("存在相同key值，出现错误！！" + item.m_UniqueID);
            }
        }
    }

    /// <summary>
    /// 通过唯一的ID查找摄像机位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCamPos(string id)
    {
        if (m_AllClickItem.ContainsKey(id))
        {
            return m_AllClickItem[id].m_CamPos;
        }
        else
        {
            Debug.LogError("未找到指定key值");
            return Vector3.zero;
        }
    }

    /// <summary>
    /// 通过唯一ID查找摄像机旋转
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCamRotate(string id)
    {
        if (m_AllClickItem.ContainsKey(id))
        {
            return m_AllClickItem[id].m_Rotation;
        }
        else
        {
            Debug.LogError("未找到指定key值");
            return Vector3.zero;
        }
    }
}