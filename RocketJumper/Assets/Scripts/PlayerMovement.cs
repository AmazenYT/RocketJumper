using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb; // Rigidbody 2D component of the character
    public float minJumpForce = 5f; // The weakest jump possible
    public float maxJumpForce = 15f; // The strongest jump possible
    public float chargeTime = 0f; // The ability to charge the jump
    public float maxChargeTime = 3f; // The amount of time it will take to charge the jump
    private bool isCharging = false; // Whether the player is charging the jump
    private bool isGrounded = false; // Whether the player is standing on the ground
    public GameObject aimArrowPrefab; // Reference to the aiming arrow prefab
    private GameObject currentAimArrow; // Reference to the currently active aiming arrow
    public float distanceFromPlayer = 1f; // Distance to spawn the aiming indicator in front of the player
    
    public AudioSource jumpSound; //Sound triggered on jump

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Assign Rigidbody2D component
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) // Start charging the jump
        {
            chargeTime = 0f;
            isCharging = true;

            // Create the aiming indicator
            CreateAimArrow();
        }

        if (Input.GetKey(KeyCode.Space) && isCharging && isGrounded) // Charge the jump
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);

            // Update the aiming indicator's position and rotation
            UpdateAimArrow();
        }

        if (Input.GetKeyUp(KeyCode.Space) && isCharging && isGrounded) // Release the jump
        {
            Jump();
            isCharging = false;
            jumpSound.Play(); // Play jump sound

            // Destroy the aiming indicator
            DestroyAimArrow();

        }
    }

    // Method to calculate the direction of the mouse relative to the player
    Vector2 mouseDirection()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get mouse position in world space as Vector2
        Vector2 playerPos = transform.position; // Get player position as Vector2
        Vector2 direction = (mousePos - playerPos).normalized; // Normalize to get only the direction
        return direction;
    }

    // Method to handle the jump logic
    void Jump()
    {
        Vector2 direction = mouseDirection(); // Use the mouseDirection method to get the direction

        float chargePercent = chargeTime / maxChargeTime;
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePercent);
        // Calculate the jump force based on charge time

        rb.linearVelocity = Vector2.zero; // Reset velocity before applying the jump force
        rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);
    }

    // Detect when the player lands on the ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Player is on the ground
        }
    }

    // Detect when the player leaves the ground
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // Player is not on the ground
        }
    }

    // Create the aiming arrow
    void CreateAimArrow()
    {
        // Destroy the current indicator if it exists
        if (currentAimArrow != null)
        {
            Destroy(currentAimArrow);
        }

        // Create a new aiming indicator
        if (aimArrowPrefab != null)
        {
            currentAimArrow = Instantiate(aimArrowPrefab, transform.position, Quaternion.identity);
            currentAimArrow.transform.SetParent(transform); // Attach to the player
        }
    }

    // Update the aiming arrow's position and rotation
    void UpdateAimArrow()
    {
        if (currentAimArrow != null)
        {
            Vector2 predictedLandingPosition = PredictLandingPosition(); // Get the predicted landing position
            Vector2 direction = (predictedLandingPosition - (Vector2)transform.position).normalized; // Direction toward the landing position
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Calculate the angle in degrees

            // Position the indicator at the predicted landing position
            currentAimArrow.transform.position = predictedLandingPosition;

            // Rotate the indicator to face the predicted landing position
            currentAimArrow.transform.eulerAngles = new Vector3(0, 0, angle);

            float chargePercent = chargeTime / maxChargeTime;
            float scale = Mathf.Lerp(0.2f, 0.5f, chargePercent);
            currentAimArrow.transform.localScale = new Vector3(scale, scale, 1f); 
            //this is to make the arrow grow in size as the jump is charged
        }
    }

    // Predict the landing position based on jump force and trajectory
    Vector2 PredictLandingPosition()
    {
        float chargePercent = chargeTime / maxChargeTime;
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePercent);

        Vector2 direction = mouseDirection(); // Get the direction toward the mouse
        float gravity = Physics2D.gravity.y * rb.gravityScale; // Get gravity from Rigidbody2D
        float timeToApex = jumpForce / -gravity; // Time to reach the highest point
        float totalFlightTime = timeToApex * 2; // Total flight time (up and down)

        // Predict the landing position based on physics
        Vector2 predictedLandingPosition = (Vector2)transform.position + direction * jumpForce * totalFlightTime;
        return predictedLandingPosition;
    }

    // Destroy the aiming arrow
    void DestroyAimArrow()
    {
        if (currentAimArrow != null)
        {
            Destroy(currentAimArrow);
        }
    }
}