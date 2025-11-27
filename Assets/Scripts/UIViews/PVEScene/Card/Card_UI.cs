using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Card_UI : MonoBehaviour
{
    private float intensity = 6f;
    private Vector2 duration_range = new(0.1f, 0.25f);

    bool waveLock=false;

    Transform card_trans;
    Transform shadow;

    Tween waveTween;

    Vector3 startPos;

    void Awake()
    {
        card_trans = transform.Find("bg");
        shadow = card_trans.Find("mask").Find("shadow_2");
        startPos = shadow.position;

        Wave();
    }

    void FixedUpdate()
    {
        shadow.rotation = Quaternion.Euler(Vector3.zero);
        shadow.position = startPos;
        if(MouseListener.MouseMove&&MouseListener.Distance(transform.position+new Vector3(200,0,0))<400)
        {
            if(!waveLock)
            {
                waveLock = true;
                float value = Mathf.Clamp(MouseListener.DistanceY(transform.position), -1, 1);
                Wave(value*MouseListener.MouseSpeed);
            }
        }
        else
        {
            waveLock = false;
        }
    }

    void Wave()
    {
        float targetAngle = Random.Range(-intensity, intensity);
        float duration = Random.Range(duration_range.x, duration_range.y);

        waveTween?.Kill();

        waveTween = card_trans.DOLocalRotate(new Vector3(0, 0, targetAngle), duration).SetEase(Ease.OutQuad);
    }
    
    void Wave(float range)
    {
        float new_internsity = Mathf.Clamp(range,-intensity,intensity);
        Vector2 v2 = new_internsity > 0 ? new Vector2(0, new_internsity) : new Vector2(new_internsity, 0);
        float targetAngle = Random.Range(v2.x, v2.y);
        float duration = Random.Range(duration_range.x, duration_range.y);

        waveTween?.Kill();

        waveTween = card_trans.DOLocalRotate(new Vector3(0, 0, targetAngle), duration).SetEase(Ease.OutQuad);
    }
}
