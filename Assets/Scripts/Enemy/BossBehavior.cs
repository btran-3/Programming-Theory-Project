using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossBehavior : MonoBehaviour
{

    [SerializeField] Collider colliderA;
    [SerializeField] TextMeshPro currentStateText;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hitSound;
    [SerializeField] PlayerBehavior playerBehavior;

    //Fixed variables
    private float xMoveRange;
    private float maxRoamTightness;

    private float maxBossHealth = 10f;
    private int contactDamage = 2;


    //CHANGE THESE VALUES AFTER MIDWAY POINT
    private float roamSpeed;


    //Dynamic variables
    private float roamTightness;
    private float currentBossHealth;

    //GET AND SET
    public int pub_contactDamage
    {
        get { return contactDamage; }
    }

    public float pub_currentBossHealth
    {
        get { return currentBossHealth; }
        private set { currentBossHealth = value;
            Debug.Log(currentBossHealth);
            if (currentBossHealth <= 0)
            {
                SwitchState(State.DEATH);
            }
        
        }
    }


    enum State { BEGIN, ROAMING, FOLLOW, ZOOM, SPAWNENEMY, DEATH }
    State _state; //our current state


    void Start()
    {
        xMoveRange = 6f;
        roamSpeed = 1f;
        maxRoamTightness = 10f;
        currentBossHealth = maxBossHealth;
    }


    public void StartBossRoom()
    {
        SwitchState(State.BEGIN);
    }


    void SwitchState(State newState)
    {
        EndState(); //like OnDestroy, but for the state that's about to end
        _state = newState;
        BeginState(newState);
    }

    void BeginState(State newState) //acts like Start()
    {
        currentStateText.SetText(newState.ToString());
        switch (newState)
        {
            case State.BEGIN: //stay still at beginning
                StartCoroutine(DelayStateSwitch(State.ROAMING, 0.75f));
                break;
            case State.ROAMING:
                LeanTween.value(0f, maxRoamTightness, 2f).setEaseInOutSine().setOnUpdate(IncreateRoamTightness);
                break;
            case State.FOLLOW:
                break;
            case State.ZOOM:
                break;
            case State.SPAWNENEMY:
                break;
            case State.DEATH:
                gameObject.SetActive(false);
                break;
        }
    }
    
    void Update() //Update for each state
    {
        switch (_state) //looks at current state
        {
            case State.BEGIN:
                break;
            case State.ROAMING:
                float targetX = Mathf.Sin(Time.time * roamSpeed) * xMoveRange;
                Vector3 targetPos = new Vector3(targetX, 0, 3.75f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * roamTightness);
                break;
            case State.FOLLOW:
                break;
            case State.ZOOM:
                break;
            case State.SPAWNENEMY:
                break;
            case State.DEATH:
                break;
        }
    }

    void EndState() //acts like OnDestroy() for the current state
    {
        switch (_state)
        {
            case State.BEGIN:
                break;
            case State.ROAMING:
                break;
            case State.FOLLOW:
                break;
            case State.ZOOM:
                break;
            case State.SPAWNENEMY:
                break;
            case State.DEATH:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            //play hit sound before potential death update
            if (this.gameObject.activeInHierarchy)
            {
                audioSource.PlayOneShot(hitSound);
            }

            
            pub_currentBossHealth -= playerBehavior.pub_playerDamage;

            /*
            if (enemyRenderer != null)
            {
                LeanTween.cancel(gameObject);
                enemyRenderer.material.color = Color.red;
                LeanTween.color(this.gameObject, defaultColor, 0.25f).setDelay(0.05f)
                    .setEase(LeanTweenType.easeOutCubic);
            } */
        }
    }

    IEnumerator DelayStateSwitch(State newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchState(newState);
    }

    void IncreateRoamTightness(float value)
    {
        roamTightness = value;
        //Debug.Log(value);
    }

}