using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyStats
{
    public enum EnemyType { Normal, Elite, Boss }
    public EnemyType enemyType;

    [SerializeField] protected EnemyData enemyData;
    protected AINavigation aiNavigation;
    protected GameObject player;
    protected Animator animator;
    protected RagdollController ragdollController;
    [SerializeField] protected Collider[] enemyCols;
    private CombatCollisionController collisionController;
    protected EnemyCanvas enemyCanvas;

    [SerializeField] private ParticleSystemEmitter firePS;

    public event System.Action<Enemy> EnemyDied;

    private Coroutine burnRoutine;
    protected Coroutine stunRoutine;
    private AudioSource audioSource;

    protected float speedModifier = 1;

    public virtual void InitEnemy()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ragdollController = GetComponent<RagdollController>();
        animator = GetComponent<Animator>();
        collisionController = GetComponent<CombatCollisionController>();
        aiNavigation = GetComponent<AINavigation>();
        enemyCanvas = GetComponent<EnemyCanvas>();
        audioSource = GetComponent<AudioSource>();
        aiNavigation.InitNavMeshAgent();

        ragdollController.DeactivateRagdoll();
    }

    public void SpawnEnemy(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        gameObject.SetActive(true);
    }

    public bool ChasePlayer(float attackRange)
    {
        aiNavigation.SetNavMeshTarget(player.transform.position, enemyData.moveSpeed * speedModifier);

        if (aiNavigation.OnReachTarget(attackRange))
        {
            aiNavigation.StopNavigation();
            return false;
        }
        else
        {
            aiNavigation.ResumeNavigation();
            return true;
        }
    }

    public void BurnEnemy(float duration, float interval, int damage)
    {
        if (burnRoutine != null)
            StopCoroutine(burnRoutine);

        ApplyStatusEffect(StatusEffect.StatusEffectType.Burn, true, StatusEffect.StatusEffectCategory.Debuff, duration);
        firePS.PlayLoopingPS();
        burnRoutine = StartCoroutine(StartBurning(duration, interval, damage));
    }

    private IEnumerator StartBurning(float duration, float interval, int damage)
    {
        float timeRemaining = duration;

        while (timeRemaining > 0)
        {
            if (health <= 0)
                break;

            enemyCanvas.SetHealthBarActive(true);
            TakeDamage((int)(damage * itemStats.burnDamageModifier), Vector3.zero, Vector3.zero, DamagePopup.ColorType.WHITE, true);
            PlayerController.Instance.AddPoints(3);
            yield return new WaitForSeconds(interval);
            timeRemaining -= interval;
        }

        firePS.StopLoopingPS();
        burnRoutine = null;
    }

    public void OnDamageEventStart(int col)
    {
        collisionController.EnableCollider(enemyData.damage, col);
    }

    public void OnDamageEventEnd(int col)
    {
        collisionController.DisableCollider(col);
    }

    protected void SetEnemyColliders(bool active)
    {
        foreach (Collider col in enemyCols)
            col.enabled = active;
    }

    public override void OnTakeDamage(int damage, Vector3 position, Vector3 direction, DamagePopup.ColorType color)
    {
        base.OnTakeDamage(damage, position, direction, color);
        enemyCanvas.SetHealthbar(health, maxHealth);

        if (health > 0)
            return;

        if (burnRoutine != null)
        {
            StopCoroutine(burnRoutine);
            burnRoutine = null;
        }

        EnemyDied?.Invoke(this);
        SetEnemyColliders(false);
        CheckDeathExplosion();
        enemyCanvas.OnEnemyDie();

        ragdollController.ActivateRagdoll(Vector3.zero, enemyData.deathPushbackForce);

        int randNum = Random.Range(0, 100);
        if (randNum <= itemStats.drumReloadPercentage)
        {
            AudioManager.Instance.PlayOneShot(Sound.SoundName.XKillDrum);
            PlayerController.Instance.RefillAmmoClip();
        }

        StartCoroutine(OnDie());
    }

    private IEnumerator OnDie()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    private void CheckDeathExplosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, itemStats.dynamiteExplodeRadius);

        if (hitColliders.Length > 0)
            audioSource.PlayOneShot(AudioManager.Instance.FindSound(Sound.SoundName.DynamiteExplode).clip);

        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = Utility.Instance.GetTopmostParent(hitCollider.transform).GetComponent<Enemy>();
            if (enemy == null || enemy == this)
                continue;

            Vector3 hitDir = (transform.position - hitCollider.transform.position).normalized;
            enemy.TakeDamage(itemStats.dynamiteExplodeDamage, Vector3.zero, -hitDir, DamagePopup.ColorType.WHITE, false);
            enemy.BurnEnemy(5f, 0.5f, (int)(itemStats.dynamiteExplodeDamage * itemStats.dynamiteBurnDamageModifier));
        }
    }

    public void ApplyRoarBuff(float speedModifier, float duration)
    {
        StartCoroutine(RoarBuffRoutine(speedModifier, duration));
    }

    private IEnumerator RoarBuffRoutine(float speedMod, float duration)
    {
        speedModifier = speedMod;
        aiNavigation.SetNavMeshTarget(player.transform.position, enemyData.moveSpeed * speedModifier);
        ApplyStatusEffect(StatusEffect.StatusEffectType.MoveSpeed, true, StatusEffect.StatusEffectCategory.Buff, duration);

        animator.SetFloat("moveSpeed", speedModifier);

        yield return new WaitForSeconds(duration);

        speedModifier = 1;
        animator.SetFloat("moveSpeed", speedModifier);
    }

    public virtual IEnumerator OnStun()
    {
        yield return null;
    }

    public void StunEnemy(float duration)
    {
        if (health <= 0)
            return;

        if (stunRoutine == null)
            stunRoutine = StartCoroutine(OnStun());

        ApplyStatusEffect(StatusEffect.StatusEffectType.Stun, true, StatusEffect.StatusEffectCategory.Debuff, duration);
    }

    public void SetHealth(int health, int maxHealth)
    {
        this.health = health;
        this.maxHealth = maxHealth;
        enemyCanvas.SetHealthbar(health, maxHealth);
    }

    public void SetHealthbar(bool active)
    {
        if (health <= 0)
        {
            enemyCanvas.SetHealthBarActive(false);
            return;
        }

        if (burnRoutine != null)
            return;

        enemyCanvas.SetHealthbar(health, maxHealth);
        enemyCanvas.SetHealthBarActive(active);
    }

    private void ApplyStatusEffect(StatusEffect.StatusEffectType statusEffect, bool haveTimer, StatusEffect.StatusEffectCategory statusEffectCategory, float duration)
    {
        if (health > 0)
            enemyCanvas.ApplyStatusEffect(statusEffect, haveTimer, statusEffectCategory, duration);
        else
            enemyCanvas.OnEnemyDie();
    }
}