using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class TeddyEnemy : MonoBehaviour
{
    FirstPersonController player;
    NavMeshAgent agent;
    Animator animator;

    [Header("Movement")]
    [SerializeField] float stoppingDistance = 2f;

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
        }
    }

    void Update()
    {
        if (player == null || agent == null)
        {
            return;
        }

        agent.SetDestination(player.transform.position);

        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }

    public void Die()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, 3f);
    }
}
