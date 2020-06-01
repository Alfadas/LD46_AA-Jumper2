using System.Collections.Generic;
using UnityEngine;

public class BombBay : AirshipPart
{
    [Header("Bombs")]
    [Tooltip("Prefab of the bomb to drop")]
    [SerializeField] Bomb bombPrefab = null;
    [Tooltip("Count of boombs to drop")]
    [SerializeField] int bombCount = 2;
    [Header("Drop point")]
    [Tooltip("Z Point for bomb drop")]
    [SerializeField] int bombDropPoint = -200;
    [Tooltip("-Y distance to ship")]
    [SerializeField] int bombSpawnDistance = 2;
    bool droped = false; //bool to secure one time drop
    Transform[] dropPoints = null; //array of drop Points attached to the part
     
    protected override void Start()
    {
        base.Start();
        List<Transform> transforms = new List<Transform>(GetComponentsInChildren<Transform>());
        if (transforms.Count == 1)
        {
            Debug.LogWarning("No bomb drop points found");
            return;
        }
        transforms.Remove(transform);
        dropPoints = transforms.ToArray();
    }
    private void Update()
    {
        if (transform.position.z < bombDropPoint && !droped) // if behind dropPoint and no bomb droped
        {
            DropBombs();
        }
    }
    public void DropBombs()
    {
        droped = true;
        for(int i = 0; i< bombCount; i += dropPoints.Length)
        {
            foreach(Transform dropPoint in dropPoints)
            {
                if (i< dropPoints.Length)
                {
                    Instantiate(bombPrefab, new Vector3(dropPoint.position.x, dropPoint.position.y - bombSpawnDistance, dropPoint.position.z), Quaternion.identity);
                }
            }
        } 
    }
}