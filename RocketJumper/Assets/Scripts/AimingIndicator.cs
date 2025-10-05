using UnityEngine;

public class AimingIndicator : MonoBehaviour
{
    public float distanceFromPlayer = 1f; // Distance of the indicator from the player

    void Update()
    {
        // Update the aim direction
        SetAimDirection(AimDirectionArrow());
    }

    // Calculate the angle of the mouse relative to the player
    private float AimDirectionArrow()
    {
        Vector2 direction = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.parent.position;
        return Mathf.Atan2(direction.y, direction.x); // Return the angle in radians
    }

    // Set the position and rotation of the aiming indicator
    private void SetAimDirection(float angle)
    {
        // Position the indicator at a fixed distance in front of the player
        float fixedDistance = distanceFromPlayer; // Use the distanceFromPlayer value for positioning
        transform.localPosition = new Vector3(
            fixedDistance * Mathf.Cos(angle),
            fixedDistance * Mathf.Sin(angle),
            0f
        );

        // Rotate the indicator to face the mouse direction
        transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg); // Apply rotation directly
    }
}