using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBehavior : MonoBehaviour
{
    #region PlayerBaseStats
    private int playerBaseHealth = 6;
    private int playerBaseBlanks = 0;
    private int playerBaseMoney = 0;
    private float playerBaseSpeed = 7.5f;
    private float playerBaseMaxAcceleration = 1000f;
    private float playerBaseDamage = 0.8f;
    private float playerBaseFireCooldown = 0.35f; //less is faster
    private float playerBaseProjectileSpeed = 12f;
    private float playerBaseProjectileRange = 0.75f;
    private float playerBlankRadius = 6f;
    #endregion

    #region Dynamic variables
    private int maxPlayerHealth;
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

    private string currentRoomTypeMusic;

    #endregion

    #region Public Get Stats
    public int pub_maxPlayerHealth
    {
        get { return maxPlayerHealth; }
        private set
        {
            maxPlayerHealth = value;
            Debug.Log(pub_maxPlayerHealth);
            uiManager.UpdateHealthText();
        }
    }
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
            uiManager.DrawHearts();
        }
    }
    public float pub_playerBlankRadius
    {
        get { return playerBlankRadius;}
    }
    public float pub_projectileSpeed {
        get { return playerBaseProjectileSpeed; }
    }
    public float pub_playerDamage {
        get { return playerBaseDamage; }
        private set {
            playerBaseDamage = value;
            Debug.Log(pub_playerDamage);
        }
    }
    public float pub_playerSpeed
    {
        get { return playerBaseSpeed; }
        private set {
            playerBaseSpeed = value;
            Debug.Log(pub_playerSpeed);
        }
    }
    public float pub_playerFireCooldown
    {
        get { return playerBaseFireCooldown; }
        private set
        {
            playerBaseFireCooldown = value;
            Debug.Log(pub_playerFireCooldown);
        }
    }

    public float pub_playerProjectileRange
    {
        get { return playerBaseProjectileRange; }
    }

    public int pub_currentPlayerMoney
    {
        get { return currentPlayerMoney; }
        private set
        {
            if (value < currentPlayerMoney)
            {
                Debug.Log("Spending money");
                audioSource.PlayOneShot(spendMoneySound);
            }
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
    [SerializeField] private AudioClip[] musicTracks;
    [SerializeField] private AudioClip spendMoneySound;
    [SerializeField] private AudioClip negativeSound;

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
        currentRoomTypeMusic = "Hostile";

        defaultColor = gameObject.GetComponent<Renderer>().material.color;
        blankRadiusMesh.SetActive(false);

        //methods can only be subscribed or removed using += or -=
        GameEvents.instance.upgradePlayerStats += TakeUpgradeItem;
        GameEvents.instance.useDispenser += UseDispenser;


        //if integrating save data, load all stat/inventory stuff here then return to exit the Start method
        maxPlayerHealth = playerBaseHealth;
        currentPlayerHealth = playerBaseHealth;
        currentPlayerMoney = playerBaseMoney;
        currentPlayerBlanks = playerBaseBlanks;


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

            desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * pub_playerSpeed;
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
        if (collision.gameObject.CompareTag("Enemy") && !doesPlayerHaveIFrames && this.gameObject.activeInHierarchy)
        {
            TakeDamageFromEnemy(collision);
        }
        //the following is for testing
        else if (collision.gameObject.name == "Debug hurt player" && !doesPlayerHaveIFrames && this.gameObject.activeInHierarchy)
        {
            audioSource.PlayOneShot(playerHurtSounds[Random.Range(0, playerHurtSounds.Length)]);
            PlayerTakeDamage(1);
        }
    }
    private void TakeDamageFromEnemy(Collision collision) //onCollisionStay - enemy contact damage
    {
        audioSource.PlayOneShot(playerHurtSounds[Random.Range(0, playerHurtSounds.Length)]);
        if (collision.gameObject.GetComponent<EnemyBase>() != null)
        {
            int dmg = collision.gameObject.GetComponent<EnemyBase>().pub_enemyDamage;
            PlayerTakeDamage(dmg);
        }
        else if (collision.gameObject.GetComponent<BossBehavior>() != null)
        {
            int dmg = collision.gameObject.GetComponent<BossBehavior>().pub_contactDamage;
            PlayerTakeDamage(dmg);
        }
        
    }

    private void TakeDamageFromEnemy(Collider other) //onTriggerEnter - enemy projectile damage
    {
        audioSource.PlayOneShot(playerHurtSounds[Random.Range(0, playerHurtSounds.Length)]);
        int dmg = other.gameObject.GetComponent<ProjectileEnemy>().pub_enemyProjectileDamage;
        //Debug.Log(dmg);
        PlayerTakeDamage(dmg);
        
    }

    private void TakeUpgradeItem(string itemTag, int price, int health, float damage, float speed, float firerate) //GameEvent
    {
        switch (itemTag)
        {
            case "health":
                Debug.Log(itemTag + " upgraded from " + pub_maxPlayerHealth + " to");
                pub_maxPlayerHealth += health;
                pub_currentPlayerHealth = pub_maxPlayerHealth;
                pub_currentPlayerMoney -= price;
                break;
            case "damage":
                Debug.Log(itemTag + " upgraded from " + pub_playerDamage + " to");
                pub_playerDamage += damage;
                pub_currentPlayerMoney -= price;
                break;
            case "speed":
                Debug.Log(itemTag + " upgraded from " + pub_playerSpeed + " to");
                pub_playerSpeed += speed;
                pub_currentPlayerMoney -= price;
                break;
            case "firerate":
                Debug.Log(itemTag + " upgraded from " + pub_playerFireCooldown + " to");
                pub_playerFireCooldown += firerate;
                pub_currentPlayerMoney -= price;
                break;
            default:
                Debug.LogWarning("Item type not defined");
                break;
        }
    }

    private void UseDispenser(int cost)
    {
        pub_currentPlayerMoney -= cost;
    }

    private void OnCollisionEnter(Collision collision)
    {
        TouchPickup(collision);
        NotEnoughMoneySounds(collision);
    }

    private void NotEnoughMoneySounds(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dispenser")
            && collision.gameObject.GetComponent<DispenserBehavior>().pub_cost > currentPlayerMoney)
        {
            audioSource.PlayOneShot(negativeSound);
        }
        else if (collision.gameObject.CompareTag("UpgradeItem")
            && collision.gameObject.GetComponent<UpgradeItemBehavior>().pub_price > currentPlayerMoney)
        {
            audioSource.PlayOneShot(negativeSound);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Room"))
        {
            PlayerEntersRoom(other);
        }

        if (other.gameObject.CompareTag("EnemyProjectile") && !doesPlayerHaveIFrames && this.gameObject.activeInHierarchy)
        {
            TakeDamageFromEnemy(other);
        }
    }

    private void PlayerEntersRoom(Collider other)
    {
        canPlayerMove = false;

        pub_currentRoomIndex++;
        playerRB.velocity = Vector3.zero;
        playerRB.angularVelocity = Vector3.zero;

        Invoke("AllowPlayerToMove", 0.65f);

        if (other.gameObject.GetComponent<RoomBehavior>() != null) //regular hostile room
        {
            if (currentRoomTypeMusic != "Hostile")
            {
                currentRoomTypeMusic = "Hostile";
                MusicManager.instance.SwapTrack(musicTracks[0]);
            } //change music
            transform.position = other.gameObject.GetComponent<RoomBehavior>().pub_playerStartPos;
        }
        else if ((other.gameObject.GetComponent<RoomWithItemsBehavior>() != null)) //item room
        {
            if (currentRoomTypeMusic != "Friendly")
            {
                currentRoomTypeMusic = "Friendly";
                MusicManager.instance.SwapTrack(musicTracks[1]);
            } //change music
            transform.position = other.gameObject.GetComponent<RoomWithItemsBehavior>().pub_playerStartPos;
        }
        else if (other.gameObject.GetComponent<RoomShopBehavior>() != null) //shop
        {
            if (currentRoomTypeMusic != "Friendly")
            {
                currentRoomTypeMusic = "Friendly";
                MusicManager.instance.SwapTrack(musicTracks[1]);
            } //change music
            transform.position = other.gameObject.GetComponent<RoomShopBehavior>().pub_playerStartPos;
        }
        else if (other.gameObject.GetComponent<RoomFinalBossBehavior>() != null) //boss room
        {
            currentRoomTypeMusic = "Boss";
            MusicManager.instance.SwapTrack(musicTracks[2]);
            transform.position = other.gameObject.GetComponent<RoomFinalBossBehavior>().pub_playerStartPos;
        }
    }

    private void AllowPlayerToMove()
    {
        canPlayerMove = true;
    }

    private void TouchPickup(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pickup")
            && collision.gameObject.GetComponent<PickupBehavior>().pub_pickupType != "HalfHeart")
        {
            Destroy(collision.gameObject);
            pub_currentPlayerMoney += collision.gameObject.GetComponent<PickupBehavior>().pub_moneyValue;
            pub_currentPlayerBlanks += collision.gameObject.GetComponent<PickupBehavior>().pub_blankValue;
            
        }
        else if (collision.gameObject.CompareTag("Pickup")
            && collision.gameObject.GetComponent<PickupBehavior>().pub_pickupType == "HalfHeart"
            && currentPlayerHealth < maxPlayerHealth)
        {
            //Debug.LogWarning("need to prevent player from pciking up too many hearts and over-healing");
            Destroy(collision.gameObject);
            pub_currentPlayerHealth += collision.gameObject.GetComponent<PickupBehavior>().pub_healthValue;
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
            canPlayerFire = Time.time + playerBaseFireCooldown;
            int rand = Random.Range(0, projectilePool.Count);
            //projectilePool[rand].GetComponent<ProjectileBase>().ShootProjectile(shootDirection);
            projectilePool[rand].GetComponent<ProjectileBase>().ShootProjectile(transform.position, shootDirection, pub_projectileSpeed, pub_playerProjectileRange);
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
        globalOnDestroySounds.PlayPlayerDeathSound();
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
                if (col.gameObject.GetComponent<EnemyBase>() != null) //if has enemy script
                {
                    col.gameObject.GetComponent<EnemyBase>().BlankKnockback();
                }
                else if (col.gameObject.GetComponent<ProjectileEnemy>() != null)
                {
                    col.gameObject.GetComponent<ProjectileEnemy>().DisableProjectile();
                }
            }
        }
    }

    private void ActivateBlankAnimation()
    {
        blankRadiusMesh.transform.SetParent(null);
        //blankRadiusMesh.GetComponent<MeshRenderer>().material.color = Color.white;
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


    private void OnDestroy()
    {
        GameEvents.instance.upgradePlayerStats -= TakeUpgradeItem;
        GameEvents.instance.useDispenser -= UseDispenser;
    }
}
