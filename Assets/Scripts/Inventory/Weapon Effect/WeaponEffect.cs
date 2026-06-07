using UnityEngine;

[CreateAssetMenu(fileName = "newWeaponEffect", menuName = "Data/Item Effect/Weapon Effect")]
public class WeaponEffect : ItemEffect
{
    [SerializeField] private GameObject weaponPrefab;

    public GameObject weapon { get; private set; } = null;

    public override void Unuse(GameObject user)
    {
        base.Unuse(user);

        var player = user.GetComponent<Player>();
        if (player.IsPrimaryAttackExist)
        {
            player.primaryAttackState.Init(null, CombatInputs.primary);
        }
        else if (player.IsSecondaryAttackExist)
        {
            player.secondaryAttackState.Init(null, CombatInputs.secondary);;
        }
        var equippedWeapon = weapon.GetComponent<Weapon>();
        if (equippedWeapon != null)
        {
            equippedWeapon.IsEquipped = false;
        }
    }

    public override bool Use(GameObject user)
    {
        var player = user.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player component not found on the user GameObject.");
            return false;
        }

        if (weaponPrefab == null)
        {
            Debug.LogError("Weapon prefab is not assigned.");
            return false;
        }

        if(weapon == null)
        {
            // Instantiate the weapon prefab and set it as the player's current weapon
            var weaponInstance = Instantiate(weaponPrefab, player.transform.position, Quaternion.identity);
            weaponInstance.transform.SetParent(player.transform); // Set the weapon as a child of the player
            weaponInstance.transform.localRotation = Quaternion.Euler(Vector3.zero); // Reset position to player
            weapon = weaponInstance;
        }

        var equippedWeapon = weapon.GetComponent<Weapon>();
        if (player.IsPrimaryAttackExist)
        {
            player.primaryAttackState.Init(equippedWeapon, CombatInputs.primary);
        }
        else if (player.IsSecondaryAttackExist)
        {
            player.secondaryAttackState.Init(equippedWeapon, CombatInputs.secondary);
        }
        equippedWeapon.IsEquipped = true;
        return true;
    }
}
