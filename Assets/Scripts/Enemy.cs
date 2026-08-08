using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if an enemy collides with the main player leader OR a follower unit
        if (other.CompareTag("Player") || other.CompareTag("Unit"))
        {
            // If it hits the main leader, the leader acts as the "manager".
            // We tell the manager to remove 1 unit.
            if (other.CompareTag("Player"))
            {
                CrowdManager crowd = other.GetComponent<CrowdManager>();
                if (crowd != null && crowd.units.Count > 0)
                {
                    crowd.RemoveUnits(1);
                    if (VFXManager.instance != null) VFXManager.instance.SpawnHitParticle(transform.position);
                    Destroy(gameObject); 
                }
            }
            // If it hits a follower unit directly, we just destroy the enemy
            // AND we remove 1 unit from the crowd.
            else if (other.CompareTag("Unit"))
            {
                // Find the main leader's crowd manager
                CrowdManager crowd = FindObjectOfType<CrowdManager>();
                if (crowd != null)
                {
                    crowd.RemoveUnits(1);
                    if (VFXManager.instance != null) VFXManager.instance.SpawnHitParticle(transform.position);
                    Destroy(gameObject);
                }
            }
        }
    }
}
