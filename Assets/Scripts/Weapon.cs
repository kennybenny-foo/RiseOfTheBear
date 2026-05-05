using StarterAssets;
using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [SerializeField] float damage = 25f;
    [SerializeField] float range = 100f;

    [Header("Effects")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] Material bulletTrailMaterial;
    [SerializeField] float trailDuration = 0.02f;
    [SerializeField] float trailWidth = 0.02f;

    StarterAssetsInputs starterAssetsInputs;
    Camera mainCamera;

    void Awake()
    {
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (starterAssetsInputs.shoot)
        {
            Shoot();
            starterAssetsInputs.ShootInput(false);
        }
    }

    void Shoot()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (bulletSpawnPoint == null)
        {
            Debug.LogWarning("Bullet Spawn Point is not assigned!");
            return;
        }

        Vector3 rayStart = mainCamera.transform.position;
        Vector3 rayDirection = mainCamera.transform.forward;

        Vector3 trailStart = bulletSpawnPoint.position;
        Vector3 trailEnd;

        RaycastHit hit;

        if (Physics.Raycast(rayStart, rayDirection, out hit, range))
        {
            trailEnd = hit.point;

            Health health = hit.collider.GetComponent<Health>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
        else
        {
            trailEnd = rayStart + rayDirection * range;
        }

        StartCoroutine(CreateBulletTrail(trailStart, trailEnd));
    }

    IEnumerator CreateBulletTrail(Vector3 startPoint, Vector3 endPoint)
    {
        GameObject trailObject = new GameObject("Bullet Trail");

        LineRenderer line = trailObject.AddComponent<LineRenderer>();

        line.positionCount = 2;
        line.useWorldSpace = true;

        line.SetPosition(0, startPoint);
        line.SetPosition(1, endPoint);

        line.startWidth = trailWidth;
        line.endWidth = trailWidth;

        line.startColor = Color.yellow;
        line.endColor = Color.yellow;

        if (bulletTrailMaterial != null)
        {
            line.material = bulletTrailMaterial;
        }

        yield return new WaitForSeconds(trailDuration);

        Destroy(trailObject);
    }
}