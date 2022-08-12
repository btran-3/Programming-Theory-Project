using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    public static MenuNavigation instance;
    [SerializeField] Image blackFade;
    [SerializeField] MusicManager musicManager;

    private void Awake()
    {
        if (instance != null) //if an instance exists already
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        blackFade.gameObject.SetActive(true);
        LeanTween.value(0, 1, 1f).setOnComplete(FadeFromBlack); //delay fading out
        //FadeFromBlack();
    }

    public void LoadThisScene(int sceneIndex)
    {
        if (sceneIndex == 0) //go to main menu
        {
            FadeToBlack();
            StartCoroutine(DelaySwitchScene(sceneIndex, 1));
            StartCoroutine(SwitchMusic(sceneIndex, 1));
        }
        else if (sceneIndex == 1) //go to Main game
        {
            FadeToBlack();
            StartCoroutine(DelaySwitchScene(sceneIndex, 1));
            StartCoroutine(SwitchMusic(sceneIndex, 1));
        }
    }

    void FadeToBlack()
    {
        blackFade.gameObject.SetActive(true);
        LeanTween.cancel(blackFade.gameObject);
        blackFade.color = new Color(0, 0, 0, 0); //0 opacity

        LeanTween.value(0, 1, 1f).setIgnoreTimeScale(true).setOnUpdate(UpdateBlackFadeAlpha);
    }

    void FadeFromBlack()
    {
        blackFade.gameObject.SetActive(true);
        LeanTween.cancel(blackFade.gameObject);
        blackFade.color = new Color(0, 0, 0, 1); //100% opacity

        LeanTween.value(1, 0, 1f).setIgnoreTimeScale(true).setOnUpdate(UpdateBlackFadeAlpha);
        Debug.Log("Should start fading out now");
    }

    IEnumerator DelaySwitchScene(int sceneIndex, float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        FadeFromBlack();
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator SwitchMusic(int sceneIndex, float waitTime)
    {
        if (sceneIndex == 0)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            musicManager.ReturnToDefault();
        }

        if (sceneIndex == 1) //if main game
        {
            yield return new WaitForSecondsRealtime(waitTime);
            musicManager.SwapTrack(musicManager.pub_hostileMusic);
        }

    }

    void UpdateBlackFadeAlpha(float alphaChange)
    {
        blackFade.color = new Color(0, 0, 0, alphaChange);
        Debug.Log("fading to " + alphaChange + " now");
    }

    IEnumerator DisableObject(GameObject disableThis, float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        disableThis.SetActive(false);
    }

}
