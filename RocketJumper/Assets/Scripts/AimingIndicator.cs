using UnityEngine;

public class AimingIndicator : MonoBehaviour
{
    [Tooltip("Distance from player (local) where the arrow should sit.")]
    public float distanceFromPlayer = 1f;
    

    // This method is called by PlayerMovement every frame the arrow should update.
    // worldDirection: a normalized world-space direction vector pointing toward the target/landing.
    // localDistance: how far from the player's transform the arrow should appear (in local space).
    // scaleFactor: the uniform scale to apply to the arrow (child local scale).
    public void SetFromWorldDirection(Vector2 worldDirection, float localDistance, float scaleFactor)
    {
        // convert direction to angle in radians
        float angle = Mathf.Atan2(worldDirection.y, worldDirection.x); // radians

        // If parent is flipped horizontally (localScale.x < 0), mirror the angle across Y axis:
        // angle -> PI - angle (same as 180deg - angle)
        if (transform.parent != null && transform.parent.localScale.x < 0f)
        {
            angle = Mathf.PI - angle;
        }

        // Set local position relative to parent using the computed angle
        float x = localDistance * Mathf.Cos(angle);
        float y = localDistance * Mathf.Sin(angle);
        transform.localPosition = new Vector3(x, y, 0f);

        // Set local rotation
        float degrees = angle * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0f, 0f, degrees);

        // Set local scale (uniform)
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    // If nothing calls SetFromWorldDirection (e.g., manual use), fall back to mouse-based behavior:
    void Update()
    {
        // Only run fallback if not called by PlayerMovement (useful for testing the prefab in scene)
        // If PlayerMovement is controlling the arrow, it will call SetFromWorldDirection each frame.
        if (transform.parent == null) return; // no parent, nothing sensible to do
        // We avoid constantly overriding when PlayerMovement is active — it will overwrite anyway.
    }
}