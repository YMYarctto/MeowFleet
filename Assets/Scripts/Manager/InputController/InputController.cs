using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    static GameAction _inputAction;
    public static GameAction InputAction { get => _inputAction; }

    private static InputController _instance;
    public static InputController instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<InputController>();
                if (!_instance)
                {
                    return null;
                }
            }
            return _instance;
        }
    }

    public void Init()
    {
        _inputAction = new();
        _inputAction.Enable();
        CloseAllActionMaps();
    }

    public void CloseAllActionMaps()
    {
        foreach (var action_map in _inputAction.asset.actionMaps)
        {
            action_map.Disable();
        }
    }

    public void SelectActionMap(InputActionMap map)
    {
        CloseAllActionMaps();
        map.Enable();
    }
}
