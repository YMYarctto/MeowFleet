using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TaskPageButtonGroup : UIView<TaskPageButtonGroup>
{
    CanvasGroup canvasGroup;

    public override void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = canvasGroup.interactable = false;
        Disable();
    }

    public void ShowButton()
    {
        Enable();
        canvasGroup.DOFade(1,0.2f).SetEase(Ease.OutQuad).OnComplete(()=>canvasGroup.blocksRaycasts = canvasGroup.interactable = true);
    }
}
