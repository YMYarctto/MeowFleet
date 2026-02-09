using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BG_PVE : UIView
{
    public override UIView currentView => this;

    Tween tween;
    bool page_on_enemy;

    public override void Init()
    {
    }

    public void NextPage()
    {
        int page_index = page_on_enemy ? 0 : 1;
        page_on_enemy = !page_on_enemy;
        tween?.Kill();
        tween = transform.DOLocalMoveY(page_index * 1440, 0.25f).SetEase(Ease.InOutQuad);
    }

    public void PlayerPage()
    {
        if (page_on_enemy)
        {
            NextPage();
        }
    }

    public void EnemyPage()
    {
        if (!page_on_enemy)
        {
            NextPage();
        }
    }
}
