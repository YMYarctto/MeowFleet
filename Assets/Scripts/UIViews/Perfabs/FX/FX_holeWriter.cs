using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FX_holeWriter : FX
{
    Sequence sequence;

    void Awake()
    {
        sequence = DOTween.Sequence();
        sequence.SetAutoKill(false).SetRecyclable(true);
        sequence.Append(transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InQuad));
        sequence.Join(transform.DORotate(new Vector3(0, 0, 90), 0.2f).SetEase(Ease.InQuad));
    }

    void OnEnable()
    {
        transform.localScale = Vector3.one * 30;
        sequence.Restart();
    }

    void OnDisable()
    {
        sequence?.Pause();
        // transform.position = new(-10000,-10000,0);
    }
}
