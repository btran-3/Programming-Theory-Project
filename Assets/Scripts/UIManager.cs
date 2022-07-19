using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be initialized before setting the UI
[DefaultExecutionOrder(1000)]
public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject playerGO;
    [SerializeField] PlayerBehavior playerBehavior;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI blanksText;

    void Start()
    {
        healthText.SetText("Health: " + playerBehavior.pub_currentPlayerHealth);
        moneyText.SetText("$" + playerBehavior.pub_currentPlayerMoney);
        blanksText.SetText("B: " + playerBehavior.pub_currentPlayerBlanks);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealthText()
    {
        healthText.SetText("Health: " + playerBehavior.pub_currentPlayerHealth);
    }

    public void UpdateMoneyText()
    {
        moneyText.SetText("$" + playerBehavior.pub_currentPlayerMoney);
    }

    public void UpdateBlanksText()
    {
        blanksText.SetText("B: " + playerBehavior.pub_currentPlayerBlanks);
    }
}
