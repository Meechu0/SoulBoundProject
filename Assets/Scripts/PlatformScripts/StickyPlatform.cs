using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        col.transform.parent = transform;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        col.transform.parent = null;
    }
}
