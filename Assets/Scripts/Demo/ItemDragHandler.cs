using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Lưu parent ban đầu (slot hiện tại)
        originalParent = transform.parent;

        // Di chuyển item lên root để không bị ảnh hưởng bởi layout của slot
        transform.SetParent(transform.root);

        // Tắt raycast để tránh xung đột với các slot
        canvasGroup.blocksRaycasts = false;

        // Làm mờ item khi kéo
        canvasGroup.alpha = 0.6f;
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

        // Kiểm tra vị trí thả
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

        // Lấy slot gốc (nơi bắt đầu kéo)
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null && dropSlot != originalSlot)
        {
            // Nếu thả vào một slot khác và slot đó khác slot ban đầu

            // Gán item cho slot mới
            dropSlot.SetItem(originalSlot.item);

            // Xóa item trong slot ban đầu
            originalSlot.ClearSlot();

            // Đặt item vào slot mới
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;

            // Đặt vị trí của item ở trung tâm slot
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else
        {
            // Nếu không thả vào slot hợp lệ, trả về slot ban đầu
            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }
}
