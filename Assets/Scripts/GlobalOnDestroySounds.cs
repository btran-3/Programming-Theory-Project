using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalOnDestroySounds : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] pickupCollectedSounds;
    [SerializeField] private AudioClip[] playerDeathSounds;
    [SerializeField] private AudioClip[] enemyDeathSounds;
    [SerializeField] private AudioClip projectileHitObstacleSound;
    [SerializeField] private AudioClip upgradeItemSound;
    [SerializeField] private AudioClip[] blankPickupSounds;
    [SerializeField] private AudioClip halfHeartPickupSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameEvents.instance.upgradeItemPlaySFX += PlayUpgradeItemSound;
    }

    public void PlayPlayerDeathSound()
    {
        int rand = Random.Range(0, playerDeathSounds.Length);
        audioSource.PlayOneShot(playerDeathSounds[rand]);
    }

    public void PlayEnemyDeathSound(string enemyType)
    {
        if (enemyType == "normal")
        {
            audioSource.PlayOneShot(enemyDeathSounds[0]);
        }
        else if (enemyType == "normalBigger")
        {
            audioSource.PlayOneShot(enemyDeathSounds[1]);
        }
        else if (enemyType == "flying")
        {
            audioSource.PlayOneShot(enemyDeathSounds[0]);
            Debug.Log("assign custom death sound for flying");
        }
        else if (enemyType == "shooting")
        {
            audioSource.PlayOneShot(enemyDeathSounds[0]);
            Debug.Log("assign custom death sound for shooting");
        }
    }

    public void PlayProjectileHitObstacleSound()
    {
        audioSource.PlayOneShot(projectileHitObstacleSound, 0.3f);
    }

    void PlayUpgradeItemSound()
    {
        audioSource.PlayOneShot(upgradeItemSound);
    }

    public void PlayPickupCollectedSound(string pickUpType)
    {
        if (pickUpType == "Blank")
        {
            int rand = Random.Range(0, 3);
            audioSource.PlayOneShot(pickupCollectedSounds[rand]);
        }
        else if (pickUpType == "Money")
        {
            int rand = Random.Range(3, 6);
            audioSource.PlayOneShot(pickupCollectedSounds[rand]);
        }
        else if (pickUpType == "HalfHeart")
        {
            audioSource.PlayOneShot(pickupCollectedSounds[6]);
        }
    }

    private void OnDestroy()
    {
        GameEvents.instance.upgradeItemPlaySFX -= PlayUpgradeItemSound;
    }
}
