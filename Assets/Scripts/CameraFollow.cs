using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 10f;

    void Start()
    {
        // If offset isn't set in the editor, calculate it based on current position
        if (offset == Vector3.zero && target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // We only want the camera to follow the Z axis (forward), not the X axis (left/right swiping)
        // This keeps the runway centered on the screen!
        Vector3 desiredPosition = new Vector3(transform.position.x, target.position.y + offset.y, target.position.z + offset.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
