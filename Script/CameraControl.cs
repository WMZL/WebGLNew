using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using HighlightingSystem;

public class CameraControl : MonoBehaviour
{
	#region 摄像机控制
	//滚滚轮放大缩小速度
	public float mouseScrollSpeed = 40;
	//单单按右键旋转的速度
	public float mouseDragSpeed1 = 40;
	//按中键移动的速度
	public float mouseDragSpeed2 = 1.26f;
	//“上一次”光标的位置
	public Vector3 mouseLastPosition = new Vector3(0, 0, 0);
	//光标位置变化量，等于现在光标位置减上一次光标的位置
	public Vector3 mousePositionDelta = new Vector3(0, 0, 0);
	//单单按鼠标右键旋转相机的速度
	private Vector3 rotateDelta = new Vector3(0, 0, 0);
	//public Texture2D m_texture2D1, m_texture2D2, m_texture2D3, m_texture2D4;//光标图案
	/// <summary>
	/// 接收默认定位消息
	/// </summary>
	private string m_RecvLocateGameObject = "RecvLocateGameObject";
	/// <summary>
	/// 默认偏移位置
	/// </summary>
	private Vector3 m_Offset;
	/// <summary>
	/// 当前选中，高亮显示
	/// </summary>
	private Highlighter m_CurHlighter;

	private void Start()
	{
		MessageManager.Instance.AddMessageEventListener(m_RecvLocateGameObject, LocateGameObject);
		m_Offset = new Vector3(0, 0, 10);
	}

	private void FixedUpdate()
	{
		MouseEvents();
	}

	private void MouseEvents()
	{
		//按住鼠标右键拖动
		if (Input.GetMouseButton(1))
		{
			mousePositionDelta = Input.mousePosition - mouseLastPosition;
			mouseLastPosition = Input.mousePosition;
			if (mousePositionDelta.magnitude != 0)
			{
				rotateDelta = new Vector3(-mousePositionDelta.y * Time.deltaTime * mouseDragSpeed1,
				mousePositionDelta.x * Time.deltaTime * mouseDragSpeed1, 0);
				//按Alt+鼠标右键
				if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
				{
					this.transform.Translate(new Vector3(0, 0, Time.deltaTime * mouseScrollSpeed * (rotateDelta.x + rotateDelta.y)), Space.Self);
					//Cursor.SetCursor(m_texture2D4, new Vector2(5, 5), CursorMode.ForceSoftware);
				}
				//单单按鼠标右键
				else
				{
					transform.eulerAngles += rotateDelta;
					//Cursor.SetCursor(m_texture2D1, new Vector2(5, 5), CursorMode.ForceSoftware);
				}
			}
		}
		//按住鼠标中键拖动
		else if (Input.GetMouseButton(2))
		{
			mousePositionDelta = Input.mousePosition - mouseLastPosition;
			mouseLastPosition = Input.mousePosition;
			if (mousePositionDelta.magnitude != 0)
			{
				rotateDelta = new Vector3(-mousePositionDelta.x * Time.deltaTime * mouseDragSpeed2,
				  -mousePositionDelta.y * Time.deltaTime * mouseDragSpeed2, 0);
				transform.Translate(rotateDelta, Space.Self);

				//Cursor.SetCursor(m_texture2D3, new Vector2(5, 5), CursorMode.ForceSoftware);
			}
		}
		//如果没有按鼠标除了滚滚轮的键，则实时更新光标“上一次”的位置mouseLastPosition
		else
		{
			mouseLastPosition = Input.mousePosition;//重新赋初值
													//Cursor.SetCursor(m_texture2D2, new Vector2(5, 5), CursorMode.ForceSoftware);//更改鼠标图标
		}
		//滚轮——放大缩小
		if (Input.mouseScrollDelta.y != 0)
		{
			//滚轮滚了多少
			transform.Translate(new Vector3(0, 0, Time.deltaTime * mouseScrollSpeed * Input.mouseScrollDelta.y), Space.Self);
		}
	}

	#endregion

	/// <summary>
	///定位某一个物体,此方法中对模型的操作，应转移到别的模块去做，减少耦合
	/// </summary>
	private void LocateGameObject(string key, params object[] arms)
	{
		Transform targetTran = null;
		string objectId = arms[0].ToString();
		bool isdefault = (bool)arms[1];
		Vector3 pos = (Vector3)(arms[2]);
		Vector3 rotate = (Vector3)(arms[3]);
		int alarmLevel = (int)arms[4];
		bool isAddCollider = (bool)arms[5];

		if (GenerateTreeNeed.Instance.AllGameObjectDic.ContainsKey(objectId))
		{
			targetTran = GenerateTreeNeed.Instance.AllGameObjectDic[objectId].transform;
		}
		else
		{
			Debug.LogError("传入ID有误！");
			return;
		}

		//默认观察视角
		if (isdefault)
		{
			transform.DOMove(targetTran.position - m_Offset, 2.5f).OnComplete(() =>
			{
				transform.DORotate(transform.forward, 2f);
			});
		}
		//读取发送的指定配置视角
		else
		{
			transform.transform.DOMove(pos, 2.5f).OnComplete(() =>
			{
				transform.transform.DORotate(rotate, 2f);
			});
		}

		if (alarmLevel > 0)
		{
			Debug.LogError("有设备需要报警，后续处理");
		}

		if (isAddCollider)
		{
			if (targetTran != null)
			{
				BoxCollider bc = targetTran.GetComponent<BoxCollider>();
				if (!bc)
				{
					bc = targetTran.gameObject.AddComponent<BoxCollider>();
				}
				else
				{
					Debug.Log("当前物体已经添加过Collider!");
				}
			}
			else
			{
				Debug.Log("当前物体未加入字典！");
			}
		}

		if (m_CurHlighter)
		{
			if (targetTran.name == m_CurHlighter.transform.name)
			{
				return;
			}
			Highlighter h = m_CurHlighter.gameObject.GetComponent<Highlighter>();
			DestroyImmediate(h);
		}
		m_CurHlighter = targetTran.gameObject.AddComponent<Highlighter>();
		m_CurHlighter.constant = true;
	}

	private void OnDestroy()
	{
		MessageManager.Instance.RemoveMessageEventListener(m_RecvLocateGameObject, LocateGameObject);
	}
}
