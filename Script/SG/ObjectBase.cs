using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物体状态
/// </summary>
public enum ObjectStage
{
    None = 0,

    /// <summary>
    /// 显示状态
    /// </summary>
    Show = 1 << 1,

    /// <summary>
    /// 不显示状态
    /// </summary>
    Disable = 1 << 2,

    /// <summary>
    /// 销毁状态
    /// </summary>
    Destroy = 1 << 3
}

/// <summary>
/// 物体的基类
/// </summary>
public class ObjectBase : MonoBehaviour
{
    /// <summary>
    /// 物体类型
    /// </summary>
    protected int m_ObjectType;
    public int ObjectType
    {
        get { return m_ObjectType; }
        set { m_ObjectType = value; }
    }

    /// <summary>
    /// 物体名字
    /// </summary>
    protected string m_ObjectName;
    public string ObjectName
    {
        get { return m_ObjectName; }
        set { m_ObjectName = value; }
    }

    /// <summary>
    /// 物体状态
    /// </summary>
    protected ObjectStage m_ObjectStage;
    public ObjectStage Stage { get { return m_ObjectStage; } }

    /// <summary>
    /// 物体的标识
    /// 这个ID内容是表格当中的内容
    /// </summary>
    protected int m_ObjectID;
    public int ID
    {
        get { return m_ObjectID; }
        set { m_ObjectID = value; }
    }

    /// <summary>
    /// 物体唯一标志
    /// 这个ID内容是服务器下发下来给客户端的
    /// </summary>
    protected int m_ObjectUID;
    public int UID
    {
        get { return m_ObjectUID; }
        set { m_ObjectUID = value; }
    }

    protected int m_PoolID;
    public int PoolID
    {
        get { return m_PoolID; }
        set { m_PoolID = value; }
    }

    /// <summary>
    /// 掉落ID
    /// </summary>
    protected int m_ObjectPID;
    public int PID
    {
        get { return m_ObjectPID; }
        set { m_ObjectPID = value; }
    }

    /// <summary>
    /// 物体X坐标
    /// </summary>
    protected float m_ObjectX;
    public float X
    {
        get { return m_ObjectX; }
        set { m_ObjectX = value; }
    }

    /// <summary>
    /// 物体Y坐标
    /// </summary>
    protected float m_objectY;
    public float Y
    {
        get { return m_objectY; }
        set { m_objectY = value; }
    }

    /// <summary>
    /// 物体模型ID
    /// </summary>
    protected int m_ModelID;
    public int ModelID
    {
        get { return m_ModelID; }
        set { m_ModelID = value; }
    }

    protected virtual void Awake()
    {
        m_ObjectStage = ObjectStage.None;
    }

    protected virtual void OnEnable()
    {
        m_ObjectStage = ObjectStage.Show;
    }

    protected virtual void OnDisable()
    {
        m_ObjectStage = ObjectStage.Disable;
    }

    protected virtual void OnDestroy()
    {
        m_ObjectStage = ObjectStage.Destroy;
    }

    protected virtual bool Update()
    {
        if (m_ObjectStage == ObjectStage.Destroy)
        {
            return false;
        }

        return true;
    }

    //protected virtual bool FixedUpdate()
    //{
    //    if (m_ObjectStage == ObjectStage.Destroy)
    //    {
    //        return false;
    //    }

    //    return true;
    //}

    protected virtual bool LateUpdate()
    {
        if (m_ObjectStage == ObjectStage.Disable)
        {
            return false;
        }

        return true;
    }

    public override bool Equals(object other)
    {
        ObjectBase ob = other as ObjectBase;
        if (ob == null)
        {
            return false;
        }

        if (this.ObjectType == ob.ObjectType)
        {
            return this.UID == ob.UID && this.PID == ob.PID;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}