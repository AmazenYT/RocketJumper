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
    public ParticleSystem chargeEffect; // Particle effect for charging
    public float armParticleMinRate = 0f;
    public float armParticleMaxRate = 50f;
    public AudioSource jumpSound; // Sound triggered on jump
    public Animator animator;
    public string paramIsGrounded = "isGrounded";
    public string paramIsCharging = "isCharging";
    public string paramSpeed = "Speed";
    public string paramJumpTrigger = "Jump";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Assign Rigidbody2D component
    }

    void Update()
{
    // Start charging
    if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && isGrounded)
    {
        chargeTime = 0f;
        isCharging = true;
        CreateAimArrow();

    if (chargeEffect != null)
            {
                var em = chargeEffect.emission;
                em.enabled = true;
                chargeEffect.Play();
            }
    }

    // Charging
    if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0)) && isCharging && isGrounded)
    {
        chargeTime += Time.deltaTime;
        chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
        UpdateAimArrow();

        if (chargeEffect != null)
            {
                float t = (maxChargeTime > 0f) ? (chargeTime / maxChargeTime) : 0f;
                float rate = Mathf.Lerp(armParticleMinRate, armParticleMaxRate, t);
                var em = chargeEffect.emission;
                em.rateOverTime = new ParticleSystem.MinMaxCurve(rate);
            }
    }

    // Release jump
    if ((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0)) && isCharging && isGrounded)
    {
        Jump();
        isCharging = false;
        if (jumpSound != null) jumpSound.Play();
        DestroyAimArrow();

        if (chargeEffect != null)
            {
                chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                var em = chargeEffect.emission;
                em.rateOverTime = new ParticleSystem.MinMaxCurve(armParticleMinRate);
            }
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

        // Reset velocity before applying the jump force (use linearVelocity)
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);
        }

        if (animator != null)
        {
            animator.SetTrigger(paramJumpTrigger);
            animator.SetBool(paramIsGrounded, false);
        }

        if (direction.x < 0) // Jumping to the left
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x > 0) // Jumping to the right
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Detect when the player lands on the ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Player is on the ground
        }
        if (animator != null)
            animator.SetBool(paramIsGrounded, true);
    }

    // Detect when the player leaves the ground
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // Player is not on the ground
        }
        if (animator != null)
            animator.SetBool(paramIsGrounded, false);
    }

    // Create the aiming arrow (as a child so it stays with the player)
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
            currentAimArrow = Instantiate(aimArrowPrefab, transform.position + Vector3.right * distanceFromPlayer, Quaternion.identity);
            currentAimArrow.transform.SetParent(transform); // Attach to the player (keep as child)

            // Ensure the indicator script is present (it will be used to position/rotate/scale)
            var ai = currentAimArrow.GetComponent<AimingIndicator>();
            if (ai == null)
            {
                Debug.LogWarning("Aim arrow prefab is missing AimingIndicator script. Adding one automatically.");
                currentAimArrow.AddComponent<AimingIndicator>();
            }
        }
    }

    // Update the aiming arrow's position and rotation by sending direction & scale to the child indicator
    void UpdateAimArrow()
    {
        if (currentAimArrow == null) return;

        Vector2 predictedLandingPosition = PredictLandingPosition(); // Get the predicted landing position
        Vector2 direction = (predictedLandingPosition - (Vector2)transform.position).normalized; // Direction toward the landing position

        // scale for charge indication
        float chargePercent = chargeTime / maxChargeTime;
        float scale = Mathf.Lerp(0.2f, 0.5f, chargePercent);

        // Send the direction + distance + scale to the child's AimingIndicator script
        var ai = currentAimArrow.GetComponent<AimingIndicator>();
        if (ai != null)
        {
            ai.SetFromWorldDirection(direction, distanceFromPlayer, scale);
        }
        else
        {
            // fallback: if indicator missing, position directly (rare)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float rad = angle * Mathf.Deg2Rad;
            currentAimArrow.transform.localPosition = new Vector3(distanceFromPlayer * Mathf.Cos(rad),
                                                                  distanceFromPlayer * Mathf.Sin(rad),
                                                                  0f);
            currentAimArrow.transform.localEulerAngles = new Vector3(0f, 0f, angle);
            currentAimArrow.transform.localScale = new Vector3(scale, scale, 1f);
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

        if (chargeEffect != null)
        {
            chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            var em = chargeEffect.emission;
            em.rateOverTime = new ParticleSystem.MinMaxCurve(armParticleMinRate);
        }
    }
}
