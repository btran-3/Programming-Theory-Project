using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviorA : MonoBehaviour
{
    [SerializeField]
    private float enemyHealth = 6;
    public float pub_enemyHealth {
        get { return enemyHealth; }
        private set { enemyHealth = value;
            if (enemyHealth <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    [SerializeField] private GameObject playerGO;
    [SerializeField] PlayerBehavior playerBehavior;
    private Renderer enemyRenderer;
    private Color defaultColor;

    private float playerBlankRadius;
    private bool isFollowingPlayer;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    void Start()
    {
        playerBlankRadius = playerBehavior.pub_playerBlankRadius;
        gameObject.SetActive(false);
        enemyRenderer = GetComponent<Renderer>();
        defaultColor = enemyRenderer.material.color;
    }


    void Update()
    {
        if (isFollowingPlayer)
        {
            agent.SetDestination(playerGO.transform.position);
        }
    }

    public void SpawnEnemy()
    {
        gameObject.SetActive(true);
        agent.enabled = true;
        Invoke("StartFollowingPlayer", 0.5f);
    }

    private void StartFollowingPlayer()
    {
        isFollowingPlayer = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
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

}
