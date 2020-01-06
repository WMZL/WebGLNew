using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class EventTriggerListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler,
    IPointerExitHandler, IPointerUpHandler, ISelectHandler, IUpdateSelectedHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public UnityAction<GameObject> m_OnClick;
    public UnityAction<GameObject> m_OnDown;
    public UnityAction<GameObject> m_OnEnter;
    public UnityAction<GameObject> m_OnExit;
    public UnityAction<GameObject> m_OnUp;
    public UnityAction<GameObject> m_OnSelect;
    public UnityAction<GameObject> m_OnUpdateSelect;
    public UnityAction<PointerEventData, GameObject> m_OnBeginDrag;
    public UnityAction<PointerEventData, GameObject> m_OnDrag;
    public UnityAction<PointerEventData, GameObject> m_OnEndDrag;

    private object m_EventData;
    public object EventData
    {
        get { return m_EventData; }
        set { m_EventData = value; }
    }

    public static EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null)
        {
            listener = go.AddComponent<EventTriggerListener>();
        }

        return listener;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_OnClick != null)
        {
            m_OnClick(this.gameObject);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (null != m_OnDown)
        {
            m_OnDown(this.gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (null != m_OnEnter)
        {
            m_OnEnter(this.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (null != m_OnExit)
        {
            m_OnExit(this.gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (null != m_OnUp)
        {
            m_OnUp(this.gameObject);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (null != m_OnSelect)
        {
            m_OnSelect(this.gameObject);
        }
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (null != m_OnUpdateSelect)
        {
            m_OnUpdateSelect(this.gameObject);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (null != m_OnBeginDrag)
        {
            m_OnBeginDrag(eventData, this.gameObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (null != m_OnDrag)
        {
            m_OnDrag(eventData, this.gameObject);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (null != m_OnEndDrag)
        {
            m_OnEndDrag(eventData, this.gameObject);
        }
    }
}