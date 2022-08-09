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
    private float timerForEvents;
    private float playerXPos;

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
        timerForEvents = 0;
        switch (newState)
        {
            case State.BEGIN: //stay still at beginning
                StartCoroutine(DelayStateSwitch(State.ROAMING, 0.75f));
                break;
            case State.ROAMING:
                roamTightness = 0; //reset tightness
                LeanTween.value(0f, maxRoamTightness, 2f).setEaseInOutSine().setOnUpdate(IncreaseRoamTightness); //gradually catch up to sine movement target
                break;
            case State.FOLLOW:
                break;
            case State.ZOOM:
                playerXPos = playerBehavior.transform.position.x;
                LeanTween.moveX(gameObject, playerXPos, 1f).setEaseOutCubic();

                int rand = Random.Range(0, 5); //randomly decide which state to go to next
                switch (rand)
                {
                    case 0:
                    case 1:
                        Debug.Log("Back to roaming state");
                        StartCoroutine(DelayStateSwitch(State.ROAMING, 1f));
                        break;
                    case 2:
                        Debug.Log("Insert spawn enemy state here");
                        StartCoroutine(DelayStateSwitch(State.ROAMING, 1f));
                        break;
                    case 3:
                    case 4:
                        Debug.Log("Insert follow state here");
                        StartCoroutine(DelayStateSwitch(State.FOLLOW, 1f));
                        break;
                }

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
                float sinTargetX = Mathf.Sin(Time.time * roamSpeed) * xMoveRange;
                Vector3 targetPos = new Vector3(sinTargetX, 0, 3.75f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * roamTightness);

                timerForEvents += Time.deltaTime;
                float bottomBoundZPos = transform.position.z - colliderA.bounds.extents.z;
                if (timerForEvents >= 3f && playerBehavior.transform.position.z >= bottomBoundZPos)
                {
                    SwitchState(State.ZOOM);
                }
                else if (timerForEvents >= 6f)
                {
                    SwitchState(State.FOLLOW);
                }
                break;
            case State.FOLLOW:
                //moved to LateUpdate
                /*
                float playerTargetX = playerBehavior.transform.position.x;
                Vector3 playerTargetPos = new Vector3(playerTargetX, 0, 3.75f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, playerTargetPos, Time.deltaTime * 2);

                //switch to roaming after random time
                timerForEvents += Time.deltaTime;
                float rand = Random.Range(4, 9);
                if (timerForEvents >= rand)
                {
                    SwitchState(State.ROAMING);
                }
                */
                break;
            case State.ZOOM:
                break;
            case State.SPAWNENEMY:
                break;
            case State.DEATH:
                break;
        }
        //Debug.Log(timerForEvents);
    }

    private void LateUpdate()
    {
        switch (_state) //looks at current state
        {
            case State.BEGIN:
                break;
            case State.ROAMING:
                break;
            case State.FOLLOW:
                float playerTargetX = playerBehavior.transform.position.x;
                Vector3 playerTargetPos = new Vector3(playerTargetX, 0, 3.75f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, playerTargetPos, Time.deltaTime * 2);

                //switch to roaming after random time
                timerForEvents += Time.deltaTime;
                float rand = Random.Range(4, 9);
                if (timerForEvents >= rand)
                {
                    SwitchState(State.ROAMING);
                }
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

    void IncreaseRoamTightness(float value)
    {
        roamTightness = value;
        //Debug.Log(value);
    }

}