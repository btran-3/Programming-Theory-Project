using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankExplosion : MonoBehaviour
{
    [SerializeField] private PlayerBehavior playerBehavior;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnTriggerStay(Collider other)
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space) && playerBehavior.pub_currentPlayerBlanks > 0)
        {
            playerBehavior.pub_currentPlayerBlanks--;

            if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<EnemyBehaviorA>() != null)
            {
                other.gameObject.GetComponent<EnemyBehaviorA>().BlankKnockback();
            }
        }
        */
    }

}
