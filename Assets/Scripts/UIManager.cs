using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject playerGO;
    [SerializeField] PlayerBehavior playerBehavior;
    [SerializeField] private TextMeshProUGUI healthText;

    void Start()
    {
        healthText.SetText("Health: " + playerBehavior.pub_currentPlayerHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealthText()
    {
        healthText.SetText("Health: " + playerBehavior.pub_currentPlayerHealth);
    }
}
