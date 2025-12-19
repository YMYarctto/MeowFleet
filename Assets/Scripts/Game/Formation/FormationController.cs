using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.InputSystem;

public class FormationController : MonoBehaviour
{
    public Vector2Int MapSize;

    public Ship_Formation ShipOnDrag{ get => ship_on_drag; set => ship_on_drag = value; }
    Ship_Formation ship_on_drag;

    GridCellGroup_Formation gridCellGroup;
    Dictionary<Vector2Int, LayoutDATA> placed_layout;
    Dictionary<Vector2Int,int> formation_dict;
    List<Vector2Int> placed_map;

    public Canvas canvas{ get; private set; }
    public Transform DragGroupTrans{ get; private set; }
    public Transform ShipGroupTrans{ get; private set; }
    public Transform RaycastGroup{ get; private set; }

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

        InputController.instance.SelectActionMap(InputController.InputAction.PVEMap);
        placed_layout = new();
        formation_dict = new();
        RefreshPlacedMap();
    }

    void Start()
    {
        gridCellGroup = UIManager.instance.GetUIView<GridCellGroup_Formation>();
    }

    void OnEnable()
    {
        InputController.InputAction.PVEMap.Rotate.started += OnRotate;
    }

    void OnDisable()
    {
        InputController.InputAction.PVEMap.Rotate.started -= OnRotate;
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

    public void SetPlacedLayout(Vector2Int center,LayoutDATA layout,int id)
    {
        placed_layout.Add(center, layout);
        formation_dict.Add(center, id);
        RefreshPlacedMap();
        DataManager.instance.SaveData.SetFormationData(MapSize,formation_dict);
    }

    public void RemovePlaced(Vector2Int center)
    {
        if (placed_layout.ContainsKey(center))
        {
            placed_layout.Remove(center);
            formation_dict.Remove(center);
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
