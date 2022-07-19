using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField] GameObject playerGO;
    [SerializeField] PlayerBehavior playerBehavior;

    [SerializeField] AudioClip[] audioClips;

    private AudioSource audioSource;
    private Rigidbody rb;
    private Rigidbody playerRB;

    private Vector3 startingScale;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerRB = playerGO.GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        this.gameObject.SetActive(false);
    }

    private void Start()
    {
        startingScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        DisableProjectileUponHit(other);
    }

    public void ShootProjectile(Vector3 shootdirection)
    {
        
        transform.localScale = startingScale;
        transform.position = playerGO.transform.position;
        playerBehavior.projectilePool.Remove(this.gameObject);
        this.gameObject.SetActive(true);
        rb.AddForce((shootdirection * playerBehavior.pub_projectileSpeed) + (playerRB.velocity / 2), ForceMode.Impulse);
        LeanTween.scale(this.gameObject, startingScale / 1.5f, 1f).setEase(LeanTweenType.easeInExpo).setOnComplete(DisableProjectileBecauseOfRange);
    }

    private void DisableProjectileUponHit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Environment"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            LeanTween.cancel(gameObject);
            this.gameObject.SetActive(false);
            playerBehavior.projectilePool.Add(this.gameObject);
        }
    }

    private void DisableProjectileBecauseOfRange()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        LeanTween.cancel(gameObject);
        this.gameObject.SetActive(false);
        playerBehavior.projectilePool.Add(this.gameObject);
    }
}
