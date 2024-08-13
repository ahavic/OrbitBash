using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InputHandler
{
    bool controlsEnabled = false;
    public bool ControlsEnabled
    {
        get => controlsEnabled;
        set
        {
            if(value)
            {
                TurnEnabled = true;
                FireEnabled = true;
                BoostEnabled = true;
                controlsEnabled = true;
            }
        }
    }

    bool turnEnabled = false;
    public bool TurnEnabled
    {
        get => turnEnabled;
        set => turnEnabled = value;
    }

    bool fireEnabled = false;
    public bool FireEnabled
    {
        get => fireEnabled;
        set => fireEnabled = value;
    }

    bool boostEnabled = false;
    public bool BoostEnabled
    {
        get => boostEnabled;
        set => boostEnabled = value;
    }

    #region Basic Movement Inputs

    public float HorizontalInput() =>
        controlsEnabled && turnEnabled ? Input.GetAxisRaw("Horizontal") : 0;
    public bool BoostInput() =>
         controlsEnabled && boostEnabled && Input.GetKey(KeyCode.B);
    public bool BrakeInput() =>
        controlsEnabled && Input.GetKey(KeyCode.Space);

    #endregion

    #region Manuever Inputs

    public bool BarrelRollInput() =>
        controlsEnabled && Input.GetKeyDown(KeyCode.Q);
    public bool LoopInput() =>
        controlsEnabled && Input.GetKeyDown(KeyCode.G);
    public bool UTurnInput() =>
        controlsEnabled && Input.GetKeyDown(KeyCode.U);

    #endregion

    #region Fire Inputs
    public bool MissileInput() =>
        controlsEnabled && fireEnabled && Input.GetKeyDown(KeyCode.F);
    public bool LaserInput() =>
        controlsEnabled && fireEnabled && Input.GetKey(KeyCode.L);
    public bool BombInput() =>
        controlsEnabled && fireEnabled && Input.GetKeyDown(KeyCode.M);
    #endregion

    public bool DisplayPlayerList() =>
        controlsEnabled && Input.GetKey(KeyCode.Tab);
}
