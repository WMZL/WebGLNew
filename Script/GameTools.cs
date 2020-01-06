using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System;

public class GameTools : SingletonClass<GameTools>
{
    /// <summary>
    /// 将索引值转化成颜色
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public Color HexToColor(string hex)
    {
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;
        float g = bg / 255f;
        float b = bb / 255f;
        return new Color(r, g, b);
    }

    /// <summary>
    /// 将颜色值转化成索引
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        return hex;
    }

    /// <summary>
    /// 获取MD5
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GenerateMD5(string name)
    {
        string filemd5 = null;
        try
        {
            MD5 md5 = MD5.Create();
            byte[] filebyte = Encoding.UTF8.GetBytes(name);
            byte[] md5filebyte = md5.ComputeHash(filebyte);
            filemd5 = BitConverter.ToString(md5filebyte).Replace("-", "").ToLower();
            //Debug.Log(filemd5);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Data);
        }
        return filemd5;
    }
}
