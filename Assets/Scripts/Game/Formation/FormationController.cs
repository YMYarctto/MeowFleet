using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class FormationController : MonoBehaviour
{
    void Awake()
    {
        InputController.instance.SelectActionMap(ActionMapRegistry.FormationMap);
        InputController.InputAction.FormationMap.Rotate.started += OnRotate;
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx.ReadValue<float>());
    }
}
