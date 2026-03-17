using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interference : Skill
{
    public override int Order => 999;

    public override void OnSkillInvoke(Vector2Int target)
    {
        PVEController.instance.PlayerSkill<interference>(target);
    }
}