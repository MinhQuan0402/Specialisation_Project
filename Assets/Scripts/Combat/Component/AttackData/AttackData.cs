using UnityEngine;

public class AttackData
{
    [field: SerializeField, HideInInspector] public string Name {  get; private set; }

    public void SetAttackName(int i) => Name = $"Attack {i}";
}
