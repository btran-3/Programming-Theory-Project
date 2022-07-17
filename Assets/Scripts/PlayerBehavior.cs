using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    #region PlayerBaseStats
    private float playerBaseSpeed = 10f;
    private float playerBaseMaxAcceleration = 1000f;
    private float playerBaseDamage = 1f;
    private float playerBaseFireRate;
    private float playerBaseProjectileSpeed = 15f;
    private float playerBaseRange;
    #endregion

    #region Public Get Stats
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



    Vector3 velocity, desiredVelocity;

    Rigidbody playerRB;
    //https://catlikecoding.com/unity/tutorials/movement/physics/


    void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
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

    private void MovePlayerRigidbody()
    {
        velocity = playerRB.velocity;
        float maxSpeedChange = playerBaseMaxAcceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        playerRB.velocity = velocity;
    }

    void FireProjectiles()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (projectilePool.Count > 0)
            {
                int rand = Random.Range(0, projectilePool.Count);
                projectilePool[rand].GetComponent<ProjectileBehavior>().ShootProjectile();
            }
        }
    }

    void UseBlank()
    {

    }

}
