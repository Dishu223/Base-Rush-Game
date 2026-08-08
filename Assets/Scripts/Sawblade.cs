using UnityEngine;

public class Sawblade : MonoBehaviour
{
    [Header("Sawblade Settings")]
    public float spinSpeed = 360f;

    void Update()
    {
        // Spin the sawblade visually
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If a unit hits a sawblade, the unit dies but the sawblade takes NO damage!
        if (other.CompareTag("Player") || other.CompareTag("Unit"))
        {
            CrowdManager crowd = null;
            
            if (other.CompareTag("Player")) crowd = other.GetComponent<CrowdManager>();
            else crowd = FindObjectOfType<CrowdManager>();

            if (crowd != null && crowd.units.Count > 0)
            {
                crowd.RemoveUnits(1);
                
                if (VFXManager.instance != null) VFXManager.instance.SpawnHitParticle(other.transform.position);
            }
        }
    }
}
