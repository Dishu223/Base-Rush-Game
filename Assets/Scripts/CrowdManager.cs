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
        // Load the upgraded starting army size from the shop!
        // Defaults to 4 if we haven't bought anything yet.
        int startAmount = PlayerPrefs.GetInt("StartingArmySize", 4);
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

    private System.Collections.IEnumerator BossFightSequence()
    {
        // Phase 1: Rise up into the air slowly!
        float riseDuration = 1.5f;
        float elapsed = 0f;
        Vector3[] startPositions = new Vector3[units.Count];
        Vector3[] targetPositions = new Vector3[units.Count];

        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] == null) continue;
            startPositions[i] = units[i].position;
            // Rise up between 3 and 7 units high, slightly randomized
            targetPositions[i] = units[i].position + new Vector3(0, Random.Range(3f, 7f), 0);
            
            // Spawn some cool lightning particles while they rise!
            if (VFXManager.instance != null)
            {
                VFXManager.instance.SpawnLightning(targetPositions[i]);
            }
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
                }
            }
            yield return null;
        }

        // Suspend in the air for a dramatic pause (2 seconds!)
        yield return new WaitForSeconds(2.0f);

        // Phase 2: Shoot at the boss 1 by 1 with exponential speed!
        Boss bossScript = bossTarget.GetComponent<Boss>();
        float waitTime = 0.2f; // Starts slow!
        
        while (units.Count > 0)
        {
            // If the boss is destroyed, stop firing!
            if (bossTarget == null) break;

            Transform attacker = units[units.Count - 1];
            units.RemoveAt(units.Count - 1);
            
            if (attacker != null)
            {
                StartCoroutine(ShootUnitAtBoss(attacker, bossScript));
            }

            // Update UI
            if (UIManager.instance != null) UIManager.instance.UpdateUnitCount(units.Count);

            // Wait before launching the next one
            yield return new WaitForSeconds(waitTime);

            // Exponentially speed up the attack! Clamped at 0.015f so it doesn't freeze.
            waitTime = Mathf.Max(0.015f, waitTime * 0.85f);
        }

        // If boss is dead and we have units left, we won! Let's update UI just in case.
        if (bossTarget == null && UIManager.instance != null)
        {
            UIManager.instance.UpdateUnitCount(units.Count);
        }
    }

    private System.Collections.IEnumerator ShootUnitAtBoss(Transform unit, Boss bossScript)
    {
        while (unit != null && bossTarget != null)
        {
            // Move incredibly fast towards the boss
            unit.position = Vector3.MoveTowards(unit.position, bossTarget.position, 40f * Time.deltaTime);
            
            if (Vector3.Distance(unit.position, bossTarget.position) < 1.5f)
            {
                if (bossScript != null) bossScript.TakeDamage(1);
                Destroy(unit.gameObject);

                if (units.Count == 0 && bossScript != null && bossScript.health > 0)
                {
                    if (GameManager.instance != null) GameManager.instance.GameOver();
                }
                yield break;
            }
            yield return null;
        }
    }

    public void AddUnits(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
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
            else
            {
                // Trigger Game Over!
                if (GameManager.instance != null) GameManager.instance.GameOver();
                break;
            }
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
