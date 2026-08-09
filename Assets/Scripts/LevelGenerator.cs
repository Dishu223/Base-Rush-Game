using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject groundSegmentPrefab; // A flat stretch of runway
    public GameObject finishLinePrefab;
    public GameObject bossPrefab; // The final boss
    public GameObject[] gatePrefabs; // Array of different gate prefabs
    public GameObject[] obstaclePrefabs; // Enemies, walls, sawblades
    public GameObject coinPrefab;

    [Header("Level Settings")]
    public int segmentsToSpawn = 5;
    public float segmentLength = 50f; // Z length of one ground segment
    public float runwayWidth = 4f; // How far left/right we can spawn things

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        float currentZ = 0f;
        int level = 1;
        if (GameManager.instance != null) level = GameManager.instance.currentLevel;

        // Increase level length! +1 segment every 2 levels
        int totalSegments = segmentsToSpawn + (level / 2);
        
        // Pick a random floor color for this level to make it feel fresh!
        Color floorColor = Random.ColorHSV(0f, 1f, 0.5f, 0.8f, 0.2f, 0.5f);

        for (int i = 0; i < totalSegments; i++)
        {
            // 1. Spawn Ground
            Vector3 segmentPos = new Vector3(0, 0, currentZ);
            GameObject ground = Instantiate(groundSegmentPrefab, segmentPos, Quaternion.identity, transform);
            
            // Apply level color
            Renderer r = ground.GetComponent<Renderer>();
            if (r != null) r.material.color = floorColor;

            // 2. Spawn obstacles. Start at Z=15 on the first segment so player has a small buffer.
            float startSpawnZ = (i == 0) ? 15f : currentZ;
            
            // Ensure we stop spawning obstacles well before the Finish Line!
            float endSpawnZ = currentZ + segmentLength;
            if (i == totalSegments - 1) endSpawnZ -= 15f; 
            
            SpawnObstaclesOnSegment(startSpawnZ, endSpawnZ);

            currentZ += segmentLength;
        }

        // 3. Spawn Finish Line at the end (Raised to Y=1 so it's not underground)
        Vector3 finishPos = new Vector3(0, 1f, currentZ);
        Instantiate(finishLinePrefab, finishPos, Quaternion.identity, transform);

        // 4. Spawn the Boss behind the finish line and upgrade it based on Level!
        if (bossPrefab != null)
        {
            GameObject bossObj = Instantiate(bossPrefab, new Vector3(0, 1f, currentZ + 20f), Quaternion.identity, transform);
            Boss bossScript = bossObj.GetComponent<Boss>();
            if (bossScript != null)
            {
                bossScript.health += (level - 1) * 20; // +20 Health per level!
                
                // Scale up boss slightly per level (max 2x)
                float scaleMod = Mathf.Min(1f + ((level - 1) * 0.1f), 2f);
                bossObj.transform.localScale *= scaleMod;
                
                // Random Boss color per level!
                Renderer bossRenderer = bossObj.GetComponent<Renderer>();
                if (bossRenderer != null) bossRenderer.material.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.5f, 1f);
            }
        }
    }

    void SpawnObstaclesOnSegment(float startZ, float endZ)
    {
        // Increase rows from 3 to 6 for WAY more activity!
        int rows = 6; 
        float distancePerRow = (endZ - startZ) / rows;

        for (int r = 0; r < rows; r++)
        {
            float rowZ = startZ + (r * distancePerRow); 
            
            // Randomly decide what to spawn. Removed 0 (Nothing) so it ALWAYS spawns something!
            int spawnType = Random.Range(1, 4);

            switch (spawnType)
            {
                case 1:
                    SpawnGates(rowZ);
                    break;
                case 2:
                    SpawnCoins(rowZ);
                    break;
                case 3:
                    SpawnObstacle(rowZ);
                    break;
            }
        }
    }

    void SpawnGates(float zPos)
    {
        if (gatePrefabs.Length == 0) return;

        // Spawn a left gate and a right gate (raised up to Y = 1.5 so they aren't underground)
        GameObject gate1 = gatePrefabs[Random.Range(0, gatePrefabs.Length)];
        GameObject gate2 = gatePrefabs[Random.Range(0, gatePrefabs.Length)];

        Instantiate(gate1, new Vector3(-runwayWidth / 2f, 1.5f, zPos), Quaternion.identity, transform);
        Instantiate(gate2, new Vector3(runwayWidth / 2f, 1.5f, zPos), Quaternion.identity, transform);
    }

    void SpawnCoins(float zPos)
    {
        if (coinPrefab == null) return;

        // Spawn a line of 3 to 5 coins
        int numCoins = Random.Range(3, 6);
        float randomX = Random.Range(-runwayWidth, runwayWidth); // Pick a random lane
        
        for (int i = 0; i < numCoins; i++)
        {
            // Raised to Y=1f so coins aren't underground
            // We use coinPrefab.transform.rotation so it keeps its 90-degree X rotation!
            Instantiate(coinPrefab, new Vector3(randomX, 1f, zPos + (i * 2f)), coinPrefab.transform.rotation, transform);
        }
    }

    void SpawnObstacle(float zPos)
    {
        if (obstaclePrefabs.Length == 0) return;

        GameObject obstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        float randomX = Random.Range(-runwayWidth, runwayWidth);
        
        // Raised to Y=1f so enemies and sawblades sit on the ground
        Instantiate(obstacle, new Vector3(randomX, 1f, zPos), Quaternion.identity, transform);
    }
}
