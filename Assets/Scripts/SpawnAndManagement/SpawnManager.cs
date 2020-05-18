using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] MetalManager metalManager;
    [SerializeField] EnemyList enemyList;
    [SerializeField] bool active = true;
    [SerializeField] int laneHightStepMax;
    [Header("SpawnPointChanger")]
    [Tooltip("Absolute x y dif from lane mid")]
    [SerializeField] int laneMidDif = 3;
    [Header("SpawnTime")]
    [SerializeField] int startWaitTime = 20;
    [SerializeField] int spawnInterval = 10;
    [Header("WaveValue")]
    [SerializeField] int startValue = 2;
    [SerializeField] float valuePercPerWave = 0.1f;
    [SerializeField] float valueDiviation = 0.1f;
    [Header("OverTimeClassChange")]
    [SerializeField] float maxClassBuildupFactor = 0.9f;
    [Header("Ships")]
    [SerializeField] GameObject[] enemyModels;
    [SerializeField] int[] cost;

    Lane[] laneArray;// Array of all lanes
    bool spawning = false; //secure one time spawn  
    int currentValue; 
    int wave = 0; // current wave

    private void Start()
    {
        currentValue = startValue;
        laneArray = gameObject.GetComponentsInChildren<Lane>();
        StartCoroutine(SpawnCycle());
    }
    IEnumerator SpawnCycle()
    {
        yield return new WaitForSeconds(startWaitTime);
        while (true)
        {
            if (active)
            {
                wave++;
                int nextWaveValue = currentValue + Mathf.RoundToInt(currentValue * Random.Range(-valueDiviation, valueDiviation));
                Debug.Log("Wave " + wave + " started, value:" + nextWaveValue);
                StartCoroutine(SpawnEnemies(CreateSpawnList(nextWaveValue)));
                yield return new WaitWhile(() => spawning == true);
                currentValue += Mathf.CeilToInt(currentValue * valuePercPerWave);
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                yield return new WaitUntil(() => active == true);
            }
        }
    }

    List<int> CreateSpawnList(int remainingValue)
    {
        int maxClass = Mathf.RoundToInt((-Mathf.Pow(maxClassBuildupFactor, wave) + 1) * (enemyModels.Length - 1)) + 1;
        //int maxClass = enemyModels.Length + 1;
        List<int> newEnemies = new List<int>();
        while (remainingValue > 0)
        {
            int newEnemy = Random.Range(0, maxClass);
            if (cost[newEnemy] <= remainingValue)
            {
                remainingValue -= cost[newEnemy];
                newEnemies.Add(newEnemy);
            }
        }
        return newEnemies;
    }

    IEnumerator SpawnEnemies(List<int> enemies)
    {
        spawning = true;
        foreach (int enemy in enemies)
        {
            Lane lane = null;
            if(wave*0.2f < laneHightStepMax)
            {
                while (lane == null)
                {
                    List<Lane> freeLanes = laneArray.Where(n => !n.HasAirship).ToList();
                    while (freeLanes.Count > 0 && lane == null)
                    {
                        Lane freeLane = freeLanes[Random.Range(0, freeLanes.Count)];
                        if (freeLane.LaneHightStep < wave * 0.2f)
                        {
                            lane = TrySetLane(enemy, freeLanes, freeLane);
                        }
                    }
                    if (lane == null)
                    {
                        yield return new WaitForSeconds(0.2f);
                    }
                }
                SpawnNewEnemy(enemy, lane);
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                while (lane == null)
                {
                    List<Lane> freeLanes = laneArray.Where(n => !n.HasAirship).ToList();
                    while (freeLanes.Count > 0 && lane == null)
                    {
                        Lane freeLane = freeLanes[Random.Range(0, freeLanes.Count)];
                        lane = TrySetLane(enemy, freeLanes, freeLane);
                    }
                    if (lane == null)
                    {
                        yield return new WaitForSeconds(0.2f);
                    }
                }
                SpawnNewEnemy(enemy, lane);
                yield return new WaitForSeconds(0.1f);
            }
        }
        spawning = false;
    }

    private static Lane TrySetLane(int enemy, List<Lane> freeLanes, Lane freeLane)
    {
        if (enemy >= freeLane.LaneHightStep)
        {
            freeLanes.Remove(freeLane);
            return freeLane;
        }
        else
        {
            return null;
        }
    }

    private void SpawnNewEnemy(int enemy, Lane lane)
    {
        GameObject newEnemy = Instantiate(enemyModels[enemy], GetPosition(lane), Quaternion.identity, enemyList.transform);
        Airship newEnemyRenderer = newEnemy.GetComponent<Airship>();
        lane.SetAirship(newEnemyRenderer);
        enemyList.AddEnemy(newEnemyRenderer);
        newEnemyRenderer.SetManagingObjects(metalManager, enemyList);
    }

    Vector3 GetPosition(Lane lane)
    {

        float hight = lane.transform.position.y + Random.Range(-laneMidDif, laneMidDif);
        float width = lane.transform.position.x + Random.Range(-laneMidDif, laneMidDif);

        return new Vector3(width, hight, transform.position.z);
    }
}
