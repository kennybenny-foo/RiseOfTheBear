using StarterAssets;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] bool isLocked = true;

    public void SetLocked(bool locked)
    {
        isLocked = locked;

        if (isLocked)
        {
            Debug.Log("Prize chute exit locked.");
        }
        else
        {
            Debug.Log("Prize chute exit unlocked.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the exit trigger: " + other.gameObject.name);

        FirstPersonController player = other.GetComponentInParent<FirstPersonController>();

        if (player == null)
        {
            Debug.Log("It was not the player.");
            return;
        }

        if (isLocked)
        {
            Debug.Log("Exit is locked. Defeat all enemies first.");
            return;
        }

        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.WinGame();
        }
    }
}