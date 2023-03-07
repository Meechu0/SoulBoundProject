using UnityEngine;

public class MousePointer : MonoBehaviour
{
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        DontDestroyOnLoad(gameObject);
    }
}
