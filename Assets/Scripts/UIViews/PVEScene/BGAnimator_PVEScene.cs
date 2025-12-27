using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BGAnimator_PVEScene : UIView
{
    public override UIView currentView => this;

    public float bg_setting_duration;
    public float button_duration;
    public float button_interval;
    public Ease out_ease;
    public Ease in_ease;
    public Ease in_out_ease;

    Transform button_return;
    Transform button_mainMenu;
    GameObject interaction;

    RectTransform bg_setting;
    Image bg_shadow_image;
    Color bg_shadow_color;
    Image bg_shadow2_image;

    Sequence setting_seq;

    float button_targetPosX;
    float button_initPosX;

    public override void Init()
    {
        bg_setting = transform.Find("Setting").GetComponent<RectTransform>();
        bg_shadow_image = transform.Find("bg_shadow").GetComponent<Image>();
        bg_shadow_color = bg_shadow_image.color;
        bg_shadow2_image = transform.Find("bg_shadow_2").GetComponent<Image>();
        interaction = transform.Find("interaction").gameObject;

        button_return = transform.Find("button_setting_returnGame");
        button_mainMenu = transform.Find("button_setting_main_menu");

        bg_shadow_color.a = 0f;
        bg_shadow_image.color = bg_shadow2_image.color = bg_shadow_color;

        bg_setting.localEulerAngles = new Vector3(0, 0, -60);
        button_targetPosX = -1280;
        button_initPosX = -1680;
        button_return.localPosition = new Vector3(button_initPosX, button_return.localPosition.y, 0);
        button_mainMenu.localPosition = new Vector3(button_initPosX, button_mainMenu.localPosition.y, 0);
        interaction.SetActive(false);

        InitSettingAnimation();
    }

    public void SettingEnable()
    {
        Time.timeScale = 0f;
        setting_seq.PlayForward();
    }

    public void SettingDisable()
    {
        Time.timeScale = 1f;
        setting_seq.PlayBackwards();
    }

    private void InitSettingAnimation()
    {
        setting_seq = DOTween.Sequence().SetUpdate(true);;
        setting_seq.SetAutoKill(false);
        setting_seq.Pause();
        setting_seq.Join(DOTween.To(() => bg_shadow_color.a, x => bg_shadow_color.a = x, 1f,bg_setting_duration).SetEase(in_ease).OnUpdate(() =>
        {
            bg_shadow_image.color = bg_shadow2_image.color = bg_shadow_color;
        }));
        setting_seq.Join(bg_setting.DOLocalRotate(Vector3.zero, bg_setting_duration).SetEase(out_ease));
        setting_seq.Insert(button_interval,button_return.DOLocalMoveX(button_targetPosX, button_duration).SetEase(out_ease));
        setting_seq.Insert(button_interval*2,button_mainMenu.DOLocalMoveX(button_targetPosX, button_duration).SetEase(out_ease));
    }
}
