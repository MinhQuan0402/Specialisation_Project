using UnityEngine;

public class EntityData : ScriptableObject
{
    public float maxHealth;

    [Range(0f, 1f)] public float stunResist;
    [Range(0f, 1f)] public float knockbackResist;

    public enum OnZeroHealth
    {
        None,
        Destroy,
        Callback,
        Explode
    }

    public OnZeroHealth onZeroHealth;
    public GameObject[] ExplosionPrefabs;

    public static EntityData CreateInstance()
    {
        EntityData instance = CreateInstance<EntityData>();
        return instance;
    }
}
