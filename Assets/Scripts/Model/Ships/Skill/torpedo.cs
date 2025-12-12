using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class torpedo : Skill
{
    public override void OnSkillInvoke(Vector2Int target)
    {
        Vector2Int coord = PVEController.instance.GetEdgeCoord(target, _direction);
        List<Vector2Int> range = PVEController.instance.CurrentSkillRange;
        PVEController.instance.SetSkillRange(new List<Vector2Int>() { Vector2Int.zero });
        bool vertical = _direction.x == 0;
        Vector2Int size = EnemyController.instance.size;
        for(int i=0;i<(vertical?size.y:size.x);i++)
        {
            bool hit = EnemyController.instance.PlayerCheck(coord).Contains(ActionMessage.ActionResult.Hit);
            if(hit)break;
            PVEController.instance.PlayerHit(coord);
            coord += _direction;
        }
        PVEController.instance.SetSkillRange(range);
        PVEController.instance.PlayerHit(coord);
    }
}
