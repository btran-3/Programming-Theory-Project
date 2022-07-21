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
    [SerializeField] protected float enemyDamage;
    [SerializeField] protected float enemySpeed = 5f;
    [SerializeField] protected float enemyAcceleration = 15f;
    [SerializeField] protected float enemyStoppingDistance = 1.4f;
    #endregion

    #region public get variables
    public float pub_enemyHealth
    {
        get { return enemyHealth; }
        private set { enemyHealth = value;
            if (enemyHealth <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public float pub_enemyDamage
    {
        get { return enemyDamage; }
    }
    #endregion

    #region references
    //internal
    AudioSource audioSource;
    NavMeshAgent navMeshAgent;
    Renderer enemyRenderer;
    Color defaultColor;

    //external references
    [SerializeField] protected RoomBehavior roomBehavior;
    [SerializeField] protected GameObject playerGO;
    [SerializeField] protected PlayerBehavior playerBehavior;
    [SerializeField] protected OnDestroySounds onDestroySounds;

    [SerializeField] AudioClip hitSound;
    #endregion

    #region dynamic variables
    private bool isFollowingPlayer;
    #endregion

    #region misc. variables
    private float playerBlankRadius;
    #endregion

    //dealing damage is in Player script

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        enemyRenderer = GetComponent<Renderer>();
        defaultColor = enemyRenderer.material.color;

        if (GetComponent<NavMeshAgent>() != null) //does this have a NavMeshAgent component?
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.speed = enemySpeed;
            navMeshAgent.acceleration = enemyAcceleration;
            navMeshAgent.stoppingDistance = enemyStoppingDistance;
            navMeshAgent.enabled = false;
        }
        
        gameObject.SetActive(false);
        playerBlankRadius = playerBehavior.pub_playerBlankRadius;
    }

    protected void EnableEnemy()
    {
        gameObject.SetActive(true);
        FollowPlayer();
    }

    void Update()
    {

    }

    protected void OnTriggerEnter(Collider other)
    {
        HitByPlayerProjectile(other);
    }

    //play sound, take damage, animate hit color, and apply custom projectile knockback
    private void HitByPlayerProjectile(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            //play hit sound before potential death update
            if (this.gameObject.activeInHierarchy)
            {
                audioSource.PlayOneShot(hitSound);
            }

            pub_enemyHealth -= playerBehavior.pub_playerDamage;

            LeanTween.cancel(gameObject);
            enemyRenderer.material.color = Color.red;
            LeanTween.color(this.gameObject, defaultColor, 0.25f).setDelay(0.05f)
                .setEase(LeanTweenType.easeOutCubic);

            ProjectileKnockBack();
        }
    }

    protected abstract void ProjectileKnockBack();
    //could be calculated differently for navmesh, physics, no knockback at all, etc.
    //see EnemyBehaviorA for navmesh knockback code

    protected abstract void FollowPlayer();
    //enemy may use navMesh, position damping, not follow player at all, etc

    protected abstract void BlankKnockBack();
    //enemy may or may not be impacted by blank knockback
    //see EnemyBehaviorA for blank knockback code

    protected void OnDestroy()
    {
        onDestroySounds.PlayEnemyDeathSound(enemyType); //remove from room list
        roomBehavior.spawnedEnemies.Remove(gameObject);
    }

}
