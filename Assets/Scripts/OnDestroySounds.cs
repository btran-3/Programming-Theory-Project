using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroySounds : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] AudioClip[] pickUpCollectedSounds;
    [SerializeField] AudioClip[] enemyDeathSounds;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPickupCollectedSound(string pickUpType)
    {
        if (pickUpType == "Blank")
        {
            int rand = Random.Range(0, 3);
            audioSource.PlayOneShot(pickUpCollectedSounds[rand]);

        }
        else if (pickUpType == "Money")
        {
            int rand = Random.Range(3, 6);
            audioSource.PlayOneShot(pickUpCollectedSounds[rand]);
        }
        
    }

    public void PlayEnemyDeathSound(string enemyType)
    {
        if (enemyType == "small")
        {
            audioSource.PlayOneShot(enemyDeathSounds[0], 0.7f);
        }
        else if (enemyType == "large")
        {
            audioSource.PlayOneShot(enemyDeathSounds[1], 0.7f);
        }
        else
        {
            audioSource.PlayOneShot(enemyDeathSounds[0], 0.7f);
            Debug.Log(enemyType + " does not have a custom death sound");
        }
    }

}