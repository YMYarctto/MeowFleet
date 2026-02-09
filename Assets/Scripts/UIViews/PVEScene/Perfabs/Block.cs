using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    GameObject range;
    GameObject core;
    GameObject body;

    void Awake()
    {
        range = transform.Find("range").gameObject;
        core = transform.Find("core").gameObject;
        body = transform.Find("body").gameObject;
    }

    public void SetState(State state)
    {
        range.SetActive(state==State.range);
        core.SetActive(state==State.core);
        body.SetActive(state==State.body);
        GetComponent<RectTransform>().sizeDelta = Vector2.one*(state==State.range?20:44);
    }

    public static void Group(State state,List<Vector2Int> coords,RectTransform content,RectTransform viewport,int coreNum=0)
    {
        GameObject perfab = ResourceManager.instance.GetPerfabByType<Block>();
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
            if (coreNum > 0)
            {
                obj.GetComponent<Block>().SetState(State.core);
                coreNum--;
            }
            else
            {
                obj.GetComponent<Block>().SetState(state);
            }
            obj.GetComponent<RectTransform>().anchoredPosition = new((c-min).x*init_size.x,(c-min).y*init_size.y);
        }
        content.sizeDelta = new Vector2(init_size.x*gridWidth,init_size.y*gridHeight);
        float scaleX = Mathf.Clamp(viewport.sizeDelta.x/content.sizeDelta.x,0,1);
        float scaleY = Mathf.Clamp(viewport.sizeDelta.y/content.sizeDelta.y,0,1);
        float scale = scaleX<scaleY?scaleX:scaleY;
        content.localScale *= scale;
        content.pivot = new Vector2(0.5f,0.5f);
        content.anchoredPosition = Vector3.zero;
    }

    public enum State
    {
        range,
        core,
        body
    }
}
