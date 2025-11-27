using UnityEngine;

public class BackgroundScaling : MonoBehaviour
{
    
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        Vector3 size = sr.sprite.bounds.size;
        transform.localScale = new Vector3(
            worldScreenWidth / size.x,
            worldScreenHeight / size.y,
            1f
        );
    }
}
