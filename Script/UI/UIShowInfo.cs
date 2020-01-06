using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShowInfo : UIBase
{
    /// <summary>
    /// 当前名称
    /// </summary>
    public Text m_CurName;
    /// <summary>
    /// 数据来源
    /// </summary>
    public Text m_DataSource;
    /// <summary>
    /// 识别结果
    /// </summary>
    public Text m_Result;
    /// <summary>
    /// 报警等级
    /// </summary>
    public Text m_AlrameLv;

    public override bool InitData(Type type, UILayer layer, params object[] arms)
    {
        if (!base.InitData(type, layer, arms))
            return false;
        //if (arms.Length>0)
        //{
        //    m_ShowInfo = (TestClass)arms[0];
        //    m_CurName.text = m_ShowInfo.nodefullname;
        //    m_DataSource.text = m_ShowInfo.source;
        //    m_Result.text = m_ShowInfo.tasksresult;
        //    m_AlrameLv.text = m_ShowInfo.alarmlevel;
        //}

        return true;
    }

    public void Close()
    {
        UIManager.Instance.CloseUI(this);
    }
}
