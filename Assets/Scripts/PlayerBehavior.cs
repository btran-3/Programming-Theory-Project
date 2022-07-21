using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBehavior : MonoBehaviour
{
    #region PlayerBaseStats
    private int playerBaseHealth = 5;
    private int playerBaseBlanks = 10;
    private int playerBaseMoney;
    private float playerBaseSpeed = 10f;
    private float playerBaseMaxAcceleration = 1000f;
    private float playerBaseDamage = 1f;
    private float playerBaseFireRate = 0.30f;
    private float playerBaseProjectileSpeed = 12f;
    private float playerBaseProjectileRange = 1f;
    private float playerBlankRadius = 5f;
    #endregion

    #region Dynamic variables

    private int currentPlayerHealth;
    private int currentPlayerMoney;
    private int currentPlayerBlanks;
    private bool doesPlayerHaveIFrames;
    private float canPlayerFire;
    private bool canPlayerUseBlanks = true;

    //private bool didPlayerEnterRoom;
    private bool canPlayerMove = true;

    Vector3 velocity, desiredVelocity;
    private int currentRoomIndex = 0;

    #endregion

    #region Public Get Stats
    public int pub_currentPlayerHealth
    {
        get { return currentPlayerHealth; }
        private set
        {
            currentPlayerHealth = value;
            if (currentPlayerHealth <0)
            {
                currentPlayerHealth = 0;
            }
            uiManager.UpdateHealthText();
        }
    }
    public int pub_currentPlayerMoney
    {
        get { return currentPlayerMoney; }
        private set
        {
            currentPlayerMoney = value;
            uiManager.UpdateMoneyText();
        }
    }
    public int pub_currentPlayerBlanks
    {
        get { return currentPlayerBlanks; }
        private set
        { 
            currentPlayerBlanks = value;
            uiManager.UpdateBlanksText();
        }
    }

    public float pub_playerBlankRadius
    {
        get { return playerBlankRadius;}
    }

    public float pub_projectileSpeed {
        get { return playerBaseProjectileSpeed; }
            //need to add with stat upgrades
    }
    public float pub_playerDamage {
        get { return playerBaseDamage; }
        //add with stat upgrades
    }

    public float pub_playerProjectileRange
    {
        get { return playerBaseProjectileRange; }
    }

    public int pub_currentRoomIndex
    {
        get { return currentRoomIndex; }
        private set
        {
            currentRoomIndex = value;
            cameraCinemachine.ActivateNextCamera(currentRoomIndex);
        }
    }


    #endregion

    #region References
    public List<GameObject> projectilePool;
    [SerializeField] private AudioClip[] playerHurtSounds;

    [SerializeField] UIManager uiManager;
    [SerializeField] GameObject blankRadiusMesh;
    [SerializeField] GlobalOnDestroySounds globalOnDestroySounds;
    [SerializeField] CameraCinemachine cameraCinemachine;

    Rigidbody playerRB; //https://catlikecoding.com/unity/tutorials/movement/physics/
    AudioSource audioSource;
    private Color defaultColor;
    #endregion

    void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        defaultColor = gameObject.GetComponent<Renderer>().material.color;
        currentPlayerHealth = playerBaseHealth;
        currentPlayerMoney = playerBaseMoney;
        currentPlayerBlanks = playerBaseBlanks;

        blankRadiusMesh.SetActive(false);
    }


    void Update()
    {
        GetPlayerInput();
        FireProjectiles();
        UseBlank();
    }

    private void GetPlayerInput()
    {
        if (canPlayerMove)
        {
            Vector2 playerInput;
            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            playerInput = Vector2.ClampMagnitude(playerInput, 1f);

            desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * playerBaseSpeed;
        }
        else
        {
            desiredVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        MovePlayerRigidbody();
    }

    private void OnCollisionStay(Collision collision)
    {
        TakeDamageFromEnemy(collision);
    }

    private void TakeDamageFromEnemy(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !doesPlayerHaveIFrames && this.gameObject.activeInHierarchy)
        {
            audioSource.PlayOneShot(playerHurtSounds[Random.Range(0, playerHurtSounds.Length)]);
            //OLD int dmg = collision.gameObject.GetComponent<EnemyBehaviorA>().pub_dealDamage;
            int dmg = collision.gameObject.GetComponent<EnemyBase>().pub_enemyDamage;
            PlayerTakeDamage(dmg);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        TouchPickup(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHitsRoomEnterTrigger(other);
    }

    private void PlayerHitsRoomEnterTrigger(Collider other)
    {
        if (other.gameObject.CompareTag("Room"))
        {
            canPlayerMove = false;

            pub_currentRoomIndex++;
            playerRB.velocity = Vector3.zero;
            playerRB.angularVelocity = Vector3.zero;
            transform.position = other.gameObject.GetComponent<RoomBehavior>().pub_playerStartPos;
            Invoke("AllowPlayerToMove", 0.65f);
        }
    }

    private void AllowPlayerToMove()
    {
        canPlayerMove = true;
    }

    private void TouchPickup(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pickup"))
        {
            Destroy(collision.gameObject);
            pub_currentPlayerMoney += collision.gameObject.GetComponent<PickupBehavior>().pub_moneyValue;
            pub_currentPlayerBlanks += collision.gameObject.GetComponent<PickupBehavior>().pub_blankValue;
        }
    }

    private void MovePlayerRigidbody()
    {
        velocity = playerRB.velocity;
        float maxSpeedChange = playerBaseMaxAcceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        playerRB.velocity = velocity;
    }

    private void FireProjectiles()
    {
        if (Input.GetAxis("Fire1") < 0 && canPlayerMove)
        {
            //left
            ShootThisDirection(Vector3.left);
        }
        else if (Input.GetAxis("Fire1") > 0 && canPlayerMove)
        {
            //right
            ShootThisDirection(Vector3.right);
        }
        else if (Input.GetAxis("Fire2") < 0 && canPlayerMove)
        {
            //down
            ShootThisDirection(Vector3.back);
        }
        else if (Input.GetAxis("Fire2") > 0 && canPlayerMove)
        {
            //up
            ShootThisDirection(Vector3.forward);
        }
    }

    private void ShootThisDirection(Vector3 shootDirection)
    {
        if (projectilePool.Count > 0 && Time.time > canPlayerFire)
        {
            canPlayerFire = Time.time + playerBaseFireRate;
            int rand = Random.Range(0, projectilePool.Count);
            projectilePool[rand].GetComponent<ProjectileBehavior>().ShootProjectile(shootDirection);
        }
    }

    private void PlayerTakeDamage(int damage)
    {
        pub_currentPlayerHealth -= damage;
        doesPlayerHaveIFrames = true; //give the player i-frames

        if (currentPlayerHealth <= 0)
        {
            PlayerDeath();
            return;
        }

        //gameObject.GetComponent<Renderer>().material.color = Color.red;
        LeanTween.color(this.gameObject, Color.red, 0f);
        LeanTween.color(this.gameObject, defaultColor, 0f).setDelay(0.15f);
        LeanTween.color(this.gameObject, Color.Lerp(Color.red, defaultColor, 0.35f), 0f).setDelay(0.3f);
        LeanTween.color(this.gameObject, defaultColor, 0f).setDelay(0.45f);
        LeanTween.color(this.gameObject, Color.Lerp(Color.red, defaultColor, 0.60f), 0f).setDelay(0.6f);
        LeanTween.color(this.gameObject, defaultColor, 0f).setDelay(0.75f);
        LeanTween.color(this.gameObject, Color.Lerp(Color.red, defaultColor, 0.85f), 0f).setDelay(0.9f);
        LeanTween.color(this.gameObject, defaultColor, 0f).setDelay(1.05f).setOnComplete(PlayerRecoverFromDamage);
        //Invoke("PlayerRecoverFromDamage", 1.05f);
    }

    private void PlayerDeath()
    {
        globalOnDestroySounds.playPlayerDeathSound();
        gameObject.SetActive(false);
        Debug.Log("GAME OVER");
    }

    private void PlayerRecoverFromDamage()
    {
        if (doesPlayerHaveIFrames)
        {
            doesPlayerHaveIFrames = false;
            //gameObject.GetComponent<Renderer>().material.color = defaultColor;
        }
    }

    private void UseBlank()
    {
        if (Input.GetKeyDown(KeyCode.Space) && pub_currentPlayerBlanks > 0 && canPlayerUseBlanks)
        {
            pub_currentPlayerBlanks--;
            canPlayerUseBlanks = false;

            ActivateBlankAnimation();

            Collider[] overlapSphere = Physics.OverlapSphere(gameObject.transform.position, playerBlankRadius);
            foreach (Collider col in overlapSphere)
            {
                if (col.gameObject.GetComponent<EnemyBehaviorA>() != null) //if has enemy script
                {
                    col.gameObject.GetComponent<EnemyBehaviorA>().BlankKnockback();
                }
            }
        }
    }

    private void ActivateBlankAnimation()
    {
        blankRadiusMesh.transform.SetParent(null);
        blankRadiusMesh.GetComponent<MeshRenderer>().material.color = Color.white;
        blankRadiusMesh.transform.position = transform.position;
        blankRadiusMesh.SetActive(true);
        LeanTween.alpha(blankRadiusMesh, 0.5f, 0f);
        LeanTween.alpha(blankRadiusMesh, 0f, 0.4f).setEase(LeanTweenType.easeOutCirc);
        LeanTween.scale(blankRadiusMesh, new Vector3(8, 8, 8), 0.4f).setEase(LeanTweenType.easeOutCirc).setOnComplete(DisableBlankRadiusMesh);
    }

    private void DisableBlankRadiusMesh()
    {
        blankRadiusMesh.SetActive(false);
        blankRadiusMesh.transform.SetParent(this.gameObject.transform, false);
        blankRadiusMesh.transform.localScale = Vector3.one * 0.9f;
        canPlayerUseBlanks = true;
    }

}
