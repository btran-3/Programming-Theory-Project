using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlying : EnemyBase
{
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isFollowingPlayer)
        {
            Vector3 playerPos = playerGO.transform.position;
            Vector3 targetPos = playerPos - transform.position;
            Vector3 targetPosNormalized = (playerPos - transform.position).normalized;
            Vector3 targetPosAveraged = (targetPos + targetPosNormalized) / 2;
            targetPosAveraged.y = 0;
            rb.velocity = targetPosAveraged * enemySpeed * Time.fixedDeltaTime;
        }
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(rb.position.x, 1, rb.position.z);
    }

    protected override void ProjectileKnockBack(Collider other) //for rigidbody enemy
    {
        Vector3 knockbackDirection;
        knockbackDirection.x = transform.position.x - other.gameObject.transform.position.x;
        knockbackDirection.y = 0f;
        knockbackDirection.z = transform.position.z - other.gameObject.transform.position.z;
        knockbackDirection = (knockbackDirection.normalized) * 10;
        rb.AddForce(knockbackDirection, ForceMode.VelocityChange);
        Debug.Log(knockbackDirection);
    }


    protected override void EnemyMovement()
    {
        Invoke("DelayCanFollowPlayer", enemyMovementDelay);
    }

    void DelayCanFollowPlayer()
    {
        isFollowingPlayer = true;
    }


    protected override void BlankKnockBack()
    {

    }
    //enemy may or may not be impacted by blank knockback
    //see EnemyBehaviorA for blank knockback code

}
