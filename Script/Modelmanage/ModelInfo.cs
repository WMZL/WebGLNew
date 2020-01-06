using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

public class ModelInfo : MonoBehaviour
{
    private ModelInfoData m_SelfData;

    public ModelInfoData SelfData { get { return m_SelfData; } set { m_SelfData = value; } }

    private void Awake()
    {
        if (m_SelfData == null)
        {
            m_SelfData = new ModelInfoData();
        }
    }
    /// <summary>
    /// 更新数据
    /// </summary>
    public void UpdateModelInfo(ModelInfoData value)
    {
        SelfData = new ModelInfoData();
        Camera cam = Camera.main;
        SelfData.type = value.type;
        SelfData.modelid = GameTools.Instance.GenerateMD5(transform.name);
        SelfData.mapX = transform.position.x.ToString();
        SelfData.mapY = transform.position.y.ToString();
        SelfData.mapZ = transform.position.z.ToString();
        SelfData.rotateX = transform.eulerAngles.x.ToString();
        SelfData.rotateY = transform.eulerAngles.y.ToString();
        SelfData.rotateZ = transform.eulerAngles.z.ToString();
        SelfData.scaleX = transform.localScale.x.ToString();
        SelfData.scaleY = transform.localScale.y.ToString();
        SelfData.scaleZ = transform.localScale.z.ToString();
        SelfData.cameraX = cam.transform.position.x.ToString();
        SelfData.cameraY = cam.transform.position.y.ToString();
        SelfData.cameraZ = cam.transform.position.z.ToString();
        SelfData.cameraRotateX = cam.transform.eulerAngles.x.ToString();
        SelfData.cameraRotateY = cam.transform.eulerAngles.y.ToString();
        SelfData.cameraRotateZ = cam.transform.eulerAngles.z.ToString();
        SelfData.originalColor = GameTools.Instance.ColorToHex(transform.GetComponentInChildren<MeshRenderer>().material.color);
        Highlighter h = transform.GetComponent<Highlighter>();
        if (h)
        {
            SelfData.selectedColor = GameTools.Instance.ColorToHex(h.constantColor);
        }
        else
        {
            SelfData.selectedColor = GameTools.Instance.ColorToHex(Color.yellow);
        }

        SelfData.alarmColor = GameTools.Instance.ColorToHex(Color.red);
    }

    /// <summary>
    /// 保存信息
    /// </summary>
    public void SavaModeInfo()
    {
        Camera cam = Camera.main;
        SelfData.cmd = (int)U3DSENGTOJSMSG.SENDOBJECTDATA;
        SelfData.modelid = GameTools.Instance.GenerateMD5(transform.name);
        SelfData.mapX = transform.position.x.ToString();
        SelfData.mapY = transform.position.y.ToString();
        SelfData.mapZ = transform.position.z.ToString();
        SelfData.rotateX = transform.eulerAngles.x.ToString();
        SelfData.rotateY = transform.eulerAngles.y.ToString();
        SelfData.rotateZ = transform.eulerAngles.z.ToString();
        SelfData.scaleX = transform.localScale.x.ToString();
        SelfData.scaleY = transform.localScale.y.ToString();
        SelfData.scaleZ = transform.localScale.z.ToString();
        SelfData.cameraX = cam.transform.position.x.ToString();
        SelfData.cameraY = cam.transform.position.y.ToString();
        SelfData.cameraZ = cam.transform.position.z.ToString();
        SelfData.cameraRotateX = cam.transform.eulerAngles.x.ToString();
        SelfData.cameraRotateY = cam.transform.eulerAngles.y.ToString();
        SelfData.cameraRotateZ = cam.transform.eulerAngles.z.ToString();
        SelfData.originalColor = "";
        SelfData.selectedColor = GameTools.Instance.ColorToHex(Color.yellow);
        SelfData.alarmColor = GameTools.Instance.ColorToHex(Color.red);
        BoxCollider bc = transform.GetComponent<BoxCollider>();
        if (bc)
        {
            SelfData.colliderCenterX = bc.center.x.ToString();
            SelfData.colliderCenterY = bc.center.y.ToString();
            SelfData.colliderCenterZ = bc.center.z.ToString();
            SelfData.colliderSizeX = bc.size.x.ToString();
            SelfData.colliderSizeY = bc.size.y.ToString();
            SelfData.colliderSizeZ = bc.size.z.ToString();
        }
        else
        {
            Debug.Log("没有Collider信息");
        }
    }

    /// <summary>
    /// 根据传值设置属性值
    /// </summary>
    public void SetItemInfo(ModelInfoData infodata)
    {
        m_SelfData = infodata;
        transform.position = new Vector3(float.Parse(infodata.mapX), float.Parse(infodata.mapY), float.Parse(infodata.mapZ));
        transform.eulerAngles = new Vector3(float.Parse(infodata.rotateX), float.Parse(infodata.rotateY), float.Parse(infodata.rotateZ));
        transform.localScale = new Vector3(float.Parse(infodata.scaleX), float.Parse(infodata.scaleY), float.Parse(infodata.scaleZ));
    }

    /// <summary>
    /// 清空数据
    /// </summary>
    public void ClearData()
    {
        m_SelfData = null;
    }
}
