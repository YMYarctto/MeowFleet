using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_FormationToLastPage : BaseButton_Setting
{
    FrontPage last_page;

    public override void OnPointerClick(PointerEventData eventData)
    {
        FormationController.instance.Hide();
        if (last_page == FrontPage.TaskSelectPage)
        {
            TaskSelectController.instance.Show();
        }
    }

    public void SetLastPage(FrontPage page)
    {
        last_page = page;
    }
}
