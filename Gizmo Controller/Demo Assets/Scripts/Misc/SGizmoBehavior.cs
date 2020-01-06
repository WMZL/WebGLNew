
using UnityEngine;
using System.Collections;

/// <summary>
/// Gizmo行为
/// </summary>
public class SGizmoBehavior : MonoBehaviour
{
	#region 私有变量

	private GizmoController m_gizmoController;                  //Gizmo控制器

	#endregion

	#region 反射成员

	/// <summary>
	/// 初始化
	/// </summary>
	void Start()
	{
		m_gizmoController = GizmoController.Instance;
		m_gizmoController.Hide();
	}

	/// <summary>
	/// 鼠标点击，则显示gizmo
	/// </summary>
	void OnMouseDown()
	{
		if (m_gizmoController == null)
			return;

		//if (m_gizmoController.IsOverAxis())
		//    return;

		m_gizmoController.SetSelectedObject(transform);

		if (m_gizmoController.IsHidden())
		{
			m_gizmoController.Show(GIZMO_MODE.TRANSLATE);
		}
	}

	#endregion
}