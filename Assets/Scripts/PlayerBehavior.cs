using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    #region PlayerBaseStats
    private float playerBaseSpeed = 10f;
    private float playerBaseMaxSpeed = 10f;
    private float playerBaseMaxAcceleration = 1000f;
    private float playerBaseDamage;
    private float playerBaseFireRate;
    private float playerBaseProjectileSpeed;
    private float playerBaseRange;
    #endregion

    Vector3 velocity, desiredVelocity;
    int intOne, intTwo, intThree;

    Rigidbody playerRB;
    //https://catlikecoding.com/unity/tutorials/movement/physics/


    void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
    }


    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * playerBaseSpeed;
    }

    private void FixedUpdate()
    {

        velocity = playerRB.velocity;
        float maxSpeedChange = playerBaseMaxAcceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        playerRB.velocity = velocity;



    }

    void MovePlayer(Vector3 playerMovement)
    {
        playerRB.AddForce(playerMovement, ForceMode.Impulse);
    }

    void FireProjectiles()
    {

    }

    void UseBlank()
    {

    }

}
