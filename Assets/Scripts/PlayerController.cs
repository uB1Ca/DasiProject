using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour,HealthObserver
{
    [Header("Player Settings")]
    [SerializeField] private AnimatorController animatorController;
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private float autoAttackCooldown = 1f;
    [SerializeField] private int playerAttackDamage = 1;
    [SerializeField] private int playerMaxHealth = 100;
    [SerializeField] private int currentHealth = 0;
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotateSpeed = 35;
    [SerializeField] private float moveSmoothness = 5f; // Adjust as needed.

    [Header("Inventory Settings")]
    [SerializeField] private GameObject bag;

    public TreeController treeController;
    private Rigidbody _rigidbody;
    private FieldOfView fov;
    public GameObject gameOverRestart;

    private bool isMoving;
    private bool isAttacking;

    private float nextAutoAttackTime = 0f;

    private int logsCollected = 0;
    public HealthBar playerHealthBar;

    private void Start()
    {
        InitializeComponents();
        currentHealth = playerMaxHealth;
        playerHealthBar.SetMaxHealth(playerMaxHealth);
    }

    private void InitializeComponents()
    {
        fov = GetComponentInChildren<FieldOfView>();
        _rigidbody = GetComponent<Rigidbody>();
        gameOverRestart.SetActive(false);
    }

    private void Update()
    {
        HandleMovement();
        HandleAutoAttack();
    }

    private void HandleMovement()
    {
        float horizontalInput = joystick.Horizontal;
        float verticalInput = joystick.Vertical;

        Vector3 moveVector = new Vector3(horizontalInput, 0f, verticalInput);
        if (moveVector.magnitude > 0.1f) // Apply a dead zone threshold.
        {
            RotateTowardsDirection(moveVector);
            isMoving = true;
            isAttacking = false;
        }
        else
        {
            isMoving = false;
        }

        UpdateAnimatorState();
        MoveCharacter(moveVector);
    }

    private void RotateTowardsDirection(Vector3 direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
    }

    private void UpdateAnimatorState()
    {
        if (isMoving)
        {
            animatorController.PlayWalk();
        }
        else if (isAttacking)
        {
            animatorController.SetTrigger("Attack");
        }
        else
        {
            animatorController.PlayIdle();
        }
    }

    private void MoveCharacter(Vector3 moveVector)
    {
        Vector3 targetPosition = transform.position + moveVector.normalized * moveSpeed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSmoothness * Time.deltaTime);
    }

    private void HandleAutoAttack()
    {
        if (Time.time >= nextAutoAttackTime)
        {
            AutoAttack();
            nextAutoAttackTime = Time.time + autoAttackCooldown;
        }
    }

    private void AutoAttack()
    {
        bool hasValidTarget = false;
        foreach (Transform visibleTarget in fov.visibleTargets)
        {
            if (visibleTarget == null)
            {
                continue;
            }

            if (visibleTarget.CompareTag("Enemy"))
            {
                AttackEnemy(visibleTarget);
                hasValidTarget = true;
            }
            else if (visibleTarget.CompareTag("Tree"))
            {
                AttackTree(visibleTarget);
                hasValidTarget = true;
            }
        }

        isAttacking = hasValidTarget;
    }

    private void AttackEnemy(Transform enemyTransform)
    {
        EnemyController enemy = enemyTransform.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(playerAttackDamage);
        }
    }

    private void AttackTree(Transform treeTransform)
    {
        TreeController tree = treeTransform.GetComponent<TreeController>();
        if (tree != null && !tree.IsCut)
        {
            tree.TakeDamage(playerAttackDamage);
            isAttacking = true;
            // Set IsCut to true when the tree is cut down
        }
        else
        {
            isAttacking = false;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        playerHealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void UpdateHealth(int health)
    {
        currentHealth = health;
        playerHealthBar.SetHealth(currentHealth);
    }

    private void Die()
    {
        Debug.Log("Player Die");
        gameOverRestart.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Log"))
        {
            CollectLog(other.gameObject);
        }
    }

    private void CollectLog(GameObject log)
    {
        if (InventoryManager.Instance.AddItem(log))
        {
            StartCoroutine(MoveLogToBag(log, bag.transform.position, 1.0f)); // Adjust duration as needed.
        }
    }

    private IEnumerator MoveLogToBag(GameObject log, Vector3 bagPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = log.transform.position;

        while (elapsedTime < duration && log != null) // Check if the log is null
        {
            float t = elapsedTime / duration;
            log.transform.position = Vector3.Lerp(initialPosition, bagPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (log != null)
        {
            log.transform.position = bagPosition;
        
            // Handle any post-collection logic here.
            // For example, destroying the log or pooling it for later use.
            Destroy(log);
        }
    }
}
