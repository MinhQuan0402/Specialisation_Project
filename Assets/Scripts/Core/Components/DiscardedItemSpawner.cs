using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardedItemSpawner : CoreComponent
{
    [SerializeField] private Vector2 spawnDirection;
    [SerializeField] private float spawnVelocity;
    [SerializeField] private Item itemPrefab;
    [SerializeField] private Vector2 spawnOffset;

    private InventorySystem inventorySystem;
    private Movement movement;

    public void HandleItemDiscarded(ItemInstance itemInstance)
    {
        var spawnPoint = movement.FindRelativePoint(spawnOffset);
        var itemPickup = Instantiate(itemPrefab, spawnPoint, Quaternion.identity);
        itemPickup.gameObject.SetActive(true);
        itemPickup.transform.SetParent(null);
        itemPickup.SetItemInstance(itemInstance);

        var adjustedSpawnDirection = new Vector2(
            spawnDirection.x * movement.FacingDirection,
            spawnDirection.y
        );

        itemPickup.Rigidbody2D.linearVelocity = adjustedSpawnDirection.normalized * spawnVelocity;
    }

    protected override void Awake()
    {
        base.Awake();

        inventorySystem = core.GetCoreComponent<InventorySystem>();
        movement = core.GetCoreComponent<Movement>();
    }

    private void OnEnable()
    {
        if (inventorySystem == null) return;
        inventorySystem.OnWeaponDiscarded += HandleItemDiscarded;
    }

    private void OnDisable()
    {
        if (inventorySystem == null) return;
        inventorySystem.OnWeaponDiscarded -= HandleItemDiscarded;
    }
}