using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class InputEntry
{

}

public class InputSignalTrigger: MonoBehaviour
{
    private bool inputPressed = false;

    public void OnNotify(SignalAsset signal)
    {
        StartCoroutine(WaitingForInput());
    }

    IEnumerator WaitingForInput()
    {
        var director = GetComponent<PlayableDirector>();
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        Player.Instance.InputHandler.OnInteract += () => inputPressed = true;
        yield return new WaitUntil( () => inputPressed == true);
        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
        inputPressed = false;
    }
}