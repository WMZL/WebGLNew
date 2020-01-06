using System;
using UnityEngine;
using System.Collections;

public enum GIZMO_MODE
{
    TRANSLATE = 0,
    ROTATE = 1,
    SCALE = 2
}

public enum GIZMO_AXIS
{
    NONE = 0,
    X = 1,
    XY = 2,
    XZ = 3,
    Y = 4,
    YZ = 5,
    Z = 6
}

[System.Serializable]
public partial class GizmoController : MonoBehaviour
{
    public int RotationSpeed;
    //Snap settings
    public int SnappedRotationSpeed;
    public int SnappedScaleMultiplier;
    public bool Snapping;
    public float MoveSnapIncrement;
    public float AngleSnapIncrement;
    public float ScaleSnapIncrement;
    public Texture2D[] GizmoControlButtonImages;
    public int LayerID;
    //Mode Settings
    public bool AllowTranslate;
    public bool AllowRotate;
    public bool AllowScale;
    public bool ShowGizmoControlWindow;
    //Shortcut Keys
    public bool EnableShortcutKeys;
    //public string TranslateShortcutKey;
    //public string RotateShortcutKey;
    //public string ScaleShortcutKey;
    //public string SnapShortcutKey;
    //private
    private GIZMO_MODE _mode;
    private Transform _selectedObject;
    private bool _showGizmo;
    private GIZMO_AXIS _activeAxis;
    private Transform _selectedAxis;
    private Vector3 _lastIntersectPosition;
    private Vector3 _currIntersectPosition;
    private bool _draggingAxis;
    private Vector3 _translationDelta;
    private float _rotationSnapDelta;
    private Vector3 _scaleSnapDelta;
    private Vector3 _moveSnapDelta;
    private bool _ignoreRaycast;
    private float _XRotationDisplayValue;
    private float _YRotationDisplayValue;
    private float _ZRotationDisplayValue;
    private Vector2 _controlWinPosition;
    private int _controlWinWidth;
    private int _controlWinHeight;
    private GUISpinner AxisSpinner;
    private GUISpinner SnapSpinner;

    private float XRotationDisplayValue = 0;
    private float YRotationDisplayValue = 0;
    private float ZRotationDisplayValue = 0;
    public virtual void Start()//Start
    {
        this.Hide();
    }

    /// <summary>
	/// 私有变量，存根
	/// </summary>
	private static GizmoController _instance;
    /// <summary>
	/// 获取实例
	/// </summary>
	public static GizmoController Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject ob = Instantiate(Resources.Load(@"GizmoAdvanced", typeof(GameObject))) as GameObject;
                _instance = ob.GetComponent<GizmoController>();
            }
            return _instance;
        }
    }
    //Returns true if currently dragging the gizmo with the mouse
    public virtual bool IsDraggingAxis()//IsDragging
    {
        return this._draggingAxis;
    }

    //Returns true if the mouse is currently hovering over one of the gizmo axis controls
    public virtual bool IsOverAxis()//IsOverAxis
    {
        if (this._selectedAxis != null)
        {
            return true;
        }
        return false;
    }

    //Make the gizmo visible
    //Pass in GIZMO_MODE value which will set the current mode of the gizmo control
    public virtual void Show(GIZMO_MODE mode)//Show
    {
        if (this._selectedObject == null)
        {
            return;
        }
        foreach (Transform t in this.transform)
        {
            if (t.GetComponent<Renderer>())
            {
                t.GetComponent<Renderer>().enabled = true;
            }
            if (t.GetComponent<Collider>())
            {
                t.GetComponent<Collider>().isTrigger = false;
            }
        }
        //for
        this.SetMode(mode);
        this._showGizmo = true;
    }

    //Hides the gizmo
    public virtual void Hide()//Hide
    {
        foreach (Transform t in this.transform)
        {
            if (t.GetComponent<Renderer>())
            {
                t.GetComponent<Renderer>().enabled = false;
            }
            if (t.GetComponent<Collider>())
            {
                t.GetComponent<Collider>().isTrigger = true;
            }
        }
        //for
        this._selectedObject = null;
        this._selectedAxis = null;
        this._showGizmo = false;
    }

    //Returns true if the Gizmo control is currently not visible
    public virtual bool IsHidden()//IsHidden
    {
        return !this._showGizmo;
    }

    //Set's the X/Y position in screen coordinates of the gizmo control window
    public virtual void SetControlWinPosition(Vector2 position)//SetControlWinPosition
    {
        this._controlWinPosition = position;
    }

    //Toggles snap mode on and off
    public virtual void ToggleSnapping()//ToggleSnapping
    {
        this.Snapping = !this.Snapping;
    }

    //Set's snapping mode
    // Pass in boolean True=Snap on, False=Snap off
    public virtual void SetSnapping(bool snap)//SetSnapping
    {
        this.Snapping = snap;
    }

    //Returns the active GIZMO_MODE
    public virtual GIZMO_MODE GetMode()//GetMode
    {
        return this._mode;
    }

    public virtual void ResetTransformToSelectedObject()//ResetPositionToSelectedObject
    {
        if (this._selectedObject == null)
        {
            return;
        }
        this.transform.position = this._selectedObject.position;
        if (this._mode != GIZMO_MODE.TRANSLATE)
        {
            this.transform.rotation = this._selectedObject.rotation;
        }
    }

    //Set's the allowable modes for the Gizmo
    //move - Can use move/translate mode
    //rotate - Can use rotate mode
    //scale - Can use scale mode
    public virtual void SetAllowedModes(bool move, bool rotate, bool scale)//SetAllowsModes
    {
        this.AllowTranslate = move;
        this.AllowRotate = rotate;
        this.AllowScale = scale;
    }

    public virtual void ResetTransformations(bool resetPosition)//ResetTransformations
    {
        if (resetPosition)
        {
            this.transform.position = Vector3.zero;
        }
        //if
        this.transform.rotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;
        if (this._selectedObject != null)
        {
            this._selectedObject.transform.position = this.transform.position;
            this._selectedObject.transform.rotation = this.transform.rotation;
            this._selectedObject.transform.localScale = Vector3.one;
        }
        //if
        this._XRotationDisplayValue = 0;
        this._YRotationDisplayValue = 0;
        this._ZRotationDisplayValue = 0;
    }

    public virtual void OnGUI()//OnGUI
    {
        if (this._selectedObject == null)
        {
            return;
        }
        if (this._draggingAxis && (this._mode == GIZMO_MODE.ROTATE))
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
            screenPos.y = Screen.height - screenPos.y;
            float RotationValue = 0;
            switch (this._activeAxis)
            {
                case GIZMO_AXIS.X:
                    RotationValue = this._XRotationDisplayValue;
                    break;
                case GIZMO_AXIS.Y:
                    RotationValue = this._YRotationDisplayValue;
                    break;
                case GIZMO_AXIS.Z:
                    RotationValue = this._ZRotationDisplayValue;
                    break;
            }
            //switch
            if (this.Snapping)
            {
                GUI.Label(new Rect(screenPos.x, screenPos.y, 140, 20), "Snap Rotation Amt: " + Math.Round(RotationValue));
            }
            else
            {
                GUI.Label(new Rect(screenPos.x, screenPos.y, 120, 20), "Rotation Amt: " + Math.Round(RotationValue, 1));
            }
        }
        //if	
        if (this._showGizmo && this.ShowGizmoControlWindow)
        {
            if ((this.AllowTranslate || this.AllowRotate) || this.AllowScale)
            {
                Rect ControlRect = new Rect(this._controlWinPosition.x, this._controlWinPosition.y, this._controlWinWidth, this._controlWinHeight);
                if (ControlRect.Contains(Event.current.mousePosition))
                {

                    {
                        int _103 = 1;
                        Color _104 = GUI.color;
                        _104.a = _103;
                        GUI.color = _104;
                    }
                }
                else
                {

                    {
                        float _105 = 0.8f;
                        Color _106 = GUI.color;
                        _106.a = _105;
                        GUI.color = _106;
                    }
                }
                this.DrawGizmoControls(ControlRect);

                {
                    int _107 = 1;
                    Color _108 = GUI.color;
                    _108.a = _107;
                    GUI.color = _108;
                }
            }
        }
    }

    public virtual void DrawGizmoControls(Rect groupRect)//DrawGizmoControls
    {
        GUI.Box(groupRect, "Gizmo Control");
        Rect innerRect = new Rect(groupRect.x + 4, groupRect.y + 20, groupRect.width - 8, groupRect.height - 24);
        GUI.BeginGroup(innerRect);
        if (this.GizmoControlButtonImages.Length != 0)
        {
            GIZMO_MODE tmpMode = (GIZMO_MODE)GUI.Toolbar(new Rect(0, 0, innerRect.width, 20), (int)this._mode, this.GizmoControlButtonImages);
            if (GUI.changed)
            {
                GUI.changed = false;
                if (((this.AllowTranslate && (tmpMode == GIZMO_MODE.TRANSLATE)) || (this.AllowRotate && (tmpMode == GIZMO_MODE.ROTATE))) || (this.AllowScale && (tmpMode == GIZMO_MODE.SCALE)))
                {
                    this.SetMode(tmpMode);
                }
            }
        }
        else
        {
            //if
            //if
            float CtrlBtnWidth = innerRect.width / 3;
            int CtrlBtnOffset = 0;
            if (this.AllowTranslate && GUI.Button(new Rect(CtrlBtnOffset, 0, CtrlBtnWidth, 20), "P"))
            {
                this.SetMode(GIZMO_MODE.TRANSLATE);
            }
            CtrlBtnOffset = (int)(CtrlBtnOffset + CtrlBtnWidth);
            if (this.AllowRotate && GUI.Button(new Rect(CtrlBtnOffset, 0, CtrlBtnWidth, 20), "R"))
            {
                this.SetMode(GIZMO_MODE.ROTATE);
            }
            CtrlBtnOffset = (int)(CtrlBtnOffset + CtrlBtnWidth);
            if (this.AllowScale && GUI.Button(new Rect(CtrlBtnOffset, 0, CtrlBtnWidth, 20), "S"))
            {
                this.SetMode(GIZMO_MODE.SCALE);
            }
        }
        switch (this._mode)
        {
            case GIZMO_MODE.TRANSLATE:
                if (this.AllowTranslate)
                {

                    {
                        float _109 = this.AxisSpinner.Draw(new Rect(0, 25, innerRect.width, 20), "X:", this.transform.position.x);
                        Vector3 _110 = this.transform.position;
                        _110.x = _109;
                        this.transform.position = _110;
                    }

                    {
                        float _111 = this.AxisSpinner.Draw(new Rect(0, 47, innerRect.width, 20), "Y:", this.transform.position.y);
                        Vector3 _112 = this.transform.position;
                        _112.y = _111;
                        this.transform.position = _112;
                    }

                    {
                        float _113 = this.AxisSpinner.Draw(new Rect(0, 69, innerRect.width, 20), "Z:", this.transform.position.z);
                        Vector3 _114 = this.transform.position;
                        _114.z = _113;
                        this.transform.position = _114;
                    }
                    this._selectedObject.transform.position = this.transform.position;
                    this.Snapping = GUI.Toggle(new Rect(0, 94, innerRect.width, 20), this.Snapping, "Snap Mode");
                    this.MoveSnapIncrement = this.SnapSpinner.Draw(new Rect(0, 114, innerRect.width, 20), "Snap By:", this.MoveSnapIncrement, 1);
                }
                else
                {
                    //if
                    GUI.Label(new Rect(0, 40, innerRect.width, 40), "Disabled");
                }
                break;
            case GIZMO_MODE.ROTATE:
                if (this.AllowRotate)
                {
                    if (this._draggingAxis)
                    {
                        this.AxisSpinner.Draw(new Rect(0, 25, innerRect.width, 20), "X:", this.transform.rotation.eulerAngles.x);
                        this.AxisSpinner.Draw(new Rect(0, 47, innerRect.width, 20), "Y:", this.transform.rotation.eulerAngles.y);
                        this.AxisSpinner.Draw(new Rect(0, 69, innerRect.width, 20), "Z:", this.transform.rotation.eulerAngles.z);
                    }
                    else
                    {

                        {
                            float _115 = this.AxisSpinner.Draw(new Rect(0, 25, innerRect.width, 20), "X:", this.transform.rotation.eulerAngles.x);
                            Quaternion _116 = this.transform.rotation;
                            Vector3 _117 = _116.eulerAngles;
                            _117.x = _115;
                            _116.eulerAngles = _117;
                            this.transform.rotation = _116;
                        }

                        {
                            float _118 = this.AxisSpinner.Draw(new Rect(0, 47, innerRect.width, 20), "Y:", this.transform.rotation.eulerAngles.y);
                            Quaternion _119 = this.transform.rotation;
                            Vector3 _120 = _119.eulerAngles;
                            _120.y = _118;
                            _119.eulerAngles = _120;
                            this.transform.rotation = _119;
                        }

                        {
                            float _121 = this.AxisSpinner.Draw(new Rect(0, 69, innerRect.width, 20), "Z:", this.transform.rotation.eulerAngles.z);
                            Quaternion _122 = this.transform.rotation;
                            Vector3 _123 = _122.eulerAngles;
                            _123.z = _121;
                            _122.eulerAngles = _123;
                            this.transform.rotation = _122;
                        }
                        if (GUI.changed || (this._selectedObject.transform.rotation != this.transform.rotation))
                        {
                            GUI.changed = false;
                            this._selectedObject.transform.rotation = this.transform.rotation;
                        }
                    }
                    //if
                    //if
                    this.Snapping = GUI.Toggle(new Rect(0, 94, innerRect.width, 20), this.Snapping, "Snap Mode");
                    this.AngleSnapIncrement = this.SnapSpinner.Draw(new Rect(0, 114, innerRect.width, 20), "Snap By:", this.AngleSnapIncrement, 1);
                }
                else
                {
                    //if
                    GUI.Label(new Rect(0, 40, innerRect.width, 40), "Disabled");
                }
                break;
            case GIZMO_MODE.SCALE:
                if (this.AllowScale)
                {

                    {
                        float _124 = this.AxisSpinner.Draw(new Rect(0, 25, innerRect.width, 20), "X:", this._selectedObject.localScale.x);
                        Vector3 _125 = this._selectedObject.localScale;
                        _125.x = _124;
                        this._selectedObject.localScale = _125;
                    }

                    {
                        float _126 = this.AxisSpinner.Draw(new Rect(0, 47, innerRect.width, 20), "Y:", this._selectedObject.localScale.y);
                        Vector3 _127 = this._selectedObject.localScale;
                        _127.y = _126;
                        this._selectedObject.localScale = _127;
                    }

                    {
                        float _128 = this.AxisSpinner.Draw(new Rect(0, 69, innerRect.width, 20), "Z:", this._selectedObject.localScale.z);
                        Vector3 _129 = this._selectedObject.localScale;
                        _129.z = _128;
                        this._selectedObject.localScale = _129;
                    }
                    this.Snapping = GUI.Toggle(new Rect(0, 94, innerRect.width, 20), this.Snapping, "Snap Mode");
                    this.ScaleSnapIncrement = this.SnapSpinner.Draw(new Rect(0, 114, innerRect.width, 20), "Snap By:", this.ScaleSnapIncrement, 1);
                }
                else
                {
                    //if
                    GUI.Label(new Rect(0, 40, innerRect.width, 40), "Disabled");
                }
                break;
        }
        //else
        //switch
        GUI.EndGroup();
    }

    public virtual void SetSelectedAxisObject(Transform axisObject)//SetSelectedAxisObject
    {
        if (this._selectedAxis != null)
        {
            if (axisObject == null)
            {
                this._selectedAxis.SendMessage("SetAxisColor", (object)AXIS_COLOR.NORMAL);
            }
            else
            {
                //if
                if (this._selectedAxis.name != axisObject.name)
                {
                    this._selectedAxis.SendMessage("SetAxisColor", (object)AXIS_COLOR.NORMAL);
                }
            }
        }
        //if
        //if
        this._selectedAxis = axisObject;
        if (this._selectedAxis != null)
        {
            this._selectedAxis.SendMessage("SetAxisColor", (object)AXIS_COLOR.HOVER);
        }
    }

    public virtual void SetDragHilight(bool SetDrag)//SetDragHihlight
    {
        if (this._selectedAxis == null)
        {
            return;
        }
        if (SetDrag)
        {
            this._selectedAxis.SendMessage("SetAxisColor", (object)AXIS_COLOR.DRAG);
        }
        else
        {
            this._selectedAxis.SendMessage("SetAxisColor", (object)AXIS_COLOR.HOVER);
        }
    }

    private void Update()//Update
    {

        RaycastHit hit = default(RaycastHit);
        Plane plane = default(Plane);
        float snapValue = 0.0f;
        if (!this._showGizmo || (this._selectedObject == null))
        {
            return;
        }
        if (this.EnableShortcutKeys)
        {
            if (Input.GetKeyUp(KeyCode.W))
            {
                if (this.AllowTranslate)
                {
                    this.SetMode(GIZMO_MODE.TRANSLATE);
                }
            }
            //if
            if (Input.GetKeyUp(KeyCode.R))
            {
                if (this.AllowRotate)
                {
                    this.SetMode(GIZMO_MODE.ROTATE);
                }
            }
            //if
            if (Input.GetKeyUp(KeyCode.E))
            {
                if (this.AllowScale)
                {
                    this.SetMode(GIZMO_MODE.SCALE);
                }
            }
            //if
            if (Input.GetKeyUp(KeyCode.S))
            {
                this.ToggleSnapping();
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Hide();
            }
        }
        this.AxisSpinner.Update();
        this.SnapSpinner.Update();
        //Scale Gizmo relative to the the distance from the camera for consistant sizing
        float distance = Mathf.Abs(Vector3.Distance(Camera.main.transform.position, this.transform.position));
        this.transform.localScale = new Vector3(-1, 1, 1) * (distance / 8);
        int layerMask = 1 << this.LayerID;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && !this._ignoreRaycast)
        {
            if (!this._draggingAxis)
            {
                if (hit.transform.name.Contains("X"))
                {
                    this._activeAxis = GIZMO_AXIS.X;
                }
                if (hit.transform.name.Contains("Y"))
                {
                    this._activeAxis = GIZMO_AXIS.Y;
                }
                if (hit.transform.name.Contains("Z"))
                {
                    this._activeAxis = GIZMO_AXIS.Z;
                }
                if (hit.transform.name.Contains("XY"))
                {
                    this._activeAxis = GIZMO_AXIS.XY;
                }
                if (hit.transform.name.Contains("XZ"))
                {
                    this._activeAxis = GIZMO_AXIS.XZ;
                }
                if (hit.transform.name.Contains("YZ"))
                {
                    this._activeAxis = GIZMO_AXIS.YZ;
                }
                this.SetSelectedAxisObject(hit.transform);
            }
            if (Input.GetMouseButtonUp(0))
            {
                this._activeAxis = GIZMO_AXIS.NONE;
                this._lastIntersectPosition = Vector3.zero;
                this._draggingAxis = false;
                this.SetDragHilight(false);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                this._ignoreRaycast = true;
                this._activeAxis = GIZMO_AXIS.NONE;
            }
            if (!this._draggingAxis)
            {
                this.SetSelectedAxisObject(null);
            }
        }
        //else
        if (Input.GetMouseButtonUp(0))
        {
            this._ignoreRaycast = false;
        }
        //if	
        if (((this._activeAxis != GIZMO_AXIS.NONE) && Input.GetMouseButtonUp(0)) || (Input.GetMouseButtonDown(0) && !this._draggingAxis))
        {
            this._activeAxis = GIZMO_AXIS.NONE;
            this._lastIntersectPosition = Vector3.zero;
            this._currIntersectPosition = this._lastIntersectPosition;
            this._rotationSnapDelta = 0;
            this._scaleSnapDelta = Vector3.zero;
            this._moveSnapDelta = Vector3.zero;
            XRotationDisplayValue = 0;
            YRotationDisplayValue = 0;
            ZRotationDisplayValue = 0;
            this._draggingAxis = false;
            this.SetDragHilight(false);
        }
        if ((this._activeAxis != GIZMO_AXIS.NONE) && Input.GetMouseButton(0))
        {
            Vector3 objPos = this.transform.position;
            Vector3 objMovement = Vector3.zero;
            float hitDistance = 0;
            float MouseXDelta = Input.GetAxis("Mouse X");
            float MouseYDelta = Input.GetAxis("Mouse Y");
            this._draggingAxis = true;
            this.SetDragHilight(true);
            this._currIntersectPosition = this._lastIntersectPosition;
            switch (this._activeAxis)
            {
                case GIZMO_AXIS.X:
                    switch (this._mode)
                    {
                        case GIZMO_MODE.TRANSLATE:
                            plane = new Plane(Vector3.forward, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.MoveSnapIncrement > 0))
                                    {
                                        this._moveSnapDelta.x = this._moveSnapDelta.x + this._translationDelta.x;
                                        snapValue = Mathf.Round(this._moveSnapDelta.x / this.MoveSnapIncrement) * this.MoveSnapIncrement;
                                        this._moveSnapDelta.x = this._moveSnapDelta.x - snapValue;
                                        objMovement = Vector3.right * snapValue;
                                        objPos = objPos + objMovement;
                                    }
                                    else
                                    {
                                        objMovement = Vector3.right * this._translationDelta.x;
                                        objPos = objPos + objMovement;
                                    }
                                    this.transform.position = objPos;
                                    this._selectedObject.transform.position = this.transform.position;
                                }
                            }
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                        case GIZMO_MODE.ROTATE:
                            float rotXDelta = MouseXDelta * Time.deltaTime;
                            if (this.Snapping && (this.AngleSnapIncrement > 0))
                            {
                                rotXDelta = rotXDelta * this.SnappedRotationSpeed;
                                this._rotationSnapDelta = this._rotationSnapDelta + rotXDelta;
                                snapValue = Mathf.Round(this._rotationSnapDelta / this.AngleSnapIncrement) * this.AngleSnapIncrement;
                                this._rotationSnapDelta = this._rotationSnapDelta - snapValue;
                                XRotationDisplayValue = (int)(XRotationDisplayValue + snapValue);
                                this.transform.rotation = this.transform.rotation * Quaternion.AngleAxis(snapValue, Vector3.right);
                                this._selectedObject.transform.rotation = this.transform.rotation;
                            }
                            else
                            {
                                rotXDelta = rotXDelta * this.RotationSpeed;
                                XRotationDisplayValue = (int)(XRotationDisplayValue + rotXDelta);
                                this.transform.Rotate(Vector3.right * rotXDelta);
                                this._selectedObject.transform.rotation = this.transform.rotation;
                            }
                            //else
                            XRotationDisplayValue = (int)this.ClampRotationAngle(XRotationDisplayValue);
                            break;
                        case GIZMO_MODE.SCALE:
                            plane = new Plane(Vector3.forward, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.ScaleSnapIncrement > 0))
                                    {
                                        this._translationDelta = this._translationDelta * this.SnappedScaleMultiplier;
                                        this._scaleSnapDelta.x = this._scaleSnapDelta.x + this._translationDelta.x;
                                        snapValue = Mathf.Round(this._scaleSnapDelta.x / this.ScaleSnapIncrement) * this.ScaleSnapIncrement;
                                        this._scaleSnapDelta.x = this._scaleSnapDelta.x - snapValue;
                                        objMovement = Vector3.right * snapValue;
                                    }
                                    else
                                    {
                                        //if
                                        objMovement = this.transform.right * this._translationDelta.x;
                                    }
                                    //else
                                    this._selectedObject.transform.localScale = this._selectedObject.transform.localScale + objMovement;
                                }
                            }
                            //if						
                            //if		
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                    }
                    break;
                case GIZMO_AXIS.XY:
                    switch (this._mode)
                    {
                        case GIZMO_MODE.TRANSLATE:
                            plane = new Plane(Vector3.forward, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.MoveSnapIncrement > 0))
                                    {
                                        this._moveSnapDelta.x = this._moveSnapDelta.x + this._translationDelta.x;
                                        snapValue = Mathf.Round(this._moveSnapDelta.x / this.MoveSnapIncrement) * this.MoveSnapIncrement;
                                        this._moveSnapDelta.x = this._moveSnapDelta.x - snapValue;
                                        objMovement = Vector3.right * snapValue;
                                        this._moveSnapDelta.y = this._moveSnapDelta.y + this._translationDelta.y;
                                        snapValue = Mathf.Round(this._moveSnapDelta.y / this.MoveSnapIncrement) * this.MoveSnapIncrement;
                                        this._moveSnapDelta.y = this._moveSnapDelta.y - snapValue;
                                        objMovement = objMovement + (Vector3.up * snapValue);
                                        objPos = objPos + objMovement;
                                    }
                                    else
                                    {
                                        //if
                                        objMovement = Vector3.right * this._translationDelta.x;
                                        objMovement = objMovement + (Vector3.up * this._translationDelta.y);
                                        objPos = objPos + objMovement;
                                    }
                                    //else
                                    this.transform.position = objPos;
                                    this._selectedObject.transform.position = this.transform.position;
                                }
                            }
                            //if						
                            //if					
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                        case GIZMO_MODE.SCALE:
                            plane = new Plane(Vector3.forward, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.ScaleSnapIncrement > 0))
                                    {
                                        this._translationDelta = this._translationDelta * this.SnappedScaleMultiplier;
                                        this._scaleSnapDelta.x = this._scaleSnapDelta.x + this._translationDelta.x;
                                        snapValue = Mathf.Round(this._scaleSnapDelta.x / this.ScaleSnapIncrement) * this.ScaleSnapIncrement;
                                        this._scaleSnapDelta.x = this._scaleSnapDelta.x - snapValue;
                                        objMovement = Vector3.right * snapValue;
                                        this._scaleSnapDelta.y = this._scaleSnapDelta.y + this._translationDelta.y;
                                        snapValue = Mathf.Round(this._scaleSnapDelta.y / this.ScaleSnapIncrement) * this.ScaleSnapIncrement;
                                        this._scaleSnapDelta.y = this._scaleSnapDelta.y - snapValue;
                                        objMovement = objMovement + (Vector3.up * snapValue);
                                    }
                                    else
                                    {
                                        //if
                                        objMovement = this.transform.right * this._translationDelta.x;
                                        objMovement = objMovement + (this.transform.up * this._translationDelta.y);
                                    }
                                    //else
                                    this._selectedObject.transform.localScale = this._selectedObject.transform.localScale + objMovement;
                                }
                            }
                            //if						
                            //if		
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                    }
                    break;
                case GIZMO_AXIS.Y:
                    switch (this._mode)
                    {
                        case GIZMO_MODE.TRANSLATE:
                            plane = new Plane(Vector3.forward, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.MoveSnapIncrement > 0))
                                    {
                                        this._moveSnapDelta.y = this._moveSnapDelta.y + this._translationDelta.y;
                                        snapValue = Mathf.Round(this._moveSnapDelta.y / this.MoveSnapIncrement) * this.MoveSnapIncrement;
                                        this._moveSnapDelta.y = this._moveSnapDelta.y - snapValue;
                                        objMovement = Vector3.up * snapValue;
                                        objPos = objPos + objMovement;
                                    }
                                    else
                                    {
                                        //if
                                        objMovement = Vector3.up * this._translationDelta.y;
                                        objPos = objPos + objMovement;
                                    }
                                    //else
                                    this.transform.position = objPos;
                                    this._selectedObject.transform.position = this.transform.position;
                                }
                            }
                            //if							
                            //if						
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                        case GIZMO_MODE.ROTATE:
                            float rotYDelta = MouseXDelta * Time.deltaTime;
                            if (this.Snapping && (this.AngleSnapIncrement > 0))
                            {
                                rotYDelta = rotYDelta * this.SnappedRotationSpeed;
                                this._rotationSnapDelta = this._rotationSnapDelta + rotYDelta;
                                snapValue = Mathf.Round(this._rotationSnapDelta / this.AngleSnapIncrement) * this.AngleSnapIncrement;
                                this._rotationSnapDelta = this._rotationSnapDelta - snapValue;
                                YRotationDisplayValue = (int)(YRotationDisplayValue + snapValue);
                                this.transform.rotation = this.transform.rotation * Quaternion.AngleAxis(snapValue, Vector3.up);
                                this._selectedObject.transform.rotation = this.transform.rotation;
                            }
                            else
                            {
                                //if
                                rotYDelta = rotYDelta * this.RotationSpeed;
                                YRotationDisplayValue = (int)(YRotationDisplayValue + rotYDelta);
                                this.transform.Rotate(Vector3.up * rotYDelta);
                                this._selectedObject.transform.rotation = this.transform.rotation;
                            }
                            //else
                            YRotationDisplayValue = (int)this.ClampRotationAngle(YRotationDisplayValue);
                            break;
                        case GIZMO_MODE.SCALE:
                            plane = new Plane(Vector3.forward, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.ScaleSnapIncrement > 0))
                                    {
                                        this._translationDelta = this._translationDelta * this.SnappedScaleMultiplier;
                                        this._scaleSnapDelta.y = this._scaleSnapDelta.y + this._translationDelta.y;
                                        snapValue = Mathf.Round(this._scaleSnapDelta.y / this.ScaleSnapIncrement) * this.ScaleSnapIncrement;
                                        this._scaleSnapDelta.y = this._scaleSnapDelta.y - snapValue;
                                        objMovement = Vector3.up * snapValue;
                                    }
                                    else
                                    {
                                        //if
                                        objMovement = Vector3.up * this._translationDelta.y;
                                    }
                                    //else
                                    this._selectedObject.transform.localScale = this._selectedObject.transform.localScale + objMovement;
                                }
                            }
                            //if						
                            //if		
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                    }
                    break;
                case GIZMO_AXIS.XZ:
                    switch (this._mode)
                    {
                        case GIZMO_MODE.TRANSLATE:
                            plane = new Plane(Vector3.up, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.MoveSnapIncrement > 0))
                                    {
                                        this._moveSnapDelta.x = this._moveSnapDelta.x + this._translationDelta.x;
                                        snapValue = Mathf.Round(this._moveSnapDelta.x / this.MoveSnapIncrement) * this.MoveSnapIncrement;
                                        this._moveSnapDelta.x = this._moveSnapDelta.x - snapValue;
                                        objMovement = Vector3.right * snapValue;
                                        this._moveSnapDelta.z = this._moveSnapDelta.z + this._translationDelta.z;
                                        snapValue = Mathf.Round(this._moveSnapDelta.z / this.MoveSnapIncrement) * this.MoveSnapIncrement;
                                        this._moveSnapDelta.z = this._moveSnapDelta.z - snapValue;
                                        objMovement = objMovement + (Vector3.forward * snapValue);
                                        objPos = objPos + objMovement;
                                    }
                                    else
                                    {
                                        //if
                                        objMovement = Vector3.forward * this._translationDelta.z;
                                        objMovement = objMovement + (Vector3.right * this._translationDelta.x);
                                        objPos = objPos + objMovement;
                                    }
                                    //else
                                    this.transform.position = objPos;
                                    this._selectedObject.transform.position = this.transform.position;
                                }
                            }
                            //if						
                            //if					
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                        case GIZMO_MODE.SCALE:
                            plane = new Plane(Vector3.up, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.ScaleSnapIncrement > 0))
                                    {
                                        this._translationDelta = this._translationDelta * this.SnappedScaleMultiplier;
                                        this._scaleSnapDelta.x = this._scaleSnapDelta.x + this._translationDelta.x;
                                        snapValue = Mathf.Round(this._scaleSnapDelta.x / this.ScaleSnapIncrement) * this.ScaleSnapIncrement;
                                        this._scaleSnapDelta.x = this._scaleSnapDelta.x - snapValue;
                                        objMovement = Vector3.right * snapValue;
                                        this._scaleSnapDelta.z = this._scaleSnapDelta.z + this._translationDelta.z;
                                        snapValue = Mathf.Round(this._scaleSnapDelta.z / this.ScaleSnapIncrement) * this.ScaleSnapIncrement;
                                        this._scaleSnapDelta.z = this._scaleSnapDelta.z - snapValue;
                                        objMovement = objMovement + (Vector3.forward * snapValue);
                                    }
                                    else
                                    {
                                        //if
                                        objMovement = this.transform.right * this._translationDelta.x;
                                        objMovement = objMovement + (this.transform.forward * this._translationDelta.z);
                                    }
                                    //else
                                    this._selectedObject.transform.localScale = this._selectedObject.transform.localScale + objMovement;
                                }
                            }
                            //if						
                            //if		
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                    }
                    break;
                case GIZMO_AXIS.YZ:
                    switch (this._mode)
                    {
                        case GIZMO_MODE.TRANSLATE:
                            plane = new Plane(Vector3.right, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.MoveSnapIncrement > 0))
                                    {
                                        this._moveSnapDelta.y = this._moveSnapDelta.y + this._translationDelta.y;
                                        snapValue = Mathf.Round(this._moveSnapDelta.y / this.MoveSnapIncrement) * this.MoveSnapIncrement;
                                        this._moveSnapDelta.y = this._moveSnapDelta.y - snapValue;
                                        objMovement = Vector3.up * snapValue;
                                        this._moveSnapDelta.z = this._moveSnapDelta.z + this._translationDelta.z;
                                        snapValue = Mathf.Round(this._moveSnapDelta.z / this.MoveSnapIncrement) * this.MoveSnapIncrement;
                                        this._moveSnapDelta.z = this._moveSnapDelta.z - snapValue;
                                        objMovement = objMovement + (Vector3.forward * snapValue);
                                        objPos = objPos + objMovement;
                                    }
                                    else
                                    {
                                        //if
                                        objMovement = Vector3.up * this._translationDelta.y;
                                        objMovement = objMovement + (Vector3.forward * this._translationDelta.z);
                                        objPos = objPos + objMovement;
                                    }
                                    //else
                                    this.transform.position = objPos;
                                    this._selectedObject.transform.position = this.transform.position;
                                }
                            }
                            //if						
                            //if					
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                        case GIZMO_MODE.SCALE:
                            plane = new Plane(Vector3.right, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.ScaleSnapIncrement > 0))
                                    {
                                        this._translationDelta = this._translationDelta * this.SnappedScaleMultiplier;
                                        this._scaleSnapDelta.y = this._scaleSnapDelta.y + this._translationDelta.y;
                                        snapValue = Mathf.Round(this._scaleSnapDelta.y / this.ScaleSnapIncrement) * this.ScaleSnapIncrement;
                                        this._scaleSnapDelta.y = this._scaleSnapDelta.y - snapValue;
                                        objMovement = Vector3.up * snapValue;
                                        this._scaleSnapDelta.z = this._scaleSnapDelta.z + this._translationDelta.z;
                                        snapValue = Mathf.Round(this._scaleSnapDelta.z / this.ScaleSnapIncrement) * this.ScaleSnapIncrement;
                                        this._scaleSnapDelta.z = this._scaleSnapDelta.z - snapValue;
                                        objMovement = objMovement + (Vector3.forward * snapValue);
                                    }
                                    else
                                    {
                                        //if
                                        objMovement = this.transform.up * this._translationDelta.y;
                                        objMovement = objMovement + (this.transform.forward * this._translationDelta.z);
                                    }
                                    //else
                                    this._selectedObject.transform.localScale = this._selectedObject.transform.localScale + objMovement;
                                }
                            }
                            //if						
                            //if		
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                    }
                    break;
                case GIZMO_AXIS.Z:
                    switch (this._mode)
                    {
                        case GIZMO_MODE.TRANSLATE:
                            plane = new Plane(Vector3.up, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.MoveSnapIncrement > 0))
                                    {
                                        this._moveSnapDelta.z = this._moveSnapDelta.z + this._translationDelta.z;
                                        snapValue = Mathf.Round(this._moveSnapDelta.z / this.MoveSnapIncrement) * this.MoveSnapIncrement;
                                        this._moveSnapDelta.z = this._moveSnapDelta.z - snapValue;
                                        objMovement = Vector3.forward * snapValue;
                                        objPos = objPos + objMovement;
                                    }
                                    else
                                    {
                                        //if
                                        objMovement = Vector3.forward * this._translationDelta.z;
                                        objPos = objPos + objMovement;
                                    }
                                    //else
                                    this.transform.position = objPos;
                                    this._selectedObject.transform.position = this.transform.position;
                                }
                            }
                            //if							
                            //if						
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                        case GIZMO_MODE.ROTATE:
                            float rotZDelta = MouseXDelta * Time.deltaTime;
                            if (this.Snapping && (this.AngleSnapIncrement > 0))
                            {
                                rotZDelta = rotZDelta * this.SnappedRotationSpeed;
                                this._rotationSnapDelta = this._rotationSnapDelta + rotZDelta;
                                snapValue = Mathf.Round(this._rotationSnapDelta / this.AngleSnapIncrement) * this.AngleSnapIncrement;
                                this._rotationSnapDelta = this._rotationSnapDelta - snapValue;
                                ZRotationDisplayValue = (int)(ZRotationDisplayValue + snapValue);
                                this.transform.rotation = this.transform.rotation * Quaternion.AngleAxis(snapValue, Vector3.forward);
                                this._selectedObject.transform.rotation = this.transform.rotation;
                            }
                            else
                            {
                                //if
                                rotZDelta = rotZDelta * this.RotationSpeed;
                                ZRotationDisplayValue = (int)(ZRotationDisplayValue + rotZDelta);
                                this.transform.Rotate(Vector3.forward * rotZDelta);
                                this._selectedObject.transform.rotation = this.transform.rotation;
                            }
                            //else
                            ZRotationDisplayValue = (int)this.ClampRotationAngle(ZRotationDisplayValue);
                            break;
                        case GIZMO_MODE.SCALE:
                            plane = new Plane(Vector3.up, this.transform.position);
                            hitDistance = 0;
                            if (plane.Raycast(ray, out hitDistance))
                            {
                                this._currIntersectPosition = ray.direction * hitDistance;
                                if (this._lastIntersectPosition != Vector3.zero)
                                {
                                    this._translationDelta = this._currIntersectPosition - this._lastIntersectPosition;
                                    if (this.Snapping && (this.ScaleSnapIncrement > 0))
                                    {
                                        this._translationDelta = this._translationDelta * this.SnappedScaleMultiplier;
                                        this._scaleSnapDelta.z = this._scaleSnapDelta.z + this._translationDelta.z;
                                        snapValue = Mathf.Round(this._scaleSnapDelta.z / this.ScaleSnapIncrement) * this.ScaleSnapIncrement;
                                        this._scaleSnapDelta.z = this._scaleSnapDelta.z - snapValue;
                                        objMovement = Vector3.forward * snapValue;
                                    }
                                    else
                                    {
                                        //if
                                        objMovement = Vector3.forward * this._translationDelta.z;
                                    }
                                    //else
                                    this._selectedObject.transform.localScale = this._selectedObject.transform.localScale + objMovement;
                                }
                            }
                            //if						
                            //if		
                            this._lastIntersectPosition = this._currIntersectPosition;
                            break;
                    }
                    break;
            }
        }
    }

    //Sets the currently selected object transform
    //Pass in GameObject Transform
    public virtual void SetSelectedObject(Transform ObjectTransform)//ObjectTransform
    {
        if (ObjectTransform == null)
        {
            return;
        }
        this._selectedObject = ObjectTransform;
        this.transform.position = this._selectedObject.transform.position;
        this.SetMode(this._mode);
        XRotationDisplayValue = 0;
        YRotationDisplayValue = 0;
        ZRotationDisplayValue = 0;
    }

    //Set's the active GIZMO_MODE
    //GIZMO_MODE.TRANSLATE, GIZMO_MODE.ROTATE, GIZMO_MODE.SCALE
    public virtual void SetMode(GIZMO_MODE mode)//SetMode
    {
        this._mode = mode;
        switch (this._mode)
        {
            case GIZMO_MODE.TRANSLATE:
                this.transform.rotation = Quaternion.identity;
                break;
            case GIZMO_MODE.ROTATE:
                this.transform.rotation = this._selectedObject.transform.rotation;
                break;
            case GIZMO_MODE.SCALE:
                this.transform.rotation = this._selectedObject.transform.rotation;
                break;
        }
        //switch
        foreach (Transform t in this.transform)
        {
            if (t.name.Contains("Pivot"))
            {
                continue;
            }
            switch (this._mode)
            {
                case GIZMO_MODE.TRANSLATE:
                    if (((t.name.Contains("Translate") || t.name.Contains("XY")) || t.name.Contains("XZ")) || t.name.Contains("YZ"))
                    {
                        t.GetComponent<Renderer>().enabled = true;
                        t.gameObject.layer = this.LayerID;
                    }
                    else
                    {
                        //if
                        t.GetComponent<Renderer>().enabled = false;
                        t.gameObject.layer = 2;
                    }
                    break;
                case GIZMO_MODE.ROTATE:
                    if (t.name.Contains("Rotate"))
                    {
                        t.GetComponent<Renderer>().enabled = true;
                        t.gameObject.layer = this.LayerID;
                    }
                    else
                    {
                        //if
                        t.GetComponent<Renderer>().enabled = false;
                        t.gameObject.layer = 2;
                    }
                    break;
                case GIZMO_MODE.SCALE:
                    if (((t.name.Contains("Scale") || t.name.Contains("XY")) || t.name.Contains("XZ")) || t.name.Contains("YZ"))
                    {
                        t.GetComponent<Renderer>().enabled = true;
                        t.gameObject.layer = this.LayerID;
                    }
                    else
                    {
                        //if
                        t.GetComponent<Renderer>().enabled = false;
                        t.gameObject.layer = 2;
                    }
                    break;
            }
        }
    }

    public virtual float ClampRotationAngle(float value)//ClampRotation
    {
        if (value > 360)
        {
            value = value - 360;
        }
        if (value < 0)
        {
            value = value + 360;
        }
        return value;
    }

    public GizmoController()
    {
        this.RotationSpeed = 250;
        this.SnappedRotationSpeed = 500;
        this.SnappedScaleMultiplier = 4;
        this.MoveSnapIncrement = 1;
        this.AngleSnapIncrement = 5;
        this.ScaleSnapIncrement = 1;
        this.LayerID = 8;
        this.AllowTranslate = true;
        this.AllowRotate = true;
        this.AllowScale = true;
        this.ShowGizmoControlWindow = true;
        this.EnableShortcutKeys = true;
        //this.TranslateShortcutKey = "1";
        //this.RotateShortcutKey = "2";
        //this.ScaleShortcutKey = "3";
        //this.SnapShortcutKey = "s";
        this._activeAxis = GIZMO_AXIS.NONE;
        this._lastIntersectPosition = Vector3.zero;
        this._currIntersectPosition = Vector3.zero;
        this._translationDelta = Vector3.zero;
        this._scaleSnapDelta = Vector3.zero;
        this._moveSnapDelta = Vector3.zero;
        this._controlWinPosition = Vector2.zero;
        this._controlWinWidth = 105;
        this._controlWinHeight = 160;
        this.AxisSpinner = new GUISpinner();
        this.SnapSpinner = new GUISpinner();
    }

}