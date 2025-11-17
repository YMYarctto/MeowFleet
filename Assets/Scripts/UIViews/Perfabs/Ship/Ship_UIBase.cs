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

    public virtual void Init(Ship ship)
    {
        this.ship = ship;
        trans = transform;
        trans.localPosition = Vector2.zero;
        rectTransform = trans.GetComponent<RectTransform>();

        // 调整大小
        image = trans.Find("sprite").GetComponent<Image>();
        Sprite sprite = ResourceManager.instance.GetSprite(ship.Uid);
        image.sprite = sprite;
        RectTransform img_rectTransform = image.rectTransform;
        Vector2 spriteSize = new Vector2(sprite.rect.width, sprite.rect.height);
        img_rectTransform.sizeDelta = spriteSize / Global.PPU;
        img_rectTransform.pivot = new Vector2(
            sprite.pivot.x / sprite.rect.width,
            sprite.pivot.y / sprite.rect.height
        );
        img_rectTransform.localPosition = Vector2.zero;
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
