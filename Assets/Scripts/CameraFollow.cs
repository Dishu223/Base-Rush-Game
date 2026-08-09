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

    private float bossPhaseElapsed = 0f;

    public void StartCinematicBossView()
    {
        inBossPhase = true;
        bossPhaseElapsed = 0f;
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
            bossPhaseElapsed += Time.deltaTime;
            Boss boss = FindObjectOfType<Boss>();
            
            if (boss != null)
            {
                // Pan slowly to the right only!
                float panX = bossPhaseElapsed * 1.5f; 
                
                // Add a very subtle up/down cinematic sway
                float swayY = Mathf.Sin(bossPhaseElapsed * 0.8f) * 1f;

                Vector3 cinematicOffset = new Vector3(8 + panX, 10 + swayY, -8);
                
                Vector3 desiredPosition = target.position + cinematicOffset;
                finalPosition = Vector3.Lerp(transform.position, desiredPosition, 0.8f * Time.deltaTime);
                
                // Smoothly look directly at the Boss
                Vector3 direction = boss.transform.position - transform.position;
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 1.5f * Time.deltaTime);
            }
        }
        else
        {
            // Add subtle camera sway to make the run feel more dynamic!
            float swayX = Mathf.Sin(Time.time * 1f) * 0.05f;
            float swayY = Mathf.Cos(Time.time * 0.8f) * 0.05f;

            // We only want the camera to follow the Z axis (forward), not the X axis (left/right swiping)
            Vector3 normalPosition = new Vector3(transform.position.x, target.position.y + offset.y, target.position.z + offset.z);
            normalPosition += new Vector3(swayX, swayY, 0);

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
