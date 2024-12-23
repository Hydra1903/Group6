using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    public int gold; // Số vàng của người chơi
    public GameObject toolbarPanel; // Panel chứa các slot
    public GameObject slotPrefab;     // Prefab của slot
    public int slotCount;             // Số lượng slot trong inventory
    public List<Item> items;          // Danh sách vật phẩm

    private void Start()
    {
        PopulateInventory();
    }

    // Tạo danh sách inventory
    private void PopulateInventory()
    {
        for (int i = 0; i < slotCount; i++)
        {
            // Tạo slot từ prefab
            Slot slot = Instantiate(slotPrefab, toolbarPanel.transform).GetComponent<Slot>();

            // Nếu có vật phẩm, gắn vật phẩm vào slot
            if (i < items.Count)
            {
                Item item = items[i];

                // Tạo UI cho vật phẩm
                GameObject itemObject = new GameObject(item.itemName);
                itemObject.transform.SetParent(slot.transform);

                // Thêm RectTransform và Image
                RectTransform rectTransform = itemObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(50, 50);
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
            }
            else
            {
                // Nếu slot trống, tạo ô trống
                GameObject emptyObject = new GameObject("Empty");
                emptyObject.transform.SetParent(slot.transform);

                RectTransform rectTransform = emptyObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(50, 50);
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }

    public void AddItem(Item item)
    {
        items.Add(item);
        UpdateInventoryUI();
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        UpdateInventoryUI();
    }

    public bool HasItem(Item item)
    {
        return items.Contains(item);
    }

    // Cập nhật lại UI inventory khi thêm/xóa vật phẩm
    private void UpdateInventoryUI()
    {
        // Xóa các slot cũ
        foreach (Transform child in toolbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Tạo lại inventory
        PopulateInventory();
    }
}
