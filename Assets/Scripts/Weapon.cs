using StarterAssets;
using UnityEngine;
public class Weapon : MonoBehaviour
{
[SerializeField] float damage = 25f;
StarterAssetsInputs starterAssetsInputs;
void Awake()
{
starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
}
void Update()
{
if (starterAssetsInputs.shoot)
{
RaycastHit hit;
if (Physics.Raycast(
Camera.main.transform.position,
Camera.main.transform.forward,
out hit,
Mathf.Infinity))
{
// Try to get a Health component on what was hit
Health health = hit.collider.GetComponent<Health>();
// Only call TakeDamage if the hit object has Health
if (health != null)
{
health.TakeDamage(damage);
}
}
starterAssetsInputs.ShootInput(false);
}
}
}
