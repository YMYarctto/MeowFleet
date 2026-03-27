using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InformationCard_UI : UIView<InformationCard_UI>,IPointerMoveHandler
{
    public override int ID => _id;
    static int nextID;
    static int pendingID;
    int _id = pendingID;
    static readonly Vector3 window_delta = new Vector3(240,-120,0);

    int lastLinkIndex = -1;
    string info_id;
    List<Vector3> hole_v3;
    static string currentHoverId = null;
    Camera uiCamera;
    InformationWindow_UI bind_window;

    public int InfoID {get;private set;}
    int weight = 0;
    bool newCard = true;

    TMP_Text title;
    TMP_Text content;
    RectTransform rectTransform;
    CanvasGroup content_img;

    EventTrigger eventTrigger;
    Tween enterTween;
    Sequence textSequence;
    Vector2 targetAnchoredPos;

    public static void ResetID()
    {
        nextID = 0;
        pendingID = 0;
    }

    public static int PrepareNextID()
    {
        pendingID = nextID;
        return nextID++;
    }

    public override void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        Transform inner = transform.Find("inner");
        title = inner.Find("title").GetComponent<TMP_Text>();
        content = inner.Find("content").GetComponent<TMP_Text>();
        content_img = content.GetComponent<CanvasGroup>();
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
        eventTrigger.triggers.Add(entry_onPointerEnter);
        eventTrigger.triggers.Add(entry_onPointerExit);
    }

    public override void Enable()
    {
        newCard = false;
        enterTween?.Kill();
        enterTween = rectTransform.DOAnchorPos(targetAnchoredPos,0.3f).SetEase(Ease.OutQuad);
    }

    void PlayTextAnimation(string text)
    {
        float localX = content.transform.localPosition.x;
        textSequence?.Kill();
        textSequence = DOTween.Sequence();
        textSequence.Append(content.transform.DOLocalMoveX(localX+200f,0.2f).SetEase(Ease.OutQuad));
        textSequence.Join(content_img.DOFade(0,0.2f).SetEase(Ease.OutQuad));
        textSequence.AppendCallback(()=>content.text = text);
        textSequence.Insert(0.3f,content_img.DOFade(1,0.2f).SetEase(Ease.InQuad));
        textSequence.Insert(0.3f,content.transform.DOLocalMoveX(localX,0.2f).SetEase(Ease.InQuad));
    }

    bool Weight(int w)
    {
        if (w > weight)
        {
            weight = w;
            return true;
        }
        return false;
    }

    public void Show(Status status)
    {
        switch(status)
        {
            case Status.Check:
                if (Weight(1))
                {
                    PlayTextAnimation("被侦查");
                }
                break;
            case Status.Hit_Body:
                if (Weight(2))
                {
                    PlayTextAnimation("船体受损");
                }
                break;
            case Status.Hit_Core:
                if(Weight(3))
                {
                    PlayTextAnimation("核心受损");
                }
                break;
            case Status.Capture:
                if (Weight(4))
                {
                    PlayTextAnimation("已俘获");
                }
                break;
            case Status.Destroy:
                if(Weight(5))
                {
                    PlayTextAnimation("已击沉");
                }
                break;
        }
        if (!newCard)
        {
            return;
        }
        Enable();
    }

    public InformationCard_UI SetInfo(List<Vector3> hole_positions,string ship_name)
    {
        if (hole_v3==null)
        {
            hole_v3 = hole_positions;
            title.text = $"<link={info_id}><color=#b51d04>【 {ship_name} 】</color></link>";
            UIManager.instance.TryGetUIView(InfoID,out bind_window);
        }
        return this;
    }

    public void SetID(int id)
    {
        info_id = id.ToString();
        InfoID = id;
    }

    public void SetAnimationPosition(Vector2 targetPosition,float startYOffset)
    {
        targetAnchoredPos = targetPosition;
        rectTransform.anchoredPosition = new Vector2(targetPosition.x, targetPosition.y + startYOffset);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        InformationBoard.GetUIView().OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InformationBoard.GetUIView().OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        InformationBoard.GetUIView().OnDrag(eventData);
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
            title,
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
        if (!PVEController.instance.PlayerAction)return;
        TMP_LinkInfo linkInfo = title.textInfo.linkInfo[linkIndex];
        string id = linkInfo.GetLinkID();
        if (id != info_id && currentHoverId != info_id) return;
        HoleOverlay.GetUIView().ShowOverlay(hole_v3.ToArray());
        Debug.Log($"Hover id = {id}, info_id = {info_id}, {hole_v3.Count}");

        for (int i = 0; i < linkInfo.linkTextLength; i++)
        {
            int charIndex = linkInfo.linkTextfirstCharacterIndex + i;
            var charInfo = title.textInfo.characterInfo[charIndex];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Color32[] colors = title.textInfo.meshInfo[meshIndex].colors32;
            colors[vertexIndex + 0] = PresetColor.Text_Hower;
            colors[vertexIndex + 1] = PresetColor.Text_Hower;
            colors[vertexIndex + 2] = PresetColor.Text_Hower;
            colors[vertexIndex + 3] = PresetColor.Text_Hower;
        }

        title.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        bind_window?.Enable(PVEController.instance.UICamera.WorldToScreenPoint(transform.position)+window_delta);
        if(bind_window!=null)
        {
            BG_PVE.GetUIView().EnemyPage();
        }
        else
        {
            BG_PVE.GetUIView().PlayerPage();
        }
    }

    void ResetColor()
    {
        if (currentHoverId == info_id)
        {
            currentHoverId = null;
        }
        if (lastLinkIndex == -1) return;
        HoleOverlay.GetUIView().ClearOverlay();
        title.ForceMeshUpdate();
        lastLinkIndex = -1;
        bind_window?.Disable();
    }

    void OnDisable()
    {
        enterTween?.Kill();
    }

    public enum Status
    {
        Check,
        Hit_Body,
        Hit_Core,
        Capture,
        Destroy,
    }
}
