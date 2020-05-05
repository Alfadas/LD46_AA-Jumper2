using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyList : MonoBehaviour
{
    [SerializeField] List<Airship> enemies;
    private void Start()
    {
        enemies = new List<Airship>();
    }

    public void AddEnemy(Airship enemie)
    {
        enemies.Add(enemie);
    }

    public void RemoveEnemy(Airship enemie)
    {
        enemies.Remove(enemie);
    }

    public void KillAllAndClear()
    {
        for(int i = enemies.Count -1; i >= 0; i--)
        {
            enemies[i].Dissolve();
        }
        enemies.Clear();
    }

    public List<Airship> GetEnemies()
    {
        return enemies;
    }
}
