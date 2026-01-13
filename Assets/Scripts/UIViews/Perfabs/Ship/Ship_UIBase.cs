using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ship_UIBase : UIView
{
    public override UIView currentView => this;

    static int ShipID;
    protected int _ID = ShipID;
    public override int ID => _ID;

    protected Ship ship;

    protected Transform trans;
    protected RectTransform rectTransform;
    protected Image image;
    protected RectTransform img_rect;
    protected Image ui;
    protected RectTransform ui_rect;
    protected Transform core;

    public virtual void Init(Ship ship)
    {
        this.ship = ship;
        trans = transform;
        trans.localPosition = Vector2.zero;
        rectTransform = trans.GetComponent<RectTransform>();

        // 调整大小
        image = trans.Find("sprite").GetComponent<Image>();
        Sprite sprite = ResourceManager.instance.GetShipSprite(ship.DataId);
        image.sprite = sprite;
        img_rect = image.rectTransform;

        ui = trans.Find("UI").GetComponent<Image>();
        Sprite ui_sprite = ResourceManager.instance.CreateSprite(sprite);
        ui.sprite = ui_sprite;
        ui_rect = ui.rectTransform;

        core = trans.Find("core");

        Vector2 spriteSize = new Vector2(sprite.rect.width, sprite.rect.height);
        ui_rect.sizeDelta=img_rect.sizeDelta = spriteSize / Global.PPU;
        img_rect.pivot = new Vector2(
            sprite.pivot.x / sprite.rect.width,
            sprite.pivot.y / sprite.rect.height
        );
        ui_rect.localPosition=img_rect.localPosition = Vector2.zero;
    }

    public static GameObject Create<T>(int id,Ship ship,Transform parent)where T:Ship_UIBase
    {
        ShipID = id;
        var obj = Instantiate(ResourceManager.instance.GetPerfabByType<Ship_UIBase>(),parent,false);
        var ui = obj.AddComponent<T>();
        ui.Init(ship);
        return obj;
    }
}
