using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePlayer : ProjectileBase
{
    [SerializeField] private Rigidbody playerRB;

    private void Start()
    {
        GameEvents.instance.playerEnteredNewRoom += DisableProjectileIfActive;
    }

    public override void ShootProjectile(Vector3 origin, Vector3 shootDirection, float projectileSpeed, float range)
    {
        LeanTween.cancel(gameObject);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localScale = startingScale;
        transform.position = origin;
        this.gameObject.SetActive(true);
        playerBehavior.projectilePool.Remove(this.gameObject);
        rb.AddForce((shootDirection * projectileSpeed) + (playerRB.velocity / 2), ForceMode.Impulse);
        
        LeanTween.scale(this.gameObject, (startingScale / 3f), range).setEase(LeanTweenType.easeInQuart).setDelay(range / 2).setOnComplete(DisableProjectile);
    }

    protected override void DisableProjectile(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            DisableProjectile();
        }
        else if (other.gameObject.CompareTag("Environment"))
        {
            DisableProjectile();
            globalOnDestroySounds.PlayProjectileHitObstacleSound();
        }
    }

    private void DisableProjectileIfActive()
    {
        if (gameObject.activeInHierarchy)
        {
            DisableProjectile();
        }
    }

    public override void DisableProjectile()
    {
        //GlobalOnDestroySounds.instance.PlayDebugSound();
        //Debug.Log(gameObject.name + " has been disabled");

        LeanTween.cancel(gameObject);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        transform.localScale = startingScale;
        this.gameObject.SetActive(false);
        playerBehavior.projectilePool.Add(this.gameObject);
    }

    private void OnDestroy()
    {
        GameEvents.instance.playerEnteredNewRoom -= DisableProjectile;
    }

}
