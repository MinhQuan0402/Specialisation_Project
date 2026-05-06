using UnityEngine;

public class SingletonTemplate<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public T Instance {
        get { if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent<T>();
            }
        return _instance; } }
    private void OnDestroy()
    {
        if(_instance == this)
            _instance = null;
    }
}
