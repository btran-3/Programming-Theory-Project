using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlying : EnemyBase
{

    protected override void ProjectileKnockBack(Collider other)
    {

    }
    //could be calculated differently for navmesh, physics, no knockback at all, etc.
    //see EnemyBehaviorA for navmesh knockback code


    protected override void FollowPlayer()
    {
        Debug.Log("overridden method in flying enemy");
    }
    //enemy may use navMesh, position damping, not follow player at all, etc


    protected override void BlankKnockBack()
    {

    }
    //enemy may or may not be impacted by blank knockback
    //see EnemyBehaviorA for blank knockback code

}
