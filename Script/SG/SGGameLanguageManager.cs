using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGGameLanguageManager : SingletonClass<SGGameLanguageManager>
{
    private Dictionary<string, string> m_AllKeyDesc;

    public SGGameLanguageManager()
    {
        m_AllKeyDesc = new Dictionary<string, string>();
        m_AllKeyDesc.Clear();
    }

    /// <summary>
    /// 获取字典数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetDescWithKey(string key)
    {
        if (m_AllKeyDesc.ContainsKey(key))
        {
            return m_AllKeyDesc[key];
        }

        return null;
    }

    /// <summary>
    /// 添加内容
    /// </summary>
    /// <param name="key"></param>
    /// <param name="desc"></param>
    public void AddKeyDesc(string key, string desc)
    {
        if (m_AllKeyDesc.ContainsKey(key))
        {
            Debug.LogWarningFormat("the key【{0}】 has two desc【{1}】【{2}】",
                            key, m_AllKeyDesc[key], desc);
            return;
        }

        m_AllKeyDesc.Add(key, desc);
    }

    /// <summary>
    /// 移除内容
    /// </summary>
    /// <param name="key"></param>
    public void RemoveKey(string key)
    {
        if (m_AllKeyDesc.ContainsKey(key))
        {
            m_AllKeyDesc.Remove(key);
        }
    }

    /// <summary>
    /// 清除所有数据
    /// </summary>
    public void ClearData()
    {
        m_AllKeyDesc.Clear();
    }
}
