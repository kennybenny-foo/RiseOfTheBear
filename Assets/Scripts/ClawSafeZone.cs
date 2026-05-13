using StarterAssets;
using UnityEngine;

public class ClawSafeZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        FirstPersonController player = other.GetComponentInParent<FirstPersonController>();

        if (player != null)
        {
            ClawHazard[] claws = FindObjectsByType<ClawHazard>(FindObjectsSortMode.None);

            foreach (ClawHazard claw in claws)
            {
                claw.SetPlayerInSafeZone(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        FirstPersonController player = other.GetComponentInParent<FirstPersonController>();

        if (player != null)
        {
            ClawHazard[] claws = FindObjectsByType<ClawHazard>(FindObjectsSortMode.None);

            foreach (ClawHazard claw in claws)
            {
                claw.SetPlayerInSafeZone(false);
            }
        }
    }
}
