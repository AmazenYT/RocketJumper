using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb; // Rigidbody 2d component of the character
    public float minJumpForce = 5f; // the weakest jump possible, current placeholder number, will likely be adjusted when level design proper begins to work better with it
    public float maxJumpForce = 15f; // the strongest jump possible, current placeholder number, will likely be adjusted when level design proper begins to work better with it
    public float chargeTime = 0f; // the ability to charge the jump, jump can be done with an immediate press, if they don't charge it they get the default speed
    public float maxChargeTime = 3f; // the amount of time it will take to charge the jump, current placeholder of 3 seconds but could raise to 5 depending on how it plays
    private bool isCharging = false; // this will be what detects if the player is charging the jump or not to trigger the charge times 
    private bool isGrounded = false; // this is to detect if the player is currently standing on the ground or not, this is to prevent the player from being able to infinitely spam jumps in the air

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Assign Rigidbody2D component
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) // this is for the minimum jump speed by not charging the jump
        {
            chargeTime = 0f;
            isCharging = true;
        }

        if (Input.GetKey(KeyCode.Space) && isCharging && isGrounded) // this is to detect how long the jump will be charged for and to clamp it aka locking it to the max time it can be charged for
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
        }

        if (Input.GetKeyUp(KeyCode.Space) && isCharging && isGrounded) // this is to trigger the jump when the space key is released
        {
            Jump();
            isCharging = false;
        }
    }

    void Jump()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        // this is so the jump will track the mouse position and launch the player where the mouse is

        float chargePercent = chargeTime / maxChargeTime;
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePercent);
        // this will figure out the strength of the jump based on how long it's been charged compared to the max charge time and the jump forces attached to the time

        rb.linearVelocity = Vector2.zero; // Reset velocity before applying the jump force
        rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision) // Fixed typo in method name
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // this is to check if the player is standing on collision that is marked as the ground so they can activate a jump
        }
    }

    void OnCollisionExit2D(Collision2D collision) // Fixed typo in method name
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // this is to check if the player is not standing on collision marked as ground that they cannot activate a jump
        }
    }
}