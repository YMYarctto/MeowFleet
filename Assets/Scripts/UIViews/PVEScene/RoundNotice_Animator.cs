using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RoundNotice_Animator : UIView
{
    public override UIView currentView => this;

    TMP_Text round;

    Sequence sequence;

    public override void Init()
    {
        round = transform.Find("round").GetComponent<TMP_Text>();
        round.text = "1";
    }

    public void ChangeRound(int r)
    {
        sequence?.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(round.DOFade(0,0.25f));
        sequence.AppendCallback(() => round.text = r.ToString());
        sequence.Append(round.DOFade(1,0.25f));
        sequence.Play();
    }
}
