using UnityEngine;
public abstract class ItemEffect : ScriptableObject
{
    public abstract bool Use(GameObject user);
    public virtual void Unuse(GameObject user) { }
}
