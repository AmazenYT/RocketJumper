using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform posA, posB;
    public float speed;
    private Vector3 targetPos;
    private bool movingToB; 

    private void Start()
    {
        
        targetPos = posA.position;
        movingToB = true; 
    }

    private void Update()
    {
      
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

       
        if (Vector2.Distance(transform.position, targetPos) < 0.05f)
        {
            if (movingToB)
            {
                targetPos = posB.position;
            }
            else
            {
                targetPos = posA.position;
            }

            movingToB = !movingToB;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           
            collision.transform.parent = this.transform;

       
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector3 platformOffset = transform.position - playerRb.transform.position;
                playerRb.linearVelocity = new Vector2(0, playerRb.linearVelocity.y); 
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
           
                Vector3 platformMovement = transform.position - playerRb.transform.position;

              
                playerRb.linearVelocity = new Vector2(platformMovement.x, playerRb.linearVelocity.y);

                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            collision.transform.parent = null;
        }
    }
}