using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float healAmount = 25f;
    [SerializeField] float rotateSpeed = 90f;
    [SerializeField] float bobSpeed = 2f;
    [SerializeField] float bobHeight = 0.25f;

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
            playerHealth.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
