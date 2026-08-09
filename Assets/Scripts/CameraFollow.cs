using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 10f;
    
    private bool inBossPhase = false;
    
    // Screen Shake variables
    private float currentShakeDuration = 0f;
    private float currentShakeMagnitude = 0f;

    void Start()
    {
        // If offset isn't set in the editor, calculate it based on current position
        if (offset == Vector3.zero && target != null)
        {
            offset = transform.position - target.position;
        }
    }

    public void StartCinematicBossView()
    {
        inBossPhase = true;
    }

    public void TriggerShake(float duration, float magnitude)
    {
        currentShakeDuration = duration;
        currentShakeMagnitude = magnitude;
    }

    void LateUpdate()
    {
        if (target == null) return;
        
        Vector3 finalPosition = transform.position;

        if (inBossPhase)
        {
            Boss boss = FindObjectOfType<Boss>();
            if (boss != null)
            {
                // Move camera up and to the side for a wide cinematic shot, slower transition!
                Vector3 desiredPosition = target.position + new Vector3(12, 10, -8);
                finalPosition = Vector3.Lerp(transform.position, desiredPosition, 0.8f * Time.deltaTime);
                
                // Smoothly look directly at the Boss
                Vector3 direction = boss.transform.position - transform.position;
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 1.5f * Time.deltaTime);
            }
        }
        else
        {
            // We only want the camera to follow the Z axis (forward), not the X axis (left/right swiping)
            // This keeps the runway centered on the screen!
            Vector3 normalPosition = new Vector3(transform.position.x, target.position.y + offset.y, target.position.z + offset.z);
            finalPosition = Vector3.Lerp(transform.position, normalPosition, smoothSpeed * Time.deltaTime);
        }

        // Apply Screen Shake if active
        if (currentShakeDuration > 0)
        {
            finalPosition += Random.insideUnitSphere * currentShakeMagnitude;
            currentShakeDuration -= Time.deltaTime;
        }

        transform.position = finalPosition;
    }
}
