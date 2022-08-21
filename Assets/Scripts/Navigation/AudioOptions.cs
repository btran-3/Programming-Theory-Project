using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioOptions : MonoBehaviour
{
    //THANKS https://www.youtube.com/watch?v=LfU5xotjbPw

    public static float soundEffectsVolume { get; private set; }
    public static float musicVolume { get; private set; }

    public static AudioOptions instance;

    [SerializeField] Slider soundEffectsSlider;
    [SerializeField] Slider musicVolumeSlider;

    private static bool hasSoundBeenInitialized;

    private void Awake()
    {
        //use the instance that already exists in the scene
        //this allows retaining references to scene-specific objects

        if (instance == null) //keep this first instance
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (!hasSoundBeenInitialized) //set audio defaults once
        {
            hasSoundBeenInitialized = true;

            soundEffectsVolume = 1;
            musicVolume = 1;

            if (PlayerPrefs.HasKey("musicVolumePref"))
            {
                musicVolume = PlayerPrefs.GetFloat("musicVolumePref");
            }
            if (PlayerPrefs.HasKey("soundEffectsVolumePref"))
            {
                soundEffectsVolume = PlayerPrefs.GetFloat("soundEffectsVolumePref");
            }
        }

        soundEffectsSlider.value = soundEffectsVolume;
        musicVolumeSlider.value = musicVolume;
    }

    public void OnSoundEffectsSliderValueChange(float value)
    {
        soundEffectsVolume = value;
        //Debug.Log("SFX " + value);

        if (GlobalOnDestroySounds.instance != null) //menu scene does not have one of these
        {
            GlobalOnDestroySounds.instance.UpdateSoundEffectsMixerVolume(soundEffectsVolume);
        }
        else
        {
            Debug.LogWarning("cannot find instance of GlobalOnDestroySounds");
        }

        PlayerPrefs.SetFloat("soundEffectsVolumePref", value);
    }

    public void OnMusicSliderValueChange(float value)
    {
        musicVolume = value;

        MusicManager.instance.UpdateMusicMixerVolume(musicVolume);
        //Debug.Log("music manager exists");

        PlayerPrefs.SetFloat("musicVolumePref", value);
    }

}
