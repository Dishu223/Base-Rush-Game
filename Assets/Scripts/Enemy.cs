using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int health = 1; // Default 1 for normal enemies. 10+ for Brutes/Walls
    public TextMeshPro healthText; // Optional: Assign a 3D Text to show health
    
    void Start()
    {
        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = health.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Unit"))
        {
            // Find the main leader's crowd manager
            CrowdManager crowd = null;
            
            if (other.CompareTag("Player")) crowd = other.GetComponent<CrowdManager>();
            else crowd = FindObjectOfType<CrowdManager>();

            if (crowd != null && crowd.units.Count > 0)
            {
                crowd.RemoveUnits(1); // The player unit dies
                
                if (VFXManager.instance != null) VFXManager.instance.SpawnHitParticle(other.transform.position);

                // The enemy takes 1 damage
                health--;
                UpdateHealthText();

                if (health <= 0)
                {
                    if (VFXManager.instance != null) VFXManager.instance.SpawnHitParticle(transform.position);
                    Destroy(gameObject); // Enemy dies
                }
            }
        }
    }
}
