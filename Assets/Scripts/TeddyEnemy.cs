using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class TeddyEnemy : MonoBehaviour
{
    FirstPersonController player;
    NavMeshAgent agent;
    Animator animator;

    [Header("Detection")]
    [SerializeField] float detectionRange = 12f;
    [SerializeField] float fieldOfViewAngle = 120f;

    [Header("Movement")]
    [SerializeField] float stoppingDistance = 2f;
    [SerializeField] float chaseSpeed = 6f;

    [Header("Attack")]
    [SerializeField] float attackRange = 2.3f;
    [SerializeField] float attackCooldown = 1.2f;
    [SerializeField] float attackDamage = 10f;

    bool isDead = false;
    float nextAttackTime = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
            agent.speed = chaseSpeed;
            agent.isStopped = true;
        }
    }

    void Update()
    {
        if (isDead || player == null || agent == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer && distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (canSeePlayer)
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.transform.position);

        if (animator != null)
        {
            animator.SetFloat("Speed", 1f);
        }
    }

    void StopChasing()
    {
        agent.isStopped = true;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    void Attack()
    {
        agent.isStopped = true;

        Vector3 lookDirection = player.transform.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }

        if (Time.time >= nextAttackTime)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // This function is called by the Animation Event during the punch animation.
    public void DealAttackDamage()
    {
        if (isDead || player == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > attackRange)
        {
            return;
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > detectionRange)
        {
            return false;
        }

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > fieldOfViewAngle / 2f)
        {
            return false;
        }

        Vector3 eyePosition = transform.position + Vector3.up * 1.2f;
        Vector3 playerPosition = player.transform.position + Vector3.up * 1f;
        Vector3 rayDirection = playerPosition - eyePosition;

        if (Physics.Raycast(eyePosition, rayDirection.normalized, out RaycastHit hit, detectionRange))
        {
            FirstPersonController seenPlayer = hit.collider.GetComponentInParent<FirstPersonController>();

            if (seenPlayer != null)
            {
                return true;
            }
        }

        return false;
    }

    public void Die()
    {
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, 3f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;

        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftBoundary * detectionRange);
        Gizmos.DrawRay(transform.position, rightBoundary * detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

