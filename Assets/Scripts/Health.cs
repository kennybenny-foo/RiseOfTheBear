using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;

    float currentHealth;
    bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        Debug.Log(gameObject.name + " health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.EnemyDefeated();
        }

        Robot robot = GetComponent<Robot>();

        if (robot != null)
        {
            robot.Die();
            return;
        }

        TeddyEnemy teddy = GetComponent<TeddyEnemy>();

        if (teddy != null)
        {
            teddy.Die();
            return;
        }

        Destroy(gameObject);
    }
}