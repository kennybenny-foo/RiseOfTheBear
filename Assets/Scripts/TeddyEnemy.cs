using StarterAssets;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class TeddyEnemy : MonoBehaviour
{
    FirstPersonController player;
    NavMeshAgent agent;
    Animator animator;

    [Header("Detection")]
    [SerializeField] float detectionRange = 12f;
    [SerializeField] float fieldOfViewAngle = 120f;

    [Header("Patrol")]
    [SerializeField] Transform patrolPointA;
    [SerializeField] Transform patrolPointB;
    [SerializeField] float patrolSpeed = 2f;
    [SerializeField] float patrolWaitTime = 2f;
    [SerializeField] float patrolPointReachDistance = 0.6f;

    [Header("Movement")]
    [SerializeField] float stoppingDistance = 2f;
    [SerializeField] float chaseSpeed = 6f;
    [SerializeField] float turnSpeed = 8f;
    [SerializeField] float modelFacingOffsetY = 0f;

    [Header("Attack")]
    [SerializeField] float attackRange = 2.3f;
    [SerializeField] float attackCooldown = 1.2f;
    [SerializeField] float attackDamage = 10f;

    [Header("Sounds")]
    [SerializeField] AudioClip spotPlayerSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip punchHitSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioSource audioSource;

    bool isDead = false;
    bool hasSpottedPlayer = false;
    bool isWaitingAtPatrolPoint = false;

    float nextAttackTime = 0f;

    Vector3 patrolPositionA;
    Vector3 patrolPositionB;
    Vector3 currentPatrolTarget;

    bool goingToA = true;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();

        if (patrolPointA != null)
        {
            patrolPositionA = patrolPointA.position;
        }
        else
        {
            patrolPositionA = transform.position + transform.right * -4f;
        }

        if (patrolPointB != null)
        {
            patrolPositionB = patrolPointB.position;
        }
        else
        {
            patrolPositionB = transform.position + transform.right * 4f;
        }

        currentPatrolTarget = patrolPositionA;
        goingToA = true;

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updatePosition = true;

            agent.speed = patrolSpeed;
            agent.stoppingDistance = 0f;
            agent.isStopped = false;
            agent.ResetPath();
            agent.SetDestination(currentPatrolTarget);
        }

        Debug.Log(gameObject.name + " starting patrol target: " + currentPatrolTarget);
    }

    void Update()
    {
        if (isDead || player == null || agent == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer && !hasSpottedPlayer)
        {
            hasSpottedPlayer = true;
            PlaySound(spotPlayerSound);
        }

        if (!canSeePlayer)
        {
            hasSpottedPlayer = false;
        }

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
            Patrol();
        }
    }

    void Patrol()
    {
        if (isWaitingAtPatrolPoint)
        {
            return;
        }

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0f;
        agent.SetDestination(currentPatrolTarget);

        RotateTowardMovement();

        if (animator != null)
        {
            animator.SetFloat("Speed", 0.5f);
        }

        float distanceToPatrolPoint = Vector3.Distance(transform.position, currentPatrolTarget);

        if (distanceToPatrolPoint <= patrolPointReachDistance)
        {
            StartCoroutine(WaitThenSwitchPatrolPoint());
        }
    }

    IEnumerator WaitThenSwitchPatrolPoint()
    {
        isWaitingAtPatrolPoint = true;

        agent.isStopped = true;
        agent.ResetPath();

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }

        Debug.Log(gameObject.name + " reached patrol point. Waiting.");

        yield return new WaitForSeconds(patrolWaitTime);

        goingToA = !goingToA;
        currentPatrolTarget = goingToA ? patrolPositionA : patrolPositionB;

        Debug.Log(gameObject.name + " new patrol target: " + currentPatrolTarget);

        FaceTargetInstant(currentPatrolTarget);

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0f;
        agent.ResetPath();
        agent.SetDestination(currentPatrolTarget);

        if (animator != null)
        {
            animator.SetFloat("Speed", 0.5f);
        }

        isWaitingAtPatrolPoint = false;
    }

    void ChasePlayer()
    {
        isWaitingAtPatrolPoint = false;

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(player.transform.position);

        RotateTowardMovement();

        if (animator != null)
        {
            animator.SetFloat("Speed", 1f);
        }
    }

    void Attack()
    {
        isWaitingAtPatrolPoint = false;

        agent.isStopped = true;
        agent.ResetPath();

        FaceTargetInstant(player.transform.position);

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

            PlaySound(attackSound);

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void RotateTowardMovement()
    {
        Vector3 direction = agent.desiredVelocity;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = currentPatrolTarget - transform.position;
            direction.y = 0f;
        }

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, modelFacingOffsetY, 0f);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }
    }

    void FaceTargetInstant(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, modelFacingOffsetY, 0f);
        }
    }

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
            PlaySound(punchHitSound);
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

        PlaySound(deathSound);

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, 3f);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
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

        if (patrolPointA != null && patrolPointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(patrolPointA.position, 0.25f);
            Gizmos.DrawSphere(patrolPointB.position, 0.25f);
            Gizmos.DrawLine(patrolPointA.position, patrolPointB.position);
        }
    }
}