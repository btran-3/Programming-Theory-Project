using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected float enemyHealth;
    protected float enemySpeed;
    protected float enemyDamage;

    protected virtual void DealDamage()
    {
        //deal damage to player
    }


    void Update()
    {
        
    }
}
