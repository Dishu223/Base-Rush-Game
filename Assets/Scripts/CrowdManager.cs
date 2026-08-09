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
        if (bossTarget != null)
        {
            SwarmBoss();
            return; // Skip normal formatting
        }

        // Call this every frame so the units lerp smoothly to their positions
        FormatCrowd();
    }

    public void ChargeBoss(Transform boss)
    {
        bossTarget = boss;
    }

    private void SwarmBoss()
    {
        for (int i = units.Count - 1; i >= 0; i--)
        {
            if (units[i] == null) continue;

            // Move unit extremely fast towards the boss
            units[i].position = Vector3.MoveTowards(units[i].position, bossTarget.position, 25f * Time.deltaTime);

            // If it hits the boss (distance check)
            if (Vector3.Distance(units[i].position, bossTarget.position) < 1.5f)
            {
                Boss bossScript = bossTarget.GetComponent<Boss>();
                if (bossScript != null) bossScript.TakeDamage(1);

                Transform unitToRemove = units[i];
                units.RemoveAt(i);
                Destroy(unitToRemove.gameObject);

                if (units.Count == 0 && bossScript != null && bossScript.health > 0)
                {
                    if (GameManager.instance != null) GameManager.instance.GameOver();
                }
            }
        }
        
        // Update UI!
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateUnitCount(units.Count);
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
