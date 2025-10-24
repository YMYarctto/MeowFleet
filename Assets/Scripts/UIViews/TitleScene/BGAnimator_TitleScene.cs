using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BGAnimator_TitleScene : UIView
{
    public override UIView currentView => this;

    public float bg_1_duration;
    public float bg_2_interval;
    public float bg_2_duration;
    public float bg_3_interval;
    public float bg_3_duration;
    public float bg_4_interval;
    public float bg_4_duration;
    public Ease bg_ease;
    public Ease color_ease;

    Image bg_1_image;
    Color bg_1_color;
    RectTransform bg_2;
    RectTransform bg_3;
    RectTransform bg_4;

    public override void Init()
    {
        bg_1_image = transform.Find("bg_1.5").GetComponent<Image>();
        bg_1_color = bg_1_image.color;
        bg_2 = transform.Find("bg_2").GetComponent<RectTransform>();
        bg_3 = transform.Find("bg_3").GetComponent<RectTransform>();
        bg_4 = transform.Find("bg_4").GetComponent<RectTransform>();

        bg_1_color.a = 1f;
        bg_1_image.color = bg_1_color;

        bg_2.localPosition = new Vector3(-1868, -333, 0);
        bg_3.localPosition = new Vector3(1324, -122, 0);
        bg_4.localPosition = new Vector3(1077, 665, 0);
    }

    public override void Enable()
    {
        DOTween.To(() => bg_1_color.a, x => bg_1_color.a=x, 0f, bg_1_duration).SetEase(color_ease).OnUpdate(()=>{
            bg_1_image.color = bg_1_color;
        });
        Sequence loopTween_Open = DOTween.Sequence();
        loopTween_Open.Append(bg_2.DOLocalMove(new Vector3(-189, 12, 0), bg_2_duration).SetDelay(bg_2_interval).SetEase(bg_ease));
        loopTween_Open.Join(bg_3.DOLocalMove(new Vector3(650.5f, -86.4f, 0), bg_3_duration).SetDelay(bg_3_interval).SetEase(bg_ease));
        loopTween_Open.Join(bg_4.DOLocalMove(new Vector3(571, 401, 0), bg_4_duration).SetDelay(bg_4_interval).SetEase(bg_ease));
        loopTween_Open.Play();
    }

    public void ReEnable()
    {
        bg_1_color.a = 1f;
        bg_1_image.color = bg_1_color;

        bg_2.localPosition = new Vector3(-1868, -333, 0);
        bg_3.localPosition = new Vector3(1324, -122, 0);
        bg_4.localPosition = new Vector3(1077, 665, 0);

        Enable();
    }
}
