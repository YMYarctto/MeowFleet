using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoCounter : UIView
{
    TMP_Text ship_destroy;
    TMP_Text ship_capture;
    TMP_Text hitRate;
    TMP_Text defenseRate;
    TMP_Text effectiveSkill;

    int destroy_count=0;
    int capture_count=0;
    int effectiveSkill_count=0;

    int hit_on=0;
    int hit_total=0;

    int defense_on=0;
    int defense_total=0;

    public override UIView currentView => this;

    public override void Init()
    {
        ship_destroy = transform.Find("destroy").GetComponent<TMP_Text>();
        ship_capture = transform.Find("capture").GetComponent<TMP_Text>();
        hitRate = transform.Find("hitRate").GetComponent<TMP_Text>();
        defenseRate = transform.Find("defenseRate").GetComponent<TMP_Text>();
        effectiveSkill = transform.Find("effectiveSkill").GetComponent<TMP_Text>();
    }

    void Start()
    {
        PVEController.instance.OnEnemyShipCaptured+=EnemyShipCapturedCount;
        PVEController.instance.OnEnemyShipDestroyed+=EnemyShipDestoryCount;
        PVEController.instance.OnPlayerHit+=PlayerHit;
        PVEController.instance.OnEnemyHit+=EnemyHit;
        PVEController.instance.OnSkillEffective+=EffectiveSkillCount;
    }

    // void OnDisable()
    // {
    //     PVEController.instance.OnEnemyShipCaptured-=EnemyShipCapturedCount;
    //     PVEController.instance.OnEnemyShipDestroyed-=EnemyShipDestoryCount;
    //     PVEController.instance.OnPlayerHit-=PlayerHit;
    //     PVEController.instance.OnEnemyHit-=EnemyHit;
    //     PVEController.instance.OnSkillEffective-=EffectiveSkillCount;
    // }

    public void EnemyShipCapturedCount()
    {
        capture_count+=1;
        ship_capture.text = $"{capture_count}";
    }

    public void EnemyShipDestoryCount()
    {
        destroy_count+=1;
        ship_destroy.text = $"{destroy_count}";
    }

    public void PlayerHit(bool isHit)
    {
        hit_on+=isHit?1:0;
        hit_total++;
        hitRate.text = $"{(int)((float)hit_on/hit_total*100)}%";
    }

    public void EnemyHit(bool isHit)
    {
        defense_on+=isHit?0:1;
        defense_total++;
        defenseRate.text = $"{(int)((float)defense_on/defense_total*100)}%";
    }

    public void EffectiveSkillCount()
    {
        effectiveSkill_count++;
        effectiveSkill.text = $"{effectiveSkill_count}";
    }
}
