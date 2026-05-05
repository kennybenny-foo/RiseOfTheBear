using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    FirstPersonController player;
    NavMeshAgent agent;

    [Header("Death Effect")]
    public ParticleSystem deathParticles;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();
    }

    void Update()
    {
        if (player != null && agent != null)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    public void Die()
    {
        Debug.Log("Robot Die() was called");

        if (deathParticles != null)
        {
            ParticleSystem particles = Instantiate(
                deathParticles,
                transform.position,
                Quaternion.identity
            );

            particles.Play();

            Destroy(
                particles.gameObject,
                particles.main.duration + particles.main.startLifetime.constantMax
            );
        }
        else
        {
            Debug.LogWarning("Death Particles is not assigned on the Robot!");
        }

        Destroy(gameObject);
    }
}