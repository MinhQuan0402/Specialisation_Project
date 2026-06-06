using UnityEngine;

public class SingletonTemplate<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance 
    {
        get 
        { 
            if (_instance == null)
            {
                GameObject obj = new(typeof(T).Name);
                _instance = obj.AddComponent<T>();
            }

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
