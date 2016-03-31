using UnityEngine;
using System.Collections;
using InControl;

public class Controls : MonoBehaviour
{

    #region Public Unity Fields
    [Header("Config")]
    public int playerNum; //(values [0-3])
    public bool InvertSticks = false;
    public enum Mode { Controller = 0, MouseKeyboard }
    public Mode ControlsMode = Mode.Controller;
    #endregion

    #region Control Properties
    public Vector2 MoveStick
    {
        get
        {
            return (ControlsMode == Mode.Controller) ? LStickVector : WASDVector;
        }
    }

    public Vector2 AimStick
    {
        get
        {
            return (ControlsMode == Mode.Controller) ? RStickVector : ArrowKeyVector;
        }
    }

    public float YawAxis
    {
        get
        {
            float axis = 0f;
            if (ControlsMode == Mode.Controller)
            {
                if (ID.LeftBumper) axis -= 1f;
                if (ID.RightBumper) axis += 1f;
                return axis;
            }
            if (Input.GetKey(KeyCode.Q)) axis -= 1f;
            if (Input.GetKey(KeyCode.E)) axis += 1f;
            return axis;
        }
    }

    public Vector2 LStickVector
    {
        get
        {
            return InvertSticks ?
                Vector2.ClampMagnitude(ID.RightStick.Vector, 1f) :
                Vector2.ClampMagnitude(ID.LeftStick.Vector, 1f);
        }
    }

    public Vector2 RStickVector
    {
        get
        {
            return InvertSticks ?
                Vector2.ClampMagnitude(ID.LeftStick.Vector, 1f) :
                Vector2.ClampMagnitude(ID.RightStick.Vector, 1f);
        }
    }

    public Vector2 WASDVector
    {
        get
        {
            Vector2 axis = Vector2.zero;
            if (Input.GetKey(KeyCode.W)) axis += Vector2.up;
            if (Input.GetKey(KeyCode.A)) axis += Vector2.left;
            if (Input.GetKey(KeyCode.S)) axis += Vector2.down;
            if (Input.GetKey(KeyCode.D)) axis += Vector2.right;
            return axis.normalized;
        }
    }

    public Vector2 ArrowKeyVector
    {
        get
        {
            Vector2 axis = Vector2.zero;
            if (Input.GetKey(KeyCode.UpArrow)) axis += Vector2.up;
            if (Input.GetKey(KeyCode.LeftArrow)) axis += Vector2.left;
            if (Input.GetKey(KeyCode.DownArrow)) axis += Vector2.down;
            if (Input.GetKey(KeyCode.RightArrow)) axis += Vector2.right;
            return axis.normalized;
        }
    }

    public Vector3 MousePosition
    {
        get
        {
            Vector3 mouse = Input.mousePosition;
            if (!Camera.main.orthographic)
            {
                mouse.z = Camera.main.farClipPlane;
            }
            var mouse3d = Camera.main.ScreenToWorldPoint(mouse);
            mouse3d.y = 0f;
            return mouse3d;
        }
    }

    public bool FireButtonWasPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.RightTrigger.WasPressed;
            return Input.GetMouseButtonDown(0);
        }
    }

    public bool FireButtonIsPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.RightTrigger.IsPressed;
            return Input.GetMouseButton(0);
        }
    }

    public void VibrateFor(float intensity, float time)
    {
        VibrateFor(intensity, intensity, time);
    }

    public void VibrateFor(float leftMotor, float rightMotor, float time)
    {
        leftMotorInt = leftMotor;
        rightMotorInt = rightMotor;
        currVibrationTime = time;
    }
    #endregion

    #region General Use Properties
    /// <summary>
    /// Returns true if the InControl instance is currently valid.
    /// </summary>
    public bool isActive
    {
        get { return inputDevice != null; }
    }

    /// <summary>
    /// Returns the InControl reference.
    /// Returns null if the controller is disconnected.
    /// </summary>
    public InputDevice ID
    {
        get { return inputDevice; }
    }
    #endregion

    #region Internal Methods
    private InputDevice inputDevice = null;
    private bool fallbackMode = false;

    private float leftMotorInt = 0f;
    private float rightMotorInt = 0f;
    private float currVibrationTime = 0f;

    InputDevice UpdateState()
    {
        return (InputManager.Devices.Count > playerNum) ?
                InputManager.Devices[playerNum] : null;
    }

    void ControlsFBCheck()
    {
        //fallback to M+KB when controller not detected
        if (!isActive && ControlsMode == Mode.Controller)
        {
            print("Controller(" + playerNum + ") not detected, "
                + "Falling back on M+KB Controls.");
            ControlsMode = Mode.MouseKeyboard;
            fallbackMode = true;
        }
        //switch controller input back on when reconnected
        if (fallbackMode && isActive)
        {
            print("Controller(" + playerNum + ") connected, "
                + "Switched to Controller Input.");
            ControlsMode = Mode.Controller;
            fallbackMode = false;
        }
    }

    void UpdateVibration()
    {
        if (currVibrationTime > 0f)
        {
            currVibrationTime -= Time.deltaTime;
            if (isActive) ID.Vibrate(leftMotorInt, rightMotorInt);
        }
        else
        {
            if (isActive) ID.Vibrate(0f);
            currVibrationTime = 0f;
            leftMotorInt = 0f;
            rightMotorInt = 0f;
        }
    }

    void Start()
    {
        inputDevice = UpdateState();
        ControlsFBCheck();
    }

    void FixedUpdate()
    {
        inputDevice = UpdateState();
        ControlsFBCheck();
        UpdateVibration();
    }
    #endregion
}
