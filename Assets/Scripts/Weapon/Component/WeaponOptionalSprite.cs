using UnityEngine;

public class WeaponOptionalSprite : WeaponComponent<WeaponOptionSpriteData, AttackOptionalSprite>
{
    private SpriteRenderer spriteRenderer;

    private void HandleSetOptionalSpriteActive(bool value)
    {
        spriteRenderer.enabled = value;
    }

    protected override void HandleEnter()
    {
        base.HandleEnter();
        if (!currentAttackData.UseOptionalSprite) return;
        spriteRenderer.sprite = currentAttackData.Sprite;
    }
    
    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<OptionalSpriteMarker>().SpriteRenderer;
        spriteRenderer.enabled = false;
        eventHandler.OnSetOptionalSpriteActive += HandleSetOptionalSpriteActive;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        eventHandler.OnSetOptionalSpriteActive -= HandleSetOptionalSpriteActive;
    }
}