using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PVE_Notice : UIView
{
    public override UIView currentView => this;
    public override int ID => _id;

    static int NoticeID=0;
    int _id = NoticeID;

    float MaxSize=2.5f;

    TMP_Text text;
    Transform bg;
    Transform bar_trans;
    CanvasGroup image;
    CanvasGroup bg_image;

    Sequence sequence;

    static Transform UI_Notice => PVEController.instance.UI_Notice;
    static Dictionary<int, PVE_Notice> notice_dict = new();
    static Tween tween_NoticeBar;

    public override void Init()
    {
        bg = transform.Find("bg");
        bar_trans = transform.Find("bar");
        text = bar_trans.GetComponentInChildren<TMP_Text>();
        image = bar_trans.GetComponent<CanvasGroup>();
        bg_image = GetComponent<CanvasGroup>();
        Disable();
    }

    public void ShowNotice_Hit(string ship_str, string locate)
    {
        text.text = $"你命中了【 {ship_str} 】的{locate}";
        Enable();
    }
    
    public void ShowNotice_Destroy(string ship_str,string action)
    {
        text.text = $"你{action}了【 {ship_str} 】";
        Enable();
    }

    public void ShowNotice_Check(string ship_str,string locate)
    {
        text.text = $"你侦查到了【 {ship_str} 】的{locate}";
        Enable();
    }

    public void ShowNotice_Victory()
    {
        text.text = $"你击溃了敌方舰队";
        Enable();
    }

    public void ShowNotice_Defeat()
    {
        text.text = $"你的舰队被击溃了";
        Enable();
    }

    public override void Enable()
    {
        Disable();
        sequence?.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, 0.2f).SetEase(Ease.InQuad));
        sequence.Join(bar_trans.DOScale(new Vector3(1f, 1f, 1f), 0.2f).SetEase(Ease.InQuad));
        sequence.Join(bg.DOScaleY(1f, 0.2f).SetEase(Ease.InQuad));
        sequence.AppendInterval(1.2f);
        sequence.Append(bg_image.DOFade(0, 0.3f).SetEase(Ease.OutQuad));
        sequence.OnComplete(() => Destroy(gameObject));
    }

    public override void Disable()
    {
        image.alpha = 0;
        bar_trans.localScale = MaxSize * new Vector3(1f, 1f, 1f);
        bg.localScale = new(1f, 0, 1f);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        notice_dict.Remove(_id);
    }

    public static PVE_Notice Create()
    {
        NoticeID++;
        var obj = Instantiate(ResourceManager.instance.GetPerfabByType<PVE_Notice>(), UI_Notice, false);
        obj.transform.localPosition = new Vector3(0,-130*NoticeID,0);
        var ui = obj.GetComponent<PVE_Notice>();

        if (tween_NoticeBar != null && tween_NoticeBar.IsActive())
        {
            tween_NoticeBar.Kill();
        }

        tween_NoticeBar = UI_Notice.DOLocalMoveY(130 * NoticeID, 0.5f).SetEase(Ease.InOutQuart);

        notice_dict.Add(NoticeID, ui);
        return ui;
    }
}
