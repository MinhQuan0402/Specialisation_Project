using UnityEngine;

public class OptionalSpriteMarker : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer => gameObject.GetComponent<SpriteRenderer>();
}