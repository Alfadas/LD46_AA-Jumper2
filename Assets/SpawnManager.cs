using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] bool active = true;
    [SerializeField] int maxHight = 100;
    [SerializeField] int lanes = 3;
    [SerializeField] int laneHight = 30;
    [SerializeField] float laneMidDif = 0.2f;
    [SerializeField] int widthPadding = 5;
    [SerializeField] int spawnInterval = 10;
    [SerializeField] float QFunktionA = 0.07f; //stretching and compression of the funktion to determine the width 
    [SerializeField] int startValue = 2;
    [SerializeField] int valuePerWave = 5;
    [SerializeField] float maxClassBuildupFactor = 0.9f;
    [SerializeField] GameObject[] enemyModels;
    [SerializeField] int[] cost;
    int currentValue;
    int wave = 0;


    private void Start()
    {
        currentValue = startValue;
        StartCoroutine(SpawnCycle());
    }

    IEnumerator SpawnCycle()
    {
        while (true)
        {
            Debug.Log("Cycle started");
            if (active)
            {
                wave++;
                Debug.Log("Wave " + wave + " started");
                SpawnEnemies(CreateSpawnList(currentValue));
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
        Debug.Log("max " + maxClass);
        List<int> newEnemies = new List<int>();
        while (remainingValue > 0)
        {
            int newEnemy = Random.Range(0, maxClass);
            Debug.Log("selected type " + newEnemy);
            if (cost[newEnemy] <= remainingValue)
            {
                remainingValue -= cost[newEnemy];
                newEnemies.Add(newEnemy);
                Debug.Log("added type " + newEnemy);
            }
        }
        return newEnemies;
    }

    void SpawnEnemies(List<int> enemies)
    {
        foreach (int enemy in enemies)
        {
            Instantiate(enemyModels[enemy], GetPosition(), Quaternion.identity, transform);
        }
    }

    Vector3 GetPosition()
    {
        int lane = Random.Range(1, lanes + 1);
        float hight = lane * laneHight + laneHight * Random.Range(0.5f - laneMidDif, 0.5f + laneMidDif);
        float widthMax = QFunktionA * Mathf.Pow(hight, 2);
        float width = 0;
        if (widthMax > widthPadding)
        {
            widthMax -= widthPadding;
            width = Random.Range(-widthMax, widthMax);
        }

        return new Vector3(width, hight, transform.position.z);
    }
}
