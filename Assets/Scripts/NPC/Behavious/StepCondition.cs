using UnityEngine;
using System.Collections;

public class StepCondition: MonoBehaviour, INPCCondition
{
    [SerializeField] private StepData requiredStep;

    public bool IsMet
    {
        get
        {
            BaseSceneController sceneController = FindAnyObjectByType<BaseSceneController>();
            if (sceneController == null) return false;
            return sceneController.CurrentStep == requiredStep;
        }
    }
}