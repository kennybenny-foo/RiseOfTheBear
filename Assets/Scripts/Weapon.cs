using System;
using StarterAssets;
using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] float damage = 25f;
    [SerializeField] float range = 100f;

    [Header("Ammo")]
    [SerializeField] int maxAmmo = 12;
    [SerializeField] int currentAmmo;

    [Header("Effects")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] Material bulletTrailMaterial;
    [SerializeField] float trailDuration = 0.02f;
    [SerializeField] float trailWidth = 0.02f;

    [Header("Sounds")]
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip outOfAmmoSound;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioSource audioSource;

    public event Action<int, int, bool> OnAmmoChanged;

    StarterAssetsInputs starterAssetsInputs;
    Camera mainCamera;

    void Awake()
    {
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
        mainCamera = Camera.main;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        currentAmmo = maxAmmo;
        NotifyAmmoChanged();
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.ShootInput(false);
            }

            return;
        }

        if (starterAssetsInputs == null)
        {
            return;
        }

        if (starterAssetsInputs.shoot)
        {
            if (currentAmmo > 0)
            {
                Shoot();
            }
            else
            {
                PlaySound(outOfAmmoSound);
                Debug.Log("Out of ammo! Find an ammo pickup.");
                NotifyAmmoChanged();
            }

            starterAssetsInputs.ShootInput(false);
        }
    }

    void Shoot()
    {
        currentAmmo--;
        NotifyAmmoChanged();

        PlaySound(shootSound);

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

            Health health = hit.collider.GetComponentInParent<Health>();

            if (health != null)
            {
                health.TakeDamage(damage);
                PlaySound(hitSound);
            }
        }
        else
        {
            trailEnd = rayStart + rayDirection * range;
        }

        StartCoroutine(CreateBulletTrail(trailStart, trailEnd));
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);

        Debug.Log("Ammo: " + currentAmmo + " / " + maxAmmo);

        NotifyAmmoChanged();
    }

    void NotifyAmmoChanged()
    {
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo, false);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
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