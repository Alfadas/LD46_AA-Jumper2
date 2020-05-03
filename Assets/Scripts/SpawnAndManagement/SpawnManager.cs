using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] EnemyList enemyList;
    [SerializeField] bool active = true;
    [SerializeField] int laneMidDif = 3;
    [SerializeField] int startWaitTime = 20;
    [SerializeField] int spawnInterval = 10;
    [SerializeField] int startValue = 2;
    [SerializeField] int valuePerWave = 5;
    [SerializeField] float valueDiviation = 0.1f;
    [SerializeField] float maxClassBuildupFactor = 0.9f;
    [SerializeField] GameObject[] enemyModels;
    [SerializeField] int[] cost;
    [SerializeField] MetalManager metalManager;
    Lane[] laneArray;
    bool spawning = false;
    int currentValue;
    int wave = 0;

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
                currentValue += valuePerWave;
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
        int maxClass = Mathf.RoundToInt((-Mathf.Pow(maxClassBuildupFactor, wave) + 1) * (enemyModels.Length - 1));
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
            while(lane == null)
            {
                List<Lane> freeLanes = laneArray.Where(n => !n.HasAirship).ToList();
                while (freeLanes.Count > 0 && lane == null)
                {
                    lane = freeLanes[Random.Range(0, freeLanes.Count)];
                    freeLanes.Remove(lane);
                }
                if(lane == null)
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }
            GameObject newEnemy = Instantiate(enemyModels[enemy], GetPosition(lane), Quaternion.identity, enemyList.transform);
            Airship newEnemyRenderer = newEnemy.GetComponent<Airship>();
            lane.SetAirship(newEnemyRenderer);
            enemyList.AddEnemy(newEnemyRenderer);
            newEnemyRenderer.SetManagingObjects(metalManager ,enemyList);
            yield return new WaitForSeconds(0.1f);
        }
        spawning = false;
    }

    Vector3 GetPosition(Lane lane)
    {

        float hight = lane.transform.position.y + Random.Range(-laneMidDif, laneMidDif);
        float width = lane.transform.position.x + Random.Range(-laneMidDif, laneMidDif);

        return new Vector3(width, hight, transform.position.z);
    }
}
