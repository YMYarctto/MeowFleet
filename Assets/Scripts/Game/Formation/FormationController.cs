using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FormationController : MonoBehaviour
{
    void Awake()
    {
        InputController.instance.SelectActionMap(ActionMapRegistry.FormationMap);
    }

    void OnEnable()
    {
        InputController.InputAction.FormationMap.Rotate.started += OnRotate;
    }

    void OnDisable()
    {
        InputController.InputAction.FormationMap.Rotate.started -= OnRotate;
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx.ReadValue<float>());
    }
}
