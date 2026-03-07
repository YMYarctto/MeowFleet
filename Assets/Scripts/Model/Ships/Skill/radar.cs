using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class radar : Skill
{
    public override int Order => 0;

    public override void OnSkillInvoke(Vector2Int target)
    {
        PVEController.instance.PlayerCheck(target);
    }
}