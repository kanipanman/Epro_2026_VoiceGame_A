using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float jumpforce = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame) //«—ˆ“I‚É‰¹º”F¯‚É’u‚«Š·‚¦‚é
        {
            rb.linearVelocity = new Vector2(0, jumpforce);
        }
    }
}
