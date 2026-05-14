using System;
using StarterAssets;
using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] WeaponStats weaponStats;

    [Header("Ammo")]
    [SerializeField] int currentAmmo;

    [Header("Effects")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] Material bulletTrailMaterial;

    [Header("Camera Shake")]
    [SerializeField] CameraShake cameraShake;
    [SerializeField] float shakeDuration = 0.08f;
    [SerializeField] float shakeMagnitude = 0.03f;

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
        if (weaponStats == null)
        {
            Debug.LogError("WeaponStats is missing on " + gameObject.name);
            enabled = false;
            return;
        }

        currentAmmo = weaponStats.maxAmmo;
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

        if (cameraShake != null)
        {
            cameraShake.Shake(shakeDuration, shakeMagnitude);
        }

        Vector3 rayStart = mainCamera.transform.position;
        Vector3 rayDirection = mainCamera.transform.forward;

        Vector3 trailStart = bulletSpawnPoint != null ? bulletSpawnPoint.position : rayStart;
        Vector3 trailEnd = rayStart + rayDirection * weaponStats.range;

        RaycastHit hit;

        if (Physics.Raycast(rayStart, rayDirection, out hit, weaponStats.range))
        {
            trailEnd = hit.point;

            Health health = hit.collider.GetComponentInParent<Health>();

            if (health != null)
            {
                health.TakeDamage(weaponStats.damage);
                PlaySound(hitSound);
            }
        }

        StartCoroutine(CreateBulletTrail(trailStart, trailEnd));
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        currentAmmo = Mathf.Clamp(currentAmmo, 0, weaponStats.maxAmmo);

        Debug.Log("Ammo: " + currentAmmo + " / " + weaponStats.maxAmmo);

        NotifyAmmoChanged();
    }

    void NotifyAmmoChanged()
    {
        OnAmmoChanged?.Invoke(currentAmmo, weaponStats.maxAmmo, false);
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

        line.startWidth = weaponStats.trailWidth;
        line.endWidth = weaponStats.trailWidth;

        line.startColor = Color.yellow;
        line.endColor = Color.yellow;

        if (bulletTrailMaterial != null)
        {
            line.material = bulletTrailMaterial;
        }

        yield return new WaitForSeconds(weaponStats.trailDuration);

        Destroy(trailObject);
    }
}