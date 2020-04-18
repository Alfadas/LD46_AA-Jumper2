using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] EnemyList enemyList;
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
            if (active)
            {
                wave++;
                Debug.Log("Wave " + wave + " started, value:" + currentValue);
                StartCoroutine(SpawnEnemies(CreateSpawnList(currentValue)));
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
        foreach (int enemy in enemies)
        {
            GameObject newEnemy = Instantiate(enemyModels[enemy], enemyList.transform.position, Quaternion.identity, enemyList.transform);
            MeshRenderer newEnemyRenderer = newEnemy.GetComponent<MeshRenderer>();
            Vector3 newPosition = GetPosition();
            bool placeFound = false;
            int counter = 0;
            do
            {
                foreach (MeshRenderer otherEnemyRenderer in enemyList.GetEnemies())
                {
                    Debug.Log(newEnemyRenderer.bounds.extents.x + otherEnemyRenderer.bounds.extents.x);
                    Debug.Log(newEnemyRenderer.bounds.extents.y + otherEnemyRenderer.bounds.extents.y);
                    Debug.Log(newEnemyRenderer.bounds.extents.z + otherEnemyRenderer.bounds.extents.z);
                    newPosition = GetPosition();
                    if (newEnemyRenderer.bounds.extents.x + otherEnemyRenderer.bounds.extents.x > Mathf.Abs(newPosition.x - otherEnemyRenderer.bounds.center.x)
                        || newEnemyRenderer.bounds.extents.y + otherEnemyRenderer.bounds.extents.y > Mathf.Abs(newPosition.y - otherEnemyRenderer.bounds.center.y)
                        || newEnemyRenderer.bounds.extents.z + otherEnemyRenderer.bounds.extents.z > Mathf.Abs(newPosition.z - otherEnemyRenderer.bounds.center.z))
                    {
                        Debug.LogWarning("spawnpoint to close: " + newPosition);
                        break;
                    }
                    placeFound = true;
                }
                counter++;
            } while (!placeFound && counter < 100);
            Debug.Log("spawnpoint to found: " + newPosition);
            newEnemy.transform.position = newPosition;
            enemyList.AddEnemy(newEnemyRenderer);
            yield return new WaitForSeconds(0.1f);
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
