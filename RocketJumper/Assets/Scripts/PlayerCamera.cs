using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;
    public float zoomDistance;
    public float followSpeed = 1f;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, player.position.y, transform.position.z), Time.fixedDeltaTime * followSpeed);
    }
}