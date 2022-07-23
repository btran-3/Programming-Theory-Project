using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    //[SerializeField] protected GameObject playerGO;
    [SerializeField] protected PlayerBehavior playerBehavior;
    [SerializeField] protected GlobalOnDestroySounds globalOnDestroySounds;

    protected Rigidbody rb;
    protected Vector3 startingScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startingScale = transform.localScale;

        this.gameObject.SetActive(false);
    }

    //public abstract void ShootProjectile(Vector3 shootDirection);
    public abstract void ShootProjectile(Vector3 origin, Vector3 shootDirection, float projectileSpeed, float range);

    protected void OnTriggerEnter(Collider other)
    {
        DisableProjectile(other);
    }

    protected abstract void DisableProjectile(Collider other); //checks triggers
    public abstract void DisableProjectile();
    //simply destroy GO for enemies
    //look at ProjectileBehavior for player's projectile code
}
