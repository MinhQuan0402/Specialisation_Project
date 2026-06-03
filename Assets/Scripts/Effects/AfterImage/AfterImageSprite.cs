using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AfterImageSprite : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 0.1f;

    [SerializeField]
    private float alphaSet = 0.8f;

    [SerializeField]
    private float alphaDecay = 0.85f;


    [SerializeField]
    private SpriteRenderer selfSR;


    private Transform owner;
    private SpriteRenderer ownerSR;

    private float timeActivated = 0;
    private float alpha = 1.0f;

    private Color color;

    public void OnCreate(GameObject owner)
    {
        selfSR = GetComponent<SpriteRenderer>();
        this.owner = owner.transform;
        ownerSR = owner.GetComponent<SpriteRenderer>();
        if (ownerSR ==  null) ownerSR = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (selfSR == null || ownerSR == null) return;

        alpha = alphaSet;
        selfSR.sprite = ownerSR.sprite;
        transform.SetPositionAndRotation(owner.position, owner.rotation);
        timeActivated = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (selfSR == null) return;

        alpha -= alphaDecay * Time.deltaTime;
        color = new Color(1f, 1f, 1f, alpha);
        selfSR.color = color;

        if (Time.time >= (timeActivated + activeTime))
        {
            AfterImagePool.GetInstance(owner.gameObject.GetEntityId()).AddToPool(this);
        }
    }
}
