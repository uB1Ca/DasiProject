using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour, IAttackable
{
    [SerializeField]private AnimatorController animatorController;
    [SerializeField]private FieldOfView fov;
    public int enemyMaxHealth = 100;
    private int currentHealth;
    public List<Transform> waypoints;
    public Transform target;
    public float attackRange = 2f;
    public float chasingRange = 10f;
    public float movementSpeed = 3f;
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private bool isChasing = false;
    private bool isPatroling = true;

    [Header("Waypoint Stop/Wait Time")]
    [Range(0.1f, 5f)]
    public float minWaitTime = 1f;
    [Range(5f, 10f)]
    public float maxWaitTime = 6f;

    private float waitTime;
    private float waitTimer;
    private Animator animator;


    

    private void Start()
    {
        animator = GetComponent<Animator>();
        fov = GetComponent<FieldOfView>();
        agent = GetComponent<NavMeshAgent>();
        GoToNextWaypoint();
        currentHealth = enemyMaxHealth;
    }

    private void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }else if (isPatroling)
        {
            Patrol();
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (waitTimer <= 0f)
            {
                GoToNextWaypoint();
            }
            else
            {
                waitTimer -= Time.deltaTime;
            }
        }

        if (fov != null)
        {
            if (fov.visibleTargets.Contains(target))
            {
                PlayerDetected(target);
            }
            else
            {
                PlayerLost();
            }
        }
    }

    private void StartPatrol()
    {
        isChasing = false;
        isPatroling = true;
        animator.SetTrigger("WalkTrigger");
        Patrol();
    }

    private void Patrol()
    {
        if (waypoints.Count == 0)
        {
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (waitTimer <= 0)
            {
                GoToNextWaypoint();
            }
            else
            {
                waitTimer -= Time.deltaTime;
            }
        }
    }
    
    private  void GoToNextWaypoint()
    {
        agent.destination = waypoints[currentWaypointIndex].position;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        StartWait();
    }

    private void StartWait()
    {
        isPatroling = false;
        waitTime = Random.Range(minWaitTime, maxWaitTime);
        waitTimer = waitTime;
    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer > attackRange)
        {
            agent.destination = target.position;
            animator.SetTrigger("WalkTrigger");
        }

        if (Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            animator.SetTrigger("AttackTrigger");
        }
    }

    public void PlayerDetected(Transform playerTransform)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= chasingRange)
        {
            isChasing = true;
            isPatroling = false;
            target = playerTransform;
            animator.SetTrigger("WalkTrigger");
        }
    }

    public void PlayerLost()
    {
        isChasing = false;
        isPatroling = true;
        StartPatrol();
        animator.SetTrigger("IdleTrigger");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
