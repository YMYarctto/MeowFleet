using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Task_UI : UIView<Task_UI>
{
    public override int ID => id;
    static int nextID;
    int id = GetNextID();

    TMP_Text context;
    Transform button_accept;
    Transform button_refuse;
    CanvasGroup canvasGroup;
    EnemyGroup enemyGroup;
    RectTransform rectTransform;

    List<Vector3> positionList = new(){new(149,-58,0),new(266,80,0),new(84,109,0)};
    List<Vector3> rotationList = new(){Vector3.zero,new(0,0,-5),new(0,0,8)};
    int index;

    Sequence sequence;

    public static void ResetID()
    {
        nextID = 0;
    }

    static int GetNextID()
    {
        return nextID++;
    }

    public override void Init()
    {
        canvasGroup = transform.GetComponent<CanvasGroup>();
        context = transform.Find("context").GetComponent<TMP_Text>();
        button_accept = transform.Find("Button_TaskAccept");
        button_refuse = transform.Find("Button_TaskRefuse");
        rectTransform = transform.GetComponent<RectTransform>();

        EventTrigger eventTrigger_accept = InitButton(button_accept);
        EventTrigger.Entry entry_pointerClick_Accept = new EventTrigger.Entry();
        entry_pointerClick_Accept.eventID = EventTriggerType.PointerClick;
        entry_pointerClick_Accept.callback.AddListener((data) => { OnPointerClick_Accept((PointerEventData)data); });
        eventTrigger_accept.triggers.Add(entry_pointerClick_Accept);

        EventTrigger eventTrigger_refuse = InitButton(button_refuse);
        EventTrigger.Entry entry_pointerClick_Refuse = new EventTrigger.Entry();
        entry_pointerClick_Refuse.eventID = EventTriggerType.PointerClick;
        entry_pointerClick_Refuse.callback.AddListener((data) => { OnPointerClick_Refuse((PointerEventData)data); });
        eventTrigger_refuse.triggers.Add(entry_pointerClick_Refuse);
        canvasGroup.interactable = false;
    }

    void OnEnable()
    {
        canvasGroup.alpha = 1;
        index = 2;
        var rect = GetPositionAndRotation();
        rectTransform.anchoredPosition = rect.Item1;
        rectTransform.rotation = Quaternion.Euler(rect.Item2);
    }

    //Temp
    public void Init(EnemyGroup group)
    {
        enemyGroup = group;
        context.text = $"关卡：{group.uid}";
    }

    public EventTrigger InitButton(Transform button)
    {
        EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_pointerEnter = new EventTrigger.Entry();
        entry_pointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_pointerEnter.callback.AddListener((data) => { Button_OnPointerEnter((PointerEventData)data,button); });

        EventTrigger.Entry entry_pointerExit = new EventTrigger.Entry();
        entry_pointerExit.eventID = EventTriggerType.PointerExit;
        entry_pointerExit.callback.AddListener((data) => { Button_OnPointerExit((PointerEventData)data,button); });

        eventTrigger.triggers.Add(entry_pointerEnter);
        eventTrigger.triggers.Add(entry_pointerExit);

        return eventTrigger;
    }

    private void Button_OnPointerEnter(PointerEventData eventData,Transform trans)
    {
        trans.localScale = Vector3.one * 1.1f;
    }

    private void Button_OnPointerExit(PointerEventData eventData,Transform trans)
    {
        trans.localScale = Vector3.one;
    }

    private void OnPointerClick_Accept(PointerEventData eventData)
    {
        LoadDataManager.instance.PVELoadData.SetLoadData(enemyGroup);
        FormationController.instance.Show();
        TaskSelectController.instance.Hide();
    }

    private void OnPointerClick_Refuse(PointerEventData eventData)
    {
        TaskManager.instance.MoveUp();
    }

    public void MoveUp()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        sequence?.Kill();
        sequence = DOTween.Sequence();
        index--;
        if (index < 0)
        {
            sequence.Append(canvasGroup.DOFade(0,0.2f));
            sequence.AppendCallback(()=>{
                OnEnable();
                transform.SetSiblingIndex(0);
            });
            sequence.Play();
            return;
        }
        var rect = GetPositionAndRotation();
        sequence.Append(rectTransform.DOAnchorPos3D(rect.Item1,0.2f).SetEase(Ease.OutQuad));
        sequence.Join(rectTransform.DORotate(rect.Item2,0.2f).SetEase(Ease.OutQuad));
        if(index == 0)sequence.AppendCallback(()=>canvasGroup.interactable = canvasGroup.blocksRaycasts = true);
        sequence.Play();
    }

    private (Vector3,Vector3) GetPositionAndRotation()
    {
        if (index < 0 || index >= positionList.Count)
        {
            return (positionList[positionList.Count-1],rotationList[rotationList.Count-1]);
        }
        return (positionList[index],rotationList[index]);
    }
}
