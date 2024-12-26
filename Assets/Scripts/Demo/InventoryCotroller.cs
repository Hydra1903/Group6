using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    public int gold; // Số vàng của người chơi
    public GameObject inventoryPanel; // Panel chứa các slot
    public GameObject slotPrefab;     // Prefab của slot
    public int slotCount;             // Số lượng slot trong inventory
    public List<Item> inventoryItems = new List<Item>();// Danh sách vật phẩm
    private Toolbar toolbarController;

    private void Start()
    {
        toolbarController = FindObjectOfType<Toolbar>();
        PopulateInventory();
    }

    // Tạo danh sách inventory
    private void PopulateInventory()
    {
        for (int i = 0; i < slotCount; i++)
        {
            // Tạo slot từ prefab
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();

            // Nếu có vật phẩm, gắn vật phẩm vào slot
            if (i < inventoryItems.Count)
            {
                Item item = inventoryItems[i];

                // Tạo UI cho vật phẩm
                GameObject itemObject = new GameObject(item.itemName);
                itemObject.transform.SetParent(slot.transform);

                // Thêm RectTransform và Image
                RectTransform rectTransform = itemObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(80, 80);
                rectTransform.anchoredPosition = Vector2.zero;

                Image itemImage = itemObject.AddComponent<Image>();
                itemImage.sprite = item.icon;

                // Thêm CanvasGroup để quản lý hiển thị
                CanvasGroup itemCanvasGroup = itemObject.AddComponent<CanvasGroup>();
                itemCanvasGroup.alpha = 1;
                itemCanvasGroup.interactable = true;
                itemCanvasGroup.blocksRaycasts = true;

                // Gắn vật phẩm vào slot
                slot.currentItem = itemObject;

                //gắn code kéo thả cho vật phẩm 
                ItemDragHandler dragHandler = itemObject.AddComponent<ItemDragHandler>();
            }
        }
    }

    public void AddItemToInventory(Item newItem)
    {
        Item existingItem = inventoryItems.Find(item => item.itemName == newItem.itemName);
        if (existingItem != null)
        {
            existingItem.quantity += newItem.quantity;
        }
        else
        {
            inventoryItems.Add(newItem);
        }

        UpdateInventoryUI();
    }

    public void RemoveItem(Item newItem, int amount)
    {
        Item existingItem = inventoryItems.Find(item => item.itemName == newItem.itemName);
        if (existingItem != null)
        {
            if (inventoryItems != null)
            {
                existingItem.quantity -= amount;
                if (existingItem.quantity <= 0)
                {
                    inventoryItems.Remove(newItem);
                }
                UpdateInventoryUI();
            }

            UpdateInventoryUI();
        }
    }

    public bool HasItem(Item item)
    {
        return inventoryItems.Contains(item);
    }

    // Cập nhật lại UI inventory khi thêm/xóa vật phẩm
    private void UpdateInventoryUI()
    {
        // Xóa các slot cũ
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Tạo lại inventory
        PopulateInventory();
    }
}
