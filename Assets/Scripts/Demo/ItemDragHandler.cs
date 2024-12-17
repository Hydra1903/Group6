using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Lưu parent ban đầu
        originalParent = transform.parent;

        // Di chuyển item lên root để không bị ảnh hưởng bởi layout của slot
        transform.SetParent(transform.root);

        // Tắt raycast để tránh xung đột với các slot
        canvasGroup.blocksRaycasts = false;

        // Làm mờ item khi kéo
        canvasGroup.alpha = 0.6f;

        // Tắt highlight của slot ban đầu
        Slot originalSlot = originalParent.GetComponent<Slot>();
        if (originalSlot != null)
        {
            originalSlot.Deselect();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Cập nhật vị trí item theo con trỏ
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Bật lại raycast
        canvasGroup.blocksRaycasts = true;

        // Khôi phục độ trong suốt
        canvasGroup.alpha = 1f;

        // Kiểm tra vị trí thả (pointerEnter)
        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();
        if (dropSlot == null)
        {
            // Nếu pointer không trỏ đến Slot, kiểm tra parent của pointer
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }

        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null)
        {
            // Nếu thả vào một Slot
            if (dropSlot.currentItem != null)
            {
                // Đổi chỗ item nếu slot đích đã có item
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                // Nếu slot đích trống, clear slot ban đầu
                originalSlot.currentItem = null;
            }

            // Đặt item vào slot mới
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;

            // Bật highlight cho slot mới
            dropSlot.Select();
        }
        else
        {
            // Nếu không thả vào slot hợp lệ, trả về slot ban đầu
            transform.SetParent(originalParent);
            originalSlot.Select(); // Bật lại highlight của slot ban đầu
        }

        // Đặt vị trí của item ở trung tâm slot
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
