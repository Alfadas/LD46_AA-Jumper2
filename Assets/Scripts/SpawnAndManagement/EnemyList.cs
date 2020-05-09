using System.Collections.Generic;
using UnityEngine;

public class EnemyList : MonoBehaviour
{
    [Tooltip("List of all enemies")]
    [SerializeField] List<Airship> enemies;// serialized for Tutorial, test and debug
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
            enemies[i].Dissolve();//disslove to grant no reward
        }
        enemies.Clear();
    }

    public List<Airship> GetEnemies()
    {
        return enemies;
    }
}