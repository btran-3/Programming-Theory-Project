using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public abstract class EnemyBase : MonoBehaviour
{
    #region stats
    [SerializeField] protected string enemyType;
    [SerializeField] protected float enemyHealth;
    [SerializeField] protected int enemyDamage;
    [SerializeField] protected float enemySpeed;
    #endregion

    #region public get variables
    public float pub_enemyHealth
    {
        get { return enemyHealth; }
        protected set { enemyHealth = value;
            if (enemyHealth <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public int pub_enemyDamage
    {
        get { return enemyDamage; }
    }
    #endregion

    #region references
    //internal
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected Renderer enemyRenderer;
    protected Color defaultColor;

    //external references
    [SerializeField] protected RoomBehavior roomBehavior;
    [SerializeField] protected GameObject playerGO;
    [SerializeField] protected PlayerBehavior playerBehavior;
    [SerializeField] protected OnDestroySounds onDestroySounds;
    [SerializeField] protected GlobalOnDestroySounds globalOnDestroySounds;

    [SerializeField] protected AudioClip hitSound;
    #endregion

    #region dynamic variables
    protected bool isTrackingPlayer;
    #endregion

    #region misc. variables
    protected float enemyMovementDelay = 0.5f;
    protected float playerBlankRadius;
    #endregion

    //dealing damage is in Player script

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (GetComponent<Renderer>() != null)
        {
            enemyRenderer = GetComponent<Renderer>();
            defaultColor = enemyRenderer.material.color;
        }
        
        gameObject.SetActive(false);
        playerBlankRadius = playerBehavior.pub_playerBlankRadius;
    }

    public void EnableEnemy() //executed once, when player enters hostile room
    {
        gameObject.SetActive(true);
        EnableEnemyMovement();
    }

    void Update()
    {

    }

    protected void OnTriggerEnter(Collider other)
    {
        HitByPlayerProjectile(other);
    }

    //play sound, take damage, animate hit color, and execute child-specified projectile knockback
    protected virtual void HitByPlayerProjectile(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            //play hit sound before potential death update
            if (this.gameObject.activeInHierarchy)
            {
                audioSource.PlayOneShot(hitSound);
            }

            pub_enemyHealth -= playerBehavior.pub_playerDamage;

            if (enemyRenderer != null)
            {
                LeanTween.cancel(gameObject);
                enemyRenderer.material.color = Color.red;
                LeanTween.color(this.gameObject, defaultColor, 0.25f).setDelay(0.05f)
                    .setEase(LeanTweenType.easeOutCubic);
            }

            ProjectileKnockBack(other);
        }
    }

    protected abstract void ProjectileKnockBack(Collider other);
    //could be calculated differently for navmesh, physics, no knockback at all, etc.
    //see EnemyBehaviorA for navmesh knockback code

    protected abstract void EnableEnemyMovement(); //executed once
    //enemy may use navMesh, position damping, not follow player at all, etc
    //see EnemyBehaviorA for Navmesh follow player stuff

    public abstract void BlankKnockback();
    //enemy may or may not be impacted by blank knockback
    //see EnemyBehaviorA for blank knockback code

    protected void OnDestroy()
    {
        globalOnDestroySounds.PlayEnemyDeathSound(enemyType); //remove from room list
        roomBehavior.spawnedEnemies.Remove(gameObject);
    }

}
