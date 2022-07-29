using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalOnDestroySounds : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] playerDeathSounds;
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
            audioSource.PlayOneShot(blankPickupSounds[rand]);

        }
        else if (pickUpType == "HalfHeart")
        {
            audioSource.PlayOneShot(halfHeartPickupSound);
        }

    }

    private void OnDestroy()
    {
        GameEvents.instance.upgradeItemPlaySFX -= PlayUpgradeItemSound;
    }
}
