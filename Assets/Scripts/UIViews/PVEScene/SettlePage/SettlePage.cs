using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SettlePage : UIView
{
    public override UIView currentView => this;
    protected override UnityAction WaitForAllUIViewAdded => base.Disable;

    Transform bg_victory;
    Transform info_victory;
    Transform info_defeat;

    public override void Init()
    {
        bg_victory = transform.Find("bg_victory");
        Transform info = transform.Find("info");
        info_victory = info.Find("victory");
        info_defeat = info.Find("defeat");
        transform.localScale = new(0,1,1);
    }

    public override void Enable()
    {
        base.Enable();
        transform.DOScaleX(1f,0.2f).SetEase(Ease.OutQuad);
    }

    public void Victory()
    {
        bg_victory.gameObject.SetActive(true);
        info_victory.gameObject.SetActive(true);
        info_defeat.gameObject.SetActive(false);
        Enable();
    }

    public void Defeat()
    {
        bg_victory.gameObject.SetActive(false);
        info_victory.gameObject.SetActive(false);
        info_defeat.gameObject.SetActive(true);
        Enable();
    }
}
