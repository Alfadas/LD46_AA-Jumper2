using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBay : MonoBehaviour
{
    [Tooltip("Prefab of the bomb to drop")]
    [SerializeField] Bomb bombPrefab;
    [Header("Drop point")]
    [Tooltip("Z Point for bomb drop")]
    [SerializeField] int bombDropPoint = -200;
    [Tooltip("-Y distance to ship")]
    [SerializeField] int bombSpawnDistance = 2;
    bool droped = false; //bool to secure one time drop

    private void Update()
    {
        if (transform.position.z < bombDropPoint && !droped) // if behind dropPoint and no bomb droped
        {
            DropBomb();
        }
    }
    public void DropBomb()
    {
        droped = true;
        Instantiate(bombPrefab, new Vector3(transform.position.x, transform.position.y - bombSpawnDistance, transform.position.z), Quaternion.identity);
    }
}