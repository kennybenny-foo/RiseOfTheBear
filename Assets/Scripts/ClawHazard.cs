using StarterAssets;
using UnityEngine;
using System.Collections;

public class ClawHazard : MonoBehaviour
{
    FirstPersonController player;

    [Header("Tracking")]
    [SerializeField] float detectionRange = 35f;
    [SerializeField] float moveSpeed = 12f;

    [Header("Drop")]
    [SerializeField] float dropDistance = 28f;
    [SerializeField] float dropSpeed = 18f;
    [SerializeField] float liftSpeed = 14f;
    [SerializeField] float grabDistance = 2f;

    [Header("Attack")]
    [SerializeField] float damage = 25f;
    [SerializeField] float cooldown = 3f;
    [SerializeField] float warningTime = 0.75f;

    [Header("Grab Player")]
    [SerializeField] bool canGrabPlayer = true;
    [SerializeField] float grabHoldTime = 0.5f;
    [SerializeField] float carriedPlayerOffset = 3f;
    [SerializeField] float releaseDelay = 0.4f;

    [Header("Warning Indicator")]
    [SerializeField] GameObject warningIndicatorPrefab;
    [SerializeField] float warningIndicatorY = 3.35f;

    [Header("Sounds")]
    [SerializeField] AudioClip warningSound;
    [SerializeField] AudioClip dropSound;
    [SerializeField] AudioClip grabSound;
    [SerializeField] AudioClip releaseSound;
    [SerializeField] AudioClip liftSound;
    [SerializeField] AudioSource audioSource;

    GameObject activeWarningIndicator;

    [Header("Optional Claw Prongs")]
    [SerializeField] Transform leftProng;
    [SerializeField] Transform rightProng;
    [SerializeField] Transform middleProng;
    [SerializeField] float closeAngle = 25f;

    bool isWarning = false;
    bool isDropping = false;
    bool isLifting = false;
    bool isGrabbingPlayer = false;
    bool playerInSafeZone = false;

    float nextAttackTime = 0f;
    float topY;
    float bottomY;

    Vector3 startPosition;
    Vector3 attackPosition;

    Quaternion leftOpenRotation;
    Quaternion rightOpenRotation;
    Quaternion middleOpenRotation;

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();

        startPosition = transform.position;
        topY = transform.position.y;
        bottomY = topY - dropDistance;

        attackPosition = transform.position;

        if (leftProng != null)
        {
            leftOpenRotation = leftProng.localRotation;
        }

        if (rightProng != null)
        {
            rightOpenRotation = rightProng.localRotation;
        }

        if (middleProng != null)
        {
            middleOpenRotation = middleProng.localRotation;
        }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        if (playerInSafeZone)
        {
            ReturnToStart();
            return;
        }

        if (isGrabbingPlayer)
        {
            return;
        }

        float horizontalDistance = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(player.transform.position.x, 0f, player.transform.position.z)
        );

        if (horizontalDistance > detectionRange)
        {
            ReturnToStart();
            return;
        }

        if (!isWarning && !isDropping && !isLifting)
        {
            TrackPlayer();

            if (horizontalDistance <= grabDistance && Time.time >= nextAttackTime)
            {
                attackPosition = new Vector3(
                    player.transform.position.x,
                    topY,
                    player.transform.position.z
                );

                StartCoroutine(WarningThenDrop());
            }
        }

        if (isDropping)
        {
            DropClaw();
        }

        if (isLifting)
        {
            LiftClaw();
        }
    }

    void TrackPlayer()
    {
        Vector3 targetPosition = new Vector3(
            player.transform.position.x,
            topY,
            player.transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    IEnumerator WarningThenDrop()
    {
        isWarning = true;

        ShowWarningIndicator();
        PlaySound(warningSound);

        yield return new WaitForSeconds(warningTime);

        HideWarningIndicator();

        if (!playerInSafeZone)
        {
            isDropping = true;
            PlaySound(dropSound);
        }

        isWarning = false;
    }

    void ShowWarningIndicator()
    {
        if (warningIndicatorPrefab == null)
        {
            return;
        }

        Vector3 indicatorPosition = new Vector3(
            attackPosition.x,
            warningIndicatorY,
            attackPosition.z
        );

        activeWarningIndicator = Instantiate(
            warningIndicatorPrefab,
            indicatorPosition,
            Quaternion.identity
        );

        float indicatorSize = grabDistance * 2f;

        activeWarningIndicator.transform.localScale = new Vector3(
            indicatorSize,
            0.05f,
            indicatorSize
        );
    }

    void HideWarningIndicator()
    {
        if (activeWarningIndicator != null)
        {
            Destroy(activeWarningIndicator);
            activeWarningIndicator = null;
        }
    }

    void DropClaw()
    {
        Vector3 targetPosition = new Vector3(
            attackPosition.x,
            bottomY,
            attackPosition.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            dropSpeed * Time.deltaTime
        );

        if (Mathf.Abs(transform.position.y - bottomY) < 0.05f)
        {
            CloseClaw();

            if (canGrabPlayer && IsPlayerUnderClaw())
            {
                StartCoroutine(GrabPlayerRoutine());
            }
            else
            {
                TryDamagePlayer();

                isDropping = false;
                isLifting = true;

                PlaySound(liftSound);
            }
        }
    }

    IEnumerator GrabPlayerRoutine()
    {
        isDropping = false;
        isLifting = false;
        isGrabbingPlayer = true;

        TryDamagePlayer();
        PlaySound(grabSound);

        if (player != null)
        {
            player.enabled = false;
        }

        yield return new WaitForSeconds(grabHoldTime);

        Vector3 liftTarget = new Vector3(
            transform.position.x,
            topY,
            transform.position.z
        );

        PlaySound(liftSound);

        while (Vector3.Distance(transform.position, liftTarget) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                liftTarget,
                liftSpeed * Time.deltaTime
            );

            MovePlayerWithClaw();

            yield return null;
        }

        yield return new WaitForSeconds(releaseDelay);

        if (player != null)
        {
            player.enabled = true;
        }

        PlaySound(releaseSound);

        OpenClaw();

        isGrabbingPlayer = false;
        nextAttackTime = Time.time + cooldown;
    }

    void MovePlayerWithClaw()
    {
        if (player == null)
        {
            return;
        }

        Vector3 carriedPosition = new Vector3(
            transform.position.x,
            transform.position.y - carriedPlayerOffset,
            transform.position.z
        );

        player.transform.position = carriedPosition;
    }

    void LiftClaw()
    {
        Vector3 targetPosition = new Vector3(
            transform.position.x,
            topY,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            liftSpeed * Time.deltaTime
        );

        if (Mathf.Abs(transform.position.y - topY) < 0.05f)
        {
            OpenClaw();

            isLifting = false;
            nextAttackTime = Time.time + cooldown;
        }
    }

    bool IsPlayerUnderClaw()
    {
        float horizontalDistance = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(player.transform.position.x, 0f, player.transform.position.z)
        );

        return horizontalDistance <= grabDistance;
    }

    void TryDamagePlayer()
    {
        if (playerInSafeZone)
        {
            return;
        }

        if (!IsPlayerUnderClaw())
        {
            return;
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    void ReturnToStart()
    {
        if (isGrabbingPlayer)
        {
            return;
        }

        Vector3 targetPosition = new Vector3(
            startPosition.x,
            topY,
            startPosition.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    public void SetPlayerInSafeZone(bool value)
    {
        playerInSafeZone = value;

        if (playerInSafeZone)
        {
            HideWarningIndicator();

            isWarning = false;
            isDropping = false;
            isLifting = false;

            OpenClaw();

            nextAttackTime = Time.time + cooldown;
        }
    }

    void CloseClaw()
    {
        if (leftProng != null)
        {
            leftProng.localRotation = leftOpenRotation * Quaternion.Euler(0f, 0f, closeAngle);
        }

        if (rightProng != null)
        {
            rightProng.localRotation = rightOpenRotation * Quaternion.Euler(0f, 0f, -closeAngle);
        }

        if (middleProng != null)
        {
            middleProng.localRotation = middleOpenRotation * Quaternion.Euler(closeAngle, 0f, 0f);
        }
    }

    void OpenClaw()
    {
        if (leftProng != null)
        {
            leftProng.localRotation = leftOpenRotation;
        }

        if (rightProng != null)
        {
            rightProng.localRotation = rightOpenRotation;
        }

        if (middleProng != null)
        {
            middleProng.localRotation = middleOpenRotation;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grabDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * dropDistance);
    }
}