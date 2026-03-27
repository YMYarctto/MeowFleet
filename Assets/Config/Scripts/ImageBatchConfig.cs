using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Tools/ImageBatchConfig")]
public class ImageBatchConfig : ScriptableObject
{
    [Header("筛选规则")]
    public string nameRegex; // 正则表达式

    [Header("Image目标参数")]
    public Color color;
    public Sprite sprite;
    public bool overrideSprite = false;

    [ContextMenu("Apply")]
    public void Apply()
    {
        if(name == "")
        {
            return;
        }
        var images = FindObjectsOfType<Image>(true);

        Regex regex = new Regex(nameRegex);

        int count = 0;

        foreach (var img in images)
        {
            if (regex.IsMatch(img.gameObject.name))
            {
                ApplyToImage(img);
                count++;
            }
        }

        Debug.Log($"应用完成，共修改 {count} 个对象");
    }

    void ApplyToImage(Image img)
    {
        img.color = color;

        if (overrideSprite && sprite != null)
        {
            img.sprite = sprite;
        }
    }
}
