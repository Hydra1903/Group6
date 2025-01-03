using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    public GameObject toolbarPanel; // Panel chứa các slot
    public GameObject slotPrefab;  // Prefab của slot
    public int slotCount;          // Số lượng slot trong inventory
    public List<Item> toolbarItems = new List<Item>(); // Danh sách vật phẩm trong toolbar

    private InventoryController inventoryController;

    public static Toolbar instance;

    private void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        PopulateToolbar();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại Canvas khi chuyển Scene
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Đảm bảo chỉ có một instance của InventoryUIManager
        }
    }
    // Tạo danh sách toolbar
    private void PopulateToolbar()
    {
        for (int i = 0; i < slotCount; i++)
        {
            // Tạo slot từ prefab
            Slot slot = Instantiate(slotPrefab, toolbarPanel.transform).GetComponent<Slot>();

            // Nếu có vật phẩm, gắn vật phẩm vào slot
            if (i < toolbarItems.Count)
            {
                Item item = toolbarItems[i];

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

                // Gắn script kéo thả
                ItemDragHandler dragHandler = itemObject.AddComponent<ItemDragHandler>();

                // Gắn vật phẩm vào slot
                slot.SetItem(item);
                slot.currentItem = itemObject;
            }
        }
    }

    // Thêm vật phẩm vào toolbar, nếu toolbar đầy thì chuyển sang inventory
    public void AddItemToToolbar(Item newItem, int amount)
    {
        Item existingItem = toolbarItems.Find(item => item.itemName == newItem.itemName);
        if (existingItem != null)
        {
            existingItem.quantity += amount;
        }
        else
        {
            if (toolbarItems.Count < slotCount)
            {
                toolbarItems.Add(newItem);
            }
            else
            {
                inventoryController.AddItemToInventory(newItem);
            }
        }

        UpdateToolbarUI();
    }
    //Hàm xóa item bằng công cụ được chọn
    public void RemoveItemFromToolbar(Item item, int amount)
    {
        Item existingItem = toolbarItems.Find(i => i.itemName == item.itemName);
        if (existingItem != null)
        {
            existingItem.quantity -= amount;
            if (existingItem.quantity <= 0)
            {
                toolbarItems.Remove(existingItem);
                UpdateToolbarUI() ;
            }
        }
    }
    //Hàm xóa item bằng tên
    public bool RemoveItemName(string itemName, int amount)
    {
        // Tìm trong toolbar trước
        Item toolbarItem = toolbarItems.Find(item => item.itemName == itemName);
        if (toolbarItem != null)
        {
            toolbarItem.quantity -= amount;
            if (toolbarItem.quantity <= 0)
            {
                toolbarItems.Remove(toolbarItem);
            }
            UpdateToolbarUI();
            return true; // Đã tìm thấy và trừ trong toolbar
        }

        // Nếu không có trong toolbar, tìm trong inventory
        if (inventoryController != null && inventoryController.HasItem(itemName))
        {
            inventoryController.RemoveItemName(itemName, amount);
            return true; // Đã tìm thấy và trừ trong inventory
        }

        return false; // Không tìm thấy trong cả toolbar và inventory
    }


    public bool HasItem(string itemName)
    {
        return toolbarItems.Exists(item => item.itemName == itemName);
    }

    // Cập nhật lại UI khi thêm/xóa vật phẩm
    private void UpdateToolbarUI()
    {
        foreach (Transform child in toolbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        PopulateToolbar();
    }
}
