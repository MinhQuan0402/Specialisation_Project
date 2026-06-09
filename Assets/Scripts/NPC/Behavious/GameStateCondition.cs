using UnityEngine;
using System.Collections;

public class GameStateCondition : MonoBehaviour, INPCCondition
{
    [SerializeField] private GameState requiredState;
    public bool IsMet => GameManager.Instance.CurrentState == requiredState;
}