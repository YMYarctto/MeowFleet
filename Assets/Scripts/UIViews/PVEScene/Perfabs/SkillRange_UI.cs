using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRange_UI : UIView
{
    public override UIView currentView => this;

    int _id = SkillCard_UI.CardID;
    public override int ID => _id;

    GameObject perfab;
    RectTransform viewport;
    RectTransform content;

    public override void Init()
    {
        perfab = ResourceManager.instance.GetPerfabByType<SkillRange_UI>();
        viewport = GetComponent<RectTransform>();
        content = transform.Find("content").GetComponent<RectTransform>();
    }

    public void Init(List<Vector2Int> coords)
    {
        Vector2Int min = coords[0];
        Vector2Int max = coords[0];
        foreach(Vector2Int c in coords)
        {
            min = Vector2Int.Min(min, c);
            max = Vector2Int.Max(max,c);
        }
        int gridWidth  = max.x - min.x + 1;
        int gridHeight = max.y - min.y + 1;
        Vector2 init_size = content.sizeDelta;
        foreach(Vector2Int c in coords)
        {
            GameObject obj = Instantiate(perfab,content);
            obj.GetComponent<RectTransform>().anchoredPosition = new((c-min).x*init_size.x,(c-min).y*init_size.y);
        }
        content.sizeDelta = new Vector2(init_size.x*gridWidth,init_size.y*gridHeight);
        float scaleX = Mathf.Clamp(viewport.sizeDelta.x/content.sizeDelta.x,0,1);
        float scaleY = Mathf.Clamp(viewport.sizeDelta.y/content.sizeDelta.y,0,1);
        float scale = scaleX<scaleY?scaleX:scaleY;
        content.localScale = content.localScale*scale;
        content.pivot = new Vector2(0.5f,0.5f);
        content.localPosition = Vector3.zero;
    }
}
