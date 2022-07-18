using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    [SerializeField] private int moneyValue;
    [SerializeField] private int blankValue;

    private Vector3 targetScale;
    private Rigidbody rb;
    private float spawnAngVelocity = 10f;


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

    void Update()
    {
        
    }

    public void SpawnPickup()
    {
        gameObject.SetActive(true);
        transform.rotation = Random.rotation;
        rb.angularVelocity = new Vector3
            (RNG(-spawnAngVelocity, spawnAngVelocity), RNG(-spawnAngVelocity, spawnAngVelocity), RNG(-spawnAngVelocity, spawnAngVelocity));
        LeanTween.scale(this.gameObject, targetScale, 0.5f).setEase(LeanTweenType.easeOutQuart).setDelay(0.5f);

    }

    private float RNG(float a, float b)
    {
        float x = Random.Range(a, b);
        return x;
    }

}
