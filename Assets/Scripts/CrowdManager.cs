using UnityEngine;
using System.Collections.Generic;

public class CrowdManager : MonoBehaviour
{
    [Header("Crowd Settings")]
    public GameObject unitPrefab;
    public float spacing = 0.5f;

    [Header("Current Crowd")]
    public List<Transform> units = new List<Transform>();
    
    private Transform bossTarget;

    void Start()
    {
        int startAmount = 4;
        if (GameManager.instance != null) startAmount += GameManager.instance.startingArmyUpgrade;
        
        AddUnits(startAmount);
    }

    void Update()
    {
        if (bossTarget != null) return; // Skip normal formatting during boss fight

        // Call this every frame so the units lerp smoothly to their positions
        FormatCrowd();
    }

    public void ChargeBoss(Transform boss)
    {
        bossTarget = boss;
        StartCoroutine(BossFightSequence());
    }

    private int activeAttackers = 0;

    private System.Collections.IEnumerator BossFightSequence()
    {
        // Phase 1: Rise up into the air slowly!
        float riseDuration = 1.5f;
        float elapsed = 0f;
        
        Vector3[] startPositions = new Vector3[units.Count];
        Vector3[] targetPositions = new Vector3[units.Count];

        // Calculate center of the crowd for a single performance-friendly smoke puff
        Vector3 centerGround = Vector3.zero;
        int activeCount = 0;
        
        Renderer[] renderers = new Renderer[units.Count];
        Color[] originalColors = new Color[units.Count];
        
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] == null) continue;
            
            // Disable physics so they don't fall back to the ground!
            Rigidbody rb = units[i].GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
            
            // Store renderer and original color for fading
            renderers[i] = units[i].GetComponentInChildren<Renderer>();
            if (renderers[i] != null) originalColors[i] = renderers[i].material.color;

            startPositions[i] = units[i].position;
            // Rise up between 3 and 7 units high, slightly randomized
            targetPositions[i] = units[i].position + new Vector3(0, Random.Range(3f, 7f), 0);
            
            centerGround += startPositions[i];
            activeCount++;
        }

        if (activeCount > 0)
        {
            centerGround /= activeCount;
            centerGround.y = 0.5f; // Ground level
            if (VFXManager.instance != null) VFXManager.instance.SpawnSmoke(centerGround);
        }

        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / riseDuration;
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i] != null)
                {
                    // Use SmoothStep for a nice easing effect
                    units[i].position = Vector3.Lerp(startPositions[i], targetPositions[i], Mathf.SmoothStep(0, 1, t));
                    
                    // Fade color MUCH slower! (Only gets halfway there during rise)
                    if (renderers[i] != null)
                    {
                        renderers[i].material.color = Color.Lerp(originalColors[i], new Color(1f, 0.5f, 0f), t * 0.5f);
                    }
                }
            }
            yield return null;
        }

        // Start hovering animation for units suspended in air!
        Coroutine hoverCoroutine = StartCoroutine(ApplyHoverAndFade());

        // Suspend in the air for a dramatic pause (2 seconds!)
        yield return new WaitForSeconds(2.0f);

        // Phase 2: Shoot at the boss 1 by 1 with multiplied speed!
        Boss bossScript = bossTarget.GetComponent<Boss>();
        float waitTime = 0.2f; // Starts slow!
        activeAttackers = 0; // Reset just in case
        
        while (units.Count > 0)
        {
            // If the boss is destroyed, stop firing!
            if (bossTarget == null) break;

            Transform attacker = units[units.Count - 1];
            units.RemoveAt(units.Count - 1);
            
            if (attacker != null)
            {
                activeAttackers++; // Track how many units are currently flying through the air
                StartCoroutine(ShootUnitAtBoss(attacker, bossScript));
            }

            // Update UI
            if (UIManager.instance != null) UIManager.instance.UpdateUnitCount(units.Count);

            // Wait before launching the next one
            yield return new WaitForSeconds(waitTime);

            // Multiply speed gently so it doesn't get ridiculously fast too quickly. Clamped at 0.05f.
            waitTime = Mathf.Max(0.05f, waitTime * 0.95f);
        }

        // Stop the hovering effect so the remaining units FREEZE beautifully in slow-mo!
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);

        // If boss is dead and we have units left, we won! Let's update UI just in case.
        if (bossTarget == null && UIManager.instance != null)
        {
            UIManager.instance.UpdateUnitCount(units.Count);
        }
    }

    private System.Collections.IEnumerator ApplyHoverAndFade()
    {
        while (true) // Keep hovering even after the boss dies!
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i] != null)
                {
                    // Use the unit's unique hash code to create a random time offset so they all hover independently!
                    float uniqueOffset = units[i].GetHashCode() * 0.1f;
                    float bobVelocity = Mathf.Cos((Time.time + uniqueOffset) * 3f) * 0.8f; 
                    
                    units[i].position += new Vector3(0, bobVelocity * Time.deltaTime, 0);
                    
                    Renderer r = units[i].GetComponentInChildren<Renderer>();
                    if (r != null)
                    {
                        // Slowly continue fading to bright fiery orange while hovering!
                        r.material.color = Color.Lerp(r.material.color, new Color(1f, 0.5f, 0f), Time.deltaTime * 0.8f);
                    }
                }
            }
            yield return null;
        }
    }

    private System.Collections.IEnumerator ShootUnitAtBoss(Transform unit, Boss bossScript)
    {
        // Add a gorgeous dynamic trail for the cinematic flight!
        TrailRenderer trail = unit.gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.2f; // Short trail
        trail.startWidth = 0.4f;
        trail.endWidth = 0f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = Color.yellow;
        trail.endColor = new Color(1f, 0f, 0f, 0f); // Fades out to clear red
        
        Vector3 startPos = unit.position;
        
        // Generate a random control point to make the unit swerve in an arc!
        float randomSide = Random.Range(-10f, 10f);
        float randomHeight = Random.Range(-2f, 10f);
        Vector3 midPos = Vector3.Lerp(startPos, bossTarget.position, 0.5f) + new Vector3(randomSide, randomHeight, 0);
        
        float duration = 0.4f; // Flight time (0.4 seconds to reach boss)
        float elapsed = 0f;

        while (unit != null && bossTarget != null && elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Quadratic Bezier Curve calculation for smooth arcing
            Vector3 m1 = Vector3.Lerp(startPos, midPos, t);
            Vector3 m2 = Vector3.Lerp(midPos, bossTarget.position, t);
            unit.position = Vector3.Lerp(m1, m2, t);
            
            yield return null;
        }

        // The unit has arrived!
        if (unit != null && bossTarget != null)
        {
            if (bossScript != null) bossScript.TakeDamage(1);
            Destroy(unit.gameObject);
            
            activeAttackers--; // This unit is no longer flying!

            // Only trigger Game Over if the pool is empty AND all flying units have hit, but the boss is still alive!
            if (units.Count == 0 && activeAttackers == 0 && bossScript != null && bossScript.health > 0)
            {
                if (GameManager.instance != null) GameManager.instance.GameOver();
            }
        }
    }

    public int maxUnits = 1000;

    public void AddUnit()
    {
        if (units.Count >= maxUnits) return;
        
        GameObject newUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity, transform);
        units.Add(newUnit.transform);
        FormatCrowd();
    }

    public void AddUnits(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (units.Count >= maxUnits) break;
            
            GameObject newUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity, transform);
            units.Add(newUnit.transform);
        }
        FormatCrowd();
    }

    public void RemoveUnits(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (units.Count > 0)
            {
                Transform unitToRemove = units[units.Count - 1];
                units.RemoveAt(units.Count - 1);
                Destroy(unitToRemove.gameObject);
            }
        }
        
        // Trigger Game Over immediately if army drops to 0!
        if (units.Count == 0 && GameManager.instance != null && !GameManager.instance.isGameOver)
        {
            GameManager.instance.GameOver();
        }
        
        FormatCrowd();
    }

    // Arranges the crowd in a nice circular pattern (Fibonacci spiral)
    private void FormatCrowd()
    {
        for (int i = 0; i < units.Count; i++)
        {
            // The math for a sunflower seed / fibonacci spiral arrangement
            float phi = i * 137.5f; 
            // We added +1 to i here so they don't spawn exactly inside the leader at the center!
            float radius = spacing * Mathf.Sqrt(i + 1);

            // Convert polar coordinates to cartesian (x, z)
            float x = radius * Mathf.Cos(phi * Mathf.Deg2Rad);
            float z = radius * Mathf.Sin(phi * Mathf.Deg2Rad);

            // We lower the followers slightly on the Y axis so they touch the ground.
            // We also add a subtle, cute bounce using a sine wave! 
            // Using 'i' offsets the wave so they don't all bounce at the exact same time.
            float bounce = Mathf.Sin(Time.time * 15f + i) * 0.15f;
            float yOffset = -0.5f + bounce; 

            // Move the unit to this local position smoothly
            Vector3 targetLocalPosition = new Vector3(x, yOffset, z);
            units[i].localPosition = Vector3.Lerp(units[i].localPosition, targetLocalPosition, 10f * Time.deltaTime);
        }

        // Update UI!
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateUnitCount(units.Count);
        }
    }
}
