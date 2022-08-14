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
    [SerializeField] private Button menuButton;

    [SerializeField] GameObject mainMenuFirstButton, optionsFirstButton, optionsClosedButton;
    [Space(10)]
    [SerializeField] GameObject heartPrefab;
    [SerializeField] GameObject healthBarUI;
    List<UIHealthHeart> hearts = new List<UIHealthHeart>();

    [Space (10)]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI blanksText;

    [Space (10)] //Rewired stuff
    private int playerId = 0;
    private Player player;

    private bool isGamePaused;

    public bool pub_isGamePaused
    {
        get { return isGamePaused; }
        set { isGamePaused = value; }
    }


    void Start()
    {
        player = ReInput.players.GetPlayer(playerId);

        DrawHearts();

        healthText.SetText("Health: " + playerBehavior.pub_currentPlayerHealth + "/" +
            playerBehavior.pub_maxPlayerHealth);
        moneyText.SetText(playerBehavior.pub_currentPlayerMoney.ToString());
        blanksText.SetText(playerBehavior.pub_currentPlayerBlanks.ToString());

        pauseScreen.gameObject.SetActive(false);

        /*
        blackFade.gameObject.SetActive(true);
        LeanTween.value(1, 0, 1f).setOnUpdate(UpdateFadeAlpha);
        StartCoroutine(DisableObject(blackFade.gameObject, 1f));
        */
    }

    // Update is called once per frame
    void Update()
    {
        PauseUnpause();
    }

    private void PauseUnpause()
    {
        if (player.GetButtonDown("Pause") && !pub_isGamePaused)
        {
            pub_isGamePaused = true;
            pauseScreen.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (player.GetButtonDown("Pause") && pub_isGamePaused)
        {
            pub_isGamePaused = false;
            pauseScreen.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(healthBarUI.transform); //parent to hearts bar (its transform)

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

    public void UpdateHealthText()
    {
        healthText.SetText("Health: " + playerBehavior.pub_currentPlayerHealth + "/" +
            playerBehavior.pub_maxPlayerHealth);
    }

    public void UpdateMoneyText()
    {
        moneyText.SetText(playerBehavior.pub_currentPlayerMoney.ToString());
    }

    public void UpdateBlanksText()
    {
        blanksText.SetText(playerBehavior.pub_currentPlayerBlanks.ToString());
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

}
