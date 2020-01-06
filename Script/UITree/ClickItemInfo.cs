using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HighlightingSystem;

public class ClickItemInfo : MonoBehaviour
{
    /// <summary>
    /// 摄像机位置
    /// </summary>
    public Vector3 m_CamPos;
    /// <summary>
    /// 摄像机的旋转
    /// </summary>
    public Vector3 m_Rotation;
    /// <summary>
    /// 唯一的ID
    /// </summary>
    public string m_UniqueID;
    /// <summary>
    /// 物体原本的材质数组
    /// </summary>
    private MeshRenderer[] m_SelfMaterials;
    /// <summary>
    /// 拷贝出来用于还原的材质数组
    /// </summary>
    private Material[] m_Copy;
    /// <summary>
    /// 设置是否报警
    /// </summary>
    public bool m_IsOn;
    /// <summary>
    /// 用于计时
    /// </summary>
    private float temp;
    /// <summary>
    /// 报警闪烁的频率,可以外部调整
    /// </summary>
    public float gapTime = 0.3f;
    /// <summary>
    /// 报警材质
    /// </summary>
    public Material m_AlramMaterial;
    private bool IsDisplay = true;
    /// <summary>
    /// 设置摄像机的位置，用于视角切换
    /// </summary>
    public void MoveTo(string id)
    {
        if (id.Equals(m_UniqueID))
        {
            GameObject m_ThirdCam = Camera.main.gameObject;
            if (m_ThirdCam == null)
            {
                Debug.LogWarning("当前不是自由视角");
                return;
            }
            Quaternion m_InitCamRotate = m_ThirdCam.transform.rotation;
            m_ThirdCam.transform.localEulerAngles = m_InitCamRotate.eulerAngles;
            m_ThirdCam.transform.DOMove(m_CamPos, 2.5f).OnComplete(() =>
            {
                m_ThirdCam.transform.DORotate(m_Rotation, 2f);
            });
        }
    }
    /// <summary>
    /// 设置物体高亮
    /// </summary>
    public void SetHighLight(string id, Transform go)
    {
        if (!m_IsOn)
        {
            if (id.Equals(m_UniqueID))
            {
                #region 设置高亮
                if (go.GetComponent<Highlighter>())
                {
                    if (ClickItemManager.Instance.CurHlighter != null)
                    {
                        Highlighter[] hss = ClickItemManager.Instance.CurHlighter.GetComponentsInChildren<Highlighter>();
                        if (hss.Length > 0)
                        {
                            for (int i = 0; i < hss.Length; i++)
                            {
                                hss[i].constant = false;
                            }
                        }
                        ClickItemManager.Instance.CurHlighter.constant = false;
                    }

                    ClickItemManager.Instance.CurHlighter = go.GetComponent<Highlighter>();
                    Highlighter[] hs = go.GetComponentsInChildren<Highlighter>();
                    if (hs.Length > 0)
                    {
                        for (int i = 0; i < hs.Length; i++)
                        {
                            hs[i].constant = true;
                        }
                    }
                    ClickItemManager.Instance.CurHlighter.constant = true;
                }
                #endregion
            }
        }
    }

    #region 用于设置物体的报警闪烁
    private void Awake()
    {
        m_SelfMaterials = transform.GetComponentsInChildren<MeshRenderer>();
        m_Copy = new Material[m_SelfMaterials.Length];
        for (int i = 0; i < m_SelfMaterials.Length; i++)
        {
            m_Copy[i] = m_SelfMaterials[i].material;
        }
    }

    void Update()
    {
        if (m_IsOn)
        {
            Effect();
        }
    }

    /// <summary>
    /// 设置是否报警
    /// </summary>
    public void SetIsOn(int ID, string color)
    {
        if (ID == 1)
        {
            m_IsOn = true;
            m_AlramMaterial.color = GameTools.Instance.HexToColor(color);
        }
        else
        {
            m_IsOn = false;
            SetSelfMateria();
        }
    }

    /// <summary>
    /// 设置初始材质
    /// </summary>
    private void SetSelfMateria()
    {
        for (int i = 0; i < m_SelfMaterials.Length; i++)
        {
            m_SelfMaterials[i].material = m_Copy[i];
        }
    }

    private void Effect()
    {
        temp += Time.deltaTime;
        if (temp >= gapTime)
        {
            if (IsDisplay)
            {
                for (int i = 0; i < m_SelfMaterials.Length; i++)
                {
                    m_SelfMaterials[i].material = m_AlramMaterial;
                }
                IsDisplay = false;
                temp = 0;
            }
            else
            {
                for (int i = 0; i < m_SelfMaterials.Length; i++)
                {
                    m_SelfMaterials[i].material = m_Copy[i];
                }
                IsDisplay = true;
                temp = 0;
            }
        }
    }
    #endregion
}