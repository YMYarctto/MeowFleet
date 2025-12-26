using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipContainer : UIView
{
    public override UIView currentView => this;

    static int ShipID;
    protected int _ID = ShipID;
    public override int ID => _ID;

    RectTransform _rectTransform;
    public RectTransform rectTransform=>_rectTransform;

    public override void Init()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Init(int id,Ship ship)
    {
        GameObject ship_obj = Ship_UIBase.Create<Ship_Formation>(id,ship,transform);
        _rectTransform.sizeDelta = new(ship_obj.GetComponent<Ship_Formation>().SizeDelta.x,_rectTransform.sizeDelta.y);
    }

    public static ShipContainer Create(int id,Ship ship,Transform parent)
    {
        ShipID = id;
        var obj = Instantiate(ResourceManager.instance.GetPerfabByType<ShipContainer>(),parent,false);
        var ui = obj.AddComponent<ShipContainer>();
        ui.Init(id,ship);
        return ui;
    }
}
