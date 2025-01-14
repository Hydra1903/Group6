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
    public PlayerStat playerStat; // Tham chiếu đến phayerstat của người chơi
    public Toolbar toolbar; //tham chiếu đến toolbar người chơi
    public Button sellButton; // Nút Sell trong cửa hàng

    [SerializeField] private Item selectedItem; // Vật phẩm hiện tại được chọn để bán

    private void Start()
    {
        selectedItem = Player.instance.equippedTool;
        goldText.text = $"{playerStat.coins}";
        PopulateShop();
    }

    private void Update()
    {
        if (selectedItem != Player.instance.equippedTool)
        {
            selectedItem = Player.instance.equippedTool;
        }
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

    public void SellButton()
    {
        if (selectedItem == null)
        {
            Debug.Log("Không có item nào được chọn để bán!");
            return;
        }
        else 
        {
            Debug.Log("Đã bán item");
            SellItem(selectedItem);
        }
           
    }

    // Xử lý mua item
    void BuyItem(Item item)
    {
        if (playerStat.coins >= item.price)
        {
            playerStat.coins -= item.price;
            goldText.text = $"{playerStat.coins}";
            toolbar.AddItemToToolbar(item, 1);
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
        int sellPrice = item.price; // Giá bán bằng một nửa giá mua (có thể tùy chỉnh)
        playerStat.coins += sellPrice;
        goldText.text = $"{playerStat.coins}";

        // Xóa item khỏi toolbar
        toolbar.RemoveItemFromToolbar(item, 1);

        Debug.Log($"Đã bán {item.itemName} với giá {sellPrice} vàng.");

        // Xóa item được chọn sau khi bán
        selectedItem = null;
    }    

}

