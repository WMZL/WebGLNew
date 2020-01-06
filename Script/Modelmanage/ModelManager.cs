using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;
using HighlightingSystem;
using UnityEngine.UI;

public class ModelManager : MonoBehaviour
{
    #region 回调管理
    public class ResourceLoadCallbacak : IResourceLoadCallback
    {
        private ModelInfoData m_Target;
        private Action<ModelInfoData, ResourceInfo, GameObject> m_CallBackFun;

        public ResourceLoadCallbacak(ModelInfoData data, Action<ModelInfoData, ResourceInfo, GameObject> fun)
        {
            m_CallBackFun = fun;
            m_Target = data;
        }

        public override void HandleLoadCallback(ResourceInfo info, GameObject go)
        {
            if (m_CallBackFun != null)
            {
                m_CallBackFun(m_Target, info, go);
            }
        }

        public override int LoadCallbackPriority()
        {
            return 0;
        }
    }

    #endregion

    /// <summary>
    /// 添加一个新模型
    /// </summary>
    private string m_AddNewModel = "AddNewModel";
    /// <summary>
    /// 删除一个模型
    /// </summary>
    private string m_DeleteAModel = "DeleteAModel";
    /// <summary>
    /// 保存模型信息
    /// </summary>
    private string m_SavaAModelData = "SavaAModelData";
    /// <summary>
    /// 加载之前保存的模型
    /// </summary>
    private string m_LoadConfigObject = "LoadConfigObject";
    /// <summary>
    /// 设置传感器的显示隐藏
    /// </summary>
    private string m_SetSensorActive = "SetSensorActive";
    /// <summary>
    /// 传递当前选中的物体
    /// </summary>
    private string m_UpdateSelectID = "UpdateSelectID";
    /// <summary>
    /// 对象池名称         
    /// </summary>
    private string m_PoolName;
    /// <summary>
    /// 新增模型的坐标偏移
    /// </summary>
    private Vector3 m_Offset;
    /// <summary>
    /// 当前添加的物体
    /// </summary>
    private GameObject m_CurAddGameObject;
    /// <summary>
    /// 模型对象池
    /// </summary>
    private ModelPool m_ModelPool;
    /// <summary>
    /// 新物体的父节点
    /// </summary>
    private Transform m_NewModelParent;
    /// <summary>
    /// 新增物体字典
    /// </summary>
    private Dictionary<string, ModelInfo> m_NewModelDic;
    /// <summary>
    /// 传感器父节点
    /// </summary>
    private Dictionary<int, Transform> m_SensorParent;
    private void Start()
    {
        m_NewModelDic = new Dictionary<string, ModelInfo>();
        m_SensorParent = new Dictionary<int, Transform>();
        m_PoolName = "ModelPool";
        m_Offset = new Vector3(0, 10, 0);
        m_NewModelParent = new GameObject("NewModelPoolParent").transform;
        MessageManager.Instance.AddMessageEventListener(m_UpdateSelectID, UpdateSelsectObject);
        MessageManager.Instance.AddMessageEventListener(m_AddNewModel, AddANewModel);
        MessageManager.Instance.AddMessageEventListener(m_SavaAModelData, SaveModeolInfo);
        MessageManager.Instance.AddMessageEventListener(m_LoadConfigObject, LoadConfigObject);
        MessageManager.Instance.AddMessageEventListener(m_DeleteAModel, DeleteModel);
        MessageManager.Instance.AddMessageEventListener(m_SetSensorActive, SetSensorActive);
        SGGamePoolManager.Instance.PushControl<GameObject>(new ModelPool(m_PoolName), true);
        m_ModelPool = SGGamePoolManager.Instance.GetPoolControl<GameObject>(m_PoolName) as ModelPool;
    }

    /// <summary>
    /// 添加新模型，处理
    /// 对象池存储的唯一ID，是类型type字段
    /// </summary>
    private void AddANewModel(string key, params object[] arms)
    {
        int type = (int)arms[0];
        ModelInfoData infodata = new ModelInfoData();
        infodata.cmd = 10009;
        infodata.type = type;
        GameObject go = m_ModelPool.GetPoolsObject(type);
        if (go != null)
        {
            SetModeInfo(infodata, go);
        }
        else
        {
            ResourceLoadCallbacak cb = new ResourceLoadCallbacak(infodata, LoadModelCallback);
            ResourceManager.Instance.LoadResource(infodata.type.ToString(), ResourceType.Device, cb);
        }
    }

    /// <summary>
    /// 保存模型信息
    /// </summary>
    private void SaveModeolInfo(string key, params object[] arms)
    {
        ModelInfo mi = m_CurAddGameObject.GetComponent<ModelInfo>();
        mi.SavaModeInfo();
        string allmodeldata = JsonMapper.ToJson(mi.SelfData).ToString();
        //Debug.Log("当前选中物体名字：" + m_CurAddGameObject + allmodeldata + "发送给JS");
        ReadJavaInterface.Hello(allmodeldata);
    }

    /// <summary>
    /// 设置当前选中的物体
    /// </summary>
    private void UpdateSelsectObject(string key, params object[] parms)
    {
        string id = parms[0].ToString();
        m_CurAddGameObject = GenerateTreeNeed.Instance.AllGameObjectDic[id];
    }

    /// <summary>
    /// 加载之前保存的所有模型
    /// </summary>
    private void LoadConfigObject(string key, params object[] parms)
    {
        ModelInfoData realdata = parms[0] as ModelInfoData;
        if (realdata.isNew == 0)//旧模型加Collider
        {
            Debug.Log("旧模型加Collider");
            if (GenerateTreeNeed.Instance.AllGameObjectDic.ContainsKey(realdata.modelid))
            {
                GameObject oldobj = GenerateTreeNeed.Instance.AllGameObjectDic[realdata.modelid];
                oldobj.tag = "Device";
                BoxCollider bc = oldobj.GetComponent<BoxCollider>();
                if (!bc)
                {
                    bc = oldobj.AddComponent<BoxCollider>();
                    Vector3 center = new Vector3(float.Parse(realdata.colliderCenterX), float.Parse(realdata.colliderCenterY), float.Parse(realdata.colliderCenterZ));
                    bc.center = center;
                    Vector3 size = new Vector3(float.Parse(realdata.colliderSizeX), float.Parse(realdata.colliderSizeY), float.Parse(realdata.colliderSizeZ));
                    bc.size = size;
                }
                ModelInfo info = oldobj.GetComponent<ModelInfo>();
                if (!info)
                {
                    info = oldobj.AddComponent<ModelInfo>();
                    info.SelfData.cameraX = realdata.cameraX;
                    info.SelfData.cameraY = realdata.cameraY;
                    info.SelfData.cameraZ = realdata.cameraZ;
                    info.SelfData.cameraRotateX = realdata.cameraRotateX;
                    info.SelfData.cameraRotateY = realdata.cameraRotateY;
                    info.SelfData.cameraRotateZ = realdata.cameraRotateZ;
                    info.SelfData.isAdd = realdata.isAdd;
                    info.SelfData.isNew = realdata.isNew;
                }
            }
        }
        else
        {
            Debug.Log("加载之前保存的模型");
            GameObject go = m_ModelPool.GetPoolsObject(realdata.type);
            if (go != null)
            {
                SetModeInfo(realdata, go);
            }
            else
            {
                ResourceLoadCallbacak cb = new ResourceLoadCallbacak(realdata, LoadModelCallback);
                ResourceManager.Instance.LoadResource(realdata.type.ToString(), ResourceType.Device, cb);
            }
        }
    }

    /// <summary>
    /// 加载模型完成回调
    /// </summary>
    private void LoadModelCallback(ModelInfoData infodata, ResourceInfo info, GameObject go)
    {
        m_ModelPool.PushControlObject(infodata.type, go);
        GameObject newModel = m_ModelPool.GetPoolsObject(infodata.type);
        if (newModel == null)
        {
            Debug.LogError("无法取到对象池模型！");
        }

        SetModeInfo(infodata, newModel);
    }

    /// <summary>
    /// 设置添加模型的信息
    /// </summary>
    private void SetModeInfo(ModelInfoData infodata, GameObject go)
    {
        if (!m_SensorParent.ContainsKey(infodata.type))
        {
            GameObject tran = new GameObject(infodata.type.ToString());
            tran.transform.SetParent(m_NewModelParent);
            go.transform.SetParent(tran.transform);
            m_SensorParent.Add(infodata.type, tran.transform);
        }
        else
        {
            go.transform.SetParent(m_SensorParent[infodata.type]);
        }

        ModelInfo info = go.GetComponent<ModelInfo>();
        if (!info)
        {
            info = go.AddComponent<ModelInfo>();
        }
        if (info == null)
        {
            Debug.LogError("出现错误");
            return;
        }

        GenerateTreeNeed.SumCount++;
        go.name = GenerateTreeNeed.SumCount.ToString();
        string id = Guid.NewGuid().ToString("N");
        Debug.Log("随机成成的：" + id);
        string md5id = GameTools.Instance.GenerateMD5(go.name);
        if (!GenerateTreeNeed.Instance.AllGameObjectDic.ContainsKey(md5id))
        {
            GenerateTreeNeed.Instance.AllGameObjectDic.Add(md5id, go);
        }
        else
        {
            Debug.LogError("数据出错！");
        }
        if (!m_NewModelDic.ContainsKey(md5id))
        {
            m_NewModelDic.Add(md5id, info);
        }
        else
        {
            Debug.LogError("数据错误！");
        }

        m_CurAddGameObject = go;
        if (infodata.cmd == 10009)
        {
            go.transform.position = Vector3.zero + m_Offset;
            go.transform.localScale = Vector3.one;

            info.UpdateModelInfo(infodata);
            MessageManager.Instance.SendMessageEventNow("RecvLocateGameObject", md5id, true, Vector3.zero, Vector3.one, 0, false);
        }
        else
        {
            info.SetItemInfo(infodata);
        }
        SGizmoBehavior sb = go.GetComponent<SGizmoBehavior>();
        if (!sb)
        {
            if (infodata.isAdd == 0)
            {
                sb = go.AddComponent<SGizmoBehavior>();
            }
            else if (infodata.isAdd == 1)
            {
                DealBoard(infodata, go);
            }
        }
        go.SetActive(true);
    }

    /// <summary>
    /// 处理传感器上面的布告牌
    /// 设置信息
    /// </summary>
    private void DealBoard(ModelInfoData infodata, GameObject go)
    {
        Transform zuobiao = go.transform.Find("zuobiao");
        zuobiao.SetParent(go.transform.parent);
        zuobiao.transform.localPosition = go.transform.localPosition + new Vector3(0, 2, 0);
        //zuobiao.transform.localPosition = new Vector3(zuobiao.transform.localPosition.x, 10, zuobiao.transform.localPosition.z);
        zuobiao.transform.localScale = Vector3.one * 5;
        zuobiao.eulerAngles = new Vector3(-90, 0, 0);

        Text canvastext = zuobiao.Find("Canvas").GetComponentInChildren<Text>();
        //canvastext.text = infodata.devName + "\n";
        for (int i = 0; i < infodata.nodeList.Count; i++)
        {
            canvastext.text += infodata.nodeList[i].nodeName + "：" + infodata.nodeList[i].value + infodata.nodeList[i].unit + "\n";
            if (i == infodata.nodeList.Count)
            {
                canvastext.text = canvastext.text.Replace("\n", "");
            }
        }
        zuobiao.SetParent(go.transform);
        zuobiao.gameObject.SetActive(true);
    }

    /// <summary>
    /// 删除一个模型
    /// </summary>
    private void DeleteModel(string key, params object[] parms)
    {
        string id = parms[0].ToString();

        if (GenerateTreeNeed.Instance.AllGameObjectDic.ContainsKey(id))
        {
            GenerateTreeNeed.SumCount--;
            Highlighter h = GenerateTreeNeed.Instance.AllGameObjectDic[id].GetComponent<Highlighter>();
            if (h)
            {
                Destroy(h);
            }
            GenerateTreeNeed.Instance.AllGameObjectDic.Remove(id);
        }

        if (m_NewModelDic.ContainsKey(id))
        {
            ModelInfo info = m_NewModelDic[id];
            m_ModelPool.PushPoolObject(info.SelfData.type, info.gameObject);
            m_NewModelDic.Remove(id);
        }
        //TODO这里需要给JS发送消息,更新数据库
    }

    /// <summary>
    /// 设置传感器的显示隐藏
    /// </summary>
    private void SetSensorActive(string key, params object[] parms)
    {
        int type = (int)parms[0];
        bool isShow = (bool)parms[1];
        if (m_SensorParent.ContainsKey(type))
        {
            m_SensorParent[type].gameObject.SetActive(isShow);
        }
        else
        {
            Debug.Log("字典中不存在此类型");
        }
    }

    private void OnDestroy()
    {
        MessageManager.Instance.RemoveMessageEventListener(m_AddNewModel, AddANewModel);
        MessageManager.Instance.RemoveMessageEventListener(m_SavaAModelData, SaveModeolInfo);
        MessageManager.Instance.RemoveMessageEventListener(m_LoadConfigObject, LoadConfigObject);
        MessageManager.Instance.RemoveMessageEventListener(m_DeleteAModel, DeleteModel);
        MessageManager.Instance.RemoveMessageEventListener(m_SetSensorActive, SetSensorActive);
        MessageManager.Instance.RemoveMessageEventListener(m_UpdateSelectID, UpdateSelsectObject);
    }
}
