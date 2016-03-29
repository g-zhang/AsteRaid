using UnityEngine;
using System.Collections;
using InControl;

public class Controls : MonoBehaviour  {
    public enum Mode { Controller = 0, MouseKeyboard, Both }

    [Header("Config")]
    public int playerNum;
    public Mode ControlsMode = Mode.Both;

    private InputDevice inputDevice = null;

    #region Public Properties
    public bool isActive
    {
        get { return inputDevice != null; }
    }

    public InputDevice ID
    {
        get { return inputDevice; }
    }
    #endregion

    #region Internal Methods
    InputDevice UpdateState()
    {
        return (InputManager.Devices.Count > playerNum) ?
                InputManager.Devices[playerNum] : null;
    }

    void Start()
    {
        inputDevice = UpdateState();
    }

    void FixedUpdate()
    {
        inputDevice = UpdateState();
    }
    #endregion


    #region Controls Properties + Methods
    public Vector2 MoveDir(Vector3 origin, float maxDistance)
    {
        Vector2 ret = Vector2.zero;
        if(ControlsMode == Mode.MouseKeyboard || ControlsMode == Mode.Both)
        {
            Vector3 mousePos2D = Input.mousePosition;
            mousePos2D.z = -Camera.main.transform.position.z;
            Vector3 mousePos3d = Camera.main.ScreenToWorldPoint(mousePos2D);
            Vector3 dir3 = mousePos3d - origin;
            Vector2 dir = new Vector2(dir3.x, dir3.y);
            ret = Vector2.ClampMagnitude(dir, maxDistance);
        }
        if(isActive && (ControlsMode == Mode.Controller || ControlsMode == Mode.Both))
        {
            Vector2 dir = Vector2.ClampMagnitude(ID.LeftStick, 1f);
            ret = dir != Vector2.zero ? dir * maxDistance : ret;
        }

        return ret;
    }

    public Vector2 MoveDir(Vector2 origin, float maxDistance)
    {
        return MoveDir(new Vector3(origin.x, origin.y, 0f), maxDistance);
    }

    public bool SetPoint
    {
        get
        {
            bool ret = false;
            if(ControlsMode == Mode.MouseKeyboard && Input.GetMouseButtonDown(0))
            {
                ret = true;
            }
            if (ControlsMode == Mode.Controller && isActive && ID.Action1.WasPressed)
            {
                ret = true;
            }
            if(ControlsMode == Mode.Both && (Input.GetMouseButtonDown(0) || (isActive && ID.Action1.WasPressed)))
            {
                ret = true;
            }
            return ret;
        }
    }

    public bool UnsetPoint
    {
        get
        {
            bool ret = false;
            if (ControlsMode == Mode.MouseKeyboard && Input.GetMouseButtonDown(1))
            {
                ret = true;
            }
            if (ControlsMode == Mode.Controller && isActive && ID.Action2.WasPressed)
            {
                ret = true;
            }
            if (ControlsMode == Mode.Both && (Input.GetMouseButtonDown(1) || (isActive && ID.Action2.WasPressed)))
            {
                ret = true;
            }
            return ret;
        }
    }

    public bool SetFrame
    {
        get
        {
            bool ret = false;
            if (ControlsMode == Mode.MouseKeyboard && Input.GetKeyDown(KeyCode.E))
            {
                ret = true;
            }
            if (ControlsMode == Mode.Controller && isActive && ID.Action3.WasPressed)
            {
                ret = true;
            }
            if (ControlsMode == Mode.Both && (Input.GetKeyDown(KeyCode.E) || (isActive && ID.Action3.WasPressed)))
            {
                ret = true;
            }
            return ret;
        }
    }

    public bool ActivateFrames
    {
        get
        {
            bool ret = false;
            if (ControlsMode == Mode.MouseKeyboard && Input.GetKeyDown(KeyCode.Space))
            {
                ret = true;
            }
            if (ControlsMode == Mode.Controller && isActive && ID.Action4.WasPressed)
            {
                ret = true;
            }
            if (ControlsMode == Mode.Both && (Input.GetKeyDown(KeyCode.Space) || (isActive && ID.Action4.WasPressed)))
            {
                ret = true;
            }
            return ret;
        }
    }
    #endregion

}
