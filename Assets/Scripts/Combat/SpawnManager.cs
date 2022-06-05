using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static float spawnInterval;
    public static int enemiesSpawned;
    public static int enemiesThisWave;
    public static bool startSpawn = false;
    //==========================================//
    private float lastSpawnTime;
    //==========================================//
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private List<GameObject> enemyPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        lastSpawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!startSpawn)
            return;

        if (startSpawn && enemiesSpawned < enemiesThisWave)
        {
            if (Time.time > lastSpawnTime + spawnInterval)
            {
                int r = Random.Range(0, enemyPrefabs.Count);
                int r2 = Random.Range(0, spawnPoints.Count);

                Instantiate(enemyPrefabs[r], spawnPoints[r2].transform.position, Quaternion.identity);

                lastSpawnTime = Time.time;
                enemiesSpawned++;
            }
        }       
    }
}
