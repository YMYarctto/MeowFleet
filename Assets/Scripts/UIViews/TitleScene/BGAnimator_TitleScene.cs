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
    Ease color_ease = Ease.OutQuart;

    public float inner_duration;
    public float bg_1out_duration;
    public float bg_1out_interval;
    public float bg_1out_interval_pos;
    public float bg_setting_duration;
    public float bg_setting_interval;
    public float button_duration;
    public float button_interval;
    public Ease out_ease;
    public Ease in_ease;
    public Ease in_out_ease;

    Image bg_1_image;
    Color bg_1_color;
    RectTransform bg_2;
    RectTransform bg_3;
    RectTransform bg_4;

    Transform inner;
    RectTransform bg_1;
    RectTransform bg_setting;
    Image bg_shadow_image;
    Color bg_shadow_color;
    Image bg_shadow2_image;

    Transform button_return;

    Vector3 bg_2_targetPos;
    Vector3 bg_3_targetPos;
    Vector3 bg_4_targetPos;

    Vector3 bg_1_targetPos;
    Vector3 bg_1_targetRot;
    Vector3 bg_1_targetScale;
    Vector3 inner_targetRot;

    float button_targetPosX;
    float button_initPosX;

    Sequence loopTween_Open;
    Sequence setting_seq;

    public override void Init()
    {
        bg_1_image = transform.Find("bg_1.5").GetComponent<Image>();
        bg_1_color = bg_1_image.color;
        inner = transform.Find("inner");
        bg_1 = transform.Find("bg_1").GetComponent<RectTransform>();
        bg_2 = inner.Find("bg_2").GetComponent<RectTransform>();
        bg_3 = inner.Find("bg_3").GetComponent<RectTransform>();
        bg_4 = inner.Find("bg_4").GetComponent<RectTransform>();
        bg_setting = transform.Find("Setting").GetComponent<RectTransform>();
        bg_shadow_image = transform.Find("bg_shadow").GetComponent<Image>();
        bg_shadow_color = bg_shadow_image.color;
        bg_shadow2_image = transform.Find("bg_shadow_2").GetComponent<Image>();

        button_return = transform.Find("button_setting_return");

        bg_1_color.a = 1f;
        bg_1_image.color = bg_1_color;

        bg_shadow_color.a = 0f;
        bg_shadow_image.color = bg_shadow2_image.color = bg_shadow_color;

        bg_2_targetPos = bg_2.localPosition;
        bg_3_targetPos = bg_3.localPosition;
        bg_4_targetPos = bg_4.localPosition;

        bg_2.localPosition = new Vector3(60, 278, 0);
        bg_3.localPosition = new Vector3(4332, 553, 0);
        bg_4.localPosition = new Vector3(3665, 1596, 0);

        bg_1_targetPos = new Vector3(-342, 334, 0);
        bg_1_targetRot = new Vector3(0, 0, 5);
        bg_1_targetScale = new Vector3(1.54f, 1.54f, 1.54f);
        inner_targetRot = new Vector3(0, 0, 60);

        bg_setting.localEulerAngles = new Vector3(0, 0, -60);
        button_targetPosX = -1280;
        button_initPosX = -1680;
        button_return.localPosition = new Vector3(button_initPosX, button_return.localPosition.y, 0);

        bg_1.localPosition = Vector3.zero;
        bg_1.localEulerAngles = Vector3.zero;
        bg_1.localScale = Vector3.one;

        InitSettingAnimation();
    }

    public override void Enable()
    {
        DOTween.To(() => bg_1_color.a, x => bg_1_color.a = x, 0f, bg_1_duration).SetEase(color_ease).OnUpdate(() =>
        {
            bg_1_image.color = bg_1_color;
        });
        loopTween_Open?.Kill();
        loopTween_Open = DOTween.Sequence();
        loopTween_Open.Append(bg_2.DOLocalMove(bg_2_targetPos, bg_2_duration).SetDelay(bg_2_interval).SetEase(bg_ease));
        loopTween_Open.Join(bg_3.DOLocalMove(bg_3_targetPos, bg_3_duration).SetDelay(bg_3_interval).SetEase(bg_ease));
        loopTween_Open.Join(bg_4.DOLocalMove(bg_4_targetPos, bg_4_duration).SetDelay(bg_4_interval).SetEase(bg_ease));
        loopTween_Open.Play();
    }

    public void ReEnable()
    {
        bg_1_color.a = 1f;
        bg_1_image.color = bg_1_color;

        bg_2.localPosition = new Vector3(60, 278, 0);
        bg_3.localPosition = new Vector3(4332, 553, 0);
        bg_4.localPosition = new Vector3(3665, 1596, 0);

        Enable();
    }

    public void SettingEnable()
    {
        setting_seq.PlayForward();
    }

    public void SettingDisable()
    {
        setting_seq.PlayBackwards();
    }

    private void InitSettingAnimation()
    {
        setting_seq = DOTween.Sequence();
        setting_seq.SetAutoKill(false);
        setting_seq.Pause();    
        setting_seq.Append(inner.DOLocalRotate(inner_targetRot, inner_duration).SetEase(in_ease));
        setting_seq.Insert(bg_1out_interval+bg_1out_interval_pos,bg_1.DOLocalMove(bg_1_targetPos, bg_1out_duration).SetEase(in_out_ease));
        setting_seq.Insert(bg_1out_interval,bg_1.DOLocalRotate(bg_1_targetRot, bg_1out_duration).SetEase(in_out_ease));
        setting_seq.Insert(bg_1out_interval,bg_1.DOScale(bg_1_targetScale, bg_1out_duration).SetEase(in_out_ease));
        setting_seq.Join(DOTween.To(() => bg_shadow_color.a, x => bg_shadow_color.a = x, 1f, bg_1out_duration).SetDelay(bg_1out_interval).SetEase(in_ease).OnUpdate(() =>
        {
            bg_shadow_image.color = bg_shadow2_image.color = bg_shadow_color;
        }));
        setting_seq.Join(bg_setting.DOLocalRotate(Vector3.zero, bg_setting_duration).SetDelay(bg_setting_interval).SetEase(out_ease));
        setting_seq.Join(button_return.DOLocalMoveX(button_targetPosX, button_duration).SetDelay(bg_setting_interval+button_interval).SetEase(out_ease));
    }
}
