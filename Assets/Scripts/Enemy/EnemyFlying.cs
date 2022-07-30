using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlying : EnemyBase
{
    Rigidbody rb;
    float noiseAmp = 20f;
    float noiseFreq = 0.1f;
    float defaultSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultSpeed = enemySpeed;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isTrackingPlayer)
        {
            Vector3 playerPos = playerGO.transform.position;
            Vector3 targetPos = playerPos - transform.position;
            Vector3 targetPosNormalized = (playerPos - transform.position).normalized;
            Vector3 targetPosAveraged = (targetPos + targetPosNormalized);
            targetPosAveraged.y = 0;
            //rb.AddForce(targetPosNormalized * enemySpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
            rb.velocity = (targetPosAveraged + targetPosNormalized) * enemySpeed * Time.fixedDeltaTime;

            float randSeed = Random.Range(-1000f, 1000f);
            float xVar = (Mathf.PerlinNoise((Time.time + randSeed) * noiseFreq, 0f) - 0.5f) * noiseAmp;
            float yVar = (Mathf.PerlinNoise(0f, (Time.time + randSeed) * noiseFreq) - 0.5f) * noiseAmp;
            Vector3 addRandomForce = new Vector3(xVar, 0f, yVar);
            rb.velocity += addRandomForce;
        }


    }

    private void LateUpdate()
    {
        transform.position = new Vector3(rb.position.x, 1, rb.position.z);
    }

    protected override void ProjectileKnockBack(Collider other) //for rigidbody enemy
    {
        rb.velocity *= 0f;
    }


    protected override void EnableEnemyMovement()
    {
        Invoke("DelayCanFollowPlayer", enemyMovementDelay);
    }

    void DelayCanFollowPlayer()
    {
        isTrackingPlayer = true;

        //updates enemySpeed value only during tween. Awesome.
        LeanTween.value(0, defaultSpeed, 1f).setEase(LeanTweenType.easeInOutCubic).setOnUpdate(UpdateEnemySpeed);
    }

    void UpdateEnemySpeed(float value)
    {
        enemySpeed = value;
        //Debug.Log(enemySpeed);
    }

    public override void BlankKnockback()
    {
        enemySpeed = 0f;
        isTrackingPlayer = false;
        Vector3 knockbackDirection = (transform.position - playerGO.transform.position).normalized;
        knockbackDirection.y = 0;

        float distanceFromPlayer = Vector3.Distance(transform.position, playerGO.transform.position);
        float distanceDifference = playerBlankRadius - distanceFromPlayer;
        rb.velocity = knockbackDirection * distanceDifference * 2.5f;

        Invoke("DelayCanFollowPlayer", enemyMovementDelay);
    }
    //enemy may or may not be impacted by blank knockback
    //see EnemyBehaviorA for blank knockback code

}
