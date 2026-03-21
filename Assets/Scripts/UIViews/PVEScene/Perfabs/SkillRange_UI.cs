using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRange_UI : UIView<SkillRange_UI>
{
    static int pendingID;
    int _id = pendingID;
    public override int ID => _id;

    RectTransform viewport;
    RectTransform content;

    public static void PrepareNextID(int id)
    {
        pendingID = id;
    }

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
