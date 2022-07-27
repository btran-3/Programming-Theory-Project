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
    [SerializeField] Collider enemyCollider;
    [SerializeField] NavMeshAgent navMeshAgent;
    Rigidbody rb;

    float defaultSpeed;
    float colliderRadius;


    //external references
    #endregion

    #region dynamic variables
    #endregion


    private void Start()
    {
        defaultSpeed = enemySpeed;
        //navMeshAgent.speed = enemySpeed;
        navMeshAgent.acceleration = enemyAcceleration;
        navMeshAgent.stoppingDistance = enemyStoppingDistance;
        //for some reason, setting navmesh to false here prevents it from being set to true
        //in the FollowPlayer() method...not sure why?
        enemyCollider = GetComponent<Collider>();
        colliderRadius = enemyCollider.bounds.extents.x;
        //Debug.Log(colliderRadius);

        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (isTrackingPlayer && navMeshAgent.enabled)
        {
            //update destination position every frame
            navMeshAgent.SetDestination(playerGO.transform.position);
        }

        if (Vector3.Distance(transform.position, playerGO.transform.position) > (colliderRadius + 0.65f))
        {
            rb.isKinematic = true;
            navMeshAgent.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            navMeshAgent.enabled = false;
            rb.isKinematic = false;
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


    protected override void EnableEnemyMovement() //designed for NavMeshAgent
    {
        navMeshAgent.enabled = true; //place here so the enemies get "bumped up" by NavMesh before they're on screen
        Invoke("DelayFollowPlayerNavMesh", enemyMovementDelay);
    }

    void DelayFollowPlayerNavMesh()
    {
        isTrackingPlayer = true;
        LeanTween.value(0, defaultSpeed, 1f).setEase(LeanTweenType.easeInOutCubic).setOnUpdate(UpdateEnemySpeed);
    }

    void UpdateEnemySpeed(float value)
    {
        navMeshAgent.speed = value;
    }

    public override void BlankKnockback() //designed for NavMeshAgent
    {
        Vector3 knockbackDirection = (transform.position - playerGO.transform.position).normalized;
        knockbackDirection.y = 0;

        float distanceFromPlayer = Vector3.Distance(transform.position, playerGO.transform.position);
        float distanceDifference = playerBlankRadius - distanceFromPlayer;
        navMeshAgent.velocity = (knockbackDirection * distanceDifference * 3) + knockbackDirection;
    }
    //enemy may or may not be impacted by blank knockback
    //see EnemyBehaviorA for blank knockback code

}
