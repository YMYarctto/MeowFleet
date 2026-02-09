using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class InformationWindow_UI : UIView
{
    public override UIView currentView => this;
    public override int ID => _id;

    readonly int _id = EnemyController.InformationWindowID;

    static readonly Vector2 sizeDelta = new Vector2(285,357);

    TMP_Text title;
    RectTransform content;
    RectTransform viewport;
    RectTransform rectTransform;

    Sequence sequence;

    public override void Init()
    {
        title = transform.Find("title").GetComponent<TMP_Text>();
        viewport = transform.Find("viewport").GetComponent<RectTransform>();
        content = viewport.Find("content").GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = Vector2.zero;

        sequence = DOTween.Sequence();
        sequence.SetAutoKill(false);
        sequence.Append(rectTransform.DOSizeDelta(sizeDelta,0.1f).SetEase(Ease.InQuad));
        sequence.Pause();
    }

    public void Enable(Vector3 v3)
    {
        transform.position = PVEController.instance.UICamera.ScreenToWorldPoint(v3);
        sequence.PlayForward();
    }

    public override void Disable()
    {
        sequence.PlayBackwards();
    }

    public void Init(List<Vector2Int> coords,int coreNum,string str)
    {
        title.text = str;
        Block.Group(Block.State.body,coords,content,viewport,coreNum);
    }
}
