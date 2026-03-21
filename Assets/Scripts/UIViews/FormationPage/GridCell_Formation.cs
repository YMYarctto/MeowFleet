using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell_Formation : UIView<GridCell_Formation>
{
    static int pendingID;
    GameObject allow;
    GameObject forbid;
    Transform raycast;

    Transform raycast_group=>FormationController.instance.RaycastGroup;

    int _ID = pendingID;
    public override int ID => _ID;

    public static void PrepareNextID(int id)
    {
        pendingID = id;
    }

    public override void Init()
    {
        allow = transform.Find("allow").gameObject;
        forbid = transform.Find("forbid").gameObject;
        raycast = transform.Find("raycast");
        raycast.gameObject.name = $"raycast_{_ID}";
        raycast.GetComponent<GridCell_raycast>().Parent = this;
        Disable();
    }

    void Start()
    {
        raycast.SetParent(raycast_group, false);
    }

    public void Allow(bool isAllow)
    {
        allow.SetActive(isAllow);
        forbid.SetActive(!isAllow);
    }

    public override void Disable()
    {
        allow.SetActive(false);
        forbid.SetActive(false);
    }

    public Vector2Int GetVector2Int()
    {
        return new(ID % 10, ID / 10);
    }
}
