using UnityEngine;
using System.Collections;

public class UTreeData
{
    /// <summary>
    /// 偏移
    /// </summary>
    public float offset;

    /// <summary>
    /// 当前 ID
    /// </summary>
    public string id;

    /// <summary>
    /// 父 ID
    /// </summary>
    public string parentID;

    /// <summary>
    /// 层级
    /// </summary>
    public int level;

    /// <summary>
    /// 文本
    /// </summary>
    public string name;

    /// <summary>
    /// 树对象
    /// </summary>
    public object data;

    /// <summary>
    /// 展开状态
    /// </summary>
    public bool expand = false;

    public UTreeData(string id, string parentID, string name, object data, float offset = 30f)
    {
        this.offset = offset;
        this.id = id;
        this.parentID = parentID;
        this.name = name;
        this.data = data;
    }
}
