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
    public void OnNotify(SignalAsset signal)
    {
        StartCoroutine(WaitingForInput());
    }

    IEnumerator WaitingForInput()
    {
        var director = GetComponent<PlayableDirector>();
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        yield return new WaitUntil( () => (Player.Instance.InputHandler.InteractionInput == true));
        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }
}