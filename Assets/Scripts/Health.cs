using UnityEngine;
public class Health : MonoBehaviour
{
[SerializeField] float maxHealth = 100f;
float currentHealth;
void Start()
{
currentHealth = maxHealth;
}
public void TakeDamage(float amount)
{
currentHealth -= amount;
Debug.Log(gameObject.name + " health: " + currentHealth);
if (currentHealth <= 0)
{
Die();
}
}
void Die()
{
Destroy(gameObject);
}
}