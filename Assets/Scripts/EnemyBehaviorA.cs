using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviorA : MonoBehaviour
{
    private float enemyHealth = 3;
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
            //Destroy(this.gameObject);
        }
    }
}
