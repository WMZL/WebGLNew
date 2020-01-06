using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

public class ReadJavaInterface : Singleton<ReadJavaInterface>
{
    [DllImport("__Internal")]
    public static extern void Hello(string str);

    /// <summary>
    /// 传递的JSON数据
    /// </summary>
    private JsonData m_RecvJsonData;

    /// <summary>
    /// 接收的String
    /// </summary>
    private string m_RecvJsonStr;

    /// <summary>
    /// 站点的的唯一ID
    /// </summary>
    public const string m_Unitid = "unitidxzqhb";

    /// <summary>
    /// 接收JS传递的消息进行分发处理
    /// </summary>
    /// <param name="message"></param>
    private void RecvJSMessage(string message)
    {
        Debug.Log("Unity接收到JS消息" + message);
        if (message == null)
        {
            Debug.LogError("消息错误");
            return;
        }
        try
        {
            m_RecvJsonStr = message;
            m_RecvJsonData = JsonMapper.ToObject(message);
            int cmd = (int)m_RecvJsonData["cmd"];
            switch (cmd)
            {
                case (int)JSSENDTOU3D.SWITCHCAMERA:
                    RecvJSSwitchCamera();
                    break;
                case (int)JSSENDTOU3D.RESETCAMERA:
                    RecvJSReset();
                    break;
                case (int)JSSENDTOU3D.EXPORTTREE:
                    RecvJSExportTree();
                    break;
                case (int)JSSENDTOU3D.LOCATEGAMEOBJECT:
                    RecvJSLocateGameObject();
                    break;
                case (int)JSSENDTOU3D.ADDMODEL:
                    //RecvJSAddNewModel();
                    break;
                case (int)JSSENDTOU3D.ADDSAVE:
                    SaveModeleData();
                    break;
                case (int)JSSENDTOU3D.INSTANTIATEOBJECT:
                    LoadConfigObjects();
                    break;
                case (int)JSSENDTOU3D.DELETEAMODEL:
                    DeleteAModel();
                    break;
                case (int)JSSENDTOU3D.GETCOLLIDERINFO:
                    GetObjectCollider();
                    break;
                case (int)JSSENDTOU3D.SETMODELATTRIBUTES:
                    SetModelCollider();
                    break;
                case (int)JSSENDTOU3D.SETSENSORACTIVE:
                    SetSensorActive();
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("数据解析格式错误" + e.Data);
        }
    }

    #region 根据JS命令处理分发消息，方法定义为Public便于测试，后面需要更改回来
    /// <summary>
    /// 接收消息用于重置视角
    /// </summary>
    public void RecvJSReset()
    {
        AllCameraCtrl.Instance.ResetButton();
    }

    /// <summary>
    /// 接收JS消息用于切换视角
    /// </summary>
    public void RecvJSSwitchCamera()
    {
        int vietype = (int)m_RecvJsonData["viewType"];
        AllCameraCtrl.Instance.Switch(vietype);
    }

    /// <summary>
    /// 接收命令导出Unity中设备树
    /// </summary>
    public void RecvJSExportTree()
    {
        string jsonSendToJS = GenerateTreeNeed.Instance.GenerateData();
        Hello(jsonSendToJS);
    }

    /// <summary>
    /// 接收消息定位模型
    /// </summary>
    public void RecvJSLocateGameObject()
    {
        //JsonData m_RecvJsonData = JsonMapper.ToObject(str);
        string id = m_RecvJsonData["id"].ToString();
        bool isDefault = (bool)m_RecvJsonData["default"];
        float CameraPosx = float.Parse(m_RecvJsonData["cameraX"].ToString());
        float CameraPosy = float.Parse(m_RecvJsonData["cameraY"].ToString());
        float CameraPosz = float.Parse(m_RecvJsonData["cameraZ"].ToString());
        Vector3 pos = new Vector3(CameraPosx, CameraPosy, CameraPosz);
        float CameraRotatex = float.Parse(m_RecvJsonData["cameraRotateX"].ToString());
        float CameraRotatey = float.Parse(m_RecvJsonData["cameraRotateY"].ToString());
        float CameraRotatez = float.Parse(m_RecvJsonData["cameraRotateZ"].ToString());
        Vector3 rotate = new Vector3(CameraRotatex, CameraRotatey, CameraRotatez);
        int alarmLevel = (int)m_RecvJsonData["alarmLevel"];
        bool isAddCollider = (bool)m_RecvJsonData["isCollider"];
        MessageManager.Instance.SendMessageEventNow("RecvLocateGameObject", id, isDefault, pos, rotate, alarmLevel, isAddCollider);
    }

    /// <summary>
    /// 添加新的模型
    /// </summary>
    public void RecvJSAddNewModel(int type)
    {
        //int type = (int)m_RecvJsonData["type"];
        MessageManager.Instance.SendMessageEventNow("AddNewModel", type);
    }

    /// <summary>
    /// 保存新添加的模型信息，发送给JS
    /// </summary>
    public void SaveModeleData()
    {
        MessageManager.Instance.SendMessageEventNow("SavaAModelData");
    }

    /// <summary>
    /// 根据JS从数据库读取的数据动态加载模型
    /// </summary>
    public void LoadConfigObject(string test)
    {
        JsonData m_RecvJsonData = JsonMapper.ToObject(test);
        ModelInfoData infodata = new ModelInfoData();
        infodata.modelid = m_RecvJsonData["modelid"].ToString();
        infodata.isNew = (int)m_RecvJsonData["isNew"];
        infodata.isAdd = (int)m_RecvJsonData["isAdd"];
        infodata.cameraX = m_RecvJsonData["cameraX"].ToString();
        infodata.cameraY = m_RecvJsonData["cameraY"].ToString();
        infodata.cameraZ = m_RecvJsonData["cameraZ"].ToString();
        infodata.cameraRotateX = m_RecvJsonData["cameraRotateX"].ToString();
        infodata.cameraRotateY = m_RecvJsonData["cameraRotateY"].ToString();
        infodata.cameraRotateZ = m_RecvJsonData["cameraRotateZ"].ToString();
        infodata.colliderCenterX = m_RecvJsonData["colliderCenterX"].ToString();
        infodata.colliderCenterY = m_RecvJsonData["colliderCenterY"].ToString();
        infodata.colliderCenterZ = m_RecvJsonData["colliderCenterZ"].ToString();
        infodata.colliderSizeX = m_RecvJsonData["colliderSizeX"].ToString();
        infodata.colliderSizeY = m_RecvJsonData["colliderSizeY"].ToString();
        infodata.colliderSizeZ = m_RecvJsonData["colliderSizeZ"].ToString();
        if (infodata.isNew == 0)
        {
            MessageManager.Instance.SendMessageEventNow("LoadConfigObject", infodata);
            return;
        }

        infodata.type = (int)m_RecvJsonData["type"];
        infodata.mapX = m_RecvJsonData["mapX"].ToString();
        infodata.mapY = m_RecvJsonData["mapY"].ToString();
        infodata.mapZ = m_RecvJsonData["mapZ"].ToString();
        infodata.rotateX = m_RecvJsonData["rotateX"].ToString();
        infodata.rotateY = m_RecvJsonData["rotateY"].ToString();
        infodata.rotateZ = m_RecvJsonData["rotateZ"].ToString();
        infodata.scaleX = m_RecvJsonData["scaleX"].ToString();
        infodata.scaleY = m_RecvJsonData["scaleY"].ToString();
        infodata.scaleZ = m_RecvJsonData["scaleZ"].ToString();
        infodata.originalColor = m_RecvJsonData["originalColor"].ToString();
        infodata.selectedColor = m_RecvJsonData["selectedColor"].ToString();
        infodata.alarmColor = m_RecvJsonData["alarmColor"].ToString();

        MessageManager.Instance.SendMessageEventNow("LoadConfigObject", infodata);
    }

    /// <summary>
    /// 获取模型的collider
    /// </summary>
    public void GetObjectCollider()
    {
        string id = m_RecvJsonData["modelid"].ToString();
        MessageManager.Instance.SendMessageEventNow("GetColliderInfo", id);
    }

    /// <summary>
    /// 删除一个指定模型
    /// </summary>
    public void DeleteAModel()
    {
        string id = m_RecvJsonData["id"].ToString();
        MessageManager.Instance.SendMessageEventNow("DeleteAModel", id);
    }

    /// <summary>
    /// 设置调节模型的Collider
    /// </summary>
    public void SetModelCollider()
    {
        string id = m_RecvJsonData["modelid"].ToString();
        float cx = (float)m_RecvJsonData["data"]["centerX"];
        float cy = (float)m_RecvJsonData["data"]["centerY"];
        float cz = (float)m_RecvJsonData["data"]["centerZ"];
        float sx = (float)m_RecvJsonData["data"]["sizeX"];
        float sy = (float)m_RecvJsonData["data"]["sizeY"];
        float sz = (float)m_RecvJsonData["data"]["sizeZ"];
        MessageManager.Instance.SendMessageEventNow("ModifyCollider", id, cx, cy, cz, sx, sy, sz);
    }

    /// <summary>
    /// 指定传感器显示隐藏
    /// </summary>
    public void SetSensorActive()
    {
        //string[] aa = testmsg.Split('|');
        //int type = int.Parse(aa[0]);
        //bool isShow = bool.Parse(aa[1]);
        int type = (int)m_RecvJsonData["type"];
        bool isShow = (bool)m_RecvJsonData["isShow"];
        MessageManager.Instance.SendMessageEventNow("SetSensorActive", type, isShow);
    }
    #endregion

    /// <summary>
    /// 切换到观察传感器视角(暂未使用)
    /// </summary>
    public void SwitchSensor()
    {
        AllCameraCtrl.Instance.Switch(3);
        GameObject m_ThirdCam = Camera.main.gameObject;
        m_ThirdCam.transform.position = new Vector3(-173, 14, -31);
        m_ThirdCam.transform.localEulerAngles = new Vector3(28, 90, 0);
    }

    public void LoadConfigObjects()
    {
        RecvJsonClass recvData = JsonMapper.ToObject<RecvJsonClass>(m_RecvJsonStr);
        ModelInfoData infodata = new ModelInfoData();
        #region 原有的数值
        infodata.modelid = recvData.modelid;
        infodata.isNew = recvData.isNew;
        infodata.isAdd = recvData.isAdd;
        infodata.cameraX = recvData.cameraX.ToString();
        infodata.cameraY = recvData.cameraY.ToString();
        infodata.cameraZ = recvData.cameraZ.ToString();
        infodata.cameraRotateX = recvData.cameraRotateX.ToString();
        infodata.cameraRotateY = recvData.cameraRotateY.ToString();
        infodata.cameraRotateZ = recvData.cameraRotateZ.ToString();
        infodata.colliderCenterX = recvData.colliderCenterX.ToString();
        infodata.colliderCenterY = recvData.colliderCenterY.ToString();
        infodata.colliderCenterZ = recvData.colliderCenterZ.ToString();
        infodata.colliderSizeX = recvData.colliderCenterX.ToString();
        infodata.colliderSizeY = recvData.colliderCenterY.ToString();
        infodata.colliderSizeZ = recvData.colliderCenterZ.ToString();
        if (infodata.isNew == 0)
        {
            MessageManager.Instance.SendMessageEventNow("LoadConfigObject", infodata);
            return;
        }
        infodata.type = recvData.type;
        infodata.mapX = recvData.mapX.ToString();
        infodata.mapY = recvData.mapY.ToString();
        infodata.mapZ = recvData.mapZ.ToString();
        infodata.rotateX = recvData.rotateX.ToString();
        infodata.rotateY = recvData.rotateY.ToString();
        infodata.rotateZ = recvData.rotateZ.ToString();
        infodata.scaleX = recvData.scaleX.ToString();
        infodata.scaleY = recvData.scaleY.ToString();
        infodata.scaleZ = recvData.scaleZ.ToString();
        infodata.originalColor = recvData.originalColor;
        infodata.selectedColor = recvData.selectedColor;
        infodata.alarmColor = recvData.alarmColor;
        infodata.cmd = recvData.cmd;
        infodata.devName = recvData.devName;

        infodata.nodeList = new List<ModelInfoData.CanvasInfo>();
        for (int i = 0; i < recvData.nodeList.Count; i++)
        {
            ModelInfoData.CanvasInfo canvasinfo = new ModelInfoData.CanvasInfo();
            canvasinfo.nodeName = recvData.nodeList[i].nodeName;
            canvasinfo.unit = recvData.nodeList[i].unit;
            canvasinfo.value = recvData.nodeList[i].value;
            infodata.nodeList.Add(canvasinfo);
        }

        MessageManager.Instance.SendMessageEventNow("LoadConfigObject", infodata);
        #endregion
    }
}