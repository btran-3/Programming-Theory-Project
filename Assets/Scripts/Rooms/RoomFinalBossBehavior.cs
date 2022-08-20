using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class RoomFinalBossBehavior : MonoBehaviour
{
    [SerializeField] GameObject bossEnemy;
    private BossBehavior bossBehavior;
    [Space(10)]
    [SerializeField] GameObject activeEnemiesParent;

    [Space(10)]
    [SerializeField] AudioClip winJingle;
    [SerializeField] AudioClip hostileMusic;

    [Space(10)]
    private Vector3 doorDefaultScale;
    //[SerializeField] GameObject doorTop;
    [SerializeField] GameObject doorBottom;
    [Space(10)]
    [SerializeField] GameObject playerStartPos;
    [Space(10)]

    private Collider colliderA;

    private bool didPlayerWin;

    private int playerId = 0;
    private Player player;

    public Vector3 pub_playerStartPos
    {
        get { return playerStartPos.transform.position; }
    }

    private void Start()
    {
        player = ReInput.players.GetPlayer(playerId);

        doorBottom.SetActive(false);
        colliderA = GetComponent<Collider>();
        playerStartPos.GetComponent<Renderer>().enabled = false;

        doorDefaultScale = doorBottom.transform.localScale;

        StartDoorAsOpen(doorBottom);

        if (bossEnemy.GetComponent<BossBehavior>() != null)
        {
            bossBehavior = bossEnemy.GetComponent<BossBehavior>();
        }
        else
        {
            Debug.LogWarning("No bossBehavior script found on boss");
        }
    }

    void Update()
    {
        if (bossBehavior.pub_isBossDead && activeEnemiesParent.transform.childCount <= 0 && !didPlayerWin)
        {
            PlayerWon();
        }
    }

    private void PlayerWon()
    {
        didPlayerWin = true;
        Debug.Log("You won!!!!!");

        GameEvents.instance.PlayerBeatGameActions();

        StartCoroutine(ChangeRewiredInputStatus("Default", true, 0.5f));
        LeanTween.value(1, 0, 0.5f).setDelay(0.25f).setEaseInOutSine().setOnUpdate(SlowGameUponWinning);

        MusicManager.instance.track01.loop = false;
        MusicManager.instance.track02.loop = false;
        MusicManager.instance.SwapTrack(winJingle);
        StartCoroutine(MusicManager.instance.DelaySwapTrack(hostileMusic, 3f));
    }

    private void SlowGameUponWinning(float value)
    {
        Time.timeScale = value;
    }

    IEnumerator ChangeRewiredInputStatus(string categoryName, bool state, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        player.controllers.maps.SetMapsEnabled(state, categoryName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            colliderA.enabled = false;

            bossEnemy.GetComponent<BossBehavior>().StartBossRoom();
            AnimateDoorClosed(doorBottom);
        }
    }


    void StartDoorAsOpen(GameObject door)
    {
        door.transform.position += (Vector3.left * 2);
        door.transform.localScale -= (Vector3.one * 0.01f);
    }

    private void AnimateDoorClosed(GameObject door)
    {
        door.SetActive(true);
        LeanTween.move(door, door.transform.position + (Vector3.right * 2), 0.5f)
            .setEase(LeanTweenType.easeInOutCubic);
        LeanTween.scale(door, doorDefaultScale, 0.5f)
            .setEase(LeanTweenType.easeInOutCubic);
    }

}
