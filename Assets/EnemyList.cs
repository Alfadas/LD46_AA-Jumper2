using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyList : MonoBehaviour
{
    [SerializeField] List<MeshRenderer> enemies;
    private void Start()
    {
        enemies = new List<MeshRenderer>();
    }

    public void AddEnemy(MeshRenderer enemie)
    {
        enemies.Add(enemie);
    }

    public void RemoveEnemy(MeshRenderer enemie)
    {
        enemies.Remove(enemie);
    }

    public List<MeshRenderer> GetEnemies()
    {
        return enemies;
    }
}
