using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BGAnimator_TitleScene : UIView
{
    public override UIView currentView => this;

    float bg_1_duration=1f;
    float bg_2_interval=0;
    float bg_2_duration=0.5f;
    float bg_3_interval=0.05f;
    float bg_3_duration=0.5f;
    float bg_4_interval=0.1f;
    float bg_4_duration=0.5f;
    Ease bg_ease=Ease.OutBack;
    Ease color_ease=Ease.OutQuart;

    Image bg_1_image;
    Color bg_1_color;
    RectTransform bg_2;
    RectTransform bg_3;
    RectTransform bg_4;

    Vector3 bg_2_targetPos;
    Vector3 bg_3_targetPos;
    Vector3 bg_4_targetPos;

    public override void Init()
    {
        bg_1_image = transform.Find("bg_1.5").GetComponent<Image>();
        bg_1_color = bg_1_image.color;
        bg_2 = transform.Find("bg_2").GetComponent<RectTransform>();
        bg_3 = transform.Find("bg_3").GetComponent<RectTransform>();
        bg_4 = transform.Find("bg_4").GetComponent<RectTransform>();

        bg_1_color.a = 1f;
        bg_1_image.color = bg_1_color;

        bg_2_targetPos = bg_2.localPosition;
        bg_3_targetPos = bg_3.localPosition;
        bg_4_targetPos = bg_4.localPosition;

        bg_2.localPosition = new Vector3(-2501, -442, 0);
        bg_3.localPosition = new Vector3(1772, -167, 0);
        bg_4.localPosition = new Vector3(1105, 876, 0);
    }

    public override void Enable()
    {
        DOTween.To(() => bg_1_color.a, x => bg_1_color.a=x, 0f, bg_1_duration).SetEase(color_ease).OnUpdate(()=>{
            bg_1_image.color = bg_1_color;
        });
        Sequence loopTween_Open = DOTween.Sequence();
        loopTween_Open.Append(bg_2.DOLocalMove(bg_2_targetPos, bg_2_duration).SetDelay(bg_2_interval).SetEase(bg_ease));
        loopTween_Open.Join(bg_3.DOLocalMove(bg_3_targetPos, bg_3_duration).SetDelay(bg_3_interval).SetEase(bg_ease));
        loopTween_Open.Join(bg_4.DOLocalMove(bg_4_targetPos, bg_4_duration).SetDelay(bg_4_interval).SetEase(bg_ease));
        loopTween_Open.Play();
    }

    public void ReEnable()
    {
        bg_1_color.a = 1f;
        bg_1_image.color = bg_1_color;

        bg_2.localPosition = new Vector3(-2501, -442, 0);
        bg_3.localPosition = new Vector3(1772, -167, 0);
        bg_4.localPosition = new Vector3(1105, 876, 0);

        Enable();
    }
}
