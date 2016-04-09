using UnityEngine;
using System.Collections;
using InControl;
using System;

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
            return (ControlsMode == Mode.Controller) ? RStickVector : new Vector2(ToMouseVector.x, ToMouseVector.z);
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

    public Vector3 ToMouseVector
    {
        get
        {
            return (MousePosition - transform.position).normalized;
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

    public bool CycleButtonWasPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.RightStickButton.WasPressed;
            return Input.GetMouseButtonDown(1);
        }
    }

    public bool CycleButtonIsPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.RightStickButton.IsPressed;
            return Input.GetMouseButton(1);
        }
    }

    public bool BoostButtonWasPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.LeftStickButton.WasPressed || ID.LeftBumper.WasPressed;
            return Input.GetKeyDown(KeyCode.LeftShift);
        }
    }

    public bool BoostButtonIsPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.LeftStickButton.IsPressed || ID.LeftBumper.IsPressed;
            return Input.GetKey(KeyCode.LeftShift);
        }
    }

    public bool SpinButtonWasPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.RightBumper.WasPressed;
            return Input.GetKeyDown(KeyCode.Z);
        }
    }

    public bool SpinButtonIsPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.RightBumper.IsPressed;
            return Input.GetKey(KeyCode.Z);
        }
    }

    public bool StartWasPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.GetControl(InputControlType.Start).WasPressed;
            return Input.GetKeyDown(KeyCode.Return);
        }
    }

    public bool StartIsPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.GetControl(InputControlType.Start).IsPressed;
            return Input.GetKey(KeyCode.Return);
        }
    }

    public bool SelectWasPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.GetControl(InputControlType.Back).WasPressed;
            return Input.GetKeyDown(KeyCode.Backspace);
        }
    }

    public bool SelectIsPressed
    {
        get
        {
            if (ControlsMode == Mode.Controller) return ID.GetControl(InputControlType.Back).IsPressed;
            return Input.GetKey(KeyCode.Backspace);
        }
    }

    public bool InvincibliltyOn
    {
        get
        {
            if (((Input.GetKeyDown(ParseEnum<KeyCode>("Alpha" + (playerNum + 1).ToString())))
                && (Input.GetKey(KeyCode.I))) || (ControlsMode == Mode.Controller && KonamiActivated))
            {
                KonamiActivated = true;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool InvincibliltyOff
    {
        get
        {
            if (((Input.GetKeyDown(ParseEnum<KeyCode>("Alpha" + (playerNum + 1).ToString())))
                && (Input.GetKey(KeyCode.O))) || (ControlsMode == Mode.Controller && !KonamiActivated))
            {
                ResetKonamiState();
                return true;
            }
            else
            {
                return false;
            }
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

    public static T ParseEnum<T>(string value)
    {   // stackoverflow.com/questions/16100
        return (T)Enum.Parse(typeof(T), value, true);
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
            //print("Controller(" + playerNum + ") not detected, "
            //    + "Falling back on M+KB Controls.");
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

    void UpdateVibration(float dtime)
    {
        if (currVibrationTime > 0f)
        {
            currVibrationTime -= dtime;
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

    void Update()
    {
        if ((GameManager.GM != null && GameManager.GM.isPaused) ||
           (GameManager.GM == null && Time.timeScale == 0f))
        {
            inputDevice = UpdateState();
            ControlsFBCheck();
            UpdateVibration(Time.deltaTime);
        }
        if (isActive)
        {
            KonamiCodeUpdate();
        }
    }

    void FixedUpdate()
    {
        inputDevice = UpdateState();
        ControlsFBCheck();
        UpdateVibration(Time.fixedDeltaTime);
    }
    #endregion

    #region Konami Code
    private bool KonamiActivated = false;
    private enum KonamiCode { Begin = 0, U1, U2, D1, D2, L1, R1, L2, R2, B, A, size }
    private KonamiCode kcstate = KonamiCode.Begin;
    private float kcButtonPressCooldown = 1f;
    private float currKCPressTime = 0f;

    void KonamiCodeUpdate()
    {
        if (!KonamiActivated)
        {
            if (kcstate == KonamiCode.Begin && ID.DPad.Up.WasPressed)
            {
                kcstate = KonamiCode.U1;
                currKCPressTime = kcButtonPressCooldown;
            }
            else if (currKCPressTime > 0f)
            {
                currKCPressTime -= Time.deltaTime;
                switch (kcstate)
                {
                    case KonamiCode.U1:
                        if (ID.DPad.Up.WasPressed)
                        {
                            kcstate++;
                            currKCPressTime += kcButtonPressCooldown;
                        }
                        else if (ID.DPad.Down.WasPressed)
                        {
                            currKCPressTime -= kcButtonPressCooldown;
                        }
                        break;

                    case KonamiCode.U2:
                        if (ID.DPad.Down.WasPressed)
                        {
                            kcstate++;
                            currKCPressTime += kcButtonPressCooldown;
                        }
                        else if (ID.DPad.Up.WasPressed)
                        {
                            currKCPressTime -= kcButtonPressCooldown;
                        }
                        break;

                    case KonamiCode.D1:
                        if (ID.DPad.Down.WasPressed)
                        {
                            kcstate++;
                            currKCPressTime += kcButtonPressCooldown;
                        }
                        else if (ID.DPad.Up.WasPressed)
                        {
                            currKCPressTime -= kcButtonPressCooldown;
                        }
                        break;

                    case KonamiCode.D2:
                        if (ID.DPad.Left.WasPressed)
                        {
                            kcstate++;
                            currKCPressTime += kcButtonPressCooldown;
                        }
                        else if (ID.DPad.Right.WasPressed)
                        {
                            currKCPressTime -= kcButtonPressCooldown;
                        }
                        break;

                    case KonamiCode.L1:
                        if (ID.DPad.Right.WasPressed)
                        {
                            kcstate++;
                            currKCPressTime += kcButtonPressCooldown;
                        }
                        else if (ID.DPad.Left.WasPressed)
                        {
                            currKCPressTime -= kcButtonPressCooldown;
                        }
                        break;
                    case KonamiCode.R1:
                        if (ID.DPad.Left.WasPressed)
                        {
                            kcstate++;
                            currKCPressTime += kcButtonPressCooldown;
                        }
                        else if (ID.DPad.Right.WasPressed)
                        {
                            currKCPressTime -= kcButtonPressCooldown;
                        }
                        break;
                    case KonamiCode.L2:
                        if (ID.DPad.Right.WasPressed)
                        {
                            kcstate++;
                            currKCPressTime += kcButtonPressCooldown;
                        }
                        else if (ID.DPad.Left.WasPressed)
                        {
                            currKCPressTime -= kcButtonPressCooldown;
                        }
                        break;
                    case KonamiCode.R2:
                        if (ID.Action2.WasPressed)
                        {
                            kcstate++;
                            currKCPressTime += kcButtonPressCooldown;
                        }
                        else if (ID.DPad)
                        {
                            currKCPressTime -= kcButtonPressCooldown;
                        }
                        break;
                    case KonamiCode.B:
                        if (ID.Action1.WasPressed)
                        {
                            kcstate++;
                            currKCPressTime += kcButtonPressCooldown;
                        }
                        else if (ID.DPad)
                        {
                            currKCPressTime -= kcButtonPressCooldown;
                        }
                        break;
                    case KonamiCode.A:
                        //Debug.Log("Player(" + playerNum + ") has activated the Konami code!");
                        KonamiActivated = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                kcstate = KonamiCode.Begin;
                currKCPressTime = 0f;
            }
        }
        else //KonamiActivated
        {
            if (ID.LeftStickButton.WasPressed && ID.RightStickButton.WasPressed)
            {
                ResetKonamiState();
            }
        }
    }

    void ResetKonamiState()
    {
        //Debug.Log("Player(" + playerNum + ") has de-activated the Konami code!");
        kcstate = KonamiCode.Begin;
        currKCPressTime = 0f;
        KonamiActivated = false;
    }
    #endregion
}
