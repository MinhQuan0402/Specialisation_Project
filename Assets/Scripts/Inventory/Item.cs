using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemInstance item;

    [SerializeField] private float floatingSpeed = 0.1f;
    [SerializeField] private float floatingHeight = 0.5f;

    private Vector2 startPos;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = item.itemData.itemImage;
        startPos = transform.position;
    }

    void Update()
    {
        FloatingEffect();
    }

    public ItemInstance TakeItem()
    {
        Destroy(gameObject);
        return item;
    }

    private void FloatingEffect()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatingSpeed) * floatingHeight;
        transform.position = new Vector3(transform.position.x, newY);
    }
}
