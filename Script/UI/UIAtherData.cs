/*
 * Creator:ffm
 * Desc:其他数据
 * Time:7/9/2018 5:46:19 PM
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UILayer
{
    /// <summary>
    /// 默认面板
    /// </summary>
    None,

    /// <summary>
    /// 全屏面板
    /// </summary>
    Pnl,

    /// <summary>
    /// 弹框面板
    /// </summary>
    Dlg,

    /// <summary>
    /// 紧急面板
    /// </summary>
    Black,

    /// <summary>
    /// 对象池
    /// </summary>
    Pool,

    /// <summary>
    /// 顶部面板
    /// </summary>
    Top
}

public struct UIModelData
{
    /// <summary>
    /// 自身类型
    /// </summary>
    public Type m_Type;

    /// <summary>
    /// 关联界面
    /// </summary>
    public Type[] m_Relations;

    /// <summary>
    /// 是否隐藏其他界面
    /// </summary>
    public bool m_IsOverlay;

    /// <summary>
    /// 排斥界面
    /// </summary>
    public Type[] m_Excludes;

    public UIModelData(Type type, Type[] relations = null, bool overlay = false, Type[] excludes = null)
    {
        this.m_Type = type;
        this.m_Relations = relations;
        this.m_IsOverlay = overlay;
        this.m_Excludes = excludes;
    }
}
