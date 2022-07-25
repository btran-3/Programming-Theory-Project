using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : EnemyBase
{
    [SerializeField] Renderer[] enemyRenderers;
    [SerializeField] GameObject enemyProjectilePrefab;

    private float projectileSpeed = 7f;
    private float projectileRange = 0.85f;
    private float distanceFromPlayer;
    private float shootingCooldown = 1.5f;
    private float enemyShootTiming;

    private void Start()
    {
        defaultColor = enemyRenderers[0].material.color;
    }

    private void Update()
    {
        LookAtPlayer();

        if (Time.time > enemyShootTiming && distanceFromPlayer <= 8f && isTrackingPlayer)
        {
            enemyShootTiming = Time.time + shootingCooldown;
            GameObject newProjectile = Instantiate(enemyProjectilePrefab, transform.position, transform.rotation);
            newProjectile.SetActive(true); //MUST SET ACTIVE HERE
            newProjectile.GetComponent<ProjectileEnemy>()
                .ShootProjectile(transform.position, Vector3.forward, projectileSpeed, projectileRange);
        }

    }

    private void LookAtPlayer()
    {
        int damping = 10;

        Vector3 lookPosition = playerGO.transform.position - transform.position;
        lookPosition.y = 0;
        var rotation = Quaternion.LookRotation(lookPosition);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);

        distanceFromPlayer = Vector3.Distance(transform.position, playerGO.transform.position);
    }

    protected override void HitByPlayerProjectile(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            //play hit sound before potential death update
            if (this.gameObject.activeInHierarchy)
            {
                audioSource.PlayOneShot(hitSound);
            }
            pub_enemyHealth -= playerBehavior.pub_playerDamage;

            //had to override because this enemy has 2 meshes and 2 renderers
            for (int i = 0; i < enemyRenderers.Length; i++)
            {
                LeanTween.cancel(enemyRenderers[i].gameObject);
                enemyRenderers[i].material.color = Color.red;
                LeanTween.color(enemyRenderers[i].gameObject, defaultColor, 0.25f).setDelay(0.05f)
                    .setEase(LeanTweenType.easeOutCubic);
            }

            ProjectileKnockBack(other);
        }
    }

    protected override void ProjectileKnockBack(Collider other) 
    {
        //enemy is not knocked physically back at all
    }

    protected override void EnableEnemyMovement() //enemy only rotates head to shoot player
    {
        Invoke("DelayShooting", enemyMovementDelay);
    }

    void DelayShooting()
    {
        isTrackingPlayer = true;
    }


    public override void BlankKnockback() //designed for 
    {

    }

}
