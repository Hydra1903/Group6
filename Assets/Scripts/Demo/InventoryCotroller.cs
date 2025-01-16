using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; // Panel chứa các slot
    [SerializeField] private GameObject slotPrefab;     // Prefab của slot
    [SerializeField] private int slotCount;             // Số lượng slot trong inventory
    public List<Item> inventoryItems = new List<Item>();// Danh sách vật phẩm

    private Toolbar toolbarController;

    public static InventoryController instance;

    private void Start()
    {
        toolbarController = FindObjectOfType<Toolbar>();
        PopulateInventory();
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

                ItemDragHandler dragHandler = itemObject.AddComponent<ItemDragHandler>();
                // Gắn vật phẩm vào slot
                slot.currentItem = itemObject;
                slot.SetItem(item);
                //gắn code kéo thả cho vật phẩm 

                slot.UpdateQuantityText();
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
    //Hàm xóa item bằng công cụ
    public void RemoveItem(Item newItem, int amount)
    {
        Item existingItem = inventoryItems.Find(item => item.itemName == newItem.itemName);
        if (existingItem != null)
        {
            existingItem.quantity -= amount;
            if (existingItem.quantity <= 0)
            {
                inventoryItems.Remove(existingItem);  
            }
            UpdateInventoryUI();
        }
    }
    //Hàm xóa item bằng tên
    public void RemoveItemName(string itemName, int amount)
    {
        Item existingItem = inventoryItems.Find(item => item.itemName == itemName);
        if (existingItem != null)
        {
            existingItem.quantity -= amount;
            if (existingItem.quantity <= 0)
            {
                inventoryItems.Remove(existingItem);
            }
            UpdateInventoryUI();
        }
    }
    public bool HasItem(string itemName)
    {
        return inventoryItems.Exists(item => item.itemName == itemName);
    }

    // Cập nhật lại UI inventory khi thêm/xóa vật phẩm
    public void UpdateInventoryUI()
    {
        // Xóa các slot cũ
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Tạo lại inventory
        PopulateInventory();
    }

    public Item GetItemByName(string itemName) //tìm item từ inventory
    {
        return inventoryItems.Find(item => item.itemName == itemName);
    }
}
