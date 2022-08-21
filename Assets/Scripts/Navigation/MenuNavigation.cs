using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;
#if UNITY_EDITOR
using UnityEditor; //namespace only included if compiling in Unity Editor, else discarded
#endif


// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be initialized before setting the UI
[DefaultExecutionOrder(1500)]
public class MenuNavigation : MonoBehaviour
{
    public static MenuNavigation instance;
    [SerializeField] Image blackFade;

    private float introDelay = 0.75f;
    private float fadeDuration = 0.75f;

    private int playerId = 0;
    private Player rewiredPlayer;


    private void Awake()
    {
        //this allows retaining references to the original instances in each scene rather than destroying originals

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
        }

        rewiredPlayer = ReInput.players.GetPlayer(playerId);

    }

    private void Start()
    {
        //Time.timeScale = 0f;

        blackFade.gameObject.SetActive(true);
        LeanTween.value(1, 0, fadeDuration).setDelay(introDelay).setIgnoreTimeScale(true).setOnUpdate(UpdateBlackFadeAlpha);

        rewiredPlayer.controllers.maps.SetAllMapsEnabled(false); //disable all maps by default

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 0) //menu scene
        {
            //Time.timeScale = 1;
            MusicManager.instance.SwapTrack(MusicManager.instance.pub_defaultAmbiance);
            StartCoroutine(ChangeRewiredInputStatus("Menu Category", true, introDelay + fadeDuration)); //prevent running into existing fading animation
        }
        else if (sceneIndex == 1) //main game
        {
            //StartCoroutine(ChangeTimeScale(1f, 1f)); //change timescale to 1 after 1 second
            //Debug.Log("Timescale should be set to 1 after 1 second");
            //Debug.Log(Time.timeScale);
            MusicManager.instance.SwapTrack(MusicManager.instance.pub_hostileMusic);
            StartCoroutine(ChangeRewiredInputStatus("Default", true, introDelay + fadeDuration)); //prevent running into existing fading animation
        }

    }

    public void LoadThisScene(int sceneIndex) //used in button
    {
        FadeToBlack();

        rewiredPlayer.controllers.maps.SetAllMapsEnabled(false);
        if (sceneIndex == 0) //go to main menu
        {
            StartCoroutine(DelaySwitchScene(sceneIndex, fadeDuration));
        }
        else if (sceneIndex == 1) //go to Main game
        {
            StartCoroutine(DelaySwitchScene(sceneIndex, fadeDuration));
        }
    }

    void FadeToBlack()
    {
        blackFade.gameObject.SetActive(true);
        LeanTween.cancel(blackFade.gameObject);
        float startingOpacity = blackFade.color.a;

        LeanTween.value(startingOpacity, 1, fadeDuration).setIgnoreTimeScale(true).setOnUpdate(UpdateBlackFadeAlpha);
    }

    void FadeFromBlack()
    {
        blackFade.gameObject.SetActive(true);
        LeanTween.cancel(blackFade.gameObject);
        blackFade.color = new Color(0, 0, 0, 1); //100% opacity

        LeanTween.value(1, 0, fadeDuration).setIgnoreTimeScale(true).setOnUpdate(UpdateBlackFadeAlpha);
    }

    IEnumerator DelaySwitchScene(int sceneIndex, float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        FadeFromBlack();
        SceneManager.LoadScene(sceneIndex);
        Time.timeScale = 1;
        Debug.Log("The timescale is " + Time.timeScale);
    }

    void UpdateBlackFadeAlpha(float alphaChange)
    {
        if (blackFade != null)
        {
            blackFade.color = new Color(0, 0, 0, alphaChange);
        }
    }

    IEnumerator ChangeRewiredInputStatus(string categoryName, bool state, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        rewiredPlayer.controllers.maps.SetMapsEnabled(state, categoryName);
    }

    IEnumerator ChangeTimeScale(float timeScale, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = timeScale;
        Debug.Log(Time.timeScale);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else   
        Application.Quit();
#endif
    }

}
