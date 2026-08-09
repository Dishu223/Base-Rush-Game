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
            VFXManager.instance.SpawnFireworks(transform.position);
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
            // Spawn a massive fireworks explosion!
            VFXManager.instance.SpawnFireworks(transform.position);
            VFXManager.instance.SpawnFireworks(transform.position + Vector3.up * 2);
            VFXManager.instance.SpawnFireworks(transform.position + Vector3.right * 2);
            VFXManager.instance.SpawnFireworks(transform.position + Vector3.left * 2);
        }
        
        if (GameManager.instance != null)
        {
            GameManager.instance.BossDefeated();
        }
        
        Destroy(gameObject);
    }
}
