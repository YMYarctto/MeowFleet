using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class radar : Skill
{
    public override void OnSkillInvoke(Vector2Int target)
    {
        Vector2Int coord = PVEController.instance.GetEdgeCoord(target, _direction);
        bool vertical = _direction.x == 0;
        Vector2Int size = EnemyController.instance.size;
        for(int i=0;i<(vertical?size.y:size.x);i++)
        {
            bool hit = PVEController.instance.PlayerHit(coord);
            coord += _direction;
            if(hit)break;
        }
    }
}
