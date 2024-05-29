using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Enemy[] enemies;

    [SerializeField] private float normalEnemyRatio;
    [SerializeField] private int minibossAmount;
    [SerializeField] private int bossAmount;

    [SerializeField] private int waveNumber = 0;
    [SerializeField] private float waveInterval = 10;
    [SerializeField] private int spawnAmount = 0;
    [SerializeField] private int burstsAmount = 1;
    [SerializeField] private float burstInterval = 10;

    [SerializeField] private List<Enemy> enemyList = new List<Enemy>();

    public static event System.Action<int> WaveStarted;
    public static event System.Action WaveEnded;

    public static EnemySpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void StartWave(float delay)
    {
        StartCoroutine(DoStartWave(delay));
    }

    private IEnumerator DoStartWave(float delay)
    {
        yield return new WaitForSeconds(delay);

        waveNumber++;
        IncreaseSpawnAmount();

        SetupSpawnSequence();
        StartCoroutine(DoSpawning());

        WaveStarted?.Invoke(waveNumber);
    }

    public void EndWave()
    {
        minibossAmount = 0;
        bossAmount = 0;

        WaveEnded?.Invoke();

        StartWave(waveInterval);
    }

    private void IncreaseSpawnAmount()
    {
        if (waveNumber < 20)
            spawnAmount += 3;
        else
            spawnAmount = Mathf.Min((int)(0.09 * waveNumber * waveNumber - 0.0029 * waveNumber + 23.9580), 100);

        burstInterval = Mathf.Max(2 * Mathf.Pow(0.95f, waveNumber - 1), 0.1f);
    }

    private IEnumerator DoSpawning()
    {
        int burstCount = 0;

        while (burstCount < burstsAmount || enemyList.Count > 0)
        {
            burstCount++;

            for (int i = 0; i < burstsAmount; i++)
                SpawnEnemy();

            yield return new WaitForSeconds(burstInterval);
        }
    }

    private void SetupSpawnSequence()
    {
        List<Enemy.EnemyType> enemyTypes = new List<Enemy.EnemyType>();

        if (waveNumber % 15 == 0)
            bossAmount++;
        if (waveNumber % 10 == 0)
            minibossAmount++;

        int remainingSpawns = spawnAmount;

        int minibossSpawnAmount = 0;
        if (waveNumber % 5 == 0)
            minibossSpawnAmount = minibossAmount;

        int bossSpawnAmount = 0;
        if (waveNumber % 10 == 0)
            bossSpawnAmount = bossAmount;

        remainingSpawns -= minibossSpawnAmount + bossSpawnAmount;

        int normalEnemySpawnAmount = (int)(remainingSpawns * normalEnemyRatio);
        int eliteEnemySpawnAmount = remainingSpawns - normalEnemySpawnAmount;

        for (int i = 0; i < normalEnemySpawnAmount; i++)
            enemyTypes.Add(Enemy.EnemyType.Normal);

        for (int i = 0; i < eliteEnemySpawnAmount; i++)
            enemyTypes.Add(Enemy.EnemyType.Elite);

        for (int i = 0; i < minibossSpawnAmount; i++)
            enemyTypes.Add(Enemy.EnemyType.MiniBoss);

        for (int i = 0; i < bossSpawnAmount; i++)
            enemyTypes.Add(Enemy.EnemyType.Boss);

        Shuffle(enemyTypes);

        foreach (Enemy.EnemyType enemyType in enemyTypes)
        {
            Enemy enemy = GetRandomEnemyOfType(enemyType);
            if (enemy != null)
            {
                enemyList.Add(enemy);
                enemy.EnemyDied += OnEnemyDied;
            }
        }
    }

    private void SpawnEnemy()
    {
        foreach (Enemy enemy in enemyList)
        {
            if (enemy.gameObject.activeInHierarchy)
                continue;

            int randIndex = Random.Range(0, spawnPoints.Length);
            enemy.SpawnEnemy(spawnPoints[randIndex].position);
            return;
        }
    }

    private Enemy GetRandomEnemyOfType(Enemy.EnemyType enemyType)
    {
        List<Enemy> filteredEnemies = new List<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            if (enemy.enemyType == enemyType)
                filteredEnemies.Add(enemy);
        }

        if (filteredEnemies.Count == 0)
            return null;

        int randomIndex = Random.Range(0, filteredEnemies.Count);
        Enemy newEnemy = Instantiate(filteredEnemies[randomIndex]);
        newEnemy.InitEnemy();
        newEnemy.gameObject.SetActive(false);

        return newEnemy;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void OnEnemyDied(Enemy enemy)
    {
        enemy.EnemyDied -= OnEnemyDied;
        enemyList.Remove(enemy);

        if (enemyList.Count == 0)
            EndWave();
    }
}