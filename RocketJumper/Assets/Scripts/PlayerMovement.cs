using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement & Jump Settings")]
    public Rigidbody2D rb; 
    public float minJumpForce = 5f; 
    public float maxJumpForce = 15f; 
    public float chargeTime = 0f; 
    public float maxChargeTime = 3f; 
    private bool isCharging = false; 
    private bool isGrounded = false; 

    [Header("Aiming")]
    public GameObject aimArrowPrefab; 
    private GameObject currentAimArrow; 
    public float distanceFromPlayer = 1f; 

    [Header("Audio & Animation")]
    public AudioSource jumpSound; 
    public Animator animator;
    public string paramIsGrounded = "isGrounded";
    public string paramIsCharging = "isCharging";
    public string paramSpeed = "Speed";
    public string paramJumpTrigger = "Jump";

    [Header("Charge Glow")]
    public GameObject chargeGlow;           // Assign your glow sprite in the Inspector
    public Vector3 baseGlowScale = Vector3.one;
    public float maxGlowScale = 1.5f;
    public float maxGlowShake = 0.05f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (chargeGlow != null)
            chargeGlow.SetActive(false); // Hide glow initially
    }

    void Update()
    {
        // Start charging
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && isGrounded)
        {
            chargeTime = 0f;
            isCharging = true;
            CreateAimArrow();

            if (chargeGlow != null)
            {
                chargeGlow.SetActive(true);
                chargeGlow.transform.localScale = baseGlowScale;
            }

            if (animator != null)
                animator.SetBool(paramIsCharging, true);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                isCharging = false;
                DestroyAimArrow();
            }
        }

        // Charging
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0)) && isCharging && isGrounded)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
            UpdateAimArrow();

            if (chargeGlow != null)
            {
                float chargePercent = chargeTime / maxChargeTime;

                // Scale up
                float scale = Mathf.Lerp(baseGlowScale.x, maxGlowScale, chargePercent);
                chargeGlow.transform.localScale = new Vector3(scale, scale, 1f);

                // Shake
                float jitter = Mathf.Lerp(0f, maxGlowShake, chargePercent);
                chargeGlow.transform.localPosition = new Vector3(
                    UnityEngine.Random.Range(-jitter, jitter),
                    UnityEngine.Random.Range(-jitter, jitter),
                    chargeGlow.transform.localPosition.z
                );
            }
        }

        // Release jump
        if ((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0)) && isCharging && isGrounded)
        {
            Jump();
            isCharging = false;
            if (jumpSound != null) jumpSound.Play();
            DestroyAimArrow();

            if (chargeGlow != null)
            {
                chargeGlow.SetActive(false);
                chargeGlow.transform.localScale = baseGlowScale;
                chargeGlow.transform.localPosition = Vector3.zero;
            }

            if (animator != null)
                animator.SetBool(paramIsCharging, false);
        }
    }

    Vector2 mouseDirection()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = transform.position;
        return (mousePos - playerPos).normalized;
    }

    void Jump()
    {
        Vector2 direction = mouseDirection();
        float chargePercent = chargeTime / maxChargeTime;
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePercent);

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

        // Flip character
        if (direction.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (animator != null)
            animator.SetBool(paramIsGrounded, true);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;

        if (animator != null)
            animator.SetBool(paramIsGrounded, false);
    }

    void CreateAimArrow()
    {
        if (currentAimArrow != null) Destroy(currentAimArrow);

        if (aimArrowPrefab != null)
        {
            currentAimArrow = Instantiate(aimArrowPrefab, transform.position + Vector3.right * distanceFromPlayer, Quaternion.identity);
            currentAimArrow.transform.SetParent(transform);

            var ai = currentAimArrow.GetComponent<AimingIndicator>();
            if (ai == null) currentAimArrow.AddComponent<AimingIndicator>();
        }
    }

    void UpdateAimArrow()
    {
        if (currentAimArrow == null) return;

        Vector2 predictedLandingPosition = PredictLandingPosition();
        Vector2 direction = (predictedLandingPosition - (Vector2)transform.position).normalized;
        float chargePercent = chargeTime / maxChargeTime;
        float scale = Mathf.Lerp(0.2f, 0.5f, chargePercent);

        var ai = currentAimArrow.GetComponent<AimingIndicator>();
        if (ai != null)
            ai.SetFromWorldDirection(direction, distanceFromPlayer, scale);
        else
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float rad = angle * Mathf.Deg2Rad;
            currentAimArrow.transform.localPosition = new Vector3(distanceFromPlayer * Mathf.Cos(rad),
                                                                  distanceFromPlayer * Mathf.Sin(rad),
                                                                  0f);
            currentAimArrow.transform.localEulerAngles = new Vector3(0f, 0f, angle);
            currentAimArrow.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    Vector2 PredictLandingPosition()
    {
        float chargePercent = chargeTime / maxChargeTime;
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePercent);

        Vector2 direction = mouseDirection();
        float gravity = Physics2D.gravity.y * rb.gravityScale;
        float timeToApex = jumpForce / -gravity;
        float totalFlightTime = timeToApex * 2;

        return (Vector2)transform.position + direction * jumpForce * totalFlightTime;
    }

    void DestroyAimArrow()
    {
        if (currentAimArrow != null) Destroy(currentAimArrow);

        if (chargeGlow != null)
        {
            chargeGlow.SetActive(false);
            chargeGlow.transform.localScale = baseGlowScale;
            chargeGlow.transform.localPosition = Vector3.zero;
        }
    }
}
