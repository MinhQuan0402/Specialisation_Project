using UnityEngine;


[CreateAssetMenu(fileName = "Entity Name" , menuName = "Entity/Create Entity")]
public class Entity : ScriptableObject
{
    [ReadOnlyInspector]public float health;
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
    [ReadOnlyInspector] public Core core;
    private void OnEnable()
    {
        health = maxHealth;
    }
    public void Init( float _maxHealth, float _stunResist, float _knockbackResist)
    {
        health = _maxHealth;
        maxHealth = _maxHealth;
        stunResist = _stunResist;
        knockbackResist = _knockbackResist;
    }
    public static Entity CreateInstance( float _maxHealth, float _stunResist, float _knockbackResist)
    {
        Entity instance = ScriptableObject.CreateInstance<Entity>();
        instance.Init(_maxHealth, _stunResist, _knockbackResist);
        return instance;
    }

}
