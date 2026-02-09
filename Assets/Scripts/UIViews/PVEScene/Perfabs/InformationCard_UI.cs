using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InformationCard_UI : UIView,IPointerMoveHandler
{
    public override UIView currentView => this;
    public override int ID => _id;

    int _id = InformationBoard.CardID;
    static readonly Vector3 window_delta = new Vector3(240,-120,0);

    int lastLinkIndex = -1;
    string info_id;
    List<Vector3> hole_v3;
    static string currentHoverId = null;
    Camera uiCamera;
    InformationWindow_UI bind_window;

    TMP_Text title;
    TMP_Text content;

    EventTrigger eventTrigger;

    public override void Init()
    {
        Transform inner = transform.Find("inner");
        title = inner.Find("title").GetComponent<TMP_Text>();
        content = inner.Find("content").GetComponent<TMP_Text>();
        title.text = UIManager.instance.GetUIView<InformationBoard>().GetTitle();
        var canvas = content.canvas;
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_beginDrag = new EventTrigger.Entry();
        entry_beginDrag.eventID = EventTriggerType.BeginDrag;
        entry_beginDrag.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });

        EventTrigger.Entry entry_endDrag = new EventTrigger.Entry();
        entry_endDrag.eventID = EventTriggerType.EndDrag;
        entry_endDrag.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });

        EventTrigger.Entry entry_onDrag = new EventTrigger.Entry();
        entry_onDrag.eventID = EventTriggerType.Drag;
        entry_onDrag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });

        EventTrigger.Entry entry_onPointerExit = new EventTrigger.Entry();
        entry_onPointerExit.eventID = EventTriggerType.PointerExit;
        entry_onPointerExit.callback.AddListener((data)=>OnPointerExit((PointerEventData)data));

        EventTrigger.Entry entry_onPointerEnter = new EventTrigger.Entry();
        entry_onPointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_onPointerEnter.callback.AddListener((data)=>OnPointerEnter((PointerEventData)data));

        eventTrigger.triggers.Add(entry_beginDrag);
        eventTrigger.triggers.Add(entry_endDrag);
        eventTrigger.triggers.Add(entry_onDrag);
        eventTrigger.triggers.Add(entry_onPointerExit);
    }

    public override void Enable()
    {
        transform.DOLocalMoveX(-295,0.3f).SetEase(Ease.OutQuad);
    }

    public InformationCard_UI Hit(string ship_str, string locate)
    {
        content.text = $"你命中了<link=\"{info_id}><color=#b51d04>【 {ship_str} 】</color></link>的{locate}";
        return this;
    }
    
    public InformationCard_UI Destroy(string ship_str,string action)
    {
        content.text = $"你{action}了<link=\"{info_id}\"><color=#b51d04>【 {ship_str} 】</color></link>";
        return this;
    }

    public InformationCard_UI Check(string ship_str, string locate)
    {
        content.text = $"你侦查到了<link=\"{info_id}><color=#b51d04>【 {ship_str} 】</color></link>的{locate}";
        return this;
    }

    public InformationCard_UI SetInfoID(int id,List<Vector3> hole_positions)
    {
        info_id = id.ToString();
        hole_v3 = hole_positions;
        bind_window = UIManager.instance.GetUIView<InformationWindow_UI>(id);
        return this;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<InformationBoard>().OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<InformationBoard>().OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UIManager.instance.GetUIView<InformationBoard>().OnDrag(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        CheckHover(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentHoverId = info_id;
        CheckHover(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetColor();
    }

    void CheckHover(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
            content,
            eventData.position,
            uiCamera
        );
        // Debug.Log($"Hover linkIndex = {linkIndex}, last = {lastLinkIndex}");

        if (linkIndex == lastLinkIndex)
        return;

        if (linkIndex == -1)
        {
            ResetColor();
            return;
        }

        ApplyHoverColor(linkIndex);
        lastLinkIndex = linkIndex;
    }

    void ApplyHoverColor(int linkIndex)
    {
        TMP_LinkInfo linkInfo = content.textInfo.linkInfo[linkIndex];
        string id = linkInfo.GetLinkID();
        if (id != info_id && currentHoverId != info_id) return;
        UIManager.instance.GetUIView<HoleOverlay>().ShowOverlay(hole_v3.ToArray());
        Debug.Log($"Hover id = {id}, info_id = {info_id}, {hole_v3.Count}");

        for (int i = 0; i < linkInfo.linkTextLength; i++)
        {
            int charIndex = linkInfo.linkTextfirstCharacterIndex + i;
            var charInfo = content.textInfo.characterInfo[charIndex];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Color32[] colors = content.textInfo.meshInfo[meshIndex].colors32;
            colors[vertexIndex + 0] = PresetColor.Text_Hower;
            colors[vertexIndex + 1] = PresetColor.Text_Hower;
            colors[vertexIndex + 2] = PresetColor.Text_Hower;
            colors[vertexIndex + 3] = PresetColor.Text_Hower;
        }

        content.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        bind_window.Enable(PVEController.instance.UICamera.WorldToScreenPoint(transform.position)+window_delta);
        UIManager.instance.GetUIView<BG_PVE>().EnemyPage();
    }

    void ResetColor()
    {
        if (lastLinkIndex == -1) return;
        UIManager.instance.GetUIView<HoleOverlay>().ClearOverlay();
        content.ForceMeshUpdate();
        lastLinkIndex = -1;
        bind_window.Disable();
    }
}
