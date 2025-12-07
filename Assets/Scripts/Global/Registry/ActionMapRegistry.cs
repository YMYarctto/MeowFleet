using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class ActionMapRegistry
{
    static GameAction inputAction { get => InputController.InputAction; }
    
    public static InputActionMap DefaultMap{ get => inputAction.DefaultMap; }
    public static InputActionMap FormationMap{ get => inputAction.FormationMap; }
    public static InputActionMap PVEMap{ get => inputAction.PVEMap; }
}

