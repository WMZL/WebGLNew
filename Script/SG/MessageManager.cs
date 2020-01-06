using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// 事件监听回调类
/// </summary>
public interface IMessageEventListener
{
    /// <summary>
    /// 处理消息
    /// </summary>
    /// <param name="head">消息id</param>
    /// <param name="arms">消息参数</param>
    void HandleEvent(string head, params object[] arms);

    /// <summary>
    /// 同一个消息头下的消息优先级
    /// </summary>
    /// <returns></returns>
    int EventPriority();
}

/// <summary>
/// 一个消息的标准类
/// 可以用来装载那些完全不需要序列的消息
/// </summary>
public class MessageEventListenerBase : IMessageEventListener
{
    private System.Action<string, object[]> m_DeleteFunction;

    public MessageEventListenerBase(System.Action<string, object[]> fun)
    {
        m_DeleteFunction = fun;
    }

    public void HandleEvent(string head, params object[] arms)
    {
        if (m_DeleteFunction != null)
        {
            m_DeleteFunction(head, arms);
        }
    }

    public int EventPriority()
    {
        return 0;
    }

    public override bool Equals(object obj)
    {
        MessageEventListenerBase mb = obj as MessageEventListenerBase;
        return mb.m_DeleteFunction.Equals(this.m_DeleteFunction);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

/// <summary>
/// 消息内容
/// </summary>
public struct MessageInfo
{
    public int m_Identification;
    public string m_Header;
    public object[] m_Arms;

    public MessageInfo(int id, string header, params object[] arms)
    {
        m_Identification = id;
        m_Header = header;
        m_Arms = null;
        if (arms != null)
        {
            m_Arms = new object[arms.Length];
            for (int index = 0; index < arms.Length; index++)
            {
                m_Arms[index] = arms[index];
            }
        }
    }

    public override bool Equals(object obj)
    {
        MessageInfo info = (MessageInfo)obj;
        return this.m_Identification == info.m_Identification;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("message【ID:{0} Header:{1}】", m_Identification, m_Header);
    }
}

/// <summary>
/// 统一管理所有消息内容
/// </summary>
public class MessageManager : Singleton<MessageManager>
{
    /// <summary>
    /// 所有的消息监听
    /// </summary>
    private Dictionary<string, List<IMessageEventListener>> m_AllMessageDic;

    /// <summary>
    /// 消息队列群
    /// </summary>
    private Queue<MessageInfo> m_MessageQueue;
    private readonly int m_MaxMessageIdentification = 99999;
    private readonly int m_MinMessageIdentification = 1;
    private readonly int m_Step = 100;
    private int m_Current;

    private readonly string m_LockObject = "LockObject-Messagemanager";

    protected override void Awake()
    {
        base.Awake();

        m_AllMessageDic = new Dictionary<string, List<IMessageEventListener>>();
        m_AllMessageDic.Clear();

        m_MessageQueue = new Queue<MessageInfo>();
        m_MessageQueue.Clear();

        m_Current = m_MinMessageIdentification;
    }

    /// <summary>
    /// 添加一个消息监听
    /// </summary>
    /// <param name="head"></param>
    /// <param name="listener"></param>
    public void AddMessageEventListener(string head, System.Action<string, object[]> listener)
    {
        MessageEventListenerBase mb = new MessageEventListenerBase(listener);
        AddMessageEventListener(head, mb);
    }

    /// <summary>
    /// 添加一个消息监听
    /// </summary>
    /// <param name="head"></param>
    /// <param name="listener"></param>
    public void AddMessageEventListener(string head, IMessageEventListener listener)
    {
        if (m_AllMessageDic.ContainsKey(head))
        {
            if (!m_AllMessageDic[head].Contains(listener))
            {
                int index = 0;
                for (; index < m_AllMessageDic[head].Count;)
                {
                    if (m_AllMessageDic[head][index].EventPriority() < listener.EventPriority())
                    {
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }

                m_AllMessageDic[head].Insert(index, listener);
            }
        }
        else
        {
            m_AllMessageDic.Add(head, new List<IMessageEventListener>() { listener });
        }
    }

    /// <summary>
    /// 移除某一个监听方法
    /// </summary>
    /// <param name="head"></param>
    /// <param name="listener"></param>
    public void RemoveMessageEventListener(string head, System.Action<string, object[]> listener)
    {
        MessageEventListenerBase mb = new MessageEventListenerBase(listener);
        RemoveMessageEventListener(head, mb);
    }

    /// <summary>
    /// 移除一个消息监听
    /// </summary>
    /// <param name="head"></param>
    /// <param name="listener"></param>
    public void RemoveMessageEventListener(string head, IMessageEventListener listener)
    {
        if (m_AllMessageDic.ContainsKey(head))
        {
            if (m_AllMessageDic[head].Contains(listener))
            {
                m_AllMessageDic[head].Remove(listener);

                if (m_AllMessageDic[head].Count < 1)
                {
                    m_AllMessageDic.Remove(head);
                }
            }
        }
    }

    /// <summary>
    /// 移除整个消息监听
    /// </summary>
    /// <param name="head"></param>
    public void RemoveMessageEventListener(string head)
    {
        if (m_AllMessageDic.ContainsKey(head))
        {
            m_AllMessageDic[head].Clear();
            m_AllMessageDic.Remove(head);
        }
    }

    /// <summary>
    /// 清空所有的消息监听
    /// </summary>
    public void ClearMessageEventListener()
    {
        if (m_AllMessageDic.Count > 0)
        {
            m_AllMessageDic.Clear();
        }
    }

    /// <summary>
    /// 发送消息
    /// 只在与网络交互的时候使用
    /// </summary>
    /// <param name="head"></param>
    /// <param name="arms"></param>
    public void SendMessageEvent(string head, params object[] arms)
    {
        Monitor.Enter(m_LockObject);
        MessageInfo info = new MessageInfo(GetIdenterification(), head, arms);
        m_MessageQueue.Enqueue(info);
        Monitor.Exit(m_LockObject);
    }

    /// <summary>
    /// 立即分发消息
    /// mono内部使用
    /// </summary>
    /// <param name="head"></param>
    /// <param name="arms"></param>
    public void SendMessageEventNow(string head, params object[] arms)
    {
        if (m_AllMessageDic.ContainsKey(head))
        {
            for (int index = 0; index < m_AllMessageDic[head].Count; index++)
            {
                m_AllMessageDic[head][index].HandleEvent(head, arms);
            }
        }
    }

    /// <summary>
    /// 获取唯一标识
    /// </summary>
    /// <returns></returns>
    private int GetIdenterification()
    {
        m_Current += 1;

        if (m_Current > m_MaxMessageIdentification)
        {
            m_Current = m_MinMessageIdentification;
        }

        return m_Current;
    }

    protected override bool Update()
    {
        if (!base.Update())
            return false;

        List<MessageInfo> infos = GetMessageInfo();
        for (int index = 0; index < infos.Count; index++)
        {
            if (m_AllMessageDic.ContainsKey(infos[index].m_Header))
            {
                for (int i = 0; i < m_AllMessageDic[infos[index].m_Header].Count; i++)
                {
                    m_AllMessageDic[infos[index].m_Header][i].HandleEvent(infos[index].m_Header, infos[index].m_Arms);
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 获取消息
    /// </summary>
    /// <returns></returns>
    private List<MessageInfo> GetMessageInfo()
    {
        List<MessageInfo> ms = new List<MessageInfo>();

        Monitor.Enter(m_LockObject);
        if (m_MessageQueue.Count > 0)
        {
            int temp = m_Step;
            while (m_MessageQueue.Count > 0 && temp >= 0)
            {
                temp--;
                MessageInfo info = m_MessageQueue.Dequeue();
                ms.Add(info);
            }
        }
        Monitor.Exit(m_LockObject);

        return ms;
    }
}