using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossBehavior : MonoBehaviour
{

    [SerializeField] Collider colliderA;
    [SerializeField] TextMeshPro currentStateText;
    [SerializeField] Slider healthSlider;
    [Space (10)]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hitSound;
    [SerializeField] PlayerBehavior playerBehavior;
    [SerializeField] GlobalOnDestroySounds globalOnDestroySounds;
    [SerializeField] RoomFinalBossBehavior roomFinalBossBehavior;
    [SerializeField] GameObject activeEnemiesParent;
    [Space(10)]
    [SerializeField] GameObject enemyProjectile;
    [SerializeField] List<GameObject> enemyPool;
    [Space (10)]
    [SerializeField] GameObject bossFace;
    [SerializeField] Renderer bossCubeRenderer;
    [SerializeField] GameObject eyebrowLeft;
    [SerializeField] GameObject eyebrowRight;
    [SerializeField] AudioClip bossAngrySound;

    //Fixed variables
    private float xMoveRange = 6f;
    private float maxRoamTightness = 10f;
    private float maxBossHealth = 30f;
    private int contactDamage = 2;
    private float projectileRange = 1.5f;
    private Color startingFaceColor = new Color(193, 101, 32, 1) / 255;
    private Color angerFaceColor = new Color(164, 11, 14, 360) / 255;


    //CHANGE THESE VALUES AFTER MIDWAY POINT
    private float roamSpeed;
    private float minShootingCooldown, maxShootingCooldown;
    private float followShootingCooldown;
    private float projectileSpeed;
    int roamingProjectilesToShoot;
    private int minEnemyIndex, maxEnemyIndex;

    //Dynamic variables
    private float roamTightness;
    private float currentBossHealth;
    private float timerForSwitchStateEvents;
    private float randomShootingCooldown;
    private float currentShootingCooldown;
    private bool canBossTakeDamage;
    private bool isBossInSecondPhase;
    private bool isBossDead;

    private float playerXPos;

    //GET AND SET
    public int pub_contactDamage
    {
        get { return contactDamage; }
    }
    public float pub_projectileSpeed
    {
        get { return projectileSpeed; }
        private set { projectileSpeed = value; }
    }

    public float pub_currentBossHealth
    {
        get { return currentBossHealth; }
        private set { currentBossHealth = value;
            healthSlider.value = currentBossHealth;
            //Debug.Log(currentBossHealth);
            if (currentBossHealth <= (maxBossHealth/2) && !isBossInSecondPhase)
            {
                isBossInSecondPhase = true;
                SwitchState(State.NEWPHASE);
            }

            if (currentBossHealth <= 0)
            {
                SwitchState(State.DEATH);
            }
        
        }
    }
    
    public bool pub_isBossDead
    {
        get { return isBossDead; }
        private set { isBossDead = value; }
    }


    enum State { BEGIN, ROAMING, FOLLOW, ZOOM, SPAWNENEMY, NEWPHASE, KILLEDPLAYER, DEATH }
    State _state; //our current state


    void Start()
    {
        currentBossHealth = maxBossHealth;
        healthSlider.maxValue = maxBossHealth;
        healthSlider.value = maxBossHealth;
        bossCubeRenderer.GetComponent<Renderer>().material.color = startingFaceColor;
        canBossTakeDamage = true;

        //the following will all change in phase 2
        roamSpeed = 1f;
        minShootingCooldown = 1.25f;
        maxShootingCooldown = 2f;
        followShootingCooldown = 0.9f;
        pub_projectileSpeed = 7.5f;
        roamingProjectilesToShoot = 2;
        minEnemyIndex = 0;
        maxEnemyIndex = 2; //not inclusive, so must be 2. change to 2 and 4 for harder phase
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

        timerForSwitchStateEvents = 0;
        roamTightness = 0; //reset tightness
        currentShootingCooldown = 0; //reset shooting cooldown
        randomShootingCooldown = Random.Range(minShootingCooldown, maxShootingCooldown); //generate random initial shooting cooldown

        switch (newState)
        {
            case State.BEGIN: //stay still at beginning
                StartCoroutine(DelayStateSwitch(State.ROAMING, 0.75f));
                break;
            case State.ROAMING:
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
                        StartCoroutine(DelayStateSwitch(State.ROAMING, 1f));
                        break;
                    case 2:
                        Debug.Log("Insert spawn enemy state here");
                        StartCoroutine(DelayStateSwitch(State.SPAWNENEMY, 1f));
                        break;
                    case 3:
                    case 4:
                        StartCoroutine(DelayStateSwitch(State.FOLLOW, 1f));
                        break;
                }

                break;
            case State.SPAWNENEMY:
                int randInt = Random.Range(minEnemyIndex, maxEnemyIndex);
                GameObject spawnedEnemy = Instantiate((enemyPool[randInt]), (transform.position + new Vector3(0, 0, -1.5f)), Quaternion.identity);
                spawnedEnemy.SetActive(true);
                spawnedEnemy.GetComponent<EnemyBase>().EnableEnemy();
                spawnedEnemy.transform.SetParent(activeEnemiesParent.transform);

                StartCoroutine(DelayStateSwitch(State.ROAMING, 1.5f));
                break;
            case State.NEWPHASE:
                audioSource.PlayOneShot(bossAngrySound);

                canBossTakeDamage = false;
                LeanTween.cancel(this.gameObject);
                LeanTween.moveX(gameObject, 0f, 0.75f).setEaseInOutCubic();
                LeanTween.rotateAroundLocal(eyebrowLeft, Vector3.forward, -15, 0.75f).setEaseInOutSine().setDelay(0.5f);
                LeanTween.rotateAroundLocal(eyebrowRight, Vector3.forward, 15, 0.75f).setEaseInOutSine().setDelay(0.5f);
                LeanTween.color(bossCubeRenderer.gameObject, angerFaceColor, 0.75f).setEaseInOutSine().setDelay(0.5f);

                roamSpeed = 1.2f;
                minShootingCooldown = 1f;
                maxShootingCooldown = 1.75f;
                followShootingCooldown = 0.6f;
                pub_projectileSpeed = 9f;
                roamingProjectilesToShoot = 3;
                minEnemyIndex = 2;
                maxEnemyIndex = 4;

                StartCoroutine(DelayStateSwitch(State.ROAMING, 1.5f));
                break;
            case State.KILLEDPLAYER:
                break;
            case State.DEATH:
                LeanTween.scale(healthSlider.gameObject, Vector3.zero, 0.7f).setDelay(0.25f)
                    .setEaseInOutCubic().setOnComplete(SetHealthBarInactive).setIgnoreTimeScale(true);
                globalOnDestroySounds.PlayEnemyDeathSound("boss");
                pub_isBossDead = true;
                gameObject.SetActive(false);
                break;
        }
    }
    
    void Update() //Update for each state
    {
        if (!playerBehavior.gameObject.activeInHierarchy)
        {
            SwitchState(State.KILLEDPLAYER);
        }

        switch (_state) //looks at current state
        {
            case State.BEGIN:
                break;
            case State.ROAMING:
                //Roaming position
                float sinTargetX = Mathf.Sin(Time.time * roamSpeed) * xMoveRange;
                Vector3 targetPos = new Vector3(sinTargetX, 0, 3.75f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * roamTightness);

                //Shooting
                currentShootingCooldown += Time.deltaTime;
                if (currentShootingCooldown >= randomShootingCooldown)
                {
                    for (int i = 0; i < roamingProjectilesToShoot; i++)
                    {
                        ShootInRandomDirection();
                    }
                    currentShootingCooldown = 0;
                    randomShootingCooldown = Random.Range(minShootingCooldown, maxShootingCooldown);
                }

                //Switching state conditions
                timerForSwitchStateEvents += Time.deltaTime;
                float bottomBoundZPos = transform.position.z - colliderA.bounds.extents.z;
                if (timerForSwitchStateEvents >= 3f && playerBehavior.transform.position.z >= bottomBoundZPos)
                {
                    SwitchState(State.ZOOM);
                }
                else if (timerForSwitchStateEvents >= 6f)
                {
                    int rand = Random.Range(0, 2);
                    if (rand == 0)
                    {
                        SwitchState(State.FOLLOW);
                    }
                    else if (rand == 1)
                    {
                        SwitchState(State.SPAWNENEMY);
                    }
                }
                break;
            case State.FOLLOW:
                //Shooting
                currentShootingCooldown += Time.deltaTime;
                if (currentShootingCooldown >= followShootingCooldown)
                {
                    ShootAtPlayer();
                    currentShootingCooldown = 0;
                }
                break;
            case State.ZOOM:
                break;
            case State.SPAWNENEMY:
                break;
            case State.NEWPHASE:
                break;
            case State.KILLEDPLAYER:
                break;
            case State.DEATH:
                break;
        }
        //Debug.Log(timerForEvents);
    }

    private void ShootInRandomDirection()
    {
        Vector3 randomShootingDirection = new Vector3(Random.Range(-1.5f, 1.5f), 0, -1).normalized;
        GameObject newProjectile = Instantiate(enemyProjectile, transform.position, transform.rotation);
        newProjectile.SetActive(true);
        newProjectile.GetComponent<ProjectileEnemy>()
            .ShootProjectile(transform.position, randomShootingDirection, pub_projectileSpeed, projectileRange);
    }

    private void ShootAtPlayer()
    {
        Vector3 aimShootingDirection = (playerBehavior.transform.position - transform.position).normalized;
        GameObject newProjectile = Instantiate(enemyProjectile, transform.position, transform.rotation);
        newProjectile.SetActive(true);
        newProjectile.GetComponent<ProjectileEnemy>()
            .ShootProjectile(transform.position, aimShootingDirection, pub_projectileSpeed, projectileRange);
    }

    private void LateUpdate()
    {
        Vector3 lookAtPlayer = (playerBehavior.transform.position - transform.position).normalized;

        Vector3 faceLookAtPlayer = new Vector3(lookAtPlayer.x/2.5f, lookAtPlayer.y, lookAtPlayer.z/4);
        bossFace.transform.localPosition = new Vector3(0, 1, 0.1f) + faceLookAtPlayer;

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

                //switch to another state after random time
                timerForSwitchStateEvents += Time.deltaTime;
                float randTime = Random.Range(4, 9);
                int rand = Random.Range(0, 3);
                if (timerForSwitchStateEvents >= randTime)
                {
                    switch (rand)
                    {
                        case 0:
                            SwitchState(State.ROAMING);
                            break;
                        case 1:
                        case 2:
                            SwitchState(State.SPAWNENEMY);
                            break;
                        default:
                            break;
                    }
                }
                break;
            case State.ZOOM:
                break;
            case State.SPAWNENEMY:
                break;
            case State.NEWPHASE:
                break;
            case State.KILLEDPLAYER:
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
            case State.NEWPHASE:
                canBossTakeDamage = true; //allow boss to take damage again
                break;
            case State.KILLEDPLAYER:
                break;
            case State.DEATH:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile") && canBossTakeDamage)
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

    void SetHealthBarInactive()
    {
        healthSlider.gameObject.SetActive(false);
    }

}