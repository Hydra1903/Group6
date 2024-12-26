using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManagerDemo : MonoBehaviour
{
    public Text goldText; // Hiển thị số vàng người chơi
    public GameObject itemPrefab; // Prefab hiển thị item trong shop
    public Transform contentPanel; // Panel Content trong Scroll View
    public List<Item> shopItems; // Danh sách các item trong shop
    public InventoryController InventoryController; // Tham chiếu đến inventory của người chơi
    public Toolbar toolbar; //tham chiếu đến toolbar người chơi
    public Button sellButton; // Nút Sell trong cửa hàng

    private Item selectedItem; // Vật phẩm hiện tại được chọn để bán

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

            itemUI.SetUp(item, BuyItem);
        }
    }

    // Hiển thị nút Sell

    // Xử lý khi người chơi chọn item trong inventory
    public void SelectItem(Item item)
    {
        selectedItem = item;
    }

    // Xử lý mua item
    void BuyItem(Item item)
    {
        if (InventoryController.gold >= item.price)
        {
            InventoryController.gold -= item.price;
            goldText.text = InventoryController.gold.ToString();
            toolbar.AddItemToToolbar(item, item.quantity);
            Debug.Log($"Mua thành công: {item.itemName}");
        }
        else
        {
            Debug.Log("Không đủ vàng để mua!");
        }
    }

    // Xử lý bán item


}

