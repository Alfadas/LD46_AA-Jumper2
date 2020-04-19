using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBay : MonoBehaviour
{
    [SerializeField] Bomb bombPrefab;
    [SerializeField] int bombDropPoint = -200;
    [SerializeField] int bombSpawnDistance = 2;
    bool droped = false;

    private void Update()
    {
        if (transform.position.z < bombDropPoint && !droped)
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
