using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AxisType
{
    Axis,
    Trigger
}

public class InputAxis
{
    private enum AxisState
    {
        Idle = -1,
        UP = 0,
        DOWN = 1,
        STAY = 2,
    }

    private string axisName;
    private AxisState state = AxisState.Idle;
    private AxisType type = AxisType.Axis;

    public InputAxis(string axisName, AxisType type)
    {
        this.axisName = axisName;
        this.type = type;
    }

    public bool Down
    {
        get
        {
            Update();
            return state == AxisState.DOWN;
        }
    }
    public bool Stay
    {
        get
        {
            Update();
            return state == AxisState.STAY;
        }
    }
    public bool Up
    {
        get
        {
            Update();
            return state == AxisState.UP;
        }
    }

    public float Value
    {
        get { return Input.GetAxisRaw(axisName); }
    }
    public int RawValue
    {
        get
        {
            return Input.GetAxisRaw(axisName) != 0 ? 
                Input.GetAxisRaw(axisName) > 0 ? 
                1 : -1 : 0;
        }
    }

    private void Update()
    {
        switch (type)
        {
            case AxisType.Trigger:
                if (state == AxisState.Idle)
                {
                    if (Input.GetAxisRaw(axisName) > 0)
                        state = AxisState.DOWN;
                }
                else if (state > 0)
                {
                    if (Input.GetAxisRaw(axisName) > 0)
                        state = AxisState.STAY;
                    else
                        state = AxisState.UP;
                }
                else state = AxisState.Idle;
                break;

            case AxisType.Axis:
                if (state == AxisState.Idle)
                {
                    if (Input.GetAxisRaw(axisName) != 0)
                        state = AxisState.DOWN;
                }
                else if (state > 0)
                {
                    if (Input.GetAxisRaw(axisName) != 0)
                        state = AxisState.STAY;
                    else
                        state = AxisState.UP;
                }
                else state = AxisState.Idle;
                break;

            default:
                break;
        }
    }
}
