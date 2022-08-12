using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{


    //THANKS https://www.youtube.com/watch?v=1VXeyeLthdQ


    [SerializeField] AudioClip defaultAmbience;
    [SerializeField] AudioClip hostileMusic;

    public AudioClip pub_defaultAmbiance
    {
        get { return defaultAmbience; }
    }
    public AudioClip pub_hostileMusic
    {
        get { return hostileMusic; }
    }

    public AudioSource track01, track02;
    private bool isPlayingTrack01;
    public static MusicManager instance;

    private void Awake()
    {

        //this allows menu buttons to retain references to the original instances in each scene
        /*
        if (instance == null) //keep this first instance
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) //this instance is not the same as existing one, destroy the old one and use new one
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }*/

        if (instance != null) //if an instance exists already, use the new one
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        track01 = gameObject.AddComponent<AudioSource>();
        track01.loop = true;
        track02 = gameObject.AddComponent<AudioSource>();
        track02.loop = true;
        isPlayingTrack01 = true;
    }

    public void SwapTrack(AudioClip newClip)
    {
        StopAllCoroutines();
        StartCoroutine(FadeTrack(newClip));
        isPlayingTrack01 = !isPlayingTrack01;
    }

    public IEnumerator DelaySwapTrack(AudioClip audioClip, float delay)
    {
        yield return new WaitForSeconds(delay);
        track01.loop = true;
        track02.loop = true;
        SwapTrack(audioClip);
    }

    public void ReturnToDefault()
    {
        //SwapTrack(defaultAmbience);
    }

    private IEnumerator FadeTrack(AudioClip newClip)
    {
        float timeToFade = 0.5f;
        float timeElapsed = 0;

        if (isPlayingTrack01)
        {
            track02.clip = newClip;
            track02.Play();

            while (timeElapsed < timeToFade)
            {
                track02.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                track01.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            track01.Stop();
        }
        else
        {
            track01.clip = newClip;
            track01.Play();

            while (timeElapsed < timeToFade)
            {
                track01.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                track02.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

                track02.Stop();
        }
    }

}
