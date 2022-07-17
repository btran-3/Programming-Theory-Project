using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    #region PlayerBaseStats
    private int playerBaseHealth = 10;
    private float playerBaseSpeed = 10f;
    private float playerBaseMaxAcceleration = 1000f;
    private float playerBaseDamage = 1f;
    private float playerBaseFireRate = 0.30f;
    private float playerBaseProjectileSpeed = 12f;
    private float playerBaseRange;
    #endregion

    #region Dynamic variables

    private int currentPlayerHealth;
    private bool doesPlayerHaveIFrames;
    private float canPlayerFire;

    Vector3 velocity, desiredVelocity;

    #endregion

    #region Public Get Stats
    public int pub_currentPlayerHealth
    {
        get { return currentPlayerHealth; }
        set
        {
            currentPlayerHealth = value;
            uiManager.UpdateHealthText();
        }
    }
    public float pub_projectileSpeed {
        get { return playerBaseProjectileSpeed; }
            //need to add with stat upgrades
    }
    public float pub_playerDamage {
        get { return playerBaseDamage; }
        //add with stat upgrades
    }
    #endregion


    public List<GameObject> projectilePool;
    [SerializeField] UIManager uiManager;



    Rigidbody playerRB; //https://catlikecoding.com/unity/tutorials/movement/physics/
    private Color defaultColor;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        defaultColor = gameObject.GetComponent<Renderer>().material.color;
        currentPlayerHealth = playerBaseHealth;
    }


    void Update()
    {
        GetPlayerInput();
        FireProjectiles();

    }

    private void GetPlayerInput()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * playerBaseSpeed;
    }

    private void FixedUpdate()
    {
        MovePlayerRigidbody();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !doesPlayerHaveIFrames)
        {
            PlayerTakeDamage(1);
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
        if (Input.GetAxis("Fire1") < 0)
        {
            //left
            ShootThisDirection(Vector3.left);
        }
        else if (Input.GetAxis("Fire1") > 0)
        {
            //right
            ShootThisDirection(Vector3.right);
        }
        else if (Input.GetAxis("Fire2") < 0)
        {
            //down
            ShootThisDirection(Vector3.back);
        }
        else if (Input.GetAxis("Fire2") > 0)
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
        doesPlayerHaveIFrames = true;
        gameObject.GetComponent<Renderer>().material.color = Color.red;
        pub_currentPlayerHealth -= damage;

        if (currentPlayerHealth <= 0)
        {
            this.gameObject.SetActive(false);
            Debug.Log("GAME OVER");
            return;
        }

        Invoke("PlayerRecoverFromDamage", 1f);
    }

    private void PlayerRecoverFromDamage()
    {
        if (doesPlayerHaveIFrames)
        {
            doesPlayerHaveIFrames = false;
            gameObject.GetComponent<Renderer>().material.color = defaultColor;
        }
    }

    void UseBlank()
    {
        
    }

}
