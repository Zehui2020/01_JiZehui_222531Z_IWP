using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Enemy[] enemies;

    [SerializeField] private float normalEnemyRatio;
    [SerializeField] private int bossAmount;
    [SerializeField] private int bossSpawnWaveInterval;

    [SerializeField] private int waveNumber = 0;
    [SerializeField] private float waveInterval = 10;
    [SerializeField] private int spawnAmount = 0;
    [SerializeField] private int burstsAmount = 1;
    [SerializeField] private float burstInterval = 10;

    [SerializeField] private List<Enemy> enemyList = new List<Enemy>();

    public static event System.Action<int> WaveStarted;
    public static event System.Action WaveEnded;

    private Coroutine StartWaveRoutine;

    public static EnemySpawner Instance;

    private int healthIncrease = 0;
    [SerializeField] private int healthIncreaseAmount = 50;
    [SerializeField] private float healthModifier = 1.1f;

    private void Awake()
    {
        Instance = this;
    }

    public void StartWave(float delay)
    {
        if (StartWaveRoutine != null)
            StopCoroutine(StartWaveRoutine);

        // Apply wave modifications
        if (waveNumber >= 10)
        {
            healthModifier *= 1.1f;
        }
        else if (waveNumber > 0)
        {
            healthIncrease += healthIncreaseAmount;
        }

        StartWaveRoutine = StartCoroutine(DoStartWave(delay));
    }

    private IEnumerator DoStartWave(float delay)
    {
        yield return new WaitForSeconds(delay);

        waveNumber++;

        SetupSpawnSequence();
        StartCoroutine(DoSpawning());

        WaveStarted?.Invoke(waveNumber);
    }

    public void EndWave()
    {
        bossAmount = 0;
        IncreaseSpawnAmount();

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

        if (waveNumber % bossSpawnWaveInterval == 0)
            bossAmount++;

        int remainingSpawns = spawnAmount;

        int minibossSpawnAmount = 0;

        int bossSpawnAmount = 0;
        if (waveNumber % bossSpawnWaveInterval == 0)
            bossSpawnAmount = bossAmount;

        remainingSpawns -= minibossSpawnAmount + bossSpawnAmount;

        int normalEnemySpawnAmount = (int)(remainingSpawns * normalEnemyRatio);
        int eliteEnemySpawnAmount = remainingSpawns - normalEnemySpawnAmount;

        for (int i = 0; i < normalEnemySpawnAmount; i++)
            enemyTypes.Add(Enemy.EnemyType.Normal);

        for (int i = 0; i < eliteEnemySpawnAmount; i++)
            enemyTypes.Add(Enemy.EnemyType.Elite);

        for (int i = 0; i < bossSpawnAmount; i++)
            enemyTypes.Add(Enemy.EnemyType.Boss);

        Shuffle(enemyTypes);

        foreach (Enemy.EnemyType enemyType in enemyTypes)
        {
            Enemy enemy = GetRandomEnemyOfType(enemyType);
            if (enemy != null)
            {
                enemyList.Add(enemy);

                if (waveNumber >= 10)
                {
                    int newHealth = (int)((enemy.health + healthIncrease) * healthModifier);
                    enemy.SetHealth(newHealth, newHealth);
                }
                else
                {
                    int newHealth = enemy.health += healthIncrease;
                    enemy.SetHealth(newHealth, newHealth);
                }

                enemy.EnemyDied += OnEnemyDied;
            }
        }
    }

    public void SetWave(int newWaveNumber)
    {
        int repeats = newWaveNumber - waveNumber;

        for (int i = 0; i < repeats; i++)
            EndWave();

        waveNumber = newWaveNumber;
    }

    private void SpawnEnemy()
    {
        foreach (Enemy enemy in enemyList)
        {
            if (enemy.gameObject.activeInHierarchy)
                continue;

            int randIndex = Random.Range(0, spawnPoints.Length);
            enemy.SpawnEnemy(spawnPoints[randIndex].position);

            if (enemy.enemyType == Enemy.EnemyType.Boss)
                CompanionManager.Instance.ShowMessage(CompanionManager.Instance.companionMessenger.bossSpawnMessage);

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