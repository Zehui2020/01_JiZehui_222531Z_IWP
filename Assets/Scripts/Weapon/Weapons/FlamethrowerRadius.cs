using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerRadius : MonoBehaviour
{
    public delegate void EnemyEnterEvent(Enemy enemy);
    public delegate void EnemyExitEvent(Enemy enemy);

    [SerializeField] private Collider trigger;
    public List<Enemy> enemiesInRadius;

    public void EnableTrigger(bool enabled)
    {
        trigger.enabled = enabled;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (GetTopmostParent(col.transform).TryGetComponent<Enemy>(out Enemy enemy) && col.enabled)
        {
            enemiesInRadius.Add(enemy);
            enemy.EnemyDied += OnEnemyDied;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (GetTopmostParent(col.transform).TryGetComponent<Enemy>(out Enemy enemy) && col.enabled)
        {
            enemiesInRadius.Remove(enemy);
            enemy.EnemyDied -= OnEnemyDied;
        }
    }

    private void OnEnemyDied(Enemy enemy)
    {
        enemiesInRadius.Remove(enemy);
    }

    private Transform GetTopmostParent(Transform child)
    {
        Transform parent = child.parent;

        while (parent != null)
        {
            child = parent;
            parent = child.parent;
        }

        return child;
    }
}