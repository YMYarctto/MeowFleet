using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BG_PVE : UIView
{
    public override UIView currentView => this;

    Tween tween;
    bool page_on_player;

    GameObject Interaction;

    public override void Init()
    {
        Interaction = transform.Find("Interaction").gameObject;
    }

    public void NextPage()
    {
        int page_index = 0;
        if (!page_on_player)
        {
            page_index = -1;
        }
        page_on_player = !page_on_player;
        tween?.Kill();
        tween = transform.DOLocalMoveX(page_index * 1920, 0.2f).SetEase(Ease.InOutQuad);
    }

    public void SetInteractionActive(bool active)
    {
        Interaction.SetActive(active);
    }
}
