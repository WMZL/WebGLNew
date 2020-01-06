using UnityEngine;
using System.Collections;

public enum AXIS_COLOR
{
    NORMAL = 0,
    HOVER = 1,
    DRAG = 2
}

[System.Serializable]
public partial class GizmoAxisHandle : MonoBehaviour
{
    public Color NormalColor;
    public Color HoverColor;
    public Color DragColor;
    private AXIS_COLOR _axisColor;
    public virtual void Start()//Start
    {
        if (!this.transform.GetComponent<Renderer>())
        {
            return;
        }
        this.NormalColor = this.transform.GetComponent<Renderer>().material.color;
        this.HoverColor = new Color(this.NormalColor.r, this.NormalColor.g, this.NormalColor.b, 1);
        this.DragColor = new Color(1, 1, 0, 0.8f);
    }

    public virtual void SetAxisColor(AXIS_COLOR axisColor)//SetAxisColor
    {
        this._axisColor = axisColor;
        if (!this.transform.GetComponent<Renderer>())
        {
            return;
        }
        switch (this._axisColor)
        {
            case AXIS_COLOR.NORMAL:
                this.transform.GetComponent<Renderer>().material.color = this.NormalColor;
                break;
            case AXIS_COLOR.HOVER:
                this.transform.GetComponent<Renderer>().material.color = this.HoverColor;
                break;
            case AXIS_COLOR.DRAG:
                this.transform.GetComponent<Renderer>().material.color = this.DragColor;
                break;
        }
    }

    public GizmoAxisHandle()
    {
        this._axisColor = AXIS_COLOR.NORMAL;
    }

}