using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private RectTransform rectTransform;
    private Slot sourceSlot;
    private Item draggedItem;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        sourceSlot = GetComponentInParent<Slot>();
        if (sourceSlot != null)
        {
            draggedItem = sourceSlot.item;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Lưu vị trí và parent ban đầu
        originalPosition = transform.position;
        originalParent = transform.parent;
        sourceSlot = GetComponentInParent<Slot>();
        draggedItem = sourceSlot.item;

        // Cài đặt để item có thể kéo qua các UI khác
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Đưa item lên trên cùng của canvas để tránh bị che khuất
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Cập nhật vị trí của item theo chuột
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Kiểm tra xem có thả vào slot khác không
        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        Slot targetSlot = dropTarget?.GetComponent<Slot>();

        if (targetSlot != null && targetSlot != sourceSlot)
        {
            HandleItemSwap(targetSlot);
        }
        else
        {
            // Trả item về vị trí cũ nếu không thả vào slot hợp lệ
            ReturnToOriginalPosition();
        }

        // Cập nhật lại selected slot nếu cần
        if (Slot.selectedSlot == sourceSlot)
        {
            sourceSlot.Select();
        }
    }

    private void HandleItemSwap(Slot targetSlot)
    {
        // Lưu thông tin item của slot đích
        GameObject targetItem = targetSlot.currentItem;
        Item targetItemData = targetSlot.item;

        // Cập nhật item cho slot đích
        targetSlot.currentItem = gameObject;
        targetSlot.SetItem(draggedItem);
        transform.SetParent(targetSlot.transform);
        transform.localPosition = Vector3.zero;

        // Cập nhật item cho slot nguồn
        if (targetItem != null)
        {
            sourceSlot.currentItem = targetItem;
            sourceSlot.SetItem(targetItemData);
            targetItem.transform.SetParent(sourceSlot.transform);
            targetItem.transform.localPosition = Vector3.zero;
        }
        else
        {
            sourceSlot.ClearSlot();
        }

        // Cập nhật UI của cả hai slot
        UpdateInventoryAfterSwap();    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
    }

    private void UpdateInventoryAfterSwap()
    {
        // Xác định slot thuộc inventory hay toolbar
        Transform inventoryPanel = GameObject.Find("InventoryPanel")?.transform;
        Transform toolbarPanel = GameObject.Find("ToolbarPanel")?.transform;

        if (originalParent.IsChildOf(inventoryPanel) && transform.parent.IsChildOf(toolbarPanel))
        {
            // Chuyển từ inventory sang toolbar
            InventoryController.instance.RemoveItem(draggedItem, draggedItem.quantity);
            Toolbar.instance.AddItemToToolbar(draggedItem, draggedItem.quantity);
        }
        else if (originalParent.IsChildOf(toolbarPanel) && transform.parent.IsChildOf(inventoryPanel))
        {
            // Chuyển từ toolbar sang inventory
            Toolbar.instance.RemoveItemFromToolbar(draggedItem, draggedItem.quantity);
            InventoryController.instance.AddItemToInventory(draggedItem);
        }
    }
}