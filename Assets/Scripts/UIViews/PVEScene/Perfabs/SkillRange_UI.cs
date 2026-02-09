using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRange_UI : UIView
{
    public override UIView currentView => this;

    int _id = SkillCard_UI.CardID;
    public override int ID => _id;

    RectTransform viewport;
    RectTransform content;

    public override void Init()
    {
        viewport = GetComponent<RectTransform>();
        content = transform.Find("content").GetComponent<RectTransform>();
    }

    public void Init(List<Vector2Int> coords)
    {
        Block.Group(Block.State.range,coords,content,viewport);
    }
}
