using UnityEngine;
using System.Collections;

[System.Serializable]
public class GUISpinner : object
{
    public GUIStyle LabelStyle;
    public GUIStyle ButtonUpStyle;
    public GUIStyle ButtonDownStyle;
    public float StepValue;
    public float ResetValue;
    public int ButtonWidth;
    public bool ResetOnMouseClick;
    public float IncrementSpeed;
    private float _mouseDownTime;
    public GUISpinner()
    {
        this.StepValue = 0.1f;
        this.ButtonWidth = 20;
        this.ResetOnMouseClick = true;
        this.IncrementSpeed = 0.5f;
    }

    public virtual void Update()//Update
    {
        if (Input.GetMouseButton(0))
        {
            this._mouseDownTime = this._mouseDownTime + (1 * Time.deltaTime);
        }
        //if
        if (Input.GetMouseButtonUp(0))
        {
            this._mouseDownTime = 0;
        }
    }

    public virtual float Draw(Rect rect, string label, float value, float stepValue)//Draw
    {
        this.StepValue = stepValue;
        return this.Draw(rect, label, value);
    }

    public virtual float Draw(Rect rect, string label, float value)//Draw
    {
        Vector2 LabelSize = Vector2.zero;
        if (!(this.LabelStyle == null))
        {
            LabelSize = this.LabelStyle.CalcSize(new GUIContent(label));
        }
        else
        {
            LabelSize = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(label));
        }
        int labelWidth = (int) (LabelSize.x + 4);
        int textFieldWidth = (int) ((rect.width - labelWidth) - this.ButtonWidth);
        if (rect.Contains(Event.current.mousePosition) && Input.GetMouseButtonDown(1))
        {
            value = this.ResetValue;
        }
        //if
        GUI.BeginGroup(rect);
        if (!(this.LabelStyle == null))
        {
            GUI.Label(new Rect(0, 0, labelWidth, rect.height), label, this.LabelStyle);
        }
        else
        {
            GUI.Label(new Rect(0, 0, labelWidth, rect.height), label);
        }
        string valStr = GUI.TextField(new Rect(labelWidth, 0, (rect.width - labelWidth) - 20, rect.height), System.Math.Round(value, 2).ToString());
        if (GUI.changed)
        {
            if (!float.TryParse(valStr, out value))
            {
                value = this.ResetValue;
            }
        }
        //if
        Rect ButtonPlusRect = new Rect(labelWidth + textFieldWidth, 0, this.ButtonWidth, rect.height / 2);
        Rect ButtonMinusRect = new Rect(labelWidth + textFieldWidth, rect.height / 2, this.ButtonWidth, rect.height / 2);
        if (Event.current.type == EventType.Repaint)
        {
            if ((this._mouseDownTime > this.IncrementSpeed) && ButtonPlusRect.Contains(Event.current.mousePosition))
            {
                value = value + this.StepValue;
            }
            //if
            if ((this._mouseDownTime > this.IncrementSpeed) && ButtonMinusRect.Contains(Event.current.mousePosition))
            {
                value = value - this.StepValue;
            }
        }
        //if
        //if
        //if(ButtonUpStyle != null)
        //	if(GUI.Button (ButtonPlusRect, "", ButtonUpStyle)){ value += StepValue; GUI.changed = true;}
        //else
        if (GUI.Button(ButtonPlusRect, "+"))
        {
            value = value + this.StepValue;
            GUI.changed = true;
        }
        //if(ButtonDownStyle != null)
        //	if(GUI.Button (ButtonMinusRect, "", ButtonDownStyle)){value -= StepValue; GUI.changed = true;}		
        //else
        if (GUI.Button(ButtonMinusRect, "-"))
        {
            value = value - this.StepValue;
            GUI.changed = true;
        }
        GUI.EndGroup();
        return value;
    }

}