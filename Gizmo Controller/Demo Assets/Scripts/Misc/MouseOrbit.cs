using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.AddComponentMenu("Camera-Control/Mouse Orbit")]
public partial class MouseOrbit : MonoBehaviour
{
	public Transform target;
	public float distance;
	public float moveSpeed;
	public float xSpeed;
	public float ySpeed;
	public int yMinLimit;
	public int yMaxLimit;
	public int zoomSpeed;
	public float zoomNearLimit;
	public float zoomFarLimit;
	private float x;
	private float y;
	private float z;
	private float moveMultiplier;
	private bool ignoreHotControl;
	private bool isEnabled;
	private bool downButtonPressed;
	private bool leftButtonPressed;
	private bool rightButtonPressed;
	private bool upButtonPressed;
	public virtual void Start()//Start
	{
		this.resetCamera();
	}

	public virtual void resetCamera()//resetCamera
	{
		if (this.target)
		{
			this.distance = 6;
			this.x = 180;
			this.y = 15;
			this.target.transform.position = Vector3.zero;
			Quaternion rotation = Quaternion.Euler(this.y, this.x, 0);
			Vector3 position = (rotation * new Vector3(0f, 0f, -this.distance)) + this.target.position;
			this.transform.rotation = rotation;
			this.transform.position = position;
		}
	}

	public virtual void setEnabled(bool enabled)//setEnabled
	{
		this.isEnabled = enabled;
	}

	public virtual void zoom(float val)//zoom
	{
		this.distance = this.distance + (val * Time.deltaTime);
		this.distance = Mathf.Clamp(this.distance, this.zoomNearLimit, this.zoomFarLimit);
		this.ignoreHotControl = true;
	}

	public virtual void rotate(float val)//rotate
	{
		this.x = this.x + (((val * Time.deltaTime) * this.xSpeed) * 0.02f);
		this.ignoreHotControl = true;
	}

	public virtual void move(string dir)//move
	{
		switch (dir)
		{
			case "up":
				this.upButtonPressed = true;
				break;
			case "down":
				this.downButtonPressed = true;
				break;
			case "left":
				this.leftButtonPressed = true;
				break;
			case "right":
				this.rightButtonPressed = true;
				break;
		}
		//switch
		this.ignoreHotControl = true;
	}

	public virtual void LateUpdate()//LateUpdate
	{
		Vector3 dir = default(Vector3);
		Vector3 move = default(Vector3);
		if ((GUIUtility.hotControl != 0) && !this.ignoreHotControl)
		{
			return;
		}
		if (!this.isEnabled)
		{
			return;
		}
		if (this.target)
		{
			/*Rotation*/
			if (this.target && (Input.GetAxis("Mouse ScrollWheel") != 0f))
			{
				this.distance = this.distance + Input.GetAxis("Mouse ScrollWheel");
				this.distance = Mathf.Clamp(this.distance, this.zoomNearLimit, this.zoomFarLimit);
			}
			//if    	
			if (Input.GetAxis("Fire1") != 0f)
			{
				this.x = this.x + ((Input.GetAxis("Mouse X") * this.xSpeed) * 0.02f);
				this.y = this.y - ((Input.GetAxis("Mouse Y") * this.ySpeed) * 0.02f);
				this.y = MouseOrbit.ClampAngle(this.y, this.yMinLimit, this.yMaxLimit);
			}
			//if
			Quaternion rotation = Quaternion.Euler(this.y, this.x, 0);
			Vector3 position = (rotation * new Vector3(0f, 0f, -this.distance)) + this.target.position;
			this.transform.rotation = rotation;
			this.transform.position = position;
			/*Movement*/
			if (Input.GetKey("up") || this.upButtonPressed)
			{
				//Debug.Log("Moving Camera");
				dir = this.transform.position - this.target.transform.position;
				move = (dir.normalized * this.moveSpeed) * Time.deltaTime;
				if (Input.GetKey(KeyCode.LeftShift))
				{

					{
						float _27 = this.transform.position.x - (move.x * this.moveMultiplier);
						Vector3 _28 = this.transform.position;
						_28.x = _27;
						this.transform.position = _28;
					}

					{
						float _29 = this.transform.position.z - (move.z * this.moveMultiplier);
						Vector3 _30 = this.transform.position;
						_30.z = _29;
						this.transform.position = _30;
					}

					{
						float _31 = this.target.transform.position.x - (move.x * this.moveMultiplier);
						Vector3 _32 = this.target.transform.position;
						_32.x = _31;
						this.target.transform.position = _32;
					}

					{
						float _33 = this.target.transform.position.z - (move.z * this.moveMultiplier);
						Vector3 _34 = this.target.transform.position;
						_34.z = _33;
						this.target.transform.position = _34;
					}
				}
				else
				{

					{
						float _35 = this.transform.position.x - move.x;
						Vector3 _36 = this.transform.position;
						_36.x = _35;
						this.transform.position = _36;
					}

					{
						float _37 = this.transform.position.z - move.z;
						Vector3 _38 = this.transform.position;
						_38.z = _37;
						this.transform.position = _38;
					}

					{
						float _39 = this.target.transform.position.x - move.x;
						Vector3 _40 = this.target.transform.position;
						_40.x = _39;
						this.target.transform.position = _40;
					}

					{
						float _41 = this.target.transform.position.z - move.z;
						Vector3 _42 = this.target.transform.position;
						_42.z = _41;
						this.target.transform.position = _42;
					}
				}
				if (this.upButtonPressed)
				{
					this.upButtonPressed = false;
				}
			}
			if (Input.GetKey("down") || this.downButtonPressed)
			{
				//Debug.Log("Moving Camera");
				dir = this.transform.position - this.target.transform.position;
				move = (dir.normalized * this.moveSpeed) * Time.deltaTime;
				if (Input.GetKey(KeyCode.LeftShift))
				{

					{
						float _43 = this.transform.position.x + (move.x * this.moveMultiplier);
						Vector3 _44 = this.transform.position;
						_44.x = _43;
						this.transform.position = _44;
					}

					{
						float _45 = this.transform.position.z + (move.z * this.moveMultiplier);
						Vector3 _46 = this.transform.position;
						_46.z = _45;
						this.transform.position = _46;
					}

					{
						float _47 = this.target.transform.position.x + (move.x * this.moveMultiplier);
						Vector3 _48 = this.target.transform.position;
						_48.x = _47;
						this.target.transform.position = _48;
					}

					{
						float _49 = this.target.transform.position.z + (move.z * this.moveMultiplier);
						Vector3 _50 = this.target.transform.position;
						_50.z = _49;
						this.target.transform.position = _50;
					}
				}
				else
				{

					{
						float _51 = this.transform.position.x + move.x;
						Vector3 _52 = this.transform.position;
						_52.x = _51;
						this.transform.position = _52;
					}

					{
						float _53 = this.transform.position.z + move.z;
						Vector3 _54 = this.transform.position;
						_54.z = _53;
						this.transform.position = _54;
					}

					{
						float _55 = this.target.transform.position.x + move.x;
						Vector3 _56 = this.target.transform.position;
						_56.x = _55;
						this.target.transform.position = _56;
					}

					{
						float _57 = this.target.transform.position.z + move.z;
						Vector3 _58 = this.target.transform.position;
						_58.z = _57;
						this.target.transform.position = _58;
					}
				}
				if (this.downButtonPressed)
				{
					this.downButtonPressed = false;
				}
			}
			if (Input.GetKey("left") || this.leftButtonPressed)
			{
				//Debug.Log("Moving Camera");
				dir = this.transform.position - this.target.transform.position;
				//dir += ;
				move = (-this.transform.right * this.moveSpeed) * Time.deltaTime;
				if (Input.GetKey(KeyCode.LeftShift))
				{

					{
						float _59 = this.transform.position.x + (move.x * this.moveMultiplier);
						Vector3 _60 = this.transform.position;
						_60.x = _59;
						this.transform.position = _60;
					}

					{
						float _61 = this.transform.position.z + (move.z * this.moveMultiplier);
						Vector3 _62 = this.transform.position;
						_62.z = _61;
						this.transform.position = _62;
					}

					{
						float _63 = this.target.transform.position.x + (move.x * this.moveMultiplier);
						Vector3 _64 = this.target.transform.position;
						_64.x = _63;
						this.target.transform.position = _64;
					}

					{
						float _65 = this.target.transform.position.z + (move.z * this.moveMultiplier);
						Vector3 _66 = this.target.transform.position;
						_66.z = _65;
						this.target.transform.position = _66;
					}
				}
				else
				{

					{
						float _67 = this.transform.position.x + move.x;
						Vector3 _68 = this.transform.position;
						_68.x = _67;
						this.transform.position = _68;
					}

					{
						float _69 = this.transform.position.z + move.z;
						Vector3 _70 = this.transform.position;
						_70.z = _69;
						this.transform.position = _70;
					}

					{
						float _71 = this.target.transform.position.x + move.x;
						Vector3 _72 = this.target.transform.position;
						_72.x = _71;
						this.target.transform.position = _72;
					}

					{
						float _73 = this.target.transform.position.z + move.z;
						Vector3 _74 = this.target.transform.position;
						_74.z = _73;
						this.target.transform.position = _74;
					}
				}
				if (this.leftButtonPressed)
				{
					this.leftButtonPressed = false;
				}
			}
			if (Input.GetKey("right") || this.rightButtonPressed)
			{
				//Debug.Log("Moving Camera");
				dir = this.transform.position - this.target.transform.position;
				//dir += ;
				move = (this.transform.right * this.moveSpeed) * Time.deltaTime;
				if (Input.GetKey(KeyCode.LeftShift))
				{

					{
						float _75 = this.transform.position.x + (move.x * this.moveMultiplier);
						Vector3 _76 = this.transform.position;
						_76.x = _75;
						this.transform.position = _76;
					}

					{
						float _77 = this.transform.position.z + (move.z * this.moveMultiplier);
						Vector3 _78 = this.transform.position;
						_78.z = _77;
						this.transform.position = _78;
					}

					{
						float _79 = this.target.transform.position.x + (move.x * this.moveMultiplier);
						Vector3 _80 = this.target.transform.position;
						_80.x = _79;
						this.target.transform.position = _80;
					}

					{
						float _81 = this.target.transform.position.z + (move.z * this.moveMultiplier);
						Vector3 _82 = this.target.transform.position;
						_82.z = _81;
						this.target.transform.position = _82;
					}
				}
				else
				{

					{
						float _83 = this.transform.position.x + move.x;
						Vector3 _84 = this.transform.position;
						_84.x = _83;
						this.transform.position = _84;
					}

					{
						float _85 = this.transform.position.z + move.z;
						Vector3 _86 = this.transform.position;
						_86.z = _85;
						this.transform.position = _86;
					}

					{
						float _87 = this.target.transform.position.x + move.x;
						Vector3 _88 = this.target.transform.position;
						_88.x = _87;
						this.target.transform.position = _88;
					}

					{
						float _89 = this.target.transform.position.z + move.z;
						Vector3 _90 = this.target.transform.position;
						_90.z = _89;
						this.target.transform.position = _90;
					}
				}
				if (this.rightButtonPressed)
				{
					this.rightButtonPressed = false;
				}
			}
			if (Input.GetMouseButton(1))
			{
				Vector3 hMove = (this.transform.right * (this.moveSpeed * 2)) * Time.deltaTime;
				Vector3 vMove = (this.transform.up * (this.moveSpeed * 2)) * Time.deltaTime;

				{
					float _91 = this.transform.position.x + (hMove.x * Input.GetAxis("Mouse X"));
					Vector3 _92 = this.transform.position;
					_92.x = _91;
					this.transform.position = _92;
				}

				{
					float _93 = this.transform.position.z + (hMove.z * Input.GetAxis("Mouse X"));
					Vector3 _94 = this.transform.position;
					_94.z = _93;
					this.transform.position = _94;
				}

				{
					float _95 = this.transform.position.y + (vMove.y * Input.GetAxis("Mouse Y"));
					Vector3 _96 = this.transform.position;
					_96.y = _95;
					this.transform.position = _96;
				}

				{
					float _97 = this.target.transform.position.x + (hMove.x * Input.GetAxis("Mouse X"));
					Vector3 _98 = this.target.transform.position;
					_98.x = _97;
					this.target.transform.position = _98;
				}

				{
					float _99 = this.target.transform.position.z + (hMove.z * Input.GetAxis("Mouse X"));
					Vector3 _100 = this.target.transform.position;
					_100.z = _99;
					this.target.transform.position = _100;
				}

				{
					float _101 = this.target.transform.position.y + (vMove.y * Input.GetAxis("Mouse Y"));
					Vector3 _102 = this.target.transform.position;
					_102.y = _101;
					this.target.transform.position = _102;
				}
			}
		}
		//if
		if (this.ignoreHotControl)
		{
			this.ignoreHotControl = false;
		}
	}

	public static float ClampAngle(float angle, float min, float max)//ClampAngle
	{
		if (angle < -360)
		{
			angle = angle + 360;
		}
		if (angle > 360)
		{
			angle = angle - 360;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public MouseOrbit()
	{
		this.distance = 6f;
		this.moveSpeed = 500;
		this.xSpeed = 250f;
		this.ySpeed = 120f;
		this.yMinLimit = -20;
		this.yMaxLimit = 88;
		this.zoomSpeed = 10;
		this.zoomNearLimit = 1;
		this.zoomFarLimit = 200;
		this.moveMultiplier = 0.2f;
		this.isEnabled = true;
	}

}