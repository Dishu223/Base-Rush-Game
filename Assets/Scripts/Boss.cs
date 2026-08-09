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
            // Spawn fireworks closer to the camera so they aren't hidden inside the giant boss!
            VFXManager.instance.SpawnFireworks(transform.position - new Vector3(0, 0, 2.5f));
            
            // Spawn Lightning coming OUT of the boss when it takes damage!
            VFXManager.instance.SpawnLightning(transform.position);
        }
        
        // Small screen shake on every hit
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null) cam.TriggerShake(0.1f, 0.3f);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (VFXManager.instance != null) 
        {
            // Spawn a massive fireworks explosion around the edges!
            VFXManager.instance.SpawnFireworks(transform.position);
            VFXManager.instance.SpawnFireworks(transform.position + new Vector3(0, 2f, -2f));
            VFXManager.instance.SpawnFireworks(transform.position + new Vector3(2f, 0, -2f));
            VFXManager.instance.SpawnFireworks(transform.position + new Vector3(-2f, 0, -2f));
        }
        
        // Massive screen shake when the boss dies!
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null) cam.TriggerShake(0.8f, 1.0f);
        
        if (GameManager.instance != null)
        {
            GameManager.instance.BossDefeated();
        }
        
        Destroy(gameObject);
    }
}
