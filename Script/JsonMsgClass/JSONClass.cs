using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 点击模型给JS发送的json数据
/// </summary>
public class SendJsonData
{
	public int cmd;
	public string id;
}

public class LoadComplete
{
	public int cmd;
}

public class ModelInfoData
{
	public int cmd;
	public string modelid;
	public int isNew;
	public int isAdd;
	public int type;
	public string mapX;
	public string mapY;
	public string mapZ;
	public string rotateX;
	public string rotateY;
	public string rotateZ;
	public string scaleX;
	public string scaleY;
	public string scaleZ;
	public string cameraX;
	public string cameraY;
	public string cameraZ;
	public string cameraRotateX;
	public string cameraRotateY;
	public string cameraRotateZ;
	public string originalColor;
	public string selectedColor;
	public string alarmColor;
	public string colliderCenterX;
	public string colliderCenterY;
	public string colliderCenterZ;
	public string colliderSizeX;
	public string colliderSizeY;
	public string colliderSizeZ;
	public string devName;
	public List<CanvasInfo> nodeList;
	public class CanvasInfo
	{
		public string nodeName;
		public string value;
		public string unit;
	}
}

#region 消息枚举
public enum U3DSENGTOJSMSG
{
	/// <summary>
	/// 场景加载完成
	/// </summary>
	SCENELOADED = 10006,
	/// <summary>
	/// 给JS发送模型信息
	/// </summary>
	SENDOBJECTDATA = 10009,
	/// <summary>
	/// 点击模型发送
	/// </summary>
	CLICKMODEL = 10018,
}

public enum JSSENDTOU3D
{
	/// <summary>
	/// 切换视角
	/// </summary>
	SWITCHCAMERA = 10001,
	/// <summary>
	/// 重置视角
	/// </summary>
	RESETCAMERA = 10002,
	/// <summary>
	/// 导出树结构
	/// </summary>
	EXPORTTREE = 10004,
	/// <summary>
	/// 定位模型
	/// </summary>
	LOCATEGAMEOBJECT = 10007,
	/// <summary>
	/// 添加模型
	/// </summary>
	ADDMODEL = 10008,
	/// <summary>
	/// 添加模型保存
	/// </summary>
	ADDSAVE = 10010,
	/// <summary>
	/// 为了获取collider信息
	/// </summary>
	GETCOLLIDERINFO = 10011,
	/// <summary>
	/// 动态加载模型
	/// </summary>
	INSTANTIATEOBJECT = 10012,
	/// <summary>
	/// 删除指定模型
	/// </summary>
	DELETEAMODEL = 10013,
	/// <summary>
	/// 设置模型的属性
	/// </summary>
	SETMODELATTRIBUTES = 10016,
	/// <summary>
	/// 设置传感器的显示隐藏
	/// </summary>
	SETSENSORACTIVE = 10019,
}
#endregion

public class RecvJsonClass
{
	public int cmd;
	public string modelid;
	public int isNew;
	public int isAdd;
	public int type;
	public double mapX;
	public double mapY;
	public double mapZ;
	public double rotateX;
	public double rotateY;
	public double rotateZ;
	public double scaleX;
	public double scaleY;
	public double scaleZ;
	public double cameraX;
	public double cameraY;
	public double cameraZ;
	public double cameraRotateX;
	public double cameraRotateY;
	public double cameraRotateZ;
	public string originalColor;
	public string selectedColor;
	public string alarmColor;
	public double colliderCenterX;
	public double colliderCenterY;
	public double colliderCenterZ;
	public double colliderSizeX;
	public double colliderSizeY;
	public double colliderSizeZ;
	public string devName;
	public List<CanvasInfo> nodeList;
	public class CanvasInfo
	{
		public string nodeName;
		public string value;
		public string unit;
	}
}