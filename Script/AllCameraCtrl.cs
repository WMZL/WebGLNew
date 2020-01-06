using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AllCameraCtrl : Singleton<AllCameraCtrl>
{
    /// <summary>
    /// 第三人称视角摄像机
    /// </summary>
    public GameObject m_ThirdCamera;
    /// <summary>
    /// 第一人称视角摄像机
    /// </summary>
    public GameObject m_FirstCamera;
    /// <summary>
    /// 第一人称摄像机位置
    /// </summary>
    private Vector3 m_FirstInitPos;
    /// <summary>
    /// 表示当前摄像机
    /// </summary>
    //private int m_CurCamera = 3;
    /// <summary>
    /// 当前摄像机的位置
    /// </summary>
    private Vector3 m_CurCamPosition;
    /// <summary>
    /// 第一人称摄像机不动
    /// </summary>
    public GameObject m_ForFirst;

    private void Start()
    {
        //m_FirstInitPos = new Vector3(-214, 90, 16);
        //StartThirdCamera();
    }

    /// <summary>
    /// 切换到第三人称
    /// 初始值和频繁切换的值
    /// </summary>
    private void StartThirdCamera()
    {
        m_CurCamPosition = m_FirstCamera.transform.position;
        //m_CurCamera = 3;
        m_ForFirst.SetActive(false);
        m_ThirdCamera.SetActive(true);
        m_FirstCamera.SetActive(false);
        m_ThirdCamera.transform.position = m_CurCamPosition;
    }

    /// <summary>
    /// 切换到第一人称
    /// </summary>
    private void StartFirstCamera()
    {
        m_CurCamPosition = m_ThirdCamera.transform.position;
        //m_CurCamera = 1;
        m_ForFirst.SetActive(true);
        m_FirstCamera.SetActive(true);
        m_ThirdCamera.SetActive(false);
        m_FirstCamera.transform.position = m_CurCamPosition;
    }

    /// <summary>
    /// 切换摄像机
    /// </summary>
    public void Switch(int viewtype)
    {
        if (viewtype == 3)
        {
            StartThirdCamera();
        }
        else if (viewtype == 1)
        {
            StartFirstCamera();
        }
    }

    /// <summary>
    /// 点击重置
    /// </summary>
    public void ResetButton()
    {
        //m_CurCamera = 3;
        StartThirdCamera();
        Vector3 InitCamPos = new Vector3(-214, 90, 16);
        m_ThirdCamera.transform.position = InitCamPos;
        m_ThirdCamera.transform.localEulerAngles = new Vector3(44, 90, 0);
        //m_ThirdCamera.transform.DOMove(InitCamPos, 2.5f).OnComplete(() =>
        //{
        //    m_ThirdCamera.transform.DORotate(new Vector3(44, 90, 0), 2f);
        //});
    }
}