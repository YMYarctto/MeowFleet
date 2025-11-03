using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FormationController : MonoBehaviour
{
    public Ship_UI ShipOnDrag{ get => ship_on_drag; set => ship_on_drag = value; }
    Ship_UI ship_on_drag;

    GridCellGroup gridCellGroup;
    List<KeyValuePair<Vector2Int, LayoutDATA>> formation_layout;

    private static FormationController _instance;
    public static FormationController instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(FormationController)) as FormationController;
                if (!_instance)
                {
                    Debug.LogError("场景中未找到 FormationController");
                    return null;
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        InputController.instance.SelectActionMap(ActionMapRegistry.FormationMap);
    }

    void Start()
    {
        gridCellGroup = UIManager.instance.GetUIView<GridCellGroup>();
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
        ship_on_drag?.Rotate((int)ctx.ReadValue<float>());
    }
}
