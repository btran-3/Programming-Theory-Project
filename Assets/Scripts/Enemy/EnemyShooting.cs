using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : EnemyBase
{
    [SerializeField] Renderer[] enemyRenderers;

    private void Start()
    {
        defaultColor = enemyRenderers[0].material.color;
    }

    private void Update()
    {
        if (isTrackingPlayer)
        {
            int damping = 10;

            Vector3 lookPosition = playerGO.transform.position - transform.position;
            lookPosition.y = 0;
            var rotation = Quaternion.LookRotation(lookPosition);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
    }

    protected override void HitByPlayerProjectile(Collider other)
    {
        //base.HitByPlayerProjectile(other); autofilled in?
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            //play hit sound before potential death update
            if (this.gameObject.activeInHierarchy)
            {
                audioSource.PlayOneShot(hitSound);
            }

            pub_enemyHealth -= playerBehavior.pub_playerDamage;

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
        isTrackingPlayer = true;
    }


    protected override void BlankKnockBack() //designed for 
    {

    }

}
