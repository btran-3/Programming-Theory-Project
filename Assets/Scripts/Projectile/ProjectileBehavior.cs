using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField] GameObject playerGO;
    [SerializeField] PlayerBehavior playerBehavior;
    [SerializeField] GlobalOnDestroySounds globalOnDestroySounds;

    private Rigidbody rb;
    private Rigidbody playerRB;

    private Vector3 startingScale;
    private float range;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerRB = playerGO.GetComponent<Rigidbody>();

        startingScale = transform.localScale;
    }

    private void Start()
    {
        range = playerBehavior.pub_playerProjectileRange;
        this.gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        DisableProjectileUponHit(other);
    }

    public void ShootProjectile(Vector3 shootdirection)
    {
        transform.localScale = startingScale;
        transform.position = playerGO.transform.position;
        this.gameObject.SetActive(true);
        playerBehavior.projectilePool.Remove(this.gameObject);
        rb.AddForce((shootdirection * playerBehavior.pub_projectileSpeed) + (playerRB.velocity / 2), ForceMode.Impulse);
        LeanTween.scale(this.gameObject, (startingScale / 3f), range/2).setEase(LeanTweenType.easeInQuart).setDelay(range/2).setOnComplete(DisableProjectile);
    }

    private void DisableProjectileUponHit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            DisableProjectile();
        }
        else if (other.gameObject.CompareTag("Environment"))
        {
            DisableProjectile();
            globalOnDestroySounds.playProjectileHitObstacleSound();
        }
    }

    private void DisableProjectile()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        LeanTween.cancel(gameObject);
        this.gameObject.SetActive(false);
        playerBehavior.projectilePool.Add(this.gameObject);
    }
}
