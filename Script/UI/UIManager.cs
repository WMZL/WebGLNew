/*
 * Creator:ffm
 * Desc:UI框架
 * Time:7/6/2018 8:59:03 AM
* */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI管理类
/// </summary>
public class UIManager : Singleton<UIManager>
{
    private Camera m_UICamera;
    public Camera UICamera { get { return m_UICamera; } }

    /// <summary>
    /// UI读取路径
    /// </summary>
    private string m_UIModuleSavePath;
    public string UIModuleSavePath
    {
        get { return m_UIModuleSavePath; }
        set { m_UIModuleSavePath = value; }
    }

    /// <summary>
    /// 所有的界面信息
    /// </summary>
    private Dictionary<Type, UIModelData> m_AllModelDataDic;

    /// <summary>
    /// 当前显示的所有的界面
    /// </summary>
    private Dictionary<Type, UIBase> m_AllShowModule;

    /// <summary>
    /// 对象池显示的界面信息
    /// </summary>
    private List<UIBase> m_ShowPools;

    /// <summary>
    /// 显示在顶部的内容
    /// </summary>
    private List<UIBase> m_ShowTops;

    /// <summary>
    /// 显示序列
    /// </summary>
    private List<UIBase> m_ShowOrder;

    [SerializeField]
    private Transform m_Pnlts;

    [NonSerialized]
    private Transform m_Poolts;
    public Transform Poolts { get { return m_Poolts; } }

    [NonSerialized]
    private Transform m_Topts;

    [NonSerialized]
    private Transform m_Blackts;

    [NonSerialized]
    private Transform m_Roots;
    public Transform Root { get { return m_Roots; } }

    protected override void Awake()
    {
        base.Awake();

        m_AllModelDataDic = new Dictionary<Type, UIModelData>();
        m_AllModelDataDic.Clear();

        m_AllShowModule = new Dictionary<Type, UIBase>();
        m_AllShowModule.Clear();

        m_ShowPools = new List<UIBase>();
        m_ShowPools.Clear();

        m_ShowOrder = new List<UIBase>();
        m_ShowOrder.Clear();

        m_ShowTops = new List<UIBase>();
        m_ShowTops.Clear();

        m_UICamera = this.gameObject.GetComponent<Camera>();
    }

    /// <summary>
    /// 添加面板资源
    /// </summary>
    /// <param name="models"></param>
    public void AddModelData(List<UIModelData> models)
    {
        for (int index = 0; index < models.Count; index++)
        {
            AddModelData(models[index]);
        }
    }

    /// <summary>
    /// 添加面板资源
    /// </summary>
    /// <param name="model"></param>
    public void AddModelData(UIModelData model)
    {
        if (!m_AllModelDataDic.ContainsKey(model.m_Type))
        {
            m_AllModelDataDic.Add(model.m_Type, model);
        }
    }

    /// <summary>
    /// 打开界面
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arms"></param>
    public void OpenUI(Type type, params object[] arms)
    {
        OpenUIWithShowLayer(type, UILayer.None, arms);
    }

    /// <summary>
    /// 打开界面
    /// </summary>
    /// <param name="type"></param>
    /// <param name="layer"></param>
    /// <param name="armas"></param>
    public void OpenUIWithShowLayer(Type type, UILayer layer, params object[] arms)
    {
        UIModelData data;
        if (m_AllModelDataDic.TryGetValue(type, out data))
        {
            //打开关联界面     
            if (data.m_Relations != null)
            {
                foreach (Type t in data.m_Relations)
                {
                    if (m_AllModelDataDic.ContainsKey(t))
                    {
                        OpenUIP(t, default(UIModelData), layer, arms);
                    }
                }
            }

            //打开自己
            OpenUIP(type, data, layer, arms);
            //关闭隐藏别人
            if (data.m_IsOverlay)
            {
                List<Type> types = new List<Type>();
                if (data.m_Relations != null)
                {
                    types.AddRange(data.m_Relations);
                }
                List<Type> ins = new List<Type>();

                //将自己添加到排除列表当中
                ins.Add(type);
                if (data.m_Excludes != null)
                {
                    ins.AddRange(data.m_Excludes);
                }

                foreach (var item in m_AllShowModule)
                {
                    if (!ins.Contains(item.Value.UIType))
                    {
                        if (!types.Contains(item.Value.UIType))
                        {
                            if (!item.Value.m_DontOverLayer)
                            {
                                //先隐藏，后关闭
                                item.Value.SetModuleDisable();
                                item.Value.IsClose = true;
                            }
                        }
                        else
                        {
                            item.Value.SetModuleDisable();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 关闭UI
    /// </summary>
    /// <param name="type"></param>
    public void CloseUI(Type type)
    {
        UIBase ui = GetModule<UIBase>(type);
        if (ui != null)
        {
            CloseUI(ui);
        }
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    /// <param name="ui"></param>
    public void CloseUI(UIBase ui)
    {
        if (HasModule(ui))
        {
            UILayer showLayer = ui.m_ShowLayer;
            List<Type> types = GetAllInParent(ui);
            if (types.Count > 0)
            {
                for (int index = 0; index < types.Count; index++)
                {
                    if (m_AllShowModule.ContainsKey(types[index]))
                    {
                        UIBase parent = m_AllShowModule[types[index]];
                        RemoveInManager(parent, showLayer);
                        GameObject.Destroy(parent.gameObject);
                    }
                }
            }

            RemoveInManager(ui, showLayer);
            GameObject.Destroy(ui.gameObject);
            if (showLayer == UILayer.Black ||
                showLayer == UILayer.Dlg ||
                showLayer == UILayer.None ||
                showLayer == UILayer.Pnl)
            {
                OpenBackModule();
            }
        }
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="UILayer layer = UILayer.None"></param>
    /// <returns></returns>
    public T GetModule<T>(Type type, UILayer layer = UILayer.None) where T : UIBase
    {
        if (layer == UILayer.Top)
        {
            for (int index = 0; index < m_ShowTops.Count; index++)
            {
                if (m_ShowTops[index].UIType == type)
                {
                    return m_ShowTops[index] as T;
                }
            }
        }

        if (layer != UILayer.Pool)
        {
            if (m_AllShowModule.ContainsKey(type))
            {
                return m_AllShowModule[type] as T;
            }
        }

        return null;
    }

    /// <summary>
    /// 关闭所有面板
    /// </summary>
    public void CloseAll()
    {
        foreach (var item in m_AllShowModule)
        {
            item.Value.IsClose = true;
        }

        m_ShowOrder.Clear();
    }

    /// <summary>
    /// 根据鼠标位置获取世界坐标位置
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector3 GetPositionWithInputPosition(Vector2 position)
    {
        Vector3 view = UICamera.ScreenToViewportPoint(position);
        Vector3 word = UICamera.ViewportToWorldPoint(view);
        word.z = 0;
        return word;
    }

    /// <summary>
    /// 设置面板的父节点
    /// </summary>
    /// <param name="module"></param>
    private void SetModuleParent(UIBase module)
    {
        RectTransform rt = module.gameObject.GetComponent<RectTransform>();

        switch (module.m_ShowLayer)
        {
            case UILayer.None:
            case UILayer.Pnl:
            case UILayer.Dlg:
                rt.SetParent(m_Pnlts);
                m_AllShowModule.Add(module.UIType, module);
                m_ShowOrder.Insert(0, module);
                break;
            case UILayer.Pool:
                rt.SetParent(m_Poolts);
                m_ShowPools.Add(module);
                break;
            case UILayer.Top:
                rt.SetParent(m_Topts);
                m_ShowTops.Add(module);
                break;
            case UILayer.Black:
                rt.SetParent(m_Blackts);
                m_AllShowModule.Add(module.UIType, module);
                m_ShowOrder.Insert(0, module);
                break;
        }

        ///排到最后
        rt.SetAsLastSibling();
        rt.localPosition = Vector3.zero;
        rt.localEulerAngles = Vector3.zero;
        rt.localScale = Vector3.one;
        rt.sizeDelta = Vector2.zero;

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = Vector2.one * 0.5f;

        rt.gameObject.name = module.UIType.ToString();
    }

    /// <summary>
    /// 打开界面
    /// </summary>
    /// <param name="type"></param>
    /// <param name="layer"></param>
    /// <param name="arms"></param>
    private void OpenUIP(Type type, UIModelData data, UILayer layer, params object[] arms)
    {
        GameObject go;
        bool has = GetModuleObject(type, layer, out go);
        if (go != null)
        {
            go.SetActive(false);
            UIBase ui = go.GetComponent<UIBase>();
            if (ui == null) { return; }

            ui.InitData(type, layer, arms);
            if (!has)
            {
                SetModuleParent(ui);
            }
            else
            {
                if (!m_ShowOrder.Contains(ui))
                {
                    m_ShowOrder.Insert(0, ui);
                }
            }

            ui.OnShow();
        }
    }

    /// <summary>
    /// 得到界面
    /// </summary>
    /// <param name="type"></param>
    /// <param name="layer"></param>
    /// <param name="go"></param>
    /// <returns></returns>
    private bool GetModuleObject(Type type, UILayer layer, out GameObject go)
    {
        switch (layer)
        {
            case UILayer.Black:
            case UILayer.Dlg:
            case UILayer.Pnl:
            case UILayer.None:
                if (m_AllShowModule.ContainsKey(type))
                {
                    go = m_AllShowModule[type].gameObject;
                    return true;
                }
                else
                {
                    go = GameObject.Instantiate(Resources.Load(UIModuleSavePath + type.ToString()) as GameObject);
                    return false;
                }
            case UILayer.Pool:
                go = GameObject.Instantiate(Resources.Load(UIModuleSavePath + type.ToString()) as GameObject);
                return false;
            case UILayer.Top:
                for (int index = 0; index < m_ShowTops.Count; index++)
                {
                    if (m_ShowTops[index].UIType == type)
                    {
                        go = m_ShowTops[index].gameObject;
                        return true;
                    }
                }

                go = GameObject.Instantiate(Resources.Load(UIModuleSavePath + type.ToString()) as GameObject);
                return false;
        }

        go = null;
        return false;
    }

    /// <summary>
    /// 获取所有以这个界面为父界面的子界面
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private List<Type> GetAllInParent(UIBase type)
    {
        List<Type> frather = new List<Type>();
        switch (type.m_ShowLayer)
        {
            case UILayer.Black:
            case UILayer.Dlg:
            case UILayer.Pnl:
            case UILayer.None:
                foreach (var item in m_AllModelDataDic)
                {
                    if (item.Value.m_Relations != null)
                    {
                        List<Type> temp = new List<Type>();
                        temp.AddRange(item.Value.m_Relations);
                        if (temp.Contains(type.UIType))
                        {
                            frather.Add(item.Key);
                        }
                    }
                }
                break;
            case UILayer.Top:
                break;
            case UILayer.Pool:
                break;
        }

        return frather;
    }

    /// <summary>
    /// 判定面板是否存在
    /// </summary>
    /// <param name="module"></param>
    /// <returns></returns>
    private bool HasModule(UIBase module)
    {
        if (module.m_ShowLayer == UILayer.Pool)
        {
            return m_ShowPools.Contains(module);
        }

        if (module.m_ShowLayer == UILayer.Top)
        {
            return m_ShowTops.Contains(module);
        }

        return m_AllModelDataDic.ContainsKey(module.UIType);
    }

    /// <summary>
    /// 在管理类当中移除
    /// </summary>
    /// <param name="type"></param>
    /// <param name="showlayer"></param>
    private void RemoveInManager(UIBase type, UILayer showlayer)
    {
        if (showlayer == UILayer.Pool)
        {
            if (m_ShowPools.Contains(type))
            {
                m_ShowPools.Remove(type);
            }
        }
        else if (showlayer == UILayer.Top)
        {
            if (m_ShowTops.Contains(type))
            {
                m_ShowTops.Remove(type);
            }
        }
        else
        {
            //int i = -1;
            //for (int index = 0; index < m_ShowOrder.Count; index++)
            //{
            //	if (m_ShowOrder[index].Equals(type))
            //	{
            //		i = index;
            //		break;
            //	}
            //}

            //if (i >= 0 && i < m_ShowOrder.Count)
            //{
            //	m_ShowOrder.RemoveAt(i);
            //}

            if (m_ShowOrder.Contains(type))
            {
                m_ShowOrder.Remove(type);
            }

            if (m_AllShowModule.ContainsKey(type.UIType))
            {
                m_AllShowModule.Remove(type.UIType);
            }
        }
    }

    /// <summary>
    /// 打开回退界面
    /// </summary>
    private void OpenBackModule()
    {
        if (m_ShowOrder.Count > 0)
        {
            UIBase ui = m_ShowOrder[0];
            m_ShowOrder.RemoveAt(0);
            if (ui.IsClose)
            {
                OpenBackModule();
            }
            else
            {
                OpenUIWithShowLayer(ui.UIType, ui.m_ShowLayer);
            }
        }
    }
}
