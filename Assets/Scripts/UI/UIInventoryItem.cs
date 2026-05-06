using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using System;
using UnityEngine.EventSystems;
using TMPro;

public class UIInventoryItem : MonoBehaviour
{
    public Image itemImage;

    public ItemInstance ItemInstance { get; set; }

    [SerializeField] private Image borderImage;

    public event Action<UIInventoryItem> OnItemClick,
        OnItemDropOn, OnItemBeginDrag, OnItemEndDrag,
        OnRightMouseBtnClicked;

    private bool isEmpty = true;

    public void Awake()
    {
        ResetData();
        Deselect();
    }

    public void ResetData()
    {
        this.itemImage.gameObject.SetActive(false);
        this.isEmpty = true;
    }

    public void Deselect()
    {
        this.borderImage.enabled = false;
    }

    public void SetData(Sprite sprite)
    {
        if (sprite == null)
        {
            ResetData();
            return;
        }
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        this.isEmpty = false;
    }

    public void Select()
    {
        borderImage.enabled = true;
    }

    public void OnBeginDrag()
    {
        if (isEmpty) return;
        OnItemBeginDrag?.Invoke(this);
    }

    public void OnDrop()
    {
        if (isEmpty) return;
        OnItemDropOn?.Invoke(this);
    }

    public void OnEndDrag()
    {
        if (isEmpty) return;
        OnItemEndDrag?.Invoke(this);
    }

    public void OnPointerClick(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            OnItemClick?.Invoke(this);
        }
        else if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseBtnClicked?.Invoke(this);
        }
    }
}
