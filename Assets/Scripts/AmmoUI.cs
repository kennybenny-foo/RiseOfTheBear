using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    [SerializeField] TMP_Text ammoText;

    void OnEnable()
    {
        if (weapon != null)
        {
            weapon.OnAmmoChanged += UpdateAmmoText;
        }
    }

    void OnDisable()
    {
        if (weapon != null)
        {
            weapon.OnAmmoChanged -= UpdateAmmoText;
        }
    }

    void UpdateAmmoText(int currentAmmo, int maxAmmo, bool isReloading)
    {
        if (isReloading)
        {
            ammoText.text = "Reloading...";
        }
        else
        {
            ammoText.text = "Ammo: " + currentAmmo + " / " + maxAmmo;
        }
    }
}
