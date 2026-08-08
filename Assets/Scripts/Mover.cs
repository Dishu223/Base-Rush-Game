using UnityEngine;

public class Mover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float distance = 3f; // How far to move left/right
    public float speed = 2f; // How fast to move

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // PingPong creates a back-and-forth value between 0 and 'length'
        // We use time * speed to drive it, and shift it so it sweeps from -distance to +distance
        float offset = Mathf.PingPong(Time.time * speed, distance * 2) - distance;
        
        transform.position = startPos + new Vector3(offset, 0, 0);
    }
}
