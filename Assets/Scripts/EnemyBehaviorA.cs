using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviorA : MonoBehaviour
{
    [SerializeField] private float enemyHealth = 6;
    [SerializeField] private int dealDamage;
    [SerializeField] private string enemyType;

    public float pub_enemyHealth {
        get { return enemyHealth; }
        private set { enemyHealth = value;
            if (enemyHealth <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
    public int pub_dealDamage { get { return dealDamage; } }

    [SerializeField] private RoomBehavior roomBehavior;

    [SerializeField] private GameObject playerGO;
    [SerializeField] PlayerBehavior playerBehavior;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hitSound;
    [SerializeField] OnDestroySounds onDestroySounds;

    private Renderer enemyRenderer;
    private Color defaultColor;

    private float playerBlankRadius;
    private bool isFollowingPlayer;
    private NavMeshAgent agent;

    private void Awake()
    {
        //gameObject.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    void Start()
    {
        playerBlankRadius = playerBehavior.pub_playerBlankRadius;
        
        enemyRenderer = GetComponent<Renderer>();
        defaultColor = enemyRenderer.material.color;
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (isFollowingPlayer)
        {
            agent.SetDestination(playerGO.transform.position);
        }
    }

    public void SpawnEnableEnemy()
    {
        //Debug.Log("Spawned an enemy");
        gameObject.SetActive(true);
        agent.enabled = true;
        Invoke("StartFollowingPlayer", 0.75f);
    }

    private void StartFollowingPlayer()
    {
        isFollowingPlayer = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        HitByPlayerProjectile(other);
    }

    private void HitByPlayerProjectile(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            if (this.gameObject.activeInHierarchy)//play sound before potential death update
            {
                audioSource.PlayOneShot(hitSound);
            }

            pub_enemyHealth -= playerBehavior.pub_playerDamage;
            //THANKS https://www.youtube.com/watch?v=gFq0lO2E2Sc
            Vector3 knockbackDirection;
            knockbackDirection.x = transform.position.x - other.gameObject.transform.position.x;
            knockbackDirection.y = 0f;
            knockbackDirection.z = transform.position.z - other.gameObject.transform.position.z;
            agent.velocity += knockbackDirection * 3; //should vary based on enemy size per class
            //agent.velocity = Vector3.zero;

            LeanTween.cancel(gameObject);
            enemyRenderer.material.color = Color.red;
            LeanTween.color(this.gameObject, defaultColor, 0.25f).setDelay(0.05f).setEase(LeanTweenType.easeOutCubic);
        }
    }

    public void BlankKnockback()
    {
        Vector3 knockbackDirection = (transform.position - playerGO.transform.position).normalized;
        knockbackDirection.y = 0;

        float distanceFromPlayer = Vector3.Distance(transform.position, playerGO.transform.position);
        float distanceDifference = playerBlankRadius - distanceFromPlayer;
        agent.velocity = (knockbackDirection * distanceDifference * 3) + knockbackDirection;
    }

    private void OnDestroy() //remove from room list
    {
        roomBehavior.spawnedEnemies.Remove(this.gameObject);
        onDestroySounds.PlayEnemyDeathSound(enemyType);
    }

}
