using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float zoomDistance; // Unused in this implementation, but can be used for zooming
    public float followSpeed = 1f; // Speed at which the camera follows the player
    public float panSpeed = 2f; // Speed at which the camera pans
    public float panDistance = 10f; // Maximum distance the camera can pan

    private Vector3 panOffset = Vector3.zero; // Offset for panning

    void FixedUpdate()
    {
        // Handle camera panning based on arrow key input
        HandleCameraPanning();

        // Follow the player with the panning offset applied
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y + 1.5f, transform.position.z) + panOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * followSpeed);
    }

    void HandleCameraPanning()
    {
        // Reset pan offset
        panOffset = Vector3.zero;

        // Check for arrow key input and adjust the pan offset
        if (Input.GetKey(KeyCode.UpArrow))
        {
            panOffset += new Vector3(0, panDistance, 0); // Pan upward
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            panOffset += new Vector3(-panDistance, 0, 0); // Pan left
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            panOffset += new Vector3(panDistance, 0, 0); // Pan right
        }

        // Smoothly interpolate the pan offset for smoother movement
        panOffset = Vector3.Lerp(panOffset, panOffset, Time.deltaTime * panSpeed);
    }
}