using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager instance;

    [Header("Prefabs")]
    public GameObject hitParticlePrefab;
    public GameObject floatingTextPrefab;

    [Header("Special FX")]
    public GameObject lightningPrefab;
    public GameObject fireworksPrefab;
    public GameObject smokePrefab;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SpawnSmoke(Vector3 position)
    {
        if (smokePrefab != null)
        {
            Instantiate(smokePrefab, position, Quaternion.identity);
        }
    }

    public void SpawnLightning(Vector3 position)
    {
        if (lightningPrefab != null)
        {
            Instantiate(lightningPrefab, position, Quaternion.identity);
        }
    }

    public void SpawnFireworks(Vector3 position)
    {
        if (fireworksPrefab != null)
        {
            Instantiate(fireworksPrefab, position, Quaternion.identity);
        }
    }

    public void SpawnHitParticle(Vector3 position)
    {
        if (hitParticlePrefab != null)
        {
            GameObject particle = Instantiate(hitParticlePrefab, position, Quaternion.identity);
            // Particle system should be set to "Stop Action -> Destroy" in Unity editor,
            // or we can force destroy it here just in case:
            Destroy(particle, 1.5f);
        }
    }

    public void SpawnFloatingText(Vector3 position, string text, Color color, float scale = 1f)
    {
        if (floatingTextPrefab != null)
        {
            // Spawn it slightly above the hit location
            GameObject floatText = Instantiate(floatingTextPrefab, position + Vector3.up, Quaternion.identity);
            
            FloatingText ftScript = floatText.GetComponent<FloatingText>();
            if (ftScript != null)
            {
                ftScript.Setup(text, color, scale);
            }
        }
    }
}
