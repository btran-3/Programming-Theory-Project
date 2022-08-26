using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemy : ProjectileBase
{
    private int enemyProjectileDamage = 1;

    public int pub_enemyProjectileDamage
    {
        get { return enemyProjectileDamage; }
    }

    //needs to be destroyed if hit by player blank

    private void Start()
    {
        
    }

    private void Update()
    {

    }

    public override void ShootProjectile(Vector3 origin, Vector3 shootDirection, float projectileSpeed, float range)
    {
        //Debug.Log("I'm supposed to be shooting now");
        //Debug.Log(shootDirection);
        //Debug.Log(projectileSpeed);
        this.gameObject.SetActive(true); //must override the Awake() in ProjectileBase
        rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce((shootDirection * projectileSpeed), ForceMode.Impulse);
        LeanTween.scale(this.gameObject, (startingScale / 3f), range / 2).setEase(LeanTweenType.easeInQuart).setDelay(range / 2).setOnComplete(DisableProjectile);
    }

    protected override void DisableProjectile(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
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
        Destroy(gameObject);
    }
}
