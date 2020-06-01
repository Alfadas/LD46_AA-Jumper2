using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTargetEnemy : MonoBehaviour
{
    const int changeTargetIfHiddenTime = 3;

    Turret turret = null;
    EnemyList enemyList = null;

    private void Start()
    {
        enemyList = FindObjectOfType<EnemyList>();
        if(enemyList == null)
        {
            Debug.LogWarning("missing Enemy List");
        }
        turret = gameObject.GetComponent<Turret>();
        if (turret == null)
        {
            Debug.LogWarning("missing turret component");
        }
    }

    public IEnumerator ChangeTarget(Airship oldTarget)
    {
        yield return new WaitForSeconds(changeTargetIfHiddenTime);
        if (turret.Target == oldTarget && turret.TargetHidden) // controll that turrent did not change the target and target is still hidden
        {
            List<Airship> enemies = enemyList.GetEnemies().GetRange(0, enemyList.GetEnemies().Count);
            enemies.Remove(oldTarget);
            turret.TargetHidden = false;
            SearchNearestTarget(enemies.ToArray());
        }
    }

    public IEnumerator UpdateTarget()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            Airship[] enemies = enemyList.GetEnemies().ToArray();
            if (turret.Target == null)
            {
                SearchNearestTarget(enemies);
            }
            else
            {
                float distanceToEnemy = Vector3.Distance(turret.TurretAimCenter, turret.Target.transform.position);
                if (distanceToEnemy > turret.Range)
                {
                    SearchNearestTarget(enemies);
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    void SearchNearestTarget(Airship[] enemies)
    {
        float shortestDistance = turret.Range;
        Airship nearestEnemy = null;
        foreach (Airship enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(turret.TurretAimCenter, enemy.transform.position);
            if (distanceToEnemy <= turret.Range && distanceToEnemy < shortestDistance && turret.TurretAimCenter.y <= enemy.transform.position.y)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        if (nearestEnemy != null)
        {
            turret.Target = nearestEnemy;
        }
        else
        {
            turret.Target = null;
        }
    }
}