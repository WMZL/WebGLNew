using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using LitJson;
using HighlightingSystem;

public class SelectObject : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            CheckClickItem();
        }
    }

    /// <summary>
    /// 检测是否点击到物体
    /// </summary>
    private void CheckClickItem()
    {
        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform != null)
            {
                if (hit.collider.gameObject.tag.Equals("Device"))
                {
                    Transform go = hit.collider.transform;
                    Debug.Log("点击到的物体名称：" + go.name);
                    string hitid = GameTools.Instance.GenerateMD5(go.name);
                    ModelInfo info = go.GetComponent<ModelInfo>();
                    //Debug.Log("是否是新增" + info.SelfData.isAdd);
                    if (info.SelfData.isAdd == 0)
                    {
                        return;//新增时候点击
                    }
                    if (info)
                    {
                        Vector3 campos = new Vector3(float.Parse(info.SelfData.cameraX), float.Parse(info.SelfData.cameraY), float.Parse(info.SelfData.cameraZ));
                        Vector3 camrotation = new Vector3(float.Parse(info.SelfData.cameraRotateX), float.Parse(info.SelfData.cameraRotateY), float.Parse(info.SelfData.cameraRotateZ));
                        MessageManager.Instance.SendMessageEventNow("RecvLocateGameObject", hitid, false, campos, camrotation, 0, false);
                    }
                    else
                    {
                        MessageManager.Instance.SendMessageEventNow("RecvLocateGameObject", hitid, true, Vector3.zero, Vector3.zero, 0, false);
                    }

                    #region 给js发送消息
                    SendJsonData sjd = new SendJsonData();
                    sjd.cmd = (int)U3DSENGTOJSMSG.CLICKMODEL;
                    sjd.id = hitid;
                    JsonData sendjson = JsonMapper.ToJson(sjd);
                    string sendjsonStr = sendjson.ToString();
                    ReadJavaInterface.Hello(sendjsonStr);
                    #endregion
                }
                if (hit.collider.gameObject.tag.Equals("Sensor"))
                {
                    Transform go = hit.collider.transform;
                    ModelInfo info = go.GetComponentInParent<ModelInfo>();

                    #region 给js发送消息
                    SendJsonData sjd = new SendJsonData();
                    sjd.cmd = (int)U3DSENGTOJSMSG.CLICKMODEL;
                    sjd.id = info.SelfData.modelid;
                    JsonData sendjson = JsonMapper.ToJson(sjd);
                    string sendjsonStr = sendjson.ToString();
                    ReadJavaInterface.Hello(sendjsonStr);
                    #endregion
                }
            }
        }
    }
}
