using UnityEngine;

public class SingletonTemplate<T> : MonoBehaviour where T : Component
{
    private static T _instance = null;
    public static T Instance 
    {
        get 
        { 
            return _instance; 
        } 
    }

    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }

        _instance = this as T;
    }
}
