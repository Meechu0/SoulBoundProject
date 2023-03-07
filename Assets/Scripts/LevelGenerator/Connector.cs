using UnityEngine;

public class Connector : MonoBehaviour
{
    public enum Direction
    {
        Left,
        Right
    };

    public Direction connectorDir = Direction.Left;
    
    public bool hasConnection = false;
}
