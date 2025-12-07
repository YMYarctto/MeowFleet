using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BG_PVE : UIView
{
    public override UIView currentView => this;

    Tween tween;
    bool page_on_player;

    public override void Init()
    {
    }

    public void NextPage()
    {
        int page_index = 0;
        if (!page_on_player)
        {
            page_index = 1;
        }
        page_on_player = !page_on_player;
        tween?.Kill();
        tween = transform.DOLocalMoveY(page_index * 1440, 0.2f).SetEase(Ease.InOutQuad);
    }
}
