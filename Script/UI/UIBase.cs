/*
 * Creator:ffm
 * Desc:UI基础控制
 * Time:7/6/2018 9:03:37 AM
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI基类
/// </summary>
public class UIBase : ObjectBase
{
    /// <summary>
    /// module类型
    /// </summary>
    private Type m_Type;
    public Type UIType { get { return m_Type; } }

    /// <summary>
    /// UI真正显示的层级
    /// </summary>
    public UILayer m_ShowLayer;

    /// <summary>
    /// 不被清除标识
    /// </summary>
    public bool m_DontOverLayer;


    /// <summary>
    /// 是否关闭
    /// </summary>
    [SerializeField]
    private bool m_IsClose;
    public bool IsClose
    {

        get { return m_IsClose; }
        set { m_IsClose = value; }
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="layer"></param>
    /// <param name="arms"></param>
    /// <returns></returns>
    public virtual bool InitData(Type type, UILayer layer, params object[] arms)
    {
        m_IsClose = false;
        m_Type = type;
        m_ShowLayer = layer == UILayer.None ? m_ShowLayer : layer;
        return true;
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    /// <returns></returns>
    public virtual bool OnShow()
    {
        this.gameObject.SetActive(true);
        return true;
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <returns></returns>
    public virtual bool OnClose()
    {
        UIManager.Instance.CloseUI(this);
        return true;
    }

    /// <summary>
    /// 界面显示控制
    /// </summary>
    /// <param name="isShow"></param>
    /// <returns></returns>
    public virtual bool SetModuleDisable()
    {
        this.gameObject.SetActive(false);
        return true;
    }

    /// <summary>
    /// 界面更新
    /// </summary>
    /// <returns></returns>
    public virtual bool OnUpdate()
    {
        return true;
    }

    public override bool Equals(object other)
    {
        UIBase u = other as UIBase;
        if (u == null)
        {
            return false;
        }

        return u.UIType == this.UIType && u.m_ShowLayer == this.m_ShowLayer;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
