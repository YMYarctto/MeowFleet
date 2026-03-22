using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMap : UIView<EnemyMap>
{
    Transform Block;
    Transform GridCellGroup;

    readonly int CELLPX = 80;
    readonly float STANDARDPX = 960f;

    public override void Init()
    {
        Block = transform.Find("Block");
        GridCellGroup = transform.Find("GridCellGroup");
        SetMap(LoadDataManager.instance.PVELoadData.EnemyMapSize);
    }

    public void SetMap(int size)
    {
        RectTransform GridRect = GridCellGroup.GetComponent<RectTransform>();
        RectTransform BlockRect = Block.GetComponent<RectTransform>();
        GridRect.sizeDelta = Vector2.one * CELLPX * size;
        Block.localScale = Vector2.one * (CELLPX * size / STANDARDPX);
        if (size % 2 != 0)
        {
            BlockRect.anchoredPosition += Vector2.one * CELLPX / 2;
            GridRect.anchoredPosition += Vector2.one * CELLPX / 2;
        }
    }
}
