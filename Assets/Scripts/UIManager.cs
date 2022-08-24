using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;
using UnityEngine.EventSystems;


// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be initialized before setting the UI
[DefaultExecutionOrder(1000)]
public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject playerGO;
    [SerializeField] PlayerBehavior playerBehavior;

    [Space(10)]
    
    [SerializeField] private Image blackFade;
    [SerializeField] private Image pauseScreen;
    //[SerializeField] private Button menuButton;

    [Space(10)]

    //------------

    [SerializeField] GameObject mainMenu, optionsMenu;
    [SerializeField] GameObject pauseMenuFirstButton, optionsFirstButton, optionsClosedButton, winScreenFirstButton, gameOverScreenFirstButton;
    [SerializeField] GameObject winScreen, gameOverScreen;

    [SerializeField] GameObject newGameButton, optionsButton, exitButton;
    [SerializeField] GameObject optionsSoundEffectsSlider, optionsMusicSlider, optionsSoundEffectText, optionsMusicText;
    [SerializeField] GameObject gameOverText, victoryText;
    GameObject lastSelected;

    private bool isMenuTransitioning;
    private float menuAnimationOffset = 250f;
    private float menuAnimationTime = 0.65f;
    [SerializeField] AnimationCurve menuAnimationCurve;

    private float unselectedUIColorFloat = 0.196f; //323232 divided by 255
    private float selectedUIColorFloat = 0.96f; //F5F5F5 divided by 255

    private float gameTimer;

    private enum MenuState { MAINMENU, OPTIONSMENU };
    private MenuState menuState;

    //----------------


    [Space(10)]
    [SerializeField] GameObject heartPrefab;
    [SerializeField] GameObject healthBarUI;
    List<UIHealthHeart> hearts = new List<UIHealthHeart>();

    [Space (10)]
    //[SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI blanksText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Space (10)] //Rewired stuff
    private int playerId = 0;
    private Player player;

    private bool isGamePaused;
    private bool didPlayerBeatGame;

    public bool pub_isGamePaused
    {
        get { return isGamePaused; }
        set { isGamePaused = value; }
    }


    void Start()
    {
        player = ReInput.players.GetPlayer(playerId);

        DrawHearts();
        SetUIText();
        timerText.SetText("0:00");

        menuState = MenuState.MAINMENU;

        optionsMenu.transform.position += new Vector3(menuAnimationOffset, 0, 0);
        optionsMenu.GetComponent<CanvasGroup>().alpha = 0;
        winScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        pauseScreen.gameObject.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseMenuFirstButton);

        GameEvents.instance.playerBeatGame += ShowWinScreen;
        GameEvents.instance.playerLostGame += ShowGameOverScreen;
    }

    // Update is called once per frame
    void Update()
    {
        PauseUnpause();

        //if hovering on Sound Effects volume text, which covers the slider too
        if (EventSystem.current.currentSelectedGameObject == optionsSoundEffectsSlider && lastSelected != optionsSoundEffectsSlider)
        {
            SelectSoundEffectsSlider();
        }
        //if hovering on Music volume Text, which covers the slider too
        else if (EventSystem.current.currentSelectedGameObject == optionsMusicSlider && lastSelected != optionsMusicSlider)
        {
            SelectMusicSlider();
        }

        //game timer
        if (Time.timeScale == 1 && playerBehavior.pub_canPlayerMove)
        {
            gameTimer += Time.deltaTime;

            timerText.SetText(formattedTimer(gameTimer));
        }
    }

    string formattedTimer(float time)
    {
        int intTime = (int)time;
        int minutes = intTime / 60;
        int seconds = intTime % 60;
        //string timeText = string.Format ("{0:00}:{1:00}", minutes, seconds);
        string timeText = (minutes.ToString() + ":" + seconds.ToString("00"));
        return timeText;
    }

    private void PauseUnpause()
    {
        if (!didPlayerBeatGame)
        {
            if (player.GetButtonDown("Pause") && !pub_isGamePaused)
            {
                PauseGame();
            }
            else if (player.GetButtonDown("PauseMenu") && pub_isGamePaused && menuState == MenuState.MAINMENU)
            {
                ResumeGame();
            }
            else if (player.GetButtonDown("UICancel") && pub_isGamePaused && menuState == MenuState.MAINMENU)
            {
                ResumeGame();
            }
            else if (player.GetButtonDown("UICancel") && pub_isGamePaused && menuState == MenuState.OPTIONSMENU)
            {
                CloseOptionsMenu();
            }
        }
    }

    private void PauseGame()
    {
        menuState = MenuState.MAINMENU;

        player.controllers.maps.SetMapsEnabled(false, "Default");
        player.controllers.maps.SetMapsEnabled(true, "Menu Category");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseMenuFirstButton);
        
        pub_isGamePaused = true;
        pauseScreen.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        menuState = MenuState.MAINMENU;

        player.controllers.maps.SetMapsEnabled(true, "Default");
        player.controllers.maps.SetMapsEnabled(false, "Menu Category");
        pub_isGamePaused = false;
        pauseScreen.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    #region Hearts
    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(healthBarUI.transform); //parent to hearts bar (its transform)
        newHeart.transform.localScale = Vector3.one;

        UIHealthHeart UIHeartComponent = newHeart.GetComponent<UIHealthHeart>();
        UIHeartComponent.SetHeartImage(HeartStatus.Empty); //set instance's script enum status to empty
        hearts.Add(UIHeartComponent); //add instance's script to the list
    }

    public void DrawHearts()
    {
        ClearHearts(); //start fresh

        float maxHealthRemainer = playerBehavior.pub_maxPlayerHealth % 2; //check if even, or odd with a remainer
        int heartsToMake = (int)((playerBehavior.pub_maxPlayerHealth / 2 + maxHealthRemainer)); //convert to int

        for (int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(playerBehavior.pub_currentPlayerHealth - (i * 2), 0, 2);

            //Debug.Log(heartStatusRemainder);

            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }

    public void ClearHearts()
    {
        foreach(Transform t in healthBarUI.transform) //specifically children of the healthBarUI object, not the canvas's children
        {
            Destroy(t.gameObject); //this does not remove them from the canvas list, it only destroys child gameobjects
        }
        hearts = new List<UIHealthHeart>(); //clear out list, removing the missing gameobjects
    }
    #endregion

    #region text updates
    private void SetUIText()
    {
        //healthText.SetText("Health: " + playerBehavior.pub_currentPlayerHealth + "/" + playerBehavior.pub_maxPlayerHealth);
        moneyText.SetText(playerBehavior.pub_currentPlayerMoney.ToString());
        blanksText.SetText(playerBehavior.pub_currentPlayerBlanks.ToString());
    }
    /*
    public void UpdateHealthText()
    {
        healthText.SetText("Health: " + playerBehavior.pub_currentPlayerHealth + "/" +
            playerBehavior.pub_maxPlayerHealth);
    }*/

    public void UpdateMoneyText()
    {
        moneyText.SetText(playerBehavior.pub_currentPlayerMoney.ToString());
    }

    public void UpdateBlanksText()
    {
        blanksText.SetText(playerBehavior.pub_currentPlayerBlanks.ToString());
    }
    #endregion

    public void OpenOptionsMenu()
    {

        LeanTween.cancel(mainMenu);
        LeanTween.cancel(optionsMenu);

        player.controllers.maps.SetMapsEnabled(false, "Menu Category");
        StartCoroutine(ChangeRewiredInputStatus("Menu Category", true, menuAnimationTime));
        menuState = MenuState.OPTIONSMENU;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);


        StartCoroutine(DisableThisObject(mainMenu, menuAnimationTime));
        LeanTween.moveX(mainMenu, mainMenu.transform.position.x - menuAnimationOffset, menuAnimationTime).setEase(menuAnimationCurve).setIgnoreTimeScale(true);
        LeanTween.value(1, 0, (menuAnimationTime * 0.75f)).setDelay(menuAnimationTime * 0.15f).setEase(menuAnimationCurve).setOnUpdate(FadeOutMainMenu).setIgnoreTimeScale(true);

        optionsMenu.SetActive(true);
        LeanTween.moveX(optionsMenu, optionsMenu.transform.position.x - menuAnimationOffset, menuAnimationTime).setEase(menuAnimationCurve).setIgnoreTimeScale(true);
        LeanTween.value(0, 1, (menuAnimationTime * 0.75f)).setDelay(menuAnimationTime * 0.15f).setEase(menuAnimationCurve).setOnUpdate(FadeInOptionsMenu).setIgnoreTimeScale(true);
    }

    private void CloseOptionsMenu()
    {
        LeanTween.cancel(mainMenu);
        LeanTween.cancel(optionsMenu);

        player.controllers.maps.SetMapsEnabled(false, "Menu Category");
        StartCoroutine(ChangeRewiredInputStatus("Menu Category", true, menuAnimationTime));
        menuState = MenuState.MAINMENU;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);


        StartCoroutine(DisableThisObject(optionsMenu, menuAnimationTime));
        LeanTween.moveX(mainMenu, mainMenu.transform.position.x + menuAnimationOffset, menuAnimationTime).setEase(menuAnimationCurve).setIgnoreTimeScale(true);
        LeanTween.value(0, 1, (menuAnimationTime * 0.75f)).setDelay(menuAnimationTime * 0.15f).setEase(menuAnimationCurve).setOnUpdate(FadeInMainMenu).setIgnoreTimeScale(true);

        mainMenu.SetActive(true);
        LeanTween.moveX(optionsMenu, optionsMenu.transform.position.x + menuAnimationOffset, menuAnimationTime).setEase(menuAnimationCurve).setIgnoreTimeScale(true);
        LeanTween.value(1, 0, (menuAnimationTime * 0.75f)).setDelay(menuAnimationTime * 0.15f).setEase(menuAnimationCurve).setOnUpdate(FadeOutOptionsMenu).setIgnoreTimeScale(true);
    }

    private void SelectMusicSlider()
    {
        lastSelected = optionsMusicSlider;

        LeanTween.cancel(optionsSoundEffectText);
        LeanTween.cancel(optionsMusicText);

        LeanTween.value(optionsSoundEffectText.GetComponent<TextMeshProUGUI>().color.r, unselectedUIColorFloat, 0.1f).setOnUpdate(ColorSoundEffectsText).setIgnoreTimeScale(true);
        LeanTween.value(optionsMusicText.GetComponent<TextMeshProUGUI>().color.r, selectedUIColorFloat, 0.1f).setOnUpdate(ColorMusicText).setIgnoreTimeScale(true);
    }

    private void SelectSoundEffectsSlider()
    {
        lastSelected = optionsSoundEffectsSlider;

        LeanTween.cancel(optionsSoundEffectText);
        LeanTween.cancel(optionsMusicText);

        LeanTween.value(optionsSoundEffectText.GetComponent<TextMeshProUGUI>().color.r, selectedUIColorFloat, 0.1f).setOnUpdate(ColorSoundEffectsText).setIgnoreTimeScale(true);
        LeanTween.value(optionsMusicText.GetComponent<TextMeshProUGUI>().color.r, unselectedUIColorFloat, 0.1f).setOnUpdate(ColorMusicText).setIgnoreTimeScale(true);
    }

    #region Rewired

    IEnumerator ChangeRewiredInputStatus(string categoryName, bool state, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        player.controllers.maps.SetMapsEnabled(state, categoryName);
    }
    #endregion

    #region menu fade and color animations for LeanTween SetOnUpdate
    void FadeOutMainMenu(float value)
    {
        mainMenu.GetComponent<CanvasGroup>().alpha = value;
    }
    void FadeInOptionsMenu(float value)
    {
        optionsMenu.GetComponent<CanvasGroup>().alpha = value;
    }
    void FadeInMainMenu(float value)
    {
        mainMenu.GetComponent<CanvasGroup>().alpha = value;
    }
    void FadeOutOptionsMenu(float value)
    {
        optionsMenu.GetComponent<CanvasGroup>().alpha = value;
    }
    void ColorSoundEffectsText(float value)
    {
        optionsSoundEffectText.GetComponent<TextMeshProUGUI>().color = new Color(value, value, value, 1);
    }
    void ColorMusicText(float value)
    {
        optionsMusicText.GetComponent<TextMeshProUGUI>().color = new Color(value, value, value, 1);
    }

    #endregion

    IEnumerator DisableThisObject(GameObject disableThis, float delay)
    {
        //disables main or options menu after animating off
        yield return new WaitForSecondsRealtime(delay);
        disableThis.SetActive(false);
    }

    public void SelectThisGameobject(GameObject select)
    {
        //this is used for hovering over non-button text to select their respective sliders
        EventSystem.current.SetSelectedGameObject(select);
    }


    void UpdateFadeAlpha(float alphaChange)
    {
        blackFade.color =  new Color(0, 0, 0, alphaChange);
    }

    IEnumerator DisableObject(GameObject disableThis, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        disableThis.SetActive(false);
    }

    void ShowWinScreen()
    {
        didPlayerBeatGame = true;

        LeanTween.cancelAll();
        victoryText.transform.localScale = Vector3.zero;
        winScreen.GetComponent<CanvasGroup>().alpha = 0;
        
        winScreen.SetActive(true);

        LeanTween.value(0, 1, 0.75f).setOnUpdate(UpdateWinScreenAlpha).setDelay(0.5f).setEaseInOutSine().setIgnoreTimeScale(true);
        LeanTween.scale(victoryText, Vector3.one, 1f).setEaseOutBack().setIgnoreTimeScale(true).setDelay(0.75f);


        player.controllers.maps.SetMapsEnabled(false, "Default");
        player.controllers.maps.SetMapsEnabled(true, "Menu Category");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(winScreenFirstButton);
    }

    void UpdateWinScreenAlpha(float alphaChange)
    {
        winScreen.GetComponent<CanvasGroup>().alpha = alphaChange;
    }

    void UpdateGameOverScreenAlpha(float alphaChange)
    {
        gameOverScreen.GetComponent<CanvasGroup>().alpha = alphaChange;
    }

    void ShowGameOverScreen()
    {
        LeanTween.cancelAll();
        gameOverText.transform.localPosition = new Vector3(9.21f, 390, 0);
        gameOverScreen.GetComponent<CanvasGroup>().alpha = 0;

        gameOverScreen.SetActive(true);

        LeanTween.value(0, 1, 0.75f).setOnUpdate(UpdateGameOverScreenAlpha).setDelay(0.2f).setEaseInOutSine().setIgnoreTimeScale(true);
        LeanTween.moveLocal(gameOverText, new Vector3(9.21f, 90, 0f), 1f).setEaseOutBounce().setIgnoreTimeScale(true).setDelay(0.4f);

        player.controllers.maps.SetMapsEnabled(false, "Default");
        player.controllers.maps.SetMapsEnabled(true, "Menu Category");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameOverScreenFirstButton);
    }

    private void OnDestroy()
    {
        GameEvents.instance.playerBeatGame -= ShowWinScreen;
        GameEvents.instance.playerLostGame -= ShowGameOverScreen;
    }
}
