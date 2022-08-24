using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePlayer : ProjectileBase
{
    [SerializeField] private Rigidbody playerRB;

    private void Start()
    {
        GameEvents.instance.playerEnteredNewRoom += DisableProjectile;
    }

    public override void ShootProjectile(Vector3 origin, Vector3 shootDirection, float projectileSpeed, float range)
    {
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
    public override void DisableProjectile()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        LeanTween.cancel(gameObject);
        this.gameObject.SetActive(false);
        playerBehavior.projectilePool.Add(this.gameObject);
    }

    private void OnDestroy()
    {
        GameEvents.instance.playerEnteredNewRoom -= DisableProjectile;
    }

}
