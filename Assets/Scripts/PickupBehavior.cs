using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    [SerializeField] private int moneyValue;
    [SerializeField] private int blankValue;

    [SerializeField] private AudioClip[] pickupSounds;

    public int pub_moneyValue
    {
        get { return moneyValue; }
    }
    public int pub_blankValue
    {
        get { return blankValue; }
    }

    private Vector3 targetScale;
    private Rigidbody rb;
    private float spawnAngVelocity = 10f;

    private bool hasHitGround;


    void Awake()
    {
        this.gameObject.SetActive(false);
        targetScale = transform.localScale;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.gameObject.transform.localScale = Vector3.zero;
    }

    private void HitGround()
    {
        if (!hasHitGround)
        {
            hasHitGround = true;

        }
    }

    public void CollectedByPlayer()
    {
        Debug.Log("was collected by player");
    }

    public void SpawnPickup()
    {
        gameObject.SetActive(true);
        transform.rotation = Random.rotation;
        rb.angularVelocity = new Vector3
            (RNG(-spawnAngVelocity, spawnAngVelocity), RNG(-spawnAngVelocity, spawnAngVelocity), RNG(-spawnAngVelocity, spawnAngVelocity));
        LeanTween.scale(this.gameObject, targetScale, 0.5f).setEase(LeanTweenType.easeOutQuart).setDelay(Random.Range(0f, 0.5f));

    }

    private float RNG(float a, float b)
    {
        float x = Random.Range(a, b);
        return x;
    }

}
