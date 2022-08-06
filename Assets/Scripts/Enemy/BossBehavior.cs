using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossBehavior : MonoBehaviour
{

    [SerializeField] Collider colliderA;
    [SerializeField] TextMeshPro currentStateText;

    private float xMoveRange = 6f;
    private float roamSpeed = 2f;

    enum State { BEGIN, ROAMING, FOLLOW, ZOOM, SPAWNENEMY, DEATH }
    State _state; //our current state

    void Start()
    {
        
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
    
    void Update() //Update for each state
    {
        switch (_state) //looks at current state
        {
            case State.BEGIN:
                break;
            case State.ROAMING:

                float offset = 0f;
                float posDamp = 1f; //higher is tighter
                float targetX = Mathf.Sin(Time.time * roamSpeed + offset) * xMoveRange;
                Vector3 targetPos = new Vector3(targetX, 0, 3.75f);

                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * posDamp);
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

    IEnumerator DelayStateSwitch(State newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchState(newState);
    }

}