using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField] GameObject playerGO;
    [SerializeField] PlayerBehavior playerBehavior;
    
    private Rigidbody rb;
    private Rigidbody playerRB;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerRB = playerGO.GetComponent<Rigidbody>();
        this.gameObject.SetActive(false);
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        DisableProjectile(other);
    }

    public void ShootProjectile(Vector3 shootdirection)
    {
        transform.position = playerGO.transform.position;
        playerBehavior.projectilePool.Remove(this.gameObject);
        this.gameObject.SetActive(true);
        rb.AddForce((shootdirection * playerBehavior.pub_projectileSpeed) + (playerRB.velocity / 2), ForceMode.Impulse);
    }

    private void DisableProjectile(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Environment"))
        {
            this.gameObject.SetActive(false);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            playerBehavior.projectilePool.Add(this.gameObject);
        }
    }
}
