using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviorA : MonoBehaviour
{
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
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        gameObject.SetActive(false);
    }


    void Update()
    {
        agent.SetDestination(playerGO.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            pub_enemyHealth -= playerBehavior.pub_playerDamage;
            Vector2 hitVector;
            hitVector.x = other.gameObject.transform.position.x - transform.position.x;
            hitVector.y = other.gameObject.transform.position.z - transform.position.z;
            hitVector = hitVector.normalized / 5;
            transform.Translate(hitVector.x, 0, hitVector.y);
        }
    }
}
