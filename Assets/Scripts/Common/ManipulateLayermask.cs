using UnityEngine;

public class ManipulateLayermask : MonoBehaviour
{
    [SerializeField] string newLayermask;
    [SerializeField] private GameObject[] targets;

    public void ChangeLayerMaker()
    {
        foreach(var go in targets)
        {
            go.layer = LayerMask.NameToLayer(newLayermask);
        }
    }
}
