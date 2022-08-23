using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    [SerializeField] private int moneyValue;
    [SerializeField] private int blankValue;
    [SerializeField] private int healthValue;
    [SerializeField] private string pickupType;

    [SerializeField] private AudioClip[] hitGroundSounds;
    //[SerializeField] private OnDestroySounds onDestroySounds;
    [SerializeField] private GlobalOnDestroySounds globalOnDestroySounds;

    public int pub_healthValue
    {
        get { return healthValue; }
    }
    public int pub_moneyValue
    {
        get { return moneyValue; }
    }
    public int pub_blankValue
    {
        get { return blankValue; }
    }

    public string pub_pickupType
    {
        get { return pickupType; }
    }

    private Vector3 targetScale;
    private Rigidbody rb;
    private float spawnAngVel = 10f;
    private AudioSource audioSource;

    private bool hasHitGround;


    void Awake()
    {
        this.gameObject.SetActive(false);
        targetScale = transform.localScale;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //transform.rotation = Random.rotation;
        this.gameObject.transform.localScale = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            HitGround();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && pickupType == "HalfHeart"
            && other.gameObject.GetComponent<PlayerBehavior>().pub_currentPlayerHealth < other.gameObject.GetComponent<PlayerBehavior>().pub_maxPlayerHealth)
        {
            rb.useGravity = false;

            Vector3 playerPos = other.gameObject.transform.position;
            Vector3 targetPos = playerPos - transform.position;
            rb.AddForce(targetPos * 10, ForceMode.Acceleration);
        }
        else if (other.gameObject.CompareTag("Player") && pickupType != "HalfHeart")
        {
            rb.useGravity = false;

            Vector3 playerPos = other.gameObject.transform.position;
            Vector3 targetPos = playerPos - transform.position;
            rb.AddForce(targetPos * 10, ForceMode.Acceleration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            rb.useGravity = true;
        }
    }

    private void HitGround()
    {
        if (!hasHitGround)
        {
            hasHitGround = true;
            int randIndex = Random.Range(0, hitGroundSounds.Length);
            audioSource.PlayOneShot(hitGroundSounds[randIndex]);
        }
    }

    public void SpawnPickup()
    {
        gameObject.SetActive(true);
        transform.rotation = Random.rotation;
        rb.angularVelocity = new Vector3
            (RNG(-spawnAngVel, spawnAngVel), RNG(-spawnAngVel, spawnAngVel), RNG(-spawnAngVel, spawnAngVel));
        LeanTween.scale(this.gameObject, targetScale, 0.5f).setEase(LeanTweenType.easeOutQuart)
            .setDelay(Random.Range(0f, 1f));
    }

    public void SpawnPickupAtDispenser()
    {
        gameObject.SetActive(true);
        if (pub_pickupType == "HalfHeart")
        {
            transform.rotation = Quaternion.Euler(90, 0, 0);
            transform.position += new Vector3(-0.25f, 0, 0);
        }
        else if (pub_pickupType == "Blank")
        {
            transform.rotation = Quaternion.Euler(90, 45, 0);
        }
        //transform.rotation = Random.rotation;
        //rb.angularVelocity = new Vector3(RNG(-spawnAngVel, spawnAngVel), RNG(-spawnAngVel, spawnAngVel), RNG(-spawnAngVel, spawnAngVel));
        LeanTween.scale(this.gameObject, targetScale, 0.5f).setEase(LeanTweenType.easeOutQuart);
    }

    private float RNG(float a, float b)
    {
        float x = Random.Range(a, b);
        return x;
    }

    private void OnDestroy()
    {if (globalOnDestroySounds != null)
        {
            globalOnDestroySounds.PlayPickupCollectedSound(pub_pickupType);
        }
    }
}
