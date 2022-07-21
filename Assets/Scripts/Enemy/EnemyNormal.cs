using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class EnemyNormal : EnemyBase
{
    [SerializeField] protected float enemyAcceleration = 15f;
    [SerializeField] protected float enemyStoppingDistance = 1.4f;

    #region references
    //internal
    [SerializeField] protected NavMeshAgent navMeshAgent;

    //external references
    #endregion

    protected override void Start()
    {
        enemySpeed = 5f;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = enemySpeed;
        navMeshAgent.acceleration = enemyAcceleration;
        navMeshAgent.stoppingDistance = enemyStoppingDistance;
        navMeshAgent.enabled = false;
    }

    protected override void ProjectileKnockBack()
    {

    }
    //could be calculated differently for navmesh, physics, no knockback at all, etc.
    //see EnemyBehaviorA for navmesh knockback code


    protected override void FollowPlayer()
    {
        Debug.Log("overridden method in normal enemy");
    }
    //enemy may use navMesh, position damping, not follow player at all, etc


    protected override void BlankKnockBack()
    {

    }
    //enemy may or may not be impacted by blank knockback
    //see EnemyBehaviorA for blank knockback code

}
