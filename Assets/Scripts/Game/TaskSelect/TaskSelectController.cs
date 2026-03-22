using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TaskSelectController : Manager<TaskSelectController>,IScenePage
{
    CanvasGroup canvasGroup;
    Tween tween;

    void Awake()
    {
        canvasGroup = GameObject.Find("TaskSelectPage").GetComponent<CanvasGroup>();
        UIManager.instance.GetUIView<Button_FormationToLastPage>().SetLastPage(FrontPage.TaskSelectPage);
    }

    public void Show()
    {
        tween?.Kill();
        tween = canvasGroup.DOFade(1,0.2f).SetEase(Ease.OutQuad).OnComplete(()=>canvasGroup.interactable =canvasGroup.blocksRaycasts= true);
    }

    public void Hide()
    {
        canvasGroup.interactable =canvasGroup.blocksRaycasts= false;
        tween?.Kill();
        tween = canvasGroup.DOFade(0,0.2f).SetEase(Ease.OutQuad);
    }
}
