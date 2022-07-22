using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class EnemyNormal : EnemyBase
{
    [SerializeField] private float enemyAcceleration = 15f;
    [SerializeField] private float enemyStoppingDistance = 1.4f;

    #region references
    //internal
    public NavMeshAgent navMeshAgent;

    //external references
    #endregion

    private void Start()
    {
        enemySpeed = 6f;
        navMeshAgent.speed = enemySpeed;
        navMeshAgent.acceleration = enemyAcceleration;
        navMeshAgent.stoppingDistance = enemyStoppingDistance;
        //for some reason, setting navmesh to false here prevents it from being set to true
        //in the FollowPlayer() method...not sure why?
    }

    private void Update()
    {
        if (isFollowingPlayer)
        {
            //update destination position every frame
            navMeshAgent.SetDestination(playerGO.transform.position);
        }
    }

    protected override void ProjectileKnockBack(Collider other) //designed for NavMeshAgent
    {
        //THANKS https://www.youtube.com/watch?v=gFq0lO2E2Sc
        Vector3 knockbackDirection;
        knockbackDirection.x = transform.position.x - other.gameObject.transform.position.x;
        knockbackDirection.y = 0f;
        knockbackDirection.z = transform.position.z - other.gameObject.transform.position.z;
        navMeshAgent.velocity += knockbackDirection * 6;
    }


    protected override void FollowPlayer() //designed for NavMeshAgent
    {
        Invoke("DelayedEnemyMovement", enemyMovementDelay);
    }

    void DelayedEnemyMovement()
    {
        isFollowingPlayer = true;
        navMeshAgent.enabled = true;
    }


    protected override void BlankKnockBack() //designed for NavMeshAgent
    {

    }
    //enemy may or may not be impacted by blank knockback
    //see EnemyBehaviorA for blank knockback code

}
