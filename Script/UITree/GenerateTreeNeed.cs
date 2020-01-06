using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class GenerateTreeNeed : Singleton<GenerateTreeNeed>
{
	/// <summary>
	/// 读取到的所有的树信息
	/// </summary>
	public List<BllTreeNodeInfo> m_NeedInfo;
	/// <summary>
	/// 根节点
	/// </summary>
	private GameObject m_RootGameObject;
	/// <summary>
	/// 存储数据
	/// </summary>
	public List<BllTreeNodeInfo> m_listinf;
	/// <summary>
	/// 存储所有的物体信息
	/// </summary>
	private Dictionary<string, GameObject> m_AllGameObjectDic = new Dictionary<string, GameObject>();
	public Dictionary<string, GameObject> AllGameObjectDic { get { return m_AllGameObjectDic; } }
	/// <summary>
	/// 哎，不得以为之
	/// </summary>
	public static int SumCount = 0;
	/// <summary>
	/// 接收消息修改模型的BoxCollider
	/// </summary>
	private string m_ModifyCollider = "ModifyCollider";
	/// <summary>
	/// 结束编辑
	/// </summary>
	private string m_GetColliderInfo = "GetColliderInfo";
	/// <summary>
	/// 当前编辑选择的物体
	/// </summary>
	private GameObject m_CurSelectObj;
	private void Start()
	{
		MessageManager.Instance.AddMessageEventListener(m_ModifyCollider, ModifyCollider);
		MessageManager.Instance.AddMessageEventListener(m_GetColliderInfo, EndEdit);
		m_RootGameObject = GameObject.Find("unitidxzqhb");
		CreatTree(m_RootGameObject);
		//StartCoroutine(GenerateData());
	}

	private void AddTreeNode(List<BllTreeNodeInfo> bllNodes,
								 BllTreeNodeInfo child,
								 BllTreeNodeInfo parent)
	{
		parent.Children.Add(child);

		List<BllTreeNodeInfo> findNodes = bllNodes.FindAll((tn) =>
		{
			return tn.TreeParentID.Equals(child.TreeID);
		});

		foreach (BllTreeNodeInfo tempNode in findNodes)
		{
			AddTreeNode(bllNodes, tempNode, child);
		}
		bllNodes.Remove(child);
	}

	#region 生成层级对象
	/// <summary>
	/// 所有激活的物体
	/// </summary>
	private List<HierarchyData> m_CurrentGenerateDatas = new List<HierarchyData>();
	/// <summary>
	/// 初始层级
	/// </summary>
	public int m_InitHierarchy { get { return 0; } }

	/// <summary>
	/// 构建层级面板数据
	/// </summary>
	/// <param name="gameObj"> 创建对象，包含子物体 </param>
	public void CreatTree(GameObject gameObj)
	{
		m_CurrentGenerateDatas.Clear();
		HierarchyData data = new HierarchyData();
		string id = GameTools.Instance.GenerateMD5(gameObj.name);
		data.CurID = id;
		data.ParentID = m_InitHierarchy.ToString();
		data.CurName = gameObj.name;
		m_CurrentGenerateDatas.Add(data);
		AllGameObjectDic.Add(id, gameObj);
		ChildTree(gameObj, m_InitHierarchy + 1);
	}

	/// <summary>
	///  添加子物体
	/// </summary>
	private void ChildTree(GameObject gameObj, int count)
	{
		int index = gameObj.transform.childCount;
		if (index > 0)
		{
			for (int i = 0; i < index; i++)
			{
				HierarchyData data = new HierarchyData();
				GameObject obj = gameObj.transform.GetChild(i).gameObject;
				if (obj.activeInHierarchy == true)
				{
					SumCount++;
					obj.name = obj.name + "_" + SumCount.ToString();
					string id = GameTools.Instance.GenerateMD5(obj.name);
					data.CurID = id;
					data.CurName = obj.name;
					data.ParentID = GameTools.Instance.GenerateMD5(gameObj.name);
					m_CurrentGenerateDatas.Add(data);
					if (!AllGameObjectDic.ContainsKey(id))
					{
						AllGameObjectDic.Add(id, obj);
					}
					else
					{
						Debug.LogError("出现相同的Key值！字典错误！");
					}

					if (obj.transform.childCount > 0)
					{
						ChildTree(obj, count + 1);
					}
				}
			}
		}
	}

	/// <summary>
	/// 将读取的UI树数据存储起来
	/// </summary>
	private List<BllTreeNodeInfo> GenerateHierarchy()
	{
		List<BllTreeNodeInfo> lstNodes = new List<BllTreeNodeInfo>();
		if (m_CurrentGenerateDatas != null && m_CurrentGenerateDatas.Count > 0)
		{
			for (int i = 0; i < m_CurrentGenerateDatas.Count; i++)
			{
				BllTreeNodeInfo blinfo = new BllTreeNodeInfo()
				{
					NodeName = m_CurrentGenerateDatas[i].CurName,
					TreeID = m_CurrentGenerateDatas[i].CurID,
					TreeParentID = m_CurrentGenerateDatas[i].ParentID.ToString(),
				};

				lstNodes.Add(blinfo);
			}
		}
		return lstNodes;
	}
	#endregion

	/// <summary>
	/// 点击获取某一物体的所有层级
	/// </summary>
	public string GenerateData()
	{
		//yield return new WaitForSeconds(1f);
		m_listinf = GenerateHierarchy();
		Debug.Log("Unity读取到树结构：" + m_listinf.Count);
		BllTreeNodeInfo topTreeNode = new BllTreeNodeInfo();
		///topNodes即为读取的树信息
		m_NeedInfo = m_listinf.FindAll((tn) =>
		{
			return tn.TreeParentID.Equals("0");
		});
		foreach (BllTreeNodeInfo tempNode in m_NeedInfo)
		{
			AddTreeNode(m_listinf, tempNode, topTreeNode);
		}

		JsonData AllJSData = JsonMapper.ToJson(topTreeNode);
		string AllStrData = AllJSData.ToString();

		return AllStrData;
		#region 测试本地存取
		//Debug.Log("所有的数据：" + AllStrData);

		//string path = Application.persistentDataPath;
		//if (Directory.Exists(path))
		//{
		//    Debug.Log("当前存储路径" + path);
		//}
		//FileStream file = new FileStream(path + "/Test.txt", FileMode.Create);
		//byte[] bts = System.Text.Encoding.UTF8.GetBytes(AllStrData);
		//file.Write(bts, 0, bts.Length);
		//Debug.Log("保存成功！");

		//if (file != null)
		//{
		//    file.Close();
		//}
		#endregion
	}

	#region 编辑Collider
	/// <summary>
	/// 修改模型的Collider
	/// </summary>
	private void ModifyCollider(string key, params object[] parms)
	{
		string id = parms[0].ToString();
		float cx = (float)parms[1];
		float cy = (float)parms[2];
		float cz = (float)parms[3];
		float sx = (float)parms[4];
		float sy = (float)parms[5];
		float sz = (float)parms[6];
		if (m_AllGameObjectDic.ContainsKey(id))
		{
			m_CurSelectObj = m_AllGameObjectDic[id];
			m_CurSelectObj.tag = "Device";
			//m_IsEdit = true;
			DrawCollider dc = m_CurSelectObj.GetComponent<DrawCollider>();
			if (!dc)
			{
				dc = m_CurSelectObj.AddComponent<DrawCollider>();
			}
			dc.enabled = true;
			BoxCollider bc = m_CurSelectObj.GetComponent<BoxCollider>();
			if (bc)
			{
				bc.center = new Vector3(cx, cy, cz);
				bc.size = new Vector3(sx, sy, sz);
			}
		}
	}

	/// <summary>
	/// 编辑结束
	/// </summary>
	private void EndEdit(string key, params object[] parms)
	{
		string id = parms[0].ToString();
		Debug.Log(id + " " + GameTools.Instance.GenerateMD5(m_CurSelectObj.name) + "是否一致");
		DrawCollider dc = m_CurSelectObj.GetComponent<DrawCollider>();
		if (dc)
		{
			dc.enabled = false;
		}
		BoxCollider bc = m_CurSelectObj.GetComponent<BoxCollider>();
		if (bc)
		{
			Transform cam = Camera.main.transform;
			ModelInfoData data = new ModelInfoData();
			data.cmd = (int)U3DSENGTOJSMSG.SENDOBJECTDATA;
			data.modelid = id;
			data.colliderCenterX = bc.center.x.ToString();
			data.colliderCenterY = bc.center.y.ToString();
			data.colliderCenterZ = bc.center.z.ToString();
			data.colliderSizeX = bc.size.x.ToString();
			data.colliderSizeY = bc.size.y.ToString();
			data.colliderSizeZ = bc.size.z.ToString();
			data.cameraX = cam.position.x.ToString();
			data.cameraY = cam.position.y.ToString();
			data.cameraZ = cam.position.z.ToString();
			data.cameraRotateX = cam.eulerAngles.x.ToString();
			data.cameraRotateY = cam.eulerAngles.y.ToString();
			data.cameraRotateZ = cam.eulerAngles.z.ToString();
			data.originalColor = "";
			data.selectedColor = GameTools.Instance.ColorToHex(Color.yellow);
			data.alarmColor = GameTools.Instance.ColorToHex(Color.red);
			JsonData jd = JsonMapper.ToJson(data);
			string strJSon = jd.ToString();
			ReadJavaInterface.Hello(strJSon);
		}
		else
		{
			Debug.Log(m_CurSelectObj + "没有collider,10011没有返回");
		}
	}
	#endregion
}
