using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject groundSegmentPrefab; // A flat stretch of runway
    public GameObject finishLinePrefab;
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

        for (int i = 0; i < segmentsToSpawn; i++)
        {
            // 1. Spawn Ground
            Vector3 segmentPos = new Vector3(0, 0, currentZ);
            Instantiate(groundSegmentPrefab, segmentPos, Quaternion.identity, transform);

            // 2. Skip spawning obstacles on the very first segment so player has time to react
            if (i > 0)
            {
                SpawnObstaclesOnSegment(currentZ, currentZ + segmentLength);
            }

            currentZ += segmentLength;
        }

        // 3. Spawn Finish Line at the end
        Vector3 finishPos = new Vector3(0, 0, currentZ);
        Instantiate(finishLinePrefab, finishPos, Quaternion.identity, transform);
    }

    void SpawnObstaclesOnSegment(float startZ, float endZ)
    {
        // We divide the segment into "rows" to ensure things don't overlap too badly
        int rows = 3; 
        float distancePerRow = segmentLength / rows;

        for (int r = 0; r < rows; r++)
        {
            float rowZ = startZ + (r * distancePerRow) + (distancePerRow / 2f); // Center of the row
            
            // Randomly decide what to spawn in this row (0 = Nothing, 1 = Gate, 2 = Coins, 3 = Obstacle)
            int spawnType = Random.Range(0, 4);

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
            Instantiate(coinPrefab, new Vector3(randomX, 0, zPos + (i * 2f)), Quaternion.identity, transform);
        }
    }

    void SpawnObstacle(float zPos)
    {
        if (obstaclePrefabs.Length == 0) return;

        GameObject obstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        float randomX = Random.Range(-runwayWidth, runwayWidth);
        
        Instantiate(obstacle, new Vector3(randomX, 0, zPos), Quaternion.identity, transform);
    }
}
