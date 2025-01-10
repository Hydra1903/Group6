using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int coins;
    public int energy;
    public int maxEnergy;

    // Lưu thông tin về các vật phẩm trong toolbar và inventory
    public List<ItemData> toolbarItems;
    public List<ItemData> inventoryItems;

    public PlayerData(PlayerStat playerStat, Toolbar toolbar, InventoryController inventory)
    {
        coins = playerStat.coins;
        energy = playerStat.energy;
        maxEnergy = playerStat.maxEnergy;

        toolbarItems = new List<ItemData>();
        foreach (Item item in toolbar.toolbarItems)
        {
            toolbarItems.Add(new ItemData(item));
        }

        inventoryItems = new List<ItemData>();
        foreach (Item item in inventory.inventoryItems)
        {
            inventoryItems.Add(new ItemData(item));
        }
    }
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public string iconPath;  // Lưu trữ đường dẫn của Sprite icon
    public int quantity;
    public int price;
    public int energy;
    public string description;
    public string toolType;  // Lưu trữ ToolType dưới dạng chuỗi
    public string itemType;  // Lưu trữ ItemType dưới dạng chuỗi
    public string prefabPath;  // Lưu trữ đường dẫn prefab (nếu có)

    public ItemData(Item item)
    {
        itemName = item.itemName;
        iconPath = item.icon != null ? item.icon.name : "";  // Lưu tên icon hoặc một chuỗi rỗng nếu icon không có
        quantity = item.quantity;
        price = item.price;
        energy = item.energy;
        description = item.description;
        toolType = item.toolType.ToString();  // Convert ToolType enum thành chuỗi
        itemType = item.itemType.ToString();  // Convert ItemType enum thành chuỗi
        prefabPath = item.toolPrefab != null ? item.toolPrefab.name : "";  // Lưu tên prefab hoặc chuỗi rỗng nếu không có
    }

    // Hàm để chuyển đổi lại từ đường dẫn icon (sprite)
    public Sprite GetIcon()
    {
        return Resources.Load<Sprite>(iconPath);  // Giả sử icon được lưu trong thư mục Resources
    }

    // Hàm để chuyển đổi lại từ prefab path
    public GameObject GetPrefab()
    {
        return Resources.Load<GameObject>(prefabPath);  // Giả sử prefab được lưu trong thư mục Resources
    }
}
