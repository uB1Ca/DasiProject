using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]private AnimatorController animatorController;
    [SerializeField]private FloatingJoystick joystick;
    [SerializeField]private int playerAttackDamage = 10;
    [SerializeField]private float autoAttackCooldown = 1f;
    private float nextAutoAttackTime = 0f;

    [SerializeField]private float moveSpeed = 3f;
    [SerializeField]private float rotateSpeed = 35;
    
    private Rigidbody _rigidbody;
    private FieldOfView fov;

    private bool isMoving;
    private bool isAttacking;
    

    void Start()
    {
        fov = GetComponentInChildren<FieldOfView>();
        _rigidbody = GetComponent<Rigidbody>();
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
        
        Vector3 moveVector = Vector3.zero;
        moveVector.x = horizontalInput * moveSpeed * Time.deltaTime;
        moveVector.z = verticalInput * moveSpeed * Time.deltaTime;

        if (horizontalInput != 0 || verticalInput != 0)
        {
            Vector3 direction =
                Vector3.RotateTowards(transform.forward, moveVector, rotateSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direction);
            isMoving = true;
            isAttacking = false;
        }
        else
        {
            isMoving = false;
        }
        if(isMoving)
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
        _rigidbody.MovePosition(_rigidbody.position + moveVector);
    }

    private void HandleAutoAttack()
    {
        if (Time.time >= nextAutoAttackTime)
        {
            AutoAttack();
            nextAutoAttackTime = Time.time + autoAttackCooldown;
        }
    }

    void AutoAttack()
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
                EnemyController enemy = visibleTarget.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(playerAttackDamage);
                    hasValidTarget = true;
                }
            }
            else if(visibleTarget.CompareTag("Tree"))
            {
                TreeController tree = visibleTarget.GetComponent<TreeController>();
                if (tree != null)
                {
                    tree.TakeDamage(playerAttackDamage);
                    hasValidTarget = true;
                }
            }
        }

        if (hasValidTarget)
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }
    }
}