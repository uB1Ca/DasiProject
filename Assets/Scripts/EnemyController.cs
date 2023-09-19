using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour,HealthObserver
{
    private enum EnemyState
    {
        Idle,
        Chasing,
        Attacking
    }

    [Header("Components")]
    [SerializeField] private FieldOfView fov;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;

    [Header("Combat Settings")]
    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int enemyAttackDamage = 1;
    [SerializeField] private int enemyMaxHealth = 100;
    [SerializeField] private int currentHealth = 0;

    private EnemyState currentState = EnemyState.Idle;
    private Transform target;
    private float lastAttackTime;
    public HealthBar enemyHealthBar;

    private void Start()
    {
        lastAttackTime = -attackCooldown; // So that the enemy can attack immediately on seeing the player.
        currentHealth = enemyMaxHealth;
        enemyHealthBar.SetMaxHealth(enemyMaxHealth);
    }

    private void Update()
    {
        if (fov.visibleTargets.Count > 0)
        {
            target = fov.visibleTargets[0];
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            switch (currentState)
            {
                case EnemyState.Idle:
                    if (distanceToTarget <= chaseDistance)
                    {
                        currentState = EnemyState.Chasing;
                        SetAnimatorWalk(true);
                    }
                    break;

                case EnemyState.Chasing:
                    navMeshAgent.SetDestination(target.position);

                    if (distanceToTarget <= attackDistance)
                    {
                        currentState = EnemyState.Attacking;
                        SetAnimatorAttack(true);
                    }
                    break;

                case EnemyState.Attacking:
                    if (Time.time - lastAttackTime >= attackCooldown)
                    {
                        HandleAutoAttack();
                        lastAttackTime = Time.time;
                    }

                    if (distanceToTarget > attackDistance)
                    {
                        currentState = EnemyState.Chasing;
                        SetAnimatorAttack(false);
                    }
                    break;
            }
        }
        else
        {
            currentState = EnemyState.Idle;
            SetAnimatorWalk(false);
            SetAnimatorAttack(false);
        }
    }

    private void HandleAutoAttack()
    {
        bool hasValidTarget = false;

        foreach (Transform visibleTarget in fov.visibleTargets)
        {
            if (visibleTarget == null)
            {
                continue;
            }

            if (visibleTarget.CompareTag("Player")) // Assuming the player has the "Player" tag.
            {
                AttackPlayer(visibleTarget);
                hasValidTarget = true;
            }
        }

        if (!hasValidTarget)
        {
            // There are no valid targets in the FOV, so stop attacking.
            StopAttacking();
        }
    }

    private void AttackPlayer(Transform playerTransform)
    {
        PlayerController player = playerTransform.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(enemyAttackDamage);
        }
    }

    private void StopAttacking()
    {
        // Implement any logic to stop attacking or reset attack-related states.
        Debug.Log("Stopping Attack!");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        enemyHealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void UpdateHealth(int health)
    {
        currentHealth = health;
        enemyHealthBar.SetHealth(currentHealth);
    }

    private void Die()
    {
        Debug.Log("Enemy Died");
        Destroy(gameObject);
    }

    private void SetAnimatorWalk(bool walking)
    {
        animator.SetBool("isWalk", walking);
    }

    private void SetAnimatorAttack(bool attacking)
    {
        animator.SetBool("isAttack", attacking);
    }
}
