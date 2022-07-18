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
    private bool isFollowingPlayer;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.updatePosition = false;
        agent.enabled = false;
    }

    void Start()
    {
        gameObject.SetActive(false);
    }


    void Update()
    {

        
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
            agent.velocity += knockbackDirection * 3; //vary based on enemy size per class

            //agent.velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (isFollowingPlayer)
        {
            agent.SetDestination(playerGO.transform.position);
        }
        //transform.position = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref velocity, 0.3f);
    }

    private void LateUpdate()
    {
        
    }
}
