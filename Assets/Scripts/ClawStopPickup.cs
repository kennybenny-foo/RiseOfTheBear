using UnityEngine;

public class ClawStopPickup : MonoBehaviour
{
    [SerializeField] float stopDuration = 8f;
    [SerializeField] float rotateSpeed = 90f;
    [SerializeField] float bobSpeed = 2f;
    [SerializeField] float bobHeight = 0.25f;

    [Header("Sounds")]
    [SerializeField] AudioClip pickupSound;

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            ClawHazard claw = FindFirstObjectByType<ClawHazard>();

            if (claw != null)
            {
                claw.StopClaw(stopDuration);
            }

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            Destroy(gameObject);
        }
    }
}
