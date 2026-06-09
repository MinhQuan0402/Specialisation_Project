using UnityEngine;

public class AlwaysCondition : MonoBehaviour, INPCCondition
{
    public bool IsMet => true;
}