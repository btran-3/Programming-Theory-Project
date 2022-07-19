using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    [SerializeField] private int moneyValue;
    [SerializeField] private int blankValue;
    [SerializeField] private string pickupType;

    [SerializeField] private AudioClip[] hitGroundSounds;

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
        this.gameObject.transform.localScale = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            HitGround();
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

    private float RNG(float a, float b)
    {
        float x = Random.Range(a, b);
        return x;
    }

}
