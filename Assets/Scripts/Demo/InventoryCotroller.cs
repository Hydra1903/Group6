using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public int gold; // Số vàng của người chơi
    public GameObject inventoryPanel; // Panel chứa các slot
    public GameObject slotPrefab;     // Prefab của slot
    public int slotCount;             // Số lượng slot trong inventory
    public List<Item> items;          // Danh sách vật phẩm

    private void Start()
    {
        // Tạo các slot và gắn item vào các slot
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();

            if (i < items.Count)
            {
                Item item = items[i]; // Lấy vật phẩm từ danh sách

                // Tạo GameObject cho vật phẩm và gắn vào slot
                GameObject itemObject = new GameObject(item.itemName); // Tạo GameObject cho item
                itemObject.transform.SetParent(slot.transform);

                // Đảm bảo itemObject có RectTransform để hiển thị trong UI
                RectTransform rectTransform = itemObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(50, 50); // Kích thước vật phẩm

                // Thêm Image để hiển thị icon của vật phẩm
                Image itemImage = itemObject.AddComponent<Image>();
                itemImage.sprite = item.icon;

                // Thêm CanvasGroup để quản lý các thuộc tính UI của item
                CanvasGroup itemCanvasGroup = itemObject.AddComponent<CanvasGroup>();

                // Cập nhật các thuộc tính CanvasGroup tùy theo điều kiện (ví dụ: độ mờ, tương tác)
                itemCanvasGroup.alpha = 1;           // Đặt alpha của item là 1 (hiện)
                itemCanvasGroup.interactable = true; // Cho phép tương tác
                itemCanvasGroup.blocksRaycasts = true; // Chặn raycast (cho phép tương tác)

                // Căn chỉnh item trong slot
                rectTransform.anchoredPosition = Vector2.zero;

                // Thêm script kéo thả cho item
                itemObject.AddComponent<ItemDragHandler>();

                // Gán item cho slot
                slot.currentItem = itemObject;
            }
            else
            {
                // Nếu không có item, tạo một ô trống
                GameObject emptyItemObject = new GameObject("Empty");
                emptyItemObject.transform.SetParent(slot.transform);
                RectTransform emptyRect = emptyItemObject.AddComponent<RectTransform>();
                emptyRect.sizeDelta = new Vector2(50, 50);
                emptyRect.anchoredPosition = Vector2.zero;
            }
        }
    }

    public void AddItem(Item item)
    {
        items.Add(item);
        UpdateInventoryUI(); // Cập nhật lại UI khi thêm item
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        UpdateInventoryUI(); // Cập nhật lại UI khi xóa item
    }

    public bool HasItem(Item item)
    {
        return items.Contains(item);
    }

    private void UpdateInventoryUI()
    {
        // Clear the current inventory panel
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Create new slots and items
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();

            // If there is an item, add it to the slot
            if (i < items.Count)
            {
                Item item = items[i];
                GameObject itemObject = new GameObject(item.itemName);
                itemObject.transform.SetParent(slot.transform);

                RectTransform rectTransform = itemObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(50, 50);
                Image itemImage = itemObject.AddComponent<Image>();
                itemImage.sprite = item.icon;
                rectTransform.anchoredPosition = Vector2.zero;

                // Add CanvasGroup to item for handling visibility and interactions
                CanvasGroup itemCanvasGroup = itemObject.AddComponent<CanvasGroup>();
                itemCanvasGroup.alpha = 1;  // Full opacity
                itemCanvasGroup.interactable = true;  // Enable interaction
                itemCanvasGroup.blocksRaycasts = true;  // Enable raycasting

                itemObject.AddComponent<ItemDragHandler>();
                slot.currentItem = itemObject;
            }
            else
            {
                GameObject emptyItemObject = new GameObject("Empty");
                emptyItemObject.transform.SetParent(slot.transform);
                RectTransform emptyRect = emptyItemObject.AddComponent<RectTransform>();
                emptyRect.sizeDelta = new Vector2(50, 50);
                emptyRect.anchoredPosition = Vector2.zero;
            }
        }
    }
}
