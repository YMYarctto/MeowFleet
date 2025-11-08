using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pointer_Animation : MonoBehaviour
{
    Animator animator;
    Sequence seq;
    Transform up_trans;
    Image up_image;
    Color up_color;

    float progress = 0f;
    float up_target_y = 200f;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        animator.speed = 0f;
        up_trans = transform.Find("pointer_up");
        up_image = up_trans.GetComponent<Image>();
        up_color = up_image.color;

        up_color.a = 0f;
        up_image.color = up_color;
        up_trans.localPosition = new Vector3(0f, up_target_y, 0f);
    }

    private void ApplyProgress()
    {
        animator.Play("focus", 0, progress);
    }

    public void PlayForward()
    {
        seq?.Kill(false);
        animator.speed = 0f;
        float startProgress = 1f - progress;
        seq = DOTween.Sequence();

        seq.Append(DOTween.To(() => progress, x =>
        {
            progress = x;
            ApplyProgress();
            if (progress <= 0.5f)
            {
                up_color.a = progress * 2f;
                up_image.color = up_color;
            }
        }, 1f, startProgress * 0.5f));
        seq.Join(up_trans.DOLocalMoveY(0f, startProgress * 0.5f).SetEase(Ease.OutQuart));
        
        seq.Play();
    }

    public void PlayBackward()
    {
        seq?.Kill(false);
        animator.speed = 0f;
        float startProgress = progress;
        seq = DOTween.Sequence();

        if(progress>=0.5f)
        {
            seq.Append(DOTween.To(() => progress, x =>
            {
                progress = x;
                ApplyProgress();
            }, 1f, (1f-startProgress)*0.5f));

            seq.AppendCallback(() =>
            {
                startProgress = progress = 0.5f;
                ApplyProgress();
            });

            seq.Append(DOTween.To(() => progress, x =>
            {
                progress = x;
                ApplyProgress();
                up_color.a = progress * 2f;
                up_image.color = up_color;
            }, 0f, startProgress * 0.5f));
            seq.Join(up_trans.DOLocalMoveY(up_target_y, startProgress * 0.5f).SetEase(Ease.InQuad));
        }
        else
        {
            seq.Append(DOTween.To(() => progress, x =>
            {
                progress = x;
                ApplyProgress();
                if(progress<=0.5f)
                {
                    up_color.a = progress*2f;
                    up_image.color = up_color;
                }
            }, 0f, startProgress * 0.5f));
            seq.Join(up_trans.DOLocalMoveY(up_target_y, startProgress * 0.5f).SetEase(Ease.InQuad));
        }

        seq.Play();
    }
}
