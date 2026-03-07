using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomb_focus : bomb{}
public class bomb_wide:bomb{}
public class bomb : Skill
{
    public override int Order => 99;

    public override void OnSkillInvoke(Vector2Int target)
    {
        PVEController.instance.PlayerHit(target);
    }
}
