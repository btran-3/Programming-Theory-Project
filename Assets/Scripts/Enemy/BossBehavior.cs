using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossBehavior : MonoBehaviour
{

    [SerializeField] Collider colliderA;
    [SerializeField] TextMeshPro currentStateText;

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
        EndState();
        BeginState(newState);
    }

    void BeginState(State newState) //acts like Start()
    {
        currentStateText.SetText(newState.ToString());
        switch (newState)
        {
            case State.BEGIN:
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

    void EndState() //acts like OnDestroy()
    {
        switch (_state) //looks at the current state
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