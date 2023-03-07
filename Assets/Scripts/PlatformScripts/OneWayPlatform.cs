using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private Collider2D thisCollider;

    private void Awake()
    {
        thisCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Physics2D.IgnoreCollision(col, thisCollider, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Physics2D.IgnoreCollision(other, thisCollider, false);
    }
}
