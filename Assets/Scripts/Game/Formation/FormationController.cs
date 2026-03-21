using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class FormationController : Manager<FormationController>
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
    public bool EmptyMap => placed_layout.Count == 0;

    CanvasGroup canvasGroup;
    Tween tween;

    void Awake()
    {
        Global.PPU = 1f;

        canvas = GameObject.Find("FrontScene").GetComponent<Canvas>();
        DragGroupTrans = GameObject.Find("OnDrag").transform;
        ShipGroupTrans = GameObject.Find("ShipGroup").transform;
        RaycastGroup = GameObject.Find("RaycastGroup").transform;
        canvasGroup = GameObject.Find("FormationPage").GetComponent<CanvasGroup>();

        InputController.instance.SelectActionMap(InputController.InputAction.PVEMap);
        placed_layout = new();
        formation_dict = new();
        placed_map = new();
        RefreshPlacedMap();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;

        //Temp
        EnemyGroup data = DataManager.instance.RandomGetEnemyGroup(1);
        LoadDataManager.instance.PVELoadData.SetLoadData(data);
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
        placed_map.Clear();
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

    public void SavePlacedLayout()
    {
        DataManager.instance.SaveData.SetFormationData(MapSize,formation_dict);
    }

    public void Show()
    {
        tween?.Kill();
        tween = canvasGroup.DOFade(1,0.1f).SetEase(Ease.OutQuad).OnComplete(()=>canvasGroup.interactable =canvasGroup.blocksRaycasts= true);
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
