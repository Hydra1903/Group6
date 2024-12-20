using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManagerDemo : MonoBehaviour
{
    public GameObject itemPrefab; // Prefab hiển thị item trong shop
    public Transform contentPanel; // Panel Content trong Scroll View
    public List<Item> shopItems; // Danh sách các item trong shop
    public InventoryController InventoryController; // Tham chiếu đến inventory của người chơi

    private void Start()
    {
        PopulateShop();
    }

    // Hiển thị danh sách item trong shop
    void PopulateShop()
    {
        foreach (Item item in shopItems)
        {
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            ShopItemUI itemUI = newItem.GetComponent<ShopItemUI>();

            itemUI.SetUp(item, BuyItem, SellItem);
        }
    }

    // Xử lý mua item
    void BuyItem(Item item)
    {
        if (InventoryController.gold >= item.price)
        {
            InventoryController.gold -= item.price;
            InventoryController.AddItem(item);
            Debug.Log($"Mua thành công: {item.itemName}");
        }
        else
        {
            Debug.Log("Không đủ vàng để mua!");
        }
    }

    // Xử lý bán item
    void SellItem(Item item)
    {
        if (InventoryController.HasItem(item))
        {
            InventoryController.RemoveItem(item);
            InventoryController.gold += item.price;
            Debug.Log($"Bán thành công: {item.itemName}");
        }
        else
        {
            Debug.Log("Bạn không có item này để bán!");
        }
    }
}
