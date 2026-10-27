using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public float speed = 5f; // áŠQ•¨‚ÌˆÚ“®‘¬“x
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime); // ¶•ûŒü‚ÉˆÚ“®
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject); // ‰æ–ÊŠO‚Éo‚½‚çáŠQ•¨‚ğ”jŠü
    }
}
