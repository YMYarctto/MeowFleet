using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMap : UIView<EnemyMap>
{
    Transform SVGImage;
    Transform GridCellGroup;

    readonly int CELLPX = 80;
    readonly Vector2 SVGOFFSET = new(40, 40);

    public override void Init()
    {
        SVGImage = transform.Find("SVGImage");
        GridCellGroup = transform.Find("GridCellGroup");
        SetMap(LoadDataManager.instance.PVELoadData.EnemyMapSize);
    }

    public void SetMap(int size)
    {
        GridCellGroup.GetComponent<RectTransform>().sizeDelta = Vector2.one * CELLPX * size;
        RectTransform SVGRect = SVGImage.GetComponent<RectTransform>();
        SVGRect.sizeDelta = Vector2.one * CELLPX * size + SVGOFFSET;
        if (size % 2 != 0)
        {
            SVGRect.anchoredPosition += Vector2.one * CELLPX / 2;
        }
    }
}
