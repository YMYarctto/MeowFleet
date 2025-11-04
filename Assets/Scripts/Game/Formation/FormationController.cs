using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.InputSystem;

public class FormationController : MonoBehaviour
{
    public Vector2Int MapSize;

    public Ship_UI ShipOnDrag{ get => ship_on_drag; set => ship_on_drag = value; }
    Ship_UI ship_on_drag;

    GridCellGroup gridCellGroup;
    Dictionary<Vector2Int, LayoutDATA> placed_layout;
    List<Vector2Int> placed_map;

    public Canvas canvas;
    public Transform DragGroupTrans;
    public Transform ShipGroupTrans;
    public Transform RaycastGroup;

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
        canvas = GameObject.Find("Canvas_FormationScene").GetComponent<Canvas>();
        DragGroupTrans = GameObject.Find("OnDrag").transform;
        ShipGroupTrans = GameObject.Find("ShipGroup").transform;
        RaycastGroup = GameObject.Find("RaycastGroup").transform;

        InputController.instance.SelectActionMap(ActionMapRegistry.FormationMap);
        placed_layout = new();
        RefreshPlacedMap();
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

    public void RefreshPlacedMap()
    {
        placed_map = new();
        foreach (var kv in placed_layout)
        {
            placed_map.AddRange(kv.Value.LayoutInMap(kv.Key));
            foreach(var coord in kv.Value.GetAdjacentCellsInMap(kv.Key))
            {
                if(CheckCoordInMap(coord))
                {
                    placed_map.Add(coord);
                }
            }
        }
    }

    public void SetDragLayout(Vector2Int center, LayoutDATA layout)
    {
        if (!CheckLayoutInMap(center, layout))
        {
            gridCellGroup.RefreshDrag();
            return;
        }
        gridCellGroup.RefreshDrag();
        foreach (var coord in layout.LayoutInMap(center))
        {
            if (CheckCoordAvailable(coord))
            {
                gridCellGroup.SetDragAllow(coord, true);
            }
            else
            {
                gridCellGroup.SetDragAllow(coord, false);
            }
        }
    }

    public void SetPlacedLayout(Vector2Int center,LayoutDATA layout)
    {
        placed_layout.Add(center, layout);
        RefreshPlacedMap();
    }

    public void RemovePlaced(Vector2Int center)
    {
        if (placed_layout.ContainsKey(center))
        {
            placed_layout.Remove(center);
            RefreshPlacedMap();
        }
    }

    public void RefreshDrag()
    {
        gridCellGroup.RefreshDrag();
    }

    public void DragBegin()
    {
        gridCellGroup.SetPlaced(placed_map);
    }
    
    public void DragEnd()
    {
        gridCellGroup.RefreshAll();
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        ship_on_drag?.Rotate((int)ctx.ReadValue<float>());
    }

    public bool CanPlaced(Vector2Int center,LayoutDATA layout)
    {
        if (!CheckLayoutInMap(center, layout)) return false;
        foreach (var coord in layout.LayoutInMap(center))
        {
            if (!CheckCoordAvailable(coord)) return false;
        }
        return true;
    }

    bool CheckLayoutInMap(Vector2Int center, LayoutDATA layout)
    {
        foreach (var v2 in layout.LayoutInMap(center))
        {
            if (v2.x < 0 || v2.y < 0 || v2.x >= MapSize.x || v2.y >= MapSize.y)
            {
                return false;
            }
        }

        return true;
    }

    bool CheckCoordInMap(Vector2Int coord)
    {
        if (coord.x < 0 || coord.y < 0 || coord.x >= MapSize.x || coord.y >= MapSize.y)
        {
            return false;
        }
        return true;
    }
    
    bool CheckCoordAvailable(Vector2Int coord)
    {
        return !placed_map.Contains(coord);
    }
}
