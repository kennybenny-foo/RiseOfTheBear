using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;

    float currentHealth;
    bool isDead = false;

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDied;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Heal(float amount)
    {
    if (isDead)
    {
        return;
    }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Player healed. Health: " + currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Player health: " + currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player died");

        OnPlayerDied?.Invoke();

        // Simple lose behavior for now
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}