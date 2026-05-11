using System;
using System.Linq;
using UnityEngine;

public class WeaponSprite : WeaponComponent<WeaponSpriteData, AttackSprites>
{
    private SpriteRenderer baseSpriteRenderer;
    private SpriteRenderer weaponSpriteRenderer;

    private int currentWeaponSpriteIndex;
    
    private Sprite[] currentPhaseSprites;
    
    protected override void Start()
    {
        base.Start();

        baseSpriteRenderer = transform.Find("Base").GetComponent<SpriteRenderer>();
        weaponSpriteRenderer = transform.Find("WeaponSprite").GetComponent<SpriteRenderer>();
        baseSpriteRenderer.RegisterSpriteChangeCallback(HandleBaseSpriteChange);
        eventHandler.OnEnterAttackPhase += HandleEnterAttackPhase;
    }

    private void HandleEnterAttackPhase(AttackPhases phase)
    {
        currentWeaponSpriteIndex = 0;
        currentPhaseSprites = currentAttackData.PhaseSprites.FirstOrDefault(data => data.Phase == phase).Sprites;
    }

    private void HandleBaseSpriteChange(SpriteRenderer spriteRenderer)
    {
        if(!isAttackActive)
        {
            weaponSpriteRenderer.sprite = null;
            return;
        }
        
        if(currentWeaponSpriteIndex >= currentPhaseSprites.Length)
        {
            Debug.LogWarning($"{weapon.name} weapon sprites length mismatch");
        }

        weaponSpriteRenderer.sprite = currentPhaseSprites[currentWeaponSpriteIndex];
        currentWeaponSpriteIndex++;
    }

    protected override void HandleEnter()
    {
        base.HandleEnter();
        currentWeaponSpriteIndex = 0;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        baseSpriteRenderer.UnregisterSpriteChangeCallback(HandleBaseSpriteChange);
        weapon.OnEnter -= HandleEnter;
        eventHandler.OnEnterAttackPhase -= HandleEnterAttackPhase;
    }
}