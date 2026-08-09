using UnityEngine;
using TMPro;

public class Boss : MonoBehaviour
{
    [Header("Boss Settings")]
    public int health = 50;
    public TextMeshPro healthText;
    
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

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateHealthText();
        
        if (VFXManager.instance != null) 
        {
            VFXManager.instance.SpawnHitParticle(transform.position);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (VFXManager.instance != null) 
        {
            // Spawn a few extra particles for a big explosion
            VFXManager.instance.SpawnHitParticle(transform.position);
            VFXManager.instance.SpawnHitParticle(transform.position + Vector3.up);
            VFXManager.instance.SpawnHitParticle(transform.position + Vector3.right);
        }
        
        if (GameManager.instance != null)
        {
            GameManager.instance.BossDefeated();
        }
        
        Destroy(gameObject);
    }
}
