using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StageNotice_Animator : UIView
{
    public override UIView currentView => this;

    Transform left;
    Transform right;
    Transform player_skill;
    Transform player_attack;
    Image skill_image;
    Image attack_image;
    Color color;

    Transform target_trans;
    Image target_img;

    float animation_time=0.6f;
    Ease ease=Ease.InOutQuad;

    Sequence sequence;

    public override void Init()
    {
        left = transform.Find("decorate_left");
        right = transform.Find("decorate_right");
        player_skill = transform.Find("PlayerSkill");
        player_attack = transform.Find("PlayerAttack");
        skill_image = player_skill.GetComponent<Image>();
        attack_image = player_attack.GetComponent<Image>();
        color = skill_image.color;

        left.transform.localPosition = new Vector3(-100,0,0);
        right.transform.localPosition = new Vector3(100,0,0);
        player_skill.transform.localScale = Vector3.zero;
        player_attack.transform.localScale = Vector3.zero;
        color.a = 0;
        skill_image.color = attack_image.color = color;
    }

    public void Close()
    {
        if(!target_trans)
        {
            return;
        }

        color.a =1f;
        sequence?.Kill();
        sequence=DOTween.Sequence();
        sequence.Append(left.DOLocalMoveX(-100,animation_time).SetEase(ease));
        sequence.Join(right.DOLocalMoveX(100,animation_time).SetEase(ease));
        sequence.Join(target_trans.DOScale(Vector3.zero,animation_time).SetEase(ease));
        sequence.Join(DOTween.To(() => color.a, x => color.a = x, 0f, animation_time).SetEase(ease).OnUpdate(() =>
        {
            target_img.color = color;
        }));
        sequence.Play();
    }

    Sequence CloseSeq()
    {
        color.a =1f;
        Sequence seq=DOTween.Sequence();
        seq.Append(left.DOLocalMoveX(-100,animation_time).SetEase(ease));
        seq.Join(right.DOLocalMoveX(100,animation_time).SetEase(ease));
        seq.Join(target_trans.DOScale(Vector3.zero,animation_time).SetEase(ease));
        seq.Join(DOTween.To(() => color.a, x => color.a = x, 0f, animation_time).SetEase(ease).OnUpdate(() =>
        {
            target_img.color = color;
        }));
        return seq;
    }

    void OpenSeq()
    {
        sequence.Append(left.DOLocalMoveX(-300,animation_time).SetEase(ease));
        sequence.Join(right.DOLocalMoveX(300,animation_time).SetEase(ease));
        sequence.Join(target_trans.DOScale(Vector3.one,animation_time).SetEase(ease));
        sequence.Join(DOTween.To(() => color.a, x => color.a = x, 1f, animation_time).SetEase(ease).OnUpdate(() =>
        {
            target_img.color = color;
        }));
    }

    public void Open()
    {
        if(!target_trans)
        {
            return;
        }

        color.a =0f;
        sequence?.Kill();
        sequence=DOTween.Sequence();
        OpenSeq();
        sequence.Play();
    }

    public void Open_PlayerAttack()
    {
        target_trans = player_attack;
        target_img = attack_image;
        Open();
    }

    public void Open_PlayerSkill()
    {
        target_trans = player_skill;
        target_img = skill_image;
        Open();
    }

    public void Close_Open_PlayerAttack(float delay)
    {
        sequence?.Kill();
        sequence = CloseSeq();
        sequence.AppendInterval(delay);
        target_trans = player_attack;
        target_img = attack_image;
        OpenSeq();
        sequence.Play();
    }

    public void Close_Open_PlayerSkill(float delay)
    {
        sequence?.Kill();
        sequence = CloseSeq();
        sequence.AppendInterval(delay);
        target_trans = player_skill;
        target_img = skill_image;
        OpenSeq();
        sequence.Play();
    }
}
