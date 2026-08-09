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
            
            // Spawn Lightning randomly on the ENTIRE FRONT face of the boss (Since scale is 4, radius is 2)
            Vector3 lightningPos = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), -2.5f);
            VFXManager.instance.SpawnLightning(lightningPos);
        }
        
        // Subtle screen shake on every hit so it doesn't hurt the eyes!
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null) cam.TriggerShake(0.05f, 0.08f);

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
        
        // Moderate screen shake when the boss dies!
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null) cam.TriggerShake(0.4f, 0.4f);
        
        if (GameManager.instance != null)
        {
            GameManager.instance.BossDefeated();
        }
        
        Destroy(gameObject);
    }
}
